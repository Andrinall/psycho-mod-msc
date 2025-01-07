
using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    internal sealed class NPCHitHandler : CatchedComponent
    {
        bool HasCrimeAction => transform.GetPlayMaker("CarHit").FsmStates.First(v => v.Name == "Crime") != null;

        protected override void Awaked()
        {            
            if (transform.parent.parent.gameObject.name == "JokkeHiker1")
                return;

            string _parentName = transform.parent.gameObject.name;
            if (_parentName == "Teimo") return;

            if (_parentName == "SuskiHiker")
            {
                StateHook.Inject(transform.Find("HumanCollider").gameObject, "PlayerHit", "State 3", SuskiHitted);
                return;
            }

            SetupCarHitCrime();
            SetupPlayerHitCrime(transform.childCount);
        }

        void SuskiHitted() => Logic.PlayerCommittedOffence("SUSKI_HIT");
        void NPCHitted() => Logic.PlayerCommittedOffence("NPC_HIT");

        void SetupCarHitCrime()
        {
            try
            {
                bool _hasCrime = HasCrimeAction;
                StateHook.Inject(gameObject, "CarHit", _hasCrime ? "Crime" : "Crime 2", NPCHitted);
            }
            catch
            {
                StateHook.Inject(gameObject, "CarHit", "Crime 2", NPCHitted);
            }
        }

        void SetupPlayerHitCrime(int t)
        {
            if (name == "HumanTriggerCop")
            {
                StateHook.Inject(gameObject, "PlayerHit", "State 2", NPCHitted);
                return;
            }

            if (t == 1)
                StateHook.Inject(gameObject, "PlayerHit", "State 3", NPCHitted);
            else if (t == 2)
                StateHook.Inject(transform.Find("HitCollider").gameObject, "PlayerHit", "State 3", NPCHitted);
        }
    }
}
