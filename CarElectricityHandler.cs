using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class CarElectricityHandler : MonoBehaviour
    {
        private PlayMakerFSM FireElectric;

        private void OnEnable()
        {
            FireElectric = base.transform.Find("FireElectric").GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Init");
            Utils.PrintDebug("CarElectricityHandler enabled");
        }

        private void FixedUpdate()
        {
            if (FireElectric?.ActiveStateName == "Sparks")
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.SPARKS_WIRING);
                Utils.PrintDebug("Value increased by hit from electricity into satsuma");
            }
        }
    }
}
