
using System;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class FirewoodJobHandler : CatchedComponent
    {
        protected override void Awaked()
        {
            Transform _payMoney = transform
                ?.Find("LOD/NPC/Char")
                ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                ?.Find("PayMoney");

            if (_payMoney == null)
                throw new NullReferenceException("PayMoney object not exists");

            StateHook.Inject(_payMoney.gameObject, "Use", "State 1", JobCompleted);
        }

        void JobCompleted() => Logic.PlayerCompleteJob("WOOD_DELIVERY");
    }
}
