using Harmony;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class DriveByHandler : MonoBehaviour
    {
        private Transform player;
        private FsmString PlayerVehicle;
        private bool hitted;

        private void Start()
        {
            PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
            player = GameObject.Find("PLAYER").transform;
        }

        private void OnDisable()
        {
            hitted = false;
        }

        private void OnTriggerEnter()
        {
            if (hitted) return;
            if (PlayerVehicle.Value == "") return;
            if (Vector3.Distance(base.transform.position, player.position) > 2f) return;

            hitted = true;
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("DRIVEBY_INCREASE"));
            Utils.PrintDebug("DriveBy NPC hit; Value increased");
        }
    }
}
