
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Features;

using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    static class ItemsPool
    {
        static List<GameObject> Pool = new List<GameObject>();
        static GameObject[] ItemsForSave => Pool.Where(v => v != null && v.transform.parent == null).ToArray();
        public static int Length => Pool.Count;


        public static GameObject AddItem(GameObject prefab)
            => _addItemToLocalPool(prefab, Vector3.zero, Vector3.zero);

        public static GameObject AddItem(GameObject prefab, Vector3 pos, Vector3 euler)
            => _addItemToLocalPool(prefab, pos, euler);

        public static GameObject GetItem(int index)
            => Pool.ElementAtOrDefault(index);

        public static bool RemoveItem(GameObject obj)
            => Pool.Remove(obj);

        public static bool RemoveItem(Func<GameObject, bool> callback)
        {
            GameObject _item = Pool.FirstOrDefault(callback);
            
            Object.Destroy(_item);
            return Pool.Remove(_item);
        }

        public static void RemoveItems(Func<GameObject, bool> callback)
        {
            var _items = Pool.Where(callback).ToList();
            foreach (GameObject _item in _items)
            {
                Object.Destroy(_item);
                Pool.Remove(_item);
            }
        }

        public static List<ItemsPoolSaveLoadData> GetSaveData()
        {
            var _list = new List<ItemsPoolSaveLoadData>();

            foreach (var _item in ItemsForSave)
            {
                string _name = _item?.name?.Replace("(Clone)", "") ?? "";
                if (_name == "") continue;

                _list.Add(new ItemsPoolSaveLoadData
                {
                    Name = _name,
                    Position = _item?.transform?.position ?? Vector3.zero,
                    Euler = _item?.transform?.eulerAngles ?? Vector3.zero
                });
            }

            return _list;
        }

        public static void LoadData(List<ItemsPoolSaveLoadData> data)
        {
            foreach (var _item in data)
            {
                GameObject _prefab = GetPrefabByItemName(_item.Name);
                if (_prefab == null) continue;

                _addItemToLocalPool(_prefab, _item.Position, _item.Euler);
            }
        }

        public static GameObject GetPrefabByItemName(string item)
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



        static GameObject _addItemToLocalPool(GameObject prefab, Vector3 pos, Vector3 euler)
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
