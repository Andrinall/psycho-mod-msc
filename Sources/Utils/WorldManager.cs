using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Screamers;
using Object = UnityEngine.Object;

namespace Psycho.Internal
{
    internal class WorldManager
    {
        public static AnimationClip PigWalkAnimation;
        public static GameObject ClonedGrannyHiker;

        public static void CopyGrannyHiker()
        {
            GameObject _hiker = GameObject.Find("ChurchGrandma/GrannyHiker");
            ClonedGrannyHiker = GameObject.Instantiate(_hiker);
            ClonedGrannyHiker.transform.parent = null;
            ClonedGrannyHiker.name = "GrannyScreamHiker";

            var _char = ClonedGrannyHiker.transform.Find("Char");
            var _head = _char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");

            Object.Destroy(ClonedGrannyHiker.transform.GetPlayMaker("Logic"));
            Object.Destroy(_char.Find("HeadTarget/LookAt").GetPlayMaker("Random"));
            Object.Destroy(_head.GetPlayMaker("Look"));
            Object.Destroy(ClonedGrannyHiker.transform.Find("Ray").gameObject);
            Object.Destroy(ClonedGrannyHiker.transform.Find("RagDoll2").gameObject);
            Object.Destroy(_char.Find("RagDollCar").gameObject);
            Object.Destroy(_char.Find("HeadTarget").gameObject);
            Object.Destroy(_char.Find("HumanTriggerCrime").gameObject);

            Animation _animation = _char.Find("skeleton").GetComponent<Animation>();
            if (!_animation.GetClip("venttipig_pig_walk"))
                _animation.AddClip(PigWalkAnimation, "venttipig_pig_walk");

            _animation.clip = _animation.GetClip("venttipig_pig_walk");
            _animation.playAutomatically = true;
            _animation.Play("venttipig_pig_walk", PlayMode.StopAll);

            (ClonedGrannyHiker.GetComponent<MummolaCrawl>() ?? ClonedGrannyHiker.AddComponent<MummolaCrawl>()).enabled = false;
        }

        public static void SetHandsActive(bool state)
        {
            GameObject.Find("MAP/Buildings/DINGONBIISI/Hands")?.SetActive(state);

            GameObject _chair = GameObject.Find("MAP/Buildings/DINGONBIISI/autiotalo/LOD/Chair");
            _chair.GetComponent<Rigidbody>().useGravity = !state;
            _chair.transform.localPosition = new Vector3(-1.84862494f, -0.92900008f, -0.122052498f);
            _chair.transform.localEulerAngles = new Vector3(336.172913f, 190.832809f, 268.149536f);
        }

        public static void SpawnDINGONBIISIHands()
        {
            // setup hierarchy
            var parents = new List<GameObject>
            {
                new GameObject("Hands"),
                new GameObject("Stairs"),
                new GameObject("House"),
                new GameObject("Loft")
            };

            Transform _dingo = GameObject.Find("MAP/Buildings/DINGONBIISI").transform;
            Transform _main = parents[(byte)HandParent.MAIN].transform;
            _main.SetParent(_dingo, worldPositionStays: false);
            _main.gameObject.SetActive(false);

            for (int i = 1; i < 4; i++)
                parents[i].transform.SetParent(parents[(byte)HandParent.MAIN].transform, worldPositionStays: false);

            // get orig objects
            Transform _camera = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").transform;
            var objs = new List<GameObject>
            {
                _camera.Find("Hand Push").gameObject,
                _camera.Find("Hello").gameObject,
                _camera.Find("Watch").gameObject,
                _camera.Find("Drink/Hand/HandMilk").gameObject
            };

            // spawn objects
            Globals.hands_list.ForEach(v =>
            {
                GameObject clone = GameObject.Instantiate(objs[(byte)v.orig]);
                Transform t = clone.transform;

                t.SetParent(parents[(byte)v.parent].transform, worldPositionStays: false);
                t.localPosition = v.position;
                t.localEulerAngles = v.euler;
                t.localScale = new Vector3(v.scale, v.scale, v.scale);

                switch (v.orig)
                {
                    case HandOrig.MILK:
                        Object.Destroy(t.Find("Milk").gameObject);
                        break;
                    case HandOrig.PUSH:
                        Object.Destroy(t.Find("Pivot/Collider").gameObject);
                        Object.Destroy(t.Find("Pivot/RigidBody").gameObject);
                        break;
                    case HandOrig.WATCH:
                        t.Find("Animate/BreathAnim/WristwatchHand").gameObject.SetActive(true);
                        break;
                }

                clone.layer = 0;
                clone.transform.IterateAllChilds(child => child.gameObject.layer = 0);
                clone.SetActive(true);
            });
        }

        public static void CopyVenttiAnimation()
        {
            if (PigWalkAnimation) return;
            
            PigWalkAnimation = AnimationClip.Instantiate
            (
                GameObject.Find("CABIN/Cabin/Ventti/PIG/VenttiPig/Pivot/Char/skeleton")
                    .GetComponent<Animation>()
                    .GetClip("venttipig_pig_walk")
            );
        }

        public static void ChangeWalkersAnimation()
        {
            Transform _walkers = GameObject.Find("HUMANS/Randomizer/Walkers").transform;
            _walkers.gameObject.SetActive(false);
            for (int i = 0; i < _walkers.childCount; i++)
            {
                Transform _child = _walkers.GetChild(i);
                Animation _anim = _child.Find("Pivot/Char/skeleton").GetComponent<Animation>();
                if (!_anim.GetClip("venttipig_pig_walk"))
                    _anim.AddClip(PigWalkAnimation, "venttipig_pig_walk");

                (_child.GetPlayMaker("Move").GetState("Walking").Actions[0] as PlayAnimation).animName.Value =
                    Logic.inHorror ? "venttipig_pig_walk" : "fat_walk";
            }
            _walkers.gameObject.SetActive(true);
        }

        public static void ActivateDINGONBIISIMiscThing3Permanently()
        {
            GameObject _obj = GameObject.Find("MAP/Buildings/DINGONBIISI");
            if (_obj == null) return;

            Transform _house = _obj?.transform;
            _house.GetPlayMaker("Clock").GetState("Off").ClearActions();

            Transform _misc = _house.transform.Find("Misc");
            _misc.gameObject.SetActive(true);

            Transform _thing3 = _misc.Find("Thing3");
            _thing3.gameObject.SetActive(true);
            _thing3.GetPlayMaker("Distance").GetState("Random").ClearActions(0, 1);

            Transform _mover = _thing3.Find("Mover");
            _mover.gameObject.SetActive(true);
            _mover.GetPlayMaker("Position").GetState("State 4").ClearActions();
        }

        public static GameObject CreateSuicidal(Vector3 position, string name = "")
        {
            GameObject list = GameObject.Find("SuicidalList");
            if (!list)
                list = new GameObject("SuicidalList");

            GameObject newchild = UnityEngine.Object.Instantiate(Globals.Suicidal_prefab,
                GameObject.Find("PLAYER").transform.position,
                GameObject.Find("PLAYER").transform.rotation) as GameObject;

            newchild.transform.SetParent(list.transform, worldPositionStays: false);
            return newchild;
        }

        public static void AddDoorOpenCallback(string path, Action<PlayMakerFSM> callback) =>
            StateHook.Inject(GameObject.Find(path).transform.Find("Pivot/Handle").gameObject, "Use", "Open door", callback);

        public static void CloseDoor(string path)
        {
            PlayMakerFSM door = GameObject.Find(path)?.GetPlayMaker("Use");
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
                GameObject obj = parent?.transform?.Find(item.Value.path)?.gameObject;
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
            foreach (MeshRenderer renderer in Resources.FindObjectsOfTypeAll<MeshRenderer>())
            {
                if (!_check(renderer)) continue;
                Material material = renderer.materials[0];
                int hash = material.mainTexture.name.ToLower().GetHashCode();

                if (!Globals.indep_textures.ContainsKey(hash)) continue;
                if (material.mainTexture == Globals.indep_textures[hash] && !onSave) continue;
                _cache(hash, material.mainTexture);

                material.SetTexture("_MainTex", onSave ? Globals.cached[hash] as Texture : Globals.indep_textures[hash]);
            }
        }

        public static void ChangeWorldTextures(bool state)
        {

            foreach (MeshRenderer renderer in Resources.FindObjectsOfTypeAll<MeshRenderer>())
            {
                if (!_check(renderer)) continue;

                Material material = renderer.materials[0];
                int hash = material.mainTexture.name.ToLower().GetHashCode();
                if (!Globals.replaces.ContainsKey(hash)) continue;
                if (material.mainTexture == Globals.replaces[hash] && state) continue;

                _cache(hash, material.mainTexture);
                material.SetTexture("_MainTex", state ? Globals.replaces[hash] : Globals.cached[hash] as Texture);
            }

            foreach (SkinnedMeshRenderer renderer in Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>())
            {
                if (!_check(renderer)) continue;
                Material material = renderer.materials[0];
                int hash = material.mainTexture.name.ToLower().GetHashCode();
                if (!Globals.replaces.ContainsKey(hash)) continue;
                if (material.mainTexture == Globals.replaces[hash]) continue;

                _cache(hash, material.mainTexture);
                material.SetTexture("_MainTex", state ? Globals.replaces[hash] : Globals.cached[hash] as Texture);
            }
        }

        public static void ChangeBedroomModels()
        {
            bool state = Logic.inHorror;
            GameObject.Find("YARD/Building/BEDROOM2/bed_base")?.SetActive(!state);

            GameObject coffinsGroup = GameObject.Find("YARD/Building/BEDROOM2").transform.FindChild("BedroomCoffins")?.gameObject;
            if (coffinsGroup == null && state)
            {
                coffinsGroup = new GameObject("BedroomCoffins");
                GameObject coffin1 = (Object.Instantiate(Globals.Coffin_prefab,
                    new Vector3(-2.456927f, -0.5738183f, 13.52571f),
                    Quaternion.Euler(new Vector3(270f, 180.2751f, 0f))
                ) as GameObject);
                coffin1.transform.SetParent(coffinsGroup.transform, worldPositionStays: false);

                GameObject coffin2 = (Object.Instantiate(Globals.Coffin_prefab,
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
                MeshFilter filter = obj.GetComponent<MeshFilter>();
                if (filter == null) return;

                filter.mesh = mesh;
                filter.sharedMesh = mesh;
            }
            catch (Exception e)
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
                
                FloatAdd action1 = (cloudsMove.Actions[1] as FloatAdd);
                SetPosition action2 = (cloudsMove.Actions[2] as SetPosition);

                action1.everyFrame = state;
                action1.perSecond = state;

                action2.everyFrame = state;
                action2.lateUpdate = state;

                if (!state)
                    clouds.transform.position = Vector3.zero;
                else
                    cloudsFsm.SendEvent("RANDOMIZE");
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to change clouds after moving between worlds;\n{e.GetFullMessage()}");
            }
        }

        public static void SetMaterial(GameObject obj, int index, string name, Texture texture)
        {
            try
            {
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if (!renderer) return;

                Material material = renderer.materials.ElementAt(index);
                if (name.Length > 0) material.name = name;
                material.mainTexture = texture;
            }
            catch (Exception e)
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
                MeshRenderer renderer = (MeshRenderer)comp;
                if (renderer == null) return false;
                if (renderer?.materials?.Length == 0) return false;
                if (renderer.materials[0] == null) return false;

                Material material = renderer.materials[0];
                if (material.mainTexture == null) return false;
                if (material.mainTexture.name.Length == 0) return false;

                return true;
            }
            else if (comp is SkinnedMeshRenderer)
            {
                SkinnedMeshRenderer renderer = (SkinnedMeshRenderer)comp;
                if (renderer == null) return false;
                if (renderer?.materials?.Length == 0) return false;
                if (renderer.materials[0] == null) return false;

                Material material = renderer.materials[0];
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
