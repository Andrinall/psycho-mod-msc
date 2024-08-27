using System.Reflection;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Adrenaline
{
    public class Adrenaline : Mod
    {
        public override string ID => "com.adrenaline.mod";
        public override string Name => "Adrenaline";
        public override string Author => "Andrinall,@racer";
        public override string Version => "0.0.1";
        public override string Description => "";

        internal readonly List<CarData> CARS = new List<CarData> {
            new CarData { CarObject = "JONNEZ ES(Clone)",      CarName = "Jonezz",   RequiredSpeed = 70f  },
            new CarData { CarObject = "SATSUMA(557kg, 248)",   CarName = "Satsuma",  RequiredSpeed = 120f },
            new CarData { CarObject = "FERNDALE(1630kg)",      CarName = "Ferndale", RequiredSpeed = 110f },
            new CarData { CarObject = "HAYOSIKO(1500kg, 250)", CarName = "Hayosiko", RequiredSpeed = 110f },
            new CarData { CarObject = "FITTAN",                CarName = "Fittan",   RequiredSpeed = 70f  },
            new CarData { CarObject = "GIFU(750/450psi)",      CarName = "Gifu",     RequiredSpeed = 70f  }
        };

        private SettingsCheckBox lockbox;
        private List<SettingsSlider> _sliders = new List<SettingsSlider>();
        private List<string> _highValues =
            new List<string> { "JANNI_PETTERI_HIT", "VENTTI_WIN", "PISS_ON_DEVICES", "SPARK_WIRING" };

#if DEBUG
        public override void ModSettings()
        {
            Settings.AddHeader(this, "DEBUG SETTINGS");
            _sliders.Add(Settings.AddSlider(this, "adn_Value", "Current Adrenaline", 20f, 180f, AdrenalineLogic.Value, AdrenalineChanged));
            _sliders.Add(Settings.AddSlider(this, "adn_rate", "Current Loss Rate", AdrenalineLogic.config.MIN_LOSS_RATE, AdrenalineLogic.config.MAX_LOSS_RATE, AdrenalineLogic.LossRate, LossRateChanged));
            lockbox = Settings.AddCheckBox(this, "adn_lock", "Lock Adrenaline Loss", false, () => {
                AdrenalineLogic.SetDecreaseLocked(lockbox.GetValue());
            });
            
            foreach (FieldInfo field in typeof(Configuration).GetPublicFields())
            {
                if (field.IsInitOnly) continue; // ignore readonly variables for avoid errors
                _sliders.Add(Settings.AddSlider(
                    mod: this,
                    settingID: field.Name.GetHashCode().ToString(),
                    name: field.Name,
                    minValue: 0f,
                    maxValue: _highValues.Contains(field.Name) ? 50f : 2f,
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
#endif

        public override void FixedUpdate()
        {
            _sliders[0].Instance.Value = AdrenalineLogic.Value;
            _sliders[1].Instance.Value = AdrenalineLogic.LossRate;
        }

        public override void OnNewGame()
        {
            AdrenalineLogic.LossRate = AdrenalineLogic.config.MAX_LOSS_RATE - AdrenalineLogic.config.MIN_LOSS_RATE;
            AdrenalineLogic.Value = 100f;
        }

        public override void OnLoad()
        {
            Configuration temp = SaveLoad.DeserializeSaveFile<Configuration>(this, "AdrenalineModConfiguration.json");
            if (temp == null)
                Utils.PrintDebug("<color=red>DeserializeSaveFile is null</color>");
            else
                AdrenalineLogic.config = temp;

            GameObject.Find("PLAYER").AddComponent<GlobalHandler>();
            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger").AddComponent<PissOnDevicesHandler>();
            GameObject.Find("GIFU(750/450psi)/ShitTank").AddComponent<SpillHandler>();
            GameObject.Find("NPC_CARS/Amikset").AddComponent<AmiksetHandler>();
            GameObject.Find("STORE").AddComponent<StoreActionsHandler>();
            GameObject.Find("CABIN/Cabin/Ventti/Table/GameManager").AddComponent<VenttiGameHandler>();
            GameObject.Find("DANCEHALL").AddComponent<DanceHallHandler>();

            GameObject.Find("SATSUMA(557kg, 248)/Wiring").AddComponent<CarElectricityHandler>();
            GameObject.Find("SATSUMA(557kg, 248)/Body/Windshield").AddComponent<WindshieldHandler>().CarName = "Satsuma";
            GameObject.Find("GIFU(750/450psi)/LOD/WindshieldLeft").AddComponent<WindshieldHandler>().CarName = "Gifu";
            GameObject.Find("HAYOSIKO(1500kg, 250)").AddComponent<WindshieldHandler>().CarName = "Hayosiko";

            foreach (var item in CARS)
            {
                GameObject obj = GameObject.Find(item.CarObject);
                if (obj == null)
                    throw new MissingComponentException("Car with object name \"" + item.CarObject + "\" doesn't exists");

                obj.AddComponent<HighSpeedHandler>().CarName = item.CarName;
            }

            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        public override void OnSave()
        {
            SaveLoad.SerializeSaveFile(this, AdrenalineLogic.config, "AdrenalineModConfiguration");
        }
    }
}