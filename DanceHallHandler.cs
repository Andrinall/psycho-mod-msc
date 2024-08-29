using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Adrenaline
{
    internal class DanceHallHandler : MonoBehaviour
    {
        private List<string> FightStates =
            new List<string> { "State 1", "State 7", "State 9", "State 10" };

        private PlayMakerFSM ClubGuard;
        private PlayMakerFSM ClubFighter;

        private void OnEnable()
        {
            try
            {
                ClubGuard =
                    base.transform.Find("Functions/GUARD/Guard").GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "React");


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
            if (ClubGuard?.ActiveStateName == "Catch")
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GUARD_CATCH);
                Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by ClubGuard try to catch player");
            }

            if (FightStates.Contains(ClubFighter?.ActiveStateName ?? ""))
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.FIGHT_INCREASE);
                Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by fighting in Club");
            }
        }
    }
}
