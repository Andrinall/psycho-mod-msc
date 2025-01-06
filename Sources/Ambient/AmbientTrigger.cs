
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

        Transform player;
        GameObject jonnez;
        GameObject boat;

        Collider selfColl;

        FsmFloat SUN_Hours;

        public bool CheckTimeOfDay = false;


        protected override void Awaked()
        {
            localAmbientSource = GetComponent<AudioSource>();
            globalAmbientSource = null;
            globalPhychoAmbientSource = null;

            player = GameObject.Find("PLAYER").transform;
            jonnez = GameObject.Find("JONNEZ ES(Clone)");
            boat = GameObject.Find("BOAT/GFX/Colliders/Collider");

            SUN_Hours = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock").GetVariable<FsmFloat>("Hours");

            selfColl = GetComponent<Collider>();
        }

        protected override void OnFixedUpdate()
        {
            Vector3 closestPoint = selfColl.ClosestPointOnBounds(player.transform.position);

            if (Vector3.Distance(closestPoint, player.position) == 0)
            {
                Logic.CurrentAmbientTrigger = this;
                if (!CheckTimeOfDay)
                {
                    MuteAmbient(false);
                    return;
                }

                if (SUN_Hours.Value >= 22f || SUN_Hours.Value < 4f)
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
            GameObject obj = other?.gameObject;
            if (obj == null) return false;
            return obj == player.gameObject || obj == jonnez || obj == boat;
        }
    }
}
