using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Psycho
{
    [RequireComponent(typeof(AudioSource))]
    public class GrandmaDistanceChecker : MonoBehaviour
    {
        public Transform _player;
        public float Distance = 3.5f;
        bool m_bBlowed = false;

        void OnEnable() => _player = GameObject.Find("PLAYER").transform;
        
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

            AudioSource audio = transform.GetComponent<AudioSource>();
            audio.Play();

            var timer = new System.Timers.Timer(3000);
            timer.Elapsed += (sender, e) => {
                audio.Stop();
                transform.position = transform.Find("GrannyHiker").GetPlayMaker("Logic")
                    .GetVariable<FsmVector3>("WalkerOriginalPos").Value;
                transform.Find("GrannyHiker/Char").gameObject.SetActive(false);

                timer.Stop();
                Destroy(smokes);
                Destroy(this);
            };
            timer.Start();
            
            m_bBlowed = true;
        }
    }
}
