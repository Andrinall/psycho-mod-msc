
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

        static FsmBool guiUse;
        static FsmString guiInteraction;

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
            FieldInfo[] _fields = typeof(T).GetFields();
            string[] _result = new string[_fields.Length];

            for (int i = 0; i < _result.Length; i++)
            {
                string _name = _fields[i].Name;
                if (_name == "value__") continue;

                _result[i - 1] = _fields[i].Name;
            }

            return _result;
        }

        internal static string GetMethodPath(MethodInfo method)
        {
            Type _declaringType = method.DeclaringType;
            return $"{_declaringType.Namespace}::{_declaringType.Name}.{method.Name}";
        }

        internal static void SetupFSM(GameObject obj, string name, string[] eventNames, string startState, Func<PlayMakerFSM, List<FsmEvent>, FsmState[]> callback)
        {
            List<FsmEvent> _events = new List<FsmEvent>();
            PlayMakerFSM _fsm = obj.AddComponent<PlayMakerFSM>();

            _fsm.InitializeFSM();
            _fsm.enabled = false;
            _fsm.Fsm.Name = name;

            foreach (string _ev in eventNames)
                _events.Add(_fsm.AddEvent(_ev));

            FsmState[] finalStates = callback?.Invoke(_fsm, _events);
            if (finalStates == null || finalStates.Length == 0)
            {
                UnityEngine.Object.Destroy(_fsm);
                return;
            }

            _fsm.Fsm.States = (FsmState[])finalStates.Clone();
            _fsm.Fsm.StartState = startState;
            _fsm.Fsm.Start();
            _fsm.enabled = true;
        }

        internal static void SetGUIUse(bool state, string name = "")
        {
            if (!CheckGUIFsm()) return;

            guiUse.Value = state;
            guiInteraction.Value = name;
        }

        static bool CheckGUIFsm()
        {
            if (guiUse == null)
                guiUse = GetGlobalVariable<FsmBool>("GUIuse");
            if (guiInteraction == null)
                guiInteraction = GetGlobalVariable<FsmString>("GUIinteraction");

            return guiUse != null && guiInteraction != null;
        }

        internal static void InitPostcard(GameObject cloned)
        {
            Shader _text3D = Shader.Instantiate(Shader.Find("GUI/3D Text Shader"));
            Transform _text = cloned.transform.Find("Text");

            Material _material = _text.GetComponent<MeshRenderer>().material;
            _material.shader = _text3D;
            _material.color = new Color(0.0353f, 0.1922f, 0.3882f);

            TextMesh _textMesh = _text.GetComponent<TextMesh>();
            _textMesh.text = Locales.POSTCARD_TEXT[Globals.CurrentLang];
        }

        internal static void ChangeSmokingModel()
        {
            try
            {
                GameObject _smoking = GetGlobalVariable<FsmGameObject>("POV").Value
                    ?.transform?.Find("Smoking/Hand/HandSmoking")?.gameObject;

                _smoking?.transform
                    ?.Find("Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Shaft/RedHot")
                    ?.gameObject?.SetActive(!Logic.InHorror);

                WorldManager.ChangeWorldModels(_smoking.gameObject);
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to change cigarette model after moving between worlds;\n{e.GetFullMessage()}");
            }
        }

        internal static void SetPictureImage()
        {
            GameObject _picture = GameObject.Find("Picture(Clone)");
            if (_picture == null) return;

            int _idx = Mathf.FloorToInt(Logic.Points >= 0f ? 0f : -Logic.Points);
            Texture _texture = Globals.Pictures.ElementAtOrDefault(_idx);
            if (_texture == null) return;

            Material _material = _picture.GetComponent<MeshRenderer>().materials[1];
            if (_material.GetTexture("_MainTex")?.name == _texture.name) return;

            PrintDebug(eConsoleColors.YELLOW, $"SetPictureImage [{_idx}]");
            _material.SetTexture("_MainTex", _texture);
        }

        internal static void FreeResources()
        {
            foreach (var _field in typeof(Globals).GetFields())
            {
                string _fieldName = _field.Name;
                string _fieldType = _field.FieldType.Name;
                if (!_fieldName.Contains("_clip") && !_fieldName.Contains("_prefab") && !_fieldName.Contains("_texture")) continue;

                switch (_fieldType)
                {
                    case "GameObject":
                        Resources.UnloadAsset((GameObject)_field.GetValue(null));
                        _field.SetValue(null, null);
                        break;
                    case "AudioClip":
                        Resources.UnloadAsset((AudioClip)_field.GetValue(null));
                        _field.SetValue(null, null);
                        break;
                    case "Texture":
                        Resources.UnloadAsset((Texture)_field.GetValue(null));
                        _field.SetValue(null, null);
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

            foreach (var _item in Globals.ModelsReplaces)
            {
                Resources.UnloadAsset(_item.Value.mesh);
                Resources.UnloadAsset(_item.Value.texture);
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
            Transform _fpsCamera = GetGlobalVariable<FsmGameObject>("POV").Value.transform.parent;

            Vector3[] _origs = new Vector3[2] {
                _fpsCamera.localPosition,
                _fpsCamera.localEulerAngles
            };

            _fpsCamera.LookAt(targetPoint);
            return _origs;
        }

        internal static void ResetCameraLook(Vector3[] origs)
        {
            Transform _fpsCamera = GetGlobalVariable<FsmGameObject>("POV").Value.transform.parent;

            _fpsCamera.localPosition = origs[0];
            _fpsCamera.localEulerAngles = origs[1];
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
