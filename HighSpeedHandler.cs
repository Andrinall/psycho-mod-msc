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
            drivetrain = base.GetComponent<Drivetrain>();
            PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
            SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
            Utils.PrintDebug("HighSpeedHandler enabled");
        }

        private void FixedUpdate()
        {
            if (PlayerVehicle.Value != CarName) return;

            var RequiredSpeed = (float)typeof(Configuration).GetField("REQUIRED_SPEED_" + CarName).GetValue(AdrenalineLogic.config);
            if (!SeatbeltLocked.Value && drivetrain.differentialSpeed > RequiredSpeed)
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.HIGHSPEED_INCREASE);
                Utils.PrintDebug("Value++ by driving on high speed on " + CarName);
            }
        }
    }
}
