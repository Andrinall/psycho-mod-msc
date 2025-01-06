
using System;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class JokkeMovingJobHandler : CatchedComponent
    {
        GameObject _payMoney;


        protected override void Awaked()
        {
            Transform _hitcherPivotNew = transform.Find("HitcherPivotNew");            
            if (_hitcherPivotNew.childCount == 0)
            {
                _payMoney = transform.Find("JokkeHiker1/Pivot/Char")
                    ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                    ?.Find("PayMoney")?.gameObject;
            }
            else
            {
                _payMoney = _hitcherPivotNew.Find("JokkeHiker1/Pivot/Char")
                    ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                    ?.Find("PayMoney")?.gameObject;
            }

            if (_payMoney == null)
                throw new NullReferenceException("PayMoney object not exists!");

            StateHook.Inject(_payMoney.gameObject, "Use", "Anim", JobCompleted);
        }

        void JobCompleted() => Logic.PlayerCompleteJob("YOKKE_RELOCATION");
    }
}
