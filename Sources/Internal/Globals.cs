
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Features;


namespace Psycho.Internal
{
    struct ModelData
    {
        public string path { get; set; }
        public Mesh mesh { get; set; }
        public Texture texture { get; set; }
    }

    struct ScreamHand
    {
        public HandOrig orig { get; set; }
        public HandParent parent { get; set; }
        public Vector3 position { get; set; }
        public Vector3 euler { get; set; }
        public float scale { get; set; }
    }

    static class Globals
    {
        public static readonly Dictionary<int, ModelData> ModelsCached = new Dictionary<int, ModelData> { };
        public static readonly Dictionary<int, object> CachedTextures = new Dictionary<int, object> { };
        public static readonly List<AudioClip> CachedFliesSounds = new List<AudioClip> { };


        public static int CurrentLang = 0;

        public static Transform Player;

        public static FsmFloat PlayerHunger;
        public static FsmFloat PlayerThirst;
        public static FsmFloat PlayerFatigue;
        public static FsmFloat PlayerStress;
        public static FsmFloat PlayerMoney;

        public static FsmBool GUIuse;
        public static FsmString GUIinteraction;
        public static FsmString GUIsubtitle;

        public static FsmInt GlobalDay;
        public static Ray RayFromScreenPoint => Camera.main.ScreenPointToRay(Input.mousePosition);

        static FsmInt sunHours;
        public static int SUN_Hours => sunHours.Value;

        static FsmFloat sunMinutes;
        public static float SUN_Minutes => Mathf.FloorToInt(sunMinutes.Value % 60);
        

        public static Notebook Notebook = null;

        public static GameObject MailboxSheet = null;
        public static GameObject EnvelopeObject = null;
        public static GameObject CrowsList = null;
        public static GameObject SuicidalsList = null;
        public static GameObject FullScreenScreamer = null;

        public static AudioSource Heartbeat_source = null;
        public static AudioSource PhantomScream_source = null;
        public static AudioSource GlobalAmbient_source = null;
        public static AudioSource GlobalPsychoAmbient_source = null;


        public static readonly List<Vector3> PillsPositions = new List<Vector3> {
            new Vector3(462.2887f, 9.311339f, 1320.133f),
            new Vector3(1353.568f, 5.9235f, 821.1407f),
            new Vector3(1396f, 4.59463f, 850.6066f),
            new Vector3(1360.994f, 7.37431f, 805.0872f),
            new Vector3(1373.052f, 9.094034f, 795.0477f),
            new Vector3(1366.354f, 10.09609f, 798.3608f),
            new Vector3(1363.531f, 11.67623f, 803.7286f),
            new Vector3(1444.064f, -4.52272f, 722.709f),
            new Vector3(1559.36f, 4.486989f, 719.0803f),
            new Vector3(1548.925f, 4.417933f, 741.9292f),
            new Vector3(1598.357f, 3.662708f, 645.6377f),
            new Vector3(1790.758f, 3.977903f, 556.7726f),
            new Vector3(1796.5f, 3.642186f, 40.22343f),
            new Vector3(2168.991f, -1.790677f, 61.94064f),
            new Vector3(1932.791f, 6.842865f, -223.3672f),
            new Vector3(1895.464f, 5.256322f, -411.0562f),
            new Vector3(1893.25f, -3.624627f, -781.93f),
            new Vector3(1064.38f, 5.352709f, -910.4662f),
            new Vector3(664.62f, 0.9491397f, -1219.598f),
            new Vector3(454.0934f, 2.124174f, -1322.424f),
            new Vector3(286.9083f, 0.7398606f, -1269.742f),
            new Vector3(105.4728f, 4.87885f, -1357.449f),
            new Vector3(-187.5422f, 2.033351f, -942.8172f),
            new Vector3(-1196.174f, 0.1953767f, -625.333f),
            new Vector3(-1311.078f, -1.490561f, -618.7657f),
            new Vector3(-1495.032f, 26.79742f, -349.7293f),
            new Vector3(-669.2556f, 7.799858f, -721.1388f),
            new Vector3(-1271.826f, 2.043516f, -961.0248f),
            new Vector3(-240.0972f, 2.171151f, -1061.969f),
            new Vector3(-2014.459f, 105.964f, -144.1324f),
            new Vector3(-2004.393f, 68.72627f, -103.7944f),
            new Vector3(-1766.836f, 6.30492f, -353.0125f),
            new Vector3(-1619.084f, 1.936041f, 481.3835f),
            new Vector3(-1722.345f, 3.172916f, 952.5532f),
            new Vector3(-1516.232f, 7.27427f, 1369.108f),
            new Vector3(-1517.137f, 3.151923f, 1241.793f),
            new Vector3(-1409.24f, 5.365961f, 1139.492f),
            new Vector3(-1387.699f, 3.106995f, 1261.727f),
            new Vector3(-1274.975f, -0.3430491f, 1084.578f),
            new Vector3(-1118.886f, 0.650418f, 1312.829f),
            new Vector3(-845.4117f, 1.910222f, 1332.593f),
            new Vector3(-335.2787f, 0.4752745f, 1372.508f),
            new Vector3(-164.7237f, -3.985549f, 1006.097f),
            new Vector3(-180.117f, -3.341475f, 1022.654f),
            new Vector3(-785.0475f, 7.041798f, 1710.269f),
            new Vector3(1469.62f, -4.294517f, 1070.209f),
            new Vector3(2187.362f, -0.6239323f, -454.2437f),
            new Vector3(1426.629f, -4.249069f, 751.5843f)
        };

        public static readonly List<ScreamHand> HandsList = new List<ScreamHand>
        {
            // stairs section
            new ScreamHand {
                orig = HandOrig.MILK,
                parent = HandParent.STAIRS,
                position = new Vector3(-2.92797852f, -0.168999672f, -0.0360107422f),
                euler = new Vector3(9.56607628f, 92.0791168f, 162.548065f),
                scale = 1.5f
            },
            new ScreamHand
            {
                orig = HandOrig.MILK,
                parent = HandParent.STAIRS,
                position = new Vector3(-3.79699707f, -0.388000488f, -0.718017578f),
                euler = new Vector3(60.6711922f, 260.106995f, 104.075432f),
                scale = 1.5f
            },
            new ScreamHand
            {
                orig = HandOrig.MILK,
                parent = HandParent.STAIRS,
                position = new Vector3(-1.89999998f, 0.806999981f, -0.270999998f),
                euler = new Vector3(1.9119761f, 79.2116547f, 52.6669617f),
                scale = 1.5f
            },
            new ScreamHand
            {
                orig = HandOrig.MILK,
                parent = HandParent.STAIRS,
                position = new Vector3(-2.73600006f, 0.188999996f, -0.0260000005f),
                euler = new Vector3(19.481802f, 24.7313232f, 197.737717f),
                scale = 1.5f
            },
            new ScreamHand
            {
                orig = HandOrig.MILK,
                parent = HandParent.STAIRS,
                position = new Vector3(-3.1559999f, 0.414000005f, -1.09399998f),
                euler = new Vector3(338.406006f, 197.075912f, 63.9442787f),
                scale = 1.5f
            },
            // end stairs section

            // house section
            new ScreamHand
            {
                orig = HandOrig.MILK,
                parent = HandParent.HOUSE,
                position = new Vector3(-5.30700684f, 0.831999779f, -1.46496582f),
                euler = new Vector3(287.232086f, 170.030548f, 14.5584507f),
                scale = 2f
            },
            new ScreamHand
            {
                orig = HandOrig.MILK,
                parent = HandParent.HOUSE,
                position = new Vector3(-5.30700684f, 0.831999779f, -1.46496582f),
                euler = new Vector3(287.232086f, 170.030548f, 14.5584507f),
                scale = 2f
            },
            new ScreamHand
            {
                orig = HandOrig.PUSH,
                parent = HandParent.HOUSE,
                position = new Vector3(-0.885009766f, -0.284000397f, 0.158996582f),
                euler = new Vector3(359.39566f, 202.685089f, 87.7484665f),
                scale = 1.5f
            },
            new ScreamHand
            {
                orig = HandOrig.PUSH,
                parent = HandParent.HOUSE,
                position = new Vector3(-4.0090332f, -0.536999702f, 4.76702881f),
                euler = new Vector3(5.85977411f, 18.493206f, 92.2100677f),
                scale = 1.5f
            },
            new ScreamHand
            {
                orig = HandOrig.PUSH,
                parent = HandParent.HOUSE,
                position = new Vector3(-5.9029541f, -0.477999687f, 2.35900879f),
                euler = new Vector3(1.27469838f, 2.89245844f, 88.4191437f),
                scale = 2
            },
            new ScreamHand
            {
                orig = HandOrig.WATCH,
                parent = HandParent.HOUSE,
                position = new Vector3(0.233032227f, -0.222999573f, 1.45800781f),
                euler = new Vector3(325.567657f, 44.3248367f, 5.16276741f),
                scale = 2f
            },
            // end house section
        };

        public static readonly string[] PentaRecipe = new string[5] { "churchcandle", "fernflower", "mushroom", "blackegg", "walnut" };

        public static readonly Dictionary<string, Vector3> NightScreamersPointsPos = new Dictionary<string, Vector3>() // night scream sounds positions
        {
            ["bedroom"] = new Vector3(-2.338177f, 0.03142646f, 12.91463f),
            ["crying_female"] = new Vector3(-6.801387f, 0.1021783f, 6.610903f),
            ["crying_kid"] = new Vector3(-5.944736f, -0.2938192f, 14.34833f),
            ["door_knock"] = new Vector3(-13.04612f, -0.2938216f, 9.959766f),
            ["footsteps"] = new Vector3(-14.68741f, -0.2938224f, 4.410945f),
            ["glass1"] = new Vector3(-8.830304f, 0.4986353f, 4.962998f),
            ["glass2"] = new Vector3(-2.926222f, 0.4986371f, 4.988186f),
            ["kitchen_water"] = new Vector3(-8.391668f, 0.9055675f, 7.271975f),
        };



        public static void InitializeGlobalVars()
        {
            Player = GameObject.Find("PLAYER").transform;

            PlayerHunger = Utils.GetGlobalVariable<FsmFloat>("PlayerHunger");
            PlayerThirst = Utils.GetGlobalVariable<FsmFloat>("PlayerThirst");
            PlayerFatigue = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue");
            PlayerStress = Utils.GetGlobalVariable<FsmFloat>("PlayerStress");
            PlayerMoney = Utils.GetGlobalVariable<FsmFloat>("PlayerMoney");

            GUIuse = Utils.GetGlobalVariable<FsmBool>("GUIuse");
            GUIinteraction = Utils.GetGlobalVariable<FsmString>("GUIinteraction");
            GUIsubtitle = Utils.GetGlobalVariable<FsmString>("GUIsubtitle");

            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            PlayMakerFSM _sun = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Color");
            sunHours = _sun.GetVariable<FsmInt>("Time");
            sunMinutes = _sun.GetVariable<FsmFloat>("Minutes");

            Logic.IsDeadByGame = false;
        }

        public static void InitializeObjects()
        {           
            FullScreenScreamer = GameObject.Instantiate(ResourcesStorage.FullScreenScreamer_prefab);
            FullScreenScreamer.transform.SetParent(GameObject.Find("GUI").transform, false);

            GlobalAmbient_source = SoundManager.AddAudioSource(
                new GameObject("GlobalAmbient(PsychoMod)"),
                ResourcesStorage.GlobalAmbient_clip,
                0.045f
            );

            GlobalPsychoAmbient_source = SoundManager.AddAudioSource(
                new GameObject("GlobalPsychoAmbient(PsychoMod)"),
                ResourcesStorage.GlobalPsychoAmbient_clip,
                0.07f
            );


            GameObject _penta = GameObject.Instantiate(ResourcesStorage.Pentagram_prefab);
            _penta.AddComponent<Pentagram>(); // clone pentagram in dingonbiisi house
            _penta.AddComponent<PentagramEvents>();

            GameObject.Instantiate(ResourcesStorage.RoosterPoster_prefab).AddComponent<AngryRoosterPoster>();
            GameObject.Instantiate(ResourcesStorage.Ferns_prefab).AddComponent<FernFlowerSpawner>(); // clone ferns list
            CrowsList = GameObject.Instantiate(ResourcesStorage.CrowsList_prefab); // clone crows list

            AudioSource _heartbeat = GameObject.Find("PLAYER").AddComponent<AudioSource>(); // attach heartbeat sound to player
            _heartbeat.clip = ResourcesStorage.Heartbeat_clip;
            _heartbeat.loop = true;
            _heartbeat.playOnAwake = false;
            _heartbeat.priority = 128;
            _heartbeat.volume = 1;
            _heartbeat.pitch = 1;
            _heartbeat.panStereo = 0;
            _heartbeat.spatialBlend = 1;
            _heartbeat.reverbZoneMix = 1;
            _heartbeat.dopplerLevel = 1;
            _heartbeat.spread = 0;
            _heartbeat.minDistance = 1.5f;
            _heartbeat.maxDistance = 12f;
            
            Heartbeat_source = _heartbeat;
            _heartbeat.enabled = false;

            SuicidalsList = GameObject.Instantiate(ResourcesStorage.SuicidalsList_prefab); // clone suicidals for horror world
            ObjectCloner.CopySuicidal(SuicidalsList); // copy first suicidal from list for use in night screamer
            SuicidalsList.SetActive(false); // hide suicidals list

            // Load death sound
            AudioSource _src = GameObject.Find("Systems").AddComponent<AudioSource>();
            _src.clip = ResourcesStorage.HeartStop_clip;
            _src.loop = false;
            _src.volume = 1.75f;
            _src.priority = 0;
            SoundManager.DeathSound = _src;

            // add audio source for full screen screamers
            AudioSource _fssss = GameObject.Find("PLAYER").AddComponent<AudioSource>();
            _fssss.clip = SoundManager.FullScreenScreamersSounds[0];
            _fssss.loop = false;
            _fssss.playOnAwake = false;
            _fssss.priority = 1;
            _fssss.volume = 1;
            _fssss.pitch = 1;
            _fssss.panStereo = 0;
            _fssss.spatialBlend = 1;
            _fssss.reverbZoneMix = 1;
            _fssss.dopplerLevel = 1;
            _fssss.spread = 0;
            _fssss.minDistance = 1.5f;
            _fssss.maxDistance = 12f;
            SoundManager.FullScreenScreamerSoundsSource = _fssss;

            GameObject _fittan = GameObject.Find("TRAFFIC/VehiclesDirtRoad/Rally/FITTAN") ?? GameObject.Find("FITTAN");
            GameObject _fittanStore = GameObject.Instantiate(ResourcesStorage.FittanItemsStore_prefab);
            _fittanStore.transform.SetParent(_fittan.transform);
            _fittanStore.transform.localPosition = Vector3.zero; // new Vector3(2, 2, 2);
            _fittanStore.transform.localEulerAngles = Vector3.zero;
            _fittanStore.AddComponent<FittanItemsStore>();
        }

        public static void Reset()
        {
            Player = null;

            sunHours = null;
            sunMinutes = null;

            Notebook = null;
            MailboxSheet = null;
            EnvelopeObject = null;
            CrowsList = null;
            SuicidalsList = null;
            FullScreenScreamer = null;

            Heartbeat_source = null;
            PhantomScream_source = null;
            GlobalAmbient_source = null;
            GlobalPsychoAmbient_source = null;

            ModelsCached.Clear();
            CachedFliesSounds.Clear();
            CachedTextures.Clear();

            SoundManager.DeathSound = null;

            TexturesManager.Cache.Clear();

            SoundManager.FullScreenScreamerSoundsSource = null;
            SoundManager.FullScreenScreamersSounds.Clear();
        }
    }
}