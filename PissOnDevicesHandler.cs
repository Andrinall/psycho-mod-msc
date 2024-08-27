using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class PissOnDevicesHandler : MonoBehaviour
    {
        private PlayMakerFSM FluidTrigger;

        private void OnEnable()
        {
            FluidTrigger = base.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Pouring");
            Utils.PrintDebug("PissOnDevicesHandler enabled");
        }

        private void FixedUpdate()
        {
            if (FluidTrigger?.ActiveStateName == "TV")
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.PISS_ON_DEVICES);
                Utils.PrintDebug("Value increased by piss on TV and hit electricity");
            }
        }
    }
}
