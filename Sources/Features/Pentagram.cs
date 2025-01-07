
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;
using Psycho.Handlers;

using Random = UnityEngine.Random;


namespace Psycho.Features
{
    internal sealed class Pentagram : CatchedComponent
    {
        List<PentaTrigger> triggers = new List<PentaTrigger>();

        Transform candles;
        Transform spawnerPos;
        PlayMakerFSM hand;

        public bool LightsEnabled { get; private set; } = false;

        public static Dictionary<string, string[]> InnerEvents = new Dictionary<string, string[]>
        {
            ["Very good"] = new string[]
            { "money", "fuel", "beercase", "battery" },

            ["Good"] = new string[]
            { "fusesbox", "sparksbox", "coolant", "motoroil", "brakefluid", "coffee" },

            ["Normal"] = new string[]
            { "booze", "sausages", "lightbulb", "macaronbox", "chips", "sugar", "cigarettes" },

            ["Bad"] = new string[]
            { "spirit", "spoil", "hangover", "fatigue", "hunger", "blowfuses" },

            ["Very bad"] = new string[]
            { "knockout", "bursttires", "outoffuel", "blindless" }
        };
        
        Dictionary<string, float> events = new Dictionary<string, float>
        {
            { "Very good", 0.2f },
            { "Good", 0.5f },
            { "Normal", 1f },
            { "Bad", 0.5f },
            { "Very bad", 0.2f }
        };


        protected override void Awaked()
        {
            candles = transform.Find("Candles");
            spawnerPos = transform.Find("ItemsSpawner");

            Transform _triggers = transform.Find("Triggers");
            for (int i = 0; i < _triggers.childCount; i++)
                triggers.Add(_triggers.GetChild(i).gameObject.AddComponent<PentaTrigger>());

            GameObject _fireParticle = GameObject.Find("ITEMS/lantern(itemx)/light/particle");
            for (int i = 0; i < candles.childCount; i++)
            {
                Transform _candleFire = candles.GetChild(i).GetChild(0);
                GameObject _clonedFireParticle = Instantiate(_fireParticle);
                Destroy(_clonedFireParticle.GetComponent<PlayMakerFSM>());

                _clonedFireParticle.transform.SetParent(_candleFire, worldPositionStays: false);
                _clonedFireParticle.transform.localPosition = new Vector3(-0.0003f, -0.0003f, 0.0242f);
                _clonedFireParticle.transform.localEulerAngles = new Vector3(90, 0, 0);
            }

            hand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
        }

        protected override void OnFixedUpdate()
        {
            if (Logic.GameFinished)
            {
                Destroy(gameObject);
                return;
            }

            if (!LightsEnabled && Psycho.SUN_hours.Value >= 20 || Psycho.SUN_hours.Value < 5)
                SetCandlesFireActive(true);
            else if (LightsEnabled && Psycho.SUN_hours.Value > 4 && Psycho.SUN_hours.Value < 20)
                SetCandlesFireActive(false);
        }

        public void SetCandlesFireActive(bool state, bool notSet = false)
        {
            if (!LightsEnabled && Vector3.Distance(candles.position, Psycho.Player.position) > 10f) return;

            for (int i = 0; i < candles.childCount; i++)
                candles.GetChild(i).GetChild(0 /* Fire */).gameObject.SetActive(state);
            
            if (notSet) return;

            LightsEnabled = state;
        }

        public bool CheckItems()
            => triggers.All(v =>
                v.IsItemIn
                && v.Item != null
                && Globals.PentaRecipe.Contains(v.Item.name.Replace("(Clone)", "").ToLower())
                && triggers.Select(n => n.Item?.name).Distinct().ToList().Count == 5
            );

        public bool TryTriggerEvent()
        {
            if (!CheckItems()) return false;
            _spawnRandomEvent();
            return true;
        }

        public void MakeItemsUnPickable()
        {
            hand?.CallGlobalTransition("DROP_PART");

            foreach (PentaTrigger _trigger in triggers)
            {
                if (!_trigger.IsItemIn) continue;
                if (_trigger.Item == null) continue;

                _trigger.Item.layer = 0;
            }
        }

        public void DestroyItems()
        {
            foreach (PentaTrigger _trigger in triggers)
            {
                ItemsPool.RemoveItem(_trigger.Item);
                Destroy(_trigger.Item);
                _trigger.Item = null;
                _trigger.IsItemIn = false;
            }
        }


        void _spawnRandomEvent()
        {
            Utils.PrintDebug(eConsoleColors.GREEN, "All ritual items in trigger.");
            string _mainEvent = events.RandomElementByWeight(_weightSelector).Key;
            string[] _innerEvents = InnerEvents[_mainEvent];
            string _innerEvent = _innerEvents[Random.Range(0, _innerEvents.Length)];

            PentagramEvents.TriggerEvent(_innerEvent);
        }

        static float _weightSelector(KeyValuePair<string, float> t) => t.Value;
    }
}
