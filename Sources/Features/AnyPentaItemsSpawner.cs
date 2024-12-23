using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Handlers;


namespace Psycho.Features
{
    internal sealed class AnyPentaItemsSpawner : CatchedComponent
    {
        public int CandleDay = 0;
        public int MushroomDay = 4;
        public int WalnutDay = 2;

        public bool CandleSpawned = false;
        public bool MushroomSpawned = false;
        public bool WalnutSpawned = false;

        public Transform palm;
        public Transform island;
        public Transform church;

        public FsmInt GlobalDay;

        static Vector3[] mushroom_rand = new Vector3[5]
        {
            new Vector3(-9.978f, -0.901f, -7.219f),
            new Vector3(-11.085f, -0.901f, -7.571f),
            new Vector3(12.01f, -1.13f, 1.052f),
            new Vector3(6.232f, -0.929f, 7.801f),
            new Vector3(16.171f, -2.27f, -7.681f)
        };

        Vector3 NewRandomizedPosition => mushroom_rand[Random.Range(0, mushroom_rand.Length)];

        Vector3 walnut_pos = new Vector3(.071f, 0, .653f);
        Vector3 candle_pos = new Vector3(-11.264f, .089f, -3.158f);
        Vector3 mushroom_pos = mushroom_rand[0];


        protected override void Awaked()
        {
            palm = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom/jukkapalm").transform;
            island = GameObject.Find("COTTAGE/SpawnToCottage").transform;
            church = GameObject.Find("PERAJARVI/CHURCH/Church/church_base").transform;
            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
        }

        protected override void OnFixedUpdate()
        {
            int day = GlobalDay.Value % 7;
            CheckItemSpawnTimeAndSpawn(day, WalnutDay, ref WalnutSpawned, palm, Globals.Walnut_prefab, walnut_pos);
            CheckItemSpawnTimeAndSpawn(day, MushroomDay, ref MushroomSpawned, island, Globals.Mushroom_prefab, mushroom_pos);
            CheckItemSpawnTimeAndSpawn(day, CandleDay, ref CandleSpawned, church, Globals.Candle_prefab, candle_pos);
        }

        void CheckItemSpawnTimeAndSpawn(
            int day, int spawnDay, ref bool spawned,
            Transform parent, GameObject prefab, Vector3 pos
        ) {
            if (!spawned && day == spawnDay && parent.childCount == 0)
                SpawnItem(prefab, parent, pos, spawnDay, ref spawned);
            else if (spawnDay != MushroomDay && spawned && day != spawnDay)
                DestroyItem(parent, ref spawned);
            else if (spawnDay == MushroomDay && spawned && day != spawnDay && day >= 0)
                DestroyItem(parent, ref spawned);
        }

        void SpawnItem(GameObject prefab, Transform parent, Vector3 pos, int spawnDay, ref bool spawned)
        {
            GameObject cloned = ItemsPool.AddItem(prefab);
            cloned.transform.SetParent(parent, worldPositionStays: false);
            cloned.transform.localPosition = pos;

            if (spawnDay == MushroomDay)
                mushroom_pos = NewRandomizedPosition;

            if (spawnDay == CandleDay)
                cloned.transform.localEulerAngles = new Vector3(63.902f, -90, -90);

            cloned.AddComponent<ItemsGravityEnabler>();
            spawned = true;
            Utils.PrintDebug($"Item {cloned.name} spawned!");
        }

        void DestroyItem(Transform parent, ref bool spawned)
        {
            if (parent.childCount != 0)
            {

                GameObject obj = parent.GetChild(0).gameObject;
                ItemsPool.RemoveItem(obj);
                Utils.PrintDebug($"Item {obj.name} destroyed, because not picked up.");
                Destroy(obj);
            }

            spawned = false;
        }
    }
}
