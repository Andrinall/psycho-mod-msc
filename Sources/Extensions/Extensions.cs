
using System;
using System.Linq;
using System.Collections.Generic;


namespace Psycho
{
    public static class Extensions
    {
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float _totalWeight = sequence.Sum(weightSelector);
            float _itemWeightIndex = (float)new Random().NextDouble() * _totalWeight;
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
