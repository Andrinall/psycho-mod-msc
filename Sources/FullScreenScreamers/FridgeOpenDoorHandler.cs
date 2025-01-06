
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class FridgeOpenDoorHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(gameObject, "Use", "Open door", Trigger);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked FridgeOpenDoorHandler [{transform.GetPath()}]");
        }

        void Trigger() => TryShowScreamer();
    }
}
