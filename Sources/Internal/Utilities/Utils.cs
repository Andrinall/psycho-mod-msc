
using System;
using System.Linq;
using System.Reflection;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    enum eConsoleColors { WHITE, RED, YELLOW, GREEN }

    static class Utils
    {
        static string DBG_STRING => $"[{Psycho.Instance.Name}{{{Psycho.Instance.Version}}}]: ";

        public static bool WaitFrames(ref int elapsedFrames, int neededFrames)
        {
            if (elapsedFrames < neededFrames)
            {
                elapsedFrames++;
                return false;
            }

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

        public static string GetMethodPath(MethodInfo method)
        {
            Type _declaringType = method.DeclaringType;
            return $"{_declaringType.Namespace}::{_declaringType.Name}.{method.Name}";
        }

        public static void SetGUIUse(bool state, string name = "")
        {
            Globals.GUIuse.Value = state;
            Globals.GUIinteraction.Value = name;
        }

        public static void ChangeSmokingModel()
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
                PrintDebug(eConsoleColors.RED, "Failed to change cigarette model after moving between worlds;");
                ModConsole.Error(e.GetFullMessage());
            }
        }

        public static void SetPictureImage()
        {
            GameObject _picture = GameObject.Find("Picture(Clone)");
            if (_picture == null) return;

            int _idx = Mathf.FloorToInt(Logic.Points >= 0f ? 0f : -Logic.Points);
            Texture _texture = ResourcesStorage.Pictures.ElementAtOrDefault(_idx);
            if (_texture == null) return;

            Material _material = _picture.GetComponent<MeshRenderer>().materials[1];
            if (_material.GetTexture("_MainTex")?.name == _texture.name) return;

            PrintDebug($"SetPictureImage [{_idx}]");
            _material.SetTexture("_MainTex", _texture);
        }

        public static T GetGlobalVariable<T>(string name) where T : NamedVariable
            => FsmVariables.GlobalVariables.FindVariable(name) as T;

        public static void PlayScreamSleepAnim(ref bool animPlayed, Action callback)
        {
            if (animPlayed) return;
            animPlayed = true;

            ShizAnimPlayer.PlayOriginalAnimation("sleep_knockout", 4f, default, () => callback?.Invoke());
        }

        public static Vector3[] SetCameraLookAt(Vector3 targetPoint)
        {
            Transform _fpsCamera = GetGlobalVariable<FsmGameObject>("POV").Value.transform.parent;

            Vector3[] _origs = new Vector3[2] {
                _fpsCamera.localPosition,
                _fpsCamera.localEulerAngles
            };

            _fpsCamera.LookAt(targetPoint);
            return _origs;
        }

        public static void ResetCameraLook(Vector3[] origs)
        {
            Transform _fpsCamera = GetGlobalVariable<FsmGameObject>("POV").Value.transform.parent;

            _fpsCamera.localPosition = origs[0];
            _fpsCamera.localEulerAngles = origs[1];
        }

        public static void PrintDebug(string msg)
        {
#if DEBUG
            ModConsole.Print(DBG_STRING + msg);
#else
            Debug.Log(DBG_STRING + msg);
#endif
        }

        public static void PrintDebug(eConsoleColors color, string msg)
        {
#if DEBUG
            ModConsole.Print(string.Format("{0}<color={1}>{2}</color>", DBG_STRING, _getColor(color), msg));
#else
            string message = DBG_STRING + msg;
            if (color == eConsoleColors.RED)
                ModConsole.LogError(message);
            else if (color == eConsoleColors.YELLOW)
                ModConsole.LogWarning(message);
            else
                Debug.Log(message);
#endif
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
