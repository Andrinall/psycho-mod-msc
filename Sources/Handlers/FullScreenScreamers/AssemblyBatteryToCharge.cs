
using Psycho.Internal;


namespace Psycho.Handlers
{
    class AssemblyBatteryToCharge : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(gameObject, "Assembly", "Charge", ShowScreamer);
    }
}
