#define SILENT
using System.Linq;

#if DEBUG
using MSCLoader;
#endif
using UnityEngine;
using HutongGames.PlayMaker;



namespace Adrenaline
{
    internal enum eConsoleColors { WHITE, RED, YELLOW, GREEN };
    internal static class Utils
    {
        private static readonly string DBG_STRING = "[Adrenaline-DBG]: ";
        private static int GlobalDay_cached = -1;

        internal static T GetGlobalVariable<T>(string name) where T : NamedVariable
        {
            return FsmVariables.GlobalVariables.FindVariable(name) as T;
        }

        internal static string GetCarNameByObject(GameObject obj)
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

        internal static void CacheFSM(ref PlayMakerFSM obj, string path, string fsm = "")
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

        internal static void CacheFSM(ref PlayMakerFSM var, ref GameObject obj, string path, string fsm = "")
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


        internal static void PrintDebug(string msg)
        {
#if DEBUG
            ModConsole.Print(DBG_STRING + msg);
#endif
        }

        internal static bool PrintDebug(eConsoleColors color, string msg)
        {
#if DEBUG
            ModConsole.Print(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, GetColor(color), msg));
            return true;
#endif
        }

        internal static void PrintDebug(string fmt, params object[] vars)
        {
#if DEBUG
            ModConsole.Print(string.Format(DBG_STRING + fmt, vars));
#endif
        }

        internal static void PrintDebug(eConsoleColors color, string fmt, params object[] vars)
        {
#if DEBUG
            ModConsole.Print(string.Format(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, GetColor(color), fmt), vars));
#endif
        }

        internal static void ChangeMesh(GameObject obj, Mesh mesh, Texture texture, Vector2 offset, Vector2 scale)
        {
            SetMesh(obj, mesh);
            SetMaterial(obj, 0, texture.name, texture, offset, scale);
        }

        internal static void SetMesh(GameObject obj, Mesh mesh)
        {
            var filter = obj.GetComponent<MeshFilter>();
            if (!filter) return;

            filter.mesh = mesh;
            filter.sharedMesh = mesh;
        }

        /// <summary>
        /// Sets object material with texture & uv offsets
        /// </summary>
        internal static void SetMaterial(GameObject obj, int index, string name, Texture texture, Vector2 offset, Vector2 scale)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            if (!renderer) return;

            var material = renderer.materials.ElementAt(index);
            material.name = name;
            material.mainTexture = texture;
            material.mainTextureOffset = offset;
            material.mainTextureScale = scale;
        }

        /// <summary>
        /// Gets a game hours in 12 hours format (am/pm)
        /// </summary>
        internal static float GetHours12()
        {
            var hours = 24f / (360f / GetGlobalVariable<FsmFloat>("TimeRotationHour").Value);
            return (hours > 12 ? hours / 2 : hours);
        }

        /// <summary>
        /// Gets a game minutes
        /// </summary>
        internal static float GetMinutes()
        {
            return 60f / (360f / GetGlobalVariable<FsmFloat>("TimeRotationMinute").Value);
        }

        /// <summary>
        /// Returns true if game day is changed, else returns false
        /// </summary>
        internal static bool IsDayChanged()
        {
            FsmInt GlobalDays = GetGlobalVariable<FsmInt>("GlobalDays");

            if (GlobalDay_cached == -1)
            {
                GlobalDay_cached = GlobalDays.Value;
                return false;
            }

            if (GlobalDay_cached == GlobalDays.Value)
                return false;

            GlobalDay_cached = GlobalDays.Value;
            return true;
        }

        /// <summary>
        /// Returns a string console color from enum implementation
        /// </summary>
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
