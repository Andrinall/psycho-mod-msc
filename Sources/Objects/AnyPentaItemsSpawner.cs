using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using MSCLoader;
using Psycho.Extensions;


namespace Psycho.Sources.Objects
{
    internal sealed class AnyPentaItemsSpawner : CatchedComponent
    {
        public int CandleDay = 0;
        public int MushroomDay = 4;
        public int NutDay = 2;

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

        Vector3 walnut_pos = new Vector3(.071f, 0, .653f);
        Vector3 candle_pos = new Vector3(-11.264f, .089f, -3.158f);
        Vector3 mushroom_pos = mushroom_rand[0];


        internal override void Awaked()
        {
            palm = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom/jukkapalm").transform;
            island = GameObject.Find("COTTAGE/SpawnToCottage").transform;
            church = GameObject.Find("PERAJARVI/CHURCH/Church/church_base").transform;
            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
        }

        internal override void OnFixedUpdate()
        {
            int day = GlobalDay.Value % 7;
            CheckItemSpawnTimeAndSpawn(day, NutDay, ref WalnutSpawned, palm, Globals.Walnut_prefab, walnut_pos);
            CheckItemSpawnTimeAndSpawn(day, MushroomDay, ref MushroomSpawned, island, Globals.Mushroom_prefab, mushroom_pos);
            CheckItemSpawnTimeAndSpawn(day, CandleDay, ref CandleSpawned, church, Globals.Candle_prefab, candle_pos);
        }

        void CheckItemSpawnTimeAndSpawn(
            int day, int spawnDay, ref bool spawned,
            Transform parent, GameObject prefab, Vector3 pos
        ) {
            if (!spawned && day == spawnDay && parent.childCount == 0)
            {
                GameObject cloned = GameObject.Instantiate(prefab);
                cloned.transform.SetParent(parent, worldPositionStays: false);
                cloned.transform.localPosition = pos;

                if (spawnDay == MushroomDay)
                {
                    cloned.transform.localScale = new Vector3(.5f, .5f, .5f);
                    mushroom_pos = mushroom_rand[Random.Range(0, mushroom_rand.Length)];
                }

                if (spawnDay == CandleDay)
                {
                    cloned.transform.localEulerAngles = new Vector3(63.902f, -90, -90);
                    cloned.transform.localScale = new Vector3(1.2f, 1.2f, 5);
                    cloned.GetComponent<MeshRenderer>().materials[0].color = new Color(.6886792f, .6486886f, .4905215f);
                }

                _applyGravity(cloned);
                cloned.MakePickable();
                spawned = true;
            }
            else if (spawnDay != MushroomDay && spawned && day != spawnDay)
            {
                Utils.PrintDebug("destroy 1");
                _destroy(parent, ref spawned);
            }
            else if (spawnDay == MushroomDay && spawned && day != spawnDay && day >= 0)
            {
                Utils.PrintDebug("destroy 2");
                _destroy(parent, ref spawned);
            }
        }

        void _applyGravity(GameObject cloned)
        {
            Rigidbody rb = cloned.GetComponent<Rigidbody>();
            if (!rb) return;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        void _destroy(Transform parent, ref bool spawned)
        {
            if (parent.childCount != 0)
                Object.Destroy(parent.GetChild(0).gameObject);

            spawned = false;
        }
    }
}
