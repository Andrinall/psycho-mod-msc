using System;
using System.Linq;

using MSCLoader;
using UnityEngine;

namespace Psycho
{
    public class PillsItem
    {
        PlayMakerFSM fsm;

        public GameObject self;
        public int index;
        
        public PillsItem(int index) => this.TryCreatePills(index, Vector3.zero, Vector3.zero);
        public PillsItem(int index, Vector3 position) => this.TryCreatePills(index, position, Vector3.zero);
        public PillsItem(int index, Vector3 position, Vector3 euler) => this.TryCreatePills(index, position, euler);
        
        ~PillsItem()
        {
            if (self == null) return;
            UnityEngine.Object.Destroy(self);
        }
        
        void TryCreatePills(int index, Vector3 position, Vector3 euler)
        {
            try
            {
                CreatePillsItem(position, Vector3.zero);
                Utils.PrintDebug(eConsoleColors.GREEN, $"PillsItem created with idx: {index} & position: {position}");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to create PillsItem\n{e.GetFullMessage()}");
            }
        }

        void CreatePillsItem(Vector3 position, Vector3 euler)
        {
            var chips = Resources.FindObjectsOfTypeAll<Transform>()
                .First(v => v.gameObject.name == "potato chips" && v.IsPrefab());

            self = UnityEngine.Object.Instantiate(chips.gameObject);
            self.name = "pills(itemx)";
            self.transform.position = position;
            self.transform.eulerAngles = euler;
            self.transform.SetParent(GameObject.Find("ITEMS").transform);

            var ren = self.AddComponent<ItemRenamer>();
            ren.TargetName = "potato chips(itemx)";
            ren.FinalName = "pills(itemx)";

            var pills = Globals.Pills_prefab;
            WorldManager.ChangeModel(self,
                pills.GetComponent<MeshFilter>().mesh,
                pills.GetComponent<MeshRenderer>().material.mainTexture
            );

            var collider = self.GetComponent<BoxCollider>();
            var pcoll = Globals.Pills_prefab.GetComponent<BoxCollider>();
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
            StateHook.Inject(self, "Use", "Destroy", -1, () => UnityEngine.Object.Destroy(self));
        }

        void EatState()
        {
            Logic.ChangeWorld(eWorldType.MAIN);

            var index = Globals.pills_list.FindIndex(v => v.index == this.index);
            if (index == -1) return;
            
            Globals.pills_list.RemoveAt(index);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Pills removed from list with index {index}");
        }

        internal void WriteData(ref byte[] array, int offset)
        {
            var pos = self.transform.position;
            BitConverter.GetBytes(pos.x).CopyTo(array, offset);
            BitConverter.GetBytes(pos.y).CopyTo(array, offset + 4);
            BitConverter.GetBytes(pos.z).CopyTo(array, offset + 8);

            var rot = self.transform.eulerAngles;
            BitConverter.GetBytes(rot.x).CopyTo(array, offset + 12);
            BitConverter.GetBytes(rot.y).CopyTo(array, offset + 16);
            BitConverter.GetBytes(rot.z).CopyTo(array, offset + 20);
        }

        internal void ReadData(ref byte[] array, int offset)
        {
            Utils.PrintDebug(eConsoleColors.WHITE, $"Loading pills");

            self.transform.position = new Vector3(
                BitConverter.ToSingle(array, offset),
                BitConverter.ToSingle(array, offset + 4),
                BitConverter.ToSingle(array, offset + 8)
            );

            self.transform.eulerAngles = new Vector3(
                BitConverter.ToSingle(array, offset + 12),
                BitConverter.ToSingle(array, offset + 16),
                BitConverter.ToSingle(array, offset + 20)
            );
        }
    }
}
