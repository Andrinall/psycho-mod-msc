
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

        GameObject jonnez;
        GameObject boat;

        Collider selfColl;

        public bool CheckTimeOfDay = false;
        Vector3 ClosestPoint => selfColl.ClosestPointOnBounds(Globals.Player.position);

        protected override void Awaked()
        {
            localAmbientSource = GetComponent<AudioSource>();

            jonnez = GameObject.Find("JONNEZ ES(Clone)");
            boat = GameObject.Find("BOAT/GFX/Colliders/Collider");

            selfColl = GetComponent<Collider>();
        }

        protected override void OnFixedUpdate()
        {
            if (Vector3.Distance(ClosestPoint, Globals.Player.position) == 0)
            {
                Logic.CurrentAmbientTrigger = this;
                if (!CheckTimeOfDay)
                {
                    MuteAmbient(false);
                    return;
                }

                if (Globals.SUN_Hours.Value >= 22f || Globals.SUN_Hours.Value < 4f)
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
            return _obj == Globals.Player.gameObject || _obj == jonnez || _obj == boat;
        }
    }
}
