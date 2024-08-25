using HutongGames.PlayMaker;
using UnityEngine;

namespace Adrenaline
{
    internal class HighSpeedHandler : MonoBehaviour
    {
        public  CarData data;
        private Drivetrain drivetrain;
        private FsmString playerVehicle;

        private void OnEnable()
        {
            drivetrain = GetComponent<Drivetrain>();
            playerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");

            Utils.PrintDebug("HighSpeedHandler enabled");
        }

        private void FixedUpdate()
        {
            if (playerVehicle.Value != data.CarName) return;
            if (drivetrain.differentialSpeed > data.RequiredSpeed)
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.HIGHSPEED_INCREASE);
                Utils.PrintDebug("Value increased by driving on high speed");
            }
        }
    }
}
