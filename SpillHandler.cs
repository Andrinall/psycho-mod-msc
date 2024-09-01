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

        private void Start()
        {
            SpillPump = base.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "SpillPump");
            IsCrime = SpillPump.FsmVariables.GetFsmBool("Crime");
            Utils.PrintDebug(eConsoleColors.GREEN, "SpillHandler enabled");
        }

        private void FixedUpdate()
        {
            if (IsCrime?.Value == false) return;

            if (SpillPump?.ActiveStateName == "Spill grow")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("SPILL_SHIT"));
        }
    }
}
