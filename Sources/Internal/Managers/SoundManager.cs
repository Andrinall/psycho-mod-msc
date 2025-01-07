
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Handlers;

using Random = UnityEngine.Random;


namespace Psycho.Internal
{
    internal static class SoundManager
    {
        public static AudioSource DeathSound;
        public static readonly List<AudioSource> ScreamPoints = new List<AudioSource>();
        public static readonly List<AudioClip> FullScreenScreamersSounds = new List<AudioClip>();

        public static AudioSource FullScreenScreamerSoundsSource = null;

        static AudioClip adRadioStaticOrig_clip;

        static AudioSource RandomPoint => ScreamPoints[Random.Range(0, ScreamPoints.Count)];
        static AudioClip RandomSoundForFullScreenScreamer => FullScreenScreamersSounds[Random.Range(0, FullScreenScreamersSounds.Count)];
        
        public static AudioSource AddAudioSource(GameObject parent, AudioClip clip, float volume)
        {
            AudioSource _source = parent.AddComponent<AudioSource>();
            _source.clip = clip;
            _source.playOnAwake = true;
            _source.loop = true;
            _source.priority = 50;
            _source.volume = 0.12f;
            _source.spatialBlend = 0f;
            _source.dopplerLevel = 0f;
            _source.spread = 0f;
            _source.Play();
            _source.mute = true;
            return _source;
        }

        public static void ReplaceRadioStaticSound(AudioClip newClip)
        {
            GameObject _radioChannels = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(v => v != null && v.name == "RadioChannels")
                .FirstOrDefault();

            AudioSource _static = _radioChannels.transform.GetChild(2).GetComponent<AudioSource>();
            
            if (adRadioStaticOrig_clip == null)
                adRadioStaticOrig_clip = _static.clip;

            if (newClip == null && adRadioStaticOrig_clip != null)
            {
                _static.clip = adRadioStaticOrig_clip;
                return;
            }

            _static.clip = newClip;
        }

        public static void EnableRandomSoundForFullScreenScreamer()
        {
            FullScreenScreamerSoundsSource.Stop();
            FullScreenScreamerSoundsSource.clip = RandomSoundForFullScreenScreamer;
            FullScreenScreamerSoundsSource.Play();
            Utils.PrintDebug($"New random clip for full screen screamer [{FullScreenScreamerSoundsSource.clip.name}]");
        }

        public static bool IsFullScreenScreamerSoundPlaying()
            => FullScreenScreamerSoundsSource.isPlaying;

        public static bool IsSoundPlayed(params string[] obj)
            => ScreamPoints.Any(v => v != null && v.clip != null && obj.Contains(v.clip.name) && v.isPlaying);

        public static void PlayHeartbeat(bool state)
        {
            Globals.Heartbeat_source.enabled = state;
            
            if (state)
                Globals.Heartbeat_source.Play();
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
            
            AudioSource _source = rand == -1 ? RandomPoint : ScreamPoints[rand];
            if (_source.isPlaying) return;

            _source.loop = true;
            _source.Play();

            Utils.PrintDebug(eConsoleColors.GREEN, $"Played sound {_source.clip.name}; idx[{rand}]");
        }

        public static void StopScreamSound(string name)
        {
            AudioSource _point = ScreamPoints.FirstOrDefault(v => v.gameObject.name.Contains(name));
            _point.loop = false;
            _point.Stop();
        }

        public static void StopAllScreamSounds()
        {
            foreach (AudioSource _point in ScreamPoints)
            {
                if (_point == null) continue;

                _point.loop = false;
                _point.Stop();
            }
        }

        public static void ChangeFliesSounds()
            => GameObject.Find("PLAYER/Flies")?.GetComponent<FliesChanger>()?.Change();
    }
}
