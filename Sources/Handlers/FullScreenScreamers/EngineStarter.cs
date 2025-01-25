
using System.Linq;

using MSCLoader;

using Psycho.Internal;


namespace Psycho.Handlers
{
    class EngineStarter : FullScreenScreamerBase
    {
        protected override void Awaked()
        {
            PlayMakerFSM _starterFsm = transform.GetPlayMaker("Starter");
            if (_starterFsm == null)
            {
                Destroy(this);
                return;
            }

            bool _isSatsumaStarter = _starterFsm.FsmVariables.FloatVariables.Any(v => v.Name == "Wear");

            StateHook.Inject(gameObject, "Starter", _isSatsumaStarter ? "Running" : "Motor running", ShowScreamer);
        }
    }
}
