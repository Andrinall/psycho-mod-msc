
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(PlayMakerFSM))]
    class SleepTrigger : CatchedComponent
    {
        protected override void Awaked()
        {
            if (transform.root.name == "forest(Clone)")
            {
                Destroy(this);
                return;
            }

            transform.ClearFsmActions("Activate", "Calc rates", 6);
            StateHook.Inject(gameObject, "Activate", "Calc rates", UpdateFatigue, -1);
        }

        void UpdateFatigue()
            => Globals.PlayerFatigue.Value = Mathf.Clamp(Globals.PlayerFatigue.Value - Logic.Value, 0f, 100f);
    }
}