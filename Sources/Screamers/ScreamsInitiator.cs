using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Psycho
{
    public class ScreamsInitiator : MonoBehaviour
    {
        PlayMakerFSM _fsm;
        FsmInt m_iTimeOfDay;
        FsmInt m_iGlobalDay;
        FsmFloat m_fSunHours;
        FsmFloat m_fSunMinutes;
        
        FsmStateAction[] m_loCachedChurch;
        
        int[] m_liTimes = new int[3] { 1, 4, 1 };
        List<List<int>> m_liDays = new List<List<int>>()
        {
            new List<int>{ 0, 1, 3, 4, 5 },
            new List<int>{ 1, 4, 6 },
            new List<int>{ 0, 5 }
        };

        int m_iRand = -1;
        float m_fSoundStartMinutes = 0f;

        bool m_bStopped = false;
        bool m_bApplyed = false;
        bool m_bTrigger = false;
        bool m_bInstalled = false;

        void OnEnable()
        {
            if (m_bInstalled) return;
            SetupSleepTriggerHooks();
            m_bInstalled = true;
        }

        /* TODO:
         * fix fade out effect
         */

        void FixedUpdate()
        {
            if (!m_bApplyed) return;
            if (m_iRand != 0 || m_bStopped) return;
            if (Mathf.RoundToInt(m_fSunMinutes.Value) < Mathf.RoundToInt(m_fSoundStartMinutes + 10f)) return;

            SoundManager.StopAllScreamSounds();
            m_bStopped = true;
            m_bApplyed = false;

            Utils.PrintDebug($"All sounds stopped start[{Mathf.RoundToInt(m_fSoundStartMinutes)}] current[{Mathf.RoundToInt(m_fSunMinutes.Value)}] end[{Mathf.RoundToInt(m_fSoundStartMinutes + 10f)}]");
        }

        void SetupSleepTriggerHooks()
        {
            _fsm = transform.GetPlayMaker("Activate");
            m_iTimeOfDay = _fsm.GetVariable<FsmInt>("TimeOfDay");
            m_iGlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            var sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            m_fSunHours = sun.GetVariable<FsmFloat>("Hours");
            m_fSunMinutes = sun.GetVariable<FsmFloat>("Minutes");

            StateHook.Inject(gameObject, "Activate", "Check time of day", 3, () => {
                if (Logic.inHorror) return;
                if (Logic.milkUsed && Logic.milkUseTime.Minute + 1 > DateTime.Now.Minute) return;

                m_iRand = UnityEngine.Random.Range(0, 4);
                if (m_iRand == 3) return;
                if (m_iRand == 2) return; // for remove

                var day = (m_iGlobalDay.Value + 1) % 7;
                Utils.PrintDebug($"Day: {m_iGlobalDay.Value}[{day}]; rand[{m_iRand}]; contains[{m_liDays[m_iRand].Contains(day)}]");
                if (!m_liDays[m_iRand].Contains(day)) return;

                var time = m_iTimeOfDay.Value;
                var sleepTime = _fsm.GetVariable<FsmInt>("SleepTime");
                var calc = time + sleepTime.Value - 24;

                Utils.PrintDebug($"SleepTime orig[{sleepTime.Value}]; new[{24 - time + m_liTimes[m_iRand]}]; time[{time}]; calc[{calc}]; rand[{m_iRand}]");
                if (calc < m_liTimes[m_iRand] || calc == 2) return;

                sleepTime.Value = 24 - time + m_liTimes[m_iRand];
                m_bTrigger = true;
            });

            StateHook.Inject(gameObject, "Activate", "Phone", -1, () => m_bTrigger = false);

            StateHook.Inject(gameObject, "Activate", "Wake up 2", () =>
            {
                Logic.milkUsed = false;
                if (!m_bTrigger) return;
                ApplyScreamer();
                m_bTrigger = false;
            });
        }

        void ApplyScreamer()
        {
            m_bApplyed = true;

            WorldManager.CloseDoor("YARD/Building/LIVINGROOM/DoorFront/Pivot/Handle");
            WorldManager.CloseDoor("YARD/Building/BEDROOM2/DoorBedroom2/Pivot/Handle");

            if (m_iRand == 0)
            {
                m_fSoundStartMinutes = m_fSunMinutes.Value;
                SoundManager.PlayRandomScreamSound();
                m_bStopped = false;
                return;
            }

            if (m_iRand == 1)
            {
                int rand2 = 0; // Random.Range(0, 6);
                switch (rand2)
                {
                    case 0:
                        ChangeGrandmaPosition();
                        ClearGrandmaStates();
                        break;
                    case 1:
                        // ChangeSuskiPosition();
                        break;
                    case 2:
                        // ChangeKiljuguyPosition();
                        break;
                    case 3:
                        // SetupTVScreamer();
                        break;
                    case 4:
                        // SetupPhoneScreamer();
                        break;
                    case 5:
                        // SetupWaterScreamer();
                        break;
                }
                return;
            }

            if (m_iRand == 2)
                return;
        }

        void ChangeGrandmaPosition()
        {
            var grandma = GameObject.Find("ChurchGrandma");
            grandma.transform.position = new Vector3(-9.980711f, -0.593821f, 4.589845f);
            grandma.transform.Find("GrannyHiker/Char").gameObject.SetActive(true);
            grandma.AddComponent<GrandmaDistanceChecker>();
        }

        void ClearGrandmaStates()
        {
            var grandma = GameObject.Find("ChurchGrandma");
            var lookat = grandma.transform.Find("GrannyHiker/Char/HeadTarget/LookAt").GetPlayMaker("Random");
            var around = lookat.GetState("Look around");

            if (around.Actions.Length == 3)
            {
                m_loCachedChurch = new List<FsmStateAction>(around.Actions).ToArray();
                Utils.ClearActions(around);
                return;
            }
            
            around.Actions = m_loCachedChurch;
        }
    }
}
