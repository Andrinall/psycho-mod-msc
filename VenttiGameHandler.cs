using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class VenttiGameHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            try
            {
                var result = GameHook.InjectStateHook(base.gameObject, "Lose", VenttiWin);
                Utils.PrintDebug(eConsoleColors.GREEN, "VenttiGameHandler {0}", result ? "enabled" : "not loaded");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load VenttiGameHandler component");
            }
        }

        private void VenttiWin()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("VENTTI_WIN").Value);
            Utils.PrintDebug("Value increased by losing in ventti game");
        }
    }
}
