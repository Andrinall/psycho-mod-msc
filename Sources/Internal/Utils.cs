
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    internal enum eConsoleColors { WHITE, RED, YELLOW, GREEN }

    internal static class Utils
    {
        static readonly string DBG_STRING = "[Shiz-DBG]: ";

        static FsmBool GUIuse;
        static FsmString GUIinteraction;

        public static bool WaitFrames(ref int elapsedFrames, int neededFrames)
        {
            if (elapsedFrames < neededFrames)
            {
                elapsedFrames++;
                return false;
            }

            elapsedFrames = 0;
            return true;
        }

        public static string[] GetEnumFields<T>()
        {
            var fields = typeof(T).GetFields();
            string[] result = new string[fields.Length];

            for (int i = 0; i < result.Length; i++)
            {
                string name = fields[i].Name;
                if (name == "value__") continue;

                result[i - 1] = fields[i].Name;
            }

            return result;
        }

        internal static string GetMethodPath(MethodInfo method)
        {
            Type declaringType = method.DeclaringType;
            return $"{declaringType.Namespace}::{declaringType.Name}.{method.Name}";
        }

        internal static void SetupFSM(GameObject obj, string name, string[] eventNames, string startState, Func<PlayMakerFSM, List<FsmEvent>, FsmState[]> callback)
        {
            List<FsmEvent> events = new List<FsmEvent>();
            PlayMakerFSM fsm = obj.AddComponent<PlayMakerFSM>();

            fsm.InitializeFSM();
            fsm.enabled = false;
            fsm.Fsm.Name = name;

            foreach (string _ev in eventNames)
                events.Add(fsm.AddEvent(_ev));

            FsmState[] finalStates = callback?.Invoke(fsm, events);
            if (finalStates == null || finalStates.Length == 0)
            {
                UnityEngine.Object.Destroy(fsm);
                return;
            }

            fsm.Fsm.States = (FsmState[])finalStates.Clone();
            fsm.Fsm.StartState = startState;
            fsm.Fsm.Start();
            fsm.enabled = true;
        }

        internal static void SetGUIUse(bool state, string name = "")
        {
            if (!CheckGUIFsm()) return;

            GUIuse.Value = state;
            GUIinteraction.Value = name;
        }

        static bool CheckGUIFsm()
        {
            if (GUIuse == null)
                GUIuse = GetGlobalVariable<FsmBool>("GUIuse");
            if (GUIinteraction == null)
                GUIinteraction = GetGlobalVariable<FsmString>("GUIinteraction");

            return GUIuse != null && GUIinteraction != null;
        }

        internal static void InitPostcard(GameObject cloned)
        {
            Shader text3D = Shader.Instantiate(Shader.Find("GUI/3D Text Shader"));
            Transform text = cloned.transform.Find("Text");

            Material material = text.GetComponent<MeshRenderer>().material;
            material.shader = text3D;
            material.color = new Color(0.0353f, 0.1922f, 0.3882f);

            TextMesh textMesh = text.GetComponent<TextMesh>();
            textMesh.text = Locales.POSTCARD_TEXT[Globals.CurrentLang];
        }

        internal static void ChangeSmokingModel()
        {
            try
            {
                GameObject smoking = GetGlobalVariable<FsmGameObject>("POV").Value
                    ?.transform?.Find("Smoking/Hand/HandSmoking")?.gameObject;

                smoking?.transform
                    ?.Find("Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Shaft/RedHot")
                    ?.gameObject?.SetActive(!Logic.InHorror);

                WorldManager.ChangeWorldModels(smoking.gameObject);
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to change cigarette model after moving between worlds;\n{e.GetFullMessage()}");
            }
        }

        internal static void SetPictureImage()
        {
            GameObject picture = GameObject.Find("Picture(Clone)");
            if (picture == null) return;

            int idx = Mathf.FloorToInt(Logic.Points >= 0f ? 0f : -Logic.Points);
            Texture texture = Globals.Pictures.ElementAtOrDefault(idx);
            if (texture == null) return;

            Material material = picture.GetComponent<MeshRenderer>().materials[1];
            if (material.GetTexture("_MainTex")?.name == texture.name) return;

            PrintDebug(eConsoleColors.YELLOW, $"SetPictureImage [{idx}]");
            material.SetTexture("_MainTex", texture);
        }

        internal static void FreeResources()
        {
            foreach (var field in typeof(Globals).GetFields())
            {
                string fieldName = field.Name;
                string fieldType = field.FieldType.Name;
                if (!fieldName.Contains("_clip") && !fieldName.Contains("_prefab") && !fieldName.Contains("_texture")) continue;

                switch (fieldType)
                {
                    case "GameObject":
                        Resources.UnloadAsset((GameObject)field.GetValue(null));
                        field.SetValue(null, null);
                        break;
                    case "AudioClip":
                        Resources.UnloadAsset((AudioClip)field.GetValue(null));
                        field.SetValue(null, null);
                        break;
                    case "Texture":
                        Resources.UnloadAsset((Texture)field.GetValue(null));
                        field.SetValue(null, null);
                        break;
                }
            }

            Resources.UnloadAsset(Globals.Heartbeat_source);
            Globals.Heartbeat_source = null;

            Resources.UnloadAsset(Globals.MailboxSheet);
            Globals.MailboxSheet = null;

            Resources.UnloadAsset(Globals.EnvelopeObject);
            Globals.EnvelopeObject = null;

            Resources.UnloadAsset(SoundManager.DeathSound);
            SoundManager.DeathSound = null;

            Globals.ModelsCached.Clear();
            Globals.FliesCached.Clear();
            Globals.Cached.Clear();
            
            SoundManager.ScreamPoints.Clear();
            TexturesManager.Cache.Clear();

            UnityEngine.Object.Destroy(SoundManager.FullScreenScreamerSoundsSource);
            SoundManager.FullScreenScreamerSoundsSource = null;
            SoundManager.FullScreenScreamersSounds.Clear();

            Globals.FullScreenScreamer = null;
            Globals.FullScreenScreamerTextures.Clear();

            foreach (var item in Globals.ModelsReplaces)
            {
                Resources.UnloadAsset(item.Value.mesh);
                Resources.UnloadAsset(item.Value.texture);
            }
            Globals.ModelsReplaces.Clear();
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

            ShizAnimPlayer.PlayOriginalAnimation("sleep_knockout", 4f, default, () => callback?.Invoke());
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
