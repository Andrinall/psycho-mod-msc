using Harmony;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Psycho
{
    public sealed class GlobalHandler : MonoBehaviour
    {
        Transform _houseFire;
        FsmBool m_bHouseBurningState;
        bool m_bHouseOnFire = false;
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

            var fridge_paper = GameObject.Find("fridge_paper");
            StateHook.Inject(fridge_paper, "Use", "Wait button", -1, SetFridgePaperText);

            StateHook.Inject(GameObject.Find("HUMANS/Farmer/Walker"), "Speak", "Done", () => Logic.PlayerCompleteJob("FARMER_QUEST"));
            Utils.PrintDebug(eConsoleColors.GREEN, "GlobalHandler enabled");
            m_bInstalled = true;
        }

        void FixedUpdate()
        {
            Logic.Tick();

            if (m_bHouseBurningState.Value == true && !m_bHouseOnFire)
            {
                if (Vector3.Distance(_houseFire.position, transform.position) > 4f) return;
                Logic.PlayerCommittedOffence("HOUSE_BURNING");
                m_bHouseOnFire = true;
                return;
            }
        }

        void OnDestroy() => Destroy(Logic._hud);

        void SetFridgePaperText() =>
            Utils.GetGlobalVariable<FsmString>("GUIsubtitle").Value = "I should take my pills\nI shouldn't be bad";
    }
}
