using System;
using System.Linq;
using System.Collections.Generic;

using Harmony;
using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    public class Adrenaline : Mod
    {
        public override string ID => "com.adrenaline.mod";
        public override string Name => "Adrenaline";
        public override string Author => "Andrinall,@racer";
        public override string Version => "0.24.32";
        public override string Description => "";
        public override bool SecondPass => true;

        private string LastAddedComponent = "";

#if DEBUG
        private SettingsCheckBox lockbox;
        private SettingsSliderInt priceSlider;
        private List<SettingsSlider> _sliders = new List<SettingsSlider>();
        private List<string> _highValues =
            new List<string> { "JANNI_PETTERI_HIT", "VENTTI_WIN", "PISS_ON_DEVICES", "SPARK_WIRING", "COFFEE_INCREASE", "PUB_PRICE" };

        private Dictionary<string, string> localization = new Dictionary<string, string>
        {
            ["MIN_LOSS_RATE"] = "Мин.значение пассивного уменьшения",
            ["MAX_LOSS_RATE"] = "Макс.значение пассивного уменьшения",
            ["LOSS_RATE_SPEED"] = "Модификатор скорости пассивного уменьшения",
            ["DEFAULT_DECREASE"] = "Базовое уменьшение адреналина",
            ["SPRINT_INCREASE"] = "Увеличение от бега",
            ["HIGHSPEED_INCREASE"] = "Увеличение от езды на большой скорости",
            ["BROKEN_WINDSHIELD_INCREASE"] = "Увеличение при езде без лобового стекла",
            ["FIGHT_INCREASE"] = "Увеличение во время драки",
            ["WINDOW_BREAK_INCREASE"] = "Увеличение за разбивание окон (магазин, паб)",
            ["HOUSE_BURNING"] = "Увеличение во время пожара в доме",
            ["TEIMO_PISS"] = "Увеличение за обоссывание Теймо",
            ["GUARD_CATCH"] = "Увеличение при попытках охранника поймать игрока",
            ["VENTTI_WIN"] = "Увеличение адреналина при поражениях в игре со свином",
            ["JANNI_PETTERI_HIT"] = "Увеличение за накаут от NPC (Janni и Petteri)",
            ["TEIMO_SWEAR"] = "Увеличение при ругани Теймо на персонажа",
            ["PISS_ON_DEVICES"] = "Увеличение за обоссывание приборов в доме(TV)",
            ["SPARKS_WIRING"] = "Увеличение при замыкании проводки Satsuma",
            ["SPILL_SHIT"] = "Увеличение при сливе говна в неположенном месте (crime)",
            ["RALLY_PLAYER"] = "Увеличение при участии в ралли (?)",
            ["MURDER_WALKING"] = "Увеличение при приследовании мужиком с топором",
            ["COFFEE_INCREASE"] = "Увеличение от употребления кофе",
            ["ENERGY_DRINK_INCREASE"] = "Увеличение от употребления энергетика",
            ["CRASH_INCREASE"] = "Увеличение за получение урона в аварии",
            ["DRIVEBY_INCREASE"] = "Увеличение при сбитии NPC (зрители ралли)",
            ["PILLS_DECREASE"] = "Уменьшение адреналина после принятия таблеток",
            ["MURDERER_THREAT"] = "Увеличение за уклонение от удара топором",
            ["MURDERER_HIT"] = "Увеличение за удар по мужику с топором",

            ["REQUIRED_SPEED_Jonnez"] = "Мин.скорость для прибавки при езде на Jonezz",
            ["REQUIRED_SPEED_Satsuma"] = "Мин.скорость для прибавки при езде в Satsuma",
            ["REQUIRED_SPEED_Ferndale"] = "Мин.скорость для прибавки при езде в Ferndale",
            ["REQUIRED_SPEED_Hayosiko"] = "Мин.скорость для прибавки при езде в Hayosiko",
            ["REQUIRED_SPEED_Fittan"] = "Мин.скорость для прибавки при езде в Fittan",
            ["REQUIRED_SPEED_Gifu"] = "Мин.скорость для прибавки при езде в Gifu",

            ["REQUIRED_CRASH_SPEED"] = "Мин.скорость для прибавки от аварии",
            ["REQUIRED_WINDSHIELD_SPEED"] = "Мин.скорость для прибавки при езде без лобаша"
        };

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

            Settings.AddHeader(this, "DEBUG SETTINGS");
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
            
            foreach (var element in AdrenalineLogic.config)
            {
                if (element.Key == "PUB_COFFEE_PRICE") continue;

                _sliders.Add(Settings.AddSlider(
                    mod: this,
                    settingID: element.Key.GetHashCode().ToString(),
                    name: localization.GetValueSafe(element.Key),
                    minValue: 0,
                    maxValue: 250,
                    value: element.Value,
                    onValueChanged: new VariableChanger(element.Key, ref _sliders).ValueChanged
                ));
            }
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
            AdrenalineLogic.LossRate =
                AdrenalineLogic.config.GetValueSafe("MAX_LOSS_RATE") - AdrenalineLogic.config.GetValueSafe("MIN_LOSS_RATE");
            
            AdrenalineLogic.Value = 100f;
            AdrenalineLogic.isDead = false;

            if (SaveLoad.ValueExists(this, "Adrenaline"))
                SaveLoad.DeleteValue(this, "Adrenaline");
        }

        public override void OnLoad()
        {
#if DEBUG
            ConsoleCommand.Add(new CREATE_PILLS());
#endif
            AdrenalineLogic.isDead = false;
            AdrenalineLogic.Value = 100f;
            
            var asset = LoadAssets.LoadBundle("Adrenaline.Assets.energy.unity3d");
            asset.GetAllAssetNames().All(v => Utils.PrintDebug(eConsoleColors.WHITE, v) && true);
            AdrenalineLogic.can_texture = LoadAsset<Texture>(asset, "assets/textures/Energy.png");
            AdrenalineLogic.atlas_texture = LoadAsset<Texture>(asset, "assets/textures/ATLAS_OFFICE.png");
            AdrenalineLogic.coffee_cup = LoadAsset<Mesh>(asset, "assets/meshes/coffee_cup_bar_coffee.mesh.obj");
            AdrenalineLogic.empty_cup = LoadAsset<Mesh>(asset, "assets/meshes/coffee_cup_bar.mesh.obj");
            AdrenalineLogic.pills = LoadAsset<GameObject>(asset, "assets/prefabs/Pills.prefab");
            AdrenalineLogic.poster = LoadAsset<GameObject>(asset, "assets/prefabs/Poster.prefab");
            AdrenalineLogic.background = LoadAsset<GameObject>(asset, "assets/prefabs/Background.prefab");
            
            AdrenalineLogic.poster_textures = new List<Texture> {
                LoadAsset<Texture>(asset, "assets/textures/poster1.png"),
                LoadAsset<Texture>(asset, "assets/textures/poster2.png")
            };

            AdrenalineLogic.clips = new List<AudioClip> {
                LoadAsset<AudioClip>(asset, "assets/audio/heart_10.wav"),
                LoadAsset<AudioClip>(asset, "assets/audio/heart_30.wav"),
                LoadAsset<AudioClip>(asset, "assets/audio/heart_50.wav"),
                LoadAsset<AudioClip>(asset, "assets/audio/heart_bust.wav"),
                LoadAsset<AudioClip>(asset, "assets/audio/heart_stop.wav")
            };
            asset.Unload(false);

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

        private T LoadAsset<T>(AssetBundle storage, string path) where T : UnityEngine.Object
        {
            try
            {
                var asset = storage.LoadAsset<T>(path);
                if (asset == null) throw new NullReferenceException();
                return asset;
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load asset {0} from embedded resource (??!)", path);
                return null;
            }
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
    internal class CREATE_PILLS : ConsoleCommand
    {
        public override string Name => "pills";
        public override string Alias => "pl";

        public override string Help => "Debug command for spawning pills";

        public override void Run(string[] args)
        {
            new PillsItem();            
        }
    }
#endif
}