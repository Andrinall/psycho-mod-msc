using UnityEngine;
using System.Collections.Generic;

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
            Utils.PrintDebug("DanceHallHandler enabled");
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref ClubGuard, "DANCEHALL/Functions/GUARD/Guard", "React");
            Utils.CacheFSM(ref ClubFighter, "DANCEHALL/Functions/FIGHTER/Fighter", "Hit");

            if (ClubGuard?.ActiveStateName == "Catch")
            {
                AdrenalineLogic.IncreaseTimed(Configuration.GUARD_CATCH);
                Utils.PrintDebug("Value increased by ClubGuard try to catch player");
            }

            if (FightStates.Contains(ClubFighter?.ActiveStateName))
            {
                AdrenalineLogic.IncreaseTimed(Configuration.FIGHT_INCREASE);
                Utils.PrintDebug("Value increased by fighting in Club");
            }
        }
    }
}
