#if DEBUG
using System;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Features;


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

        static SettingsDropDownList debug_ScreamerSound;
        static SettingsDropDownList debug_ScreamerFear;
        static SettingsDropDownList debug_ScreamerParalysis;

        static SettingsText debug_CurrentDay;
        static SettingsText debug_LastDayMinigame;

        static SettingsDropDownList debug_PentaVeryGood;
        static SettingsDropDownList debug_PentaGood;
        static SettingsDropDownList debug_PentaNormal;
        static SettingsDropDownList debug_PentaBad;
        static SettingsDropDownList debug_PentaVeryBad;

        static FsmInt TimeOfDay;
        static FsmInt CurrentDay;

        static Minigame _minigame;

        static SettingsHeader debugSection;
        static SettingsHeader mainSection;
        static SettingsHeader screamersSection;
        static SettingsHeader minigameSection;
        static SettingsHeader pentagramSection;

        public static void Init()
        {
            debugSection = Settings.AddHeader("START DEBUG SECTION");
            mainSection = Settings.AddHeader("Main Section");
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
            Settings.AddButton("Initiate Heart Attack", KillHeartAttack);
            Settings.AddButton("Suicide On Railroad", SuicideOnRailroad);


            screamersSection = Settings.AddHeader("Screamers");
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


            minigameSection = Settings.AddHeader("Minigame");
            debug_CurrentDay = Settings.AddText("Current Day: 0");
            debug_LastDayMinigame = Settings.AddText("Last Day Minigame: 0");

            Settings.AddButton("Change Day", ChangeMinigameDay);
            Settings.AddButton("Get Card", GetMinigameCard);

            pentagramSection = Settings.AddHeader("Pentagram");
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

            Settings.AddText("\n \n \n \n \n \n ");

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
        {
            debugSection?.SetVisibility(state);
            mainSection?.SetVisibility(state);
            screamersSection?.SetVisibility(state);
            minigameSection?.SetVisibility(state);
            pentagramSection?.SetVisibility(state);
        }

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

        static void KillHeartAttack()
            => Logic.KillHeartAttack();

        static void SuicideOnRailroad()
            => Logic.KillUsingTrain();
    }
}
#endif