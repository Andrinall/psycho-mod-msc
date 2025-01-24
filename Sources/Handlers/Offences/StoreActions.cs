
using UnityEngine;

using Psycho.Internal;

namespace Psycho.Handlers
{
    class StoreActions : CatchedComponent
    {
        protected override void Awaked()
        { 
            Transform gfxStore = transform.FindChild("LOD/GFX_Store");
            Transform gfxPub = transform.FindChild("LOD/GFX_Pub");
            Transform teimo = transform.FindChild("TeimoInShop").Find("Pivot");

            GameObject _windowPub = gfxPub.Find("BreakableWindowsPub/BreakableWindowPub")?.gameObject;
            GameObject _windowStore = gfxStore.Find("BreakableWindows/BreakableWindow")?.gameObject;

            if (_windowPub != null)
                StateHook.Inject(_windowPub, "Shatter", WindowBreaked);

            if (_windowStore != null)
                StateHook.Inject(_windowStore, "Shatter", WindowBreaked);

            StateHook.Inject(teimo.Find("Speak").gameObject, "Speak", "State 1", TeimoSwears);
            StateHook.Inject(teimo.Find("FacePissTrigger").gameObject, "Reaction", "State 2", PissedOnTeimo);
            StateHook.Inject(teimo.Find("TeimoCollider").gameObject, "Reaction", "State 1", PissedOnTeimo);

            // 10+ adv sended
            GameObject _adv = transform.Find("LOD/ActivateStore/PayMoneyAdvert").gameObject;
            StateHook.Inject(_adv, "Use", "Good", AdsJobCompleted);
            StateHook.Inject(_adv, "Use", "Average", AdsJobCompleted);
        }

        void WindowBreaked()
            => Logic.PlayerCommittedOffence("WINDOW_BREAK");

        void TeimoSwears()
            => Logic.PlayerCommittedOffence("TEIMO_SWEARS");

        void PissedOnTeimo()
            => Logic.PlayerCommittedOffence("TEIMO_PISS");

        void AdsJobCompleted()
            => Logic.PlayerCompleteJob("TEIMO_ADS");
    }
}
