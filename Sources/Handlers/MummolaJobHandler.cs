using UnityEngine;

namespace Psycho
{
    [RequireComponent(typeof(PlayMakerFSM))]
    public sealed class MummolaJobHandler : MonoBehaviour
    {
        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;
            StateHook.Inject(gameObject, "Use", "State 6", () => Logic.PlayerCompleteJob("GRANNY_DELIVERY"));
            m_bInstalled = true;
        }
    }
}