using System;
using System.Linq;
using System.Collections.Generic;

using Harmony;
using MSCLoader;
using UnityEngine;
using System.Xml.Linq;

namespace Adrenaline
{
    public class Adrenaline : Mod
    {
        public override string ID => "com.adrenaline.mod";
        public override string Name => "Adrenaline";
        public override string Author => "LUAR, Andrinall, @racer";
        public override string Version => "0.5.4";
        public override string Description => "Adds a character's need for adrenaline";
        public override bool UseAssetsFolder => false;
        public override bool SecondPass => true;

        private string LastAddedComponent = "";

#if DEBUG
        private SettingsCheckBox lockbox;
        private SettingsSliderInt priceSlider;
        private List<SettingsSlider> _sliders = new List<SettingsSlider>();
        private List<string> _highValues =
            new List<string> { "JANNI_PETTERI_HIT", "VENTTI_WIN", "PISS_ON_DEVICES", "SPARK_WIRING", "COFFEE_INCREASE", "PUB_PRICE" };

        public override void ModSettings()
        {
            if (SaveLoad.ValueExists(this, "DebugAdrenaline"))
            {
                var temp = SaveLoad.ReadValueAsDictionary<string, float>(this, "DebugAdrenaline");
                foreach (var item in temp) // crutch
                    AdrenalineLogic.config[item.Key] = item.Value;
            }
            else
            {
                Utils.PrintDebug(eConsoleColors.RED, "DEBUG Settings not loaded, remove and resetting to default");
                SaveLoad.DeleteValue(this, "DebugAdrenaline");
            }

            //Settings.AddHeader(this, "Debug ", true);
            _sliders.Add(Settings.AddSlider(this, "adn_Value", "Текущее значение", 5f, 195f, AdrenalineLogic.Value, AdrenalineChanged));

            var MIN_LOSS_RATE = AdrenalineLogic.config.GetValueSafe("MIN_LOSS_RATE");
            var MAX_LOSS_RATE = AdrenalineLogic.config.GetValueSafe("MAX_LOSS_RATE");
            _sliders.Add(Settings.AddSlider(this, "adn_loss", "Скорость пассивного уменьшения", MIN_LOSS_RATE, MAX_LOSS_RATE, AdrenalineLogic.LossRate, LossRateChanged));

            var PUB_COFFEE_PRICE = AdrenalineLogic.config.GetValueSafe("PUB_COFFEE_PRICE");
            priceSlider = Settings.AddSlider(this, "adn_price", "Стоимость энергетика в пабе", 0, 500, (int)PUB_COFFEE_PRICE, PubPriceChanged);

            lockbox = Settings.AddCheckBox(this, "adn_lock", "Заблокировать уменьшение адреналина", false, delegate {
                var lock_ = lockbox.GetValue();
                AdrenalineLogic.SetDecreaseLocked(lock_, 2_000_000_000f, lock_);
            });

            foreach (var item in AdrenalineLogic.config)
            {
                if (item.Key.Contains("LOSS_RATE")) continue;
                if (item.Key == "PUB_COFFEE_PRICE") continue;
                AddSlider(item);
            }

            /*Settings.AddHeader(this, "Значения, для всего времени действия", true);
            for (var i = 1; i < 11; i++)
            {
                var element = AdrenalineLogic.config.ElementAtOrDefault(i);
                if (element.Key.Contains("LOSS_RATE")) continue;
                AddSlider(element);
            }

            Settings.AddHeader(this, "Значения, для +/- 1 раз за действие", true);
            for (var i = 11; i < 26; i++)
            {
                var element = AdrenalineLogic.config.ElementAtOrDefault(i);
                if (element.Key == "PUB_COFFEE_PRICE") continue;
                AddSlider(element);
            }

            Settings.AddHeader(this, "Минимальные значения", true);
            for (var i = 26; i < 34; i++)
                AddSlider(AdrenalineLogic.config.ElementAtOrDefault(i));*/
        }

        private void AddSlider(KeyValuePair<string, float> element)
        {
            _sliders.Add(Settings.AddSlider(
                mod: this,
                settingID: element.Key.GetHashCode().ToString(),
                name: Globals.localization.GetValueSafe(element.Key),
                minValue: 0,
                maxValue: 250,
                value: element.Value,
                onValueChanged: new VariableChanger(element.Key, ref _sliders).ValueChanged
            ));
        }

        private void AdrenalineChanged()
        {
            AdrenalineLogic.Value = _sliders[0].GetValue();
        }

        private void LossRateChanged()
        {
            AdrenalineLogic.LossRate = _sliders[1].GetValue();
        }

        private void PubPriceChanged()
        {
            GameObject.Find("STORE")
                ?.GetComponent<CustomEnergyDrink>()
                ?.SetDrinkPrice((float)priceSlider.GetValue());
        }

        public override void FixedUpdate()
        {
            _sliders[0].Instance.Value = AdrenalineLogic.Value;

            var slider1 = _sliders[1].Instance;
            slider1.Value = AdrenalineLogic.LossRate;
            slider1.Vals[0] = AdrenalineLogic.config["MIN_LOSS_RATE"];
            slider1.Vals[1] = AdrenalineLogic.config["MAX_LOSS_RATE"];
            lockbox.SetValue(AdrenalineLogic.IsDecreaseLocked());
        }
#endif

        public override void OnNewGame()
        {
            AdrenalineLogic.isDead = false;
            AdrenalineLogic.Value = 100f;
            AdrenalineLogic.LastDayUpdated = 1;
            AdrenalineLogic.UpdateLossRatePerDay();

            if (SaveLoad.ValueExists(this, "Adrenaline"))
                SaveLoad.DeleteValue(this, "Adrenaline");
        }

        public override void OnLoad()
        {
#if DEBUG
            ConsoleCommand.Add(new TP_COMMAND());
#endif
            AdrenalineLogic.isDead = false;
            AdrenalineLogic.Value = 100f;
            
            var bundle = LoadAssets.LoadBundle("Adrenaline.Assets.energy.unity3d");
            Globals.can_texture = Globals.LoadAsset<Texture>(bundle, "assets/textures/Energy.png");
            Globals.atlas_texture = Globals.LoadAsset<Texture>(bundle, "assets/textures/ATLAS_OFFICE.png");
            Globals.coffee_cup = Globals.LoadAsset<Mesh>(bundle, "assets/meshes/coffee_cup_bar_coffee.mesh.obj");
            Globals.empty_cup = Globals.LoadAsset<Mesh>(bundle, "assets/meshes/coffee_cup_bar.mesh.obj");
            Globals.pills = Globals.LoadAsset<GameObject>(bundle, "assets/prefabs/Pills.prefab");
            Globals.poster = Globals.LoadAsset<GameObject>(bundle, "assets/prefabs/Poster.prefab");
            Globals.background = Globals.LoadAsset<GameObject>(bundle, "assets/prefabs/Background.prefab");

            Globals.LoadAllPosters(bundle);
            Globals.LoadAllScreens(bundle);
            Globals.LoadAllSounds(bundle);
            bundle.Unload(false);

            if (SaveLoad.ValueExists(this, "Adrenaline"))
            {
                var data = SaveLoad.ReadValueAsDictionary<string, float>(this, "Adrenaline");
                var time = data.GetValueSafe("LossRateLockTime");
                var value = data.GetValueSafe("Value");
                AdrenalineLogic.Value = (value <= AdrenalineLogic.MIN_ADRENALINE + 20f) ? 30f : value;
                AdrenalineLogic.LastDayUpdated = Mathf.RoundToInt(data.GetValueSafe("LastDayUpdated"));
                AdrenalineLogic.LossRate = data.GetValueSafe("LossRate");
                AdrenalineLogic.SetDecreaseLocked(time > 0, time);

                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
            }
            else
                ModConsole.Print("<color=red>Unable to load Save Data, resetting to default</color>");
            
            AddComponent<GlobalHandler>("PLAYER");
            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        public override void SecondPassOnLoad()
        {
            AddComponent<PissOnDevicesHandler>("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger");
            AddComponent<AmiksetHandler>("NPC_CARS/Amikset");
            AddComponent<StoreActionsHandler>("STORE");
            AddComponent<CustomEnergyDrink>("STORE");
            AddComponent<VenttiGameHandler>("CABIN/Cabin/Ventti/Table/GameManager");
            AddComponent<DanceHallHandler>("DANCEHALL/Functions");

            AddComponent<SpillHandler>("GIFU(750/450psi)/ShitTank");
            AddComponent<CarElectricityHandler>("SATSUMA(557kg, 248)/Wiring");
            AddComponent<FerndaleSeatbeltFix>("FERNDALE(1630kg)/LOD/Seatbelts/BuckleUp");
            AddComponent<MailBoxEnvelope>("YARD/PlayerMailBox");

            Utils.CreatePoster(0, new Vector3(-18.982f, 1.123788f, 4.68906f), Quaternion.Euler(90f, 90f, 0f));
            Utils.CreatePoster(0, new Vector3(1553.545f, 6.4f, 733.9846f), Quaternion.Euler(90f, 245.15f, 0f));
            Utils.CreatePoster(1, new Vector3(-1545.73f, 5.45f, 1184.253f), Quaternion.Euler(90f, 57.42f, 0f));

            var CARS = new List<string> {
                "JONNEZ ES(Clone)", "SATSUMA(557kg, 248)", "FERNDALE(1630kg)",
                "HAYOSIKO(1500kg, 250)", "GIFU(750/450psi)" };

            foreach (var item in CARS)
            {
                try
                {
                    GameObject obj = GameObject.Find(item);
                    if (obj == null)
                        throw new MissingComponentException("Car with object name \"" + item + "\" doesn't exists");

                    obj.AddComponent<HighSpeedHandler>();
                    obj.AddComponent<WindshieldHandler>();
                }
                catch
                {
                    Utils.PrintDebug(eConsoleColors.RED, "HighSpeedHandler loading error for {0}", item);
                }
            }

            var dynamics = Resources.FindObjectsOfTypeAll<CarDynamics>().Where(v => v.transform.parent == null);
            foreach (var item in dynamics)
                item.gameObject.AddComponent<CrashHandler>();

            var humans = Resources.FindObjectsOfTypeAll<GameObject>().Where(v => v.name == "HumanTriggerCrime").ToArray();
            Utils.PrintDebug("Humans count: " + humans.Length.ToString());
            foreach (var item in humans)
                item.AddComponent<DriveByHandler>();
        }

        private T AddComponent<T>(string obj) where T : Component
        {
            LastAddedComponent = string.Format("{0}::{1}", obj, typeof(T)?.Name.ToString());
            Utils.PrintDebug(eConsoleColors.YELLOW, "Loading component " + LastAddedComponent);
            return GameObject.Find(obj)?.AddComponent<T>() ?? null;
        }

        public override void OnSave()
        {
#if DEBUG
            try
            {
                if (SaveLoad.ValueExists(this, "DebugAdrenaline")) SaveLoad.DeleteValue(this, "DebugAdrenaline");
                SaveLoad.WriteValue(this, "DebugAdrenaline", AdrenalineLogic.config);
            }
            catch
            {
                throw new UnassignedReferenceException("Unable to save DEBUG settings!");
            }
#endif
            try
            {
                SaveLoad.WriteValue(this, "Adrenaline", new Dictionary<string, float>
                {
                    ["Value"] = AdrenalineLogic.Value,
                    ["LossRate"] = AdrenalineLogic.LossRate,
                    ["LossRateLockTime"] = AdrenalineLogic.GetDecreaseLockTime(),
                    ["LastDayUpdated"] = AdrenalineLogic.LastDayUpdated
                });
            }
            catch
            {
                throw new UnassignedReferenceException("Unable to save MAIN settings!");
            }
        }
    }

#if DEBUG
    internal class TP_COMMAND : ConsoleCommand
    {
        public override string Name => "atp";
        public override string Help => "Телепортирует игрока к позиции таблеток";

        public override void Run(string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                ShowHelpInfo();
                return;
            }

            if (args[0] == "help")
            {
                ShowHelpInfo();
                return;
            }


            if (!Int32.TryParse(args[0], out var index))
            {
                ShowHelpInfo();
                return;
            }

            if (index <= 0 || index > Globals.pills_positions.Count)
            {
                Utils.PrintDebug(eConsoleColors.RED, "Указан неверный индекс!");
                ShowHelpInfo();
                return;
            }

            Globals.pills_list.ForEach(v => UnityEngine.Object.Destroy(v.self));
            Globals.pills_list.Clear();

            var item = new PillsItem(index - 1, Globals.pills_positions.ElementAt(index - 1));
            Globals.pills_list.Add(item);

            var targetPosition = item.self.transform.position;
            targetPosition.y -= 0.05f;
            GameObject.Find("PLAYER").transform.position = targetPosition;
            Utils.PrintDebug("Вы успешно телепортированы к ID:{0}", index);
        }

        private void ShowHelpInfo()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, "====== Adrenaline TP ======");
            Utils.PrintDebug(eConsoleColors.YELLOW, "Пример использования: atp 1");
            Utils.PrintDebug(eConsoleColors.YELLOW, "Минимум: 1, Максимум: {0}", Globals.pills_positions.Count);
            Utils.PrintDebug(eConsoleColors.YELLOW, "===========================");
        }
    }
#endif
}