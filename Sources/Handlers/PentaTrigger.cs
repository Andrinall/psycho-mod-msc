using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;
using Psycho.Features;
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

            Hand.CallGlobalTransition("DROP_PART");

            IsItemIn = true;
            Item = other.gameObject;
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
