using HealthMod;
using UnityEngine;

namespace Adrenaline
{
    internal static class AdrenalineLogic
    {
        private static readonly string PAPER_TEXT_FI = "Mies kuoli\nsydänkohtaukseen";
        private static readonly string PAPER_TEXT_EN = "Man found\ndead of\nheart attack\nin region of\nAlivieska";

        public static readonly float MIN_ADRENALINE = 0f;
        public static readonly float MAX_ADRENALINE = 200f;

        private static FixedHUD _hud;
        private static bool _debug = false;
        private static float _value = 100f;
        private static float _lossRate = 1.2f;
        private static bool  _lockDecrease = false;
        private static float _lockCooldown = 0f; // 12000 == 1 minute

        public static Configuration config = new Configuration();
        public static Texture texture = null;
        public static Mesh empty_cup = null;
        public static Mesh coffee_cup = null;

        public static float Value {
            get
            { return _value; }
            set
            {
                if ((value <= MIN_ADRENALINE || value >= MAX_ADRENALINE) && Health.damage(100f))
                    Health.killCustom(PAPER_TEXT_EN, PAPER_TEXT_FI);
                else if (_hud.IsElementExist("Adrenaline"))
                {
                    var clamped = Mathf.Clamp(value / 100f, 0f, 2f);

                    _hud.SetElementScale("Adrenaline", new Vector3(Mathf.Clamp(value / 100f, 0f, 1f), 1f));
                    _hud.SetElementColor("Adrenaline", (clamped <= 0.15f || clamped >= 1.75f) ? Color.red : Color.white);
                }
                _value = value;
            }
        }

        public static float LossRate
        {
            get { return _lossRate; }
            set {
                if (IsDecreaseLocked()) return;
                _lossRate = Mathf.Clamp(value, config.MIN_LOSS_RATE, config.MAX_LOSS_RATE);
            }
        }

        public static void InitHUD()
        {
            _hud = GameObject.Find("GUI/HUD").AddComponent<FixedHUD>();
            _hud.AddElement(eHUDCloneType.RECT, "Adrenaline", _hud.GetIndexByName("Money"));
            Utils.PrintDebug("HUD Enabled");
        }

        public static void DestroyHUD()
        {
            Object.Destroy(_hud);
            Utils.PrintDebug("HUD Destroyed");
        }

        public static void Tick()
        {
            if (Health.hp < 30)
                LossRate -= 0.01f * Time.fixedDeltaTime;
            if (Health.hp > 80)
                LossRate += 0.01f * Time.fixedDeltaTime;

            if (IsDecreaseLocked() && _debug) return;
            Value -= config.DEFAULT_DECREASE * LossRate * Time.fixedDeltaTime; // basic decrease adrenaline
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
    }
}
