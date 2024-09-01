using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class CarElectricityHandler : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                GameHook.InjectStateHook(base.gameObject, "Shock", "Reset", ShockHit);
                Utils.PrintDebug(eConsoleColors.GREEN, "CarElectricityHandler enabled");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load CarElecticityHandler component");
            }
        }

        private void ShockHit()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("SPARKS_WIRING"));
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased from shock hit by satsuma wiring");
        }
    }
}
