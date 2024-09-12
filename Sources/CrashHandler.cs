using Harmony;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class CrashHandler : MonoBehaviour
    {
        private Rigidbody thisRb;
        private GameObject death;
        private FsmString PlayerVehicle;

        private Vector3 velo;
        private float cooldown = 0f;
        private string upName;


        private void Start()
        {
            thisRb = GetComponent<Rigidbody>();
            death = GameObject.Find("Systems/Death");
            name = base.gameObject.name.ToUpper();
            PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
        }
        
        private void OnCollisionEnter(Collision col)
        {
            if (PlayerVehicle.Value == "") return;
            if (death?.activeSelf == true || cooldown > 0f) return;

            float num = Vector3.Distance(thisRb.velocity, velo);
            if (num < GetCrashMin()) return;
            
            cooldown = num;
            if (upName.Contains(PlayerVehicle.Value.ToUpper()))
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("CRASH_INCREASE"));
                Utils.PrintDebug("Value increased by crash on vehicle " + PlayerVehicle.Value);
            }
        }

        private void FixedUpdate()
        {
            velo = thisRb.velocity;
            if (cooldown > 0f)
                cooldown -= 0.05f;
        }

        private float GetCrashMin()
        {
            return AdrenalineLogic.config.GetValueSafe("REQUIRED_CRASH_SPEED") * 5f / 18f;
        }
    }
}
