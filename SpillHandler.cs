using HutongGames.PlayMaker;
using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class SpillHandler : MonoBehaviour
    {
        private PlayMakerFSM SpillPump;
        private FsmBool IsCrime;

        private void OnEnable()
        {
            SpillPump = base.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "SpillPump");
            IsCrime = SpillPump.FsmVariables.GetFsmBool("Crime");
            Utils.PrintDebug("SpillHandler enabled");
        }

        private void FixedUpdate()
        {
            if (SpillPump?.ActiveStateName == "Spill grow" && IsCrime?.Value == true)
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.SPILL_SHIT);
                Utils.PrintDebug("Value increased by spill grow");
            }
        }
    }
}
