using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class HighSpeedHandler : MonoBehaviour
    {
        public string CarName;
        private Drivetrain drivetrain;
        private FsmString PlayerVehicle;
        private FsmBool SeatbeltLocked;

        private void OnEnable()
        {
            try
            {
                drivetrain = base.GetComponent<Drivetrain>();
                PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
                SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
                Utils.PrintDebug(eConsoleColors.GREEN, "HighSpeedHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load HighSpeedHandler component");
            }
        }

        private void FixedUpdate()
        {
            if (PlayerVehicle.Value != CarName) return;

            var RequiredSpeed = (float)typeof(Configuration).GetField("REQUIRED_SPEED_" + CarName).GetValue(AdrenalineLogic.config);
            if (!SeatbeltLocked.Value && drivetrain.differentialSpeed > RequiredSpeed)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.HIGHSPEED_INCREASE);
        }
    }
}
