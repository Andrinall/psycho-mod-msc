﻿
using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;
using Psycho.Features;


namespace Psycho.Handlers
{
    class PentaTrigger : CatchedComponent
    {
        Pentagram penta;
        PlayMakerFSM Hand;

        public GameObject Item = null;
        public bool IsItemIn = false;

        Vector3 Position => transform.position;


        protected override void Awaked()
        {
            penta = transform.parent.parent.GetComponent<Pentagram>();
            Hand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsItemIn) return;
            if (other.gameObject == null) return;

            string _itemname = other.gameObject.name.Replace("(Clone)", "").ToLower();
            if (!Globals.PentaRecipe.Contains(_itemname)) return;

            Hand.CallGlobalTransition("DROP_PART");

            IsItemIn = true;
            Item = other.gameObject;

            Item.transform.localEulerAngles = new Vector3(0, 90f, 0);
            Item.transform.position = new Vector3(Position.x, Position.y, Position.z + 0.001f);

            Item.AddComponent<ItemsGravityCrutch>();

            penta.TryTriggerEvent();
            Utils.PrintDebug($"Item {other.gameObject.name} enter trigger {name};");
        }

        void OnTriggerExit(Collider other)
        {
            if (!IsItemIn) return;
            if (other.gameObject != Item) return;

            IsItemIn = false;
            Item = null;
            Utils.PrintDebug($"Item {other.gameObject.name} exit trigger {name};");
        }
    }
}
