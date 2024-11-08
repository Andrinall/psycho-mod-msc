using System;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using UnityEngine;

using Random = UnityEngine.Random;


namespace Psycho.Internal
{
    internal sealed class SoundManager
    {
        public static AudioSource DeathSound;
        public static List<GameObject> ScreamPoints = new List<GameObject>();

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
            AudioSource source = ScreamPoints[rand]?.GetComponent<AudioSource>();
            if (!source)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Scream AudioSource with idx {rand} is broken!");
                return;
            }

            if (source.isPlaying) return;
            
            source.loop = true;
            source.Play();

            Utils.PrintDebug($"Played sound {source.clip.name}; idx[{rand}]");
        }

        public static void StopScreamSound(string name)
        {
            AudioSource source = ScreamPoints.First(v => v.name.Contains(name))?.GetComponent<AudioSource>();
            if (source?.isPlaying == false) return;
            source?.Stop();
        }

        public static bool AnyScreamSoundIsPlaying()
        {
            foreach(GameObject scream in ScreamPoints)
            {
                AudioSource src = scream?.GetComponent<AudioSource>();
                if (!src) continue;
                if (src?.isPlaying == false) continue;

                return true;
            }

            return false;
        }

        public static void StopAllScreamSounds() {
            ScreamPoints.ForEach(v => {
                AudioSource source = v?.GetComponent<AudioSource>();
                if (!source) return;
                source.loop = false;
                source.Stop();
            });
        }

        public static void ChangeFliesSounds()
            => GameObject.Find("PLAYER/Flies")?.GetComponent<FliesChanger>()?.Change();
    }
}
