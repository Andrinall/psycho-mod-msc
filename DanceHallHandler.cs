using System.Collections.Generic;
using UnityEngine;

namespace Adrenaline
{
    internal class DanceHallHandler : MonoBehaviour
    {
        private List<string> FightStates =
            new List<string> { "State 1", "State 7", "State 9", "State 10" };

        private GameObject GUARD;
        private GameObject FIGHTER;

        private PlayMakerFSM ClubGuard;
        private PlayMakerFSM ClubFighter;

        private void OnEnable()
        {
            GUARD = base.transform.Find("GUARD").gameObject;
            FIGHTER = base.transform.Find("FIGHTER").gameObject;
            Utils.PrintDebug("DanceHallHandler enabled");
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref ClubGuard, ref GUARD, "Guard", "React");
            Utils.CacheFSM(ref ClubFighter, ref FIGHTER, "Fighter", "Hit");

            if (ClubGuard?.ActiveStateName == "Catch")
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GUARD_CATCH);
                Utils.PrintDebug("Value increased by ClubGuard try to catch player");
            }

            if (FightStates.Contains(ClubFighter?.ActiveStateName))
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.FIGHT_INCREASE);
                Utils.PrintDebug("Value increased by fighting in Club");
            }
        }
    }
}
