using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;

namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    public sealed class NPCHitHandler : MonoBehaviour
    {
        bool m_bInstalled = false;


        void OnEnable()
        {
            if (m_bInstalled) return;
            
            if (transform.parent.parent.gameObject.name == "JokkeHiker1")
                return;

            string parentName = transform.parent.gameObject.name;
            if (parentName == "Teimo") return;

            if (parentName == "SuskiHiker")
            {
                StateHook.Inject(
                    transform.Find("HumanCollider").gameObject, "PlayerHit", "State 3",
                    _ => Logic.PlayerCommittedOffence("SUSKI_HIT")
                );

                goto SkipOther;
            }

            SetupCarHitCrime();
            SetupPlayerHitCrime(transform.childCount);
            
            SkipOther:
            Utils.PrintDebug(eConsoleColors.GREEN, "NPCHitHandler enabled");
            m_bInstalled = true;
        }

        void SetupCarHitCrime()
        {
            try
            {
                bool crime = transform.GetPlayMaker("CarHit").FsmStates
                    .First(v => v.Name == "Crime") == null;

                StateHook.Inject(gameObject, "CarHit",
                    crime ? "Crime 2" : "Crime",
                    _ => Logic.PlayerCommittedOffence("NPC_HIT")
                );
            }
            catch
            {
                StateHook.Inject(gameObject, "CarHit", "Crime 2", _ => Logic.PlayerCommittedOffence("NPC_HIT"));
            }
        }

        void SetupPlayerHitCrime(int t)
        {
            if (name == "HumanTriggerCop")
            {
                StateHook.Inject(gameObject, "PlayerHit", "State 2", _ => Logic.PlayerCommittedOffence("NPC_HIT"));
                return;
            }

            if (t == 1)
            {
                StateHook.Inject(gameObject, "PlayerHit", "State 3", _ => Logic.PlayerCommittedOffence("NPC_HIT"));
                return;
            }

            if (t == 2)
            {
                StateHook.Inject(transform.Find("HitCollider").gameObject,
                    "PlayerHit", "State 3", _ => Logic.PlayerCommittedOffence("NPC_HIT")
                );
            }
        }
    }
}
