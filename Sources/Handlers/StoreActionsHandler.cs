using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class StoreActionsHandler : CatchedComponent
    {
        Transform _GFX_Store;
        Transform _GFX_Pub;
        Transform _Teimo;


        public override void Awaked()
        { 
            _GFX_Store = transform.FindChild("LOD/GFX_Store");
            _GFX_Pub = transform.FindChild("LOD/GFX_Pub");
            _Teimo = transform.FindChild("TeimoInShop").Find("Pivot");
            GameObject windowPub = _GFX_Pub.Find("BreakableWindowsPub/BreakableWindowPub")?.gameObject;
            GameObject windowStore = _GFX_Store.Find("BreakableWindows/BreakableWindow")?.gameObject;

            if (windowPub != null)
                StateHook.Inject(windowPub, "Shatter", WindowBreaked);

            if (windowStore != null)
                StateHook.Inject(windowStore, "Shatter", WindowBreaked);

            StateHook.Inject(_Teimo.Find("Speak").gameObject, "Speak", "State 1", TeimoSwears);
            StateHook.Inject(_Teimo.Find("FacePissTrigger").gameObject, "Reaction", "State 2", PissedOnTeimo);
            StateHook.Inject(_Teimo.Find("TeimoCollider").gameObject, "Reaction", "State 1", PissedOnTeimo);

            // 10+ adv sended
            GameObject adv = transform.Find("LOD/ActivateStore/PayMoneyAdvert").gameObject;
            StateHook.Inject(adv, "Use", "Good", AdsJobCompleted);
            StateHook.Inject(adv, "Use", "Average", AdsJobCompleted);
        }

        void WindowBreaked() => Logic.PlayerCommittedOffence("WINDOW_BREAK");
        void TeimoSwears() => Logic.PlayerCommittedOffence("TEIMO_SWEARS");
        void PissedOnTeimo() => Logic.PlayerCommittedOffence("TEIMO_PISS");
        void AdsJobCompleted() => Logic.PlayerCompleteJob("TEIMO_ADS");
    }
}
