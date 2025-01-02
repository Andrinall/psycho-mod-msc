using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Random = UnityEngine.Random;


namespace Psycho.Screamers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class ScreamsInitiator : CatchedComponent
    {
        readonly int[] m_liTimes = new int[3] { 1, 4, 1 };

        readonly List<List<int>> m_liDays = new List<List<int>>()
        {
            new List<int>{ 0, 1, 3, 4, 5 },
            new List<int>{ 1, 4, 6 },
            new List<int>{ 0, 5 }
        };

        readonly int[] _maxScreamTypes = new int[]
        {
            Enum.GetValues(typeof(ScreamSoundType)).Length,
            Enum.GetValues(typeof(ScreamFearType)).Length,
            Enum.GetValues(typeof(ScreamParalysisType)).Length
        };


        PlayMakerFSM _fsm;
        GameObject Electricity;

        FsmInt m_iTimeOfDay;
        FsmInt m_iGlobalDay;

        int m_iRand = -1;
        int screamerVariation = -1;
        bool m_bTrigger = false;


        protected override void Awaked()
        {
            _fsm = transform.GetPlayMaker("Activate");
            _fsm.AddEvent("SCREAMSTOP");
            _fsm.AddGlobalTransition("SCREAMSTOP", "Wake up 2");

            Electricity = GameObject.Find("YARD/Building/Dynamics/HouseElectricity/ElectricAppliances");

            m_iTimeOfDay = _fsm.GetVariable<FsmInt>("TimeOfDay");
            m_iGlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            StateHook.Inject(gameObject, "Activate", "Check time of day", CheckTimeOfDay, 3);
            StateHook.Inject(gameObject, "Activate", "Does call?", DoesCall);
            StateHook.Inject(gameObject, "Activate", "Phone", DisableScreamerIfPhone, -1);
            StateHook.Inject(gameObject, "Activate", "Wake up 2", WakeUp);
        }

        public void ApplyScreamer(ScreamTimeType rand, int variation = 0)
        {
            m_bTrigger = false;

            WorldManager.CloseDoor("YARD/Building/LIVINGROOM/DoorFront/Pivot/Handle");
            WorldManager.CloseDoor("YARD/Building/BEDROOM2/DoorBedroom2/Pivot/Handle");

            EventsManager.TriggerNightScreamer(rand, variation);
        }

        void CheckTimeOfDay()
        {
            if (Logic.GameFinished)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Psycho game is finished; Skip night screamer");
                return;
            }

            if (Logic.inHorror)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Player in horror; Skip night screamer");
                return;
            }

            if (Logic.milkUsed && (DateTime.Now - Logic.milkUseTime).Seconds < 60)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Milk usage timer < 60 seconds\nSkip night screamer");
                return;
            }

            if (m_iTimeOfDay.Value < 16)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Time of day less than ((24 + 4) - 10) or 18 hours\nSkip night screamer");
                return;
            }

            int day = (m_iGlobalDay.Value + 1) % 7;
            m_iRand = Random.Range(0, 3);
            screamerVariation = Random.Range(0, _maxScreamTypes[m_iRand]);

            // for testing
            //m_iRand = (int)ScreamTimeType.FEAR;
            //screamerVariation = (int)ScreamFearType.TV;

            if (m_iRand == 3)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "RandomEvent == 3; Skip night screamer");
                return;
            }

            if (!m_liDays[m_iRand].Contains(day))
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Event not exists in table for current day\nSkip night screamer");
                return;
            }

            if (!Electricity.activeSelf && m_iRand == (int)ScreamTimeType.FEAR && screamerVariation == (int)ScreamFearType.TV && WorldManager.GetElecCutoffTimer() >= 22000f)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "The electricity bill hasn't been paid. Screamer for TV won't work.");
                return;
            }

            Utils.PrintDebug(eConsoleColors.YELLOW, $"Day: {m_iGlobalDay.Value}[next {day}]; rand[{(ScreamTimeType)m_iRand}]; contains[{m_liDays[m_iRand].Contains(day)}]");
            FsmInt sleepTime = _fsm.GetVariable<FsmInt>("SleepTime"); // 10
            int timeOfDay = m_iTimeOfDay.Value;

            sleepTime.Value = (24 + m_liTimes[m_iRand]) - timeOfDay;
            m_bTrigger = true;

            Utils.PrintDebug(eConsoleColors.YELLOW, $"SleepTime orig[{sleepTime.Value}]; new[{sleepTime.Value}]; time[{timeOfDay}]; rand[{(ScreamTimeType)m_iRand}]");
        }

        void DoesCall(PlayMakerFSM fsm)
        {
            if (!m_bTrigger)
                return;

            fsm.SendEvent("NOCALL");
            Utils.PrintDebug(eConsoleColors.YELLOW, "Jokke phone call skipped by night screamer");
        }

        void DisableScreamerIfPhone()
            => m_bTrigger = false;

        void WakeUp()
        {
            Logic.milkUsed = false;
            if (!m_bTrigger) return;

            ApplyScreamer((ScreamTimeType)m_iRand, screamerVariation);
        }
    }
}
