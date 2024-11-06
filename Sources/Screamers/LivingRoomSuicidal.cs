using System;
using UnityEngine;

namespace Psycho.Screamers
{
    public sealed class LivingRoomSuicidal : MonoBehaviour
    {
        GameObject suicidal;
        GameObject lamp;
        DateTime enableTime;
        TimeSpan span;


        void Awake()
        {
            suicidal = transform.Find("SuicidalCustom(Clone)").gameObject;
            lamp = transform.Find("livingroom_lamp").gameObject;

            suicidal.transform.localPosition = lamp.transform.localPosition;
            suicidal.transform.localEulerAngles = new Vector3(270f, 177.6f, 0f);

            var origSource = GameObject.Find("KILJUGUY/HikerPivot/JokkeHiker2/RagDoll/pelvis/spine_mid/shoulders(xxxxx)/head/Speak")
                .GetComponent<AudioSource>();

            var newSource = suicidal.AddComponent<AudioSource>();
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

            enabled = false;
        }

        void OnEnable()
        {
            enableTime = DateTime.Now;
            suicidal.SetActive(true);
            lamp.SetActive(false);
        }

        void OnDisable()
        {
            suicidal.SetActive(false);
            lamp.SetActive(true);
        }

        void FixedUpdate()
        {
            span = (DateTime.Now - enableTime);
            if (span.Minutes == 2 && span.Seconds > 30) // 2 minutes & 30 seconds
                enabled = false; // disable component
        }
    }
}
