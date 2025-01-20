
using Psycho.Internal;


namespace Psycho.Handlers
{
    class CashRegisterUse : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(
                transform.Find("Register").gameObject, "Data",
                gameObject.name == "PubCashRegister" ? "Pay" : "Purchase", ShowScreamer
            );
    }
}
