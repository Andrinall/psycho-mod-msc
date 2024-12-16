using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class JokkeMovingJobHandler : CatchedComponent
    {
        GameObject _payMoney;


        public override void Awaked()
        {
            _payMoney = transform.Find("HitcherPivotNew/JokkeHiker1")?.Find("Pivot/Char")
                ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                ?.Find("PayMoney")?.gameObject;

            if (!_payMoney) return;
            StateHook.Inject(_payMoney, "Use", "Anim", JobCompleted);
        }

        void JobCompleted() => Logic.PlayerCompleteJob("YOKKE_RELOCATION");
    }
}
