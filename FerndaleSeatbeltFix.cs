using HutongGames.PlayMaker;
using UnityEngine;

namespace Adrenaline
{
    internal class FerndaleSeatbeltFix : MonoBehaviour
    {
        private bool forUse = true;
        private FsmBool SeatbeltLocked;
        private Transform pivot;

        private void OnEnable()
        {
            SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
            pivot = base.transform.Find("LOD/DriverHeadPivot/CameraPivot/Pivot");
            Utils.PrintDebug("FerndaleSeatbeltFix enabled");
        }

        private void FixedUpdate()
        {
            if (pivot?.childCount > 0 && SeatbeltLocked?.Value == true && forUse)
            {
                SeatbeltLocked.Value = false;
                forUse = false;
            }
            else if (base.transform.childCount == 0 && forUse)
                forUse = true;
        }
    }
}
