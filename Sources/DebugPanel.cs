#if DEBUG
using System;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Features;
using System.Collections.Generic;
using Random = UnityEngine.Random;


namespace Psycho
{
    internal static class DebugPanel
    {
        static SettingsSlider debug_Psycho;
        static SettingsSlider debug_Points;
        static SettingsText debug_IsDead;
        static SettingsCheckBox debug_GameFinished;

        static SettingsText debug_TimeOfDay;
        static SettingsCheckBox debug_MilkUsed;
        static SettingsText debug_MilkUsageTimer;

        static SettingsDropDownList debug_NotebookFillType;

        static SettingsDropDownList debug_ScreamerSound;
        static SettingsDropDownList debug_ScreamerFear;
        static SettingsDropDownList debug_ScreamerParalysis;

        static SettingsText debug_CurrentDay;
        static SettingsText debug_LastDayMinigame;

        static SettingsDropDownList debug_SpawnItem;
        static SettingsDropDownList debug_PentaVeryGood;
        static SettingsDropDownList debug_PentaGood;
        static SettingsDropDownList debug_PentaNormal;
        static SettingsDropDownList debug_PentaBad;
        static SettingsDropDownList debug_PentaVeryBad;

        static FsmInt TimeOfDay;
        static FsmInt CurrentDay;

        static Minigame _minigame;

        /*static readonly List<Vector3[]> junkCarsPos = new List<Vector3[]>
        {
            new Vector3[] { new Vector3(1553.83f, 6.938912f, 728.8507f), new Vector3 (293.7697f, 249.2135f, 87.0145f) },
            new Vector3[] { new Vector3 (1554.213f, 7.895408f, 728.2757f), new Vector3 (354.9219f, 261.3854f, 41.91655f) },
            new Vector3[] { new Vector3 (1553.669f, 7.790683f, 730.2797f), new Vector3 (338.634f, 243.1584f, 35.6148f) },
            new Vector3[] { new Vector3 (1555.464f, 7.173864f, 727.7571f), new Vector3 (324.3774f, 264.1703f, 167.7415f) }
        };*/
        static readonly List<SettingsHeader> headers = new List<SettingsHeader>();

        public static void Init()
        {
            Utils.PrintDebug("DebugPanel::Init()");
            headers.Add(Settings.AddHeader("START DEBUG SECTION", true));
            headers.Add(Settings.AddHeader("Main Section", false));
            debug_Psycho = Settings.AddSlider("PsychoValue", "Psycho", 0f, 100f, Logic.Value, PsychoValueChanged);
            debug_Points = Settings.AddSlider("PointsValue", "Points", -8, 8, Logic.Points, PointsValueChanged);
            debug_IsDead = Settings.AddText("Is Dead: False");
            debug_GameFinished = Settings.AddCheckBox("PsychoFinished", "Game Finished", Logic.GameFinished, SetGameFinished);
            Settings.AddButton("Reset All", ResetAll);

            Settings.AddButton("Switch World", SwitchWorld);
            Settings.AddButton("Increase Fatigue", IncreaseFatigue);
            Settings.AddButton("Teleport To Minigame", TeleportToMinigame);
            Settings.AddButton("Teleport To Pentagram", TeleportToPentagram);
            Settings.AddButton("Teleport To Home", TeleportToHome);
            Settings.AddButton("Teleport To Pills", TeleportToPills);
            Settings.AddButton("Initiate Heart Attack", KillHeartAttack);
            Settings.AddButton("Suicide On Railroad", SuicideOnRailroad);
            
            Settings.AddText("");

            //Settings.AddButton("Fill Septics", FillSeptics);
            //Settings.AddButton("Grab Junk Cars", GrabJunkCars);

            headers.Add(Settings.AddHeader("Notebook", true));

            debug_NotebookFillType = Settings.AddDropDownList("notebookfillpsycho", "Notebook Fill Type", new string[] { "Full True Pages", "Full False Pages", "Random Fill" }, 0);

            Settings.AddButton("Fill Notebook", FillNotebook);
            Settings.AddButton("Clear Notebook", ClearNotebook);

            headers.Add(Settings.AddHeader("Screamers", true));
            debug_TimeOfDay = Settings.AddText("Time Of Day: 0");
            debug_MilkUsed = Settings.AddCheckBox("milkUsed", "Milk Used", false, MilkUsed);
            debug_MilkUsageTimer = Settings.AddText("Milk Usage Timer: 0", false);

            var soundFields = Utils.GetEnumFields<ScreamSoundType>();
            var fearFields = Utils.GetEnumFields<ScreamFearType>();
            var paralysisFields = Utils.GetEnumFields<ScreamParalysisType>();

            debug_ScreamerSound = Settings.AddDropDownList("SoundScreamers", "Sound", soundFields, 0);
            Settings.AddButton("Trigger", TriggerSoundScreamer);

            debug_ScreamerFear = Settings.AddDropDownList("FearScreamers", "Fear", fearFields, 0);
            Settings.AddButton("Trigger", TriggerFearScreamer);

            debug_ScreamerParalysis = Settings.AddDropDownList("PrlsScreamers", "Paralysis", paralysisFields, 0);
            Settings.AddButton("Trigger", TriggerParalysisScreamer);


            headers.Add(Settings.AddHeader("Minigame", true));
            debug_CurrentDay = Settings.AddText("Current Day: 0");
            debug_LastDayMinigame = Settings.AddText("Last Day Minigame: 0");

            Settings.AddButton("Change Day", ChangeMinigameDay);
            Settings.AddButton("Get Card", GetMinigameCard);

            headers.Add(Settings.AddHeader("Pentagram", true));
            debug_SpawnItem = Settings.AddDropDownList("pentaItemSpawner", "Pentagram Item", Globals.PentaRecipe, 0);
            Settings.AddButton("Spawn", SpawnPentagramItem);
            Settings.AddText("\n");

            debug_PentaVeryGood = Settings.AddDropDownList("pentaVeryGood", "Very Good Events", Pentagram.InnerEvents["Very good"], 0);
            Settings.AddButton("Trigger", TriggerVeryGoodPentaEvent);

            debug_PentaGood = Settings.AddDropDownList("pentaGood", "Good Events", Pentagram.InnerEvents["Good"], 0);
            Settings.AddButton("Trigger", TriggerGoodPentaEvent);

            debug_PentaNormal = Settings.AddDropDownList("pentaNormal", "Normal Events", Pentagram.InnerEvents["Normal"], 0);
            Settings.AddButton("Trigger", TriggerNormalPentaEvent);

            debug_PentaBad = Settings.AddDropDownList("pentaBad", "Bad Events", Pentagram.InnerEvents["Bad"], 0);
            Settings.AddButton("Trigger", TriggerBadPentaEvent);

            debug_PentaVeryBad = Settings.AddDropDownList("pentaVeryBad", "Very Bad Events", Pentagram.InnerEvents["Very bad"], 0);
            Settings.AddButton("Trigger", TriggerVeryBadPentaEvent);

            Settings.AddText("\n \n \n ");

            SetSettingsVisible(false);
        }

        public static void UpdateData()
        {
            if (CurrentDay == null)
            {
                CurrentDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
                return;
            }

            if (TimeOfDay == null)
            {
                TimeOfDay = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger")?.GetPlayMaker("Activate")?.GetVariable<FsmInt>("TimeOfDay");
                return;
            }


            debug_Psycho.SetValue(Logic.Value);
            debug_Points.SetValue(Logic.Points);
            debug_IsDead.SetValue($"Is Dead: {Logic.isDead}");
            debug_GameFinished.SetValue(Logic.GameFinished);

            debug_TimeOfDay.SetValue($"Time Of Day: {TimeOfDay.Value}");

            if (Logic.milkUsed)
            {
                var milkTimer = 60 - (Logic.milkUseTime - DateTime.Now).Seconds;
                var clampedMilkTimer = Mathf.Clamp(milkTimer, 0, 60);
                debug_MilkUsageTimer.SetValue($"Milk Usage Timer: {clampedMilkTimer} seconds ago");
                debug_MilkUsageTimer.SetVisibility(true);
            }
            else
                debug_MilkUsageTimer.SetVisibility(false);

            debug_CurrentDay.SetValue($"Current Day: {CurrentDay.Value}");
            debug_LastDayMinigame.SetValue($"Last Day Minigame: {Logic.lastDayMinigame}");
        }

        public static void SetSettingsVisible(bool state)
            => headers.ForEach(v => v?.SetVisibility(state));

        static void PsychoValueChanged()
            => Logic.SetValue(debug_Psycho.GetValue());

        static void PointsValueChanged()
            => Logic.SetPoints(debug_Points.GetValue());

        static void SetGameFinished()
        {
            bool newValue = debug_GameFinished.GetValue();
            if (newValue)
            {
                Logic.FinishShizGame();
                return;
            }

            Logic.GameFinished = false;
            ResetAll();

            if (!FixedHUD.IsElementExist("Psycho"))
                FixedHUD.AddElement(eHUDCloneType.RECT, "Psycho", "Money");

            FixedHUD.Structurize();
        }

        static void ResetAll()
        {
            Logic.ResetValue();
            Logic.ResetPoints();
            Logic.lastDayMinigame = 0;
        }

        static void MilkUsed()
            => debug_MilkUsed.SetValue(Logic.milkUsed);

        static void FillNotebook()
        {
            int index = debug_NotebookFillType.GetSelectedItemIndex();
            bool random = index == 2;

            Globals.Notebook?.ClearPages();
            for (int i = 1; i < 14; i++)
            {
                Notebook.TryAddPage(new NotebookPage
                {
                    index = i,
                    isTruePage = random ? (Random.Range(0, 2) == 1) : index == 0
                });
            }
            Globals.Notebook?.SortPages();
            Globals.Notebook?.CreateFinalPage();

            Utils.PrintDebug(eConsoleColors.GREEN, $"Pages list filled with ({debug_NotebookFillType.GetSelectedItemName()}) pages");
        }

        static void ClearNotebook()
            => Globals.Notebook?.ClearPages();

        static void TriggerSoundScreamer()
            => EventsManager.TriggerNightScreamer(ScreamTimeType.SOUNDS, debug_ScreamerSound.GetSelectedItemIndex());

        static void TriggerFearScreamer()
            => EventsManager.TriggerNightScreamer(ScreamTimeType.FEAR, debug_ScreamerFear.GetSelectedItemIndex());

        static void TriggerParalysisScreamer()
            => EventsManager.TriggerNightScreamer(ScreamTimeType.PARALYSIS, debug_ScreamerParalysis.GetSelectedItemIndex());

        static void ChangeMinigameDay()
        {
            if (_minigame == null)
                _minigame = GameObject.Find("COTTAGE/minigame(Clone)").GetComponent<Minigame>();

            Logic.lastDayMinigame--;
            _minigame.UpdateHousekeeperCard();
        }

        static void GetMinigameCard()
        {
            if (_minigame == null)
                _minigame = GameObject.Find("COTTAGE/minigame(Clone)").GetComponent<Minigame>();

            _minigame.PlayerGetsCard();
        }

        static void SpawnPentagramItem()
        {
            var item = debug_SpawnItem.GetSelectedItemName();
            Transform player = GameObject.Find("PLAYER").transform;
            Vector3 pos = player.position;

            switch (item)
            {
                case "churchcandle":
                    ItemsPool.AddItem(Globals.Candle_prefab, pos, Vector3.zero);
                    break;
                case "fernflower":
                    ItemsPool.AddItem(Globals.FernFlower_prefab, pos, Vector3.zero);
                    break;
                case "mushroom":
                    ItemsPool.AddItem(Globals.Mushroom_prefab, pos, Vector3.zero);
                    break;
                case "blackegg":
                    ItemsPool.AddItem(Globals.BlackEgg_prefab, pos, Vector3.zero);
                    break;
                case "walnut":
                    ItemsPool.AddItem(Globals.Walnut_prefab, pos, Vector3.zero);
                    break;

                default:
                    return;
            }
            ModConsole.Print($"{item} spawned on player pos");

        }

        static void TriggerVeryGoodPentaEvent()
            => PentagramEvents.TriggerEvent(debug_PentaVeryGood.GetSelectedItemName(), true);

        static void TriggerGoodPentaEvent()
            => PentagramEvents.TriggerEvent(debug_PentaGood.GetSelectedItemName(), true);

        static void TriggerNormalPentaEvent()
            => PentagramEvents.TriggerEvent(debug_PentaNormal.GetSelectedItemName(), true);

        static void TriggerBadPentaEvent()
            => PentagramEvents.TriggerEvent(debug_PentaBad.GetSelectedItemName(), true);

        static void TriggerVeryBadPentaEvent()
            => PentagramEvents.TriggerEvent(debug_PentaVeryBad.GetSelectedItemName(), true);

        static void SwitchWorld()
            => Logic.ChangeWorld(Logic.inHorror ? eWorldType.MAIN : eWorldType.HORROR);

        static void IncreaseFatigue()
            => Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue").Value = 85f;

        static void TeleportToMinigame()
            => GameObject.Find("PLAYER").transform.position = GameObject.Find("COTTAGE/minigame(Clone)").transform.position;

        static void TeleportToPentagram()
            => GameObject.Find("PLAYER").transform.position = GameObject.Find("Penta(Clone)").transform.position;

        static void TeleportToHome()
            => GameObject.Find("PLAYER").transform.position = GameObject.Find("YARD/Building").transform.position;

        static void TeleportToPills()
        {
            if (Globals.pills?.self == null)
            {
                ModConsole.Error("Pills not exists!");
                return;
            }

            GameObject.Find("PLAYER").transform.position = Globals.pills.self.transform.position;
        }

        static void KillHeartAttack()
            => Logic.KillHeartAttack();

        static void SuicideOnRailroad()
            => Logic.KillUsingTrain();

       /* static void FillSeptics()
        {
            Utils.GetGlobalVariable<FsmInt>("PlayerKeyGifu").Value = 1;

            Transform _jobs = GameObject.Find("JOBS").transform;
            for (int i = 0; i < _jobs.childCount; i++)
            {
                Transform _child = _jobs.GetChild(i);
                if (_child == null) continue;
                if (!_child.name.Contains("HouseShit")) continue;

                Transform _npc = _child.Find("LOD/ShitNPC");
                if (_npc == null) continue;

                PlayMakerFSM _fsm = _npc.GetComponent<PlayMakerFSM>();
                if (_fsm == null) continue;

                _fsm.GetVariable<FsmFloat>("Level").Value = 10f;
                _fsm.GetVariable<FsmBool>("Called").Value = true;
            }
        }*/

        /*static void GrabJunkCars()
        {
            List<Transform> junkCars = Resources.FindObjectsOfTypeAll<Transform>().Where(v => v != null && v.name.Contains("JunkCar")).ToList();
            foreach (Transform car in junkCars)
            {
                int index = Int32.Parse(
                    new string(
                        new char[] { car.name[7] }
                    )
                );

                Utils.PrintDebug(eConsoleColors.YELLOW, $"moving JunkCar{index}");

                car.parent = null;
                car.position = junkCarsPos[index - 1][0];
                car.eulerAngles = junkCarsPos[index - 1][1];
            }

            PlayMakerFSM _job = GameObject.Find("REPAIRSHOP/JunkYardJob").GetComponent<PlayMakerFSM>();
            for (int i = 1; i < 5; i++)
                _job.GetVariable<FsmBool>($"JunkCar{i}Delivered").Value = true;

            GameObject.Find("REPAIRSHOP/LOD/Office/Fleetari").SetActive(true);
        }*/
    }
}
#endif