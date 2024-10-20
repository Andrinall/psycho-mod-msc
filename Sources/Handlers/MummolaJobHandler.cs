using UnityEngine;

namespace Psycho
{
    public sealed class MummolaJobHandler : MonoBehaviour
    {
        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;

            StateHook.Inject(gameObject, "Use", "State 6", () => Logic.PlayerCompleteJob("GRANNY_DELIVERY"));
            Utils.PrintDebug(eConsoleColors.GREEN, "MummolaJobHandler enabled");
            m_bInstalled = true;
        }
    }
}