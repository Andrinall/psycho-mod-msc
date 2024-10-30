using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using MSCLoader;


namespace Psycho.Handlers
{
    public sealed class GlobalHandler : MonoBehaviour
    {
        Transform _houseFire;
        Transform _bells;

        FsmFloat SUN_hours;
        FsmFloat SUN_minutes;
        FsmBool m_bHouseBurningState;

        Vector3 bellsOrigPos;

        bool m_bHouseOnFire = false;
        bool m_bBellsActivated = false;
        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;

            Logic._hud = GameObject.Find("GUI/HUD").AddComponent<FixedHUD>();
            Logic._hud.AddElement(eHUDCloneType.RECT, "Psycho", Logic._hud.GetIndexByName("Money"));
            Utils.PrintDebug(eConsoleColors.GREEN, "HUD Enabled");
            Logic.Points = Logic.Points;

            m_bHouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
            _houseFire = GameObject.Find("YARD/Building/HOUSEFIRE").transform;

            GameObject fridge_paper = GameObject.Find("fridge_paper");
            if (fridge_paper)
                StateHook.Inject(fridge_paper, "Use", "Wait button", -1,
                    _ => Utils.GetGlobalVariable<FsmString>("GUIsubtitle").Value = "I should take my pills\nI shouldn't be bad");

            GameObject farm_walker = GameObject.Find("HUMANS/Farmer/Walker");
            if (farm_walker)
                StateHook.Inject(farm_walker, "Speak", "Done", _ => Logic.PlayerCompleteJob("FARMER_QUEST"));

            PlayMakerFSM sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            SUN_hours = sun.GetVariable<FsmFloat>("Hours");
            SUN_minutes = sun.GetVariable<FsmFloat>("Minutes");

            _bells = GameObject.Find("PERAJARVI/CHURCH/Bells").transform;
            bellsOrigPos = _bells.position;

            Utils.PrintDebug(eConsoleColors.GREEN, "GlobalHandler enabled");
            m_bInstalled = true;
        }

        void FixedUpdate()
        {
            Logic.Tick();

            if (!m_bBellsActivated && SUN_hours.Value == 24f && Mathf.FloorToInt(SUN_minutes.Value) == 0)
            {
                _bells.gameObject.SetActive(true);
                _bells.position = GameObject.Find("PLAYER").transform.position;

                m_bBellsActivated = true;
                Utils.PrintDebug("Bells activated");
            }
            else if (m_bBellsActivated && Mathf.FloorToInt(SUN_minutes.Value) > 1)
            {
                _bells.position = bellsOrigPos;
                m_bBellsActivated = false;
            }
            
            if (m_bHouseBurningState.Value == true && !m_bHouseOnFire)
            {
                if (Vector3.Distance(_houseFire.position, transform.position) > 4f) return;
                Logic.PlayerCommittedOffence("HOUSE_BURNING");
                m_bHouseOnFire = true;
                return;
            }
        }

        void OnDestroy() => Destroy(Logic._hud);
    }
}
