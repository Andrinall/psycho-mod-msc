using Harmony;
using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    internal class StoreActionsHandler : MonoBehaviour
    {
        private PlayMakerFSM TriggerPissOnTeimo;

        private Transform GFX_Store;
        private Transform GFX_Pub;
        private Transform _Teimo;

        private void Start()
        {
            try
            {
                GFX_Store = base.transform.FindChild("LOD/GFX_Store");
                GFX_Pub = base.transform.FindChild("LOD/GFX_Pub");
                _Teimo = base.transform.FindChild("TeimoInShop");

                var windowPub = GFX_Pub.Find("BreakableWindowsPub/BreakableWindowPub")?.gameObject;
                var windowStore = GFX_Store.Find("BreakableWindows/BreakableWindow")?.gameObject;

                if (windowPub != null)
                    StateHook.Inject(windowPub, "Shatter", BreakWindow);

                if (windowStore != null)
                    StateHook.Inject(windowStore, "Shatter", BreakWindow);

                StateHook.Inject(_Teimo.transform.Find("Pivot/Speak").gameObject, "Speak", "State 1", TeimoSwears);
                Utils.PrintDebug(eConsoleColors.GREEN, "StoreActionsHandler enabled");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to load StoreActionsHandler component\n{e.GetFullMessage()}");
            }
        }

        private void FixedUpdate()
        {
            try
            {
                if (TriggerPissOnTeimo == null)
                {
                    var obj = _Teimo.Find("Pivot/FacePissTrigger");
                    if (obj == null || obj.gameObject == null) return;
                    TriggerPissOnTeimo = obj.gameObject.GetPlayMaker("Reaction");
                }
            } catch {}

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
