using System;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class LivingRoomSuicidal : ScreamerBase
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

            AudioSource origSource = GameObject.Find(
                "KILJUGUY/HikerPivot/JokkeHiker2/RagDoll/pelvis/spine_mid/shoulders(xxxxx)/head/Speak"
            )?.GetComponent<AudioSource>();

            if (origSource == null)
                throw new NullReferenceException("Original JokkeHiker2 AudioSource not exists!");

            AudioSource newSource = suicidal.AddComponent<AudioSource>();
            newSource.clip = AudioClip.Instantiate(origSource.clip);
            newSource.mute = false;
            newSource.loop = true;
            newSource.priority = 128;
            newSource.volume = 1f;
            newSource.pitch = 1f;
            newSource.panStereo = 0f;
            newSource.spatialBlend = 1f;
            newSource.reverbZoneMix = 1f;
            newSource.dopplerLevel = 1f;
            newSource.rolloffMode = AudioRolloffMode.Custom;
            newSource.minDistance = 1f;
            newSource.spread = 0f;
            newSource.maxDistance = 12f;
        }

        public override void TriggerScreamer()
        {           
            enableTime = DateTime.Now;
            suicidal?.SetActive(true);
            lamp?.SetActive(false);
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
