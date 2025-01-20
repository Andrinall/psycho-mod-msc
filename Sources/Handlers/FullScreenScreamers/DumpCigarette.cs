
using Psycho.Internal;


namespace Psycho.Handlers
{
    class DumpCigarette : FullScreenScreamerBase
    {
        protected override void Awaked()
            => StateHook.Inject(transform.Find("Smoking").gameObject, "Start", "Dump cigarette", ShowScreamer);
    }
}
