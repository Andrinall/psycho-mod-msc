using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class CarElectricityHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            GameHook.InjectStateHook(base.transform.Find("FireElectric").gameObject, "Sparks", delegate
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.SPARKS_WIRING);
                Utils.PrintDebug("Value increased by hit from electricity into satsuma");
            });
            Utils.PrintDebug("CarElectricityHandler enabled");
        }
    }
}
