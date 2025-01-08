
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Features;

using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    internal static class ItemsPool
    {
        static List<GameObject> Pool = new List<GameObject>();
        static List<GameObject> ItemsForSave => Pool.Where(v => v != null && v.transform.parent == null).ToList();
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
                Postcard.Initialize(_cloned);

            _cloned.MakePickable();

            Pool.Add(_cloned);
            return _cloned;
        }

        private static GameObject GetPrefabByItemName(string item)
        {
            switch (item)
            {
                case "ChurchCandle":
                    return ResourcesStorage.Candle_prefab;
                case "FernFlower":
                    return ResourcesStorage.FernFlower_prefab;
                case "Mushroom":
                    return ResourcesStorage.Mushroom_prefab;
                case "Walnut":
                    return ResourcesStorage.Walnut_prefab;
                case "BlackEgg":
                    return ResourcesStorage.BlackEgg_prefab;
                case "Picture":
                    return ResourcesStorage.Picture_prefab;
                case "Notebook":
                    return ResourcesStorage.Notebook_prefab;
                case "Postcard":
                    return ResourcesStorage.Postcard_prefab;
                default: return null;
            }
        }


        /// ===================================
        ///       BACKWARD COMPABILITY
        /// ===================================

        public static int base_offset { get; } = 64;

        public static void Load(byte[] array)
        {
            int offset = base_offset;
            int count = BitConverter.ToInt32(array, offset);
            offset += 4;

            if (count == 0)
            {
                Utils.PrintDebug(eConsoleColors.RED, "[ItemsPool.Load] Is empty; Skip stopped.");
                return;
            }
            if (count > 1000)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, $"[ItemPool.Load] Pool has too high length ({count})! Skip loading.");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                string sName = "".GetFromBytes(array, ref offset); // 1
                if (Logic.IsDead && Globals.PentaRecipe.Contains(sName.ToLower())) continue;

                Vector3 temp = new Vector3();
                Vector3 pos = temp.GetFromBytes(array, ref offset);
                Vector3 rot = temp.GetFromBytes(array, ref offset);

                GameObject prefab = GetPrefabByItemName(sName);
                if (prefab == null)
                {
                    Utils.PrintDebug(eConsoleColors.RED, $"[ItemsPool.Load] Loaded item {sName} has null prefab; Skip item.");
                    continue;
                }

                Utils.PrintDebug($"[LP:{i}-{offset}]:\"{sName}\";{pos};{rot};{prefab?.name}");
                _addItemToLocalPool(prefab, pos, rot);
            }
        }

        public static int GetCountInSave(byte[] array)
            => BitConverter.ToInt32(array, base_offset);

        public static int GetSizeInSave(byte[] array)
            => GetCountInSave(array) * 90;
    }
}
