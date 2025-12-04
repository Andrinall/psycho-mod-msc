
using System;
using System.Reflection;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Features;
using Psycho.Internal;

using Object = UnityEngine.Object;


namespace Psycho
{
    public sealed class Psycho : Mod
    {
        public override string ID => "PsychoMod";
        public override string Name => "Psycho";
        public override string Author => "LUAR, Andrinall, @racer";
        public override string Version => "1.0.13";
        public override string Description => "Adds a schizophrenia for your game character";
        
        public override byte[] Icon => Properties.Resources.mod_icon;

        internal static Psycho Instance;
        internal static bool IsLoaded = false;

        internal static SettingsDropDownList LangDropDownList;
        internal static SettingsCheckBox ShowFullScreenScreamers;
        internal static SettingsKeybind FastOpenKeybind;

        SettingsCheckBox canUseModWithUnofficialVersion;
        
        internal bool IsLoaderMenuOpened => loaderMenu?.activeSelf == true;
        static bool unloaded = false;

        GameObject loaderMenu;

        Transform houseFire;
        Transform bells;
        
        FsmState bellsState;
        FsmString guiSubtitle;
        
        FsmBool houseBurningState;
        bool houseOnFire = false;

        bool bellsActivated = false;
        Vector3 bellsOrigPos;

        bool hasConnection = false;
        

        public override void ModSetup()
        {
            FieldInfo _fieldSteamID = typeof(ModLoader).GetField("steamID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            hasConnection = ModLoader.CheckSteam();
            string _steamId = (string)_fieldSteamID.GetValue(null);
            string _output = $"<b><color=orange>Steam User ID</color> : {(hasConnection ? $"<color=lime>{_steamId ?? ""}</color>" : "<color=red>undefined</color>")}</b>";

#if DEBUG
            ModConsole.Print(_output);
#else
            Debug.Log(_output);
#endif

            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoad);

            SetupFunction(Setup.OnNewGame, Mod_NewGame);
            SetupFunction(Setup.OnSave, Mod_Save);

            SetupFunction(Setup.OnLoad, Mod_Load);
            SetupFunction(Setup.PostLoad, Mod_SecondPassLoad);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.FixedUpdate, Mod_FixedUpdate);

            Instance = this;
        }

        // setup mod settings
        void Mod_SettingsLoad()
            => _changeSetting();

        void Mod_Settings()
        {
            LangDropDownList = Settings.AddDropDownList(
                "psychoLang", "Language Select",
                new string[] { "English", "Russian" },
                0, _changeSetting);

            ShowFullScreenScreamers = Settings.AddCheckBox("psychoFullScreenScreamers", "Show Full Screen Screamers", true);

            if (!hasConnection)
            {
                Settings.AddText(" "); // spacing between checkboxes
                canUseModWithUnofficialVersion = Settings.AddCheckBox("psychoCanUseWithUnofficialVersion", "Use With Unofficial Version", false);
            }

            FastOpenKeybind = Keybind.Add("psychoFastOpenMail", "Fast Open Strange Letter", KeyCode.Quote);
#if DEBUG
            DebugPanel.Init();
#endif
        }

        void _changeSetting()
            => EventsManager.ChangeLanguage(LangDropDownList.GetSelectedItemIndex());
        //


        void Mod_NewGame()
        {
            SaveManager.RemoveData(this);
            Logic.SetDefaultValues();
            ResourcesStorage.UnloadAll();
            Globals.Reset();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"New game started, save file removed!");
        }

        void Mod_Load()
        {
            if (!hasConnection && canUseModWithUnofficialVersion?.GetValue() == false)
                return;

            unloaded = false;
            IsLoaded = false;
            loaderMenu = GameObject.Find("​​MSCLoade​r ​Can​vas m​enu/MSCLoader Mod Menu");
            ResourcesStorage.UnloadAll(); // clear resources for avoid game crashes after loading saved game

            // load resources from bundle
            ResourcesStorage.LoadFromBundle("Psycho.Assets.psycho.unity3d");

            // init global vars
            Globals.InitializeGlobalVars();
            Globals.InitializeObjects();

            // load save data
            if (!SaveManager.Load(this))
                Logic.SetDefaultValues(); // reset data if savedata not loaded

            if (GameObject.Find("Picture(Clone)") == null) // spawn picture frame at default position if needed
            {
                ItemsPool.AddItem(ResourcesStorage.Picture_prefab,
                    new Vector3(-10.1421f, 0.2857685f, 6.501729f),
                    new Vector3(0.01392611f, 2.436693f, 89.99937f)
                );
            }

            if (GameObject.Find("Notebook(Clone)") == null)
            {
                GameObject _notebook = ItemsPool.AddItem(ResourcesStorage.Notebook_prefab,
                    new Vector3(-2.007682f, 0.04279194f, 7.669019f),
                    new Vector3(90f, 247.8114f, 0f)
                );
                Globals.Notebook = _notebook.AddComponent<Notebook>();
                _notebook.MakePickable();
            }

            if (Logic.GameFinished) return;

            GameObject _sketchbook = (GameObject)Object.Instantiate(ResourcesStorage.Sketchbook_prefab,
                new Vector3(-4.179454f, -0.08283298f, 7.728132f),
                Quaternion.Euler(new Vector3(90f, 44.45397f, 0f))
            );
            _sketchbook.AddComponent<Sketchbook>();
            _sketchbook.MakePickable();
            ////////////

            Transform _newspaperFrame = Object.Instantiate(ResourcesStorage.Picture_prefab).transform;
            _newspaperFrame.gameObject.name = "Newspaper(Clone)";
            Object.Destroy(_newspaperFrame.GetComponent<Rigidbody>());
            Object.Destroy(_newspaperFrame.GetComponent<MeshCollider>());

            _newspaperFrame.SetParent(GameObject.Find("STORE").transform);
            _newspaperFrame.position = new Vector3(-1552.66f, 5.261985f, 1182.463f);
            _newspaperFrame.eulerAngles = new Vector3(-0.651f, 58.264f, 90f);
            _newspaperFrame.localScale = new Vector3(29.68098f, 19.2858f, 10f);

            MeshRenderer _renderer = _newspaperFrame.GetComponent<MeshRenderer>();
            _renderer.materials[1].SetTexture("_MainTex", ResourcesStorage.NewsPaper_texture);
            
            // add job handlers (what is not possible for use in second pass)
            AddComponent<Handlers.JokkeMovingJob>("JOBS/HouseDrunk/Moving");
            AddComponent<Handlers.JokkeDropOff>("KILJUGUY/HikerPivot/JokkeHiker2/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");
            AddComponent<Handlers.MummolaJob>("JOBS/Mummola/LOD/GrannyTalking/Granny/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney");

            GameObject.Find("YARD/Building/BEDROOM1").transform.Find("DoorBedroom1/Pivot/Handle")
                .GetPlayMaker("Use")
                .GetGlobalTransition("GLOBALEVENT").ToState = "Check position";

            houseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
            houseFire = GameObject.Find("YARD/Building/HOUSEFIRE").transform;

            bells = GameObject.Find("PERAJARVI/CHURCH/Bells").transform;
            bellsState = bells.parent.GetPlayMaker("Bells").GetState("Stop bells");
            bellsOrigPos = bells.position;

            guiSubtitle = Utils.GetGlobalVariable<FsmString>("GUIsubtitle");
            StateHook.Inject(GameObject.Find("fridge_paper"), "Use", "Wait button", UpdateFridgePaperText, -1);
        }

        void Mod_SecondPassLoad()
        {
            if (!hasConnection && canUseModWithUnofficialVersion?.GetValue() == false)
                throw new AccessViolationException("Steam not connected");

            if (Logic.GameFinished) return;

            // register crutch command
            ConsoleCommand.Add(new Commands.FixBrokenHUD());

            AddComponent<FixedHUD>("GUI/HUD");
            FixedHUD.AddElement(eHUDCloneType.RECT, "Psycho", "Money");
            FixedHUD.Structurize();

            // add component for make hangover in horror world
            Transform _camera = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera");
            _camera.gameObject.AddComponent<Hangover>();
            
            AddComponent<DeathSystem>("Systems");
            AddComponent<ShizAnimPlayer>("PLAYER"); // add animplayer component

            _addHandlers();
            _applyHorrorIfNeeded();
            _setupActions(_camera);

            TexturesManager.ChangeIndepTextures(false); // set textures what used independently of world
            AmbientTrigger.InitializeAllTriggers();
            
            Minigame.Initialize();

            // add inactive audio source for play in screamer
            GameObject _grandma = GameObject.Find("ChurchGrandma/GrannyHiker");
            AudioSource _source = _grandma.AddComponent<AudioSource>(); // add burn sound to grandma, used for night screamer
            _source.clip = ResourcesStorage.AcidBurn_clip;
            _source.loop = false;
            _source.volume = 2f;
            _source.priority = 5;
            _source.rolloffMode = AudioRolloffMode.Logarithmic;
            _source.minDistance = 0.5f;
            _source.maxDistance = 5f;
            _source.spatialBlend = 1f;
            _source.spread = 0f;
            _grandma.AddComponent<Screamers.GrandmaDistanceChecker>();

            SoundManager.ReplaceRadioStaticSound(ResourcesStorage.UBV_psy_clip);

            // finalize a loading
            ModConsole.Log($"[{Name}{{{Version}}}]: <color=green>Successfully loaded!</color>");
            IsLoaded = true;
#if DEBUG
            DebugPanel.SetSettingsVisible(true);
#endif
        }

        void Mod_Update()
        {
            if (!IsLoaded) return;
            if (FastOpenKeybind == null) return;
            if (Logic.GameFinished || Logic.IsDead || Logic.IsDeadByGame) return;
            if (!Logic.InHorror) return;
            if (Globals.Player == null) return;
            if (PillsItem.Index == -1) return;
            if (!IsLoaded || IsLoaderMenuOpened) return;

            if (FastOpenKeybind.GetKeybindUp())
                Globals.MailboxSheet?.SetActive(true);
        }

        void Mod_FixedUpdate()
        {
            if (!IsLoaded) return;
            if (Logic.GameFinished || Logic.IsDeadByGame) return;
            if (Globals.Player == null) return;

            Logic.Tick();

#if DEBUG
            DebugPanel.UpdateData();
#endif

            if (houseBurningState.Value == true && !houseOnFire)
            {
                if (Vector3.Distance(houseFire.position, Globals.Player.position) > 4f) return;
                Logic.PlayerCommittedOffence("HOUSE_BURNING");
                houseOnFire = true;
            }

            WorldManager.ShowCrows(!(Globals.SUN_Hours >= 20f || Globals.SUN_Hours < 6f));

            if (Logic.CurrentAmbientTrigger == null)
                AmbientTrigger.MuteGlobalAmbient(false);
                
            
            if (!bellsActivated && Globals.SUN_Hours == 24f && Globals.SUN_Minutes == 0)
            {
                (bellsState.Actions[0] as ActivateGameObject).activate = true;
                bells.gameObject.SetActive(true);
                bells.position = GameObject.Find("PLAYER").transform.position;
                bellsActivated = true;
            }
            else if (bellsActivated && Globals.SUN_Minutes > 1)
            {
                (bellsState.Actions[0] as ActivateGameObject).activate = false;
                bells.gameObject.SetActive(false);
                bells.position = bellsOrigPos;
                bellsActivated = false;
            }
        }

        void Mod_Save()
        {
            if (!IsLoaded) return;
            SaveManager.SaveData(this);
            Unload();
        }

        internal static void Unload()
        {
            if (unloaded) return;

            Utils.PrintDebug("OnUnload called");
            TexturesManager.ChangeWorldTextures(false);
            Utils.PrintDebug("ChangeWorldTextures - OK");
            TexturesManager.ChangeIndepTextures(true);
            Utils.PrintDebug("ChangeIndepTextures - OK");
            Utils.ChangeSmokingModel();
            Utils.PrintDebug("ChangeSmokingModel - OK");
            SoundManager.ChangeFliesSounds();
            Utils.PrintDebug("ChangeFliesSounds - OK");
            SoundManager.ReplaceRadioStaticSound(null);
            Utils.PrintDebug("ChangeReplaceRadioStaticSound - OK");

            EventsManager.UnSubscribeAll();
            Utils.PrintDebug("EventsManager.UnsubscribeAll - OK");
            Logic.DestroyAllObjects();
            Utils.PrintDebug("Logic.DestroyAllObjects - OK");
            ResourcesStorage.UnloadAll();
            Utils.PrintDebug("ResourcesStorage.UnloadAll - OK");
            Globals.Reset();
            Utils.PrintDebug("Globals.Reset - OK");

            PillsItem.Reset();
#if DEBUG
            DebugPanel.SetSettingsVisible(false);
#endif
            unloaded = true;
        }


        // =================================================== \\
        void _applyHorrorIfNeeded()
        {
            ObjectCloner.ActivateDINGONBIISIMiscThing3Permanently(); // activate phantom permanently
            WorldManager.SpawnDINGONBIISIHands(); // clone player hands & spawn in dingonbiisi house (default disabled)
            ObjectCloner.CopyVenttiAnimation(); // copy venttipig_pig_walk animation clip for use on wandering walkers
            ObjectCloner.CopyGrannyHiker(); // copy granny(mummola) for use in paralysis screamer
            ObjectCloner.CopyUncleChar(); // copy uncle(kesseli) for use in paralysis screamer
            ObjectCloner.CopyScreamHand(); // copy player hand for use in paralysis screamer

            // attach screamers components to game objects
            AddComponent<Screamers.TVScreamer>("YARD/Building/Dynamics/HouseElectricity/ElectricAppliances/TV_Programs");
            AddComponent<Screamers.PhoneScreamer>("YARD/Building/LIVINGROOM/Telephone");
            AddComponent<Screamers.BathroomShower>("YARD/Building/BATHROOM/Shower");
            AddComponent<Screamers.KitchenShower>("YARD/Building/KITCHEN/KitchenWaterTap");
            AddComponent<Screamers.LivingRoomSuicidal>("YARD/Building/LIVINGROOM/LOD_livingroom");
            AddComponent<Screamers.SoundScreamer>("YARD/Building");

            if (!Logic.InHorror) return;
            // if world == horror -> apply world & features

            Utils.ChangeSmokingModel();

            WorldManager.SetHandsActive(true);
            TexturesManager.ChangeWorldTextures(true);
            WorldManager.ChangeCameraFog();
            WorldManager.ChangeBedroomModels();
            WorldManager.ChangeWalkersAnimation();
            WeatherManager.StopCloudsOrRandomize();

            SoundManager.ChangeFliesSounds();
            Globals.SuicidalsList.SetActive(true); // activate suicidals 
        }

        void _addHandlers() // add player behaviour handlers, used for social points increase or decrease
        {
            AddComponent<Handlers.StoreActions>("STORE");
            AddComponent<Handlers.SpillShit>("GIFU(750/450psi)/ShitTank");
            AddComponent<Handlers.JunkYardDelivery>("REPAIRSHOP/JunkYardJob/PayMoney");
            AddComponent<Handlers.SuitcaseGrab>("KILJUGUY/SuitcaseSpawns");
            AddComponent<Handlers.FirewoodDelivery>("JOBS/HouseWood1");
            AddComponent<Handlers.SuskiHelp>("JOBS/Suski");

            AddComponent<FliesChanger>("PLAYER/Flies"); // component for change flies sound after moving between a worlds
            AddComponent<MailBoxEnvelope>("YARD/PlayerMailBox"); // component for handle custom letter
            AddComponent<AnyPentaItemsSpawner>("PLAYER"); // spawn items for pentagram

            // add handlers for HOUSE_SHIT objects (septics)
            Transform _jobs = GameObject.Find("JOBS").transform;
            for (int i = 1; i < 6; i++)
                _jobs.Find($"HouseShit{i}/LOD/ShitNPC/ShitMan{i}")
                    .Find("skeleton/pelvis/RotationPivot")
                    .Find("spine_middle/spine_upper/collar_left/shoulder_left/arm_left/hand_left/finger_left")
                    .Find("PayMoney").gameObject.AddComponent<Handlers.HouseShit>();

            // add handlers for human triggers (hit by player & crime)
            GameObject[] _objects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject _object in _objects)
            {
                if (_object.name.Contains("HumanTrigger"))
                    _object.AddComponent<Handlers.NPCHit>();
                else if (_object.name == "SleepTrigger")
                    _object.AddComponent<Handlers.SleepTrigger>();
                else if (_object.name == "Starter" && _object.transform.parent?.name != "DatabaseMotor")
                    _object.AddComponent<Handlers.EngineStarter>();
            }

            // main sleep trigger for initiating a night screamers
            GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger")
                .AddComponent<Screamers.ScreamsInitiator>();

            Transform _houseLightSwitches = GameObject.Find("YARD/Building/Dynamics/LightSwitches").transform;
            for (int i = 0; i < _houseLightSwitches.childCount; i++)
                _houseLightSwitches.GetChild(i).gameObject.AddComponent<Handlers.LightSwitch>();

            GameObject.Find("BOAT/GFX/Motor/Pivot/Ignition").AddComponent<Handlers.BoatIgnitionOn>();
            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").AddComponent<Handlers.DumpCigarette>();
            GameObject.Find("YARD/Building/Garage/BatteryCharger/TriggerCharger").AddComponent<Handlers.AssemblyBatteryToCharge>();
            GameObject.Find("YARD/Building/KITCHEN/Fridge/Pivot/Handle").AddComponent<Handlers.FridgeDoorOpen>();
            GameObject.Find("COTTAGE/Sauna/Stove/StoveTrigger").AddComponent<Handlers.CottageStoveSteam>();

            GameObject.Find("STORE/StoreCashRegister").AddComponent<Handlers.CashRegisterUse>();
            GameObject.Find("STORE").transform.Find("LOD/GFX_Pub/PubCashRegister").gameObject
                .AddComponent<Handlers.CashRegisterUse>();

            GameObject.Find("WATERFACILITY").transform.Find("LOD/Desk/FacilityCashRegister").gameObject
                .AddComponent<Handlers.CashRegisterUse>();

            GameObject.Find("REPAIRSHOP").transform.Find("LOD/Store/ShopCashRegister").gameObject
                .AddComponent<Handlers.CashRegisterUse>();

            (GameObject.Find("ITEMS/fish trap(itemx)/PickFish") ?? GameObject.Find("fish trap(itemx)/PickFish"))
                .AddComponent<Handlers.FishTrapUse>();
        }

        void _setupActions(Transform camera)
        {
            // fix for the items scale (1, 1, 1) after pick them up and drop
            camera.parent.Find("1Hand_Assemble/Hand")?.transform?.ClearFsmActions("PickUp", "Wait", 2);

            // add fatigue increasing by drink milk
            Transform _drink = camera.Find("Drink");
            PlayMakerFSM _drinkfsm = _drink.GetPlayMaker("Drink");
            FsmState _drinkState = _drinkfsm.GetState("Activate 3");

            List<FsmStateAction> _actions1 = new List<FsmStateAction>(_drinkState.Actions); // copy actions list
            _actions1.Insert(9, new FloatAdd // insert action
            {
                add = 2.5f,
                floatVariable = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue"),
                everyFrame = true,
                perSecond = true
            });
            _drinkState.Actions = _actions1.ToArray(); // replace actions list
            _drinkState.SaveActions(); // save changes


            // add swears by show finger to main swears statistics in permadeath mode
            PlayMakerFSM _functions = camera.parent.GetPlayMaker("PlayerFunctions");
            _functions.Fsm.InitData();
            FsmState _fnFinger = _functions.GetState("Finger");
            
            List<FsmStateAction> _actions2 = new List<FsmStateAction>(_fnFinger.Actions);
            _actions2.Insert(10, new AddToFsmInt
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
            _fnFinger.Actions = _actions2.ToArray();
            _fnFinger.SaveActions();

            // inject other hooks
            _injectStateHooks(camera, _drink);
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

            // player throws bottle after drinking
            StateHook.Inject(drink.gameObject, "Drink", "Throw bottle", PlayerDrunkBeer, 2);
            StateHook.Inject(drink.gameObject, "Drink", "Throw bottle 1", PlayerDrunkBooze, 2);

            // add ingame `Quit` button handler for unloading resources
            Transform _systems = GameObject.Find("Systems").transform;
            Transform _menu = _systems.Find("OptionsMenu/Menu");
            Transform _btnConfirm = _menu.Find("Btn_ConfirmQuit");
            StateHook.Inject(_btnConfirm.Find("Button").gameObject, "Button", "State 3", Unload);

            // add handler for farmer quest
            GameObject _farmerWalker = GameObject.Find("HUMANS/Farmer/Walker");
            if (!_farmerWalker) return;
            StateHook.Inject(_farmerWalker, "Speak", "Done", PlayerCompleteFarmerQuest);
        }


        // =================================================== \\
        // StateHook callbacks (actions)
        void UpdateFridgePaperText()
            => guiSubtitle.Value = Locales.FRIDGE_PAPER_TEXT[Globals.CurrentLang];       

        void MilkUsed()
        {
            Logic.MilkUsed = true;
            Logic.MilkUseTime = DateTime.Now;
        }

        void FittanCrashedByPlayer()
            => Logic.PlayerCommittedOffence("FITTAN_CRASH");

        void PlayerDropOffGrandma()
            => Logic.PlayerCompleteJob("GRANNY_CHURCH");

        void GrandmaAngry()
            => Logic.PlayerCommittedOffence("GRANNY_ANGRY");

        void PlayerSwears()
            => Logic.PlayerCommittedOffence("PLAYER_SWEARS");

        void PlayerDrunkBeer()
            => Logic.BeerBottlesDrunked++;

        void PlayerDrunkBooze()
            => Logic.PlayerCommittedOffence("DRUNK_BOOZE");

        void PlayerCompleteFarmerQuest()
            => Logic.PlayerCompleteJob("FARMER_QUEST");


        // =================================================== \\
        T AddComponent<T>(string path) where T : Component
            => GameObject.Find(path)?.AddComponent<T>() ?? null;
    }
}