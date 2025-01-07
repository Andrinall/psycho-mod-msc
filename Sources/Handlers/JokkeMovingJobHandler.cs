
using System;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class JokkeMovingJobHandler : CatchedComponent
    {
        GameObject payMoney;


        protected override void Awaked()
        {
            Transform _hitcherPivotNew = transform.Find("HitcherPivotNew");            
            if (_hitcherPivotNew.childCount == 0)
            {
                payMoney = transform.Find("JokkeHiker1/Pivot/Char")
                    ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                    ?.Find("PayMoney")?.gameObject;
            }
            else
            {
                payMoney = _hitcherPivotNew.Find("JokkeHiker1/Pivot/Char")
                    ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                    ?.Find("PayMoney")?.gameObject;
            }

            if (payMoney == null)
                throw new NullReferenceException("PayMoney object not exists!");

            StateHook.Inject(payMoney.gameObject, "Use", "Anim", JobCompleted);
        }

        void JobCompleted() => Logic.PlayerCompleteJob("YOKKE_RELOCATION");
    }
}
