using UnityEngine;

namespace Psycho
{
    public sealed class StoreActionsHandler : MonoBehaviour
    {
        Transform _GFX_Store;
        Transform _GFX_Pub;
        Transform _Teimo;

        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;

            _GFX_Store = transform.FindChild("LOD/GFX_Store");
            _GFX_Pub = transform.FindChild("LOD/GFX_Pub");
            _Teimo = transform.FindChild("TeimoInShop").Find("Pivot");
            GameObject windowPub = _GFX_Pub.Find("BreakableWindowsPub/BreakableWindowPub")?.gameObject;
            GameObject windowStore = _GFX_Store.Find("BreakableWindows/BreakableWindow")?.gameObject;

            if (windowPub != null)
                StateHook.Inject(windowPub, "Shatter", () => Logic.PlayerCommittedOffence("WINDOW_BREAK_INCREASE"));

            if (windowStore != null)
                StateHook.Inject(windowStore, "Shatter", () => Logic.PlayerCommittedOffence("WINDOW_BREAK_INCREASE"));

            StateHook.Inject(_Teimo.Find("Speak").gameObject, "Speak", "State 1", _ => Logic.PlayerCommittedOffence("TEIMO_SWEARS"));
            StateHook.Inject(_Teimo.Find("FacePissTrigger").gameObject, "Reaction", "State 2", _ => Logic.PlayerCommittedOffence("TEIMO_PISS"));
            StateHook.Inject(_Teimo.Find("TeimoCollider").gameObject, "Reaction", "State 1", _ => Logic.PlayerCommittedOffence("TEIMO_PISS"));

            // 10+ adv sended
            GameObject adv = transform.Find("LOD/ActivateStore/PayMoneyAdvert").gameObject;
            StateHook.Inject(adv, "Use", "Good", _ => Logic.PlayerCompleteJob("TEIMO_ADS"));
            StateHook.Inject(adv, "Use", "Average", _ => Logic.PlayerCompleteJob("TEIMO_ADS"));

            m_bInstalled = true;
        }
    }
}
