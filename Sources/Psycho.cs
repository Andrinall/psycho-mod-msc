using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Objects;
using Psycho.Commands;
using Psycho.Handlers;
using Psycho.Internal;
using Psycho.Screamers;
using Psycho.Extensions;

using Object = UnityEngine.Object;

namespace Psycho
{
    public sealed class Psycho : Mod
    {
        public override string ID => "PsychoMod";
        public override string Name => "Psycho";
        public override string Author => "LUAR, Andrinall, @racer";
        public override string Version => "0.86.8";
        public override string Description => "Adds a schizophrenia for your game character";
        public override bool UseAssetsFolder => false;
        public override bool LoadInMenu => true;
        public override bool SecondPass => true;

        string _saveDataPath = Application.persistentDataPath + "\\Psycho.dat";

        public override void OnMenuLoad() => Resources.UnloadUnusedAssets(); // tested
        
        public override void OnNewGame()
        {
            Utils.FreeResources();
            File.Delete(_saveDataPath);
            SetDefaultValuesForLogic();
            Utils.PrintDebug(eConsoleColors.RED, $"New game started, save file removed!");
        }

        public override void OnLoad()
        {
            Utils.FreeResources(); // clear resources for avoid game crashes after loading saved game

            var pointsPos = new Dictionary<string, Vector3>()
            {
                ["bedroom"] = new Vector3(-2.338177f, 0.03142646f, 12.91463f),
                ["door_knock"] = new Vector3(-13.04612f, -0.2938216f, 9.959766f),
                ["glass1"] = new Vector3(-3.980298f, 0.4950065f, 14.64838f),
                ["glass2"] = new Vector3(-10.51819f, 0.4991404f, 14.96361f),
                ["kitchen_water"] = new Vector3(-8.391668f, 0.9055675f, 7.271975f)
            };

            // load resources from bundle
            AssetBundle _bundle = LoadAssets.LoadBundle("Psycho.Assets.bundle.unity3d");
            Globals.Pills_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/Pills.prefab");
            Globals.Background_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/Background.prefab");
            GameObject _picture_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/picture.prefab");
            Globals.Coffin_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/coffin.prefab");
            Globals.SmokeParticleSystem_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/smoke.prefab");
            Globals.AcidBurnSound = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/acid_burn.mp3");

            GameObject crows_list = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/crowslist.prefab");
            GameObject.Instantiate(crows_list);
            
            GameObject suicidals_list = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/customsuicidals.prefab");
            var clonedlist = GameObject.Instantiate(suicidals_list);
            WorldManager.CopySuicidal(clonedlist);
            clonedlist.SetActive(false);

            // load all replaces
            _bundle.GetAllAssetNames().ToList().ForEach(v =>
            {
                if (v.Contains("assets/replaces"))
                {
                    if (v.Contains("/horror"))
                    {
                        Globals.replaces.Add(
                            v.Replace("assets/replaces/horror/", "").Replace(".png", "").ToLower().GetHashCode(),
                            Globals.LoadAsset<Texture>(_bundle, v)
                        );
                    }
                    else if (v.Contains("/sounds"))
                        Globals.horror_flies.Add(Globals.LoadAsset<AudioClip>(_bundle, v));
                    else if (v.Contains("/allworlds"))
                    {
                        Globals.indep_textures.Add(
                            v.Replace("assets/replaces/allworlds/", "").Replace(".png", "").ToLower().GetHashCode(),
                            Globals.LoadAsset<Texture>(_bundle, v)
                        );
                    }
                }
                else if (v.Contains("assets/pictures"))
                    Globals.pictures.Add(Globals.LoadAsset<Texture>(_bundle, v));
                else if (v.Contains("assets/audio/screamers"))
                {
                    var item = v.Replace("assets/audio/screamers/", "").Replace(".mp3", "");
                    var emptyPoint = new GameObject("ScreamPoint(" + item + ")");
                    var source = emptyPoint.AddComponent<AudioSource>();
                    source.clip = Globals.LoadAsset<AudioClip>(_bundle, v);
                    source.loop = true;
                    source.volume = v.Contains("bedroom") ? 3f : 2f;
                    source.priority = 0;
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                    source.minDistance = 0.5f;
                    source.maxDistance = 4f;
                    source.spatialBlend = 1f;
                    source.spread = 0f;
                    emptyPoint.transform.position = pointsPos[item];
                    SoundManager.ScreamPoints.Add(emptyPoint);
                }
            });

            Utils.PrintDebug(eConsoleColors.YELLOW, $"Horror textures loaded: {Globals.replaces.Count}");
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Independently textures loaded: {Globals.indep_textures.Count}");

            // load smoking replaces
            Texture cig_texture = Globals.LoadAsset<Texture>(_bundle, "assets/replaces/smoking/hand.png");
            Globals.models_replaces.Add("cigarette_filter".GetHashCode(), new ModelData
            {
                path = "Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Filter",
                mesh = Globals.LoadAsset<Mesh>(_bundle, "assets/replaces/smoking/cigarette_filter.obj"),
                texture = cig_texture
            });

            Globals.models_replaces.Add("cigarette_shaft".GetHashCode(), new ModelData
            {
                path = "Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Shaft",
                mesh = Globals.LoadAsset<Mesh>(_bundle, "assets/replaces/smoking/cigarette_shaft.obj"),
                texture = cig_texture
            });

            // Load death sound
            AudioSource src = GameObject.Find("Systems").AddComponent<AudioSource>();
            src.clip = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/heart_stop.wav");
            src.loop = false;
            src.volume = 1.75f;
            src.priority = 0;
            SoundManager.DeathSound = src;

            //
            Globals.LoadAllScreens(_bundle);
            _bundle.Unload(false); // unload bundle without unloading resources

            // load saved data
            try
            {
                byte[] value = File.ReadAllBytes(_saveDataPath);
                Logic.isDead = BitConverter.ToBoolean(value, 0);
                Logic.inHorror = BitConverter.ToBoolean(value, 1);
                Logic.envelopeSpawned = BitConverter.ToBoolean(value, 2);
                Logic.Value = BitConverter.ToSingle(value, 3);
                Logic.Points = BitConverter.ToSingle(value, 7);

                Vector3 picture_pos = new Vector3(
                    BitConverter.ToSingle(value, 11),
                    BitConverter.ToSingle(value, 15),
                    BitConverter.ToSingle(value, 19)
                );

                Vector3 picture_rot = new Vector3(
                    BitConverter.ToSingle(value, 23),
                    BitConverter.ToSingle(value, 27),
                    BitConverter.ToSingle(value, 31)
                );

                (Object.Instantiate(_picture_prefab, picture_pos, Quaternion.Euler(picture_rot)) as GameObject).MakePickable();

                Utils.PrintDebug($"Value:{Logic.Value}; dead:{Logic.isDead}; env:{Logic.envelopeSpawned}; horror:{Logic.inHorror}");
                if (Logic.isDead)
                {
                    Logic.isDead = false;
                    Logic.ResetValue();
                    Logic.Points = 0;
                }

                if (!Logic.inHorror || Logic.envelopeSpawned)
                    goto SkipLoadPills;

                PillsItem item = new PillsItem(0);
                item.ReadData(ref value, 35);
                item.self.SetActive(Logic.inHorror);
                Globals.pills_list.Add(item);

            SkipLoadPills:
                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
            }
            catch (Exception e)
            {
                ModConsole.Error("<color=red>Unable to load Save Data, resetting to default</color>");
                Utils.PrintDebug(eConsoleColors.RED, e.GetFullMessage());
                SetDefaultValuesForLogic();
            }

            if (GameObject.Find("picture(Clone)") == null)
            {
                (Object.Instantiate(_picture_prefab,
                    new Vector3(-10.1421f, 0.2857685f, 6.501729f),
                    Quaternion.Euler(new Vector3(0.01392611f, 2.436693f, 89.99937f))
                ) as GameObject).MakePickable();
            }

            AddComponent<GlobalHandler>("PLAYER");
            AddComponent<JokkeMovingJobHandler>("JOBS/HouseDrunk/Moving");
            AddComponent<JokkeDropOffHandler>("KILJUGUY/HikerPivot/JokkeHiker2/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");
            AddComponent<MummolaJobHandler>("JOBS/Mummola/LOD/GrannyTalking/Granny/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");

            SoundManager.ScreamPoints
                .ForEach(v => v.transform.SetParent(GameObject.Find("YARD/Building").transform, worldPositionStays: false));
            
            // change amount of milk in Teimo store
            GameObject.Find("STORE/Inventory")
                .GetComponents<PlayMakerHashTableProxy>()
                .ToList()
                .ForEach(v => {
                    if (v.preFillIntList[7] == 0) return;
                    v.preFillIntList[7] = 5;
                });
        }

        public override void SecondPassOnLoad()
        {
            _registerCommands();

            // add component for make hangover in horror world
            GameObject camera = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").gameObject;
            camera.AddComponent<Hangover>();

            // add animplayer component
            Logic.shizAnimPlayer = AddComponent<ShizAnimPlayer>("PLAYER");
            Logic.death = GameObject.Find("Systems").transform.Find("Death").gameObject; // cache ingame player death system

            _addHandlers();
            _applyHorrorIfNeeded();
            _setupActions(camera.transform);

            WorldManager.ChangeIndepTextures(false);

            // add inactive audio source for play in screamer
            GameObject _grandma = GameObject.Find("ChurchGrandma");
            AudioSource source = _grandma.AddComponent<AudioSource>();
            source.clip = Globals.AcidBurnSound;
            source.loop = false;
            source.volume = 2f;
            source.priority = 0;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 0.5f;
            source.maxDistance = 5f;
            source.spatialBlend = 1f;
            source.spread = 0f;

            // whisp spawn
            /*
             * var obj = GameObject.Find("MAP/Buildings/DINGONBIISI/Misc/Thing1");
             * var mover = obj.Find("Mover");
             * obj.SetActive(true);
             * mover.SetActive(true);
             */

            ModConsole.Print("[Schizophrenia]: <color=green>Successfully loaded!</color>");
            Resources.UnloadUnusedAssets(); // tested
        }

        public override void OnSave()
        {
            WorldManager.ChangeWorldTextures(false);
            WorldManager.ChangeIndepTextures(true);
            Utils.ChangeSmokingModel();
            SoundManager.ChangeFliesSounds();

            byte[] array = new byte[11 + 24]; // values bytes + item bytes
            BitConverter.GetBytes(Logic.isDead).CopyTo(array, 0); // 1
            BitConverter.GetBytes(Logic.inHorror).CopyTo(array, 1); // 1
            BitConverter.GetBytes(Logic.envelopeSpawned).CopyTo(array, 2); // 1
            BitConverter.GetBytes(Logic.Value).CopyTo(array, 3); // 4
            BitConverter.GetBytes(Logic.Points).CopyTo(array, 7); // 4

            Transform picture = GameObject.FindGameObjectsWithTag("PART")
                .First(v => v.name == "picture(Clone)").transform;
            BitConverter.GetBytes(picture.position.x).CopyTo(array, 11);
            BitConverter.GetBytes(picture.position.y).CopyTo(array, 15);
            BitConverter.GetBytes(picture.position.z).CopyTo(array, 19);
            BitConverter.GetBytes(picture.eulerAngles.x).CopyTo(array, 23);
            BitConverter.GetBytes(picture.eulerAngles.x).CopyTo(array, 27);
            BitConverter.GetBytes(picture.eulerAngles.x).CopyTo(array, 31);

            if (!Logic.inHorror || Logic.envelopeSpawned)
            {
                File.WriteAllBytes(_saveDataPath, array);
                return;
            }

            Globals.pills_list.ElementAtOrDefault(0)?.WriteData(ref array, 35);
            File.WriteAllBytes(_saveDataPath, array);
        }


        void SetDefaultValuesForLogic()
        {
            Logic.isDead = false;
            Logic.inHorror = false;
            Logic.envelopeSpawned = false;
            Logic.milkUsed = false;
            Logic.Value = 100f;
            Logic.Points = 0f;
            
        }


        void _applyHorrorIfNeeded()
        {
            WorldManager.ActivateDINGONBIISIMiscThing3Permanently();
            WorldManager.SpawnDINGONBIISIHands();
            WorldManager.CopyVenttiAnimation();
            WorldManager.CopyGrannyHiker();
            WorldManager.CopyUncleChar();
            WorldManager.CopyScreamHand();

            if (!Logic.inHorror) return;
            Utils.ChangeSmokingModel();

            WorldManager.SetHandsActive(true);
            WorldManager.ChangeWorldTextures(true);
            WorldManager.ChangeCameraFog();
            WorldManager.ChangeBedroomModels();
            WorldManager.ChangeWalkersAnimation();
            WorldManager.StopCloudsOrRandomize();

            SoundManager.ChangeFliesSounds();
            GameObject.Find("CustomSuicidals").SetActive(true);
        }

        void _addHandlers()
        {
            AddComponent<StoreActionsHandler>("STORE");
            AddComponent<SpillHandler>("GIFU(750/450psi)/ShitTank");
            AddComponent<MailBoxEnvelope>("YARD/PlayerMailBox");
            AddComponent<JunkYardJobHandler>("REPAIRSHOP/JunkYardJob/PayMoney");
            AddComponent<SuitcaseHandler>("KILJUGUY/SuitcaseSpawns");
            AddComponent<FliesChanger>("PLAYER/Flies");
            AddComponent<LivingRoomSuicidal>("YARD/Building/LIVINGROOM/LOD_livingroom");

            WorldManager.AddDoorOpenCallback("YARD/Building/LIVINGROOM/DoorFront", _ => SoundManager.StopScreamSound("door_knock"));
            WorldManager.AddDoorOpenCallback("YARD/Building/BEDROOM2/DoorBedroom2", _ => SoundManager.StopScreamSound("bedroom"));

            // add handlers for HOUSE_SHIT objects (septics)
            for (int i = 1; i < 6; i++)
                GameObject.Find($"JOBS/HouseShit{i}/LOD/ShitNPC/Man").transform
                    .Find("skeleton/pelvis/RotationPivot")
                    .Find("spine_middle/spine_upper/collar_left/shoulder_left/arm_left/hand_left/finger_left")
                    .Find("PayMoney").gameObject.AddComponent<HouseShitHandler>();

            // add handlers for human triggers (hit by player & crime)
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            objects.Where(v => v.name.Contains("HumanTrigger")).ToList()
                .ForEach(v => v.AddComponent<NPCHitHandler>());

            // add handlers for sleep triggers
            objects.Where(v => v.name == "SleepTrigger").ToList()
                .ForEach(v => v.AddComponent<SleepTriggerHandler>());

            GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger")
                .AddComponent<ScreamsInitiator>();

            // add crash handler for cars (reset psycho after crashing)
            Resources.FindObjectsOfTypeAll<CarDynamics>()
                .Where(v => v.transform.parent == null).ToList()
                .ForEach(v => v.gameObject.AddComponent<CrashHandler>());
        }

        void _registerCommands()
        {
#if DEBUG
            // register debug commands
            ConsoleCommand.Add(new TeleportToPills());
            ConsoleCommand.Add(new ChangeWorld());
            ConsoleCommand.Add(new Kill());
            ConsoleCommand.Add(new Scream());
            ConsoleCommand.Add(new Shower());
            ConsoleCommand.Add(new KitchenTap());
#endif
            // register crutch command
            ConsoleCommand.Add(new FixBrokenHUD());
        }

        void _setupActions(Transform camera)
        {
            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").transform?
                .ClearActions("PickUp", "Wait", 2);

            // add fatigue increasing by drink milk
            Transform drink = camera.transform.Find("Drink");
            FsmState drink_state = drink.GetPlayMaker("Drink").GetState("Activate 3");
            
            var actions = new List<FsmStateAction>(drink_state.Actions); // copy actions list
            actions.Insert(9, new FloatAdd // insert action
            {
                add = 2.5f,
                floatVariable = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue"),
                everyFrame = true,
                perSecond = true
            });
            drink_state.Actions = actions.ToArray(); // replace actions list
            drink_state.SaveActions(); // save changes

            _injectStateHooks(drink);
        }

        void _injectStateHooks(Transform drink)
        {
            // add milk usage handler
            StateHook.Inject(drink.gameObject, "Drink", "Activate 3", _ =>
            {
                Logic.milkUsed = true;
                Logic.milkUseTime = DateTime.Now;
            });

            GameObject yard_sleep = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger");
            PlayMakerFSM sleep_fsm = yard_sleep.GetPlayMaker("Activate");

            StateHook.Inject( // add FITTAN crash handler (crime)
                GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN").transform.Find("CrashEvent").gameObject,
                "Crash", "Crime",
                _ => Logic.PlayerCommittedOffence("FITTAN_CRASH")
            );

            StateHook.Inject( // add handler for mission delivery Granny to church
                GameObject.Find("ChurchGrandma/GrannyHiker"), "Logic", "Start walking",
                _ => Logic.PlayerCompleteJob("GRANNY_CHURCH")
            );

            StateHook.Inject( // add Granny angry handler
                GameObject.Find("JOBS/Mummola/TalkEngine"), "Granny", "Speak 27",
                _ => Logic.PlayerCommittedOffence("GRANNY_ANGRY")
            );

            StateHook.Inject( // add arrest handler for horror world
                GameObject.Find("Systems/PlayerWanted"), "Activate", "State 2",
                _ => {
                    if (!Logic.inHorror) return;
                    Logic.KillHeartAttack();
                });
        }

        T AddComponent<T>(string obj) where T : Component
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Loading component {obj}::{typeof(T)?.Name?.ToString()}");
            return GameObject.Find(obj)?.AddComponent<T>() ?? null;
        }
    }
}