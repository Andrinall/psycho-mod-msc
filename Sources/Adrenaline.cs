using System.Linq;
using System.Collections.Generic;

using Harmony;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

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

        private Dictionary<string, float> savedata = new Dictionary<string, float>();

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

            _sliders.Add(Settings.AddSlider(this, "adn_Value", "Текущее значение", 5f, 195f, AdrenalineLogic.Value, AdrenalineChanged));
            _sliders.Add(Settings.AddSlider(this, "adn_loss", "Скорость пассивного уменьшения", 0f, 8f, AdrenalineLogic.LossRate, LossRateChanged));

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
            _sliders[1].Instance.Value = AdrenalineLogic.LossRate;
            lockbox.SetValue(AdrenalineLogic.IsDecreaseLocked());
        }
#endif

        public override void OnNewGame()
        {
            SetDefaultValuesForLogic();

            if (SaveLoad.ValueExists(this, "Adrenaline"))
                SaveLoad.DeleteValue(this, "Adrenaline");
        }

        private void SetDefaultValuesForLogic()
        {
            AdrenalineLogic.isDead = false;
            AdrenalineLogic.Value = 100f;
            AdrenalineLogic.LastDayUpdated = Utils.GetGlobalVariable<FsmInt>("GlobalDay").Value;
            AdrenalineLogic.UpdateLossRatePerDay(AdrenalineLogic.LastDayUpdated);
            AdrenalineLogic.SetDecreaseLocked(false, 0);
        }

        public override void OnLoad()
        {
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
                savedata = SaveLoad.ReadValueAsDictionary<string, float>(this, "Adrenaline");
                if (savedata == null) return;

                var time = savedata.GetValueSafe("LossRateLockTime");
                var value = savedata.GetValueSafe("Value");
                AdrenalineLogic.Value = (value <= 20f) ? 30f : value;
                AdrenalineLogic.LastDayUpdated = Mathf.RoundToInt(savedata.GetValueSafe("LastDayUpdated"));
                AdrenalineLogic.LossRate = savedata.GetValueSafe("LossRate");
                AdrenalineLogic.SetDecreaseLocked(time > 50f, time);
                if (AdrenalineLogic.isDead)
                {
                    AdrenalineLogic.isDead = false;
                    if (AdrenalineLogic.LossRate >= 3.5f)
                        AdrenalineLogic.UpdateLossRatePerDay();
                }

                Utils.PrintDebug("Value:{0}; time:{1}, day:{2}, loss:{3}", value, time, AdrenalineLogic.LastDayUpdated, AdrenalineLogic.LossRate);
                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
            }
            else
            {
                ModConsole.Print("<color=red>Unable to load Save Data, resetting to default</color>");
                SetDefaultValuesForLogic();
            }
            
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

            var humans = Resources.FindObjectsOfTypeAll<GameObject>().Where(v => v.name == "HumanTriggerCrime");
            Utils.PrintDebug("Humans count: " + humans.Count());
            foreach (var item in humans)
                item.AddComponent<DriveByHandler>();

            GameObject.Find("YARD/PlayerMailBox/EnvelopeDoctor")
                .SetActive(savedata.GetValueSafe("DoctorMailSpawned") == 1f);
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
                if (SaveLoad.ValueExists(this, "DebugAdrenaline"))
                    SaveLoad.DeleteValue(this, "DebugAdrenaline");

                SaveLoad.WriteValue(this, "DebugAdrenaline", AdrenalineLogic.config);
            }
            catch
            {
                throw new UnassignedReferenceException("Unable to save DEBUG settings!");
            }
#endif
            SaveLoad.WriteValue(this, "Adrenaline", new Dictionary<string, float>
            {
                ["Value"] = AdrenalineLogic.Value,
                ["LossRate"] = AdrenalineLogic.LossRate,
                ["LossRateLockTime"] = AdrenalineLogic.GetDecreaseLockTime(),
                ["LastDayUpdated"] = AdrenalineLogic.LastDayUpdated,
                ["DoctorMailSpawned"] = GameObject.Find("YARD/PlayerMailBox/EnvelopeDoctor").activeSelf ? 1f : 0f
            });
        }
    }

#if DEBUG
    internal class VariableChanger
    {
        private string field;
        private string ID;
        private List<SettingsSlider> _sliders = null;

        internal VariableChanger(string name, ref List<SettingsSlider> t)
        {
            this.field = name;
            this.ID = name.GetHashCode().ToString();
            this._sliders = t;
        }

        internal void ValueChanged()
        {
            var slider = _sliders.Find(v => v.Instance.ID == ID);
            AdrenalineLogic.config[field] = slider.GetValue();
            Utils.PrintDebug("Set value for " + field + " == " + slider.GetValue());
        }
    }
#endif
}