using MSCLoader;
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

            try
            {
                _GFX_Store = transform.FindChild("LOD/GFX_Store");
                _GFX_Pub = transform.FindChild("LOD/GFX_Pub");
                _Teimo = transform.FindChild("TeimoInShop").Find("Pivot");
                var windowPub = _GFX_Pub.Find("BreakableWindowsPub/BreakableWindowPub")?.gameObject;
                var windowStore = _GFX_Store.Find("BreakableWindows/BreakableWindow")?.gameObject;

                if (windowPub != null)
                    StateHook.Inject(windowPub, "Shatter", () => Logic.PlayerCommittedOffence("WINDOW_BREAK_INCREASE"));

                if (windowStore != null)
                    StateHook.Inject(windowStore, "Shatter", () => Logic.PlayerCommittedOffence("WINDOW_BREAK_INCREASE"));

                StateHook.Inject(_Teimo.Find("Speak").gameObject, "Speak", "State 1", () => Logic.PlayerCommittedOffence("TEIMO_SWEARS"));
                StateHook.Inject(_Teimo.Find("FacePissTrigger").gameObject, "Reaction", "State 2", () => Logic.PlayerCommittedOffence("TEIMO_PISS"));
                StateHook.Inject(_Teimo.Find("TeimoCollider").gameObject, "Reaction", "State 1", () => Logic.PlayerCommittedOffence("TEIMO_PISS"));

                // 10+ adv sended
                var adv = transform.Find("LOD/ActivateStore/PayMoneyAdvert").gameObject;
                StateHook.Inject(adv, "Use", "Good", () => Logic.PlayerCompleteJob("TEIMO_ADS"));
                StateHook.Inject(adv, "Use", "Average", () => Logic.PlayerCompleteJob("TEIMO_ADS"));
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Unable to load StoreActionsHandler component\n{e.GetFullMessage()}");
            }

            m_bInstalled = true;
        }
    }
}
