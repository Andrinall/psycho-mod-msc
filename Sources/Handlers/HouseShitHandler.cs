using UnityEngine;

namespace Psycho
{
    [RequireComponent(typeof(PlayMakerFSM))]
    public sealed class HouseShitHandler : MonoBehaviour
    {
        bool m_bInstalled = false;

        void OnEnable()
        {
            if (m_bInstalled) return;
            StateHook.Inject(gameObject, "Use", "State 1", _ => Logic.PlayerCompleteJob("SEPTIC_TANK"));
            m_bInstalled = true;
        }
    }
}
