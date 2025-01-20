
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    class MummolaJob : MonoBehaviour
    {
        void Awake()
            => StateHook.Inject(gameObject, "Use", "State 6", JobCompleted);

        void JobCompleted()
            => Logic.PlayerCompleteJob("GRANNY_DELIVERY");
    }
}