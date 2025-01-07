
using System;
using System.Linq;
using System.Collections.Generic;

using Harmony;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Ambient;

using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    internal enum eWorldType { MAIN, HORROR }

    internal static class Logic
    {
        static readonly string PAPER_TEXT_FI_MAX = "Mies kuoli\nsydänkohtaukseen";
        static readonly string PAPER_TEXT_FI_POINTS = "\nMies teki itsemurhan";

        static readonly float DEFAULT_DECREASE = 0.0185f; // -99.9 for 1 hour 30 minutes
        static readonly float DEFAULT_INCREASE = 0.0555f; // +99.9 for 30 minutes

        static readonly float MIN_VALUE = 0f;
        static readonly float MAX_VALUE = 100f;

        private static float _value = 100f;
        private static float _points = 0f;
        
        private static int beerBottlesDrunked = 0;

        public static int LastDayMinigame = 0;
        public static bool MilkUsed = false;
        public static bool IsDead = false;
        public static bool InHorror = false;
        public static bool EnvelopeSpawned = false;

        public static AmbientTrigger CurrentAmbientTrigger = null;
        public static DateTime MilkUseTime = DateTime.MinValue;
        
        internal static DateTime LastTimeTriggerScreamer { get; private set; } = default;
        internal static int MinutesToNextScreamer { get; private set; } = 0;

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
            ["SPILL_SHIT"]    = 3f / 5,
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

        public static bool GameFinished = false;

        public static float Value
        {
            get => _value;
            private set
            {
                if (GameFinished) return;

                _value = value;
                psycho.Value = value;

                if (IsDead) return;

                if (value <= MIN_VALUE && !InHorror)
                    ChangeWorld(eWorldType.HORROR);
                else if (value > MAX_VALUE)
                    KillHeartAttack();
                else if (FixedHUD.IsElementExist("Psycho") == true)
                {
                    float clamped = Mathf.Clamp(value / MAX_VALUE, 0f, 1f);
                    FixedHUD.SetElementScale("Psycho", new Vector3(clamped, 1f));
                    FixedHUD.SetElementColor("Psycho", (clamped >= 0.85f && InHorror) ? Color.red : Color.white);
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

                if (IsDead) return;

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
            get => beerBottlesDrunked;
            set
            {
                if (value == 5)
                {
                    PlayerCommittedOffence("DRUNK_BEER");
                    beerBottlesDrunked = 0;
                    return;
                }

                beerBottlesDrunked = value;
            }
        }


        public static void SetValue(float value)
            => Value = value;

        public static void SetPoints(float points)
            => Points = points;

        public static void ResetValue(float horror = 0f, float main = 100f)
            => Value = Mathf.Clamp(Value + (InHorror ? -horror : main), 0f, 100f);

        public static void ResetValue()
            => Value = (InHorror ? 0f : 100f);

        public static void ResetPoints()
            => Points = 0;

        public static void SetDefaultValues()
        {
            IsDead = false;
            GameFinished = false;
            InHorror = false;
            EnvelopeSpawned = false;
            MilkUsed = false;
            LastDayMinigame = 0;
            beerBottlesDrunked = 0;
            Value = 100f;
            Points = 0f;
        }

        public static bool IsFullScreenScreamerAvailableForTrigger()
        {
            if ((DateTime.Now - LastTimeTriggerScreamer).Minutes < MinutesToNextScreamer)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, $"FullScreenScreamer timer is not counting down");
                return false;
            }

            return true;
        }

        public static void EnableRandomFullScreenScreamer()
        {
            SoundManager.EnableRandomSoundForFullScreenScreamer();

            LastTimeTriggerScreamer = DateTime.Now;
            MinutesToNextScreamer = UnityEngine.Random.Range(10, 20);

            int _randomTexture = UnityEngine.Random.Range(0, ResourcesStorage.FullScreenScreamerTextures.Count);
            Globals.FullScreenScreamer.transform
                .GetChild(1).gameObject
                .GetComponent<MeshRenderer>().material
                .SetTexture("_MainTex", ResourcesStorage.FullScreenScreamerTextures[_randomTexture]);

            Globals.FullScreenScreamer.SetActive(true);
        }

        public static void ResetFullScreenScreamerCooldown()
        {
            LastTimeTriggerScreamer = default;
            MinutesToNextScreamer = 0;
        }

        public static void ChangeWorld(eWorldType type)
        {
            if (GameFinished) return;
            
            InHorror = type == eWorldType.HORROR;
            if (InHorror)
            {
                Value = 0f;

                if (Globals.EnvelopeObject == null || Globals.EnvelopeObject?.activeSelf == false)
                {
                    Globals.EnvelopeObject.SetActive(true);
                    EnvelopeSpawned = true;
                }
            }
            else
            {
                Value = 100f;

                if (Globals.EnvelopeObject?.activeSelf == true)
                {
                    Globals.EnvelopeObject.SetActive(false);
                    EnvelopeSpawned = false;
                }
            }

            Utils.PrintDebug(eConsoleColors.GREEN, $"World changed to {(type == eWorldType.MAIN ? "MAIN" : "HORROR")}");
            KnockOutPlayer();
        }


        internal static void Tick()
        {
            if (GameFinished) return;

            if (InHorror)
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
            float _previous = Points;
            float _newValue = Points - config.GetValueSafe(offence);

            if (_newValue < _previous && offence != "PLAYER_SWEARS" && !offence.Contains("DRUNK"))
                ResetValue(horror: 25, main: 15f);

            Points = _newValue;
            Utils.PrintDebug(eConsoleColors.RED, $"Player committed offence : {offence} \"{comment}\"");
        }

        internal static void KillUsingTrain()
        {
            try
            {
                PlayMakerFSM _train = GameObject.Find("TRAIN/SpawnEast/TRAIN").GetPlayMaker("Move");
                Globals.Player.GetComponent<CharacterMotor>().canControl = false;

                StateHook.Inject(_train.gameObject, "Player", "Die 2",
                    () => DeathSystem.KillCustom("Train", Locales.DEATH_PAPER[1, Globals.CurrentLang], PAPER_TEXT_FI_POINTS), 0); // -1

                _train.SendEvent("FINISHED"); // reset current state
                while (_train.ActiveStateName != "State 2")
                    _train.SendEvent("FINISHED");

                ShizAnimPlayer.PlayAnimation("sleep_knockout", 15f, default, () =>
                {
                    Globals.Player.position = new Vector3(244.3646f, -1.039873f, -1262.394f);
                    Globals.Player.eulerAngles = new Vector3(0f, 233.4375f, 0f);

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
            if (!InHorror) return;
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
            if (InHorror)
                ChangeWorld(eWorldType.MAIN);

            GameFinished = true;
            FixedHUD.RemoveElement("Psycho");
            FixedHUD.Structurize();

            AudioSource.PlayClipAtPoint(ResourcesStorage.FinishGame_clip, GameObject.Find("PLAYER").transform.position);
            DestroyAllObjects();

            Utils.PrintDebug(eConsoleColors.GREEN, "Shiz game finished!");
        }

        internal static void DestroyAllObjects()
        {
            EventsManager.OnScreamerTriggered.RemoveAllListeners();
            EventsManager.OnScreamerFinished.RemoveAllListeners();

            foreach (CatchedComponent _catched in Resources.FindObjectsOfTypeAll<CatchedComponent>())
            {
                if (_catched == null) continue;

                string _typeName = _catched.GetType().Name;
                string[] _objectsToDestroy = new string[]
                {
                    "Minigame", "AmbientTrigger", "MovingHand",
                    "MovingUncleHead","MummolaCrawl", "Sketchbook",
                    "Notebook", "FernFlowerSpawner", "AngryRoosterPoster",
                    "Pentagram", "PentagramEvents"
                };

                if (_objectsToDestroy.Contains(_typeName))
                {
                    Object.Destroy(_catched.gameObject);
                    continue;
                }

                Object.Destroy(_catched);
            }

            Object.Destroy(GameObject.Find("DingonbiisiAmbientTrigger"));
            Object.Destroy(GameObject.Find("GlobalAmbient(PsychoMod)"));
            Object.Destroy(GameObject.Find("GlobalPsychoAmbient(PsychoMod)"));
            Object.Destroy(GameObject.Find("HouseAmbientTrigger"));
            Object.Destroy(GameObject.Find("IslandAmbientTrigger"));
            Object.Destroy(GameObject.Find("Picture(Clone)"));

            ItemsPool.RemoveItems(v => true);

            StateHook.DisposeAllHooks();
        }


        static void KnockOutPlayer()
        {
            try
            {                
                CharacterMotor _motor = Globals.Player.GetComponent<CharacterMotor>();
                _motor.canControl = false;

                FsmFloat _volume = Utils.GetGlobalVariable<FsmFloat>("GameVolume");
                _volume.Value = 0;

                ShizAnimPlayer.PlayAnimation("sleep_knockout", 8f, default, () =>
                {
                    WorldManager.ChangeWorldTextures(InHorror);
                    WorldManager.ChangeBedroomModels();
                    Utils.ChangeSmokingModel();
                    SoundManager.ChangeFliesSounds();
                    WorldManager.StopCloudsOrRandomize();
                    WorldManager.ChangeCameraFog();
                    WorldManager.ChangeWalkersAnimation();
                    WorldManager.SetHandsActive(InHorror);

                    GameObject.Find("CustomSuicidals(Clone)")?.SetActive(InHorror);
                    _changeFittanDriverHeadPivotRotation();

                    ShizAnimPlayer.PlayAnimation("sleep_off", default, default, () => {
                        _motor.canControl = true;
                        _volume.Value = 1;
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
                GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN/Driver/skeleton/pelvis/spine_middle/spine_upper/HeadPivot")
                ?? GameObject.Find("FITTAN/Driver/skeleton/pelvis/spine_middle/spine_upper/HeadPivot");

            if (!_fittanDriverHeadPivot) return;
            _fittanDriverHeadPivot.GetPlayMaker("Look").enabled = !InHorror;

            Transform _head = _fittanDriverHeadPivot.transform.Find("head");
            if (!_head) return;

            _head.localRotation = Quaternion.Euler(new Vector3(
                _head.localEulerAngles.x,
                InHorror ? 320f : 270f,
                InHorror ? 62f : 254f
            ));

            _head.Find("eye_glasses_regular").gameObject.SetActive(!InHorror);
        }
    }
}