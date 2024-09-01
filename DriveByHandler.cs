using Harmony;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Adrenaline
{
    internal class DriveByHandler : MonoBehaviour
    {
        private FsmString PlayerVehicle;
        private bool hitted;

        private void Start()
        {
            PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
        }

        private void OnDisable()
        {
            hitted = false;
        }

        private void OnTriggerEnter()
        {
            if (PlayerVehicle.Value == "") return;
            if (hitted) return;

            hitted = true;
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("DRIVEBY_INCREASE"));
            Utils.PrintDebug("DriveBy NPC hit; Value increased");
        }
    }
}
