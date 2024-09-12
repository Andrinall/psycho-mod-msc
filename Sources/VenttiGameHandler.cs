using Harmony;
using MSCLoader;
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
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to load VenttiGameHandler component\n{e.GetFullMessage()}");
            }
        }

        private void VenttiWin()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("VENTTI_WIN"));
        }
    }
}
