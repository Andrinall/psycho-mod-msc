using System.Linq;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Adrenaline
{
    internal enum eConsoleColors { WHITE, RED, YELLOW, GREEN }
    internal enum ASIndex : int { HEART10, HEART30, HEART50, HEARTBUST, HEARTSTOP }

    internal static class Utils
    {
        private static readonly string DBG_STRING = "[Adrenaline-DBG]: ";

        /// <summary>
        /// Create a pills with random position in game world
        /// </summary>
        internal static void CreateRandomPills()
        {
        Generate:
            var idx = Random.Range(0, Globals.pills_positions.Count);
            if (Globals.pills_list.Any(v => v.index == idx)) goto Generate;
            Globals.pills_list.Add(new PillsItem(idx, Globals.pills_positions.ElementAtOrDefault(idx)));

            var Image = Resources.FindObjectsOfTypeAll<GameObject>()
                .First(v => v.name == "Sheets").transform
                .Find("DoctorMail/Background/Image");

            var texture = Globals.mailScreens.ElementAtOrDefault(idx);
            Image.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
        }

        /// <summary>
        /// Create a custom poster with specified position & rotation,
        /// 
        /// </summary>
        internal static void CreatePoster(int textureIndex, Vector3 position, Quaternion rotation)
        {
            if (textureIndex < 0 || textureIndex >= Globals.poster_textures.Count)
                throw new System.IndexOutOfRangeException($"Use texture index from 0 to {Globals.poster_textures.Count-1}");

            var poster = PrefabManager.Instantiate(Globals.poster);
            poster.gameObject.name = "AdrenalineADV_Poster";
            poster.transform.position = position;
            poster.transform.rotation = rotation;
            poster.GetComponent<MeshRenderer>().material
                .SetTexture("_MainTex", Globals.poster_textures.ElementAtOrDefault(textureIndex));
        }

        /// <summary>
        /// Play hearth sound from list by index
        /// </summary>
        internal static void PlaySound(ASIndex index)
        {
            StopAllAudios((int)index);
            var item = Globals.audios.ElementAt((int)index);
            if (item.isPlaying) return;
            item.Play();
        }

        /// <summary>
        /// Play a sound after "Activate Dead Body" from game
        /// </summary>
        internal static void PlayDeathSound()
        {
            Globals.audios.ElementAt((int)ASIndex.HEARTSTOP)
                .gameObject.transform.position = GameObject.Find("PLAYER").transform.position;

            PlaySound(ASIndex.HEARTSTOP);
        }

        /// <summary>
        /// Stop all custom hearth sounds
        /// </summary>
        internal static void StopAllAudios(int index = -1)
        {
            var blacklist = Globals.audios.ElementAtOrDefault(index);
            var list = Globals.audios.Where(v => v.isPlaying && v.clip.name != blacklist.clip.name);
            
            foreach (var item in list) item.Stop();
        }

        /// <summary>
        /// Easy gets a fsm global variable by type & name
        /// </summary>
        internal static T GetGlobalVariable<T>(string name) where T : NamedVariable
        {
            return FsmVariables.GlobalVariables.FindVariable(name) as T;
        }

        /// <summary>
        /// Returns ingame car name by GameObject
        /// (used for compare global variable "PlayerCurrentVehicle" & car name from GameObject)
        /// </summary>
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

        /// <summary>
        /// Print debug message.
        /// </summary>
        internal static void PrintDebug(string msg)
        {
            ModConsole.Print(DBG_STRING + msg);
        }

        /// <summary>
        /// Print colored debug
        /// </summary>
        internal static void PrintDebug(eConsoleColors color, string msg)
        {
            ModConsole.Print(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, GetColor(color), msg));
        }

        /// <summary>
        /// Replace a mesh & texture for gameobject
        /// </summary>
        internal static void ChangeModel(GameObject obj, Mesh mesh, Texture texture, Vector2 offset, Vector2 scale)
        {
            SetMesh(obj, mesh);
            SetMaterial(obj, 0, texture.name, texture, offset, scale);
        }

        /// <summary>
        /// Sets a object mesh in MeshFilter component
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        internal static bool IsPrefab(this Transform tempTrans)
        {
            if (!tempTrans.gameObject.activeInHierarchy && tempTrans.gameObject.activeSelf)
            {
                return tempTrans.root == tempTrans;
            }
            return false;
        }
    }
}
