using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class PissOnDevicesHandler : MonoBehaviour
    {
        private PlayMakerFSM FluidTrigger;

        private void OnEnable()
        {
            Utils.PrintDebug("PissOnDevicesHandler enabled");
            FluidTrigger = base.transform.Find("Piss/Fluid/FluidTrigger").GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Pouring");
        }

        private void FixedUpdate()
        {
            if (FluidTrigger?.ActiveStateName == "TV")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.PISS_ON_DEVICES);
                Utils.PrintDebug("Value increased by piss on TV and hit electricity");
            }
        }
    }
}
