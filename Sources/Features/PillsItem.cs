
using System;
using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Handlers;
using Psycho.Internal;

using Object = UnityEngine.Object;


namespace Psycho.Features
{
    public sealed class PillsItem
    {
        PlayMakerFSM fsm;

        public GameObject self;
        public int index;
        

        public PillsItem()
            => TryCreatePills(Vector3.zero, Vector3.zero);

        public PillsItem(Vector3 position)
            => TryCreatePills(position, Vector3.zero);

        public PillsItem(Vector3 position, Vector3 euler)
            => TryCreatePills(position, euler);
        
        ~PillsItem()
        {
            if (self == null) return;
            Object.Destroy(self);
        }
        
        void TryCreatePills(Vector3 position, Vector3 euler)
        {
            try
            {
                CreatePillsItem(position, euler);
                Utils.PrintDebug(eConsoleColors.GREEN, $"PillsItem created with position: {position}");
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to create PillsItem\n{e.GetFullMessage()}");
            }
        }

        void CreatePillsItem(Vector3 position, Vector3 euler)
        {
            Transform chips = Resources.FindObjectsOfTypeAll<Transform>()
                .First(v => v.gameObject.name == "potato chips" && v.IsPrefab());

            self = (GameObject)Object.Instantiate(chips.gameObject, position, Quaternion.Euler(euler));
            self.name = "pills(itemx)";
            //self.transform.position = position;
            //self.transform.eulerAngles = euler;
            self.transform.SetParent(GameObject.Find("ITEMS").transform);

            ItemRenamer renamer = self.AddComponent<ItemRenamer>();
            renamer.TargetName = "potato chips(itemx)";
            renamer.FinalName = "pills(itemx)";

            GameObject pillsPrefab = Globals.Pills_prefab;
            WorldManager.ChangeModel(self,
                pillsPrefab.GetComponent<MeshFilter>().mesh,
                pillsPrefab.GetComponent<MeshRenderer>().material.mainTexture
            );

            BoxCollider collider = self.GetComponent<BoxCollider>();
            BoxCollider pcoll = Globals.Pills_prefab.GetComponent<BoxCollider>();
            collider.contactOffset = pcoll.contactOffset;
            collider.center = pcoll.center;
            collider.name = pcoll.name;
            collider.size = pcoll.size;

            fsm = self.GetPlayMaker("Use");
            fsm.GetState("Eat").ClearActions(6, 3);
            fsm.GetState("Load").ClearActions();
            fsm.GetState("Save").ClearActions();
            fsm.GetState("Destroy").ClearActions();

            StateHook.Inject(self, "Use", "Eat", EatState);
            StateHook.Inject(self, "Use", "Destroy", () => Object.Destroy(self), -1);
        }

        void EatState()
        {
            Logic.ChangeWorld(eWorldType.MAIN);

            Globals.Pills = null;
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Pills with index {index} removed");
        }

        internal void WriteData(ref byte[] array, int offset)
        {
            BitConverter.GetBytes(index).CopyTo(array, offset);
            offset += 4;

            self.transform.position.CopyBytes(ref array, ref offset);
            self.transform.eulerAngles.CopyBytes(ref array, ref offset);
        }

        internal static PillsItem ReadData(ref byte[] array, int offset)
        {
            int index = BitConverter.ToInt32(array, offset);
            offset += 4;

            Vector3 position = Vector3.zero.GetFromBytes(array, ref offset);
            Vector3 euler = Vector3.zero.GetFromBytes(array, ref offset);
            
            PillsItem item = new PillsItem(position, euler);
            item.self.SetActive(Logic.InHorror);
            return item;
        }
    }
}
