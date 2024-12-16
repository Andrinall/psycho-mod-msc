using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class JokkeDropOffHandler : CatchedComponent
    {
        public override void Awaked()
            => StateHook.Inject(gameObject, "Use", "State 1", JobCompleted);

        void JobCompleted() => Logic.PlayerCompleteJob("YOKKE_DROPOFF");
    }
}
