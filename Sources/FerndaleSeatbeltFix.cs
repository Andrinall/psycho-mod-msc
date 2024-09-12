using System.Linq;

using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class FerndaleSeatbeltFix : MonoBehaviour
    {
        private FsmBool SeatbeltLocked;
        private FsmBool IsBukled;
        private FsmString PlayerCurrentVehicle;

        private void Start()
        {
            try
            {
                PlayerCurrentVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
                SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
                IsBukled = base.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Use").FsmVariables.GetFsmBool("IsBuckled");
                Utils.PrintDebug(eConsoleColors.GREEN, "FerndaleSeatbeltFix enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load FerndaleSeatbeltFix component");
            }
        }

        private void FixedUpdate()
        {
            if (PlayerCurrentVehicle.Value == "Ferndale" && !IsBukled.Value && SeatbeltLocked.Value)
                SeatbeltLocked.Value = false;
        }
    }
}
