
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    internal class FittanItemsStore : CatchedComponent
    {
        const int COOLDOWN_FRAMES = 120;
        const float ITEM_PRICE = 500f;


        GameObject container;
        List<Collider> colliders = new List<Collider>();

        Vector3 ItemsSpawnPoint = new Vector3(-4.850542f, 1.522235f, 7.135977f);
        
        int elapsedFrames = 120;
        bool offerAborted = false;


        protected override void Awaked()
        {
            var _container = transform.Find("Container");
            container = _container.gameObject;

            for (int i = 0; i < _container.childCount; i++)
            {
                var _child = _container.GetChild(i);
                colliders.Add(_child.GetComponent<BoxCollider>());
            }
        }

        protected override void OnFixedUpdate()
        {
            CheckVisibleAndSetIfNeeded();
            if (Camera.main == null) return;
            if (!Logic.InHorror) return;

            if (elapsedFrames < COOLDOWN_FRAMES) // clicking cooldown
            {
                Globals.GUIsubtitle.Value = offerAborted
                    ? Locales.BUY_ITEMS[0, Globals.CurrentLang]
                    : Locales.BUY_ITEMS[1, Globals.CurrentLang];

                elapsedFrames++;
                if (elapsedFrames == COOLDOWN_FRAMES)
                    offerAborted = false;

                return;
            }

            if (!Physics.Raycast(Globals.RayFromScreenPoint, out var _hitInfo, 2f)) return;
            if (_hitInfo.collider == null) return;
            if (!colliders.Any(v => v.gameObject == _hitInfo.collider.gameObject)) return;

            string _name = _hitInfo.collider.gameObject.name;
            Utils.SetGUIUse(true, $"{_name} {ITEM_PRICE}mk");
            
            if (!cInput.GetKeyUp("Use")) return;
            BuyItem(_name);
        }

        void BuyItem(string itemName)
        {
            elapsedFrames = 0;

            Utils.SetGUIUse(false);
            if (Globals.PlayerMoney.Value < ITEM_PRICE)
            {
                offerAborted = true;
                return;
            }

            GameObject _prefab = ItemsPool.GetPrefabByItemName(itemName);
            if (_prefab == null) return;

            Globals.PlayerMoney.Value -= ITEM_PRICE;
            ItemsPool.AddItem(_prefab, ItemsSpawnPoint, Vector3.zero);
            Utils.PrintDebug($"FittanItemsStore.BuyItem({itemName}) : Add Item");
        }

        void CheckVisibleAndSetIfNeeded()
        {
            if (container.activeSelf != Logic.InHorror)
                container.SetActive(Logic.InHorror);
        }
    }
}
