using MSCLoader;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal static class Utils
    {
        public static T GetGlobalVariable<T>(string name) where T : NamedVariable
        {
            return FsmVariables.GlobalVariables.FindVariable(name) as T;
        }

        public static void CacheFSM(ref PlayMakerFSM obj, string path, string fsm = "")
        {
            if (obj?.gameObject != null) return;
            
            if (fsm.Length == 0)
                obj = GameObject.Find(path)?.GetComponent<PlayMakerFSM>();
            else
                obj = GameObject.Find(path)?.GetComponents<PlayMakerFSM>().FirstOrDefault(x => x.FsmName == fsm);

            if (obj?.gameObject != null) PrintDebug("Cached FSM: " + path + " | " + fsm);
        }

        public static void PrintDebug(string msg)
        {
#if DEBUG
            ModConsole.Print("[Adrenaline-DBG]: " + msg);
#endif
        }
    }
}
