using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Features;
using Psycho.Commands;
using Psycho.Handlers;
using Psycho.Internal;
using Psycho.Screamers;
#if DEBUG
using Psycho.Debug;
#endif
using Object = UnityEngine.Object;


namespace Psycho
{
    public sealed class Psycho : Mod
    {
        public override string ID => "PsychoMod";
        public override string Name => "Psycho";
        public override string Author => "LUAR, Andrinall, @racer";
        public override string Version => "0.9.8-beta4";
        public override string Description => "Adds a schizophrenia for your game character";

        internal static SettingsDropDownList lang;
        internal static Keybind fastOpen;

        internal static bool IsLoaded = false;

        GameObject LoaderMenu;

        Transform _player;
        Transform _houseFire;
        Transform _bells;
        
        FsmState _bellsState;
        FsmFloat SUN_minutes;
        FsmFloat SUN_hours;
        FsmString GUIsubtitle;
        
        FsmBool m_bHouseBurningState;
        bool m_bHouseOnFire = false;

        bool m_bBellsActivated = false;
        Vector3 bellsOrigPos;
        internal bool IsLoaderMenuOpened => LoaderMenu?.activeSelf == true;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnNewGame, Mod_NewGame);
            SetupFunction(Setup.OnSave, Mod_Save);

            SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.OnLoad, Mod_Load);
            SetupFunction(Setup.PostLoad, Mod_SecondPassLoad);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.FixedUpdate, Mod_FixedUpdate);
        }

        // setup mod settings
        void Mod_SettingsLoad() => ChangeSetting();

        void Mod_Settings()
        {
            lang = Settings.AddDropDownList(
                "psychoLang", "Language Select",
                new string[] { "English", "Russian" },
                0, ChangeSetting);

            fastOpen = Keybind.Add(this, "psychoFastOpenMail", "Fast Open Strange Letter", KeyCode.Quote);
#if DEBUG
            DebugPanel.Init();
#endif
        }
        //


        void Mod_NewGame()
        {
            SaveManager.RemoveFile();
            Logic.SetDefaultValues();
            Utils.FreeResources();
            Utils.PrintDebug(eConsoleColors.RED, $"New game started, save file removed!");
        }

        void Mod_Load()
        {
            IsLoaded = false;
            LoaderMenu = GameObject.Find("​​MSCLoade​r ​Can​vas m​enu/MSCLoader Mod Menu");
            Utils.FreeResources(); // clear resources for avoid game crashes after loading saved game

            AssetBundle _bundle = LoadAssets.LoadBundle("Psycho.Assets.bundle.unity3d");
            Globals.LoadAssets(_bundle);
            _bundle.Unload(false);

            SaveManager.LoadData();

            Transform newspaperFrame = Object.Instantiate(Globals.Picture_prefab).transform;
            newspaperFrame.gameObject.name = "Newspaper(Clone)";
            Object.Destroy(newspaperFrame.GetComponent<Rigidbody>());
            Object.Destroy(newspaperFrame.GetComponent<MeshCollider>());

            newspaperFrame.SetParent(GameObject.Find("STORE").transform);
            newspaperFrame.position = new Vector3(-1552.66f, 5.261985f, 1182.463f);
            newspaperFrame.eulerAngles = new Vector3(-0.651f, 58.264f, 90f);
            newspaperFrame.localScale = new Vector3(29.68098f, 19.2858f, 10f);

            MeshRenderer renderer = newspaperFrame.GetComponent<MeshRenderer>();
            renderer.materials[1].SetTexture("_MainTex", Globals.NewsPaper_texture);
            
            // add job handlers (what is not possible for use in second pass)
            AddComponent<JokkeMovingJobHandler>("JOBS/HouseDrunk/Moving");
            AddComponent<JokkeDropOffHandler>("KILJUGUY/HikerPivot/JokkeHiker2/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");
            AddComponent<MummolaJobHandler>("JOBS/Mummola/LOD/GrannyTalking/Granny/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");

            m_bHouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
            _houseFire = GameObject.Find("YARD/Building/HOUSEFIRE").transform;
            _player = GameObject.Find("PLAYER").transform;

            PlayMakerFSM sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            SUN_hours = sun.GetVariable<FsmFloat>("Hours");
            SUN_minutes = sun.GetVariable<FsmFloat>("Minutes");

            _bells = GameObject.Find("PERAJARVI/CHURCH/Bells").transform;
            _bellsState = _bells.parent.GetPlayMaker("Bells").GetState("Stop bells");
            bellsOrigPos = _bells.position;

            GUIsubtitle = Utils.GetGlobalVariable<FsmString>("GUIsubtitle");
            StateHook.Inject(GameObject.Find("fridge_paper"), "Use", "Wait button", UpdateFridgePaperText, -1);
        }

        void Mod_SecondPassLoad()
        {
#if DEBUG
            _registerDebugCommands();
#endif
            // register crutch command
            ConsoleCommand.Add(new FixBrokenHUD());

            AddComponent<FixedHUD>("GUI/HUD");
            FixedHUD.AddElement(eHUDCloneType.RECT, "Psycho", "Money");
            FixedHUD.Structurize();
            Logic.SetPoints(Logic.Points);

            // add component for make hangover in horror world
            Transform camera = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera");
            camera.gameObject.AddComponent<Hangover>();
            
            AddComponent<DeathSystem>("Systems");
            AddComponent<ShizAnimPlayer>("PLAYER"); // add animplayer component

            _addHandlers();
            _applyHorrorIfNeeded();
            _setupActions(camera);

            WorldManager.ChangeIndepTextures(false); // set textures what used independently of world
            Minigame.Initialize();

            // add inactive audio source for play in screamer
            GameObject _grandma = GameObject.Find("ChurchGrandma/GrannyHiker");
            AudioSource source = _grandma.AddComponent<AudioSource>(); // add burn sound to grandma, used for night screamer
            source.clip = Globals.AcidBurn_clip;
            source.loop = false;
            source.volume = 2f;
            source.priority = 5;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 0.5f;
            source.maxDistance = 5f;
            source.spatialBlend = 1f;
            source.spread = 0f;
            _grandma.AddComponent<GrandmaDistanceChecker>();

            // whisp spawn
            /*
             * var obj = GameObject.Find("MAP/Buildings/DINGONBIISI/Misc/Thing1");
             * var mover = obj.Find("Mover");
             * obj.SetActive(true);
             * mover.SetActive(true);
             */

            ModConsole.Print($"[{Name}{{{Version}}}]: <color=green>Successfully loaded!</color>");
            //Resources.UnloadUnusedAssets(); // tested (for remove in release version)
            IsLoaded = true;
#if DEBUG
            DebugPanel.SetSettingsVisible(true);
#endif
        }

        void Mod_Update()
        {
            if (Logic.GameFinished) return;

            if (IsLoaded && !IsLoaderMenuOpened && fastOpen.GetKeybindUp() && Logic.inHorror && !Logic.envelopeSpawned)
            {
                Globals.mailboxSheet?.SetActive(true);
            }
        }

        void Mod_FixedUpdate()
        {
            if (Logic.GameFinished) return;

            Logic.Tick();

#if DEBUG
            DebugPanel.UpdateData();
#endif

            if (m_bHouseBurningState.Value == true && !m_bHouseOnFire)
            {
                if (Vector3.Distance(_houseFire.position, _player.position) > 4f) return;
                Logic.PlayerCommittedOffence("HOUSE_BURNING");
                m_bHouseOnFire = true;
            }

            if (!m_bBellsActivated && SUN_hours.Value == 24f && Mathf.FloorToInt(SUN_minutes.Value) == 0)
            {
                (_bellsState.Actions[0] as ActivateGameObject).activate = true;
                _bells.gameObject.SetActive(true);
                _bells.position = GameObject.Find("PLAYER").transform.position;
                m_bBellsActivated = true;
            }
            else if (m_bBellsActivated && Mathf.FloorToInt(SUN_minutes.Value) > 1)
            {
                (_bellsState.Actions[0] as ActivateGameObject).activate = false;
                _bells.gameObject.SetActive(false);
                _bells.position = bellsOrigPos;
                m_bBellsActivated = false;
            }
        }

        void Mod_Save()
        {
            // restore original game textures for materials (avoid game crash)
            OnUnload();
            SaveManager.SaveData();
        }

        void OnUnload()
        {
            WorldManager.ChangeWorldTextures(false);
            WorldManager.ChangeIndepTextures(true);
            Utils.ChangeSmokingModel();
            SoundManager.ChangeFliesSounds();
            TexturesManager.Cache.Clear();

            EventsManager.UnSubscribeAll();
#if DEBUG
            DebugPanel.SetSettingsVisible(false);
#endif
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
            AddComponent<PhoneRing>("YARD/Building/LIVINGROOM/Telephone");
            AddComponent<BathroomShower>("YARD/Building/BATHROOM/Shower");
            AddComponent<KitchenShower>("YARD/Building/KITCHEN/KitchenWaterTap");
            AddComponent<LivingRoomSuicidal>("YARD/Building/LIVINGROOM/LOD_livingroom");
            AddComponent<SoundScreamer>("YARD/Building");

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
            Globals.suicidalsList.SetActive(true); // activate suicidals 
        }

        void _addHandlers()
        {
            // add player behaviour handlers, used for social points increase or decrease

            AddComponent<StoreActionsHandler>("STORE");
            AddComponent<SpillHandler>("GIFU(750/450psi)/ShitTank");
            AddComponent<JunkYardJobHandler>("REPAIRSHOP/JunkYardJob/PayMoney");
            AddComponent<SuitcaseHandler>("KILJUGUY/SuitcaseSpawns");
            AddComponent<FirewoodJobHandler>("JOBS/HouseWood1");
            AddComponent<SuskiHelpHandler>("JOBS/Suski");

            AddComponent<FliesChanger>("PLAYER/Flies"); // component for change flies sound after moving between a worlds
            AddComponent<MailBoxEnvelope>("YARD/PlayerMailBox"); // component for handle custom letter
            AddComponent<AnyPentaItemsSpawner>("PLAYER");

            // add handlers for HOUSE_SHIT objects (septics)
            for (int i = 1; i < 6; i++)
                GameObject.Find($"JOBS/HouseShit{i}/LOD/ShitNPC/ShitMan").transform
                    .Find("skeleton/pelvis/RotationPivot")
                    .Find("spine_middle/spine_upper/collar_left/shoulder_left/arm_left/hand_left/finger_left")
                    .Find("PayMoney").gameObject.AddComponent<HouseShitHandler>();

            // add handlers for human triggers (hit by player & crime)
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.name.Contains("HumanTrigger"))
                    obj.AddComponent<NPCHitHandler>();
                else if (obj.name == "SleepTrigger")
                    obj.AddComponent<SleepTriggerHandler>();
            }


            // main sleep trigger for initiating a night screamers
            GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger")
                .AddComponent<ScreamsInitiator>();


            // add crash handler for cars (reset psycho after crashing)
            foreach (CarDynamics cd in Resources.FindObjectsOfTypeAll<CarDynamics>())
            {
                if (cd.transform.parent != null) continue;
                cd.gameObject.AddComponent<CrashHandler>();
            }
        }

#if DEBUG
        void _registerDebugCommands()
        {
            // register debug commands
            ConsoleCommand.Add(new TeleportToPills());
            ConsoleCommand.Add(new ChangeWorld());
            ConsoleCommand.Add(new Kill());
            ConsoleCommand.Add(new Scream());
            ConsoleCommand.Add(new Finish());
            ConsoleCommand.Add(new Penta());
            ConsoleCommand.Add(new MinigameCMD());
            ConsoleCommand.Add(new NotebookCMD());
        }
#endif

        void _setupActions(Transform camera)
        {
            // fix for the items scale (1, 1, 1) after pick them up and drop
            camera.parent.Find("1Hand_Assemble/Hand")?.transform?.ClearActions("PickUp", "Wait", 2);

            // add fatigue increasing by drink milk
            Transform drink = camera.Find("Drink");
            PlayMakerFSM drinkfsm = drink.GetPlayMaker("Drink");
            FsmState drink_state = drinkfsm.GetState("Activate 3");

            var actions1 = new List<FsmStateAction>(drink_state.Actions); // copy actions list
            actions1.Insert(9, new FloatAdd // insert action
            {
                add = 2.5f,
                floatVariable = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue"),
                everyFrame = true,
                perSecond = true
            });
            drink_state.Actions = actions1.ToArray(); // replace actions list
            drink_state.SaveActions(); // save changes


            PlayMakerFSM functions = camera.parent.GetPlayMaker("PlayerFunctions");
            functions.Fsm.InitData();
            FsmState fn_finger = functions.GetState("Finger");
            
            var actions2 = new List<FsmStateAction>(fn_finger.Actions);
            actions2.Insert(10, new AddToFsmInt
            {
                gameObject = new FsmOwnerDefault
                {
                    GameObject = GameObject.Find("Systems/Statistics"),
                    OwnerOption = OwnerDefaultOption.SpecifyGameObject
                },
                fsmName = "Data",
                variableName = "SwearwordsUsed",
                add = 1,
                everyFrame = false
            });
            fn_finger.Actions = actions2.ToArray();
            fn_finger.SaveActions();

            // 
            _injectStateHooks(camera, drink);
        }

        void _injectStateHooks(Transform camera, Transform drink)
        {
            // add milk usage handler (used for skip night screamers)
            StateHook.Inject(drink.gameObject, "Drink", "Activate 3", MilkUsed);

            // add FITTAN crash handler (crime) (-points)
            StateHook.Inject(
                GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN").transform.Find("CrashEvent").gameObject,
                "Crash", "Crime", FittanCrashedByPlayer
            );

            // add handler for mission delivery Granny to church (+points)
            StateHook.Inject(GameObject.Find("ChurchGrandma/GrannyHiker"), "Logic", "Start walking", PlayerDropOffGrandma);

            // add Granny angry handler (-points)
            StateHook.Inject(GameObject.Find("JOBS/Mummola/TalkEngine"), "Granny", "Speak 27", GrandmaAngry);

            // add arrest handler for horror world
            StateHook.Inject(GameObject.Find("Systems/PlayerWanted"), "Activate", "State 2", Logic.KillHeartAttack);

            // player swears trigger
            StateHook.Inject(camera.parent.Find("SpeakDatabase").gameObject, "Speech", "Swear", PlayerSwears, 7);
            StateHook.Inject(camera.parent.gameObject, "PlayerFunctions", "Finger", PlayerSwears, 11);

            StateHook.Inject(drink.gameObject, "Drink", "Throw bottle", PlayerDrunkBeer, 2);
            StateHook.Inject(drink.gameObject, "Drink", "Throw bottle 1", PlayerDrunkBooze, 2);

            GameObject farmer_walker = GameObject.Find("HUMANS/Farmer/Walker");
            if (!farmer_walker) return;
            StateHook.Inject(farmer_walker, "Speak", "Done", PlayerCompleteFarmerQuest);

            Transform systems = GameObject.Find("Systems").transform;
            Transform menu = systems.Find("OptionsMenu/Menu");
            Transform btnconfirm = menu.Find("Btn_ConfirmQuit");
            StateHook.Inject(btnconfirm.Find("Button").gameObject, "Button", "State 3", OnUnload);
        }

        void UpdateFridgePaperText() => GUIsubtitle.Value = Locales.FRIDGE_PAPER_TEXT[Globals.CurrentLang];

        void ChangeSetting()
            => EventsManager.ChangeLanguage(lang.GetSelectedItemIndex());

        void MilkUsed()
        {
            Logic.milkUsed = true;
            Logic.milkUseTime = DateTime.Now;
        }

        void FittanCrashedByPlayer() => Logic.PlayerCommittedOffence("FITTAN_CRASH");

        void PlayerDropOffGrandma() => Logic.PlayerCompleteJob("GRANNY_CHURCH");

        void GrandmaAngry() => Logic.PlayerCommittedOffence("GRANNY_ANGRY");

        void PlayerSwears() => Logic.PlayerCommittedOffence("PLAYER_SWEARS");

        void PlayerDrunkBeer() => Logic.BeerBottlesDrunked++;
        void PlayerDrunkBooze() => Logic.PlayerCommittedOffence("DRUNK_BOOZE");

        void PlayerCompleteFarmerQuest() => Logic.PlayerCompleteJob("FARMER_QUEST");

        T AddComponent<T>(string path) where T : Component
            => GameObject.Find(path)?.AddComponent<T>() ?? null;
    }
}