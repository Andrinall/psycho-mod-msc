using UnityEngine;

namespace Adrenaline
{
    internal class AmiksetHandler : MonoBehaviour
    {
        private GameObject Janni;
        private GameObject Petteri;

        private PlayMakerFSM JanniHit;
        private PlayMakerFSM PetteriHit;

        private void OnEnable()
        {
            try
            {
                Janni = base.transform.FindChild("KYLAJANI")?.gameObject;
                Petteri = base.transform.FindChild("AMIS2")?.gameObject;
                Utils.PrintDebug(eConsoleColors.GREEN, "AmiksetHandler enabled");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load AmiksetHandler component");
            }
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref JanniHit, ref Janni, "Driver/Animations", "Logic");
            Utils.CacheFSM(ref PetteriHit, ref Petteri, "Passengers 3/Animations", "Logic");

            if (JanniHit?.ActiveStateName == "Hit")
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.JANNI_PETTERI_HIT);
                Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by Janni hit player");
            }

            if (PetteriHit?.ActiveStateName == "Hit")
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.JANNI_PETTERI_HIT);
                Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by Petteri hit player");
            }
        }
    }
}
