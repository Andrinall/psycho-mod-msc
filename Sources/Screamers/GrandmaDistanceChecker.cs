
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

        const float DISTANCE = 3.5f;

        AudioSource audio;
        bool isBlowed = false;


        public override void InitScreamer()
        {
            audio = GetComponent<AudioSource>();
        }

        public override void TriggerScreamer()
        {
            transform.position = new Vector3(-9.980711f, -0.593821f, 4.589845f);
            transform.Find("Char").gameObject.SetActive(true);

            isBlowed = false;
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (isBlowed) return;
            if (Vector3.Distance(transform.position, Psycho.Player.position) > DISTANCE) return;
            BlowUpGrandmaAndReset();
        }


        public void BlowUpGrandmaAndReset()
        {
            GameObject _smokes = Instantiate(Globals.SmokeParticleSystem_prefab);
            _smokes.transform.position = transform.position;
            _smokes.transform.localScale = new Vector3(0.0075f, 0.0075f, 0.0075f);

            audio?.Play();
            Globals.Heartbeat_source?.Play();

            var _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += (sender, e) =>
            {
                transform.position = transform.GetPlayMaker("Logic").GetVariable<FsmVector3>("WalkerOriginalPos").Value;
                transform.Find("Char").gameObject.SetActive(false);

                audio?.Stop();
                Globals.Heartbeat_source?.Stop();

                _timer.Stop();
                Destroy(_smokes);
                base.Stop();
            };
            _timer.Start();
            
            isBlowed = true;
        }

    }
}
