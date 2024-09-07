using System.Linq;
using System.Collections.Generic;

using Harmony;
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class PillsItem
    {
        public GameObject self;
        private PlayMakerFSM fsm;
        private Vector3 position;

        public PillsItem()
        {
            Transform player = GameObject.Find("PLAYER").transform;
            position = player.position;
            CreatePillsItem();
        }

        public PillsItem(Vector3 position)
        {
            this.position = position;
            CreatePillsItem();
        }

        private void CreatePillsItem()
        {
            var chips = Resources.FindObjectsOfTypeAll<Transform>().First(v => v.gameObject.name == "potato chips" && v.IsPrefab());

            self = Object.Instantiate(chips.gameObject);
            self.name = "pills(itemx)";
            self.transform.position = position;

            var ren = self.AddComponent<ItemRenamer>();
            ren.TargetName = "potato chips(itemx)";
            ren.FinalName = "pills(itemx)";

            var mesh = AdrenalineLogic.pills.GetComponent<MeshFilter>().mesh;
            var texture = AdrenalineLogic.pills.GetComponent<MeshRenderer>().material.mainTexture;
            Utils.ChangeMesh(self, mesh, texture, Vector2.zero, Vector2.one);

            var collider = self.GetComponent<BoxCollider>();
            var pcoll = AdrenalineLogic.pills.GetComponent<BoxCollider>();
            collider.contactOffset = pcoll.contactOffset;
            collider.center = pcoll.center;
            collider.name = pcoll.name;
            collider.size = pcoll.size;

            fsm = self.GetPlayMaker("Use");
            var state_eat = PlayMakerExtensions.GetState(fsm, "Eat");
            List<FsmStateAction> list = new List<FsmStateAction>(state_eat.Actions);
            list.RemoveRange(6, 3);
            state_eat.Actions = list.ToArray();

            GameHook.InjectStateHook(self, "Use", "Eat", EatState);

            var state_destroy = PlayMakerExtensions.GetState(fsm, "Destroy");
            var dlist = new List<FsmStateAction>(state_destroy.Actions);
            dlist.Clear();
            state_destroy.Actions = dlist.ToArray();
            GameHook.InjectStateHook(self, "Use", "Destroy", DestroyState, true);

            self.SetActive(true);
        }

        private void EatState()
        {
            AdrenalineLogic.Value += AdrenalineLogic.config.GetValueSafe("PILLS_DECREASE");
        }

        private void DestroyState()
        {
            Object.Destroy(self);
        }
    }
}
