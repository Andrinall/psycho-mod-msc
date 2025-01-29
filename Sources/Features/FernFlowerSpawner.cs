
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    class FernFlowerSpawner : CatchedComponent
    {
        List<GameObject> flowers = new List<GameObject>();

        GameObject RandomFlower => flowers[Random.Range(0, flowers.Count)];
        bool IsFlowerSpawned => flowers.Any(v => v.activeSelf);


        protected override void Awaked()
        {
            for (int i = 0; i < transform.childCount; i++)
                flowers.Add(transform.GetChild(i).GetChild(1).gameObject);
        }

        protected override void OnFixedUpdate()
        {
            int _day = Globals.GlobalDay.Value % 7;

            if (_day == 3 && Globals.SUN_Hours >= 20)
                SpawnRandomFlower();
            else if (_day == 4 && Globals.SUN_Hours < 4)
                SpawnRandomFlower();
            else
                DespawnItems();
        }


        public void SpawnRandomFlower()
        {
            if (IsFlowerSpawned) return;

            GameObject _point = RandomFlower;
            GameObject _flower = ItemsPool.AddItem(ResourcesStorage.FernFlower_prefab);

            _flower.transform.SetParent(_point.transform, worldPositionStays: false);
            _flower.transform.localPosition = new Vector3(0f, 0.14f, 0f);
            _flower.transform.eulerAngles = new Vector3(90f, 270f, 0f);
            _flower.transform.localScale = new Vector3(.5f, .5f, .5f);
            _flower.AddComponent<ItemsGravityCrutch>();

            _point.SetActive(true);
            Utils.PrintDebug("Fern Flower Spawned");
        }

        void DespawnItems()
        {
            if (!IsFlowerSpawned) return;

            foreach (GameObject _flower in flowers)
            {
                if (!_flower.activeSelf || _flower.transform.childCount == 0) goto setActive;

                GameObject _child = _flower.transform.GetChild(0).gameObject;
                ItemsPool.RemoveItem(_child);
                Destroy(_child);
            setActive:
                _flower.SetActive(false);
            }
        }
    }
}
