
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class FridgeOpenDoorHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Use", "Open door", ShowScreamer);
    }
}
