
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class CottageStoveSteamHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Steam", "Steam", ShowScreamer);
    }
}
