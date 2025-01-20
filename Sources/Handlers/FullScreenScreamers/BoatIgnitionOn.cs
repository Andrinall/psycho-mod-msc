
using Psycho.Internal;


namespace Psycho.Handlers
{
    class BoatIgnitionOn : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Jank", "Start", ShowScreamer);
    }
}
