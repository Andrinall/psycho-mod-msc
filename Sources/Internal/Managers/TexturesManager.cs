
using System.Linq;
using System.Collections.Generic;

using UnityEngine;


namespace Psycho.Internal
{
    static class TexturesManager
    {
        public static Dictionary<Material, Texture> Cache = new Dictionary<Material, Texture>();
        static Material[] globalMaterials;

        const string PROPERTY = "_MainTex";

        public static void ReplaceTextures(List<Texture> container)
        {            
            globalMaterials = Resources.FindObjectsOfTypeAll<Material>();

            int _replaced = 0;
            foreach (Texture _texture in container)
            {
                if (!ReplaceTextureForMaterials(_texture)) continue;
                _replaced++;
            }

            Utils.PrintDebug($"ReplaceTextures ends [replaced {_replaced}][cache len: {Cache.Count}]");
        }

        public static void RestoreDefaults(List<Texture> container)
        {
            if (Cache.Count == 0) return;

            globalMaterials = Resources.FindObjectsOfTypeAll<Material>();

            Dictionary<Material, Texture> _clonedCache = new Dictionary<Material, Texture>(Cache);
            List<Material> _forRemove = new List<Material>();

            foreach (KeyValuePair<Material, Texture> _item in _clonedCache)
            {
                if (_item.Key == null || _item.Value == null) continue;
                if (!container.Any(v => v.name == _item.Value?.name)) continue;
                _item.Key.SetTexture(PROPERTY, _item.Value);
                _forRemove.Add(_item.Key);
            }

            Utils.PrintDebug($"RestoreDefaults ends [restored: {_forRemove.Count}][cache len: {Cache.Count}]");
            _forRemove.ForEach(v => Cache.Remove(v));
        }

        /// <summary>
        /// Setup independently textures
        /// </summary>
        /// <param name="onSave">if true - reset to default textures</param>
        public static void ChangeIndepTextures(bool onSave)
        {
            if (onSave)
                RestoreDefaults(ResourcesStorage.IndependentlyTextures);
            else
                ReplaceTextures(ResourcesStorage.IndependentlyTextures);
        }

        public static void ChangeWorldTextures(bool state)
        {
            if (state)
                ReplaceTextures(ResourcesStorage.Replaces);
            else
                RestoreDefaults(ResourcesStorage.Replaces);
        }

        static bool ReplaceTextureForMaterials(Texture texture, bool removeCache = false)
        {
            if (texture == null) return false;

            string _name = texture.name;

            Material[] _materialsWhereContainsTexture = globalMaterials.Where(v => v.GetTexture("_MainTex")?.name == _name).ToArray();
            if (_materialsWhereContainsTexture.Length == 0) return false;

            bool _result = true;
            foreach (Material _material in _materialsWhereContainsTexture)
            {
                if (_material == null) continue;

                if (!CacheMaterialTextureAndSetNew(_material, texture, removeCache))
                    _result = false;
            }

            return _result;
        }

        static bool CacheMaterialTextureAndSetNew(Material material, Texture texture, bool removeCache = false)
        {
            if (removeCache)
            {
                if (!Cache.ContainsKey(material)) return false;
                material.SetTexture(PROPERTY, texture);

                if (!Cache.Remove(material))
                    return false;

                return true;
            }

            if (Cache.ContainsKey(material)) return false;
            Cache.Add(material, material.GetTexture(PROPERTY));
            material.SetTexture(PROPERTY, texture);
            return true;
        }
    }
}
