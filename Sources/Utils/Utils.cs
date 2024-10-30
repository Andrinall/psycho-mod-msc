using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Objects;

namespace Psycho.Internal
{
    internal enum eConsoleColors { WHITE, RED, YELLOW, GREEN }

    internal static class Utils
    {
        static readonly string DBG_STRING = "[Shiz-DBG]: ";

        internal static void ChangeSmokingModel()
        {
            try
            {
                GameObject smoking = GetGlobalVariable<FsmGameObject>("POV").Value
                    ?.transform?.Find("Smoking/Hand/HandSmoking")?.gameObject;

                smoking?.transform
                    ?.Find("Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Shaft/RedHot")
                    ?.gameObject?.SetActive(!Logic.inHorror);

                WorldManager.ChangeWorldModels(smoking.gameObject);
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Failed to change cigarette model after moving between worlds;\n{e.GetFullMessage()}");
            }
        }

        internal static void CreateRandomPills()
        {
            try
            {
            Generate:
                int idx = UnityEngine.Random.Range(0, Globals.pills_positions.Count - 1);
                if (Globals.pills_list.Any(v => v.index == idx))
                    goto Generate;

                Globals.pills_list.Add(new PillsItem(idx, Globals.pills_positions.ElementAt(idx)));

                Transform Image = Globals.mailboxSheet.transform.Find("Background/Image");
                Texture texture = Globals.mailScreens.Find(v => v.name == idx.ToString());
                Image.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);

                PrintDebug(eConsoleColors.YELLOW, $"Generated pills: {idx}, {Image.name}");
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Failed to create a random pills;\n{e.GetFullMessage()}");
            }
        }

        internal static void SetPictureImage()
        {
            GameObject picture = GameObject.Find("picture(Clone)");
            if (picture == null) return;

            int idx = Mathf.FloorToInt(Logic.Points >= 0f ? 0f : -Logic.Points);
            Texture texture = Globals.pictures.ElementAtOrDefault(idx);
            if (texture == null) return;

            Material material = picture.GetComponent<MeshRenderer>().materials[1];
            if (material.GetTexture("_MainTex")?.name == texture.name) return;

            PrintDebug($"SetPictureImage [{idx}]");
            material.SetTexture("_MainTex", texture);
        }

        internal static void FreeResources()
        {
            Logic.death = null;
            Logic.knockOut = null;
            Logic.shizAnimPlayer = null;
            Logic._hud = null;

            Resources.UnloadAsset(Globals.Suicidal_prefab);
            Globals.Suicidal_prefab = null;

            Resources.UnloadAsset(Globals.AcidBurnSound);
            Globals.AcidBurnSound = null;
            
            Resources.UnloadAsset(Globals.SmokeParticleSystem_prefab);
            Globals.SmokeParticleSystem_prefab = null;

            Resources.UnloadAsset(Globals.Background_prefab);
            Globals.Background_prefab = null;

            Resources.UnloadAsset(Globals.Pills_prefab);
            Globals.Pills_prefab = null;

            Resources.UnloadAsset(Globals.mailboxSheet);
            Globals.mailboxSheet = null;

            Resources.UnloadAsset(Globals.envelopeObject);
            Globals.envelopeObject = null;

            Resources.UnloadAsset(Globals.Crow_prefab);
            Globals.Crow_prefab = null;

            Resources.UnloadAsset(SoundManager.DeathSound);
            SoundManager.DeathSound = null;

            SoundManager.ScreamPoints.ForEach(v => UnityEngine.Object.Destroy(v));
            SoundManager.ScreamPoints.Clear();

            Resources.UnloadAsset(Globals.Picture_prefab);
            Globals.Picture_prefab = null;

            Resources.UnloadAsset(Globals.Coffin_prefab);
            Globals.Coffin_prefab = null;

            Globals.pills_list.Clear();
            Globals.models_cached.Clear();
            Globals.flies_cached.Clear();
            Globals.cached.Clear();
            
            Globals.mailScreens.ForEach(v => Resources.UnloadAsset(v));
            Globals.mailScreens.Clear();

            foreach (var item in Globals.models_replaces)
            {
                Resources.UnloadAsset(item.Value.mesh);
                Resources.UnloadAsset(item.Value.texture);
            }
            Globals.models_replaces.Clear();

            foreach (var item in Globals.replaces)
                Resources.UnloadAsset(item.Value);
            Globals.replaces.Clear();

            foreach (var item in Globals.indep_textures)
                Resources.UnloadAsset(item.Value);
            Globals.indep_textures.Clear();

            Globals.horror_flies.ForEach(v => Resources.UnloadAsset(v));
            Globals.horror_flies.Clear();
        }

        internal static void ClearActions(Transform obj, string fsm, string state, int index = -1, int count = -1)
        {
            try
            {
                obj?.GetPlayMaker(fsm)?.GetState(state)?.ClearActions(index, count);
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"[1] Failed to clears actions;\n{e.GetFullMessage()}");
            }
        }

        internal static void ClearActions(this FsmState state, int index = -1, int count = -1)
        {
            try
            {
                var list = state.Actions.ToList();
                if (index == -1 && count == -1)
                    list.Clear();
                else if (index != -1 && count == -1)
                    list.RemoveRange(index, 1);
                else
                    list.RemoveRange(index, count);

                state.Actions = list.ToArray();
                state.SaveActions();
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"[2] Failed to clears actions;\n{e.GetFullMessage()}");
            }
        }
        
        internal static void PrintDebug(string msg) =>
            ModConsole.Print(DBG_STRING + msg);

        internal static void PrintDebug(eConsoleColors color, string msg) =>
            ModConsole.Print(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, _getColor(color), msg));

        internal static T GetGlobalVariable<T>(string name) where T : NamedVariable =>
            FsmVariables.GlobalVariables.FindVariable(name) as T;


        internal static void AddEvent(this PlayMakerFSM fsm, string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            fsm.Fsm.Events = new List<FsmEvent>(fsm.Fsm.Events)
                { new FsmEvent(eventName) }.ToArray();
        }

        internal static bool IsPrefab(this Transform tempTrans)
        {
            if (tempTrans.gameObject.activeInHierarchy && !tempTrans.gameObject.activeSelf) return false;
            return tempTrans.root == tempTrans;
        }

        internal static void IterateAllChilds(this Transform obj, Action<Transform> handler)
        {
            if (obj.childCount == 0) return;
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                handler?.Invoke(child);

                if (child.childCount == 0) continue;
                child.IterateAllChilds(handler);
            }
        }

        static string _getColor(eConsoleColors color)
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
