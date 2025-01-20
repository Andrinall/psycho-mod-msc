
using Psycho.Internal;


namespace Psycho.Handlers
{
    class FridgeDoorOpen : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Use", "Open door", ShowScreamer);
    }
}
