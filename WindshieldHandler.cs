using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class WindshieldHandler : MonoBehaviour
    {
        private FsmBool Damaged;
        private FsmBool Helmet;
        private FsmString PlayerVehicle;
        private Drivetrain drivetrain;
        public string CarName;

        private void OnEnable()
        {
            try
            {
                if (base.GetComponents<Drivetrain>().Length != 0)
                {
                    drivetrain = base.GetComponent<Drivetrain>();
                    Damaged = base.transform.Find("LOD/Windshield").GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("Damaged");
                    Helmet = Utils.GetGlobalVariable<FsmBool>("PlayerHelmet");
                    Utils.PrintDebug(eConsoleColors.GREEN, "WindshieldHandler enabled for hayosiko");
                }
                else
                {
                    drivetrain = base.transform.parent.parent.GetComponent<Drivetrain>();
                    Damaged = base.GetComponent<PlayMakerFSM>().FsmVariables?.GetFsmBool("Damaged");
                    Utils.PrintDebug(eConsoleColors.GREEN, "WindshieldHandler enabled");
                }
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load WindshieldHandler");
            }
            PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
        }

        private void FixedUpdate()
        {
            if (PlayerVehicle?.Value != CarName) return;
            if (!Helmet.Value && Damaged?.Value == true && drivetrain?.differentialSpeed > AdrenalineLogic.config.REQUIRED_WINDSHIELD_SPEED)
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.BROKEN_WINDSHIELD_INCREASE);
                Utils.PrintDebug("Value increased by driving with broken windshield and speed > " + AdrenalineLogic.config.REQUIRED_WINDSHIELD_SPEED.ToString());
            }
        }
    }
}
