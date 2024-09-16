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
        private Vector3 position = Vector3.zero;
        private Vector3 rotation = Vector3.zero;

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

        internal PillsItem(int index, Vector3 position, Vector3 euler)
        {
            this.index = index;
            this.position = position;
            this.rotation = euler;
            try
            {
                CreatePillsItem();
                Utils.PrintDebug(eConsoleColors.GREEN, $"PillsItem created with idx: {index} & position: {position} & rotation: {rotation}");
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
            self.transform.eulerAngles = rotation;
            self.transform.SetParent(GameObject.Find("ITEMS").transform);

            var ren = self.AddComponent<ItemRenamer>();
            ren.TargetName = "potato chips(itemx)";
            ren.FinalName = "pills(itemx)";

            var pills = Globals.pills;
            Utils.ChangeModel(self,
                pills.GetComponent<MeshFilter>().mesh,
                pills.GetComponent<MeshRenderer>().material.mainTexture,
                Vector2.zero, Vector2.one);

            var collider = self.GetComponent<BoxCollider>();
            var pcoll = Globals.pills.GetComponent<BoxCollider>();
            collider.contactOffset = pcoll.contactOffset;
            collider.center = pcoll.center;
            collider.name = pcoll.name;
            collider.size = pcoll.size;

            fsm = self.GetPlayMaker("Use");
            Utils.ClearActions(fsm.GetState("Eat"), 6, 3);
            Utils.ClearActions(fsm.GetState("Load"));
            Utils.ClearActions(fsm.GetState("Save"));
            Utils.ClearActions(fsm.GetState("Destroy"));

            StateHook.Inject(self, "Use", "Eat", () => EatState());
            StateHook.Inject(self, "Use", "Destroy", -1, () => DestroyState());
        }

        private void EatState()
        {
            AdrenalineLogic.UpdateLossRatePerDay();
            AdrenalineLogic.SetDecreaseLocked(true, 12000f);

            var index = Globals.pills_list.FindIndex(v => v.self == self);
            if (index == -1) return;
            
            Globals.pills_list.RemoveAt(index);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Pills removed from list with index {index}");
        }

        private void DestroyState()
        {
            Object.Destroy(self);
        }
    }
}
