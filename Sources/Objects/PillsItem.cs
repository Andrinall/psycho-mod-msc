using System;
using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;
using Psycho.Extensions;

namespace Psycho.Objects
{
    public sealed class PillsItem
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

            ItemRenamer renamer = self.AddComponent<ItemRenamer>();
            renamer.TargetName = "potato chips(itemx)";
            renamer.FinalName = "pills(itemx)";

            GameObject pillsPrefab = Globals.Pills_prefab;
            WorldManager.ChangeModel(self,
                pillsPrefab.GetComponent<MeshFilter>().mesh,
                pillsPrefab.GetComponent<MeshRenderer>().material.mainTexture
            );

            var collider = self.GetComponent<BoxCollider>();
            var pcoll = Globals.Pills_prefab.GetComponent<BoxCollider>();
            collider.contactOffset = pcoll.contactOffset;
            collider.center = pcoll.center;
            collider.name = pcoll.name;
            collider.size = pcoll.size;

            fsm = self.GetPlayMaker("Use");
            fsm.GetState("Eat").ClearActions(6, 3);
            fsm.GetState("Load").ClearActions();
            fsm.GetState("Save").ClearActions();
            fsm.GetState("Destroy").ClearActions();

            StateHook.Inject(self, "Use", "Eat", _ => EatState());
            StateHook.Inject(self, "Use", "Destroy", -1, _ => UnityEngine.Object.Destroy(self));
        }

        void EatState()
        {
            Logic.ChangeWorld(eWorldType.MAIN);

            int index = Globals.pills_list.FindIndex(v => v.index == this.index);
            if (index == -1) return;
            
            Globals.pills_list.RemoveAt(index);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Pills removed from list with index {index}");
        }

        internal void WriteData(ref byte[] array, int offset)
        {
            Vector3 pos = self.transform.position;
            BitConverter.GetBytes(pos.x).CopyTo(array, offset);
            BitConverter.GetBytes(pos.y).CopyTo(array, offset + 4);
            BitConverter.GetBytes(pos.z).CopyTo(array, offset + 8);

            Vector3 rot = self.transform.eulerAngles;
            BitConverter.GetBytes(rot.x).CopyTo(array, offset + 12);
            BitConverter.GetBytes(rot.y).CopyTo(array, offset + 16);
            BitConverter.GetBytes(rot.z).CopyTo(array, offset + 20);
        }

        internal void ReadData(ref byte[] array, int offset)
        {
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
