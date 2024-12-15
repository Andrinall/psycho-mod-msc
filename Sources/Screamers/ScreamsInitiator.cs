using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Extensions;
using Random = UnityEngine.Random;


namespace Psycho.Screamers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class ScreamsInitiator : CatchedComponent
    {
        PlayMakerFSM _fsm;
        PlayMakerFSM _phoneCord;

        FsmInt m_iTimeOfDay;
        FsmInt m_iGlobalDay;
        FsmFloat m_fSunHours;
        FsmFloat m_fSunMinutes;

        DateTime StartScream;
        public TimeSpan Span { get; private set; }
        public readonly int SoundDisableTime = 90;
        
        int[] m_liTimes = new int[3] { 1, 4, 1 };
        List<List<int>> m_liDays = new List<List<int>>()
        {
            new List<int>{ 0, 1, 3, 4, 5 },
            new List<int>{ 1, 4, 6 },
            new List<int>{ 0, 5 }
        };

        int m_iRand = -1;

        bool m_bStopped = false;
        bool _soundApplyed = false;
        bool m_bTrigger = false;

        int[] _maxScreamTypes = new int[]
        {
            Enum.GetValues(typeof(ScreamSoundType)).Length,
            Enum.GetValues(typeof(ScreamFearType)).Length,
            Enum.GetValues(typeof(ScreamParalysisType)).Length
        };


        internal override void Awaked()
        {
            SetupSleepTriggerHooks();
            EventsManager.OnScreamerFinished.AddListener(ScreamerFinished);
        }

        internal override void OnFixedUpdate()
        {
            if (!_soundApplyed) return;
            if (m_bStopped) return;
            Span = (DateTime.Now - StartScream);
            if (Span.Seconds < SoundDisableTime) return;

            SoundManager.StopAllScreamSounds();
            WorldManager.ShowCrows(true);
            m_bStopped = true;
            _soundApplyed = false;

            Utils.PrintDebug(eConsoleColors.YELLOW, $"All scream sounds stopped");
        }


        public void ApplyScreamer(ScreamTimeType rand, int variation = 0)
        {
            m_bTrigger = false;

            WorldManager.CloseDoor("YARD/Building/LIVINGROOM/DoorFront/Pivot/Handle");
            WorldManager.CloseDoor("YARD/Building/BEDROOM2/DoorBedroom2/Pivot/Handle");

            if (m_iRand == (int)ScreamTimeType.SOUNDS) // 1:00
            {
                SoundManager.PlayRandomScreamSound(variation);
                StartScream = DateTime.Now;
                m_bStopped = false;
                _soundApplyed = true;

                Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer triggered [{m_iRand} : {variation}]");
                return;
            }

            if (m_iRand == (int)ScreamTimeType.FEAR)
            {
                if (variation == (int)ScreamFearType.TV && !WorldManager.GetElecMeterSwitchState())
                    return;

                if (variation == (int)ScreamFearType.PHONE)
                    _enablePhoneCord();
            }

            WorldManager.TurnOffElecMeter();
            WorldManager.ShowCrows(false);
            EventsManager.TriggerNightScreamer(rand, variation);
        }

        void ScreamerFinished() => WorldManager.ShowCrows(true);

        void SetupSleepTriggerHooks()
        {
            _fsm = transform.GetPlayMaker("Activate");
            _fsm.AddEvent("SCREAMSTOP");
            _fsm.AddGlobalTransition("SCREAMSTOP", "Wake up 2");

            _phoneCord = GameObject.Find("YARD/Building/LIVINGROOM/Telephone/Cord/PhoneCordOut/Trigger").GetComponent<PlayMakerFSM>();
            _phoneCord.AddEvent("SCREAMTRIGGER");
            _phoneCord.AddGlobalTransition("SCREAMTRIGGER", "Position");

            m_iTimeOfDay = _fsm.GetVariable<FsmInt>("TimeOfDay");
            m_iGlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            PlayMakerFSM sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            m_fSunHours = sun.GetVariable<FsmFloat>("Hours");
            m_fSunMinutes = sun.GetVariable<FsmFloat>("Minutes");

            StateHook.Inject(gameObject, "Activate", "Check time of day", 3, _ => {
                if (Logic.GameFinished) return;
                if (Logic.inHorror) return;
                if (Logic.milkUsed && (DateTime.Now - Logic.milkUseTime).Seconds < 60) return;

                m_iRand = Random.Range(0, 3);
                if (m_iRand == 3) return;

                int day = (m_iGlobalDay.Value + 1) % 7;

                Utils.PrintDebug(eConsoleColors.YELLOW, $"Day: {m_iGlobalDay.Value}[next {day}]; rand[{(ScreamTimeType)m_iRand}]; contains[{m_liDays[m_iRand].Contains(day)}]");
                if (!m_liDays[m_iRand].Contains(day)) return;

                FsmInt sleepTime = _fsm.GetVariable<FsmInt>("SleepTime"); // 10
                int timeOfDay = m_iTimeOfDay.Value;

                sleepTime.Value = (24 + m_liTimes[m_iRand]) - timeOfDay;
                m_bTrigger = true;

                Utils.PrintDebug(eConsoleColors.YELLOW, $"SleepTime orig[{sleepTime.Value}]; new[{sleepTime.Value}]; time[{timeOfDay}]; rand[{(ScreamTimeType)m_iRand}]");
            });

            StateHook.Inject(gameObject, "Activate", "Does call?", 0, fsm =>
            {
                if (m_bTrigger)
                    fsm.SendEvent("NOCALL");
            });

            StateHook.Inject(gameObject, "Activate", "Phone", -1, _ => m_bTrigger = false);

            StateHook.Inject(gameObject, "Activate", "Wake up 2", _ =>
            {
                Logic.milkUsed = false;
                if (!m_bTrigger) return;

                ApplyScreamer((ScreamTimeType)m_iRand, Random.Range(0, _maxScreamTypes[m_iRand]));
            });
        }

        void _enablePhoneCord()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, "Phone cord enabled.");
            _phoneCord.gameObject.transform.parent.gameObject.SetActive(true);
            _phoneCord.CallGlobalTransition("SCREAMTRIGGER");
        }
    }
}
