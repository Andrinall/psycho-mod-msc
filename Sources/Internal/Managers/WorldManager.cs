
using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    internal static class WorldManager
    {
        static int elapsedFrames = 0;
         
        public static void ShowCrows(bool state)
        {
            if (Globals.CrowsList.activeSelf == state) return;
            Globals.CrowsList.SetActive(state);
        }

        public static void SpawnPhantomBehindPlayer(float distance = 0.75f)
        {
            elapsedFrames = 0;

            Transform _player = Globals.Player;
            var _cloned = ObjectCloner.ClonedPhantom;
            _cloned.transform.position = _player.position - _player.forward * distance;
            _cloned.transform.LookAt(_player.position);
            _cloned.SetActive(true);

            SoundManager.PlayHeartbeat(true);
        }

        public static bool ClonedPhantomTick(int neededFrames, Action callback = null)
        {
            if (!ObjectCloner.ClonedPhantom.activeSelf) return false;

            if (!Utils.WaitFrames(ref elapsedFrames, neededFrames))
                return false;

            ObjectCloner.ClonedPhantom.SetActive(false);
            Globals.PhantomScream_source?.Play();
            callback?.Invoke();

            SoundManager.PlayHeartbeat(false);
            return true;
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
            var _parents = new List<GameObject>
            {
                new GameObject("Hands"),
                new GameObject("Stairs"),
                new GameObject("House"),
                new GameObject("Loft")
            };

            Transform _dingo = GameObject.Find("MAP/Buildings/DINGONBIISI").transform;
            Transform _main = _parents[(byte)HandParent.MAIN].transform;
            _main.SetParent(_dingo, worldPositionStays: false);
            _main.gameObject.SetActive(false);

            for (int i = 1; i < 4; i++)
                _parents[i].transform.SetParent(_parents[(byte)HandParent.MAIN].transform, worldPositionStays: false);

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
            Globals.HandsList.ForEach(v =>
            {
                GameObject _clone = GameObject.Instantiate(objs[(byte)v.orig]);
                Transform _t = _clone.transform;

                _t.SetParent(_parents[(byte)v.parent].transform, worldPositionStays: false);
                _t.localPosition = v.position;
                _t.localEulerAngles = v.euler;
                _t.localScale = new Vector3(v.scale, v.scale, v.scale);

                switch (v.orig)
                {
                    case HandOrig.MILK:
                        Object.Destroy(_t.Find("Milk").gameObject);
                        break;
                    case HandOrig.PUSH:
                        Object.Destroy(_t.Find("Pivot/Collider").gameObject);
                        Object.Destroy(_t.Find("Pivot/RigidBody").gameObject);
                        break;
                    case HandOrig.WATCH:
                        _t.Find("Animate/BreathAnim/WristwatchHand").gameObject.SetActive(true);
                        break;
                }

                _clone.layer = 0;
                _clone.transform.IterateAllChilds(child => child.gameObject.layer = 0);
                _clone.SetActive(true);
            });
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
                    _anim.AddClip(ObjectCloner.PigWalkAnimation, "venttipig_pig_walk");

                Vector3 _angles = _anim.transform.parent.localEulerAngles;
                _anim.transform.parent.localEulerAngles = new Vector3(_angles.x, _angles.y, Logic.InHorror ? 0 : 90);
                (_child.GetPlayMaker("Move").GetState("Walking").Actions[0] as PlayAnimation).animName.Value =
                    Logic.InHorror ? "venttipig_pig_walk" : "fat_walk";
            }
            _walkers.gameObject.SetActive(true);
        }

        public static void ChangeCameraFog()
        {
            CameraFog _cameraFog = Utils.GetGlobalVariable<FsmGameObject>("POV").Value.GetComponent<CameraFog>();
            FsmVariables _rainFogVars = GameObject.Find("PLAYER/Rain").GetPlayMaker("Rain").FsmVariables;

            if (Logic.InHorror)
            {
                _changeFog_Internal(
                    _cameraFog, _rainFogVars,
                    0.5f, 50f, 0.08f, Color.gray,
                    0.09f, 0.08f
                );
                return;
            }

            _changeFog_Internal(
                _cameraFog, _rainFogVars,
                0f, 100f, 0.001f, Color.clear,
                0.02f, 0.001f
            );
        }

        public static void ChangeWorldModels(GameObject parent)
        {
            foreach (var _item in ResourcesStorage.ModelsReplaces)
            {
                GameObject _obj = parent?.transform?.Find(_item.Value.path)?.gameObject;
                if (_obj == null) continue;

                if (Logic.InHorror)
                {
                    if (!Globals.ModelsCached.ContainsKey(_item.Key))
                    {
                        Globals.ModelsCached.Add(_item.Key, new ModelData
                        {
                            mesh = _obj.GetComponent<MeshFilter>().mesh,
                            texture = _obj.GetComponent<MeshRenderer>().materials[0].GetTexture("_MainTex")
                        });
                    }

                    SetMesh(_obj, _item.Value.mesh);
                    SetMaterial(_obj, 0, "", _item.Value.texture);
                    continue;
                }

                if (!Globals.ModelsCached.ContainsKey(_item.Key)) continue;

                ModelData data = Globals.ModelsCached[_item.Key];
                SetMesh(_obj, data.mesh);
                SetMaterial(_obj, 0, "", data.texture);
            }
        }

        public static void ChangeBedroomModels()
        {
            bool _state = Logic.InHorror;
            GameObject.Find("YARD/Building/BEDROOM2/bed_base")?.SetActive(!_state);

            GameObject _coffinsGroup = GameObject.Find("YARD/Building/BEDROOM2").transform.FindChild("BedroomCoffins")?.gameObject;
            if (_coffinsGroup == null && _state)
            {
                _coffinsGroup = new GameObject("BedroomCoffins");
                GameObject _coffin1 = (Object.Instantiate(ResourcesStorage.Coffin_prefab,
                    new Vector3(-2.456927f, -0.5738183f, 13.52571f),
                    Quaternion.Euler(new Vector3(270f, 180.2751f, 0f))
                ) as GameObject);
                _coffin1.transform.SetParent(_coffinsGroup.transform, worldPositionStays: false);

                GameObject _coffin2 = (Object.Instantiate(ResourcesStorage.Coffin_prefab,
                    new Vector3(-2.456927f, -0.5738185f, 12.52524f),
                    Quaternion.Euler(new Vector3(270f, 180.2751f, 0f))
                ) as GameObject);
                _coffin2.transform.SetParent(_coffinsGroup.transform, worldPositionStays: false);
            }

            _coffinsGroup?.SetActive(_state);
        }

        public static void ChangeModel(GameObject obj, Mesh mesh, Texture texture)
        {
            SetMesh(obj, mesh);
            SetMaterial(obj, 0, texture.name, texture);
        }


        static void SetMaterial(GameObject obj, int index, string name, Texture texture)
        {
            try
            {
                MeshRenderer _renderer = obj.GetComponent<MeshRenderer>();
                if (!_renderer) return;

                Material _material = _renderer.materials[index];
                if (name.Length > 0) _material.name = name;
                _material.mainTexture = texture;
            }
            catch (Exception e)
            {
                ModConsole.Error($"Unable to change material for {obj?.name}, idx {index}, name {name}, tex {texture?.name};\n{e.GetFullMessage()}");
            }
        }

        static void SetMesh(GameObject obj, Mesh mesh)
        {
            try
            {
                MeshFilter _filter = obj.GetComponent<MeshFilter>();
                if (_filter == null) return;

                _filter.mesh = mesh;
                _filter.sharedMesh = mesh;
            }
            catch (Exception e)
            {
                ModConsole.Error($"Unable to change mesh for {obj?.name}, mesh {mesh?.name};\n{e.GetFullMessage()}");
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
    }
}
