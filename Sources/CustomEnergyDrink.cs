using System.Linq;
using System.Collections.Generic;

using Harmony;
using UnityEngine;
using HutongGames.PlayMaker;
using MSCLoader;

namespace Adrenaline
{
    internal class CustomEnergyDrink : MonoBehaviour
    {
        private List<Transform> prefabs;
        private FsmVariables OrderVars;

        private void Start()
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

                try
                {
                    var prefab = base.transform.Find("LOD/GFX_Pub/paper_stand");
                    if (prefab == null) Utils.PrintDebug(eConsoleColors.RED, "TryReplacePrefab(pub_desk,bool) | prefab is null!");

                    Utils.SetMaterial(prefab.gameObject, 0, "ATLAS_OFFICE(Clone)", Globals.atlas_texture, Vector2.zero, Vector2.one);
                }
                catch (System.Exception e)
                {
                    Utils.PrintDebug(eConsoleColors.RED, $"Failed to set texture for pub_desk\n{e.GetFullMessage()}");
                }

                var cren = prefabs.Find(v => v.name == "Coffee").gameObject.AddComponent<ItemRenamer>();
                cren.TargetName = "coffee(itemx)";
                cren.FinalName = "energy drunk(itemx)";

                var cfren = prefabs.Find(v => v.name == "CoffeeFly").gameObject.AddComponent<ItemRenamer>();
                cfren.TargetName = "empty cup(Clone)";
                cfren.FinalName = "empty can(Clone)";

                SetDrinkPrice(AdrenalineLogic.config.GetValueSafe("PUB_COFFEE_PRICE"));
                Utils.PrintDebug(eConsoleColors.GREEN, "CustomEnergyDrink enabled");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to load CustomEnergyDrink component\n{e.GetFullMessage()}");
            }
        }

        internal void SetDrinkPrice(float price)
        {
            var OrderCoffee = base.transform.Find("LOD/ActivateBar/OrderList/4").GetPlayMaker("Buy");
            OrderVars = OrderCoffee.FsmVariables;
            OrderVars.GetFsmString("Notification").Value = string.Format("ENERGY DRINK {0} MK", price);
            OrderVars.GetFsmFloat("Price").Value = price;
        }

        private void TryReplacePrefab(string name, bool isEmpty = false)
        {
            var prefab = prefabs.Find(v => v.name == name);
            ReplacePrefab(prefab, isEmpty);
        }
        
        private void TryReplacePrefab(Transform obj, bool isEmpty = false)
        {
            ReplacePrefab(obj, isEmpty);
        }

        private void ReplacePrefab(Transform obj, bool isEmpty)
        {

            if (obj == null)
                Utils.PrintDebug(eConsoleColors.RED, "ReplacePrefab(obj,bool) | base prefab is null");

            try
            {
                var mesh = isEmpty ? Globals.empty_cup : Globals.coffee_cup;
                Utils.ChangeModel(obj.gameObject, mesh, Globals.can_texture, Vector2.zero, Vector2.one);

                if (obj.childCount == 0) return;
                Utils.ChangeModel(obj.GetChild(0).gameObject, mesh, Globals.can_texture, Vector2.zero, Vector2.one);
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to replace prefab {obj?.name.ToString()}\n{e.GetFullMessage()}");
            }
        }
    }
}
