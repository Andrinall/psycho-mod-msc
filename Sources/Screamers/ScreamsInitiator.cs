
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
        readonly int[] targetTimes = new int[3] { 1, 4, 1 };

        readonly List<List<int>> targetDays = new List<List<int>>()
        {
            new List<int>{ 0, 1, 3, 4, 5 },
            new List<int>{ 1, 4, 6 },
            new List<int>{ 0, 5 }
        };

        readonly int[] maxScreamTypes = new int[]
        {
            Enum.GetValues(typeof(ScreamSoundType)).Length,
            Enum.GetValues(typeof(ScreamFearType)).Length,
            Enum.GetValues(typeof(ScreamParalysisType)).Length
        };


        PlayMakerFSM fsm;
        GameObject electricity;

        FsmInt timeOfDay;
        FsmInt globalDay;

        int lastRand = -1;
        int screamerVariation = -1;
        bool triggered = false;


        protected override void Awaked()
        {
            fsm = transform.GetPlayMaker("Activate");
            fsm.AddEvent("SCREAMSTOP");
            fsm.AddGlobalTransition("SCREAMSTOP", "Wake up 2");

            electricity = GameObject.Find("YARD/Building/Dynamics/HouseElectricity/ElectricAppliances");

            timeOfDay = fsm.GetVariable<FsmInt>("TimeOfDay");
            globalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            StateHook.Inject(gameObject, "Activate", "Check time of day", CheckTimeOfDay, 3);
            StateHook.Inject(gameObject, "Activate", "Does call?", DoesCall);
            StateHook.Inject(gameObject, "Activate", "Phone", DisableScreamerIfPhone, -1);
            StateHook.Inject(gameObject, "Activate", "Wake up 2", WakeUp);
        }

        public void ApplyScreamer(ScreamTimeType rand, int variation = 0)
        {
            triggered = false;
            EventsManager.TriggerNightScreamer(rand, variation);
        }

        void CheckTimeOfDay()
        {
            if (Logic.GameFinished)
            {
                Utils.PrintDebug("Psycho game is finished; Skip night screamer");
                return;
            }

            if (Logic.InHorror)
            {
                Utils.PrintDebug("Player in horror; Skip night screamer");
                return;
            }

            if (Logic.MilkUsed && (DateTime.Now - Logic.MilkUseTime).Seconds < 60)
            {
                Utils.PrintDebug("Milk usage timer < 60 seconds\nSkip night screamer");
                return;
            }

            if (timeOfDay.Value < 16)
            {
                Utils.PrintDebug("Time of day less than ((24 + 4) - 10) or 18 hours\nSkip night screamer");
                return;
            }

            int day = (globalDay.Value + 1) % 7;
            lastRand = Random.Range(0, 3);
            screamerVariation = Random.Range(0, maxScreamTypes[lastRand]);

            if (lastRand == 3)
            {
                Utils.PrintDebug("RandomEvent == 3; Skip night screamer");
                return;
            }

            if (!targetDays[lastRand].Contains(day))
            {
                Utils.PrintDebug("Event not exists in table for current day\nSkip night screamer");
                return;
            }

            if (!electricity.activeSelf && lastRand == (int)ScreamTimeType.FEAR && screamerVariation == (int)ScreamFearType.TV && Electricity.GetCutoffTimer() >= 22000f)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "The electricity bill hasn't been paid. Screamer for TV won't work.");
                return;
            }

            Utils.PrintDebug($"Day: {globalDay.Value}[next {day}]; rand[{(ScreamTimeType)lastRand}]; contains[{targetDays[lastRand].Contains(day)}]");
            FsmInt _sleepTime = fsm.GetVariable<FsmInt>("SleepTime"); // 10
            int _timeOfDay = timeOfDay.Value;

            _sleepTime.Value = (24 + targetTimes[lastRand]) - _timeOfDay;
            triggered = true;

            Utils.PrintDebug($"SleepTime orig[{_sleepTime.Value}]; new[{_sleepTime.Value}]; time[{timeOfDay}]; rand[{(ScreamTimeType)lastRand}]");
        }

        void DoesCall(PlayMakerFSM fsm)
        {
            if (!triggered)
                return;

            fsm.SendEvent("NOCALL");
            Utils.PrintDebug("Jokke phone call skipped by night screamer");
        }

        void DisableScreamerIfPhone()
            => triggered = false;

        void WakeUp()
        {
            Logic.MilkUsed = false;
            if (!triggered) return;

            ApplyScreamer((ScreamTimeType)lastRand, screamerVariation);
        }
    }
}
