using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    [RequireComponent(typeof(AudioSource))]
    internal sealed class ScreamSoundDistanceChecker : CatchedComponent
    {
        Transform Player;
        AudioSource Source;
        float TargetDistance = 1.75f;

        float Distance => Vector3.Distance(transform.position, Player.position);



        internal override void Awaked()
        {
            Source = GetComponent<AudioSource>();
            Player = GameObject.Find("PLAYER").transform;
        }

        internal override void OnFixedUpdate()
        {
            if (!Source.isPlaying) return;            
            if (Distance > TargetDistance) return;

            Source.loop = false;
            Source.Stop();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"{name} - AudioClip stopped by distance checker!");
        }
    }
}
