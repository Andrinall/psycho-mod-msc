using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class ItemRenamer : MonoBehaviour
    {
        private void Update()
        {
            if (base.gameObject.name == "coffee(itemx)")
            {
                base.gameObject.name = "energy drunk(itemx)";
                Object.Destroy(this);
                return;
            }

            if (base.gameObject.name == "empty cup(Clone)")
            {
                base.gameObject.name = "empty can(Clone)";
                Object.Destroy(this);
                return;
            }
        }
    }

    internal class CustomEnergyDrink : MonoBehaviour
    {
        private List<Transform> prefabs;
        private FsmVariables OrderVars;

        private void OnEnable()
        {
            try
            {
                prefabs = Resources.FindObjectsOfTypeAll<Transform>().Where(v => v.root.IsPrefab()).ToList();

                TryReplacePrefab("Coffee");
                TryReplacePrefab("coffee_cup_bar.mesh", true);
                TryReplacePrefab("coffee_cup_bar_coffee.mesh");
                TryReplacePrefab("CoffeeFly", true);
                TryReplacePrefab(base.transform.Find("TeimoInShop/Pivot/Teimo/skeleton/pelvis/spine_middle/spine_upper/collar_left/shoulder_left/arm_left/hand_left/ItemPivot/CoffeeCup"));
                TryReplacePrefab(GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink/Hand/Coffee").transform, true);

                prefabs.Find(v => v.name == "Coffee").gameObject.AddComponent<ItemRenamer>();
                prefabs.Find(v => v.name == "CoffeeFly").gameObject.AddComponent<ItemRenamer>();

                SetDrinkPrice(AdrenalineLogic.config.PUB_PRICE);
                Utils.PrintDebug(eConsoleColors.GREEN, "CustomEnergyDrink enabled");
            } catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load CustomEnergyDrink component");
            }
        }

        public void SetDrinkPrice(float price)
        {
            var OrderCoffee = base.transform.Find("LOD/ActivateBar/OrderList/4")
                .GetComponents<PlayMakerFSM>().First(v => v.FsmName == "Buy");

            OrderVars = OrderCoffee.FsmVariables;
            OrderVars.GetFsmString("Notification").Value = string.Format("ENERGY DRINK {0} MK", price);
            OrderVars.GetFsmFloat("Price").Value = price;
        }

        private void TryReplacePrefab(string name, bool isEmpty = false)
        {
            try
            {
                var prefab = prefabs.Find(v => v.name == name);
                if (prefab == null)
                    Utils.PrintDebug(eConsoleColors.RED, "TryReplacePrefab({0},bool) | prefab is null!", name);

                SetMaterial(prefab, isEmpty);

                if (prefab.childCount > 0)
                    SetMaterial(prefab.GetChild(0), isEmpty);
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to set texture for {0} prefab", name);
            }
        }

        private void TryReplacePrefab(Transform obj, bool isEmpty = false)
        {
            try
            {
                if (obj == null)
                    Utils.PrintDebug(eConsoleColors.RED, "TryReplacePrefab(Transform,bool) | prefab is null");

                SetMaterial(obj, isEmpty);
                if (obj.childCount > 0)
                    SetMaterial(obj.GetChild(0), isEmpty);
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to set texture for {0} prefab", obj?.name.ToString());
            }
        }
        
        private void SetMaterial(Transform obj, bool isEmpty)
        {
            if (obj == null)
                return;

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
