using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class JokkeMovingJobHandler : CatchedComponent
    {
        GameObject _payMoney;
        bool m_bInstalled = false;

        internal override void Awaked()
        {
            _payMoney = transform.Find("HitcherPivotNew/JokkeHiker1")?.Find("Pivot/Char")
                ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                ?.Find("PayMoney")?.gameObject;

            if (!_payMoney) return;
            StateHook.Inject(_payMoney, "Use", "Anim", _ => Logic.PlayerCompleteJob("YOKKE_RELOCATION"));
        }

        /*void OnEnable()
            => _findObject();

        void FixedUpdate()
        {
            if (_payMoney?.transform == null)
            {
                _findObject();
                return;
            }

            if (_payMoney?.activeSelf == true && !m_bInstalled)
            {
                StateHook.Inject(_payMoney, "Use", "Anim", _ => Logic.PlayerCompleteJob("YOKKE_RELOCATION"));
                m_bInstalled = true;
            }
        }

        void _findObject() =>
            _payMoney = transform.Find("HitcherPivotNew/JokkeHiker1")?.Find("Pivot/Char")
                ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                ?.Find("PayMoney")?.gameObject;*/
    }
}
