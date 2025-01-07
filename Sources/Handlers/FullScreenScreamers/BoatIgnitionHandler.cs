
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class BoatIgnitionHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Jank", "Start", ShowScreamer);
    }
}
