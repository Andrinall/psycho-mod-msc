
using System;
using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;

using Object = UnityEngine.Object;


namespace Psycho.Features
{
    class PillsItem
    {
        static PlayMakerFSM fsm;

        public static GameObject Self;
        public static int Index;
        
        public static void TryCreatePills(int index, Vector3 position, Vector3 euler)
        {
            if (Self != null) return;

            try
            {
                CreatePillsItem(position, euler);
                Index = index;
                Utils.PrintDebug(eConsoleColors.GREEN, $"PillsItem created with position: {position}");
            }
            catch (Exception e)
            {
                Index = -1;
                Self = null;
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to create PillsItem\n{e.GetFullMessage()}");
            }
        }

        static void CreatePillsItem(Vector3 position, Vector3 euler)
        {
            Transform _chips = Resources.FindObjectsOfTypeAll<Transform>()
                .First(v => v.gameObject.name == "potato chips" && v.IsPrefab());

            Self = (GameObject)Object.Instantiate(_chips.gameObject, position, Quaternion.Euler(euler));
            Self.name = "pills(itemx)";
            Self.transform.SetParent(GameObject.Find("ITEMS").transform);

            ItemRenamer _renamer = Self.AddComponent<ItemRenamer>();
            _renamer.TargetName = "potato chips(itemx)";
            _renamer.FinalName = "pills(itemx)";

            GameObject _pillsPrefab = ResourcesStorage.Pills_prefab;
            WorldManager.ChangeModel(Self,
                _pillsPrefab.GetComponent<MeshFilter>().mesh,
                _pillsPrefab.GetComponent<MeshRenderer>().material.mainTexture
            );

            BoxCollider _collider = Self.GetComponent<BoxCollider>();
            BoxCollider _pcoll = ResourcesStorage.Pills_prefab.GetComponent<BoxCollider>();
            _collider.contactOffset = _pcoll.contactOffset;
            _collider.center = _pcoll.center;
            _collider.name = _pcoll.name;
            _collider.size = _pcoll.size;

            fsm = Self.GetPlayMaker("Use");
            fsm.GetState("Eat").ClearActions(6, 3);
            fsm.GetState("Load").ClearActions();
            fsm.GetState("Save").ClearActions();
            fsm.GetState("Destroy").ClearActions();

            StateHook.Inject(Self, "Use", "Eat", EatState);
            StateHook.Inject(Self, "Use", "Destroy", () => Object.Destroy(Self), -1);
        }

        static void EatState()
        {
            Logic.ChangeWorld(eWorldType.MAIN);
            
            Object.Destroy(Self);
            Index = -1;
            Self = null;

            Utils.PrintDebug($"Pills with index {Index} removed");
        }
    }
}
