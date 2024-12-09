#define TEST

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
        bool m_bApplyed = false;
        bool m_bTrigger = false;


        internal override void Awaked()
        {
            SetupSleepTriggerHooks();
            EventsManager.OnScreamerFinished.AddListener(ScreamerFinished);
        }

        internal override void OnFixedUpdate()
        {
            if (!m_bApplyed) return;
            if (m_bStopped) return;
            Span = (DateTime.Now - StartScream);
            if (Span.Seconds < SoundDisableTime) return;

            SoundManager.StopAllScreamSounds();
            WorldManager.ShowCrows(true);
            m_bStopped = true;
            m_bApplyed = false;

            Utils.PrintDebug(eConsoleColors.YELLOW, $"All scream sounds stopped");
        }


        public void ApplyScreamer(ScreamTimeType rand, int variation = 0)
        {
            m_bApplyed = true;
            m_bTrigger = false;

            WorldManager.CloseDoor("YARD/Building/LIVINGROOM/DoorFront/Pivot/Handle");
            WorldManager.CloseDoor("YARD/Building/BEDROOM2/DoorBedroom2/Pivot/Handle");

            if (rand == ScreamTimeType.SOUNDS) // 1:00
            {
                WorldManager.ShowCrows(false); // 12
                SoundManager.PlayRandomScreamSound(variation);
                StartScream = DateTime.Now;
                m_bStopped = false;

                Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer triggered [{rand} : {variation}]");
                return;
            }

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

                int day = (m_iGlobalDay.Value + 1) % 7;
                Utils.PrintDebug(eConsoleColors.YELLOW, $"Day: {m_iGlobalDay.Value}[next {day}]; rand[{(ScreamTimeType)m_iRand}]; contains[{m_liDays[m_iRand].Contains(day)}]");
#if !TEST
                if (!m_liDays[m_iRand].Contains(day)) return;
#endif

                FsmInt sleepTime = _fsm.GetVariable<FsmInt>("SleepTime");
                int time = m_iTimeOfDay.Value;
                int calc = time + sleepTime.Value - 24;

                Utils.PrintDebug(eConsoleColors.YELLOW, $"SleepTime orig[{sleepTime.Value}]; new[{24 - time + m_liTimes[m_iRand]}]; time[{time}]; calc[{calc}]; rand[{(ScreamTimeType)m_iRand}]");
                if (calc < m_liTimes[m_iRand] || calc == 2) return;

                sleepTime.Value = 24 - time + m_liTimes[m_iRand];
                m_bTrigger = true;
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
#if TEST
                //Utils.PrintDebug("Apply screamer");
                ApplyScreamer(ScreamTimeType.PARALYSIS, (int)ScreamParalysisType.KESSELI);
#else
                int[] temp = new int[2] {
                    Enum.GetValues(typeof(ScreamFearType)).Length,
                    Enum.GetValues(typeof(ScreamParalysisType)).Length
                };

                int variation = m_iRand > 0 ? Random.Range(0, temp[m_iRand - 1]) : -1;

                if
                (
                    m_iRand == (int)ScreamTimeType.FEAR
                    && variation == (int)ScreamFearType.TV
                    && !WorldManager.GetElecMeterSwitchState()
                )
                { return; }
               
                if (variation == (int)ScreamFearType.PHONE)
                    _enablePhoneCord();

                WorldManager.TurnOffElecMeter();
                ApplyScreamer((ScreamTimeType)m_iRand, variation);
#endif
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
