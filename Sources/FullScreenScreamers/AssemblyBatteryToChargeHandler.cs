
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class AssemblyBatteryToChargeHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(gameObject, "Assembly", "Charge", Trigger);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked AssemblyBatteryToCharge [{transform.GetPath()}]");
        }

        void Trigger() => TryShowScreamer();
    }
}
