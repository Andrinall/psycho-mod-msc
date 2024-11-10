using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


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
            if ((GlobalDay.Value % 7) != 3) return;

            if (SUN_hours.Value > 18 || SUN_hours.Value < 4)
            {
                if (AnyFlowerIsSpawned()) return;
            }
            else
            {
                if (!AnyFlowerIsSpawned()) return;
            }
        }

        bool AnyFlowerIsSpawned()
            => Flowers.Any(v => v.activeSelf);

        void SpawnRandomFlower()
        {
            GameObject point = Flowers[Random.Range(0, Flowers.Count)];
            point.SetActive(true);

            GameObject flower = GameObject.Instantiate(Globals.FernFlower_prefab);
            flower.transform.SetParent(point.transform, worldPositionStays: false);
            flower.transform.localPosition = Vector3.zero;
            flower.transform.localEulerAngles = Vector3.zero;
        }
    }
}
