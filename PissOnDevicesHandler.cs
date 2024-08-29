using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class PissOnDevicesHandler : MonoBehaviour
    {
        private PlayMakerFSM FluidTrigger;

        private void OnEnable()
        {
            try
            {
                FluidTrigger = base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger")
                    .GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Pouring");

                Utils.PrintDebug(eConsoleColors.GREEN, "PissOnDevicesHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load PissOnDevicesHandler enabled");
            }
        }

        private void FixedUpdate()
        {
            if (FluidTrigger?.ActiveStateName == "TV")
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.PISS_ON_DEVICES);
                Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by piss on TV and hit electricity");
            }
        }
    }
}
