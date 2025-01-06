
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class LightSwitchHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(gameObject, "Use", "ON", Trigger);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked LightSwitch [{transform.GetPath()}]");
        }

        void Trigger() => TryShowScreamer();
    }
}
