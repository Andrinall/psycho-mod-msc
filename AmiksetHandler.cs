using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class AmiksetHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            try
            {
                GameHook.InjectStateHook(base.transform.Find("KYLAJANI/Driver/Animations").gameObject, "Logic", "Hit", NPC_Hit_Player);
                GameHook.InjectStateHook(base.transform.FindChild("AMIS2/Passengers 3/Animations").gameObject, "Logic", "Hit", NPC_Hit_Player);

                Utils.PrintDebug(eConsoleColors.GREEN, "AmiksetHandler enabled");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load AmiksetHandler component");
            }
        }

        private void NPC_Hit_Player()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("JANNI_PETTERI_HIT").Value);
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by NPC hit player");
        }
    }
}
