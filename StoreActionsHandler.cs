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
            Utils.PrintDebug("StoreActionsHandler enabled");
            GFX_Store = base.transform.Find("LOD/GFX_Store").gameObject;
            GFX_Pub = base.transform.Find("LOD/GFX_Pub").gameObject;
            _Teimo = base.transform.Find("TeimoInShop").gameObject;
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref PubWindow, ref GFX_Pub, "BreakableWindowsPub/BreakableWindowPub", "Break");
            Utils.CacheFSM(ref TeimoSwears, ref _Teimo, "Pivot/Speak", "Speak");
            Utils.CacheFSM(ref StoreWindow, ref GFX_Store, "BreakableWindows/BreakableWindow", "Break");
            Utils.CacheFSM(ref TriggerPissOnTeimo, ref _Teimo, "Pivot/FacePissTrigger");

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
