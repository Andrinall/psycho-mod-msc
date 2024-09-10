using System.Linq;

using Harmony;
using UnityEngine;

namespace Adrenaline
{
    internal class PissOnDevicesHandler : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                StateHook.Inject(base.gameObject, "Pouring", "State 14", -1,PouringOnDevices);

                Utils.PrintDebug(eConsoleColors.GREEN, "PissOnDevicesHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load PissOnDevicesHandler enabled");
            }
        }

        private void PouringOnDevices()
        {
            var rnd = Random.Range(1, 7);
            
            Resources.FindObjectsOfTypeAll<Transform>()
                .First(v => v.name == "Fuse" + rnd.ToString() + "Pivot")
                .Find("fuse holder(Clone)")
                .GetComponents<PlayMakerFSM>()
                .First(v => v.FsmName == "Use")
                .SendEvent("BLOWFUSE");

            // clone state OFF from MainSwitch
            var mainswitch = GameObject.Find("YARD/Building/Dynamics/FuseTable/Fusetable/MainSwitch");
            var switchfsm = mainswitch.GetComponent<PlayMakerFSM>();
            switchfsm.FsmVariables.GetFsmBool("Switch").Value = false; // states Wait Player -> Wait Button -> Switch
            // states Position -> OFF
            GameObject.Find("Systems/ElectricityBills").GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("MainSwitch").Value = false;
            GameObject.Find("YARD/Building/Dynamics/FuseTable/ElectricShockPoint").SetActive(false);
            //PlayMakerFSM.BroadcastEvent("ELEC_CUTOFF");
            mainswitch.transform.Find("Pivot").transform.localEulerAngles = new Vector3(25f, 0);
            //

            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("PISS_ON_DEVICES"));
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by shocking while pissing on electric device");
        }
    }
}
