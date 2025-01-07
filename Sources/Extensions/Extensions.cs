
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho
{
    public static class Extensions
    {
        public static void SetupFSM(this GameObject obj, string name, string[] eventNames, string startState, Func<PlayMakerFSM, List<FsmEvent>, FsmState[]> callback)
        {
            List<FsmEvent> _events = new List<FsmEvent>();
            PlayMakerFSM _fsm = obj.AddComponent<PlayMakerFSM>();

            _fsm.InitializeFSM();
            _fsm.enabled = false;
            _fsm.Fsm.Name = name;

            foreach (string _ev in eventNames)
                _events.Add(_fsm.AddEvent(_ev));

            FsmState[] finalStates = callback?.Invoke(_fsm, _events);
            if (finalStates == null || finalStates.Length == 0)
            {
                UnityEngine.Object.Destroy(_fsm);
                return;
            }

            _fsm.Fsm.States = (FsmState[])finalStates.Clone();
            _fsm.Fsm.StartState = startState;
            _fsm.Fsm.Start();
            _fsm.enabled = true;
        }

        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float _totalWeight = sequence.Sum(weightSelector);
            float _itemWeightIndex = (float)new System.Random().NextDouble() * _totalWeight;
            float _currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                _currentWeightIndex += item.Weight;

                if (_currentWeightIndex >= _itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }
    }
}
