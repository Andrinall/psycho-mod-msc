
using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Ambient;
using Psycho.Screamers;

using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    internal static class WorldManager
    {
        static AnimationClip PigWalkAnimation;
        static GameObject ClonedGrannyHiker;
        static GameObject ClonedPhantom;

        static int elapsedFrames = 0;
         
        public static void AddAmbientTriggers()
        {
            GameObject _houseTriggerObj = new GameObject("HouseAmbientTrigger");
            _houseTriggerObj.transform.position = new Vector3(-6.67f, 0.7f, 9.47f);
            SoundManager.AddAudioSource(_houseTriggerObj, Globals.HouseAmbient_clip, 0.22f);

            BoxCollider _box = _houseTriggerObj.AddComponent<BoxCollider>();
            _box.isTrigger = true;
            _box.center = new Vector3(-0.4707041f, 0.172927f, 0.4841557f);
            _box.size = new Vector3(11.49759f, 2.885854f, 10.05963f);

            _houseTriggerObj.AddComponent<AmbientTrigger>().CheckTimeOfDay = true;



            GameObject _islandAmbientObj = new GameObject("IslandAmbientTrigger");
            _islandAmbientObj.transform.position = new Vector3(-878.7f, -3.695f, 496.6f);
            SoundManager.AddAudioSource(_islandAmbientObj, Globals.IslandAmbient_clip, 0.26f);

            SphereCollider _sphere = _islandAmbientObj.AddComponent<SphereCollider>();
            _sphere.isTrigger = true;
            _sphere.center = new Vector3(-0.4707041f, 0.172927f, 0.4841557f);
            _sphere.radius = 70f;

            _islandAmbientObj.AddComponent<AmbientTrigger>();



            GameObject _dingonbiisiAmbientObj = new GameObject("DingonbiisiAmbientTrigger");
            _dingonbiisiAmbientObj.transform.position = new Vector3(1368.03f, 10.63f, 799.7194f);
            _dingonbiisiAmbientObj.transform.eulerAngles = new Vector3(0f, 39.213f, 0f);
            SoundManager.AddAudioSource(_dingonbiisiAmbientObj, Globals.DingonbiisiAmbient_clip, 0.09f);

            BoxCollider _box2 = _dingonbiisiAmbientObj.AddComponent<BoxCollider>();
            _box2.isTrigger = true;
            _box2.center = new Vector3(-0.04533959f, 0.3857429f, -0.001901387f);
            _box2.size = new Vector3(18.84068f, 7.897154f, 5.921038f);

            _dingonbiisiAmbientObj.AddComponent<AmbientTrigger>();
        }

        public static void ShowCrows(bool state)
        {
            if (Globals.CrowsList.activeSelf == state) return;
            Globals.CrowsList.SetActive(state);
        }

        public static void SpawnPhantomBehindPlayer(float distance = 0.75f)
        {
            elapsedFrames = 0;

            Transform _player = Psycho.Player;
            ClonedPhantom.transform.position = _player.position - _player.forward * distance;
            ClonedPhantom.transform.LookAt(_player.position);
            ClonedPhantom.SetActive(true);

            SoundManager.PlayHeartbeat(true);
        }

        public static bool ClonedPhantomTick(int neededFrames, Action callback = null)
        {
            if (!ClonedPhantom.activeSelf) return false;

            if (!Utils.WaitFrames(ref elapsedFrames, neededFrames))
                return false;

            ClonedPhantom.SetActive(false);
            Globals.PhantomScream_source?.Play();
            callback?.Invoke();

            SoundManager.PlayHeartbeat(false);
            return true;
        }

        public static float GetElecCutoffTimer()
        {
            GameObject _bills = GameObject.Find("Systems/ElectricityBills");
            PlayMakerFSM _data = _bills?.GetComponent<PlayMakerFSM>();
            FsmFloat _waitCutoff = _data?.GetVariable<FsmFloat>("WaitCutoff");

            return _waitCutoff?.Value ?? 0f;
        }

        public static void SetElecMeterState(bool state)
        {
            Transform _fuseTable = GameObject.Find("YARD/Building/Dynamics/FuseTable")?.transform;
            if (_fuseTable == null) return;
            _fuseTable.Find("ElectricShockPoint")?.gameObject?.SetActive(state);

            Transform _mainSwitch = _fuseTable.Find("Fusetable/MainSwitch");
            if (_mainSwitch == null) return;

            // states Wait Player -> Wait Button -> Switch
            FsmBool _switchState = _mainSwitch?.GetComponent<PlayMakerFSM>()?.GetVariable<FsmBool>("Switch");
            if (_switchState == null) return;
            _switchState.Value = state;

            // states Position -> OFF
            FsmBool _elecSwitch = GameObject.Find("Systems/ElectricityBills")
                ?.GetComponent<PlayMakerFSM>()
                ?.GetVariable<FsmBool>("MainSwitch");

            if (_elecSwitch == null) return;
            _elecSwitch.Value = state;

            Transform _switchPivot = _mainSwitch.Find("Pivot");
            if (_switchPivot == null) return;
            _switchPivot.localEulerAngles = new Vector3(state ? 335f : 25f, 0);
        }


        public static void CopyScreamHand()
        {
            GameObject _handMilk = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink/Hand/HandMilk");
            Transform _bedroom = GameObject.Find("YARD/Building/BEDROOM1").transform;
            Transform _screamHand = Object.Instantiate(_handMilk).transform;
            Object.Destroy(_screamHand.Find("Milk").gameObject);

            _screamHand.SetParent(_bedroom, worldPositionStays: false);
            _screamHand.gameObject.name = "ScreamHand";

            _screamHand.Find("Armature").gameObject.SetActive(false);
            _screamHand.Find("hand_rigged").gameObject.SetActive(false);

            _screamHand.IterateAllChilds(v => v.gameObject.layer = 0);

            _screamHand.Find("Armature").localEulerAngles = new Vector3(-90, 0, 0);
            _screamHand.position = new Vector3(-12.00460433959961f, 1.1982498168945313f, 15.551212310791016f);
            _screamHand.eulerAngles = new Vector3(0.4349295198917389f, 0.05798640847206116f, 0.02807953953742981f);
            _screamHand.localScale = new Vector3(2f, 2f, 2f);

            if (_screamHand.gameObject.GetComponent<MovingHand>() == null)
                _screamHand.gameObject.AddComponent<MovingHand>();

            _screamHand.gameObject.SetActive(true);
        }

        public static void CopyUncleChar()
        {
            GameObject _uncleOrig = GameObject.Find("YARD/UNCLE/UncleWalking/Uncle");
            Transform _uncleClone = GameObject.Instantiate(_uncleOrig).transform;

            Transform _char = _uncleClone.Find("Char");
            Object.Destroy(_uncleClone.Find("Ray").gameObject);
            Object.Destroy(_char.Find("OriginalPos").gameObject);
            Object.Destroy(_char.Find("LookTarget").gameObject);
            Object.Destroy(_char.Find("HumanCollider").gameObject);
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot").GetComponent<PlayMakerFSM>());
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot/head/Smoking").GetComponent<PlayMakerFSM>());
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney").gameObject);
            _char.gameObject.SetActive(false);

            _uncleClone.SetParent(GameObject.Find("YARD/Building/BEDROOM1").transform, worldPositionStays: false);
            _char.position = new Vector3(-11.72816f, 0.3139997f, 11.2811f);
            _char.eulerAngles = new Vector3(0.0f, 270f, 0.0f);

            _uncleClone.gameObject.name = "ScreamUncle";

            if (_uncleClone.gameObject.GetComponent<MovingUncleHead>() == null)
                _uncleClone.gameObject.AddComponent<MovingUncleHead>();

            _uncleClone.gameObject.SetActive(true);
        }

        public static void CopySuicidal(GameObject cloned)
        {
            Transform _suicidal = GameObject.Instantiate(cloned.transform.GetChild(0).gameObject).transform;
            Transform _livingroom = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom").transform;

            _suicidal.SetParent(_livingroom, worldPositionStays: false);
            _suicidal.position = new Vector3(-1451.8280029296875f, -3.5810000896453859f, -1057.7840576171875f);
            _suicidal.localPosition = Vector3.zero;
            _suicidal.gameObject.SetActive(false);
        }

        public static void CopyGrannyHiker()
        {
            GameObject _hiker = GameObject.Find("ChurchGrandma/GrannyHiker");
            ClonedGrannyHiker = GameObject.Instantiate(_hiker);
            
            Transform _char = ClonedGrannyHiker.transform.Find("Char");
            Transform _head = _char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");

            Object.Destroy(ClonedGrannyHiker.transform.GetPlayMaker("Logic"));
            Object.Destroy(ClonedGrannyHiker.transform.Find("Ray").gameObject);
            Object.Destroy(ClonedGrannyHiker.transform.Find("RagDoll2").gameObject);
            Object.Destroy(_head.GetPlayMaker("Look"));
            Object.Destroy(_char.Find("HeadTarget/LookAt").GetPlayMaker("Random"));
            Object.Destroy(_char.Find("RagDollCar").gameObject);
            Object.Destroy(_char.Find("HeadTarget").gameObject);
            Object.Destroy(_char.Find("HumanTriggerCrime").gameObject);
            _char.gameObject.SetActive(false);
            
            ClonedGrannyHiker.transform.parent = null;
            ClonedGrannyHiker.name = "GrannyScreamHiker";

            Animation _animation = _char.Find("skeleton").GetComponent<Animation>();
            if (!_animation.GetClip("venttipig_pig_walk"))
                _animation.AddClip(PigWalkAnimation, "venttipig_pig_walk");

            _animation.clip = _animation.GetClip("venttipig_pig_walk");
            _animation.playAutomatically = true;
            _animation.Play("venttipig_pig_walk", PlayMode.StopAll);

            if (ClonedGrannyHiker.GetComponent<MummolaCrawl>() == null)
                ClonedGrannyHiker.AddComponent<MummolaCrawl>();

            ClonedGrannyHiker.gameObject.SetActive(true);
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

        public static bool GetElecMeterSwitchState()
        {
            return GameObject.Find("Systems/ElectricityBills")
                ?.GetComponent<PlayMakerFSM>()
                ?.GetVariable<FsmBool>("MainSwitch")
                ?.Value ?? false;
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

                Vector3 _angles = _anim.transform.parent.localEulerAngles;
                _anim.transform.parent.localEulerAngles = new Vector3(_angles.x, _angles.y, Logic.InHorror ? 0 : 90);
                (_child.GetPlayMaker("Move").GetState("Walking").Actions[0] as PlayAnimation).animName.Value =
                    Logic.InHorror ? "venttipig_pig_walk" : "fat_walk";
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

            ClonedPhantom = GameObject.Instantiate(_mover.gameObject);
            Object.Destroy(ClonedPhantom.GetComponent<PlayMakerFSM>());
            ClonedPhantom.SetActive(false);

            ClonedPhantom.transform.parent = null;
            ClonedPhantom.transform.position = Vector3.zero;
            ClonedPhantom.transform.eulerAngles = Vector3.zero;
        }

        public static void AddDoorOpenCallback(string path, Action callback)
            => StateHook.Inject(GameObject.Find(path).transform.Find("Pivot/Handle").gameObject, "Use", "Open door", callback);

        public static void CloseDoor(string path)
        {
            PlayMakerFSM _door = GameObject.Find(path)?.GetPlayMaker("Use");
            if (_door == null) return;

            FsmBool _open = _door.GetVariable<FsmBool>("DoorOpen");
            if (_open == null) return;

            _open.Value = true;
            _door.SendEvent("GLOBALEVENT");
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
            foreach (var _item in Globals.ModelsReplaces)
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


        /// <summary>
        /// Setup independently textures
        /// </summary>
        /// <param name="onSave">if true - reset to default textures</param>
        public static void ChangeIndepTextures(bool onSave)
        {
            if (onSave)
                TexturesManager.RestoreDefaults(Globals.IndependentlyTextures);
            else
                TexturesManager.ReplaceTextures(Globals.IndependentlyTextures);
        }


        public static void ChangeWorldTextures(bool state)
        {
            if (state)
                TexturesManager.ReplaceTextures(Globals.Replaces);
            else
                TexturesManager.RestoreDefaults(Globals.Replaces);
        }

        public static void ChangeBedroomModels()
        {
            bool _state = Logic.InHorror;
            GameObject.Find("YARD/Building/BEDROOM2/bed_base")?.SetActive(!_state);

            GameObject _coffinsGroup = GameObject.Find("YARD/Building/BEDROOM2").transform.FindChild("BedroomCoffins")?.gameObject;
            if (_coffinsGroup == null && _state)
            {
                _coffinsGroup = new GameObject("BedroomCoffins");
                GameObject _coffin1 = (Object.Instantiate(Globals.Coffin_prefab,
                    new Vector3(-2.456927f, -0.5738183f, 13.52571f),
                    Quaternion.Euler(new Vector3(270f, 180.2751f, 0f))
                ) as GameObject);
                _coffin1.transform.SetParent(_coffinsGroup.transform, worldPositionStays: false);

                GameObject _coffin2 = (Object.Instantiate(Globals.Coffin_prefab,
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

        public static void StopCloudsOrRandomize()
        {
            try
            {
                bool _state = !Logic.InHorror;
                GameObject _clouds = GameObject.Find("MAP/CloudSystem/Clouds");
                PlayMakerFSM _cloudsFsm = _clouds.GetPlayMaker("Weather");
                FsmState _cloudsMove = _cloudsFsm.GetState("Move clouds");
                
                FloatAdd _action1 = (_cloudsMove.Actions[1] as FloatAdd);
                SetPosition _action2 = (_cloudsMove.Actions[2] as SetPosition);

                _action1.everyFrame = _state;
                _action1.perSecond = _state;

                _action2.everyFrame = _state;
                _action2.lateUpdate = _state;

                if (!_state)
                    _clouds.transform.position = Vector3.zero;
                else
                    _cloudsFsm.SendEvent("RANDOMIZE");
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to change clouds after moving between worlds;\n{e.GetFullMessage()}");
            }
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
