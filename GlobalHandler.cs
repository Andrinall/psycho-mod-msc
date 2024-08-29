using UnityEngine;
using HutongGames.PlayMaker;
using System.Linq;

namespace Adrenaline
{
    internal class GlobalHandler : MonoBehaviour
    {
        private FsmFloat PlayerMovementSpeed;
        private FsmBool HouseBurningState;
        private FsmBool RallyPlayerOnStage;

        private PlayMakerFSM kiljuguy;

        private void OnEnable()
        {
            try
            {
                AdrenalineLogic.InitHUD();

                PlayerMovementSpeed = Utils.GetGlobalVariable<FsmFloat>("PlayerMovementSpeed");
                HouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
                RallyPlayerOnStage = Utils.GetGlobalVariable<FsmBool>("RallyPlayerOnStage");

                var fpsCamera = base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera");
                var drink = fpsCamera.Find("Drink").gameObject;
                GameHook.InjectStateHook(drink, "Activate 5", delegate
                {
                    AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.COFFEE_INCREASE);
                    AdrenalineLogic.SetDecreaseLocked(true, 12000f);
                });

                GameHook.InjectStateHook(drink, "Activate 7", delegate
                {
                    AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.COFFEE_INCREASE);
                    AdrenalineLogic.SetDecreaseLocked(true, 12000f); // 1 minute
                });

                GameHook.InjectStateHook(drink, "HomeCoffee", delegate
                {
                    AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.COFFEE_INCREASE);
                    AdrenalineLogic.SetDecreaseLocked(true, 12000f);
                });

                Utils.PrintDebug(eConsoleColors.GREEN, "GlobalHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load GlobalHandler component");
            }

            try
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Loading component FITTAN::HighSpeedHandler");
                Resources.FindObjectsOfTypeAll<Transform>().Where(v => v.name == "FITTAN") // avoid error (scene contains two fittan objects)
                    .Where(v => v.GetComponents<PlayMakerFSM>().Length == 4).ToArray()[0].gameObject
                    .AddComponent<HighSpeedHandler>().CarName = "Fittan";
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load HighSpeedHandler component for FITTAN");
            }
        }

        private void OnDestroy()
        {
            AdrenalineLogic.DestroyHUD();
        }

        private void FixedUpdate()
        {
            AdrenalineLogic.Tick();

            Utils.CacheFSM(ref kiljuguy, "KILJUGUY/KiljuMurderer", "Move");

            if (PlayerMovementSpeed?.Value >= 3.5)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.SPRINT_INCREASE); // increase adrenaline while player sprinting

            if (HouseBurningState?.Value == true)
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.HOUSE_BURNING); // increase adrenaline while house is burning

            if (kiljuguy?.ActiveStateName == "Walking")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.MURDER_WALKING); // ??
        }
    }
}
