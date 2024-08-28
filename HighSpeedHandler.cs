using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class HighSpeedHandler : MonoBehaviour
    {
        public string CarName;
        private Drivetrain drivetrain;
        private FsmString playerVehicle;

        private void OnEnable()
        {
            drivetrain = base.GetComponent<Drivetrain>();
            playerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
            Utils.PrintDebug("HighSpeedHandler enabled");
        }

        private void FixedUpdate()
        {
            if (playerVehicle.Value != CarName) return;

            var RequiredSpeed = (float)typeof(Configuration).GetField("REQUIRED_SPEED_" + CarName).GetValue(AdrenalineLogic.config);
            if (drivetrain.differentialSpeed > RequiredSpeed)
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.HIGHSPEED_INCREASE);
                Utils.PrintDebug("Value++ by driving on high speed on " + CarName);
            }
        }
    }
}
