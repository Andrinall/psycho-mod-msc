using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    [RequireComponent(typeof(AudioSource))]
    internal sealed class ScreamSoundDistanceChecker : MonoBehaviour
    {
        Transform player;
        public AudioSource source;
        public float distance;

        void Awake()
        {
            source = GetComponent<AudioSource>();
            player = GameObject.Find("PLAYER").transform;
        }

        void FixedUpdate()
        {
            if (!source.isPlaying) return;
            distance = Vector3.Distance(transform.position, player.position);
            
            if (distance > 1.75f) return;

            source.loop = false;
            source.Stop();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"{name} - audio clip stopped by distance checker!");
        }
    }
}
