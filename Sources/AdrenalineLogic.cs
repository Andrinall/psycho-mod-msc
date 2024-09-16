using System;
using System.Collections.Generic;

using Harmony;
using HealthMod;
using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    internal static class AdrenalineLogic
    {
        private static readonly string PAPER_TEXT_FI = "Mies kuoli\nsydänkohtaukseen";
        private static readonly string PAPER_TEXT_EN_MAX = "Man found\ndead of\nheart attack\nin region of\nAlivieska";
        private static readonly string PAPER_TEXT_EN_MIN = "Man found\ndead of\ncardiac arrest\nin region of\nAlivieska";

        private static bool _debug = false;
        private static float _value = 100f;
        private static float _lossRate = 0f;
        private static bool _lockDecrease = false;
        private static float _lockCooldown = 0f; // 12000 == 1 minute

        internal static readonly float MIN_ADRENALINE = 0f;
        internal static readonly float MAX_ADRENALINE = 200f;

        internal static int LastDayUpdated = 1;
        internal static bool isDead = false;
        internal static bool envelopeSpawned = false;
        internal static GameObject mailboxSheet = null;

        internal static FixedHUD _hud;

        internal static Dictionary<string, float> config = new Dictionary<string, float>
        {
            // timed
            ["LOSS_RATE_SPEED"] = 0.0005f,
            ["MIN_LOSS_RATE"] = 0.0f,
            ["MAX_LOSS_RATE"] = 3.0f,
            ["DEFAULT_DECREASE"] = 0.18f,
            ["SPRINT_INCREASE"] = 0.10f,
            ["HIGHSPEED_INCREASE"] = 0.35f,
            ["BROKEN_WINDSHIELD_INCREASE"] = 0.35f,
            ["HELMET_DECREASE"] = 0.08f,
            ["SEATBELT_DECREASE"] = 0.08f,
            ["FIGHT_INCREASE"] = 0.47f,
            ["HOUSE_BURNING"] = 0.56f,
            ["TEIMO_PISS"] = 1.0f,
            ["SPILL_SHIT"] = 1.1f,
            ["RALLY_PLAYER"] = 1f,
            ["SMOKING_DECREASE"] = 0.3f,

            // once
            ["MURDERER_THREAT"] = 30f,
            ["MURDERER_HIT"] = 15f,
            ["WINDOW_BREAK_INCREASE"] = 15.0f,
            ["GUARD_CATCH"] = 2.50f,
            ["VENTTI_WIN"] = 11.0f,
            ["JANNI_PETTERI_HIT"] = 45.5f,
            ["TEIMO_SWEAR"] = 0.9f,
            ["PISS_ON_DEVICES"] = 25f,
            ["SPARKS_WIRING"] = 15f,
            ["ENERGY_DRINK_INCREASE"] = 15f,
            ["COFFEE_INCREASE"] = 9.3f,
            ["DRIVEBY_INCREASE"] = 5f,
            ["CRASH_INCREASE"] = 20f,
            ["SLEEP_DECREASE"] = 20f,

            // vars for check
            ["PUB_COFFEE_PRICE"] = 250f,
            ["REQUIRED_SPEED_Jonnez"] = 70f,
            ["REQUIRED_SPEED_Satsuma"] = 120f,
            ["REQUIRED_SPEED_Ferndale"] = 110f,
            ["REQUIRED_SPEED_Hayosiko"] = 100f,
            ["REQUIRED_SPEED_Fittan"] = 70f,
            ["REQUIRED_SPEED_Gifu"] = 70f,

            ["REQUIRED_CRASH_SPEED"] = 30f,
            ["REQUIRED_WINDSHIELD_SPEED"] = 45f
        };

        internal static float Value {
            get { return _value; }
            set
            {
                if (IsDecreaseLocked() && _debug) return;

                _value = value;

                if (_hud == null) return;
                if (isDead) return;
                
                if (_value <= MIN_ADRENALINE)
                    KillCustom(PAPER_TEXT_EN_MIN, PAPER_TEXT_FI);
                else if (_value >= MAX_ADRENALINE)
                    KillCustom(PAPER_TEXT_EN_MAX, PAPER_TEXT_FI);
                else if (_hud?.IsElementExist("Adrenaline") == true)
                {
                    var clamped = Mathf.Clamp(value / 100f, 0f, 2f);

                    _hud.SetElementScale("Adrenaline", new Vector3(Mathf.Clamp(value / 200f, 0f, 1f), 1f));

                    Color color;
                    ASIndex index;
                    if (clamped <= 0.15f || clamped >= 1.75f)
                    {
                        color = Color.red;
                        index = (clamped <= 0.15f) ? ASIndex.HEART10 : ASIndex.HEART50;
                    }
                    else if (clamped >= 0.75f && clamped <= 1.55f)
                    {
                        color = Color.green;
                        index = ASIndex.HEART30;
                    }
                    else
                    {
                        color = Color.white;
                        index = ASIndex.HEART30;
                    }

                    Utils.PlaySound(clamped >= 1.9f ? ASIndex.HEARTBUST : index);

                    _hud.SetElementColor("Adrenaline", color);
                }
            }
        }

        /// <summary>
        /// Adrenaline value decrease multiplier
        /// </summary>
        internal static float LossRate
        {
            get { return _lossRate; }
            set {
                if (IsDecreaseLocked()) return;
                _lossRate = Mathf.Clamp(value, 0f, 50f);
            }
        }

        /// <summary>
        /// Increase lossRate & decrease adrenaline value. Called per FixedUpdate from GlobalHandler.
        /// </summary>
        internal static void Tick()
        {
            var lossSpeed = config.GetValueSafe("LOSS_RATE_SPEED");
            LossRate += lossSpeed * Time.fixedDeltaTime;

            if (IsDecreaseLocked() && _debug) return;
            Value -= config.GetValueSafe("DEFAULT_DECREASE") * LossRate * Time.fixedDeltaTime; // basic decrease adrenaline
        }
        
        /// <summary>
        /// Increase adrenaline value using multiply your value to Time.fixedDeltaTime (0.02f)
        /// </summary>
        internal static void IncreaseTimed(float val)
        {
            Value += val * Time.fixedDeltaTime;
        }

        /// <summary>
        /// Increase adrenaline value using your value
        /// (adrenaline += value)
        /// </summary>
        internal static void IncreaseOnce(float val)
        {
            Value += val;
        }

        /// <summary>
        /// Reset lossRate to optimal value using last updated day from internal variable
        /// </summary>
        internal static void UpdateLossRatePerDay()
        {
            LossRate = 0.2f * ((float)LastDayUpdated / 4f);
        }

        /// <summary>
        /// Reset lossRate to optimal value using your value for calculate day
        /// </summary>
        internal static void UpdateLossRatePerDay(int day)
        {
            LossRate = 0.2f * ((float)day / 4f);
        }

        /// <summary>
        /// Lock lossRate decrease for any ticks (seconds * 200)
        /// </summary>
        internal static void SetDecreaseLocked(bool state, float time = 12000f, bool debug = false)
        {
            if (state == _lockDecrease) return;
            _lockDecrease = state;
            _lockCooldown = time;
            _debug = debug;
        }

        /// <summary>
        /// Returns a current time for unlock lossRate decrease
        /// </summary>
        internal static float GetDecreaseLockTime()
        {
            return (_lockDecrease && _lockCooldown > 0) ? _lockCooldown : 0f;
        }

        /// <summary>
        /// Returns status of lossRate decrease lock
        /// </summary>
        internal static bool IsDecreaseLocked()
        {
            if (_lockCooldown > 0)
                _lockCooldown -= Time.fixedDeltaTime;
            if (_lockCooldown < 0) _lockCooldown = 0;

            return (_lockDecrease && _lockCooldown > 0);
        }

        /// <summary>
        /// Kills player by heart attack
        /// </summary>
        private static void KillCustom(string en, string fi)
        {
            if (isDead) return;

            Utils.PrintDebug("KillCustom called!!");
            try
            {
                if (ModLoader.IsModPresent("Health"))
                {
                    Utils.PlayDeathSound();
                    Health.killCustom(en, fi);
                    isDead = true;
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Error in health.killcustom\n{e.GetFullMessage()}");
            }

            try
            {
                if (isDead) return;
                Utils.PlayDeathSound();
                var death = GameObject.Find("Systems/Death");
                if (death == null) return;
                var paper = death.transform.Find("GameOverScreen/Paper/Fatigue");
                death.SetActive(true);
                paper.Find("TextEN").GetComponent<TextMesh>().text = en;
                paper.Find("TextFI").GetComponent<TextMesh>().text = fi;
                isDead = true;
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Error in killCustom method\n{e.GetFullMessage()}");
            }
        }
    }
}
