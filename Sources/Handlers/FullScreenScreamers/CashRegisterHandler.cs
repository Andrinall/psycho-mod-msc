
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class CashRegisterHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(
                transform.Find("Register").gameObject, "Data",
                gameObject.name == "PubCashRegister" ? "Pay" : "Purchase", ShowScreamer
            );
    }
}
