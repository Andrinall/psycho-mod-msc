
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class SleepTriggerHandler : CatchedComponent
    {
        FsmFloat playerFatigue;

        protected override void Awaked()
        {
            playerFatigue = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue");

            transform.ClearFsmActions("Activate", "Calc rates", 6);
            StateHook.Inject(gameObject, "Activate", "Calc rates", UpdateFatigue, -1);
        }

        void UpdateFatigue() => playerFatigue.Value = Mathf.Clamp(playerFatigue.Value - Logic.Value, 0f, 100f);
    }
}