using HutongGames.PlayMaker;
using UnityEngine;

namespace Adrenaline
{
    internal class FerndaleSeatbeltFix : MonoBehaviour
    {
        private bool forUse = true;
        private FsmBool SeatbeltLocked;

        private void OnEnable()
        {
            SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
            Utils.PrintDebug("FerndaleSeatbeltFix enabled");
        }

        private void FixedUpdate()
        {
            if (base.transform.childCount > 0 && SeatbeltLocked.Value && forUse)
            {
                SeatbeltLocked.Value = false;
                forUse = false;
            }
            else if (base.transform.childCount == 0 && forUse)
                forUse = true;
        }
    }
}
