using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class HouseShitHandler : CatchedComponent
    {
        public override void Awaked() => StateHook.Inject(gameObject, "Use", "State 1", JobCompleted);

        void JobCompleted() => Logic.PlayerCompleteJob("SEPTIC_TANK");
    }
}
