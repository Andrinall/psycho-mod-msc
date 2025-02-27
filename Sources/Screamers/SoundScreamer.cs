﻿
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    class SoundScreamer : CatchedComponent
    {
        const int DISABLE_TIME = 90;
        const float TARGET_DISTANCE = 1.75f;

        List<AudioSource> screamSoundPoints;

        DateTime startScream;
        TimeSpan span;

        int soundVariant = -1;
        bool soundEnabled = false;


        protected override void Awaked()
        {
            screamSoundPoints = SoundManager.ScreamPoints
                .Where(v => v.clip.name != "door_knock" && v.clip.name != "water_kitchen")
                .ToList();

            // add door callbacks for disable night screamer sounds
            DoorsManager.AddDoorOpenCallback("YARD/Building/LIVINGROOM/DoorFront", DisableSoundsLinkedToDoorFront);
            DoorsManager.AddDoorOpenCallback("YARD/Building/BEDROOM2/DoorBedroom2", DisableSoundLinkedToDoorBedroom);

            EventsManager.OnScreamerTriggered.AddListener(TriggerEvent);
        }

        protected override void OnFixedUpdate()
        {
            if (!soundEnabled) return;

            foreach (AudioSource _source in screamSoundPoints)
            {
                if (!_source.isPlaying) continue;
                if (Vector3.Distance(_source.transform.position, Globals.Player.position) > TARGET_DISTANCE) continue;

                Utils.PrintDebug($"AudioClip{{{_source.clip.name}}} stopped by distance checker!");
                StopEvent();
                return;
            }

            span = (DateTime.Now - startScream);
            if (span.Seconds < DISABLE_TIME) return;

            StopEvent();
            Utils.PrintDebug("All scream sounds stopped");
        }

        void TriggerEvent(ScreamTimeType time, int variation)
        {
            if (time != ScreamTimeType.SOUNDS) return;

            SoundManager.PlayRandomScreamSound(variation);
            soundVariant = variation;
            startScream = DateTime.Now;
            soundEnabled = true;

            Utils.PrintDebug(eConsoleColors.GREEN, $"Sound triggered [{(ScreamSoundType)variation}]");
        }

        void StopEvent()
        {
            SoundManager.StopAllScreamSounds();
            soundEnabled = false;

            EventsManager.FinishScreamer(ScreamTimeType.SOUNDS, soundVariant);
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
