
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class SoundScreamer : CatchedComponent
    {
        const int SoundDisableTime = 90;
        const float TargetDistance = 1.75f;

        List<AudioSource> ScreamSoundPoints;
        Transform Player;

        DateTime StartScream;
        TimeSpan Span;

        int SoundVariant = -1;
        bool SoundEnabled = false;


        protected override void Awaked()
        {
            ScreamSoundPoints = SoundManager.ScreamPoints
                .Where(v => v.clip.name != "door_knock" && v.clip.name != "water_kitchen")
                .ToList();

            Player = GameObject.Find("PLAYER").transform;

            // add door callbacks for disable night screamer sounds
            WorldManager.AddDoorOpenCallback("YARD/Building/LIVINGROOM/DoorFront", DisableSoundsLinkedToDoorFront);
            WorldManager.AddDoorOpenCallback("YARD/Building/BEDROOM2/DoorBedroom2", DisableSoundLinkedToDoorBedroom);

            EventsManager.OnScreamerTriggered.AddListener(TriggerEvent);
        }

        protected override void OnFixedUpdate()
        {
            if (!SoundEnabled) return;

            foreach (AudioSource source in ScreamSoundPoints)
            {
                if (!source.isPlaying) continue;
                if (Vector3.Distance(source.transform.position, Player.position) > TargetDistance) continue;

                Utils.PrintDebug(eConsoleColors.YELLOW, $"AudioClip{{{source.clip.name}}} stopped by distance checker!");
                StopEvent();
                return;
            }

            Span = (DateTime.Now - StartScream);
            if (Span.Seconds < SoundDisableTime) return;

            StopEvent();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"All scream sounds stopped");
        }

        void TriggerEvent(ScreamTimeType time, int variation)
        {
            if (time != ScreamTimeType.SOUNDS) return;

            SoundManager.PlayRandomScreamSound(variation);
            SoundVariant = variation;
            StartScream = DateTime.Now;
            SoundEnabled = true;

            Utils.PrintDebug(eConsoleColors.GREEN, $"Sound triggered [{(ScreamSoundType)variation}]");
        }

        void StopEvent()
        {
            SoundManager.StopAllScreamSounds();
            SoundEnabled = false;

            EventsManager.FinishScreamer(ScreamTimeType.SOUNDS, SoundVariant);
        }

        void DisableSoundsLinkedToDoorFront()
        {
            if (!SoundManager.IsSoundPlayed("door_knock", "footsteps")) return;
            StopEvent();
        }

        void DisableSoundLinkedToDoorBedroom()
        {
            if (!SoundManager.IsSoundPlayed("bedroom", "crying_kid")) return;
            StopEvent();
        }
    }
}
