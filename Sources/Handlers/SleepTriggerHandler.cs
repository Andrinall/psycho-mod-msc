using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Extensions;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class SleepTriggerHandler : CatchedComponent
    {
        FsmFloat m_fPlayerFatigue;

        public override void Awaked()
        {
            m_fPlayerFatigue = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue");

            transform.ClearActions("Activate", "Calc rates", 6);
            StateHook.Inject(gameObject, "Activate", "Calc rates", UpdateFatigue, -1);
        }

        void UpdateFatigue() => m_fPlayerFatigue.Value = Mathf.Clamp(m_fPlayerFatigue.Value - Logic.Value, 0f, 100f);
    }
}