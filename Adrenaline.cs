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
        public override string Version => "0.20.13";
        public override string Description => "";

        private readonly List<string> CARS = new List<string> {
            "JONNEZ ES(Clone)",
            "SATSUMA(557kg, 248)",
            "FERNDALE(1630kg)",
            "HAYOSIKO(1500kg, 250)",
            "GIFU(750/450psi)",
        };

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
            ["HOUSE_BURNING"] = "Увеличение во время пожара в доме (??)",
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

            ["REQUIRED_SPEED_Jonezz"] = "Мин.скорость для прибавки при езде на Jonezz",
            ["REQUIRED_SPEED_Satsuma"] = "Мин.скорость для прибавки при езде в Satsuma",
            ["REQUIRED_SPEED_Ferndale"] = "Мин.скорость для прибавки при езде в Ferndale",
            ["REQUIRED_SPEED_Hayosiko"] = "Мин.скорость для прибавки при езде в Hayosiko",
            ["REQUIRED_SPEED_Fittan"] = "Мин.скорость для прибавки при езде в Fittan",
            ["REQUIRED_SPEED_Gifu"] = "Мин.скорость для прибавки при езде в Gifu",

            ["REQUIRED_WINDSHIELD_SPEED"] = "Мин.скорость для прибавки при езде без лобаша"
        };

        public override void ModSettings()
        {
            Settings.AddHeader(this, "DEBUG SETTINGS");
            _sliders.Add(Settings.AddSlider(this, "adn_Value", "Текущее значение", 10f, 190f, AdrenalineLogic.Value, AdrenalineChanged));

            var MIN_LOSS_RATE = AdrenalineLogic.config.GetValueSafe("MIN_LOSS_RATE");
            var MAX_LOSS_RATE = AdrenalineLogic.config.GetValueSafe("MAX_LOSS_RATE");
            _sliders.Add(Settings.AddSlider(this, "adn_loss", "Скорость пассивного уменьшения", 
                MIN_LOSS_RATE.Value, MAX_LOSS_RATE.Value, AdrenalineLogic.LossRate, LossRateChanged));

            var PUB_COFFEE_PRICE = AdrenalineLogic.config.GetValueSafe("PUB_COFFEE_PRICE");
            priceSlider = Settings.AddSlider(this, "adn_price", "Стоимость энергетика в пабе", 
                (int)PUB_COFFEE_PRICE.minValue, (int)PUB_COFFEE_PRICE.maxValue, (int)PUB_COFFEE_PRICE.Value, PubPriceChanged);

            lockbox = Settings.AddCheckBox(this, "adn_lock", "Заблокировать уменьшение адреналина", false, delegate {
                var lock_ = lockbox.GetValue();
                AdrenalineLogic.SetDecreaseLocked(lock_, 2_000_000_000f, lock_);
            });
            
            foreach (var element in AdrenalineLogic.config)
            {
                _sliders.Add(Settings.AddSlider(
                    mod: this,
                    settingID: element.Key.GetHashCode().ToString(),
                    name: localization.GetValueSafe(element.Key),
                    minValue: element.Value.minValue,
                    maxValue: element.Value.maxValue,
                    value: element.Value.Value,
                    onValueChanged: OnValueChanged
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

        private void OnValueChanged()
        {
            foreach (var element in AdrenalineLogic.config)
            {
                var slider = _sliders.Find(v => v.Instance.ID == element.Key.GetHashCode().ToString());
                float value = slider.GetValue();
                if (value == element.Value.Value) continue;
                
                element.Value.Value = value;
                return;
            }
        }

        public override void FixedUpdate()
        {
            _sliders[0].Instance.Value = AdrenalineLogic.Value;

            var slider1 = _sliders[1].Instance;
            slider1.Value = AdrenalineLogic.LossRate;
            slider1.Vals[0] = AdrenalineLogic.config.GetValueSafe("MIN_LOSS_RATE").Value;
            slider1.Vals[1] = AdrenalineLogic.config.GetValueSafe("MAX_LOSS_RATE").Value;
            lockbox.SetValue(AdrenalineLogic.IsDecreaseLocked());
        }
#endif

        public override void OnNewGame()
        {
            AdrenalineLogic.LossRate =
                AdrenalineLogic.config.GetValueSafe("MAX_LOSS_RATE").Value - AdrenalineLogic.config.GetValueSafe("MIN_LOSS_RATE").Value;
            
            AdrenalineLogic.Value = 100f;

            if (SaveLoad.ValueExists(this, "Adrenaline"))
                SaveLoad.DeleteValue(this, "Adrenaline");
        }

        public override void OnLoad()
        {
#if DEBUG
            if (SaveLoad.ValueExists(this, "DebugAdrenaline"))
                AdrenalineLogic.config = SaveLoad.ReadValueAsDictionary<string, ConfigItem>(this, "DebugAdrenaline");
            else
                Utils.PrintDebug(eConsoleColors.RED, "DEBUG Settings not loaded, resetting to default");
#endif
            try
            {
                var asset = LoadAssets.LoadBundle("Adrenaline.Assets.energy.unity3d");
                AdrenalineLogic.texture = asset.LoadAsset<Texture>("energy.png");
                AdrenalineLogic.empty_cup = asset.LoadAsset<Mesh>("coffee_cup_bar.mesh.obj");
                AdrenalineLogic.coffee_cup = asset.LoadAsset<Mesh>("coffee_cup_bar_coffee.mesh.obj");
                asset.Unload(false);
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load asset from embedded resource (??!)");
            }

            try
            {
                AddComponent<GlobalHandler>("PLAYER");
                if (SaveLoad.ValueExists(this, "Adrenaline"))
                {
                    var data = SaveLoad.ReadValueAsDictionary<string, float>(this, "Adrenaline");
                    var time = data.GetValueSafe("LossRateLockTime");
                    var value = data.GetValueSafe("Value");
                    AdrenalineLogic.Value = (value <= AdrenalineLogic.MIN_ADRENALINE + 5f) ? 20f : value;
                    AdrenalineLogic.LossRate = data.GetValueSafe("LossRate");
                    AdrenalineLogic.SetDecreaseLocked(time > 0, time);

                    Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
                }
                else
                    ModConsole.Print("<color=red>Unable to load Save Data, resetting to default</color>");

                AddComponent<PissOnDevicesHandler>("PLAYER");
                AddComponent<SpillHandler>("GIFU(750/450psi)/ShitTank");
                AddComponent<AmiksetHandler>("NPC_CARS/Amikset");
                AddComponent<StoreActionsHandler>("STORE");
                AddComponent<CustomEnergyDrink>("STORE");
                AddComponent<VenttiGameHandler>("CABIN/Cabin/Ventti/Table/GameManager");
                AddComponent<DanceHallHandler>("DANCEHALL");

                AddComponent<CarElectricityHandler>("SATSUMA(557kg, 248)");
                AddComponent<FerndaleSeatbeltFix>("FERNDALE(1630kg)");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load component {0}", LastAddedComponent);
            }

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
            
            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        private T AddComponent<T>(string obj) where T : Component
        {
            LastAddedComponent = string.Format("{0}::{1}", obj, typeof(T).Name.ToString());
            Utils.PrintDebug(eConsoleColors.YELLOW, "Loading component " + LastAddedComponent);
            return GameObject.Find(obj).AddComponent<T>();
        }

        public override void OnSave()
        {
#if DEBUG
            SaveLoad.WriteValue(this, "DebugAdrenaline", AdrenalineLogic.config);
#endif
            SaveLoad.WriteValue(this, "Adrenaline", new Dictionary<string, float>
            {
                ["Value"] = AdrenalineLogic.Value,
                ["LossRate"] = AdrenalineLogic.LossRate,
                ["LossRateLockTime"] = AdrenalineLogic.GetDecreaseLockTime()
            });
        }
    }
}