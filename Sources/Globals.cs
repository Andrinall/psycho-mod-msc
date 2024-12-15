using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Features;
using Psycho.Internal;
using Psycho.Screamers;


namespace Psycho
{
    public enum HandOrig : byte { PUSH = 0, HELLO = 1, WATCH = 2, MILK = 3 }
    public enum HandParent : byte { MAIN = 0, STAIRS = 1, HOUSE = 2, LOFT = 3 }

    public enum ScreamTimeType : int { SOUNDS = 0, FEAR = 1, PARALYSIS = 2 }
    public enum ScreamSoundType : int { BEDROOM = 0, CRYFEMALE = 1, CRYKID = 2, KNOCK = 3, FOOTSTEPS = 4, GLASS1 = 5, GLASS2 = 6, WATER = 7 }
    public enum ScreamFearType : int { GRANNY = 0, SUICIDAL = 1, WATERKITCHEN = 2, WATERBATHROOM = 3, TV = 4, PHONE = 5 }
    public enum ScreamParalysisType : int { GRANNY = 0, HAND = 1, KESSELI = 2 }

    public struct ModelData
    {
        public string path { get; set; }
        public Mesh mesh { get; set; }
        public Texture texture { get; set; }
    }

    public struct ScreamHand
    {
        public HandOrig orig { get; set; }
        public HandParent parent { get; set; }
        public Vector3 position { get; set; }
        public Vector3 euler { get; set; }
        public float scale { get; set; }
    }


    public static class Globals
    {
        public static readonly List<PillsItem> pills_list = new List<PillsItem> { };
        public static readonly List<Texture> mailScreens = new List<Texture> { };

        public static readonly List<Texture> TaroCards = new List<Texture> { };

        public static readonly Dictionary<int, ModelData> models_cached = new Dictionary<int, ModelData> { };
        public static readonly Dictionary<int, ModelData> models_replaces = new Dictionary<int, ModelData> { };

        public static readonly Dictionary<int, object> cached = new Dictionary<int, object> { };
        public static readonly List<Texture> replaces = new List<Texture> { };
        public static readonly List<Texture> indep_textures = new List<Texture> { };

        public static readonly List<AudioClip> flies_cached = new List<AudioClip> { };
        public static readonly List<AudioClip> horror_flies = new List<AudioClip> { };

        public static readonly List<Texture> pictures = new List<Texture> { };
        
        public static readonly List<Texture> SketchbookPages = new List<Texture> { };

        public static GameObject Pentagram_prefab = null;
        public static GameObject Candle_prefab = null;
        public static GameObject FernFlower_prefab = null;
        public static GameObject Nut_prefab = null;
        public static GameObject BlackEgg_prefab = null;
        public static GameObject Mushroom_prefab = null;
        public static GameObject Walnut_prefab = null;

        public static GameObject Background_prefab = null;
        public static GameObject Pills_prefab = null;
        public static GameObject Crow_prefab = null;
        public static GameObject Picture_prefab = null;
        public static GameObject Coffin_prefab = null;
        public static GameObject Suicidal_prefab = null;
        public static GameObject SmokeParticleSystem_prefab = null;
        public static GameObject CottageMinigame_prefab = null;
        
        public static GameObject Notebook_prefab = null;
        public static GameObject NotebookPage_prefab = null;
        public static GameObject NotebookGUI_prefab = null;

        public static GameObject Sketchbook_prefab = null;
        public static GameObject SketchbookGUI_prefab = null;

        public static GameObject Postcard_prefab = null;

        public static GameObject mailboxSheet = null;
        public static GameObject envelopeObject = null;

        public static GameObject suicidalsList = null;

        public static AudioClip AcidBurn_clip = null;
        public static AudioClip ScreamCall_clip = null;
        public static AudioClip PhantomScream_clip = null;
        public static AudioClip TVScream_clip = null;
        public static AudioClip UncleScream_clip = null;
        public static AudioClip HousekeeperLaughs_clip = null;

        public static AudioSource PhantomScream_source = null;
        public static AudioSource Heartbeat_source = null;

        public static Texture NotebookPages_texture = null;
        public static Texture NotebookStartPage_texture = null;
        public static Texture NotebookFinalPage_texture = null;

        public static Texture NewsPaper_texture = null;

        internal static NotebookMain Notebook = null;

        public static int CurrentLang = 0;

        public static readonly List<Vector3> pills_positions = new List<Vector3> {
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

        public static readonly List<ScreamHand> hands_list = new List<ScreamHand>
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


        public static void LoadAssets(AssetBundle _bundle)
        {
            var pointsPos = new Dictionary<string, Vector3>() // night scream sounds positions
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

            // load resources from bundle
            Pills_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/pills.prefab");
            Candle_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/churchcandle.prefab");
            FernFlower_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/fernflower.prefab");
            Walnut_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/walnut.prefab");
            Nut_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/nut.prefab");
            BlackEgg_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/blackegg.prefab");
            Mushroom_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/mushroom.prefab");

            Background_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/background.prefab");
            Picture_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/picture.prefab");
            Coffin_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/coffin.prefab");
            SmokeParticleSystem_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/smoke.prefab");
            CottageMinigame_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/minigame.prefab");

            Notebook_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/notebook.prefab");
            NotebookPage_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/notebook page.prefab");
            NotebookGUI_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/notebookgui.prefab");

            NotebookPages_texture = LoadAsset<Texture>(_bundle, "assets/textures/page(1-13)(notebook).png");
            NotebookStartPage_texture = LoadAsset<Texture>(_bundle, "assets/textures/page14.png");
            NotebookFinalPage_texture = LoadAsset<Texture>(_bundle, "assets/textures/page15(false).png");

            Sketchbook_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/sketchbook.prefab");
            SketchbookGUI_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/sketchbookgui.prefab");

            Postcard_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/postcard.prefab");

            AcidBurn_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/acid_burn.mp3");
            ScreamCall_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/screamcall.wav");
            PhantomScream_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/phantomscream.mp3");
            TVScream_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/tvscreamer.mp3");
            UncleScream_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/uncle_screamer.mp3");
            HousekeeperLaughs_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/housekeeper_laughs.wav");

            NewsPaper_texture = LoadAsset<Texture>(_bundle, "assets/textures/newspaper.png");

            GameObject penta = GameObject.Instantiate(LoadAsset<GameObject>(_bundle, "assets/prefabs/penta.prefab"));
            penta.AddComponent<Pentagram>(); // clone pentagram in dingonbiisi house
            penta.AddComponent<PentagramEvents>();

            GameObject.Instantiate(LoadAsset<GameObject>(_bundle, "assets/prefabs/rooster_poster.prefab")).AddComponent<AngryRoosterPoster>();
            GameObject.Instantiate(LoadAsset<GameObject>(_bundle, "assets/prefabs/ferns.prefab")).AddComponent<FernFlowerSpawner>(); // clone ferns list
            GameObject.Instantiate(LoadAsset<GameObject>(_bundle, "assets/prefabs/crowslist.prefab")); // clone crows list

            AudioSource heartbeat = GameObject.Find("PLAYER").AddComponent<AudioSource>(); // attach heartbeat sound to player
            heartbeat.clip = LoadAsset<AudioClip>(_bundle, "assets/audio/heartbeat.wav");
            heartbeat.loop = true;
            heartbeat.playOnAwake = false;
            heartbeat.priority = 128;
            heartbeat.volume = 1;
            heartbeat.pitch = 1;
            heartbeat.panStereo = 0;
            heartbeat.spatialBlend = 1;
            heartbeat.reverbZoneMix = 1;
            heartbeat.dopplerLevel = 1;
            heartbeat.spread = 0;
            heartbeat.minDistance = 1.5f;
            heartbeat.maxDistance = 12f;
            
            Heartbeat_source = heartbeat;
            heartbeat.enabled = false;

            suicidalsList = GameObject.Instantiate(LoadAsset<GameObject>(_bundle, "assets/prefabs/customsuicidals.prefab")); // clone suicidals for horror world
            WorldManager.CopySuicidal(suicidalsList); // copy first suicidal from list for use in night screamer
            suicidalsList.SetActive(false); // hide suicidals list

            // load all replaces
            Transform building = GameObject.Find("YARD/Building").transform;
            foreach (string name in _bundle.GetAllAssetNames())
            {
                if (name.Contains("assets/replaces")) // load texture & sound replaces
                {
                    if (name.Contains("/horror"))
                    { // load replaces for horror world
                        replaces.Add(
                            //name.Replace("assets/replaces/horror/", "").Replace(".png", "").ToLower().GetHashCode(),
                            LoadAsset<Texture>(_bundle, name)
                        );
                    }
                    else if (name.Contains("/sounds")) // replaces for flies sounds in horror world
                        horror_flies.Add(LoadAsset<AudioClip>(_bundle, name));
                    else if (name.Contains("/allworlds")) // load texture used independently of world
                        indep_textures.Add(
                            //name.Replace("assets/replaces/allworlds/", "").Replace(".png", "").ToLower().GetHashCode(),
                            LoadAsset<Texture>(_bundle, name)
                        );
                }
                else if (name.Contains("assets/pictures")) // load textures for picture in frame
                    pictures.Add(LoadAsset<Texture>(_bundle, name));
                else if (name.Contains("assets/audio/screamers"))
                { // load sounds for night screamer
                    string item = name.Replace("assets/audio/screamers/", "").Replace(".mp3", "");
                    GameObject emptyPoint = new GameObject($"ScreamPoint({item})");
                    AudioSource source = emptyPoint.AddComponent<AudioSource>();
                    source.clip = LoadAsset<AudioClip>(_bundle, name);
                    source.loop = true;
                    source.volume = name.Contains("crying") ? 0.4f : 0.9f;
                    source.priority = 0;
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                    source.minDistance = 1.5f;
                    source.maxDistance = 12;
                    source.spatialBlend = 1;
                    source.spread = 0;
                    source.dopplerLevel = 1;

                    if (!name.Contains("door_knock") && !name.Contains("kitchen_water"))
                        emptyPoint.AddComponent<ScreamSoundDistanceChecker>();

                    emptyPoint.transform.SetParent(building, worldPositionStays: false);
                    emptyPoint.transform.position = pointsPos[item];
                    SoundManager.ScreamPoints.Add(source);
                }
                else if (name.Contains("screens/"))
                    mailScreens.Add(LoadAsset<Texture>(_bundle, name));
                else if (name.Contains("textures/taro"))
                    TaroCards.Add(LoadAsset<Texture>(_bundle, name));
                else if (name.Contains("textures/album"))
                    SketchbookPages.Add(LoadAsset<Texture>(_bundle, name));
            }

            mailScreens.Sort(delegate (Texture item, Texture target) {
                return (int.Parse(item.name) < int.Parse(target.name)) ? -1 : 0;
            });

            // load smoking replaces
            Texture cig_texture = LoadAsset<Texture>(_bundle, "assets/replaces/smoking/hand.png");
            models_replaces.Add("cigarette_filter".GetHashCode(), new ModelData
            {
                path = "Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Filter",
                mesh = LoadAsset<Mesh>(_bundle, "assets/replaces/smoking/cigarette_filter.obj"),
                texture = cig_texture
            });

            models_replaces.Add("cigarette_shaft".GetHashCode(), new ModelData
            {
                path = "Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Shaft",
                mesh = LoadAsset<Mesh>(_bundle, "assets/replaces/smoking/cigarette_shaft.obj"),
                texture = cig_texture
            });

            // Load death sound
            AudioSource src = GameObject.Find("Systems").AddComponent<AudioSource>();
            src.clip = LoadAsset<AudioClip>(_bundle, "assets/audio/heart_stop.wav");
            src.loop = false;
            src.volume = 1.75f;
            src.priority = 0;
            SoundManager.DeathSound = src;
        }

        static T LoadAsset<T>(AssetBundle bundle, string path) where T : UnityEngine.Object
        {
            try
            {
                T asset = bundle.LoadAsset<T>(path);
                if (asset == null) throw new NullReferenceException();
                return asset;
            }
            catch (Exception e)
            {
                ModConsole.Error($"Unable to load asset {path} from embedded resource;\n{e.GetFullMessage()}");
            }
            return null;
        }
    }
}