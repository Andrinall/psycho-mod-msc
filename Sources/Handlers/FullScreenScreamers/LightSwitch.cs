
using Psycho.Internal;


namespace Psycho.Handlers
{
    class LightSwitch : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Use", "ON", ShowScreamer);
    }
}
