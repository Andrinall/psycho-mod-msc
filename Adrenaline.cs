using System.Reflection;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using Harmony;
using System.Linq;

namespace Adrenaline
{
    public class Adrenaline : Mod
    {
        public override string ID => "com.adrenaline.mod";
        public override string Name => "Adrenaline";
        public override string Author => "Andrinall,@racer";
        public override string Version => "0.19.10";
        public override string Description => "";

        internal readonly List<CarData> CARS = new List<CarData> {
            new CarData { CarObject = "JONNEZ ES(Clone)",      CarName = "Jonezz",   RequiredSpeed = 70f  },
            new CarData { CarObject = "SATSUMA(557kg, 248)",   CarName = "Satsuma",  RequiredSpeed = 120f },
            new CarData { CarObject = "FERNDALE(1630kg)",      CarName = "Ferndale", RequiredSpeed = 110f },
            new CarData { CarObject = "HAYOSIKO(1500kg, 250)", CarName = "Hayosiko", RequiredSpeed = 110f },
            new CarData { CarObject = "FITTAN",                CarName = "Fittan",   RequiredSpeed = 70f  },
            new CarData { CarObject = "GIFU(750/450psi)",      CarName = "Gifu",     RequiredSpeed = 70f  }
        };

#if DEBUG
        private SettingsCheckBox lockbox;
        private SettingsSliderInt priceSlider;
        private List<SettingsSlider> _sliders = new List<SettingsSlider>();
        private List<string> _highValues =
            new List<string> { "JANNI_PETTERI_HIT", "VENTTI_WIN", "PISS_ON_DEVICES", "SPARK_WIRING", "COFFEE_INCREASE", "PUB_PRICE" };

        private Dictionary<string, string> localization = new Dictionary<string, string>
        {
            ["MIN_LOSS_RATE"] = "Минимально возможная скорость пассивного уменьшения",
            ["MAX_LOSS_RATE"] = "Максимально возможная скорость пассивного уменьшения",
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
            _sliders.Add(Settings.AddSlider(this, "adn_Value", "Текущее значение", 20f, 180f, AdrenalineLogic.Value, AdrenalineChanged));
            _sliders.Add(Settings.AddSlider(this, "adn_rate", "Скорость пассивного уменьшения", AdrenalineLogic.config.MIN_LOSS_RATE, AdrenalineLogic.config.MAX_LOSS_RATE, AdrenalineLogic.LossRate, LossRateChanged));
            priceSlider = Settings.AddSlider(this, "adn_price", "Стоимость энергетика в пабе", 0, 50, (int)AdrenalineLogic.config.PUB_PRICE, PubPriceChanged);
            lockbox = Settings.AddCheckBox(this, "adn_lock", "Заблокировать уменьшение адреналина", false, delegate {
                var lock_ = lockbox.GetValue();
                AdrenalineLogic.SetDecreaseLocked(lock_, 2_000_000_000f, lock_);
            });
            
            foreach (FieldInfo field in typeof(Configuration).GetPublicFields())
            {
                if (field.IsInitOnly) continue; // ignore readonly variables for avoid errors
                _sliders.Add(Settings.AddSlider(
                    mod: this,
                    settingID: field.Name.GetHashCode().ToString(),
                    name: localization.GetValueSafe(field.Name),
                    minValue: 0f,
                    maxValue: _highValues.Contains(field.Name) ? 50f : (field.Name.Contains("REQUIRED_SPEED") ? 180f : 2f),
                    value: (float)field.GetValue(AdrenalineLogic.config),
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
            GameObject.Find("STORE").GetComponent<CustomEnergyDrink>().SetDrinkPrice((float)priceSlider.GetValue());
        }

        private void OnValueChanged()
        {
            foreach (FieldInfo field in typeof(Configuration).GetPublicFields())
            {
                if (field.IsInitOnly) continue;
                var slider = _sliders.Find(v => v.Instance.Name == field.Name);
                float value = slider.GetValue();
                if (value == (float)field.GetValue(AdrenalineLogic.config)) continue;
                
                field.SetValue(AdrenalineLogic.config, value);
                return;
            }
        }

        public override void FixedUpdate()
        {
            _sliders[0].Instance.Value = AdrenalineLogic.Value;
            _sliders[1].Instance.Value = AdrenalineLogic.LossRate;
            lockbox.Instance.Value = AdrenalineLogic.IsDecreaseLocked();
        }
#endif

        public override void OnNewGame()
        {
            AdrenalineLogic.LossRate = AdrenalineLogic.config.MAX_LOSS_RATE - AdrenalineLogic.config.MIN_LOSS_RATE;
            AdrenalineLogic.Value = 100f;
            if (SaveLoad.ValueExists(this, "Adrenaline"))
                SaveLoad.DeleteValue(this, "Adrenaline");
        }

        public override void OnLoad()
        {
#if DEBUG
            Configuration temp = SaveLoad.DeserializeSaveFile<Configuration>(this, "AdrenalineModConfiguration.json");
            if (temp == null)
                Utils.PrintDebug("<color=red>DeserializeSaveFile is null</color>");
            else
                AdrenalineLogic.config = temp;
#endif
            var asset = LoadAssets.LoadBundle("Adrenaline.Assets.energy.unity3d");
            AdrenalineLogic.texture = asset.LoadAsset<Texture>("energy.png");
            AdrenalineLogic.empty_cup = asset.LoadAsset<Mesh>("coffee_cup_bar.mesh.obj");
            AdrenalineLogic.coffee_cup = asset.LoadAsset<Mesh>("coffee_cup_bar_coffee.mesh.obj");
            //foreach (var assetName in asset.GetAllAssetNames()) Utils.PrintDebug("[bundle] Asset: " + assetName);
            asset.Unload(false);

            GameObject.Find("PLAYER").AddComponent<GlobalHandler>();
            if (SaveLoad.ValueExists(this, "Adrenaline"))
            {
                var data = SaveLoad.ReadValueAsDictionary<string, float>(this, "Adrenaline");
                var time = data.GetValueSafe("LossRateLockTime");
                var value = data.GetValueSafe("Value");
                AdrenalineLogic.Value = (value <= AdrenalineLogic.MIN_ADRENALINE + 5f) ? 20f : value;
                AdrenalineLogic.LossRate = data.GetValueSafe("LossRate");
                AdrenalineLogic.SetDecreaseLocked(time > 0, time);

                Utils.PrintDebug("<color=green>Save Data Loaded!</color>");
            }
            else
                ModConsole.Print("<color=red>Unable to load Save Data, resetting to default</color>");

            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger").AddComponent<PissOnDevicesHandler>();
            GameObject.Find("GIFU(750/450psi)/ShitTank").AddComponent<SpillHandler>();
            GameObject.Find("NPC_CARS/Amikset").AddComponent<AmiksetHandler>();
            GameObject.Find("STORE").AddComponent<StoreActionsHandler>();
            GameObject.Find("STORE").AddComponent<CustomEnergyDrink>();
            GameObject.Find("CABIN/Cabin/Ventti/Table/GameManager").AddComponent<VenttiGameHandler>();
            GameObject.Find("DANCEHALL").AddComponent<DanceHallHandler>();

            GameObject.Find("SATSUMA(557kg, 248)/Wiring").AddComponent<CarElectricityHandler>();
            GameObject.Find("SATSUMA(557kg, 248)/Body/Windshield").AddComponent<WindshieldHandler>().CarName = "Satsuma";
            GameObject.Find("GIFU(750/450psi)/LOD/WindshieldLeft").AddComponent<WindshieldHandler>().CarName = "Gifu";
            GameObject.Find("HAYOSIKO(1500kg, 250)").AddComponent<WindshieldHandler>().CarName = "Hayosiko";
            GameObject.Find("FERNDALE(1630kg)").AddComponent<FerndaleSeatbeltFix>();

            try
            {
                foreach (var item in CARS)
                {
                    GameObject obj = GameObject.Find(item.CarObject);
                    if (obj == null)
                        throw new MissingComponentException("Car with object name \"" + item.CarObject + "\" doesn't exists");

                    obj.AddComponent<HighSpeedHandler>().CarName = item.CarName;
                }
            }
            catch (MissingComponentException ex)
            {
                Utils.PrintDebug("<color=red>HighSpeedHandlers loading error: " + ex.GetFullMessage() + "</color>");
            }


            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        public override void OnSave()
        {
#if DEBUG
            SaveLoad.SerializeSaveFile(this, AdrenalineLogic.config, "AdrenalineModConfiguration.json");
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