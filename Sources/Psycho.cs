using System;
using System.Linq;
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
using Psycho.Extensions;


namespace Psycho
{
    public sealed class Psycho : Mod
    {
        public override string ID => "PsychoMod";
        public override string Name => "Psycho";
        public override string Author => "LUAR, Andrinall, @racer";
        public override string Version => "0.9-beta_0.2.1";
        public override string Description => "Adds a schizophrenia for your game character";
        public override bool UseAssetsFolder => false;
        public override bool SecondPass => true;


        SettingsDropDownList lang;

        // setup mod settings
        public override void ModSettings()
        {
            lang = Settings.AddDropDownList(this,
                "psychoLang", "Language Select",
                new string[] { "English", "Russian" },
                0, _changeSettingName);
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
            SaveManager.RemoveFile();
            Logic.SetDefaultValues();
            Utils.FreeResources();
            Utils.PrintDebug(eConsoleColors.RED, $"New game started, save file removed!");
        }

        public override void OnLoad()
        {
            Utils.FreeResources(); // clear resources for avoid game crashes after loading saved game

            AssetBundle _bundle = LoadAssets.LoadBundle("Psycho.Assets.bundle.unity3d");
            Globals.LoadAssets(_bundle);
            _bundle.Unload(false);

            SaveManager.LoadData();

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

            SaveManager.SaveData();
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
            AddComponent<AnyPentaItemsSpawner>("PLAYER");

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