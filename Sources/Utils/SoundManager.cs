using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Random = UnityEngine.Random;


namespace Psycho.Internal
{
    internal sealed class SoundManager
    {
        public static AudioSource DeathSound;
        public static List<AudioSource> ScreamPoints = new List<AudioSource>();

        public static void PlayHeartbeat(bool state)
        {
            Globals.HeartbeatSound.enabled = state;
            if (state) Globals.HeartbeatSound.Play();
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

            if (rand == -1)
                rand = Random.Range(0, ScreamPoints.Count);

            StopAllScreamSounds();
            
            AudioSource source = ScreamPoints[rand];
            if (source.isPlaying) return;
            
            source.loop = true;
            source.Play();

            Utils.PrintDebug($"Played sound {source.clip.name}; idx[{rand}]");
        }

        public static void StopScreamSound(string name)
            => ScreamPoints.First(v => v.gameObject.name.Contains(name))?.Stop();

        public static bool AnyScreamSoundIsPlaying()
            => ScreamPoints.Any(v => v.isPlaying);

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
