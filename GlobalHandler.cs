using UnityEngine;
using HutongGames.PlayMaker;

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
            PlayerMovementSpeed = Utils.GetGlobalVariable<FsmFloat>("PlayerMovementSpeed");
            HouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
            RallyPlayerOnStage = Utils.GetGlobalVariable<FsmBool>("RallyPlayerOnStage");

            var drink = base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink").gameObject;
            GameHook.InjectStateHook(drink, "Activate 5", delegate
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.COFFEE_INCREASE);
            });

            GameHook.InjectStateHook(drink, "Activate 7", delegate
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.COFFEE_INCREASE);
            });

            GameHook.InjectStateHook(drink, "HomeCoffee", delegate
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.COFFEE_INCREASE);
            });

            AdrenalineLogic.InitHUD();
            Utils.PrintDebug("GlobalHandler enabled");
        }

        private void OnDestroy()
        {
            AdrenalineLogic.DestroyHUD();
            Utils.PrintDebug("GlobalHandler destroyed");
        }

        private void FixedUpdate()
        {
            AdrenalineLogic.Tick();

            Utils.CacheFSM(ref kiljuguy, "KILJUGUY/KiljuMurderer", "Move");

            if (PlayerMovementSpeed?.Value >= 3.5)
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.SPRINT_INCREASE); // increase adrenaline while player sprinting
                Utils.PrintDebug("Value increased by player sprinting");
            }

            if (HouseBurningState?.Value == true)
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.HOUSE_BURNING); // increase adrenaline while house is burning
                Utils.PrintDebug("Value increased by house burning");
            }

            if (kiljuguy?.ActiveStateName == "Walking")
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.MURDER_WALKING);
                Utils.PrintDebug("Value increased by KiljuMurderer is Walking");
            }
        }
    }
}
