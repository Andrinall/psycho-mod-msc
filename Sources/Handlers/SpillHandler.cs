using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class SpillHandler : CatchedComponent
    {
        FsmBool m_bIsCrime;


        internal override void Awaked()
        {
            m_bIsCrime = transform.GetPlayMaker("SpillPump").FsmVariables.GetFsmBool("Crime");
            StateHook.Inject(gameObject, "SpillPump", "Spill grow", _ => SpillShit());
        }

        void SpillShit()
        {
            if (m_bIsCrime?.Value == false) return;
            Logic.PlayerCommittedOffence("SPILL_SHIT");
        }
    }
}
