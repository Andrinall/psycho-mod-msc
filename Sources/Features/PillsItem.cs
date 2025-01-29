
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

        public static void Reset()
        {
            Object.Destroy(Self);
            Index = -1;
            Self = null;
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

            PlayMakerFSM _fsm = Self.GetPlayMaker("Use");
            _fsm.GetState("Eat").ClearActions(6, 3);
            _fsm.GetState("Load").ClearActions();
            _fsm.GetState("Save").ClearActions();
            _fsm.GetState("Destroy").ClearActions();

            StateHook.Inject(Self, "Use", "Eat", EatState);
            StateHook.Inject(Self, "Use", "Destroy", () => Object.Destroy(Self), -1);
        }

        static void EatState()
        {
            Logic.ChangeWorld(eWorldType.MAIN);
            Utils.PrintDebug($"Pills with index {Index} removed");
        }
    }
}
