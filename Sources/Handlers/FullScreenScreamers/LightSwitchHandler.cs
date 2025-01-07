
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class LightSwitchHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Use", "ON", ShowScreamer);
    }
}
