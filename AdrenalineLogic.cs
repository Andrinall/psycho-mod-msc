using Harmony;
using HealthMod;
using MSCLoader;
using System.Collections.Generic;
using UnityEngine;

namespace Adrenaline
{
    public class ConfigItem
    {
        public float Value;
        public float minValue = 0f;
        public float maxValue = 1f;
    }

    public static class AdrenalineLogic
    {
        private static readonly string PAPER_TEXT_FI = "Mies kuoli\nsydänkohtaukseen";
        private static readonly string PAPER_TEXT_EN_MAX = "Man found\ndead of\nheart attack\nin region of\nAlivieska";
        private static readonly string PAPER_TEXT_EN_MIN = "Man found\ndead of\ncardiac arrest\nin region of\nAlivieska";

        public static readonly float MIN_ADRENALINE = 0f;
        public static readonly float MAX_ADRENALINE = 200f;

        private static bool _debug = false;
        private static float _value = 100f;
        private static float _lossRate = 1.2f;
        private static bool  _lockDecrease = false;
        private static float _lockCooldown = 0f; // 12000 == 1 minute

        internal static FixedHUD _hud;
        public static Texture can_texture = null;
        public static Texture atlas_texture = null;
        public static Mesh empty_cup = null;
        public static Mesh coffee_cup = null;
        public static bool isDead = false;

        public static Dictionary<string, float> config = new Dictionary<string, float>
        {
            // timed
            ["LOSS_RATE_SPEED"]            = 0.50f,
            ["MIN_LOSS_RATE"]              = 0.40f,
            ["MAX_LOSS_RATE"]              = 1.4f,
            ["DEFAULT_DECREASE"]           = 0.18f,
            ["SPRINT_INCREASE"]            = 0.30f,
            ["HIGHSPEED_INCREASE"]         = 0.35f,
            ["BROKEN_WINDSHIELD_INCREASE"] = 0.35f,
            ["FIGHT_INCREASE"]             = 0.47f,
            ["HOUSE_BURNING"]              = 0.56f,
            ["TEIMO_PISS"]                 = 1.50f,
            ["SPILL_SHIT"]                 = 1.1f,
            
            // once
            ["WINDOW_BREAK_INCREASE"]      = 15.0f,
            ["GUARD_CATCH"]                = 2.50f,
            ["VENTTI_WIN"]                 = 11.0f,
            ["JANNI_PETTERI_HIT"]          = 45.5f,
            ["TEIMO_SWEAR"]                = 3.0f,
            ["PISS_ON_DEVICES"]            = 25f,
            ["SPARKS_WIRING"]              = 15f,
            ["MURDER_WALKING"]             = 0.45f,
            ["COFFEE_INCREASE"]            = 11.3f,
            ["RALLY_PLAYER"]               = 1f,

            // any
            ["PUB_COFFEE_PRICE"]        = 14f,

            ["REQUIRED_SPEED_Jonnez"]   = 70f,
            ["REQUIRED_SPEED_Satsuma"]  = 120f,
            ["REQUIRED_SPEED_Ferndale"] = 110f,
            ["REQUIRED_SPEED_Hayosiko"] = 110f,
            ["REQUIRED_SPEED_Fittan"]   = 70f,
            ["REQUIRED_SPEED_Gifu"]     = 70f,

            ["REQUIRED_WINDSHIELD_SPEED"] = 45f
        };

        public static float Value {
            get { return _value; }
            set
            {
                if (value <= MIN_ADRENALINE && !isDead)
                    KillCustom(PAPER_TEXT_EN_MIN, PAPER_TEXT_FI);
                else if ((value >= MAX_ADRENALINE) && !isDead)
                    KillCustom(PAPER_TEXT_EN_MAX, PAPER_TEXT_FI);
                else if (_hud?.IsElementExist("Adrenaline") == true)
                {
                    var clamped = Mathf.Clamp(value / 100f, 0f, 2f);

                    _hud.SetElementScale("Adrenaline", new Vector3(Mathf.Clamp(value / 200f, 0f, 1f), 1f));
                    
                    var color = Color.white;
                    if (clamped <= 0.15f || clamped >= 1.75f)
                        color = Color.red;
                    else if (clamped >= 0.75f && clamped <= 1.55f)
                        color = Color.green;

                    _hud.SetElementColor("Adrenaline", color);
                }
                _value = value;
            }
        }

        public static float LossRate
        {
            get { return _lossRate; }
            set {
                if (IsDecreaseLocked()) return;
                var MIN_LOSS_RATE = config.GetValueSafe("MIN_LOSS_RATE");
                var MAX_LOSS_RATE = config.GetValueSafe("MAX_LOSS_RATE");
                _lossRate = Mathf.Clamp(value, MIN_LOSS_RATE, MAX_LOSS_RATE);
            }
        }

        public static void Tick()
        {
            if (ModLoader.IsModPresent("Health"))
            {
                var lossSpeed = config.GetValueSafe("LOSS_RATE_SPEED");
                if (Health.hp < 30)
                    LossRate -= lossSpeed * Time.fixedDeltaTime;
                if (Health.hp > 80)
                    LossRate += lossSpeed * Time.fixedDeltaTime;
            }

            if (IsDecreaseLocked() && _debug) return;
            Value -= config.GetValueSafe("DEFAULT_DECREASE") * LossRate * Time.fixedDeltaTime; // basic decrease adrenaline
        }

        public static void IncreaseTimed(float val)
        {
            Value += val * Time.fixedDeltaTime;
        }

        public static void IncreaseOnce(float val)
        {
            Value += val;
        }

        public static void SetDecreaseLocked(bool state, float time = 12000f, bool debug = false)
        {
            if (state == _lockDecrease) return;
            _lockDecrease = state;
            _lockCooldown = time;
            _debug = debug;
        }

        public static float GetDecreaseLockTime()
        {
            return (_lockDecrease && _lockCooldown > 0) ? _lockCooldown : 0f;
        }

        public static bool IsDecreaseLocked()
        {
            if (_lockCooldown > 0)
                _lockCooldown -= Time.fixedDeltaTime;
            if (_lockCooldown < 0) _lockCooldown = 0;

            return (_lockDecrease && _lockCooldown > 0);
        }

        public static bool IsPrefab(this Transform tempTrans)
        {
            if (!tempTrans.gameObject.activeInHierarchy && tempTrans.gameObject.activeSelf)
            {
                return tempTrans.root == tempTrans;
            }
            return false;
        }

        private static void KillCustom(string en, string fi)
        {
            isDead = true;
            if (ModLoader.IsModPresent("Health"))
            {
                Health.killCustom(en, fi);
                return;
            }

            var death = GameObject.Find("Systems/Death");
            var paper = death.transform.Find("GameOverScreen/Paper/Fatigue");
            death.SetActive(true);
            paper.Find("TextEN").GetComponent<TextMesh>().text = en;
            paper.Find("TextFI").GetComponent<TextMesh>().text = fi;
        }
    }
}
