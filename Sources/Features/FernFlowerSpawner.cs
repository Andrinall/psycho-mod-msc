
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Psycho.Internal;
using Psycho.Handlers;


namespace Psycho.Features
{
    internal sealed class FernFlowerSpawner : CatchedComponent
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
            if (Logic.GameFinished)
            {
                Destroy(this);
                return;
            }

            int _day = Globals.GlobalDay.Value % 7;

            if (_day == 3 && Globals.SUN_Hours.Value > 18)
                SpawnRandomFlower();
            else if (_day == 4 && Globals.SUN_Hours.Value < 4)
                SpawnRandomFlower();
            else
                DespawnItems();
        }


        internal void SpawnRandomFlower()
        {
            if (IsFlowerSpawned) return;

            GameObject _point = RandomFlower;
            GameObject _flower = ItemsPool.AddItem(ResourcesStorage.FernFlower_prefab);

            _flower.transform.SetParent(_point.transform, worldPositionStays: false);
            _flower.transform.localPosition = Vector3.zero;
            _flower.transform.localScale = new Vector3(.5f, .5f, .5f);
            _flower.AddComponent<ItemsGravityEnabler>();
            _flower.GetComponent<Rigidbody>().useGravity = false;

            _point.SetActive(true);
        }

        void DespawnItems()
        {
            if (!IsFlowerSpawned) return;

            foreach (GameObject flower in flowers)
            {
                if (!flower.activeSelf || flower.transform.childCount == 0) goto setActive;

                GameObject _child = flower.transform.GetChild(0).gameObject;
                ItemsPool.RemoveItem(_child);
                Destroy(_child);
            setActive:
                flower.SetActive(false);
            }
        }
    }
}
