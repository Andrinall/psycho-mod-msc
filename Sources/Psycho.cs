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
        public override string Version => "0.9-beta_0.1";
        public override string Description => "Adds a schizophrenia for your game character";
        public override bool UseAssetsFolder => false;
        public override bool SecondPass => true;

        string _saveDataPath = Application.persistentDataPath + "\\Psycho.dat";

        SettingsDropDownList lang;

        // setup mod settings
        public override void ModSettings()
        {
            lang = Settings.AddDropDownList(this, "psychoLang", "Language Select", new string[] { "English", "Russian" }, 0, _changeSettingName);
        }

        void _changeSettingName()
        {
            Globals.CurrentLang = lang.GetSelectedItemIndex();

            bool blang = Globals.CurrentLang == 0;
            lang.Instance.Name = blang ? "Language select" : "Выбор языка";
        }

        public override void ModSettingsLoaded() => _changeSettingName();
        
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

            var pointsPos = new Dictionary<string, Vector3>() // night scream sounds positions
            {
                ["bedroom"] = new Vector3(-2.338177f, 0.03142646f, 12.91463f),
                ["crying_female"] = new Vector3(-6.801387f, 0.1021783f, 6.610903f),
                ["crying_kid"] = new Vector3(-5.944736f, -0.2938192f, 14.34833f),
                ["door_knock"] = new Vector3(-13.04612f, -0.2938216f, 9.959766f),
                ["footsteps"] = new Vector3(-14.68741f, -0.2938224f, 4.410945f),
                ["glass1"] = new Vector3(-8.830304f, 0.4986353f, 4.962998f),
                ["glass2"] = new Vector3(-2.926222f, 0.4986371f, 4.988186f),
                ["kitchen_water"] = new Vector3(-8.391668f, 0.9055675f, 7.271975f),
            };

            // load resources from bundle
            AssetBundle _bundle = LoadAssets.LoadBundle("Psycho.Assets.bundle.unity3d");
            Globals.Pills_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/Pills.prefab");
            Globals.Background_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/Background.prefab");
            GameObject _picture_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/picture.prefab");
            Globals.Coffin_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/coffin.prefab");
            Globals.SmokeParticleSystem_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/smoke.prefab");
            Globals.AcidBurnSound = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/acid_burn.mp3");
            Globals.ScreamCallClip = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/screamcall.wav");
            Globals.PhantomScreamSound = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/phantomscream.mp3");
            Globals.TVScreamSound = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/tvscreamer.mp3");
            Globals.UncleScreamSound = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/uncle_screamer.mp3");
            Globals.Pentagram_prefab = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/penta.prefab");
            GameObject.Instantiate(Globals.Pentagram_prefab).AddComponent<Pentagram>();

            GameObject crows_list = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/crowslist.prefab");
            GameObject.Instantiate(crows_list); // clone crows list

            AudioSource heartbeat = GameObject.Find("PLAYER").AddComponent<AudioSource>(); // attach heartbeat sound to player
            heartbeat.clip = Globals.LoadAsset<AudioClip>(_bundle, "assets/audio/heartbeat.wav");
            heartbeat.loop = true;
            heartbeat.playOnAwake = false;
            heartbeat.priority = 128;
            heartbeat.volume = 1;
            heartbeat.pitch = 1;
            heartbeat.panStereo = 0;
            heartbeat.spatialBlend = 1;
            heartbeat.reverbZoneMix = 1;
            heartbeat.dopplerLevel = 1;
            heartbeat.spread = 0;
            heartbeat.minDistance = 1.5f;
            heartbeat.maxDistance = 12f;
            Globals.HeartbeatSound = heartbeat;
            heartbeat.enabled = false;

            GameObject suicidals_list = Globals.LoadAsset<GameObject>(_bundle, "assets/prefabs/customsuicidals.prefab");
            var clonedlist = GameObject.Instantiate(suicidals_list); // clone suicidals for horror world
            WorldManager.CopySuicidal(clonedlist); // copy first suicidal from list for use in night screamer
            clonedlist.SetActive(false); // hide suicidals list


            Transform building = GameObject.Find("YARD/Building").transform;
            // load all replaces
            _bundle.GetAllAssetNames().ToList().ForEach(v =>
            {
                if (v.Contains("assets/replaces")) // load texture & sound replaces
                {
                    if (v.Contains("/horror"))
                    { // load replaces for horror world
                        Globals.replaces.Add(
                            v.Replace("assets/replaces/horror/", "").Replace(".png", "").ToLower().GetHashCode(),
                            Globals.LoadAsset<Texture>(_bundle, v)
                        );
                    }
                    else if (v.Contains("/sounds")) // replaces for flies sounds in horror world
                        Globals.horror_flies.Add(Globals.LoadAsset<AudioClip>(_bundle, v));
                    else if (v.Contains("/allworlds"))
                    { // load texture used independently of world
                        Globals.indep_textures.Add(
                            v.Replace("assets/replaces/allworlds/", "").Replace(".png", "").ToLower().GetHashCode(),
                            Globals.LoadAsset<Texture>(_bundle, v)
                        );
                    }
                }
                else if (v.Contains("assets/pictures")) // load textures for picture in frame
                    Globals.pictures.Add(Globals.LoadAsset<Texture>(_bundle, v));
                else if (v.Contains("assets/audio/screamers"))
                { // load sounds for night screamer
                    string item = v.Replace("assets/audio/screamers/", "").Replace(".mp3", "");
                    GameObject emptyPoint = new GameObject($"ScreamPoint({item})");
                    AudioSource source = emptyPoint.AddComponent<AudioSource>();
                    source.clip = Globals.LoadAsset<AudioClip>(_bundle, v);
                    source.loop = true;
                    source.volume = v.Contains("crying") ? 0.4f : 0.9f;
                    source.priority = 0;
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                    source.minDistance = 1.5f;
                    source.maxDistance = 12;
                    source.spatialBlend = 1;
                    source.spread = 0;
                    source.dopplerLevel = 1;
                    
                    if (!v.Contains("door_knock") && !v.Contains("kitchen_water"))
                        emptyPoint.AddComponent<ScreamSoundDistanceChecker>();

                    emptyPoint.transform.SetParent(building, worldPositionStays: false);
                    emptyPoint.transform.position = pointsPos[item];
                    SoundManager.ScreamPoints.Add(emptyPoint);
                }
            });

            //Utils.PrintDebug(eConsoleColors.YELLOW, $"Horror textures loaded: {Globals.replaces.Count}");
            //Utils.PrintDebug(eConsoleColors.YELLOW, $"Independently textures loaded: {Globals.indep_textures.Count}");

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

            // load screenshots for indicate a pills position in letter
            Globals.LoadAllScreens(_bundle);

            _bundle.Unload(false); // unload bundle without unloading resources

            // load saved data
            try
            {
                byte[] value = File.ReadAllBytes(_saveDataPath);
                Logic.isDead = BitConverter.ToBoolean(value, 0);
                Logic.inHorror = BitConverter.ToBoolean(value, 1);
                Logic.envelopeSpawned = BitConverter.ToBoolean(value, 2);
                Logic.SetValue(BitConverter.ToSingle(value, 3));
                Logic.SetPoints(BitConverter.ToSingle(value, 7));

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

                // spawn picture frame & set saved position
                (Object.Instantiate(_picture_prefab, picture_pos, Quaternion.Euler(picture_rot)) as GameObject).MakePickable();

                Utils.PrintDebug($"Value:{Logic.Value}; dead:{Logic.isDead}; env:{Logic.envelopeSpawned}; horror:{Logic.inHorror}");
                if (Logic.isDead)
                {
                    Logic.isDead = false;
                    Logic.ResetValue();
                    Logic.SetPoints(0);
                }

                if (!Logic.inHorror || Logic.envelopeSpawned)
                    goto SkipLoadPills;

                // spawn pills in needed
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
                
                SetDefaultValuesForLogic(); // reset data if savedata not loaded

                if (GameObject.Find("Picture(Clone)") == null) // spawn picture frame at default position if needed
                {
                    (Object.Instantiate(_picture_prefab,
                        new Vector3(-10.1421f, 0.2857685f, 6.501729f),
                        Quaternion.Euler(new Vector3(0.01392611f, 2.436693f, 89.99937f))
                    ) as GameObject).MakePickable();
                }
            }

            // add global handler & job handlers (what is not possible for use in second pass)
            AddComponent<GlobalHandler>("PLAYER");
            AddComponent<JokkeMovingJobHandler>("JOBS/HouseDrunk/Moving");
            AddComponent<JokkeDropOffHandler>("KILJUGUY/HikerPivot/JokkeHiker2/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");
            AddComponent<MummolaJobHandler>("JOBS/Mummola/LOD/GrannyTalking/Granny/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");
        }

        public override void SecondPassOnLoad()
        {
            _registerCommands();

            // add component for make hangover in horror world
            GameObject camera = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").gameObject;
            camera.AddComponent<Hangover>();

            Logic.shizAnimPlayer = AddComponent<ShizAnimPlayer>("PLAYER"); // add animplayer component
            Logic.death = GameObject.Find("Systems").transform.Find("Death").gameObject; // cache ingame player death system

            _addHandlers();
            _applyHorrorIfNeeded();
            _setupActions(camera.transform);

            WorldManager.ChangeIndepTextures(false); // set textures what used independently of world

            // add inactive audio source for play in screamer
            GameObject _grandma = GameObject.Find("ChurchGrandma/GrannyHiker");
            AudioSource source = _grandma.AddComponent<AudioSource>(); // add burn sound to grandma, used for night screamer
            source.clip = Globals.AcidBurnSound;
            source.loop = false;
            source.volume = 2f;
            source.priority = 5;
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

            ModConsole.Print($"[{Name}{{{Version}}}]: <color=green>Successfully loaded!</color>");
            Resources.UnloadUnusedAssets(); // tested (for remove in release version)
        }

        public override void OnSave()
        {
            // restore original game textures for materials (avoid game crash)
            WorldManager.ChangeWorldTextures(false);
            WorldManager.ChangeIndepTextures(true);
            Utils.ChangeSmokingModel();
            SoundManager.ChangeFliesSounds();

            // save data
            byte[] array = new byte[11 + 24]; // values bytes + item bytes
            BitConverter.GetBytes(Logic.isDead).CopyTo(array, 0); // 1
            BitConverter.GetBytes(Logic.inHorror).CopyTo(array, 1); // 1
            BitConverter.GetBytes(Logic.envelopeSpawned).CopyTo(array, 2); // 1
            BitConverter.GetBytes(Logic.Value).CopyTo(array, 3); // 4
            BitConverter.GetBytes(Logic.Points).CopyTo(array, 7); // 4

            Transform picture = GameObject.Find("Picture(Clone)")?.transform;
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
            Logic.SetValue(100f);
            Logic.SetPoints(0f);   
        }


        void _applyHorrorIfNeeded()
        {
            WorldManager.ActivateDINGONBIISIMiscThing3Permanently(); // activate phantom permanently
            WorldManager.SpawnDINGONBIISIHands(); // clone player hands & spawn in dingonbiisi house (default disabled)
            WorldManager.CopyVenttiAnimation(); // copy venttipig_pig_walk animation clip for use on wandering walkers
            WorldManager.CopyGrannyHiker(); // copy granny(mummola) for use in paralysis screamer
            WorldManager.CopyUncleChar(); // copy uncle(kesseli) for use in paralysis screamer
            WorldManager.CopyScreamHand(); // copy player hand for use in paralysis screamer

            // attach screamers components to game objects
            AddComponent<TVScreamer>("YARD/Building/Dynamics/HouseElectricity/ElectricAppliances/TV_Programs");
            AddComponent<PhoneRing>("YARD/Building/LIVINGROOM/Telephone/Logic");
            AddComponent<BathroomShower>("YARD/Building/BATHROOM/Shower");
            AddComponent<KitchenShower>("YARD/Building/KITCHEN/KitchenWaterTap");
            AddComponent<LivingRoomSuicidal>("YARD/Building/LIVINGROOM/LOD_livingroom");

            if (!Logic.inHorror) return;
            // if world == horror -> apply world & features

            Utils.ChangeSmokingModel();

            WorldManager.SetHandsActive(true);
            WorldManager.ChangeWorldTextures(true);
            WorldManager.ChangeCameraFog();
            WorldManager.ChangeBedroomModels();
            WorldManager.ChangeWalkersAnimation();
            WorldManager.StopCloudsOrRandomize();

            SoundManager.ChangeFliesSounds();
            GameObject.Find("CustomSuicidals").SetActive(true); // activate suicidals 
        }

        void _addHandlers()
        {
            // add player behaviour handlers, used for social points increase or decrease
            AddComponent<StoreActionsHandler>("STORE");
            AddComponent<SpillHandler>("GIFU(750/450psi)/ShitTank");
            AddComponent<JunkYardJobHandler>("REPAIRSHOP/JunkYardJob/PayMoney");
            AddComponent<SuitcaseHandler>("KILJUGUY/SuitcaseSpawns");

            AddComponent<FliesChanger>("PLAYER/Flies"); // component for change flies sound after moving between a worlds
            AddComponent<MailBoxEnvelope>("YARD/PlayerMailBox"); // component for handle custom letter

            // add door callbacks for disable night screamer sounds
            WorldManager.AddDoorOpenCallback("YARD/Building/LIVINGROOM/DoorFront", _ => {
                SoundManager.StopScreamSound("door_knock");
                SoundManager.StopScreamSound("footsteps");
            });

            WorldManager.AddDoorOpenCallback("YARD/Building/BEDROOM2/DoorBedroom2", _ => {
                SoundManager.StopScreamSound("bedroom");
                SoundManager.StopScreamSound("crying_kid");
                SoundManager.StopScreamSound("glass1");
            });

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

            // main sleep trigger for initiating a night screamers
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
            ConsoleCommand.Add(new TVTexChange());
            ConsoleCommand.Add(new Phone());
            ConsoleCommand.Add(new Phantom());
            ConsoleCommand.Add(new Finish());
            ConsoleCommand.Add(new Penta());
#endif
            // register crutch command
            ConsoleCommand.Add(new FixBrokenHUD());
        }

        void _setupActions(Transform camera)
        {
            // fix for the items scale (1, 1, 1) after pick them up and drop
            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand")?.transform
                ?.ClearActions("PickUp", "Wait", 2);

            // add fatigue increasing by drink milk
            Transform drink = camera.Find("Drink");
            PlayMakerFSM drinkfsm = drink.GetPlayMaker("Drink");
            FsmState drink_state = drinkfsm.GetState("Activate 3");

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

            // 
            _injectStateHooks(drink);
        }

        void _injectStateHooks(Transform drink)
        {
            // add milk usage handler (used for skip night screamers)
            StateHook.Inject(drink.gameObject, "Drink", "Activate 3", _ =>
            {
                Logic.milkUsed = true;
                Logic.milkUseTime = DateTime.Now;
            });

            StateHook.Inject( // add FITTAN crash handler (crime) (-points)
                GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN").transform.Find("CrashEvent").gameObject,
                "Crash", "Crime",
                _ => Logic.PlayerCommittedOffence("FITTAN_CRASH")
            );

            StateHook.Inject( // add handler for mission delivery Granny to church (+points)
                GameObject.Find("ChurchGrandma/GrannyHiker"), "Logic", "Start walking",
                _ => Logic.PlayerCompleteJob("GRANNY_CHURCH")
            );

            StateHook.Inject( // add Granny angry handler (-points)
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

        T AddComponent<T>(string path) where T : Component
            => GameObject.Find(path)?.AddComponent<T>() ?? null;
    }
}