using UnityEngine;

namespace Adrenaline
{
    internal class PissOnDevicesHandler : MonoBehaviour
    {
        private PlayMakerFSM FluidTrigger;

        private void OnEnable()
        {
            Utils.PrintDebug("PissOnDevicesHandler enabled");
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref FluidTrigger, "PLAYER/Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger", "Pouring");

            if (FluidTrigger?.ActiveStateName == "TV")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.PISS_ON_DEVICES);
                Utils.PrintDebug("Value increased by piss on TV and hit electricity");
            }
        }
    }
}
