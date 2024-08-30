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
        private static readonly string PAPER_TEXT_EN = "Man found\ndead of\nheart attack\nin region of\nAlivieska";

        public static readonly float MIN_ADRENALINE = 0f;
        public static readonly float MAX_ADRENALINE = 200f;

        private static bool _debug = false;
        private static bool isDead = false;
        private static float _value = 100f;
        private static float _lossRate = 1.2f;
        private static bool  _lockDecrease = false;
        private static float _lockCooldown = 0f; // 12000 == 1 minute

        internal static FixedHUD _hud;
        public static Texture can_texture = null;
        public static Texture atlas_texture = null;
        public static Mesh empty_cup = null;
        public static Mesh coffee_cup = null;

        public static Dictionary<string, ConfigItem> config = new Dictionary<string, ConfigItem>
        {
            // timed
            ["LOSS_RATE_SPEED"]            = new ConfigItem { Value = 0.50f, minValue = 0.05f, maxValue = 1f  },
            ["MIN_LOSS_RATE"]              = new ConfigItem { Value = 0.40f, minValue = 0.2f, maxValue = 1f   },
            ["MAX_LOSS_RATE"]              = new ConfigItem { Value = 1.4f,  minValue = 1.0f, maxValue = 2.0f },
            ["DEFAULT_DECREASE"]           = new ConfigItem { Value = 0.18f, minValue = 0.1f, maxValue = 0.5f },
            ["SPRINT_INCREASE"]            = new ConfigItem { Value = 0.30f, minValue = 0.1f, maxValue = 0.5f },
            ["HIGHSPEED_INCREASE"]         = new ConfigItem { Value = 0.35f, minValue = 0.2f, maxValue = 0.7f },
            ["BROKEN_WINDSHIELD_INCREASE"] = new ConfigItem { Value = 0.35f, minValue = 0.2f, maxValue = 0.7f },
            ["FIGHT_INCREASE"]             = new ConfigItem { Value = 0.47f, minValue = 0.2f, maxValue = 0.7f },
            ["HOUSE_BURNING"]              = new ConfigItem { Value = 0.56f, minValue = 0.2f, maxValue = 2.5f },
            ["TEIMO_PISS"]                 = new ConfigItem { Value = 1.50f, minValue = 1.0f, maxValue = 5.0f },
            ["SPILL_SHIT"]                 = new ConfigItem { Value = 1.1f,  minValue = 0.5f, maxValue = 2.0f },
            
            // once
            ["WINDOW_BREAK_INCREASE"]      = new ConfigItem { Value = 15.0f, minValue = 5.0f, maxValue = 40f },
            ["GUARD_CATCH"]                = new ConfigItem { Value = 2.50f, minValue = 1.0f, maxValue = 10f },
            ["VENTTI_WIN"]                 = new ConfigItem { Value = 11.0f, minValue = 4.0f, maxValue = 15f },
            ["JANNI_PETTERI_HIT"]          = new ConfigItem { Value = 45.5f, minValue = 20f,  maxValue = 50f },
            ["TEIMO_SWEAR"]                = new ConfigItem { Value = 3.0f,  minValue = 1.5f, maxValue = 8f  },
            ["PISS_ON_DEVICES"]            = new ConfigItem { Value = 25f,   minValue = 10f,  maxValue = 30f },
            ["SPARKS_WIRING"]              = new ConfigItem { Value = 15f,   minValue = 5.0f, maxValue = 30f },
            ["MURDER_WALKING"]             = new ConfigItem { Value = 0.45f, minValue = 0.2f, maxValue = 5f  },
            ["COFFEE_INCREASE"]            = new ConfigItem { Value = 11.3f, minValue = 5.0f, maxValue = 20f },
            ["RALLY_PLAYER"]               = new ConfigItem { Value = 1f }, // ??

            // any
            ["PUB_COFFEE_PRICE"]        = new ConfigItem { Value = 14f, minValue = 5f, maxValue = 40f },

            ["REQUIRED_SPEED_Jonezz"]   = new ConfigItem { Value = 70f,  minValue = 50f, maxValue = 80f  },
            ["REQUIRED_SPEED_Satsuma"]  = new ConfigItem { Value = 120f, minValue = 80f, maxValue = 150f },
            ["REQUIRED_SPEED_Ferndale"] = new ConfigItem { Value = 110f, minValue = 70f, maxValue = 135f },
            ["REQUIRED_SPEED_Hayosiko"] = new ConfigItem { Value = 110f, minValue = 60f, maxValue = 120f },
            ["REQUIRED_SPEED_Fittan"]   = new ConfigItem { Value = 70f,  minValue = 50f, maxValue = 120f },
            ["REQUIRED_SPEED_Gifu"]     = new ConfigItem { Value = 70f,  minValue = 50f, maxValue = 120f },

            ["REQUIRED_WINDSHIELD_SPEED"] = new ConfigItem { Value = 45f, minValue = 30f, maxValue = 50f }
        };

        public static float Value {
            get { return _value; }
            set
            {
                if ((value <= MIN_ADRENALINE || value >= MAX_ADRENALINE) && !isDead)
                {
                    if (ModLoader.IsModPresent("Health") && Health.damage(100f))
                        Health.killCustom(PAPER_TEXT_EN, PAPER_TEXT_FI);
                    else
                        KillCustom(PAPER_TEXT_EN, PAPER_TEXT_FI);
                    
                    isDead = true;
                }
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
                var MIN_LOSS_RATE = AdrenalineLogic.config.GetValueSafe("MIN_LOSS_RATE");
                var MAX_LOSS_RATE = AdrenalineLogic.config.GetValueSafe("MAX_LOSS_RATE");
                _lossRate = Mathf.Clamp(value, MIN_LOSS_RATE.Value, MAX_LOSS_RATE.Value);
            }
        }

        public static void Tick()
        {
            if (ModLoader.IsModPresent("Health"))
            {
                var lossSpeed = config.GetValueSafe("LOSS_RATE_SPEED").Value;
                if (Health.hp < 30)
                    LossRate -= lossSpeed * Time.fixedDeltaTime;
                if (Health.hp > 80)
                    LossRate += lossSpeed * Time.fixedDeltaTime;
            }

            if (IsDecreaseLocked() && _debug) return;
            Value -= config.GetValueSafe("DEFAULT_DECREASE").Value * LossRate * Time.fixedDeltaTime; // basic decrease adrenaline
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
            var death = GameObject.Find("Systems/Death");
            var paper = death.transform.Find("GameOverScreen/Paper/Fatigue");
            death.SetActive(true);
            paper.Find("TextEN").GetComponent<TextMesh>().text = en;
            paper.Find("TextFI").GetComponent<TextMesh>().text = fi;
        }
    }
}
