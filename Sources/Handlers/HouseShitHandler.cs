using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class HouseShitHandler : CatchedComponent
    {
        internal override void Awaked()
            => StateHook.Inject(gameObject, "Use", "State 1", _ => Logic.PlayerCompleteJob("SEPTIC_TANK"));
    }
}
