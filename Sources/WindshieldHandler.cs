using Harmony;
using UnityEngine;
using HutongGames.PlayMaker;
using MSCLoader;

namespace Adrenaline
{
    internal class WindshieldHandler : MonoBehaviour
    {
        private FsmBool Damaged;
        private FsmBool Helmet;
        private FsmString PlayerVehicle;
        private Drivetrain drivetrain;
        private string CarName;

        private void Start()
        {
            try
            {
                CarName = Utils.GetCarNameByObject(base.transform.gameObject);
                
                Helmet = Utils.GetGlobalVariable<FsmBool>("PlayerHelmet");
                PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");

                switch (CarName)
                {
                    case "Satsuma":
                    {
                        Damaged = base.transform.Find("Body/Windshield")
                            .GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("Damaged");
                        break;
                    }
                    case "Gifu":
                    {
                        Damaged = base.transform.Find("LOD/WindshieldLeft")
                            .GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("Damaged");
                        break;
                    }
                    case "Hayosiko":
                    {
                        Damaged = base.transform.Find("LOD/Windshield")
                            .GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("Damaged");
                        break;
                    }
                    default:
                    {
                        Destroy(this);
                        return;
                    }
                }

                drivetrain = base.GetComponent<Drivetrain>();
                Utils.PrintDebug(eConsoleColors.GREEN, $"WindshieldHandler enabled for {CarName}");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to load WindshieldHandler for {CarName}\n{e.GetFullMessage()}");
            }
        }

        private void FixedUpdate()
        {
            if (PlayerVehicle?.Value != CarName) return;
            if (Damaged.Value == false) return;
            if (Helmet.Value == true) return;

            var requiredSpeed = AdrenalineLogic.config.GetValueSafe("REQUIRED_WINDSHIELD_SPEED");
            if (drivetrain?.differentialSpeed > requiredSpeed)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("BROKEN_WINDSHIELD_INCREASE"));
        }
    }
}