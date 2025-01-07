
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class SpillHandler : CatchedComponent
    {
        FsmBool isCrime;


        protected override void Awaked()
        {
            isCrime = transform.GetPlayMaker("SpillPump").FsmVariables.GetFsmBool("Crime");
            StateHook.Inject(gameObject, "SpillPump", "Spill grow", SpillShit);
        }

        void SpillShit()
        {
            if (isCrime?.Value == false) return;
            Logic.PlayerCommittedOffence("SPILL_SHIT");
        }
    }
}
