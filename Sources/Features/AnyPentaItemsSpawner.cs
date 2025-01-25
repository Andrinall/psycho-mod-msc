
using UnityEngine;

using Psycho.Internal;
using Psycho.Handlers;


namespace Psycho.Features
{
    class AnyPentaItemsSpawner : CatchedComponent
    {
        readonly Vector3 walnutPos = new Vector3(.071f, 0, .653f);
        readonly Vector3 candlePos = new Vector3(-11.264f, .089f, -3.158f);


        public int CandleDay = 0;
        public int MushroomDay = 4;
        public int WalnutDay = 2;

        public bool CandleSpawned = false;
        public bool MushroomSpawned = false;
        public bool WalnutSpawned = false;

        public Transform palm;
        public Transform island;
        public Transform church;

        static Vector3[] mushroomRand = new Vector3[5]
        {
            new Vector3(-9.978f, -0.901f, -7.219f),
            new Vector3(-11.085f, -0.901f, -7.571f),
            new Vector3(12.01f, -1.13f, 1.052f),
            new Vector3(6.232f, -0.929f, 7.801f),
            new Vector3(16.171f, -2.27f, -7.681f)
        };
        Vector3 mushroom_pos = mushroomRand[0];

        Vector3 NewRandomizedPosition => mushroomRand[Random.Range(0, mushroomRand.Length)];



        protected override void Awaked()
        {
            palm = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom/jukkapalm")?.transform;
            island = GameObject.Find("COTTAGE/SpawnToCottage")?.transform;
            church = GameObject.Find("PERAJARVI/CHURCH/Church/church_base")?.transform;

            if (palm == null || island == null || church == null)
                throw new System.NullReferenceException("Could't be find needed objects.\nMay be caused by a conflict for other mods.");
        }

        protected override void OnFixedUpdate()
        {
            if (ResourcesStorage.Walnut_prefab == null || ResourcesStorage.Mushroom_prefab == null || ResourcesStorage.Candle_prefab == null)
                throw new System.NullReferenceException("Prefabs doesn't loaded from bundle.");

            if (Globals.GlobalDay == null)
                throw new System.NullReferenceException("Global FSMString `GlobalDay` doesn't exists.");

            int _day = Globals.GlobalDay.Value % 7;
            CheckItemSpawnTimeAndSpawn(_day, WalnutDay, ref WalnutSpawned, palm, ResourcesStorage.Walnut_prefab, walnutPos);
            CheckItemSpawnTimeAndSpawn(_day, MushroomDay, ref MushroomSpawned, island, ResourcesStorage.Mushroom_prefab, mushroom_pos);
            CheckItemSpawnTimeAndSpawn(_day, CandleDay, ref CandleSpawned, church, ResourcesStorage.Candle_prefab, candlePos);
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
            GameObject _cloned = ItemsPool.AddItem(prefab);
            _cloned.transform.SetParent(parent, worldPositionStays: false);
            _cloned.transform.localPosition = pos;

            if (spawnDay == MushroomDay)
                mushroom_pos = NewRandomizedPosition;

            if (spawnDay == CandleDay)
                _cloned.transform.localEulerAngles = new Vector3(63.902f, -90, -90);

            _cloned.AddComponent<ItemsGravityEnabler>();
            spawned = true;
            Utils.PrintDebug($"Item {_cloned.name} spawned!");
        }

        void DestroyItem(Transform parent, ref bool spawned)
        {
            if (parent.childCount != 0)
            {

                GameObject _obj = parent.GetChild(0).gameObject;
                ItemsPool.RemoveItem(_obj);
                Utils.PrintDebug($"Item {_obj.name} destroyed, because not picked up.");
                Destroy(_obj);
            }

            spawned = false;
        }
    }
}
