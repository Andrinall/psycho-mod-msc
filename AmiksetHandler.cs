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
            Janni = base.transform.Find("KYLAJANI").gameObject;
            Petteri = base.transform.Find("AMIS2").gameObject;
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref JanniHit, ref Janni, "Driver/Animations", "Logic");
            Utils.CacheFSM(ref PetteriHit, ref Petteri, "Passengers 3/Animations", "Logic");

            if (JanniHit?.ActiveStateName == "Hit")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.JANNI_PETTERI_HIT);
                Utils.PrintDebug("Value increased by Janni hit player");
            }

            if (PetteriHit?.ActiveStateName == "Hit")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.JANNI_PETTERI_HIT);
                Utils.PrintDebug("Value increased by Petteri hit player");
            }
        }
    }
}
