using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Handlers;
using Psycho.Extensions;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


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

        public Dictionary<string, string[]> InnerEvents = new Dictionary<string, string[]>
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
        
        string[] recipe = new string[5] { "candle", "fernflower", "mushroom", "blackegg", "walnut" };

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
                Triggers.Add(_triggers.GetChild(i).gameObject.AddComponent<PentaTrigger>());

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

            if (!CheckItems()) return;
            SpawnRandomEvent();
        }


        public void SetCandlesFireActive(bool state)
        {
            if (!LightsEnabled && Vector3.Distance(Candles.position, Player.position) > 10f) return;

            for (int i = 0; i < Candles.childCount; i++)
                Candles.GetChild(i).GetChild(0 /* Fire */).gameObject.SetActive(state);
            LightsEnabled = state;
        }

        public bool GetCandlesFireActive() => LightsEnabled;

        public bool CheckItems()
        {
            return Triggers.All(v =>
                v.IsItemIn
                && v.Item != null
                && recipe.Contains(v.Item.name.Replace("(Clone)", "").ToLower())
                && !Triggers.Any(r => r.Item == v.Item)
            );
        }

        void SpawnRandomEvent()
        {
            Utils.PrintDebug("all penta items in trigger");
            string _mainEvent = Events.RandomElementByWeight(_weightSelector).Key;
            string[] _innerEvents = InnerEvents[_mainEvent];
            string _innerEvent = _innerEvents[Random.Range(0, _innerEvents.Length)];
            
            GetComponent<PentagramEvents>().Activate(_innerEvent);
        }

        public void DestroyItems()
        {
            Triggers.ForEach(v => {
                Destroy(v.Item);
                v.Item = null;
                v.IsItemIn = false;
            });
        }

        static float _weightSelector(KeyValuePair<string, float> t) => t.Value;
    }
}
