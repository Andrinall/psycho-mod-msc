using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    [RequireComponent(typeof(AudioSource))]
    internal sealed class ScreamSoundDistanceChecker : MonoBehaviour
    {
        Transform Player;
        AudioSource Source;
        float Distance;

        public float TargetDistance = 1.75f;


        void Awake()
        {
            Source = GetComponent<AudioSource>();
            Player = GameObject.Find("PLAYER").transform;
        }

        void FixedUpdate()
        {
            if (!Source.isPlaying) return;
            Distance = Vector3.Distance(transform.position, Player.position);
            
            if (Distance > TargetDistance) return;

            Source.loop = false;
            Source.Stop();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"{name} - audio clip stopped by distance checker!");
        }
    }
}
