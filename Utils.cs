#define SILENT
using System.Linq;

using UnityEngine;
using HutongGames.PlayMaker;
#if DEBUG
using MSCLoader;
#endif


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
                obj = GameObject.Find(path)?.GetComponents<PlayMakerFSM>().First(x => x.FsmName == fsm);

#if !SILENT
            if (obj?.gameObject != null) PrintDebug(string.Format("Cached FSM: {0} | {1}", path, fsm));
            else PrintDebug(string.Format("Failed to cache FSM: {0} | {1}", path, fsm));
#endif
        }

        public static void CacheFSM(ref PlayMakerFSM var, ref GameObject obj, string path, string fsm = "")
        {
            if (var?.gameObject != null) return;
            if (obj == null) return;

            if (fsm.Length == 0)
                var = obj.transform.Find(path)?.GetComponent<PlayMakerFSM>();
            else
                var = obj.transform.Find(path)?.GetComponents<PlayMakerFSM>().First(x => x.FsmName == fsm);

#if !SILENT
            if (var?.gameObject != null) PrintDebug(string.Format("Cached FSM: {0}/{1} | {2}", obj.name, path, fsm));
            else PrintDebug(string.Format("Failed to cache FSM: {0}/{1} | {2}", obj.name, path, fsm));
#endif
        }


        public static void PrintDebug(string msg)
        {
#if DEBUG
            ModConsole.Print("[Adrenaline-DBG]: " + msg);
#endif
        }
    }
}
