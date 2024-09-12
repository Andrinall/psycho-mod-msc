using Harmony;
using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    internal class KiljuMurdererHandler : MonoBehaviour
    {
        private void Awake()
        {
            try
            {
                StateHook.Inject(
                    base.transform.Find("Pivot/Char").gameObject, "PlayerThreat", "Swing",
                    () => AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("MURDERER_THREAT"))
                );

                StateHook.Inject(
                    base.transform.Find("Pivot/Char/HumanCollider").gameObject, "PlayerHit", "State 3",
                    () => AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("MURDERER_HIT"))
                );
                
                Utils.PrintDebug(eConsoleColors.GREEN, "KiljuMurdererHandler component loaded & hooks installed");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"KiljuMurdererHandler not loaded\n{e.GetFullMessage()}");
            }
        }
    }
}
