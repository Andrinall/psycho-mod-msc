
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Features;
using Psycho.Handlers;

using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    internal static class ItemsPool
    {
        static List<GameObject> Pool = new List<GameObject>();
        static List<GameObject> ItemsForSave => Pool.Where(v => v != null && v.transform.parent == null).ToList();

        public static readonly int base_offset = 64;
        internal static int Length => ItemsForSave.Count;

        internal static GameObject AddItem(GameObject prefab)
            => _addItemToLocalPool(prefab, Vector3.zero, Vector3.zero);

        internal static GameObject AddItem(GameObject prefab, Vector3 pos, Vector3 euler)
            => _addItemToLocalPool(prefab, pos, euler);

        internal static GameObject GetItem(int index)
            => Pool.ElementAtOrDefault(index);

        internal static bool RemoveItem(GameObject obj)
            => Pool.Remove(obj);

        internal static bool RemoveItem(Func<GameObject, bool> callback)
        {
            GameObject _item = Pool.FirstOrDefault(callback);
            
            Object.Destroy(_item);
            return Pool.Remove(_item);
        }

        internal static void RemoveItems(Func<GameObject, bool> callback)
        {
            var _items = Pool.Where(callback).ToList();
            foreach (GameObject _item in _items)
            {
                Object.Destroy(_item);
                Pool.Remove(_item);
            }
        }

        internal static List<ItemsPoolSaveLoadData> GetSaveData()
        {
            var _list = new List<ItemsPoolSaveLoadData>();

            foreach (var _item in Pool)
            {
                if (_item == null) continue;
                if (_item.transform.parent != null) continue;

                _list.Add(new ItemsPoolSaveLoadData
                {
                    Name = _item.name.Replace("(Clone)", ""),
                    Position = _item.transform.position,
                    Euler = _item.transform.eulerAngles
                });
            }

            return _list;
        }

        internal static void LoadData(List<ItemsPoolSaveLoadData> data)
        {
            foreach (var _item in data)
            {
                GameObject _prefab = GetPrefabByItemName(_item.Name);
                if (_prefab == null) continue;

                _addItemToLocalPool(_prefab, _item.Position, _item.Euler);
            }
        }

        private static GameObject _addItemToLocalPool(GameObject prefab, Vector3 pos, Vector3 euler)
        {
            GameObject _cloned = (GameObject)Object.Instantiate(prefab, pos, Quaternion.Euler(euler));
            if (prefab.name == "Notebook")
                Globals.Notebook = _cloned.AddComponent<Notebook>();
            if (prefab.name == "Postcard")
            {
                _cloned.AddComponent<ItemsGravityEnabler>();
                Utils.InitPostcard(_cloned);
            }

            _cloned.MakePickable();

            Pool.Add(_cloned);
            return _cloned;
        }

        private static GameObject GetPrefabByItemName(string item)
        {
            switch (item)
            {
                case "ChurchCandle":
                    return Globals.Candle_prefab;
                case "FernFlower":
                    return Globals.FernFlower_prefab;
                case "Mushroom":
                    return Globals.Mushroom_prefab;
                case "Walnut":
                    return Globals.Walnut_prefab;
                case "BlackEgg":
                    return Globals.BlackEgg_prefab;
                case "Picture":
                    return Globals.Picture_prefab;
                case "Notebook":
                    return Globals.Notebook_prefab;
                case "Postcard":
                    return Globals.Postcard_prefab;
                default: return null;
            }
        }
    }
}
