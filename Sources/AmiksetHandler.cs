using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class AmiksetHandler : MonoBehaviour
    {
        private GameObject Anim;

        private void OnEnable()
        {
            try
            {
                StateHook.Inject(base.gameObject, "Logic", "Hit", NPC_Hit_Player);

                Utils.PrintDebug(eConsoleColors.GREEN, $"AmiksetHandler enabled for {base.gameObject.name}");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Error while loading AmiksetHandler (OnEnable)");
            }
        }

        private void NPC_Hit_Player()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("JANNI_PETTERI_HIT"));
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by NPC hit player");
        }
    }
}
