using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class StoreActionsHandler : MonoBehaviour
    {
        private PlayMakerFSM TriggerPissOnTeimo;

        private GameObject GFX_Store;
        private GameObject GFX_Pub;
        private GameObject _Teimo;

        private void Start()
        {
            try
            {
                GFX_Store = base.transform.FindChild("LOD/GFX_Store").gameObject;
                GFX_Pub = base.transform.FindChild("LOD/GFX_Pub").gameObject;
                _Teimo = base.transform.FindChild("TeimoInShop").gameObject;

                var windowPub = GFX_Pub.transform.Find("BreakableWindowsPub/BreakableWindowPub")?.gameObject;
                var windowStore = GFX_Store.transform.Find("BreakableWindows/BreakableWindow")?.gameObject;

                if (windowPub != null)
                    StateHook.Inject(windowPub, "Shatter", BreakWindow);

                if (windowStore != null)
                    StateHook.Inject(windowStore, "Shatter", BreakWindow);

                StateHook.Inject(_Teimo.transform.Find("Pivot/Speak").gameObject, "Speak", "State 1", TeimoSwears);
                Utils.PrintDebug(eConsoleColors.GREEN, "StoreActionsHandler enabled");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load StoreActionsHandler component");
            }
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref TriggerPissOnTeimo, ref _Teimo, "Pivot/FacePissTrigger");

            if (TriggerPissOnTeimo?.ActiveStateName == "State 2")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("TEIMO_PISS"));
        }

        private void BreakWindow()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("WINDOW_BREAK_INCREASE"));
            Utils.PrintDebug(eConsoleColors.WHITE, "Adrenaline increased by shatter");
        }

        private void TeimoSwears()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("TEIMO_SWEAR"));
            Utils.PrintDebug(eConsoleColors.WHITE, "Adrenaline increased by teimo swears");
        }
    }
}
