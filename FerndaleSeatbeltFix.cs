using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class FerndaleSeatbeltFix : MonoBehaviour
    {
        private bool forUse = true;
        private FsmBool SeatbeltLocked;
        private Transform pivot;

        private void OnEnable()
        {
            try
            {
                SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
                pivot = base.transform.Find("LOD/DriverHeadPivot/CameraPivot/Pivot");
                Utils.PrintDebug(eConsoleColors.GREEN, "FerndaleSeatbeltFix enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load FerndaleSeatbeltFix component");
            }
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
