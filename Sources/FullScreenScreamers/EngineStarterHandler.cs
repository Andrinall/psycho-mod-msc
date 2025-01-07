
using System.Linq;

using MSCLoader;

using Psycho.Internal;


namespace Psycho.FullScreenScreamers
{
    internal class EngineStarterHandler : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            bool _isSatsumaStarter = transform.GetPlayMaker("Starter").FsmVariables.FloatVariables.Any(v => v.Name == "Wear");

            StateHook.Inject(gameObject, "Starter", _isSatsumaStarter ? "Running" : "Motor running", TryShowScreamer);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Awaked EngineStarterHandler [{transform.GetPath()}]");
        }
    }
}
