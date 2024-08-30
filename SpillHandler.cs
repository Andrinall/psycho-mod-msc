using System.Linq;
using UnityEngine;
using HutongGames.PlayMaker;
using Harmony;

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
            Utils.PrintDebug(eConsoleColors.GREEN, "SpillHandler enabled");
        }

        private void FixedUpdate()
        {
            if (SpillPump?.ActiveStateName == "Spill grow" && IsCrime?.Value == true)
            {
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("SPILL_SHIT").Value);
                Utils.PrintDebug("Value increased by spill grow");
            }
        }
    }
}
