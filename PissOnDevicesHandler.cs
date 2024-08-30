using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class PissOnDevicesHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            try
            {
                var result = GameHook.InjectStateHook(
                    base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger").gameObject,
                    "Pouring", "Wait", Pouring);

                Utils.PrintDebug(eConsoleColors.GREEN, "PissOnDevicesHandler {0}", result ? "enabled" : "failed setup");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load PissOnDevicesHandler enabled");
            }
        }

        private void Pouring()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("PISS_ON_DEVICES").Value);
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by piss on TV and hit electricity");
        }
    }
}
