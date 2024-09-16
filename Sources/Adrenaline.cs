using System;
using System.IO;
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
        public override string Version => "0.63.26";
        public override string Description => "Adds a character's need for adrenaline";
        public override bool UseAssetsFolder => false;
        public override bool SecondPass => true;

        private string LastAddedComponent = "";
        private string SaveDataPath = Application.persistentDataPath + "\\Adrenaline.dat";

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

            var i = 0;
            var _lossRateItems = AdrenalineLogic.config.Count(v => v.Key.Contains("LOSS_RATE"));
            var max_value = 250f;
            foreach (var item in AdrenalineLogic.config)
            {
                if (item.Key.Contains("LOSS_RATE")) continue;
                if (item.Key == "PUB_COFFEE_PRICE") continue;

                if (i == 0)
                {
                    Settings.AddText(this, "<color=red>=========================== Timed ============================</color>");
                    max_value = 2f;
                }
                else if (i == 13 - _lossRateItems + 2)
                {
                    Settings.AddText(this, "<color=red>===========================  Once  ============================</color>");
                    max_value = 50f;
                }
                else if (i == 29 - _lossRateItems)
                {
                    Settings.AddText(this, "<color=red>=========================  Min.vars  ===========================</color>");
                    max_value = 250f;
                }

                AddSlider(item, max_value);
                i++;
            }
        }

        private void AddSlider(KeyValuePair<string, float> element, float maxValue = 250f)
        {
            _sliders.Add(Settings.AddSlider(
                mod: this,
                settingID: element.Key.GetHashCode().ToString(),
                name: Globals.localization.GetValueSafe(element.Key),
                minValue: 0f,
                maxValue: maxValue,
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
            File.Delete(SaveDataPath);
            SetDefaultValuesForLogic();
            Utils.PrintDebug(eConsoleColors.RED, $"New game started, save file removed!");
        }

        private void SetDefaultValuesForLogic()
        {
            AdrenalineLogic.isDead = false;
            AdrenalineLogic.envelopeSpawned = false;
            AdrenalineLogic.Value = 100f;
            AdrenalineLogic.LastDayUpdated = Utils.GetGlobalVariable<FsmInt>("GlobalDay").Value;
            AdrenalineLogic.UpdateLossRatePerDay(AdrenalineLogic.LastDayUpdated);
            AdrenalineLogic.SetDecreaseLocked(false, 0);
        }

        public override void OnLoad()
        {
            UnloadResources();

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

            try
            {
                byte[] value = File.ReadAllBytes(SaveDataPath);
                AdrenalineLogic.isDead = BitConverter.ToBoolean(value, 0);
                AdrenalineLogic.envelopeSpawned = BitConverter.ToBoolean(value, 1);
                AdrenalineLogic.Value = BitConverter.ToSingle(value, 2);
                AdrenalineLogic.LossRate = BitConverter.ToSingle(value, 6);
                AdrenalineLogic.LastDayUpdated = BitConverter.ToInt32(value, 10);
                var time = BitConverter.ToSingle(value, 14);
                AdrenalineLogic.SetDecreaseLocked(time > 0, time);
                if (AdrenalineLogic.isDead)
                {
                    if (AdrenalineLogic.LossRate > 3.5f)
                        AdrenalineLogic.UpdateLossRatePerDay();

                    AdrenalineLogic.Value = 30f;
                    AdrenalineLogic.isDead = false;
                }

                int _items = BitConverter.ToInt32(value, 18);
                if (_items == 0) goto SkipLoadPills;

                int _offset = 22;
                Utils.PrintDebug(eConsoleColors.WHITE, $"Loading {_items} pills");
                for (var i = 0; i < _items; i++)
                {
                    Utils.PrintDebug(eConsoleColors.WHITE, $"Loading pills with idx: {i}");
                    float x = BitConverter.ToSingle(value, _offset);
                    float y = BitConverter.ToSingle(value, _offset + 4);
                    float z = BitConverter.ToSingle(value, _offset + 8);
                    float rX = BitConverter.ToSingle(value, _offset + 12);
                    float rY = BitConverter.ToSingle(value, _offset + 16);
                    float rZ = BitConverter.ToSingle(value, _offset + 20);
                    Utils.PrintDebug(eConsoleColors.WHITE, $"Position: x:{x}, y:{y}, z:{z}");

                    Globals.pills_list.Add(
                        new PillsItem(i, new Vector3(x, y, z), new Vector3(rX, rY, rZ))
                    );
                    _offset += 24;
                }

                SkipLoadPills:
                Utils.PrintDebug($"Value:{AdrenalineLogic.Value}; time:{time}, day:{AdrenalineLogic.LastDayUpdated}, loss:{AdrenalineLogic.LossRate}, dead:{AdrenalineLogic.isDead}, env:{AdrenalineLogic.envelopeSpawned}");
                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
            }
            catch (Exception e)
            {
                ModConsole.Error("<color=red>Unable to load Save Data, resetting to default</color>");
                Utils.PrintDebug(eConsoleColors.RED, e.GetFullMessage());
                SetDefaultValuesForLogic();
            }
            
            AddComponent<GlobalHandler>("PLAYER");
            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        public override void SecondPassOnLoad()
        {
            ConsoleCommand.Add(new FixBrokenHUD());
#if DEBUG
            ConsoleCommand.Add(new TeleportToPills());
#endif

            AddComponent<PissOnDevicesHandler>("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger");
            AddComponent<AmiksetHandler>("NPC_CARS/Amikset/KYLAJANI/Driver/Animations");
            AddComponent<AmiksetHandler>("NPC_CARS/Amikset/AMIS2/Passengers 3/Animations");
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
                        throw new MissingComponentException($"Car with object name \"{item}\" doesn't exists");

                    obj.AddComponent<HighSpeedHandler>();
                    obj.AddComponent<WindshieldHandler>();
                }
                catch (Exception e)
                {
                    Utils.PrintDebug(eConsoleColors.RED, $"HighSpeedHandler loading error for {item}\n{e.GetFullMessage()}");
                }
            }

            var _objects = Resources.FindObjectsOfTypeAll<GameObject>();

            var dynamics = Resources.FindObjectsOfTypeAll<CarDynamics>().Where(v => v.transform.parent == null);
            foreach (var item in dynamics)
                item.gameObject.AddComponent<CrashHandler>();

            var humans = _objects.Where(v => v.name == "HumanTriggerCrime");
            Utils.PrintDebug("Humans count: " + humans.Count());
            foreach (var item in humans)
                item.AddComponent<DriveByHandler>();
        }

        private T AddComponent<T>(string obj) where T : Component
        {
            LastAddedComponent = string.Format("{0}::{1}", obj, typeof(T)?.Name.ToString());
            Utils.PrintDebug(eConsoleColors.YELLOW, "Loading component " + LastAddedComponent);
            return GameObject.Find(obj)?.AddComponent<T>() ?? null;
        }

        private void UnloadResources()
        {
            AdrenalineLogic.mailboxSheet = null;

            Globals.pills_list.Clear();
            Globals.poster_textures.Clear();
            Globals.mailScreens.Clear();
            Globals.audios.Clear();
            Globals.clips.Clear();
            Globals.background = null;
            Globals.pills = null;
            Globals.poster = null;
            Globals.can_texture = null;
            Globals.atlas_texture = null;
            Globals.empty_cup = null;
            Globals.coffee_cup = null;

            var handler = GameObject.Find("PLAYER")?.GetComponent<GlobalHandler>();
            if (handler) UnityEngine.Object.Destroy(handler);
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
            catch (Exception e)
            {
                throw new UnassignedReferenceException($"Unable to save DEBUG settings!\n{e.GetFullMessage()}");
            }
#endif

            var list = Globals.pills_list;
            int _items = list.Count;

            byte[] array = new byte[18 + 4 + (_items * 24)];
            BitConverter.GetBytes(AdrenalineLogic.isDead).CopyTo(array, 0); // 1
            BitConverter.GetBytes(AdrenalineLogic.envelopeSpawned).CopyTo(array, 1); // 1
            BitConverter.GetBytes(AdrenalineLogic.Value).CopyTo(array, 2); // 4
            BitConverter.GetBytes(AdrenalineLogic.LossRate).CopyTo(array, 6); // 4
            BitConverter.GetBytes(AdrenalineLogic.LastDayUpdated).CopyTo(array, 10); // 4
            BitConverter.GetBytes(AdrenalineLogic.GetDecreaseLockTime()).CopyTo(array, 14); // 4
            BitConverter.GetBytes(_items).CopyTo(array, 18); // 4
            Debug.LogWarning($"[ADRENALINE DBG]: _items Count: {_items}; array len: {array.Length}");

            if (_items == 0)
            {
                File.WriteAllBytes(SaveDataPath, array);
                return;
            }
            
            int _offset = 22;
            for (var i = 0; i < _items; i++)
            {
                var _item = list.ElementAt(i);
                var pos = _item.self.transform.position;
                var rot = _item.self.transform.eulerAngles;
                BitConverter.GetBytes(pos.x).CopyTo(array, _offset);
                BitConverter.GetBytes(pos.y).CopyTo(array, _offset + 4);
                BitConverter.GetBytes(pos.z).CopyTo(array, _offset + 8);
                BitConverter.GetBytes(rot.x).CopyTo(array, _offset + 12);
                BitConverter.GetBytes(rot.y).CopyTo(array, _offset + 16);
                BitConverter.GetBytes(rot.z).CopyTo(array, _offset + 20);
                Debug.LogWarning($"[ADRENALINE DBG]: Pills inserted with data x:{pos.x},y:{pos.y},z:{pos.z}; rX:{rot.x},rY:{rot.y},rZ:{rot.z}");
                _offset += 24;
            }

            File.WriteAllBytes(SaveDataPath, array);
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
            Utils.PrintDebug($"Set value for {field} == {slider.GetValue()}");
        }
    }
#endif
}