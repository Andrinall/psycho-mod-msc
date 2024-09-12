using System.Linq;

using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    internal class PillsItem
    {
        internal GameObject self;
        internal int index;
        private PlayMakerFSM fsm;
        private Vector3 position;

        internal PillsItem(int index, Vector3 position)
        {
            this.position = position;
            this.index = index;
            try
            {
                CreatePillsItem();
                Utils.PrintDebug(eConsoleColors.GREEN, $"PillsItem created with idx: {index} & position: {position}");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to create PillsItem\n{e.GetFullMessage()}");
            }
        }

        ~PillsItem()
        {
            if (self == null) return;
            Object.Destroy(self);
        }

        private void CreatePillsItem()
        {
            var chips = Resources.FindObjectsOfTypeAll<Transform>()
                .First(v => v.gameObject.name == "potato chips" && v.IsPrefab());

            self = Object.Instantiate(chips.gameObject);
            self.name = "pills(itemx)";
            self.transform.position = position;
            self.transform.SetParent(GameObject.Find("ITEMS").transform);

            var ren = self.AddComponent<ItemRenamer>();
            ren.TargetName = "potato chips(itemx)";
            ren.FinalName = "pills(itemx)";

            Utils.ChangeModel(self,
                Globals.pills.GetComponent<MeshFilter>().mesh,
                Globals.pills.GetComponent<MeshRenderer>().material.mainTexture,
                Vector2.zero, Vector2.one);

            var collider = self.GetComponent<BoxCollider>();
            var pcoll = Globals.pills.GetComponent<BoxCollider>();
            collider.contactOffset = pcoll.contactOffset;
            collider.center = pcoll.center;
            collider.name = pcoll.name;
            collider.size = pcoll.size;

            fsm = self.GetPlayMaker("Use");
            var state_eat = PlayMakerExtensions.GetState(fsm, "Eat");
            var list = state_eat.Actions.ToList();
            list.RemoveRange(6, 3);
            state_eat.Actions = list.ToArray();
            StateHook.Inject(self, "Use", "Eat", EatState);

            var state_destroy = PlayMakerExtensions.GetState(fsm, "Destroy");
            var dlist = state_destroy.Actions.ToList();
            dlist.Clear();
            state_destroy.Actions = dlist.ToArray();
            StateHook.Inject(self, "Use", "Destroy", -1,DestroyState);
        }

        private static void EatState()
        {
            AdrenalineLogic.UpdateLossRatePerDay();
        }

        private void DestroyState()
        {
            Object.Destroy(self);
        }
    }
}
