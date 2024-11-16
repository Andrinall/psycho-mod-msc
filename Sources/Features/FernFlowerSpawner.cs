using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Handlers;


namespace Psycho.Features
{
    internal sealed class FernFlowerSpawner : MonoBehaviour
    {
        List<GameObject> Flowers = new List<GameObject>();

        FsmInt GlobalDay;
        FsmFloat SUN_hours;

        void Awake()
        {
            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
            SUN_hours = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock").GetVariable<FsmFloat>("Hours");

            for (int i = 0; i < transform.childCount; i++)
                Flowers.Add(transform.GetChild(i).GetChild(1).gameObject);
        }

        void FixedUpdate()
        {
            if (((GlobalDay.Value % 7) == 3) && (SUN_hours.Value > 18 || SUN_hours.Value < 4))
            {
                if (AnyFlowerIsSpawned()) return;
                SpawnRandomFlower();
            }
            else
            {
                if (!AnyFlowerIsSpawned()) return;
                DespawnItems();
            }
        }

        internal bool AnyFlowerIsSpawned()
            => Flowers.Any(v => v.activeSelf);

        internal void SpawnRandomFlower()
        {
            GameObject point = Flowers[Random.Range(0, Flowers.Count)];
            GameObject flower = Globals.AddPentaItem(Globals.FernFlower_prefab);

            flower.transform.SetParent(point.transform, worldPositionStays: false);
            flower.transform.localPosition = Vector3.zero;
            flower.transform.localScale = new Vector3(.5f, .5f, .5f);
            flower.AddComponent<ItemsGravityEnabler>();
            flower.GetComponent<Rigidbody>().useGravity = false;

            point.SetActive(true);
        }

        void DespawnItems()
        {
            foreach (GameObject flower in Flowers)
            {
                if (!flower.activeSelf || flower.transform.childCount == 0) goto setActive;

                GameObject child = flower.transform.GetChild(0).gameObject;
                Globals.RemovePentaItem(child);
                Destroy(child);
            setActive:
                flower.SetActive(false);
            }
        }
    }
}
