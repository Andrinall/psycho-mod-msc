using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Handlers;
using Psycho.Extensions;


namespace Psycho.Objects
{
    internal sealed class Pentagram : CatchedComponent
    {
        List<PentaTrigger> Triggers = new List<PentaTrigger>();

        Transform Player;
        Transform Candles;
        Transform SpawnerPos;


        Dictionary<string, float> Events = new Dictionary<string, float>
        {
            { "Very good", 0.2f },
            { "Good", 0.5f },
            { "Normal", 1f },
            { "Bad", 0.5f },
            { "Very bad", 0.2f }
        };

        Dictionary<string, string[]> InnerEvents = new Dictionary<string, string[]>
        {
            ["Very good"] = new string[]
            { "money", "fuel", "beer", "battery" },
            
            ["Good"] = new string[]
            { "fuses", "sparks", "coolant", "oil", "brakeoil", "coffee" },
            
            ["Normal"] = new string[]
            { "drunk", "sausages", "lamp(satsuma)", "macaronbox", "chips", "sugar", "cigarettes" },

            ["Bad"] = new string[]
            { "spirit", "spoil", "hangover", "fatigue", "hunger", "blowfuses" },

            ["Very bad"] = new string[]
            { "knockout", "bursttire", "outoffuel", "blindless" }
        };
        
        string[] recipe = new string[5]
        { "candle", "flower", "amanita", "egg", "nut" };


        FsmFloat SUN_hours;
        FsmFloat SUN_minutes;
        bool LightsEnabled = false;

        internal override void Awaked()
        {
            Player = GameObject.Find("PLAYER").transform;
            Candles = transform.Find("Candles");
            SpawnerPos = transform.Find("ItemsSpawner");

            Transform _triggers = transform.Find("Triggers");
            for (int i = 0; i < _triggers.childCount; i++)
                _triggers.GetChild(i).GetComponent<PentaTrigger>();

            GameObject _fireParticle = GameObject.Find("ITEMS/lantern(itemx)/light/particle");
            for (int i = 0; i < Candles.childCount; i++)
            {
                Transform _candleFire = Candles.GetChild(i).GetChild(0);
                GameObject _clonedFireParticle = Object.Instantiate(_fireParticle);
                Object.Destroy(_clonedFireParticle.GetComponent<PlayMakerFSM>());

                _clonedFireParticle.transform.SetParent(_candleFire, worldPositionStays: false);
                _clonedFireParticle.transform.localPosition = new Vector3(-0.0003f, -0.0003f, 0.0242f);
                _clonedFireParticle.transform.localEulerAngles = new Vector3(90, 0, 0);
            }

            PlayMakerFSM sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            SUN_hours = sun.GetVariable<FsmFloat>("Hours");
            SUN_minutes = sun.GetVariable<FsmFloat>("Minutes");
        }

        internal override void Enabled()
        {
            
        }

        internal override void Disabled()
        {
            
        }

        internal override void OnFixedUpdate()
        {
            if (!LightsEnabled && SUN_hours.Value > 18 || SUN_hours.Value < 5)
                SetCandlesFireActive(true);
            else if (LightsEnabled && SUN_hours.Value > 4 && SUN_hours.Value < 19)
                SetCandlesFireActive(false);
        }



        public void SetCandlesFireActive(bool state)
        {
            if (!LightsEnabled && Vector3.Distance(Candles.position, Player.position) > 10f) return;

            for (int i = 0; i < Candles.childCount; i++)
                Candles.GetChild(i).GetChild(0 /* Fire */).gameObject.SetActive(state);
            LightsEnabled = state;
        }

        public bool GetCandlesFireActive() => LightsEnabled;

        bool _checkItems()
        {
            for (int i = 0; i < Triggers.Count; i++)
            {
                PentaTrigger trigger = Triggers[i];
                
                if (!trigger.IsItemIn) return false;
                if (!trigger.Item.name.Contains(recipe[i])) return false;
            }

            return true;
        }

        string _randomEvent()
            => Events.RandomElementByWeight(_weightSelector).Key;

        string _randomInnerEvent()
        {
            string _event = _randomEvent();
            string[] _inner = InnerEvents[_event];
            return _inner[Random.Range(0, _inner.Length)];
        }

        float _weightSelector(KeyValuePair<string, float> t) => t.Value;
    }
}
