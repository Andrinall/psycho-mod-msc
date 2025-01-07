
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class FishTrapHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Use", "Bool test", ShowScreamer);
    }
}
