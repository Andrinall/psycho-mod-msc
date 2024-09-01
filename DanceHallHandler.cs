using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Harmony;

namespace Adrenaline
{
    internal class DanceHallHandler : MonoBehaviour
    {
        private List<string> FightStates =
            new List<string> { "State 1", "State 7", "State 9", "State 10" };

        private PlayMakerFSM ClubFighter;

        private void Start()
        {
            try
            {
                var ClubGuard = base.transform.Find("Functions/GUARD/Guard")?.gameObject;
                if (ClubGuard != null)
                    GameHook.InjectStateHook(ClubGuard, "React", "Catch", GuardCatchingPlayer);
                
                ClubFighter =
                    base.transform.Find("Functions/FIGHTER/Fighter").GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Hit");

                Utils.PrintDebug(eConsoleColors.GREEN, "DanceHallHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load DanceHallHandler component");
            }
        }

        private void FixedUpdate()
        {
            if (FightStates.Contains(ClubFighter.ActiveStateName))
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("FIGHT_INCREASE"));
                Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by fighting in Club");
            }
        }

        private void GuardCatchingPlayer()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("GUARD_CATCH"));
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by ClubGuard try to catch player");
        }
    }
}
