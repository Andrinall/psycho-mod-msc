
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class AssemblyBatteryToChargeHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Assembly", "Charge", ShowScreamer);
    }
}
