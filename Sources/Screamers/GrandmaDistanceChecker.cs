using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    [RequireComponent(typeof(AudioSource))]
    internal sealed class GrandmaDistanceChecker : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.FEAR;
        public override int ScreamerVariant => (int)ScreamFearType.GRANNY;

        const float Distance = 3.5f;

        Transform _player;
        AudioSource audio;
        bool m_bBlowed = false;


        public override void InitScreamer()
        {
            _player = GameObject.Find("PLAYER").transform;
            audio = transform.GetComponent<AudioSource>();
        }

        public override void TriggerScreamer()
        {
            transform.position = new Vector3(-9.980711f, -0.593821f, 4.589845f);
            transform.Find("Char").gameObject.SetActive(true);

            m_bBlowed = false;
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
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
                base.Stop();
            };
            timer.Start();
            
            m_bBlowed = true;
        }

    }
}
