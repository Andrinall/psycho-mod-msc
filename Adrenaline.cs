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

        private List<SettingsSlider> _sliders = new List<SettingsSlider>();
        private SettingsCheckBox lockbox;
        private List<string> _highValues =
            new List<string> { "JANNI_PETTERI_HIT", "VENTTI_WIN", "PISS_ON_DEVICES", "SPARK_WIRING" };

        public override void ModSettings()
        {
#if DEBUG
            Settings.AddHeader(this, "DEBUG SETTINGS");
            _sliders.Add(Settings.AddSlider(this, "adn_Value", "Current Adrenaline", 20f, 180f, AdrenalineLogic.Value, OnValueChanged));
            _sliders.Add(Settings.AddSlider(this, "adn_rate", "Current Loss Rate", AdrenalineLogic.config.MIN_LOSS_RATE, AdrenalineLogic.config.MAX_LOSS_RATE, AdrenalineLogic.LossRate, OnValueChanged));
            lockbox = Settings.AddCheckBox(this, "adn_lock", "Lock Loss Rate", AdrenalineLogic.IsDecreaseLocked(), CheckBoxChecked);
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
#endif
        }
#if DEBUG
        private void OnValueChanged()
        {
            foreach (FieldInfo field in typeof(Configuration).GetPublicFields())
            {
                var slider = _sliders.Find(v => v.Instance.Name == field.Name);
                float value = slider.GetValue();
                if (value == AdrenalineLogic.config.GetFieldValue<float>(field.Name)) continue;
                
                field.SetValue(AdrenalineLogic.config, value);
                return;
            }

            if (_sliders[0].GetValue() != AdrenalineLogic.Value)
            {
                AdrenalineLogic.Value = _sliders[0].GetValue();
                return;
            }

            if (_sliders[1].GetValue() != AdrenalineLogic.LossRate)
            {
                AdrenalineLogic.LossRate = _sliders[1].GetValue();
                return;
            }
        }
        private void CheckBoxChecked()
        {
            if (lockbox.GetValue())
                AdrenalineLogic.SetDecreaseLocked(12000);
            else
                AdrenalineLogic.SetDecreaseLocked(1);
        }
#endif

        public override void OnNewGame()
        {
            AdrenalineLogic.LossRate = AdrenalineLogic.config.MAX_LOSS_RATE - AdrenalineLogic.config.MIN_LOSS_RATE;
            AdrenalineLogic.Value = 100f;
        }

        public override void OnLoad()
        {
            AdrenalineLogic.config = SaveLoad.DeserializeSaveFile<Configuration>(this, "AdrenalineModConfiguration.json");

            var player = GameObject.Find("PLAYER");
            player.AddComponent<GlobalHandler>();
            player.AddComponent<HighSpeedHandler>();

            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").AddComponent<PissOnDevicesHandler>();
            GameObject.Find("STORE").AddComponent<StoreActionsHandler>();
            GameObject.Find("DANCEHALL/Functions").AddComponent<DanceHallHandler>();
            GameObject.Find("CABIN/Cabin").AddComponent<VenttiGameHandler>();
            GameObject.Find("NPC_CARS/Amikset").AddComponent<AmiksetHandler>();
            GameObject.Find("SATSUMA(557kg, 248)/Wiring/FireElectric").AddComponent<CarElectricityHandler>();
            GameObject.Find("GIFU(750/450psi)/ShitTank").AddComponent<SpillHandler>();

            foreach (var item in CARS)
            {
                GameObject obj = GameObject.Find(item.CarObject);
                if (obj == null)
                    throw new MissingComponentException("Car with object name \"" + item.CarObject + "\" doesn't exists");

                obj.AddComponent<HighSpeedHandler>().data = item;
            }

            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        public override void OnSave()
        {
            UnityEngine.Object.Destroy(GameObject.Find("PLAYER")?.GetComponent<GlobalHandler>());

            SaveLoad.SerializeSaveFile(this, AdrenalineLogic.config, "AdrenalineModConfiguration");
        }
    }
}