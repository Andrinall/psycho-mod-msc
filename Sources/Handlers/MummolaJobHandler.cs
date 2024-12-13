using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class MummolaJobHandler : CatchedComponent
    {
        internal override void Awaked()
            => StateHook.Inject(gameObject, "Use", "State 6", _ => Logic.PlayerCompleteJob("GRANNY_DELIVERY"));
    }
}