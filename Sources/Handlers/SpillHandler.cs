using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Psycho
{
    public sealed class SpillHandler : MonoBehaviour
    {
        FsmBool m_bIsCrime;


        void Start()
        {
            m_bIsCrime = transform.GetPlayMaker("SpillPump").FsmVariables.GetFsmBool("Crime");
            StateHook.Inject(gameObject, "SpillPump", "Spill grow", () => SpillShit());
        }

        void SpillShit()
        {
            if (m_bIsCrime?.Value == false) return;
            Logic.PlayerCommittedOffence("SPILL_SHIT");
            Utils.PrintDebug(eConsoleColors.WHITE, "Points decreased by crimed spill shit");
        }
    }
}
