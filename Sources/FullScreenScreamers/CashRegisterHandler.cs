
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class CashRegisterHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(
                transform.Find("Register").gameObject, "Data",
                gameObject.name == "PubCashRegister" ? "Pay" : "Purchase", Trigger
            );
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked CashRegisterHandler [{transform.GetPath()}]");
        }

        void Trigger() => TryShowScreamer();
    }
}
