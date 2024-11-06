using System;
using System.Linq;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Objects;


namespace Psycho.Internal
{
    internal enum eConsoleColors { WHITE, RED, YELLOW, GREEN }

    internal sealed class Utils
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

            Resources.UnloadAsset(Globals.ScreamCallClip);
            Globals.ScreamCallClip = null;

            Resources.UnloadAsset(Globals.PhantomScreamSound);
            Globals.PhantomScreamSound = null;

            Resources.UnloadAsset(Globals.TVScreamSound);
            Globals.TVScreamSound = null;

            Resources.UnloadAsset(Globals.UncleScreamSound);
            Globals.UncleScreamSound = null;

            Resources.UnloadAsset(Globals.HeartbeatSound);
            Globals.HeartbeatSound = null;

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
        
        internal static void PrintDebug(string msg) =>
            ModConsole.Print(DBG_STRING + msg);

        internal static void PrintDebug(eConsoleColors color, string msg) =>
            ModConsole.Print(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, _getColor(color), msg));

        internal static T GetGlobalVariable<T>(string name) where T : NamedVariable =>
            FsmVariables.GlobalVariables.FindVariable(name) as T;

        internal static void PlayScreamSleepAnim(ref bool animPlayed, Action callback)
        {
            if (animPlayed) return;
            animPlayed = true;

            Logic.shizAnimPlayer.PlayAnimation("sleep_knockout", default, 4f, default, () => callback?.Invoke());
        }

        internal static Vector3[] SetCameraLookAt(Vector3 targetPoint)
        {
            Transform fpsCamera = GetGlobalVariable<FsmGameObject>("POV").Value.transform.parent;

            Vector3[] origs = new Vector3[2] {
                fpsCamera.localPosition,
                fpsCamera.localEulerAngles
            };

            fpsCamera.LookAt(targetPoint);
            return origs;
        }

        internal static void ResetCameraLook(Vector3[] origs)
        {
            Transform fpsCamera = GetGlobalVariable<FsmGameObject>("POV").Value.transform.parent;

            fpsCamera.localPosition = origs[0];
            fpsCamera.localEulerAngles = origs[1];
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
