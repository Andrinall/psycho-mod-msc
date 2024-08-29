using UnityEngine;

namespace Adrenaline
{
    internal class VenttiGameHandler : MonoBehaviour
    {
        private PlayMakerFSM VenttiGame;

        private void OnEnable()
        {
            try
            {
                GameHook.InjectStateHook(base.gameObject, "Lose", delegate
                {
                    AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.VENTTI_WIN);
                    Utils.PrintDebug("Value increased by losing in ventti game");
                });
                // VenttiGame = base.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Use");
                Utils.PrintDebug(eConsoleColors.GREEN, "VenttiGameHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load VenttiGameHandler component");
            }
        }
    }
}
