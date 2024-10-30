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
        static readonly string PAPER_TEXT_EN_MAX = "Man found\ndead of\nheart attack\nin region of\nAlivieska";

        static readonly string PAPER_TEXT_FI_POINTS = "\nMies teki itsemurhan";
        static readonly string PAPER_TEXT_EN_POINTS = "Man found\nafter committing\nsuicide in\nregion of\nAlivieska";

        static readonly float DEFAULT_DECREASE = 0.0185f; // -99.9 for 1 hour 30 minutes
        static readonly float DEFAULT_INCREASE = 0.0555f; // +99.9 for 30 minutes

        static readonly float MIN_VALUE = 0f;
        static readonly float MAX_VALUE = 100f;


        static bool _gameFinished = false;
        static float _points = 0;
        static float _value = 100f;


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

        internal static Dictionary<string, float> config = new Dictionary<string, float>
        {
            // Increase
            ["GRANNY_DELIVERY"] = 1f,
            ["GRANNY_CHURCH"] = 1f,
            ["TEIMO_ADS"] = 1f,
            ["FARMER_QUEST"] = 5f,
            ["SEPTIC_TANK"] = 0.2f,
            ["YOKKE_DROPOFF"] = 1f,
            ["YOKKE_RELOCATION"] = 2f,
            ["JUNK_YARD"] = 0.25f,

            // Decrease
            ["WINDOW_BREAK_INCREASE"] = 0.5f,
            ["TEIMO_PISS"] = 0.5f,
            ["TEIMO_SWEARS"] = 0.1f,
            ["NPC_HIT"] = 1f,
            ["GRANNY_ANGRY"] = 2f,
            ["HOUSE_BURNING"] = 3f,
            ["SPILL_SHIT"] = 3f,
            ["GRAB_SUITCASE"] = 7.1f,
            ["FITTAN_CRASH"] = 3f,
            ["SUSKI_HIT"] = 3f
        };



        public static float Value {
            get { return _value; }
            set
            {
                _value = value;
                psycho.Value = _value;
                if (_gameFinished) return;
                if (_hud == null) return;
                if (isDead) return;

                if (_value <= MIN_VALUE && !inHorror)
                    ChangeWorld(eWorldType.HORROR);
                else if (_value > MAX_VALUE)
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
            get { return _points; }
            set
            {
                float prev = _points;
                _points = value;
                if (_gameFinished) return;
                if (_hud == null) return;
                if (isDead) return;

                Utils.SetPictureImage();

                if (prev > _points)
                    Value = inHorror ? 0f : 100f; // reset shiz hud bar

                if (_points > 7.0f)
                    FinishShizGame();
                else if (_points < -7.0f)
                    KillUsingTrain();

                Utils.PrintDebug(eConsoleColors.YELLOW, $"New value for points {_points}; prev: {prev}");
            }
        }



        public static void ChangeWorld(eWorldType type)
        {
            if (type == eWorldType.MAIN)
            {
                inHorror = false;
                Value = 100f;

                if (Globals.envelopeObject?.activeSelf == true)
                {
                    Globals.envelopeObject.SetActive(false);
                    envelopeSpawned = false;
                }

                KnockOutPlayer();
                return;
            }

            inHorror = true;
            Value = 0f;

            if (Globals.envelopeObject == null || Globals.envelopeObject?.activeSelf == false)
            {
                Globals.envelopeObject.SetActive(true);
                envelopeSpawned = true;
            }

            KnockOutPlayer();
            Utils.PrintDebug(eConsoleColors.GREEN, $"World changed to {(type == 0 ? "MAIN" : "HORROR")}");
        }

        public static void ResetValue() => Value = inHorror ? 0f : 100f;
        public static void ResetPoints() => Points = 0;


        internal static void Tick()
        {
            if (_gameFinished) return;

            if (inHorror)
            {
                Value += DEFAULT_INCREASE * Time.fixedDeltaTime;
                return;
            }

            Value -= DEFAULT_DECREASE * Time.fixedDeltaTime;
        }

        internal static void PlayerCompleteJob(string job, int multiplier = 1, string comment = default)
            => Points += config.GetValueSafe(job) * multiplier;

        internal static void PlayerCommittedOffence(string offence, string comment = default)
            => Points -= config.GetValueSafe(offence);

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
                    paper.Find("TextEN").GetComponent<TextMesh>().text = PAPER_TEXT_EN_POINTS;
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
                    () => KillCustom(PAPER_TEXT_EN_MAX, PAPER_TEXT_FI_MAX)
                );
            }
            catch (Exception e)
            {
                ModConsole.Error($"Error in KillHearthAttack;\n{e.GetFullMessage()}");
            }
        }


        static void KnockOutPlayer()
        {
            try
            {
                GameObject player = GameObject.Find("PLAYER");
                CharacterMotor motor = player.GetComponent<CharacterMotor>();
                motor.canControl = false;

                shizAnimPlayer?.PlayAnimation("sleep_knockout", default, 8f, default, () =>
                {
                    player.transform.position = new Vector3(-11.12955f, -0.2938208f, 13.61279f);
                    player.transform.eulerAngles = new Vector3(0f, 158.85f, 0f);

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

                    shizAnimPlayer?.PlayAnimation("sleep_off", true, default, default, () => motor.canControl = true);
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

            Utils.PrintDebug($"fittan driver head pivot == null? {_fittanDriverHeadPivot == null}");
            if (!_fittanDriverHeadPivot) return;

            _fittanDriverHeadPivot.GetPlayMaker("Look").enabled = !inHorror;

            Transform _head = _fittanDriverHeadPivot.transform.Find("head");
            Utils.PrintDebug($"fittan HeadPivot/head == null? {_head == null}");
            if (!_head) return;
            _head.localRotation = Quaternion.Euler(new Vector3(
                _head.localEulerAngles.x,
                _head.localEulerAngles.y,
                inHorror ? 64f : 270f
            ));

            _head.Find("eye_glasses_regular").gameObject.SetActive(!inHorror);
        }

        static void FinishShizGame()
        {
            _gameFinished = true;
            // ...

            Utils.PrintDebug(eConsoleColors.YELLOW, "Shiz game finished!");
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