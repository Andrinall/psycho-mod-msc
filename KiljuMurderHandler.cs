using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Adrenaline
{
    internal class KiljuMurderHandler : MonoBehaviour
    {
        private PlayMakerFSM KiljuMurder;

        private void OnEnable()
        {
            Utils.PrintDebug("KiljuMurderHandler enabled");
            KiljuMurder = base.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Move");
        }

        private void FixedUpdate()
        {
            if (KiljuMurder?.ActiveStateName == "Walking")
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.MURDER_WALKING);
                Utils.PrintDebug("Value increased a killer is coming for you");
            }
        }
    }
}
