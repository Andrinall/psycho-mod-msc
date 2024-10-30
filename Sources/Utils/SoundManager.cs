using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

namespace Psycho.Internal
{
    internal sealed class SoundManager
    {
        public static AudioSource DeathSound;
        public static List<GameObject> ScreamPoints = new List<GameObject>();


        public static void PlayDeathSound()
        {
            try
            {
                if (DeathSound.isPlaying) return;
                ScreamPoints.ForEach(v => v?.GetComponent<AudioSource>()?.Stop());
                AudioSource.PlayClipAtPoint(DeathSound.clip, GameObject.Find("PLAYER").transform.position, 1.5f);
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"PlayDeathSound error in KillCustor;\n{e.GetFullMessage()}");
            }
        }

        public static void PlayRandomScreamSound(int rand = -1)
        {
            if (DeathSound.isPlaying) return;

            if (rand == -1)
                rand = Random.Range(0, ScreamPoints.Count);

            AudioSource source = ScreamPoints[rand].GetComponent<AudioSource>();
            if (source.isPlaying) return;

            source.loop = true;
            source.Play();
            Utils.PrintDebug($"Played sound {source.clip.name}; idx[{rand}]");
        }

        public static void StopScreamSound(string name)
        {
            AudioSource source = ScreamPoints.Find(v => v.name.Contains(name)).GetComponent<AudioSource>();
            if (!source.isPlaying) return;
            source.Stop();
        }

        public static bool AnyScreamSoundIsPlaying()
        {
            foreach(GameObject scream in ScreamPoints)
            {
                AudioSource src = scream.GetComponent<AudioSource>();
                if (!src.isPlaying) continue;
                return true;
            }

            return false;
        }

        public static void StopAllScreamSounds() => ScreamPoints.ForEach(v => v.GetComponent<AudioSource>()?.Stop());

        public static void ChangeFliesSounds() => GameObject.Find("PLAYER/Flies")?.GetComponent<FliesChanger>()?.Change();
    }
}
