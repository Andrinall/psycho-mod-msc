#if DEBUG
using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Features;

using Random = UnityEngine.Random;


namespace Psycho.Internal
{
    static class DebugPanel
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
            Settings.AddButton("Reset All", Logic.SetDefaultValues);

            Settings.AddButton("Switch World", SwitchWorld);
            Settings.AddButton("Increase Fatigue", IncreaseFatigue);
            Settings.AddButton("Teleport To Minigame", TeleportToMinigame);
            Settings.AddButton("Teleport To Pentagram", TeleportToPentagram);
            Settings.AddButton("Teleport To Home", TeleportToHome);
            Settings.AddButton("Teleport To Pills", TeleportToPills);
            Settings.AddButton("Initiate Heart Attack", KillHeartAttack);
            Settings.AddButton("Suicide On Railroad", SuicideOnRailroad);
            Settings.AddButton("Reset FullScreenScreamer Cooldown", Logic.ResetFullScreenScreamerCooldown);
            
            Settings.AddText("");

            headers.Add(Settings.AddHeader("Notebook", true));

            debug_NotebookFillType = Settings.AddDropDownList("notebookfillpsycho", "Notebook Fill Type", new string[] { "Full True Pages", "Full False Pages", "Random Fill" }, 0);

            Settings.AddButton("Fill Notebook", FillNotebook);
            Settings.AddButton("Clear Notebook", ClearNotebook);

            headers.Add(Settings.AddHeader("Screamers", true));
            debug_TimeOfDay = Settings.AddText("Time Of Day: 0");
            debug_MilkUsed = Settings.AddCheckBox("milkUsed", "Milk Used", false, MilkUsed);
            debug_MilkUsageTimer = Settings.AddText("Milk Usage Timer: 0", false);

            string[] _soundFields = Utils.GetEnumFields<ScreamSoundType>();
            string[] _fearFields = Utils.GetEnumFields<ScreamFearType>();
            string[] _paralysisFields = Utils.GetEnumFields<ScreamParalysisType>();

            debug_ScreamerSound = Settings.AddDropDownList("SoundScreamers", "Sound", _soundFields, 0);
            Settings.AddButton("Trigger", TriggerSoundScreamer);

            debug_ScreamerFear = Settings.AddDropDownList("FearScreamers", "Fear", _fearFields, 0);
            Settings.AddButton("Trigger", TriggerFearScreamer);

            debug_ScreamerParalysis = Settings.AddDropDownList("PrlsScreamers", "Paralysis", _paralysisFields, 0);
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
            debug_IsDead.SetValue($"Is Dead: {Logic.IsDead}");
            debug_GameFinished.SetValue(Logic.GameFinished);

            debug_TimeOfDay.SetValue($"Time Of Day: {TimeOfDay.Value}");

            if (Logic.MilkUsed)
            {
                int _milkTimer = 60 - (Logic.MilkUseTime - DateTime.Now).Seconds;
                int _clampedMilkTimer = Mathf.Clamp(_milkTimer, 0, 60);
                debug_MilkUsageTimer.SetValue($"Milk Usage Timer: {_clampedMilkTimer} seconds ago");
                debug_MilkUsageTimer.SetVisibility(true);
            }
            else
                debug_MilkUsageTimer.SetVisibility(false);

            debug_CurrentDay.SetValue($"Current Day: {CurrentDay.Value}");
            debug_LastDayMinigame.SetValue($"Last Day Minigame: {Logic.LastDayMinigame}");
        }

        public static void SetSettingsVisible(bool state)
            => headers.ForEach(v => v?.SetVisibility(state));

        static void PsychoValueChanged()
            => Logic.SetValue(debug_Psycho.GetValue());

        static void PointsValueChanged()
            => Logic.SetPoints(debug_Points.GetValue());

        static void SetGameFinished()
        {
            if (Logic.GameFinished || Logic.IsDeadByGame) return;

            bool _newValue = debug_GameFinished.GetValue();
            if (_newValue)
            {
                Logic.FinishShizGame();
                return;
            }
        }

        static void MilkUsed()
            => debug_MilkUsed.SetValue(Logic.MilkUsed);

        static void FillNotebook()
        {
            int _index = debug_NotebookFillType.GetSelectedItemIndex();
            bool _useRandom = _index == 2;

            Globals.Notebook?.ClearPages();
            for (int i = 1; i < 14; i++)
            {
                Notebook.TryAddPage(new NotebookPage
                {
                    index = i,
                    isTruePage = _useRandom ? (Random.Range(0, 2) == 1) : _index == 0
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

            Logic.LastDayMinigame--;
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
            string _itemName = debug_SpawnItem.GetSelectedItemName();
            Vector3 _pos = Globals.Player.position;

            switch (_itemName)
            {
                case "churchcandle":
                    ItemsPool.AddItem(ResourcesStorage.Candle_prefab, _pos, Vector3.zero);
                    break;
                case "fernflower":
                    ItemsPool.AddItem(ResourcesStorage.FernFlower_prefab, _pos, Vector3.zero);
                    break;
                case "mushroom":
                    ItemsPool.AddItem(ResourcesStorage.Mushroom_prefab, _pos, Vector3.zero);
                    break;
                case "blackegg":
                    ItemsPool.AddItem(ResourcesStorage.BlackEgg_prefab, _pos, Vector3.zero);
                    break;
                case "walnut":
                    ItemsPool.AddItem(ResourcesStorage.Walnut_prefab, _pos, Vector3.zero);
                    break;

                default:
                    return;
            }
            ModConsole.Print($"{_itemName} spawned on player pos");

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
            => Logic.ChangeWorld(Logic.InHorror ? eWorldType.MAIN : eWorldType.HORROR);

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
            if (Globals.Pills?.self == null)
            {
                ModConsole.Error("Pills not exists!");
                return;
            }

            GameObject.Find("PLAYER").transform.position = Globals.Pills.self.transform.position;
        }

        static void KillHeartAttack()
            => Logic.KillHeartAttack();

        static void SuicideOnRailroad()
            => Logic.KillUsingTrain();
    }
}
#endif