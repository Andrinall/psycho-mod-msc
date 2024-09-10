using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class AmiksetHandler : MonoBehaviour
    {
        private bool installed = false;
        private bool activated = false;
        private GameObject Anim1;
        private GameObject Anim2;

        private void Start()
        {
            Anim1 = base.transform.Find("KYLAJANI/Driver/Animations").gameObject;
            Anim2 = base.transform.Find("AMIS2/Passengers 3/Animations").gameObject;
            
            if (!base.gameObject.activeSelf)
            {
                base.gameObject.SetActive(true);
                activated = true;
            }
        }

        private void OnEnable()
        {
            if (installed) return;

            StateHook.Inject(Anim1, "Logic", "Hit", NPC_Hit_Player);
            StateHook.Inject(Anim2, "Logic", "Hit", NPC_Hit_Player);
            installed = true;

            if (activated) base.gameObject.SetActive(false);
            Utils.PrintDebug(eConsoleColors.GREEN, "AmiksetHandler enabled");
        }

        private void NPC_Hit_Player()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("JANNI_PETTERI_HIT"));
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by NPC hit player");
        }
    }
}
