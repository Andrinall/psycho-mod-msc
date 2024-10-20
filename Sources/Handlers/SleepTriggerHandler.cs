using UnityEngine;
using HutongGames.PlayMaker;

namespace Psycho
{
    public sealed class SleepTriggerHandler : MonoBehaviour
    {
        FsmFloat m_fPlayerFatigue;
        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;

            m_fPlayerFatigue = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue");

            Utils.ClearActions(transform, "Activate", "Calc rates", 6);
            StateHook.Inject(gameObject, "Activate", "Calc rates", -1, () =>
                m_fPlayerFatigue.Value = Mathf.Clamp(m_fPlayerFatigue.Value - Logic.Value, 0f, 100f));

            m_bInstalled = true;
        }
    }
}