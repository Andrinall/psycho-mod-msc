using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Handlers;
using Random = UnityEngine.Random;


namespace Psycho.Internal
{
    internal sealed class SoundManager
    {
        public static AudioSource DeathSound;
        public static List<AudioSource> ScreamPoints = new List<AudioSource>();

        static AudioSource RandomPoint => ScreamPoints[Random.Range(0, ScreamPoints.Count)];

        public static void PlayHeartbeat(bool state)
        {
            Globals.Heartbeat_source.enabled = state;
            if (state) Globals.Heartbeat_source.Play();
        }

        public static void PlayDeathSound()
        {
            try
            {
                if (DeathSound.isPlaying) return;
                StopAllScreamSounds();
                AudioSource.PlayClipAtPoint(DeathSound.clip, GameObject.Find("PLAYER").transform.position, 1.5f);
            }
            catch (Exception e)
            {
                ModConsole.Error($"PlayDeathSound error in KillCustor;\n{e.GetFullMessage()}");
            }
        }

        public static void PlayRandomScreamSound(int rand = -1)
        {
            if (DeathSound.isPlaying) return;
            if (rand >= ScreamPoints.Count) return;

            StopAllScreamSounds();
            
            AudioSource source = rand == -1 ? RandomPoint : ScreamPoints[rand];
            if (source.isPlaying) return;
            
            source.loop = true;
            source.Play();

            Utils.PrintDebug(eConsoleColors.GREEN, $"Played sound {source.clip.name}; idx[{rand}]");
        }

        public static void StopScreamSound(string name)
            => ScreamPoints.First(v => v.gameObject.name.Contains(name))?.Stop();

        public static void StopAllScreamSounds()
        {
            foreach (AudioSource point in ScreamPoints)
            {
                point.loop = false;
                point.Stop();
            }
        }

        public static void ChangeFliesSounds()
            => GameObject.Find("PLAYER/Flies")?.GetComponent<FliesChanger>()?.Change();
    }
}
