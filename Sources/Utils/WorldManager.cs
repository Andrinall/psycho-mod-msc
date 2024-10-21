using System;
using System.Linq;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace Psycho
{
    public class ModelData
    {
        public string path;
        public Mesh mesh;
        public Texture texture;
    }

    public class WorldManager
    {
        public static void ActivateDINGONBIISIMiscThing3Permanently()
        {
            var _obj = GameObject.Find("MAP/Buildings/DINGONBIISI");
            if (_obj == null) return;

            var _house = _obj?.transform;
            _house.GetPlayMaker("Clock").GetState("Off").ClearActions();

            var _misc = _house.transform.Find("Misc");
            _misc.gameObject.SetActive(true);

            var _thing3 = _misc.Find("Thing3");
            _thing3.gameObject.SetActive(true);
            _thing3.GetPlayMaker("Distance").GetState("Random").ClearActions(0, 1);

            var _mover = _thing3.Find("Mover");
            _mover.gameObject.SetActive(true);
            _mover.GetPlayMaker("Position").GetState("State 4").ClearActions();
        }
        
        public static GameObject CreateSuicidal(Vector3 position, string name = "") =>
            UnityEngine.Object.Instantiate(Globals.Suicidal_prefab,
                GameObject.Find("PLAYER").transform.position,
                GameObject.Find("PLAYER").transform.rotation) as GameObject;

        public static void AddDoorOpenCallback(string path, Action callback) =>
            StateHook.Inject(GameObject.Find(path).transform.Find("Pivot/Handle").gameObject, "Use", "Open door", callback);

        public static void CloseDoor(string path)
        {
            var door = GameObject.Find(path)?.GetPlayMaker("Use");
            door.GetVariable<FsmBool>("DoorOpen").Value = true;
            door.SendEvent("GLOBALEVENT");
        }

        public static void ChangeCameraFog()
        {
            CameraFog cameraFog = Utils.GetGlobalVariable<FsmGameObject>("POV").Value.GetComponent<CameraFog>();
            FsmVariables rainFogVars = GameObject.Find("PLAYER/Rain").GetPlayMaker("Rain").FsmVariables;

            if (Logic.inHorror)
            {
                _changeFog_Internal(
                    cameraFog, rainFogVars,
                    0.5f, 50f, 0.08f, Color.gray,
                    0.09f, 0.08f
                );
                return;
            }

            _changeFog_Internal(
                cameraFog, rainFogVars,
                0f, 100f, 0.001f, Color.clear,
                0.02f, 0.001f
            );
        }

        public static void ChangeWorldModels(GameObject parent)
        {
            foreach (var item in Globals.models_replaces)
            {
                var obj = parent?.transform?.Find(item.Value.path)?.gameObject;
                if (obj == null) continue;

                if (Logic.inHorror)
                {
                    if (!Globals.models_cached.ContainsKey(item.Key))
                    {
                        Globals.models_cached.Add(item.Key, new ModelData
                        {
                            mesh = obj.GetComponent<MeshFilter>().mesh,
                            texture = obj.GetComponent<MeshRenderer>().materials[0].GetTexture("_MainTex")
                        });
                    }

                    SetMesh(obj, item.Value.mesh);
                    SetMaterial(obj, 0, "", item.Value.texture);
                    continue;
                }

                if (!Globals.models_cached.ContainsKey(item.Key)) continue;

                ModelData data = Globals.models_cached[item.Key];
                SetMesh(obj, data.mesh);
                SetMaterial(obj, 0, "", data.texture);
            }
        }

        public static void ChangeIndepTextures(bool onSave)
        {
            foreach (var renderer in Resources.FindObjectsOfTypeAll<MeshRenderer>())
            {
                if (!_check(renderer)) continue;
                var material = renderer.materials[0];
                var hash = material.mainTexture.name.ToLower().GetHashCode();

                if (!Globals.indep_textures.ContainsKey(hash)) continue;
                if (material.mainTexture == Globals.indep_textures[hash] && !onSave) continue;
                _cache(hash, material.mainTexture);

                material.SetTexture("_MainTex", onSave ? Globals.cached[hash] as Texture : Globals.indep_textures[hash]);
            }
        }

        public static void ChangeWorldTextures(bool state)
        {

            foreach (var renderer in Resources.FindObjectsOfTypeAll<MeshRenderer>())
            {
                if (!_check(renderer)) continue;

                var material = renderer.materials[0];
                var hash = material.mainTexture.name.ToLower().GetHashCode();
                if (!Globals.replaces.ContainsKey(hash)) continue;
                if (material.mainTexture == Globals.replaces[hash] && state) continue;

                _cache(hash, material.mainTexture);
                material.SetTexture("_MainTex", state ? Globals.replaces[hash] : Globals.cached[hash] as Texture);
            }

            foreach (var renderer in Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>())
            {
                if (!_check(renderer)) continue;
                var material = renderer.materials[0];
                var hash = material.mainTexture.name.ToLower().GetHashCode();
                if (!Globals.replaces.ContainsKey(hash)) continue;
                if (material.mainTexture == Globals.replaces[hash]) continue;

                _cache(hash, material.mainTexture);
                material.SetTexture("_MainTex", state ? Globals.replaces[hash] : Globals.cached[hash] as Texture);
            }
        }

        public static void ChangeBedroomModels()
        {
            var state = Logic.inHorror;
            GameObject.Find("YARD/Building/BEDROOM2/bed_base")?.SetActive(!state);

            var coffinsGroup = GameObject.Find("YARD/Building/BEDROOM2").transform.FindChild("BedroomCoffins")?.gameObject;
            if (coffinsGroup == null && state)
            {
                coffinsGroup = new GameObject("BedroomCoffins");

                var coffin1 = (UnityEngine.Object.Instantiate(Globals.Coffin_prefab,
                    new Vector3(-2.456927f, -0.5738183f, 13.52571f),
                    Quaternion.Euler(new Vector3(270f, 180.2751f, 0f))
                ) as GameObject);
                coffin1.transform.SetParent(coffinsGroup.transform, worldPositionStays: false);

                var coffin2 = (UnityEngine.Object.Instantiate(Globals.Coffin_prefab,
                    new Vector3(-2.456927f, -0.5738185f, 12.52524f),
                    Quaternion.Euler(new Vector3(270f, 180.2751f, 0f))
                ) as GameObject);
                coffin2.transform.SetParent(coffinsGroup.transform, worldPositionStays: false);
            }

            coffinsGroup?.SetActive(state);
        }

        public static void ChangeModel(GameObject obj, Mesh mesh, Texture texture)
        {
            SetMesh(obj, mesh);
            SetMaterial(obj, 0, texture.name, texture);
        }

        public static void SetMesh(GameObject obj, Mesh mesh)
        {
            try
            {
                var filter = obj.GetComponent<MeshFilter>();
                if (filter == null) return;

                filter.mesh = mesh;
                filter.sharedMesh = mesh;
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Unable to change mesh for {obj?.name}, mesh {mesh?.name};\n{e.GetFullMessage()}");
            }
        }

        public static void StopCloudsOrRandomize()
        {
            try
            {
                bool state = !Logic.inHorror;
                GameObject clouds = GameObject.Find("MAP/CloudSystem/Clouds");
                PlayMakerFSM cloudsFsm = clouds.GetPlayMaker("Weather");
                FsmState cloudsMove = cloudsFsm.GetState("Move clouds");
                var action1 = (cloudsMove.Actions[1] as FloatAdd);
                var action2 = (cloudsMove.Actions[2] as SetPosition);

                action1.everyFrame = state;
                action1.perSecond = state;

                action2.everyFrame = state;
                action2.lateUpdate = state;

                if (!state)
                    clouds.transform.position = Vector3.zero;
                else
                    cloudsFsm.SendEvent("RANDOMIZE");
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Failed to change clouds after moving between worlds;\n{e.GetFullMessage()}");
            }
        }

        public static void SetMaterial(GameObject obj, int index, string name, Texture texture)
        {
            try
            {
                var renderer = obj.GetComponent<MeshRenderer>();
                if (!renderer) return;

                var material = renderer.materials.ElementAt(index);
                if (name.Length > 0) material.name = name;
                material.mainTexture = texture;
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Unable to change material for {obj?.name}, idx {index}, name {name}, tex {texture?.name};\n{e.GetFullMessage()}");
            }
        }

        static void _changeFog_Internal(
            CameraFog cam, FsmVariables rain,
            float start, float end, float density, Color color,
            float fogOn, float fogOff
        )
        {
            if (cam == null || rain == null) return;
            cam.StartDistance = start;
            cam.EndDistance = end;
            cam.Density = density;
            cam.Color = color;

            rain.GetFsmFloat("FogOn").Value = fogOn;
            rain.GetFsmFloat("FogOff").Value = fogOff;
        }

        static bool _check(Component comp)
        {
            if (comp is MeshRenderer)
            {
                var renderer = comp as MeshRenderer;
                if (renderer == null) return false;
                if (renderer?.materials?.Length == 0) return false;
                if (renderer.materials[0] == null) return false;

                var material = renderer.materials[0];
                if (material.mainTexture == null) return false;
                if (material.mainTexture.name.Length == 0) return false;

                return true;
            }
            else if (comp is SkinnedMeshRenderer)
            {
                var renderer = comp as SkinnedMeshRenderer;
                if (renderer == null) return false;
                if (renderer?.materials?.Length == 0) return false;
                if (renderer.materials[0] == null) return false;

                var material = renderer.materials[0];
                if (material.mainTexture == null) return false;
                if (material.mainTexture.name.Length == 0) return false;

                return true;
            }
            return false;
        }

        static void _cache(int hash, Texture tex)
        {
            if (Globals.cached.ContainsKey(hash)) return;
            Globals.cached.Add(hash, tex);
        }
    }
}
