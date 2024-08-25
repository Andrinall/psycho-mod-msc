using MSCLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        public override void OnNewGame()
        {
            AdrenalineLogic.LossRate = Configuration.MAX_LOSS_RATE - Configuration.MIN_LOSS_RATE;
            AdrenalineLogic.Value = 100f;
        }

        public override void OnLoad()
        {
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
        }
    }
}