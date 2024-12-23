using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Handlers;
using Random = UnityEngine.Random;


namespace Psycho.Features
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
        
        FsmFloat SUN_hours;
        FsmFloat SUN_minutes;
        PlayMakerFSM Hand;
        public bool LightsEnabled { get; private set; } = false;


        protected override void Awaked()
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
                GameObject _clonedFireParticle = Instantiate(_fireParticle);
                Destroy(_clonedFireParticle.GetComponent<PlayMakerFSM>());

                _clonedFireParticle.transform.SetParent(_candleFire, worldPositionStays: false);
                _clonedFireParticle.transform.localPosition = new Vector3(-0.0003f, -0.0003f, 0.0242f);
                _clonedFireParticle.transform.localEulerAngles = new Vector3(90, 0, 0);
            }

            PlayMakerFSM sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock");
            SUN_hours = sun.GetVariable<FsmFloat>("Hours");
            SUN_minutes = sun.GetVariable<FsmFloat>("Minutes");
            Hand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
        }

        protected override void OnFixedUpdate()
        {
            if (!LightsEnabled && SUN_hours.Value >= 20 || SUN_hours.Value < 5)
                SetCandlesFireActive(true);
            else if (LightsEnabled && SUN_hours.Value > 4 && SUN_hours.Value < 20)
                SetCandlesFireActive(false);
        }

        public void SetCandlesFireActive(bool state, bool notSet = false)
        {
            if (!LightsEnabled && Vector3.Distance(Candles.position, Player.position) > 10f) return;

            for (int i = 0; i < Candles.childCount; i++)
                Candles.GetChild(i).GetChild(0 /* Fire */).gameObject.SetActive(state);
            
            if (notSet) return;

            LightsEnabled = state;
        }

        public bool CheckItems()
            => Triggers.All(v =>
                v.IsItemIn
                && v.Item != null
                && Globals.PentaRecipe.Contains(v.Item.name.Replace("(Clone)", "").ToLower())
                && Triggers.Select(n => n.Item?.name).Distinct().ToList().Count == 5
            );

        public bool TryTriggerEvent()
        {
            if (!CheckItems()) return false;
            SpawnRandomEvent();
            return true;
        }

        void SpawnRandomEvent()
        {
            Utils.PrintDebug(eConsoleColors.GREEN, "All ritual items in trigger.");
            string _mainEvent = Events.RandomElementByWeight(_weightSelector).Key;
            string[] _innerEvents = InnerEvents[_mainEvent];
            string _innerEvent = _innerEvents[Random.Range(0, _innerEvents.Length)];
            
            PentagramEvents.TriggerEvent(_innerEvent);
        }

        public void MakeItemsUnPickable()
        {
            Hand?.CallGlobalTransition("DROP_PART");

            foreach (PentaTrigger trigger in Triggers)
            {
                if (!trigger.IsItemIn) continue;
                if (trigger.Item == null) continue;

                trigger.Item.layer = 0;
            }
        }

        public void DestroyItems()
        {
            foreach (PentaTrigger trigger in Triggers)
            {
                ItemsPool.RemoveItem(trigger.Item);
                Destroy(trigger.Item);
                trigger.Item = null;
                trigger.IsItemIn = false;
            }
        }

        static float _weightSelector(KeyValuePair<string, float> t) => t.Value;
    }
}
