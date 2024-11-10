using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Handlers;


namespace Psycho.Objects
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

        internal void SpawnRandomFlower(bool byCmd = false)
        {
            GameObject point = Flowers[Random.Range(0, Flowers.Count)];
            if (byCmd && point.activeSelf) return;

            GameObject flower = GameObject.Instantiate( Globals.Walnut_prefab /*Globals.FernFlower_prefab*/ );
            flower.transform.SetParent(point.transform, worldPositionStays: false);
            flower.transform.localPosition = Vector3.zero;
            flower.transform.localEulerAngles = Vector3.zero;
            flower.AddComponent<ItemsGravityEnabler>();
            flower.MakePickable();

            if (byCmd) flower.name = "flower(cmd)";

            point.SetActive(true);
        }

        void DespawnItems()
        {
            Flowers.ForEach(v =>
            {
                if (!v.activeSelf || v.transform.childCount == 0) goto setActive;

                GameObject child = v.transform.GetChild(0).gameObject;
                if (child.name == "flower(cmd)") return;

                Object.Destroy(child);
                
            setActive:
                v.SetActive(false);
            });
        }
    }
}
