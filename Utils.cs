#define SILENT
using System.Linq;

using UnityEngine;
using HutongGames.PlayMaker;

#if DEBUG
using MSCLoader;
#endif


namespace Adrenaline
{
    internal enum eConsoleColors { WHITE, RED, YELLOW, GREEN };
    internal static class Utils
    {
        private static readonly string DBG_STRING = "[Adrenaline-DBG]: ";

        public static T GetGlobalVariable<T>(string name) where T : NamedVariable
        {
            return FsmVariables.GlobalVariables.FindVariable(name) as T;
        }

        public static string GetCarNameByObject(GameObject obj)
        {
            if (obj.name == "FITTAN") return "Fittan"; // crutch
            if (obj.name == "JONNEZ ES(Clone)") return "Jonnez"; // crutch

            int idx = obj.name.IndexOf('(');
            if (idx == -1) return "unknown";

            var sub = obj.name.Substring(0, idx);
            var arr = sub.ToLower().ToCharArray();
            arr[0] = sub[0];

            return new string(arr);
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
            ModConsole.Print(DBG_STRING + msg);
#endif
        }

        public static void PrintDebug(eConsoleColors color, string msg)
        {
#if DEBUG
            ModConsole.Print(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, GetColor(color), msg));
#endif
        }

        public static void PrintDebug(string fmt, params object[] vars)
        {
#if DEBUG
            ModConsole.Print(string.Format(DBG_STRING + fmt, vars));
#endif
        }

        public static void PrintDebug(eConsoleColors color, string fmt, params object[] vars)
        {
#if DEBUG
            ModConsole.Print(string.Format(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, GetColor(color), fmt), vars));
#endif
        }

        public static void SetMesh(GameObject obj, Mesh mesh)
        {
            var filter = obj.GetComponent<MeshFilter>();
            if (!filter) return;

            filter.mesh = mesh;
            filter.sharedMesh = mesh;
        }

        public static void SetMaterial(GameObject obj, int index, string name, Texture texture, Vector2 offset, Vector2 scale)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            if (!renderer) return;

            var material = renderer.materials.ElementAt(index);
            material.name = name;
            material.mainTexture = texture;
            material.mainTextureOffset = offset;
            material.mainTextureScale = scale;
        }

        private static string GetColor(eConsoleColors color)
        {
            switch (color)
            {
                case eConsoleColors.WHITE:
                    return "white";
                case eConsoleColors.RED:
                    return "red";
                case eConsoleColors.YELLOW:
                    return "yellow";
                case eConsoleColors.GREEN:
                    return "green";
                default:
                    return "white";
            }
        }
    }
}
