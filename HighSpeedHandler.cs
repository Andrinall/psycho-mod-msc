using UnityEngine;
using HutongGames.PlayMaker;
using Harmony;

namespace Adrenaline
{
    internal class HighSpeedHandler : MonoBehaviour
    {
        private string CarName;
        private Drivetrain drivetrain;
        private FsmString PlayerVehicle;
        private FsmBool SeatbeltLocked;
        private FsmBool Helmet;

        private void Start()
        {
            try
            {
                CarName = Utils.GetCarNameByObject(base.transform.gameObject);

                drivetrain = base.GetComponent<Drivetrain>();
                PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
                SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
                Helmet = Utils.GetGlobalVariable<FsmBool>("PlayerHelmet");
                Utils.PrintDebug(eConsoleColors.GREEN, "HighSpeedHandler enabled for {0}", CarName);
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load HighSpeedHandler component for {0}", CarName);
            }
        }

        private void FixedUpdate()
        {
            if (PlayerVehicle.Value != CarName) return;
            if (CarName == "Jonnez" && Helmet.Value == true) return;
            if (CarName != "Jonnez" && SeatbeltLocked.Value == true) return;

            var RequiredSpeed = AdrenalineLogic.config.GetValueSafe("REQUIRED_SPEED_" + CarName);
            if (drivetrain?.differentialSpeed >= RequiredSpeed)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("HIGHSPEED_INCREASE"));
        }
    }
}
