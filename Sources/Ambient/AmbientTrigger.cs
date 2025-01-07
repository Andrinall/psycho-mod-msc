
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Ambient
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Collider))]
    public class AmbientTrigger : CatchedComponent
    {
        AudioSource localAmbientSource;
        AudioSource globalAmbientSource;
        AudioSource globalPhychoAmbientSource;

        GameObject jonnez;
        GameObject boat;

        Collider selfColl;

        public bool CheckTimeOfDay = false;
        Vector3 ClosestPoint => selfColl.ClosestPointOnBounds(Psycho.Player.position);

        protected override void Awaked()
        {
            localAmbientSource = GetComponent<AudioSource>();
            globalAmbientSource = null;
            globalPhychoAmbientSource = null;

            jonnez = GameObject.Find("JONNEZ ES(Clone)");
            boat = GameObject.Find("BOAT/GFX/Colliders/Collider");

            selfColl = GetComponent<Collider>();
        }

        protected override void OnFixedUpdate()
        {
            if (Vector3.Distance(ClosestPoint, Psycho.Player.position) == 0)
            {
                Logic.CurrentAmbientTrigger = this;
                if (!CheckTimeOfDay)
                {
                    MuteAmbient(false);
                    return;
                }

                if (Psycho.SUN_hours.Value >= 22f || Psycho.SUN_hours.Value < 4f)
                {
                    MuteAmbient(false);
                    return;
                }

                MuteAmbient(true);
                return;
            }
            else
            {
                if (Logic.CurrentAmbientTrigger != this) return;
                
                MuteAmbient(true);
                Logic.CurrentAmbientTrigger = null;
            }
        }
        public void MuteAmbient(bool state)
        {
            SoundManager.MuteSource(localAmbientSource, state);
            SoundManager.MuteGlobalAmbient(!state);

        }

        bool _isNeededObject(Collider other)
        {
            GameObject _obj = other?.gameObject;
            if (_obj == null) return false;
            return _obj == Psycho.Player.gameObject || _obj == jonnez || _obj == boat;
        }
    }
}
