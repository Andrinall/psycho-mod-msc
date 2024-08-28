using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class CustomEnergyDrink : MonoBehaviour
    {
        private List<Transform> prefabs;

        private void OnEnable()
        {
            prefabs = Resources.FindObjectsOfTypeAll<Transform>().Where(v => v.root.IsPrefab()).ToList();

            Utils.PrintDebug("PREFAB objects in scene: " + prefabs.Count);
            Utils.PrintDebug("Scene contains Coffee prefab" + prefabs.Any(v => v.name == "Coffee"));
            Utils.PrintDebug("Scene contains empty coffee cup prefab" + prefabs.Any(v => v.name == "coffee_cup_bar.mesh"));

            TryReplacePrefab("Coffee");
            TryReplacePrefab("coffee_cup_bar.mesh", true);
            TryReplacePrefab("coffee_cup_bar_coffee.mesh");
            TryReplacePrefab("CoffeeFly", true);
            TryReplacePrefab(base.transform.Find("TeimoInShop/Pivot/Teimo/skeleton/pelvis/spine_middle/spine_upper/collar_left/shoulder_left/arm_left/hand_left/ItemPivot/CoffeeCup"));

            Utils.PrintDebug("CustomEnergyDrink enabled");
            Utils.PrintDebug("CustomTexture: " + AdrenalineLogic.texture.name.ToString());
        }

        private void TryReplacePrefab(string name, bool isEmpty = false)
        {
            try
            {
                var prefab = prefabs.Find(v => v.name == name);
                if (prefab == null) Utils.PrintDebug("TryReplacePrefab(" + name + ",bool) | prefab is null!");
                
                SetMaterial(prefab, isEmpty);
                
                if(prefab.childCount > 0)
                    SetMaterial(prefab.GetChild(0), isEmpty);
            }
            catch
            {
                Utils.PrintDebug("<color=red>Unable to set texture for " + name + " prefab</color>");
            }
        }

        private void TryReplacePrefab(Transform obj, bool isEmpty = false)
        {
            try
            {
                if (obj == null) Utils.PrintDebug("TryReplacePrefab(Transform,bool) | prefab is null");

                SetMaterial(obj, isEmpty);
                if (obj.childCount > 0)
                    SetMaterial(obj.GetChild(0), isEmpty);
            }
            catch
            {
                Utils.PrintDebug("<color=red>Unable to set texture for " + obj?.name.ToString() + "prefab</color>");
            }
        }
        
        private void SetMaterial(Transform obj, bool isEmpty)
        {
            if (obj == null)
            {
                Utils.PrintDebug("obj == null in SetMaterial");
                return;
            }
            var renderer = obj?.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var mat = renderer.materials.ElementAt(0);
                var filter = obj.GetComponent<MeshFilter>();

                mat.name = "Energy";
                mat.mainTexture = AdrenalineLogic.texture;
                mat.mainTextureOffset = new Vector2(0f, 0f);
                mat.mainTextureScale = new Vector2(1f, 1f);
                
                var mesh = (isEmpty ? AdrenalineLogic.empty_cup : AdrenalineLogic.coffee_cup);
                filter.mesh = mesh;
                filter.sharedMesh = mesh;
            }

        }
    }
}
