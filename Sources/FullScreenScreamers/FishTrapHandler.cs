
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class FishTrapHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(gameObject, "Use", "Bool test", TryShowScreamer);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked FishTrapHandler [{transform.GetPath()}]");
        }
    }
}
