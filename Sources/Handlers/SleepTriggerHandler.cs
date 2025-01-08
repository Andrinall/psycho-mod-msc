
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class SleepTriggerHandler : CatchedComponent
    {
        protected override void Awaked()
        {
            transform.ClearFsmActions("Activate", "Calc rates", 6);
            StateHook.Inject(gameObject, "Activate", "Calc rates", UpdateFatigue, -1);
        }

        void UpdateFatigue()
            => Globals.PlayerFatigue.Value = Mathf.Clamp(Globals.PlayerFatigue.Value - Logic.Value, 0f, 100f);
    }
}