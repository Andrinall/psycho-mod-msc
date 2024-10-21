using UnityEngine;
using HutongGames.PlayMaker;

namespace Psycho
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class CrashHandler : MonoBehaviour
    {
        Rigidbody _thisRb;
        GameObject _death;

        FsmString m_sPlayerVehicle;
        
        Vector3 m_vVelo;
        float m_fCooldown = 0f;
        string m_sUpName;


        void OnEnable()
        {
            _thisRb = GetComponent<Rigidbody>();
            _death = GameObject.Find("Systems/Death");
            m_sUpName = gameObject.name.ToUpper();
            m_sPlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");

            Utils.PrintDebug(eConsoleColors.GREEN, $"CrashHandler enabled for car: {m_sUpName}");
        }

        void OnCollisionEnter(Collision col)
        {
            if (m_sPlayerVehicle.Value == "") return;
            if (_death?.activeSelf == true || m_fCooldown > 0f) return;

            float num = Vector3.Distance(_thisRb.velocity, m_vVelo);
            if (num < 8.3f) return;

            m_fCooldown = num;
            if (m_sUpName.Contains(m_sPlayerVehicle.Value.ToUpper()))
                Logic.ResetValue();
        }

        void FixedUpdate()
        {
            m_vVelo = _thisRb.velocity;
            if (m_fCooldown > 0f)
                m_fCooldown -= 0.05f;
        }
    }
}