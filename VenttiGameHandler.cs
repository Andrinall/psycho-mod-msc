using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class VenttiGameHandler : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                StateHook.Inject(base.gameObject, "Lose", VenttiWin);
                Utils.PrintDebug(eConsoleColors.GREEN, "VenttiGameHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load VenttiGameHandler component");
            }
        }

        private void VenttiWin()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("VENTTI_WIN"));
        }
    }
}
