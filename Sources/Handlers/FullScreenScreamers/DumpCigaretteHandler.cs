
using Psycho.Internal;


namespace Psycho.Handlers
{
    internal class DumpCigaretteHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(transform.Find("Smoking").gameObject, "Start", "Dump cigarette", ShowScreamer);
    }
}
