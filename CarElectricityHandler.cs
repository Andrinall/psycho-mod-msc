using UnityEngine;

namespace Adrenaline
{
    internal class CarElectricityHandler : MonoBehaviour
    {
        private PlayMakerFSM FireElectric;

        private void OnEnable()
        {
            FireElectric = base.GetComponent<PlayMakerFSM>();
            Utils.PrintDebug("CarElectricityHandler: " + FireElectric.FsmName);
        }

        private void FixedUpdate()
        {
            if (FireElectric?.ActiveStateName == "Sparks")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.SPARKS_WIRING);
                Utils.PrintDebug("Value increased by hit from electricity into satsuma");
            }
        }
    }
}
