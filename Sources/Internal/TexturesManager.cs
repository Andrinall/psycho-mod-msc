using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Steamworks;

namespace Psycho.Internal
{
    internal static class TexturesManager
    {
        internal static Dictionary<Material, Texture> Cache = new Dictionary<Material, Texture>();
        static Material[] GlobalMaterials;

        const string TexProperty = "_MainTex";

        internal static void ReplaceTextures(List<Texture> container)
        {            
            GlobalMaterials = Resources.FindObjectsOfTypeAll<Material>();

            int replaced = 0;
            foreach (Texture texture in container)
            {
                if (!ReplaceTextureForMaterials(texture)) continue;
                replaced++;
            }

            Utils.PrintDebug($"ReplaceTextures ends [replaced {replaced}][cache len: {Cache.Count}]");
        }

        internal static void RestoreDefaults(List<Texture> container)
        {
            if (Cache.Count == 0) return;

            GlobalMaterials = Resources.FindObjectsOfTypeAll<Material>();

            Dictionary<Material, Texture> clonedCache = new Dictionary<Material, Texture>(Cache);
            List<Material> forRemove = new List<Material>();

            foreach (KeyValuePair<Material, Texture> item in clonedCache)
            {
                if (item.Key == null || item.Value == null) continue;
                if (!container.Any(v => v.name == item.Value?.name)) continue;
                item.Key.SetTexture(TexProperty, item.Value);
                forRemove.Add(item.Key);
            }

            Utils.PrintDebug($"RestoreDefaults ends [restored: {forRemove.Count}][cache len: {Cache.Count}]");
            forRemove.ForEach(v => Cache.Remove(v));
        }

        static bool ReplaceTextureForMaterials(Texture texture, bool removeCache = false)
        {
            if (texture == null) return false;

            string name = texture.name;

            Material[] materialsWhereContainsTexture = GlobalMaterials.Where(v => v.GetTexture("_MainTex")?.name == name).ToArray();
            if (materialsWhereContainsTexture.Length == 0) return false;

            bool result = true;
            foreach (Material material in materialsWhereContainsTexture)
            {
                if (material == null) continue;

                if (!CacheMaterialTextureAndSetNew(material, texture, removeCache))
                    result = false;
            }

            return result;
        }

        static bool CacheMaterialTextureAndSetNew(Material material, Texture texture, bool removeCache = false)
        {
            if (removeCache)
            {
                if (!Cache.ContainsKey(material)) return false;
                material.SetTexture(TexProperty, texture);

                if (!Cache.Remove(material))
                    return false;

                return true;
            }

            if (Cache.ContainsKey(material)) return false;
            Cache.Add(material, material.GetTexture(TexProperty));
            material.SetTexture(TexProperty, texture);
            return true;
        }
    }
}
