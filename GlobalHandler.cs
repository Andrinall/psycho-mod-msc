using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class GlobalHandler : MonoBehaviour
    {
        private FsmFloat PlayerMovementSpeed;
        private FsmBool HouseBurningState;
        private FsmBool RallyPlayerOnStage;

        private void OnEnable()
        {
            PlayerMovementSpeed = Utils.GetGlobalVariable<FsmFloat>("PlayerMovementSpeed");
            HouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
            RallyPlayerOnStage = Utils.GetGlobalVariable<FsmBool>("RallyPlayerOnStage");
            
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
        }
    }
}
