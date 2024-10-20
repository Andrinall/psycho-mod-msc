using UnityEngine;

namespace Psycho
{
    public sealed class HouseShitHandler : MonoBehaviour
    {
        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;

            StateHook.Inject(gameObject, "Use", "State 1", () => Logic.PlayerCompleteJob("SEPTIC_TANK"));

            Utils.PrintDebug(eConsoleColors.GREEN, "HouseShitHandler enabled");
            m_bInstalled = true;
        }
    }
}
