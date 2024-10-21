using UnityEngine;

namespace Psycho
{
    [RequireComponent(typeof(PlayMakerFSM))]
    public sealed class JokkeDropOffHandler : MonoBehaviour
    {
        bool m_bInstalled = false;

        void OnEnable()
        {
            if (m_bInstalled) return;
            StateHook.Inject(gameObject, "Use", "State 1", () => Logic.PlayerCompleteJob("YOKKE_DROPOFF"));
            m_bInstalled = true;
        }
    }
}
