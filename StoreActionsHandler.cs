using UnityEngine;

namespace Adrenaline
{
    internal class StoreActionsHandler : MonoBehaviour
    {
        private PlayMakerFSM StoreWindow;
        private PlayMakerFSM PubWindow;
        private PlayMakerFSM TriggerPissOnTeimo;
        private PlayMakerFSM TeimoSwears;

        private GameObject GFX_Store;
        private GameObject GFX_Pub;
        private GameObject _Teimo;

        private void OnEnable()
        {
            try
            {
                GFX_Store = base.transform.FindChild("LOD/GFX_Store").gameObject;
                GFX_Pub = base.transform.FindChild("LOD/GFX_Pub").gameObject;
                _Teimo = base.transform.FindChild("TeimoInShop").gameObject;
                Utils.PrintDebug(eConsoleColors.GREEN, "StoreActionsHandler enabled");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load StoreActionsHandler component");
            }
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref PubWindow, ref GFX_Pub, "BreakableWindowsPub/BreakableWindowPub", "Break");
            Utils.CacheFSM(ref TeimoSwears, ref _Teimo, "Pivot/Speak", "Speak");
            Utils.CacheFSM(ref StoreWindow, ref GFX_Store, "BreakableWindows/BreakableWindow", "Break");
            Utils.CacheFSM(ref TriggerPissOnTeimo, ref _Teimo, "Pivot/FacePissTrigger");

            if (StoreWindow?.ActiveStateName == "Shatter")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.WINDOW_BREAK_INCREASE);

            if (PubWindow?.ActiveStateName == "Shatter")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.WINDOW_BREAK_INCREASE);

            if (TriggerPissOnTeimo?.ActiveStateName == "State 2")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.TEIMO_PISS);

            if (TeimoSwears?.ActiveStateName == "State 1")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.TEIMO_SWEAR);
        }
    }
}
