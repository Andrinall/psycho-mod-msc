
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class BoatIgnitionHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(gameObject, "Jank", "Start", TryShowScreamer);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked BoatIgnitionHandler [{transform.GetPath()}]");
        }
    }
}
