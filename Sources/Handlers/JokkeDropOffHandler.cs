using UnityEngine;

namespace Psycho
{
    public sealed class JokkeDropOffHandler : MonoBehaviour
    {
        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;

            StateHook.Inject(gameObject, "Use", "State 1", () => Logic.PlayerCompleteJob("YOKKE_DROPOFF"));

            Utils.PrintDebug(eConsoleColors.GREEN, "JokkeHiker2::DropOffJob handler enabled");
            m_bInstalled = true;
        }
    }
}
