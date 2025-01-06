
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
        public static int base_offset { get; } = 64;

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
            var item = Pool.FirstOrDefault(callback);
            
            Object.Destroy(item);
            return Pool.Remove(item);
        }

        internal static void RemoveItems(Func<GameObject, bool> callback)
        {
            var _items = Pool.Where(callback).ToList();
            foreach (var item in _items)
            {
                Object.Destroy(item);
                Pool.Remove(item);
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

        /*internal static void Save(ref byte[] array)
        {
            int offset = base_offset;
            List<GameObject> toSave = ItemsForSave;
            BitConverter.GetBytes(toSave.Count).CopyTo(array, offset);

            offset += 4;
            Utils.PrintDebug($"[SavePool]:[{offset}]: Length == {toSave.Count}");

            foreach (GameObject item in toSave)
            {
                string name = item.name.Replace("(Clone)", "");
                Vector3 pos = item.transform.position;
                Vector3 rot = item.transform.eulerAngles;

                Utils.PrintDebug($"[SavePool]:[{offset}]: name \"{name}\"; length: {name.Length}");
                Utils.PrintDebug($"[SavePool]:[{offset}]: pos: {pos}; rot: {rot}");

                name.CopyBytes(ref array, ref offset);
                pos.CopyBytes(ref array, ref offset);
                rot.CopyBytes(ref array, ref offset);
            }
        }*/

        /*public static void Load(byte[] array)
        {
            int offset = base_offset;
            int count = BitConverter.ToInt32(array, offset);
            offset += 4;

            Utils.PrintDebug(eConsoleColors.YELLOW, $"[LoadPool]: pool size: {count}");
            if (count == 0)
            {
                Utils.PrintDebug(eConsoleColors.RED, "[LoadPool]: is empty; loading stopped");
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
                    Utils.PrintDebug(eConsoleColors.RED, $"Loaded item {sName} has null prefab");
                    continue;
                }

                Utils.PrintDebug(eConsoleColors.YELLOW, $"[LP:{i}-{offset}]:\"{sName}\";{pos};{rot};{prefab?.name}");
                _addItemToLocalPool(prefab, pos, rot);
            }
        }*/

        /*public static int GetCountInSave(byte[] array)
            => BitConverter.ToInt32(array, base_offset);*/

        /*public static int GetSizeInSave(byte[] array)
            => GetCountInSave(array) * 90;*/

        private static GameObject _addItemToLocalPool(GameObject prefab, Vector3 pos, Vector3 euler)
        {
            GameObject cloned = (GameObject)Object.Instantiate(prefab, pos, Quaternion.Euler(euler));
            if (prefab.name == "Notebook")
                Globals.Notebook = cloned.AddComponent<Notebook>();
            if (prefab.name == "Postcard")
            {
                cloned.AddComponent<ItemsGravityEnabler>();
                Utils.InitPostcard(cloned);
            }

            cloned.MakePickable();

            Pool.Add(cloned);
            return cloned;
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
