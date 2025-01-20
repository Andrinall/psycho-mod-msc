
using System.Linq;

using MSCLoader;

using Psycho.Internal;


namespace Psycho.Handlers
{
    class EngineStarter : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            bool _isSatsumaStarter = transform.GetPlayMaker("Starter").FsmVariables.FloatVariables.Any(v => v.Name == "Wear");

            StateHook.Inject(gameObject, "Starter", _isSatsumaStarter ? "Running" : "Motor running", ShowScreamer);
        }
    }
}
