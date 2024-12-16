using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Features;
using Psycho.Screamers;
using Psycho.Extensions;
using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    internal static class WorldManager
    {
        public static AnimationClip PigWalkAnimation;
        public static GameObject ClonedGrannyHiker;
        public static GameObject ClonedPhantom;
        public static GameObject minigame;

        static int elapsedFrames = 0;

        public static void InitializeCottageMinigame()
        {
            GameObject bottlehide = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom/bottlehide");
            Vector3 bottlehidePos = bottlehide.transform.position;
            Vector3 bottlehideRot = bottlehide.transform.eulerAngles;
            Object.Destroy(bottlehide);

            if (NotebookMain.Pages.Any(v => v.isFinalPage)) return;
            GameObject minigame = Object.Instantiate(Globals.CottageMinigame_prefab);
            minigame.transform.SetParent(GameObject.Find("COTTAGE").transform, false);
            minigame.AddComponent<Minigame>();
        }
         
        public static void ShowCrows(bool state)
            => GameObject.Find("CrowsList(Clone)")?.SetActive(state);

        public static void SpawnPhantomBehindPlayer(float distance = 0.75f)
        {
            elapsedFrames = 0;
            Transform player = GameObject.Find("PLAYER").transform;
            ClonedPhantom.transform.position = player.position - player.forward * distance;
            ClonedPhantom.transform.LookAt(player.position);
            ClonedPhantom.SetActive(true);
            SoundManager.PlayHeartbeat(true);
        }

        public static bool ClonedPhantomTick(int neededFrames, Action callback = null)
        {
            if (!ClonedPhantom.activeSelf) return false;

            if (elapsedFrames < neededFrames)
            {
                elapsedFrames++;
                return true;
            }

            ClonedPhantom.SetActive(false);
            elapsedFrames = 0;

            Globals.PhantomScream_source?.Play();
            callback?.Invoke();
            SoundManager.PlayHeartbeat(false);
            return false;
        }

        public static void TurnOffElecMeter()
        {
            Transform FuseTable = GameObject.Find("YARD/Building/Dynamics/FuseTable").transform;
            Transform mainswitch = FuseTable.Find("Fusetable/MainSwitch");
            PlayMakerFSM switchfsm = mainswitch.GetComponent<PlayMakerFSM>();
            
            // states Wait Player -> Wait Button -> Switch
            switchfsm.GetVariable<FsmBool>("Switch").Value = false;
            
            // states Position -> OFF
            GameObject.Find("Systems/ElectricityBills")
                .GetComponent<PlayMakerFSM>()
                .GetVariable<FsmBool>("MainSwitch")
                .Value = false;

            FuseTable.Find("ElectricShockPoint").gameObject.SetActive(false);

            //PlayMakerFSM.BroadcastEvent("ELEC_CUTOFF");
            mainswitch.Find("Pivot").localEulerAngles = new Vector3(25f, 0);
        }


        public static void CopyScreamHand()
        {
            GameObject HandMilk = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink/Hand/HandMilk");
            Transform Bedroom = GameObject.Find("YARD/Building/BEDROOM1").transform;
            Transform ScreamHand = Object.Instantiate(HandMilk).transform;
            Object.Destroy(ScreamHand.Find("Milk").gameObject);

            ScreamHand.SetParent(Bedroom, worldPositionStays: false);
            ScreamHand.gameObject.name = "ScreamHand";
            
            ScreamHand.Find("Armature").gameObject.SetActive(false);
            ScreamHand.Find("hand_rigged").gameObject.SetActive(false);

            ScreamHand.IterateAllChilds(v => v.gameObject.layer = 0);

            ScreamHand.Find("Armature").localEulerAngles = new Vector3(-90, 0, 0);
            ScreamHand.position = new Vector3(-12.00460433959961f, 1.1982498168945313f, 15.551212310791016f);
            ScreamHand.eulerAngles = new Vector3(0.4349295198917389f, 0.05798640847206116f, 0.02807953953742981f);
            ScreamHand.localScale = new Vector3(2f, 2f, 2f);

            (ScreamHand.gameObject.GetComponent<MovingHand>() ?? ScreamHand.gameObject.AddComponent<MovingHand>()).enabled = false;
            ScreamHand.gameObject.SetActive(true);
        }

        public static void CopyUncleChar()
        {
            GameObject uncleOrig = GameObject.Find("YARD/UNCLE/UncleWalking/Uncle");
            Transform uncleClone = GameObject.Instantiate(uncleOrig).transform;

            Transform _char = uncleClone.Find("Char");
            Object.Destroy(uncleClone.Find("Ray").gameObject);
            Object.Destroy(_char.Find("OriginalPos").gameObject);
            Object.Destroy(_char.Find("LookTarget").gameObject);
            Object.Destroy(_char.Find("HumanCollider").gameObject);
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot").GetComponent<PlayMakerFSM>());
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot/head/Smoking").GetComponent<PlayMakerFSM>());
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney").gameObject);
            _char.gameObject.SetActive(false);

            uncleClone.SetParent(GameObject.Find("YARD/Building/BEDROOM1").transform, worldPositionStays: false);
            _char.position = new Vector3(-11.72816f, 0.3139997f, 11.2811f);
            _char.eulerAngles = new Vector3(0.0f, 270f, 0.0f);

            uncleClone.gameObject.name = "ScreamUncle";
            (uncleClone.gameObject.GetComponent<MovingUncleHead>() ?? uncleClone.gameObject.AddComponent<MovingUncleHead>())
                .enabled = false;

            uncleClone.gameObject.SetActive(true);
        }

        public static void CopySuicidal(GameObject cloned)
        {
            Transform suicidal = GameObject.Instantiate(cloned.transform.GetChild(0).gameObject).transform;
            Transform livingroom = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom").transform;
            
            suicidal.SetParent(livingroom, worldPositionStays: false);
            suicidal.position = new Vector3(-1451.8280029296875f, -3.5810000896453859f, -1057.7840576171875f);
            suicidal.localPosition = Vector3.zero;
            suicidal.gameObject.SetActive(false);
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

            (ClonedGrannyHiker.GetComponent<MummolaCrawl>() ?? ClonedGrannyHiker.AddComponent<MummolaCrawl>()).enabled = false;
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

        public static bool GetElecMeterSwitchState()
        {
            return GameObject.Find("Systems/ElectricityBills")
                .GetComponent<PlayMakerFSM>()
                .GetVariable<FsmBool>("MainSwitch")
                .Value;
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

                var angles = _anim.transform.parent.localEulerAngles;
                _anim.transform.parent.localEulerAngles = new Vector3(angles.x, angles.y, Logic.inHorror ? 0 : 90);
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
            PlayMakerFSM door = GameObject.Find(path)?.GetPlayMaker("Use");
            if (!door) return;

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


        /// <summary>
        /// Setup independently textures
        /// </summary>
        /// <param name="onSave">if true - reset to default textures</param>
        public static void ChangeIndepTextures(bool onSave)
        {
            if (onSave)
                TexturesManager.RestoreDefaults(Globals.indep_textures);
            else
                TexturesManager.ReplaceTextures(Globals.indep_textures);
        }


        public static void ChangeWorldTextures(bool state)
        {
            if (state)
                TexturesManager.ReplaceTextures(Globals.replaces);
            else
                TexturesManager.RestoreDefaults(Globals.replaces);
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


        static void SetMaterial(GameObject obj, int index, string name, Texture texture)
        {
            try
            {
                MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                if (!renderer) return;

                Material material = renderer.materials[index];
                if (name.Length > 0) material.name = name;
                material.mainTexture = texture;
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
