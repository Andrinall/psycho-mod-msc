
using Psycho.Internal;


namespace Psycho.Handlers
{
    class FishTrapUse : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Use", "Bool test", ShowScreamer);
    }
}
