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
        internal static bool loopAudio = false;

        internal static FixedHUD _hud;

        internal static Dictionary<string, float> config = new Dictionary<string, float>
        {
            // timed
            ["LOSS_RATE_SPEED"]            = 0.0006f,
            ["MIN_LOSS_RATE"]              = 0.0f,
            ["MAX_LOSS_RATE"]              = 3.0f,
            ["DEFAULT_DECREASE"]           = 0.18f,
            ["SPRINT_INCREASE"]            = 0.30f,
            ["HIGHSPEED_INCREASE"]         = 0.35f,
            ["BROKEN_WINDSHIELD_INCREASE"] = 0.35f,
            ["FIGHT_INCREASE"]             = 0.47f,
            ["HOUSE_BURNING"]              = 0.56f,
            ["TEIMO_PISS"]                 = 1.50f,
            ["SPILL_SHIT"]                 = 1.1f,
            
            // once
            ["MURDERER_THREAT"]       = 30f,
            ["MURDERER_HIT"]          = 15f,
            ["WINDOW_BREAK_INCREASE"] = 15.0f,
            ["GUARD_CATCH"]           = 2.50f,
            ["VENTTI_WIN"]            = 11.0f,
            ["JANNI_PETTERI_HIT"]     = 45.5f,
            ["TEIMO_SWEAR"]           = 3.0f,
            ["PISS_ON_DEVICES"]       = 25f,
            ["SPARKS_WIRING"]         = 15f,
            ["ENERGY_DRINK_INCREASE"] = 15f,
            ["COFFEE_INCREASE"]       = 9.3f,
            ["RALLY_PLAYER"]          = 1f,
            ["DRIVEBY_INCREASE"]      = 5f,
            ["CRASH_INCREASE"]        = 20f,
            ["PUB_COFFEE_PRICE"]      = 14f,

            // vars for check
            ["REQUIRED_SPEED_Jonnez"]   = 70f,
            ["REQUIRED_SPEED_Satsuma"]  = 120f,
            ["REQUIRED_SPEED_Ferndale"] = 110f,
            ["REQUIRED_SPEED_Hayosiko"] = 110f,
            ["REQUIRED_SPEED_Fittan"]   = 70f,
            ["REQUIRED_SPEED_Gifu"]     = 70f,

            ["REQUIRED_CRASH_SPEED"] = 30f,
            ["REQUIRED_WINDSHIELD_SPEED"] = 45f
        };

        internal static float Value {
            get { return _value; }
            set
            {
                if (_hud == null) return;
                if (isDead) return;

                _value = value;

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

        internal static float LossRate
        {
            get { return _lossRate; }
            set {
                if (IsDecreaseLocked()) return;
                var MIN_LOSS_RATE = config.GetValueSafe("MIN_LOSS_RATE");
                var MAX_LOSS_RATE = config.GetValueSafe("MAX_LOSS_RATE");
                _lossRate = Mathf.Clamp(value, MIN_LOSS_RATE, MAX_LOSS_RATE);
            }
        }

        internal static void Tick()
        {
            var lossSpeed = config.GetValueSafe("LOSS_RATE_SPEED");
            LossRate += lossSpeed * Time.fixedDeltaTime;

            if (IsDecreaseLocked() && _debug) return;
            Value -= config.GetValueSafe("DEFAULT_DECREASE") * LossRate * Time.fixedDeltaTime; // basic decrease adrenaline
        }

        internal static void IncreaseTimed(float val)
        {
            Value += val * Time.fixedDeltaTime;
        }

        internal static void IncreaseOnce(float val)
        {
            Value += val;
        }

        internal static void UpdateLossRatePerDay()
        {
            LossRate = 0.2f * ((float)LastDayUpdated / 4);
        }

        internal static void UpdateLossRatePerDay(int day)
        {
            LossRate = 0.2f * ((float)day / 4);
        }

        internal static void SetDecreaseLocked(bool state, float time = 12000f, bool debug = false)
        {
            if (state == _lockDecrease) return;
            _lockDecrease = state;
            _lockCooldown = time;
            _debug = debug;
        }

        internal static float GetDecreaseLockTime()
        {
            return (_lockDecrease && _lockCooldown > 0) ? _lockCooldown : 0f;
        }

        internal static bool IsDecreaseLocked()
        {
            if (_lockCooldown > 0)
                _lockCooldown -= Time.fixedDeltaTime;
            if (_lockCooldown < 0) _lockCooldown = 0;

            return (_lockDecrease && _lockCooldown > 0);
        }

        internal static bool IsPrefab(this Transform tempTrans)
        {
            if (!tempTrans.gameObject.activeInHierarchy && tempTrans.gameObject.activeSelf)
            {
                return tempTrans.root == tempTrans;
            }
            return false;
        }

        private static void KillCustom(string en, string fi)
        {
            if (isDead) return;

            isDead = true;
            Utils.PrintDebug("KillCustom called!!");
            try
            {
                if (ModLoader.IsModPresent("Health"))
                {
                    Utils.PlayDeathSound();
                    Health.killCustom(en, fi);
                    return;
                }
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "error in health.killcustom");
            }

            try
            {
                Utils.PlayDeathSound();
                var death = GameObject.Find("Systems/Death");
                if (death == null) return;
                var paper = death.transform.Find("GameOverScreen/Paper/Fatigue");
                paper.Find("TextEN").GetComponent<TextMesh>().text = en;
                paper.Find("TextFI").GetComponent<TextMesh>().text = fi;
                death.SetActive(isDead);
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "error in custom method");
            }
        }
    }
}
