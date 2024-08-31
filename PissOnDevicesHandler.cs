using System.Linq;
using Harmony;
using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    internal class PissOnDevicesHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            try
            {
                var result = GameHook.InjectStateHook(
                    base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/FluidTrigger").gameObject,
                    "Pouring", "Wait", Pouring);



                Utils.PrintDebug(eConsoleColors.GREEN, "PissOnDevicesHandler {0}", result ? "enabled" : "failed setup");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load PissOnDevicesHandler enabled");
            }
        }

        private void Pouring()
        {
            var rnd = Random.Range(1, 7);
            var fuseslist = Resources.FindObjectsOfTypeAll<Transform>().Where(v => v.name == "fuse holder(Clone)").ToArray();
            var randomfuseholder = fuseslist.First(v => v.transform.parent.gameObject.name == "Fuse" + rnd.ToString() + "Pivot");
            randomfuseholder.GetComponents<PlayMakerFSM>().First(v => v.FsmName == "Use").SendEvent("PROCEED");

            // clone state OFF from MainSwitch
            var mainswitch = GameObject.Find("YARD/Building/Dynamics/FuseTable/Fusetable/MainSwitch");
            var switchfsm = mainswitch.GetComponent<PlayMakerFSM>();
            switchfsm.FsmVariables.GetFsmBool("Switch").Value = false;
            GameObject.Find("Systems/ElectricityBills").GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("MainSwitch").Value = false;
            GameObject.Find("YARD/Building/Dynamics/FuseTable/ElectricShockPoint").SetActive(false);
            PlayMakerFSM.BroadcastEvent("ELEC_CUTOFF");
            mainswitch.transform.Find("Pivot").transform.localEulerAngles = new Vector3(25f, 0);
            //

            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("PISS_ON_DEVICES").Value);
            Utils.PrintDebug(eConsoleColors.WHITE, "Value increased by piss on TV and hit electricity");
        }
    }
}
