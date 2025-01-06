
using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class DumpCigaretteHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            StateHook.Inject(transform.Find("Smoking").gameObject, "Start", "Dump cigarette", Trigger);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked DumpCigaretteHandler [{transform.GetPath()}]");
        }

        void Trigger() => TryShowScreamer();
    }
}
