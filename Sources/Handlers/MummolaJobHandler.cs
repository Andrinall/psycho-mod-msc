using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class MummolaJobHandler : CatchedComponent
    {
        public override void Awaked() => StateHook.Inject(gameObject, "Use", "State 6", JobCompleted);

        void JobCompleted() => Logic.PlayerCompleteJob("GRANNY_DELIVERY");
    }
}