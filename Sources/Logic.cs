using System;
using System.Collections.Generic;

using Harmony;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho
{
    internal enum eWorldType { MAIN, HORROR }

    internal static class Logic
    {
        static readonly string PAPER_TEXT_FI_MAX = "Mies kuoli\nsydänkohtaukseen";
        //static readonly string PAPER_TEXT_EN_MAX = "Man found\ndead of\nheart attack\nin region of\nAlivieska";

        static readonly string PAPER_TEXT_FI_POINTS = "\nMies teki itsemurhan";
        //static readonly string PAPER_TEXT_EN_POINTS = "Man found\nafter committing\nsuicide in\nregion of\nAlivieska";

        static readonly float DEFAULT_DECREASE = 0.0185f; // -99.9 for 1 hour 30 minutes
        static readonly float DEFAULT_INCREASE = 0.0555f; // +99.9 for 30 minutes

        static readonly float MIN_VALUE = 0f;
        static readonly float MAX_VALUE = 100f;

        private static float _value = 100f;
        private static float _points = 0f;
        private static int beer_bottles_drunked = 0;

        public static int lastDayMinigame = 0;
        public static int numberOfSpawnedPages = 0;
        public static bool milkUsed = false;
        public static bool isDead = false;
        public static bool inHorror = false;
        public static bool envelopeSpawned = false;
        public static DateTime milkUseTime = DateTime.MinValue;

        internal static FsmFloat psycho = new FsmFloat { Name = "PsychoValue", Value = 100f };

        internal static readonly Dictionary<string, float> config = new Dictionary<string, float>
        {
            // Increase
            ["FARMER_QUEST"]     = 5.00f,
            ["YOKKE_RELOCATION"] = 2.00f,
            ["SUSKI_HELP"]       = 0.30f,
            ["GRANNY_CHURCH"]    = 0.30f,
            ["JUNK_YARD"]        = 0.25f,
            ["GRANNY_DELIVERY"]  = 0.20f,
            ["TEIMO_ADS"]        = 0.15f,
            ["YOKKE_DROPOFF"]    = 0.15f,
            ["WOOD_DELIVERY"]    = 0.15f,
            ["SEPTIC_TANK"]      = 0.10f,

            // Decrease
            ["GRAB_SUITCASE"] = 7.1f,
            ["HOUSE_BURNING"] = 3f,
            ["FITTAN_CRASH"]  = 3f,
            ["SPILL_SHIT"]    = 3f,
            ["SUSKI_HIT"]     = 3f,
            ["GRANNY_ANGRY"]  = 2f,
            ["NPC_HIT"]       = 1f,
            ["WINDOW_BREAK"]  = 0.5f,
            ["TEIMO_PISS"]    = 0.5f,
            ["DRUNK_BOOZE"]   = 0.2f,
            ["TEIMO_SWEARS"]  = 0.1f,
            ["DRUNK_BEER"]    = 0.05f,
            ["PLAYER_SWEARS"] = 0.01f
        };

        public static bool GameFinished { get; private set; } = false;

        public static float Value
        {
            get => _value;
            private set
            {
                if (GameFinished) return;

                _value = value;
                psycho.Value = value;

                if (isDead) return;

                if (value <= MIN_VALUE && !inHorror)
                    ChangeWorld(eWorldType.HORROR);
                else if (value > MAX_VALUE)
                    KillHeartAttack();
                else if (FixedHUD.IsElementExist("Psycho") == true)
                {
                    float clamped = Mathf.Clamp(value / MAX_VALUE, 0f, 1f);
                    FixedHUD.SetElementScale("Psycho", new Vector3(clamped, 1f));
                    FixedHUD.SetElementColor("Psycho", (clamped >= 0.85f && inHorror) ? Color.red : Color.white);
                }
            }
        }

        public static float Points
        {
            get => _points;
            private set
            {
                if (GameFinished) return;

                float prev = _points;
                _points = value;

                if (isDead) return;

                Utils.SetPictureImage();
                
                if (value > 7.0f)
                    FinishShizGame();
                else if (value < -7.0f)
                    KillUsingTrain();

                Utils.PrintDebug(eConsoleColors.YELLOW, $"New value for points {value}; prev: {prev}");
            }
        }

        internal static int BeerBottlesDrunked
        {
            get => beer_bottles_drunked;
            set
            {
                if (value == 5)
                {
                    PlayerCommittedOffence("DRUNK_BEER");
                    beer_bottles_drunked = 0;
                    return;
                }

                beer_bottles_drunked = value;
            }
        }


        public static void SetValue(float value) => Value = value;
        public static void SetPoints(float points) => Points = points;
        public static void ResetValue(float horror = 0f, float main = 100f)
            => Value = Mathf.Clamp(Value + (inHorror ? -horror : main), 0f, 100f);

        public static void ResetPoints() => Points = 0;

        public static void SetDefaultValues()
        {
            isDead = false;
            inHorror = false;
            envelopeSpawned = false;
            milkUsed = false;
            Value = 100f;
            Points = 0f;
        }

        public static void ChangeWorld(eWorldType type)
        {
            if (GameFinished) return;
            
            inHorror = type == eWorldType.HORROR;
            if (inHorror)
            {
                Value = 0f;

                if (Globals.envelopeObject == null || Globals.envelopeObject?.activeSelf == false)
                {
                    Globals.envelopeObject.SetActive(true);
                    envelopeSpawned = true;
                }
            }
            else
            {
                Value = 100f;

                if (Globals.envelopeObject?.activeSelf == true)
                {
                    Globals.envelopeObject.SetActive(false);
                    envelopeSpawned = false;
                }
            }

            Utils.PrintDebug(eConsoleColors.GREEN, $"World changed to {(type == eWorldType.MAIN ? "MAIN" : "HORROR")}");
            KnockOutPlayer();
        }


        internal static void Tick()
        {
            if (GameFinished) return;

            if (inHorror)
            {
                Value += DEFAULT_INCREASE * Time.fixedDeltaTime;
                return;
            }

            Value -= DEFAULT_DECREASE * Time.fixedDeltaTime;
        }

        internal static void PlayerCompleteJob(string job, int multiplier = 1, string comment = default)
        {
            Points += config.GetValueSafe(job) * multiplier;
            Utils.PrintDebug(eConsoleColors.GREEN, $"Player complete job : {job} [mult X{multiplier}] \"{comment}\"");
        }

        internal static void PlayerCommittedOffence(string offence, string comment = default)
        {
            float previous = Points;
            float newValue = Points - config.GetValueSafe(offence);

            if (newValue < previous && offence != "PLAYER_SWEARS" && !offence.Contains("DRUNK"))
                ResetValue(horror: 25, main: 15f);

            Points = newValue;
            Utils.PrintDebug(eConsoleColors.RED, $"Player committed offence : {offence} \"{comment}\"");
        }

        internal static void KillUsingTrain()
        {
            try
            {
                PlayMakerFSM train = GameObject.Find("TRAIN/SpawnEast/TRAIN").GetPlayMaker("Move");
                GameObject player = GameObject.Find("PLAYER");
                player.GetComponent<CharacterMotor>().canControl = false;

                StateHook.Inject(train.gameObject, "Player", "Die 2",
                    () => DeathSystem.KillCustom("Train", Locales.DEATH_PAPER[1, Globals.CurrentLang], PAPER_TEXT_FI_POINTS), 0); // -1

                train.SendEvent("FINISHED"); // reset current state
                while (train.ActiveStateName != "State 2")
                    train.SendEvent("FINISHED");

                ShizAnimPlayer.PlayAnimation("sleep_knockout", 15f, default, () =>
                {
                    player.transform.position = new Vector3(244.3646f, -1.039873f, -1262.394f);
                    player.transform.eulerAngles = new Vector3(0f, 233.4375f, 0f);

                    ShizAnimPlayer.PlayAnimation("sleep_off", 5f, default);
                });
            }
            catch (Exception e)
            {
                ModConsole.Error($"Error in KillUsingTrain;\n{e.GetFullMessage()}");
            }
        }

        internal static void KillHeartAttack()
        {
            if (!inHorror) return;
            try
            {
                SoundManager.PlayDeathSound();
                DeathSystem.KillCustom("Fatigue", Locales.DEATH_PAPER[0, Globals.CurrentLang], PAPER_TEXT_FI_MAX);
            }
            catch (Exception e)
            {
                ModConsole.Error($"Error in KillHearthAttack;\n{e.GetFullMessage()}");
            }
        }

        internal static void FinishShizGame()
        {
            GameFinished = true;
            if (inHorror) ChangeWorld(eWorldType.MAIN);

            FixedHUD.RemoveElement("Psycho");
            FixedHUD.Structurize();

            Utils.PrintDebug(eConsoleColors.GREEN, "Shiz game finished!");
        }


        static void KnockOutPlayer()
        {
            try
            {
                GameObject player = GameObject.Find("PLAYER");
                
                CharacterMotor motor = player.GetComponent<CharacterMotor>();
                motor.canControl = false;

                FsmFloat volume = Utils.GetGlobalVariable<FsmFloat>("GameVolume");
                volume.Value = 0;

                ShizAnimPlayer.PlayAnimation("sleep_knockout", 8f, default, () =>
                {
                    WorldManager.ChangeWorldTextures(inHorror);
                    WorldManager.ChangeBedroomModels();
                    Utils.ChangeSmokingModel();
                    SoundManager.ChangeFliesSounds();
                    WorldManager.StopCloudsOrRandomize();
                    WorldManager.ChangeCameraFog();
                    WorldManager.ChangeWalkersAnimation();
                    WorldManager.SetHandsActive(inHorror);

                    GameObject.Find("CustomSuicidals(Clone)")?.SetActive(inHorror);
                    _changeFittanDriverHeadPivotRotation();

                    ShizAnimPlayer.PlayAnimation("sleep_off", default, default, () => {
                        motor.canControl = true;
                        volume.Value = 1;
                    });
                });
            }
            catch (Exception e)
            {
                ModConsole.Error($"Error in KnockOutPlayer;\n{e.GetFullMessage()}");
            }
        }

        static void _changeFittanDriverHeadPivotRotation()
        {
            GameObject _fittanDriverHeadPivot =
                GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN/Driver/skeleton/pelvis/spine_middle/spine_upper/HeadPivot");

            if (!_fittanDriverHeadPivot) return;
            _fittanDriverHeadPivot.GetPlayMaker("Look").enabled = !inHorror;

            Transform _head = _fittanDriverHeadPivot.transform.Find("head");
            if (!_head) return;

            _head.localRotation = Quaternion.Euler(new Vector3(
                _head.localEulerAngles.x,
                _head.localEulerAngles.y,
                inHorror ? 64f : 270f
            ));

            _head.Find("eye_glasses_regular").gameObject.SetActive(!inHorror);
        }
    }
}