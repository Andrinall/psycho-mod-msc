using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class CarElectricityHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            try
            {
                var fireel = base.transform.Find("Wiring").Find("FireElectric");
                GameHook.InjectStateHook(fireel.gameObject, "Init", "Sparks", delegate
                {
                    AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("SPARKS_WIRING").Value);
                    Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by hit from electricity into satsuma");
                });
                
                Utils.PrintDebug(eConsoleColors.GREEN, "CarElectricityHandler enabled");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load CarElecticityHandler component");
            }
        }
    }
}
