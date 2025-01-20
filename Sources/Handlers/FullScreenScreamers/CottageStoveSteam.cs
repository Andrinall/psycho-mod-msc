
using Psycho.Internal;


namespace Psycho.Handlers
{
    class CottageStoveSteam : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Steam", "Steam", ShowScreamer);
    }
}
