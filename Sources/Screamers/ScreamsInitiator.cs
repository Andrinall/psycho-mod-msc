using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using Psycho.Internal;
using HutongGames.PlayMaker;

using Random = UnityEngine.Random;

namespace Psycho.Screamers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    public sealed class ScreamsInitiator : MonoBehaviour
    {
        public PlayMakerFSM _fsm;
        FsmInt m_iTimeOfDay;
        FsmInt m_iGlobalDay;
        FsmFloat m_fSunHours;
        FsmFloat m_fSunMinutes;
        
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


        public void ApplyScreamer(ScreamTimeType rand, int variation = -1)
        {
            m_bApplyed = true;

            WorldManager.CloseDoor("YARD/Building/LIVINGROOM/DoorFront/Pivot/Handle");
            WorldManager.CloseDoor("YARD/Building/BEDROOM2/DoorBedroom2/Pivot/Handle");

            if (rand == ScreamTimeType.SOUNDS) // 1:00
            {
                m_fSoundStartMinutes = m_fSunMinutes.Value;
                SoundManager.PlayRandomScreamSound(variation);
                m_bStopped = false;
                return;
            }

            if (rand == ScreamTimeType.FEAR) // 4:00
            {
                switch (_getVariativeRandom(variation, 5))
                {
                    case (int)ScreamFearType.GRANNY:
                        ChangeGrandmaPosition(new Vector3(-9.980711f, -0.593821f, 4.589845f));
                        break;
                    case (int)ScreamFearType.SUICIDAL:
                        GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom")
                            .GetComponent<LivingRoomSuicidal>().enabled = true;
                        break;
                    case (int)ScreamFearType.TV:
                        // SetupTVScreamer();
                        break;
                    case (int)ScreamFearType.PHONE:
                        // SetupPhoneScreamer();
                        break;
                    case (int)ScreamFearType.WATER:
                        // AddWaterScreamer();
                        break;
                }
                return;
            }

            if (rand == ScreamTimeType.PARALYSIS) // 1:00
            {
                switch (_getVariativeRandom(variation, 3))
                {
                    case (int)ScreamParalysisType.GRANNY:
                        _fsm.enabled = false;
                        GameObject.Find("GrannyScreamHiker").GetComponent<MummolaCrawl>().enabled = true;
                        break;
                    case (int)ScreamParalysisType.HAND:
                        // hand screamer
                        // GameObject.Find("WindowHandScreamer").GetComponent<MovingHand>().enabled = true;
                        break;
                    case (int)ScreamParalysisType.KESSELI:
                        // kesseli screamer
                        // GameObject.Find("KESSELIScreamer").GetComponent<LongNeck>().enabled = true;
                        break;
                }
            }
        }


        void SetupSleepTriggerHooks()
        {
            _fsm = transform.GetPlayMaker("Activate");
            _fsm.AddEvent("SCREAMSTOP");
            _fsm.AddGlobalTransition("SCREAMSTOP", "Wake up 2");

            m_iTimeOfDay = _fsm.GetVariable<FsmInt>("TimeOfDay");
            m_iGlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            PlayMakerFSM sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            m_fSunHours = sun.GetVariable<FsmFloat>("Hours");
            m_fSunMinutes = sun.GetVariable<FsmFloat>("Minutes");

            StateHook.Inject(gameObject, "Activate", "Check time of day", 3, _ => {
                if (Logic.inHorror) return;
                if (Logic.milkUsed && (DateTime.Now - Logic.milkUseTime).Seconds < 60) return;

                m_iRand = Random.Range(0, 3);

                int day = (m_iGlobalDay.Value + 1) % 7;
                Utils.PrintDebug($"Day: {m_iGlobalDay.Value}[{day}]; rand[{m_iRand}]; contains[{m_liDays[m_iRand].Contains(day)}]");
                if (!m_liDays[m_iRand].Contains(day)) return;

                FsmInt sleepTime = _fsm.GetVariable<FsmInt>("SleepTime");
                int time = m_iTimeOfDay.Value;
                int calc = time + sleepTime.Value - 24;

                Utils.PrintDebug($"SleepTime orig[{sleepTime.Value}]; new[{24 - time + m_liTimes[m_iRand]}]; time[{time}]; calc[{calc}]; rand[{m_iRand}]");
                if (calc < m_liTimes[m_iRand] || calc == 2) return;

                sleepTime.Value = 24 - time + m_liTimes[m_iRand];
                m_bTrigger = true;
            });

            StateHook.Inject(gameObject, "Activate", "Phone", -1, _ => m_bTrigger = false);

            StateHook.Inject(gameObject, "Activate", "Wake up 2", _ =>
            {
                Logic.milkUsed = false;
                if (!m_bTrigger) return;
                ApplyScreamer((ScreamTimeType)m_iRand);
                m_bTrigger = false;
            });

            StateHook.Inject(gameObject, "Activate", "Does call?", 0, fsm =>
            {
                if (m_bTrigger)
                    fsm.SendEvent("NOCALL");
            });
        }

        void ChangeGrandmaPosition(Vector3 position)
        {
            GameObject grandma = GameObject.Find("ChurchGrandma/GrannyHiker");
            grandma.transform.position = position;
            grandma.transform.Find("Char").gameObject.SetActive(true);
            grandma.AddComponent<GrandmaDistanceChecker>();
        }

        int _getVariativeRandom(int variation, int maxValue)
            => (variation == -1 ? Random.Range(0, maxValue) : variation);
    }
}
