using UnityEngine;

namespace Adrenaline
{
    internal class StoreActionsHandler : MonoBehaviour
    {
        private PlayMakerFSM StoreWindow;
        private PlayMakerFSM PubWindow;
        private PlayMakerFSM TriggerPissOnTeimo;
        private PlayMakerFSM TeimoSwears;

        private void OnEnable()
        {
            Utils.PrintDebug("StoreActionsHandler enabled");
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref PubWindow,   "STORE/LOD/GFX_Pub/BreakableWindowsPub/BreakableWindowPub", "Break");
            Utils.CacheFSM(ref TeimoSwears, "STORE/TeimoInShop/Pivot/Speak", "Speak");
            Utils.CacheFSM(ref StoreWindow, "STORE/LOD/GFX_Store/BreakableWindows/BreakableWindow", "Break");
            Utils.CacheFSM(ref TriggerPissOnTeimo, "STORE/TeimoInShop/Pivot/FacePissTrigger");

            if (StoreWindow?.ActiveStateName == "Shatter")
            {
                AdrenalineLogic.IncreaseTimed(Configuration.WINDOW_BREAK_INCREASE);
                Utils.PrintDebug("Value timed increased by broke store window");
            }

            if (PubWindow?.ActiveStateName == "Shatter")
            {
                AdrenalineLogic.IncreaseTimed(Configuration.WINDOW_BREAK_INCREASE);
                Utils.PrintDebug("Value timed increased by broke pub window");
            }

            if (TriggerPissOnTeimo?.ActiveStateName == "State 2")
            {
                AdrenalineLogic.IncreaseTimed(Configuration.TEIMO_PISS);
                Utils.PrintDebug("Value timed increased by piss on teimo's face");
            }

            if (TeimoSwears?.ActiveStateName == "State 1")
            {
                AdrenalineLogic.IncreaseTimed(Configuration.TEIMO_SWEAR);
                Utils.PrintDebug("Value timed increased by teimo is swears");
            }
        }
    }
}
