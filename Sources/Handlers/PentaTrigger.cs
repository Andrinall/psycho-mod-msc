using UnityEngine;

using Psycho.Internal;
using System.Linq;


namespace Psycho.Handlers
{
    internal sealed class PentaTrigger : MonoBehaviour
    {
        internal GameObject Item = null;
        internal bool IsItemIn = false;
        
        string[] recipe = new string[5] { "candle", "fernflower", "mushroom", "egg", "nut" };

        void OnTriggerEnter(Collider other)
        {
            if (IsItemIn) return;
            if (other.gameObject == null) return;
            string itemname = other.gameObject.name.Replace("(Clone)", "").ToLower();
            if (!recipe.Contains(itemname)) return;

            IsItemIn = true;
            Item = other.gameObject;
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Item {other.gameObject.name} enter penta trigger {name}; IsItemIn setted to true");
        }

        void OnTriggerExit(Collider other)
        {
            if (!IsItemIn) return;
            if (other.gameObject != Item) return;

            IsItemIn = false;
            Item = null;
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Item {other.gameObject.name} exit penta trigger {name}; IsItemIn setted to false");
        }
    }
}
