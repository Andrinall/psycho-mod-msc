using UnityEngine;

using Psycho.Internal;
using System.Linq;
using Psycho.Features;
using MSCLoader;
using Psycho.Extensions;


namespace Psycho.Handlers
{
    internal sealed class PentaTrigger : MonoBehaviour
    {
        Pentagram penta;
        PlayMakerFSM Hand;

        internal GameObject Item = null;
        internal bool IsItemIn = false;

        void Awake()
        {
            penta = transform.parent.parent.GetComponent<Pentagram>();
            Hand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsItemIn) return;
            if (other.gameObject == null) return;
            string itemname = other.gameObject.name.Replace("(Clone)", "").ToLower();
            if (!penta.recipe.Contains(itemname)) return;

            IsItemIn = true;
            Item = other.gameObject;
            Hand.CallGlobalTransition("DROP_PART");
            Item.transform.position = transform.position;

            penta.TryTriggerEvent();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Item {other.gameObject.name} enter trigger {name};");
        }

        void OnTriggerExit(Collider other)
        {
            if (!IsItemIn) return;
            if (other.gameObject != Item) return;

            IsItemIn = false;
            Item = null;
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Item {other.gameObject.name} exit trigger {name};");
        }
    }
}
