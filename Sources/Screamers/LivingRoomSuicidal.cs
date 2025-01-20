
using System;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    class LivingRoomSuicidal : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.FEAR;
        public override int ScreamerVariant => (int)ScreamFearType.SUICIDAL;


        GameObject suicidal;
        GameObject lamp;

        DateTime enableTime;
        TimeSpan span;


        public override void InitScreamer()
        {
            suicidal = transform.Find("SuicidalCustom(Clone)").gameObject;
            lamp = transform.Find("livingroom_lamp").gameObject;

            suicidal.transform.localPosition = lamp.transform.localPosition;
            suicidal.transform.localEulerAngles = new Vector3(270f, 177.6f, 0f);
            suicidal.SetActive(false);

            AudioSource _origSource = GameObject.Find(
                "KILJUGUY/HikerPivot/JokkeHiker2/RagDoll/pelvis/spine_mid/shoulders(xxxxx)/head/Speak"
            )?.GetComponent<AudioSource>();

            if (_origSource == null)
                throw new NullReferenceException("Original JokkeHiker2 AudioSource not exists!");

            AudioSource _newSource = suicidal.AddComponent<AudioSource>();
            _newSource.clip = AudioClip.Instantiate(_origSource.clip);
            _newSource.mute = false;
            _newSource.loop = true;
            _newSource.priority = 128;
            _newSource.volume = 1f;
            _newSource.pitch = 1f;
            _newSource.panStereo = 0f;
            _newSource.spatialBlend = 1f;
            _newSource.reverbZoneMix = 1f;
            _newSource.dopplerLevel = 1f;
            _newSource.rolloffMode = AudioRolloffMode.Custom;
            _newSource.minDistance = 1f;
            _newSource.spread = 0f;
            _newSource.maxDistance = 12f;
        }

        public override void TriggerScreamer()
        {           
            enableTime = DateTime.Now;
            suicidal?.SetActive(true);
            lamp?.SetActive(false);
            AudioSource.PlayClipAtPoint(ResourcesStorage.JokkeSpawned_clip, transform.position, 1f);
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!suicidal.activeSelf) return;

            span = (DateTime.Now - enableTime);
            if (span.Minutes == 2 && span.Seconds > 30) // 2 minutes & 30 seconds
            {
                suicidal?.SetActive(false);
                lamp?.SetActive(true);
                base.Stop();
            }
        }
    }
}
