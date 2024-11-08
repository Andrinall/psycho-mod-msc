using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class MummolaJobHandler : CatchedComponent
    {
        bool m_bInstalled = false;


        internal override void Awaked()
        {
            if (m_bInstalled) return;
            StateHook.Inject(gameObject, "Use", "State 6", _ => Logic.PlayerCompleteJob("GRANNY_DELIVERY"));
            m_bInstalled = true;
        }
    }
}