using UnityEngine;

using MSCLoader;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class GlobalHandler : CatchedComponent
    {
        Transform _houseFire;
        Transform _bells;
        FsmState _bellsState;

        FsmFloat SUN_hours;
        FsmFloat SUN_minutes;
        FsmBool m_bHouseBurningState;

        Vector3 bellsOrigPos;

        bool m_bHouseOnFire = false;
        bool m_bBellsActivated = false;
        bool m_bInstalled = false;


        internal override void Awaked()
        {
            if (m_bInstalled) return;

            Logic._hud = GameObject.Find("GUI/HUD").AddComponent<FixedHUD>();
            Logic._hud.AddElement(eHUDCloneType.RECT, "Psycho", Logic._hud.GetIndexByName("Money"));
            Logic.SetPoints(Logic.Points);

            m_bHouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
            _houseFire = GameObject.Find("YARD/Building/HOUSEFIRE").transform;

            GameObject fridge_paper = GameObject.Find("fridge_paper");
            if (fridge_paper)
                StateHook.Inject(fridge_paper, "Use", "Wait button", -1,
                    _ => Utils.GetGlobalVariable<FsmString>("GUIsubtitle").Value = Locales.FRIDGE_PAPER_TEXT[Globals.CurrentLang]);

            GameObject farm_walker = GameObject.Find("HUMANS/Farmer/Walker");
            if (farm_walker)
                StateHook.Inject(farm_walker, "Speak", "Done", _ => Logic.PlayerCompleteJob("FARMER_QUEST"));

            PlayMakerFSM sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            SUN_hours = sun.GetVariable<FsmFloat>("Hours");
            SUN_minutes = sun.GetVariable<FsmFloat>("Minutes");

            _bells = GameObject.Find("PERAJARVI/CHURCH/Bells").transform;
            _bellsState = _bells.parent.GetPlayMaker("Bells").GetState("Stop bells");
            bellsOrigPos = _bells.position;

            m_bInstalled = true;
        }

        internal override void OnFixedUpdate()
        {
            if (Logic.GameFinished)
            {
                enabled = false;
                return;
            }
            Logic.Tick();

            if (!m_bBellsActivated && SUN_hours.Value == 24f && Mathf.FloorToInt(SUN_minutes.Value) == 0)
            {
                (_bellsState.Actions[0] as ActivateGameObject).activate = true;
                _bells.gameObject.SetActive(true);
                _bells.position = GameObject.Find("PLAYER").transform.position;
                m_bBellsActivated = true;
            }
            else if (m_bBellsActivated && Mathf.FloorToInt(SUN_minutes.Value) > 1)
            {
                (_bellsState.Actions[0] as ActivateGameObject).activate = true;
                _bells.gameObject.SetActive(false);
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

        void OnDestroy()
            => Destroy(Logic._hud);
    }
}
