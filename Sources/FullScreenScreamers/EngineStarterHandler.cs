
using System.Linq;

using MSCLoader;

using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class EngineStarterHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            bool isSatsumaStarter = transform.GetPlayMaker("Starter").FsmVariables.FloatVariables.Any(v => v.Name == "Wear");

            StateHook.Inject(gameObject, "Starter", isSatsumaStarter ? "Running" : "Motor running", TryShowScreamer);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked EngineStarterHandler [{transform.GetPath()}]");
        }
    }
}
