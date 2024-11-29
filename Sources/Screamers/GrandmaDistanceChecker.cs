using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    [RequireComponent(typeof(AudioSource))]
    internal sealed class GrandmaDistanceChecker : MonoBehaviour
    {
        Transform _player;
        AudioSource audio;
        bool m_bBlowed = false;
        float Distance = 3.5f;


        void Awake()
        {
            _player = GameObject.Find("PLAYER").transform;
            audio = transform.GetComponent<AudioSource>();
        }

        void OnEnable()
            => WorldManager.ShowCrows(false);
        
        void OnDestroy()
            => WorldManager.ShowCrows(true);

        void FixedUpdate()
        {
            if (m_bBlowed) return;
            if (Vector3.Distance(transform.position, _player.position) > Distance) return;
            BlowUpGrandmaAndReset();
        }

        public void BlowUpGrandmaAndReset()
        {
            GameObject smokes = Instantiate(Globals.SmokeParticleSystem_prefab);
            smokes.transform.position = transform.position;
            smokes.transform.localScale = new Vector3(0.0075f, 0.0075f, 0.0075f);

            audio?.Play();
            Globals.Heartbeat_source?.Play();

            var timer = new System.Timers.Timer(3000);
            timer.Elapsed += (sender, e) =>
            {
                transform.position = transform.GetPlayMaker("Logic").GetVariable<FsmVector3>("WalkerOriginalPos").Value;
                transform.Find("Char").gameObject.SetActive(false);

                audio?.Stop();
                Globals.Heartbeat_source?.Stop();

                timer.Stop();
                Destroy(smokes);
                Destroy(this);
            };
            timer.Start();
            
            m_bBlowed = true;
        }
    }
}
