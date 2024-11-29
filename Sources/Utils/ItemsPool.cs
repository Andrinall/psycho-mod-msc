using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Extensions;
using Object = UnityEngine.Object;


namespace Psycho.Internal
{
    internal static class ItemsPool
    {
        static List<GameObject> Pool = new List<GameObject>();

        internal static int Length { get => _getSortedPoolCount(); }

        internal static GameObject AddItem(GameObject prefab)
            => _addItemToLocalPool(prefab, Vector3.zero, Vector3.zero);

        internal static GameObject AddItem(GameObject prefab, Vector3 pos, Vector3 euler)
            => _addItemToLocalPool(prefab, pos, euler);

        internal static GameObject GetItem(int index)
            => Pool.ElementAtOrDefault(index);

        internal static bool RemoveItem(GameObject obj)
            => Pool.Remove(obj);

        internal static void Save(ref byte[] array, int offset)
        {
            List<GameObject> toSave = Pool.Where(v => v != null && v.transform.parent == null).ToList();
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
        }

        public static void Load(byte[] array, int offset)
        {
            int count = BitConverter.ToInt32(array, offset);
            offset += 4;

            Utils.PrintDebug($"[LoadPool]: pool size: {count}");
            if (count == 0)
            {
                Utils.PrintDebug(eConsoleColors.RED, "[LoadPool]: is empty; loading stopped");
                return;
            }


            for (int i = 0; i < count; i++)
            {
                string sName = "".GetFromBytes(array, ref offset); // 1
                Vector3 pos = new Vector3().GetFromBytes(array, ref offset);
                Vector3 rot = new Vector3().GetFromBytes(array, ref offset);

                GameObject prefab = null;
                switch (sName)
                {
                    case "Candle":
                        prefab = Globals.Candle_prefab;
                        break;
                    case "FernFlower":
                        prefab = Globals.FernFlower_prefab;
                        break;
                    case "Mushroom":
                        prefab = Globals.Mushroom_prefab;
                        break;
                    case "Walnut":
                        prefab = Globals.Walnut_prefab;
                        break;
                    case "BlackEgg":
                        prefab = Globals.BlackEgg_prefab;
                        break;
                    case "Picture":
                        prefab = Globals.Picture_prefab;
                        break;
                }

                Utils.PrintDebug($"[LP:{i}-{offset}]:\"{sName}\";{pos};{rot};{prefab?.name}");
                if (prefab == null)
                {
                    Utils.PrintDebug($"Loaded item {sName} has null prefab");
                    continue;
                }

                _addItemToLocalPool(prefab, pos, rot);
            }
        }


        private static GameObject _addItemToLocalPool(GameObject prefab, Vector3 pos, Vector3 euler)
        {
            GameObject cloned = (GameObject)Object.Instantiate(prefab, pos, Quaternion.Euler(euler));
            cloned.MakePickable();

            Pool.Add(cloned);
            return cloned;
        }

        private static int _getSortedPoolCount()
            => Pool.Where(v => v != null && v.transform.parent == null).ToList().Count;
    }
}
