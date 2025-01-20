
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    class SpillShit : CatchedComponent
    {
        FsmBool isCrime;


        protected override void Awaked()
        {
            isCrime = transform.GetPlayMaker("SpillPump").FsmVariables.GetFsmBool("Crime");
            StateHook.Inject(gameObject, "SpillPump", "Spill grow", Handler);
        }

        void Handler()
        {
            if (isCrime?.Value == false) return;
            Logic.PlayerCommittedOffence("SPILL_SHIT");
        }
    }
}
