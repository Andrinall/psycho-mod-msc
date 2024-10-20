using UnityEngine;

namespace Psycho
{
    public sealed class JokkeMovingJobHandler : MonoBehaviour
    {
        GameObject _payMoney;
        bool m_bInstalled = false;


        void OnEnable() => _findObject();

        void FixedUpdate()
        {
            if (_payMoney?.activeSelf == true && !m_bInstalled)
            {
                StateHook.Inject(_payMoney, "Use", "Anim", () => Logic.PlayerCompleteJob("YOKKE_RELOCATION"));
                Utils.PrintDebug(eConsoleColors.WHITE, "JokkeHiker1::MovingJob handler enabled");
                m_bInstalled = true;
            }

            if (_payMoney == null) _findObject();
        }

        void _findObject() =>
            _payMoney = transform.Find("HitcherPivotNew/JokkeHiker1")?.Find("Pivot/Char")
                ?.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right")
                ?.Find("PayMoney")?.gameObject;
        
    }
}
