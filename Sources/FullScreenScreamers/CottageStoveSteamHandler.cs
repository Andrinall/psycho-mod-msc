
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class CottageStoveSteamHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(gameObject, "Steam", "Steam", TryShowScreamer);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked CottageStoveHandler [{transform.GetPath()}]");
        }
    }
}
