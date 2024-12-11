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

        internal static FixedHUD _hud = null;
        internal static GameObject death = null;
        internal static GameObject knockOut = null;
        internal static ShizAnimPlayer shizAnimPlayer = null;
        internal static FsmFloat psycho = new FsmFloat { Name = "PsychoValue", Value = 100f };

        internal static readonly Dictionary<string, float> config = new Dictionary<string, float>
        {
            // Increase
            ["FARMER_QUEST"]     = 5f,
            ["YOKKE_RELOCATION"] = 2f,
            ["GRANNY_CHURCH"]    = 0.3f,
            ["JUNK_YARD"]        = 0.25f,
            ["TEIMO_ADS"]        = 0.2f,
            ["YOKKE_DROPOFF"]    = 0.2f,
            ["GRANNY_DELIVERY"]  = 0.2f,
            ["SEPTIC_TANK"]      = 0.1f,

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
                if (_hud == null) return;
                if (isDead) return;

                if (value <= MIN_VALUE && !inHorror)
                    ChangeWorld(eWorldType.HORROR);
                else if (value > MAX_VALUE)
                    KillHeartAttack();
                else if (_hud?.IsElementExist("Psycho") == true)
                {
                    float clamped = Mathf.Clamp(value / MAX_VALUE, 0f, 1f);
                    _hud.SetElementScale("Psycho", new Vector3(clamped, 1f));
                    _hud.SetElementColor("Psycho", (clamped >= 0.85f && inHorror) ? Color.red : Color.white);
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
                if (_hud == null) return;
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
            if (type == eWorldType.MAIN)
            {
                Value = 100f;

                if (Globals.envelopeObject?.activeSelf == true)
                {
                    Globals.envelopeObject.SetActive(false);
                    envelopeSpawned = false;
                }
            }
            else
            {
                Value = 0f;

                if (Globals.envelopeObject == null || Globals.envelopeObject?.activeSelf == false)
                {
                    Globals.envelopeObject.SetActive(true);
                    envelopeSpawned = true;
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

                StateHook.Inject(train.gameObject, "Player", "Die 2", -1, _ =>
                {
                    Transform paper = death.transform.Find("GameOverScreen/Paper/Train");
                    paper.Find("TextFI").GetComponent<TextMesh>().text = PAPER_TEXT_FI_POINTS;
                    paper.Find("TextEN").GetComponent<TextMesh>().text = Locales.DEATH_PAPER[1, Globals.CurrentLang]; // PAPER_TEXT_EN_POINTS;
                });

                train.SendEvent("FINISHED"); // reset current state
                while (train.ActiveStateName != "State 2")
                    train.SendEvent("FINISHED");

                shizAnimPlayer.PlayAnimation("sleep_knockout", default, 15f, default, () =>
                {
                    player.transform.position = new Vector3(244.3646f, -1.039873f, -1262.394f);
                    player.transform.eulerAngles = new Vector3(0f, 233.4375f, 0f);

                    shizAnimPlayer.PlayAnimation("sleep_off", true, 5f, default);
                });
            }
            catch (Exception e)
            {
                ModConsole.Error($"Error in KillUsingTrain;\n{e.GetFullMessage()}");
            }
        }

        internal static void KillHeartAttack()
        {
            try
            {
                SoundManager.PlayDeathSound();
                shizAnimPlayer.PlayAnimation("sleep_knockout", default, default, default,
                    () => KillCustom(Locales.DEATH_PAPER[0, Globals.CurrentLang], PAPER_TEXT_FI_MAX)
                );
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

            _hud.RemoveElement("Psycho");
            _hud.Structurize();

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

                shizAnimPlayer?.PlayAnimation("sleep_knockout", default, 8f, default, () =>
                {
                    //player.transform.position = new Vector3(-11.12955f, -0.2938208f, 13.61279f);
                    //player.transform.eulerAngles = new Vector3(0f, 158.85f, 0f);

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

                    shizAnimPlayer?.PlayAnimation("sleep_off", true, default, default, () => {
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

        static void KillCustom(string en, string fi)
        {
            if (isDead) return;

            try
            {
                Transform paper = death.transform.Find("GameOverScreen/Paper/Fatigue");
                paper.Find("TextFI").GetComponent<TextMesh>().text = fi;
                paper.Find("TextEN").GetComponent<TextMesh>().text = en;
                death.SetActive(true);
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Error in killCustom method\n{e.GetFullMessage()}");
            }

            isDead = true;
        }
    }
}