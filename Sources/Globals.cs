using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;


namespace Psycho
{
    public static class Globals
    {
        public static List<PillsItem> pills_list = new List<PillsItem> { };
        public static List<Texture> mailScreens = new List<Texture> { };

        public static Dictionary<int, ModelData> models_cached = new Dictionary<int, ModelData> { };
        public static Dictionary<int, ModelData> models_replaces = new Dictionary<int, ModelData> { };

        public static Dictionary<int, object> cached = new Dictionary<int, object> { };
        public static Dictionary<int, Texture> replaces = new Dictionary<int, Texture> { };
        public static Dictionary<int, Texture> indep_textures = new Dictionary<int, Texture> { };

        public static List<AudioClip> flies_cached = new List<AudioClip> { };
        public static List<AudioClip> horror_flies = new List<AudioClip> { };

        public static List<Texture> pictures = new List<Texture> { };

        public static GameObject Background_prefab = null;
        public static GameObject Pills_prefab = null;
        public static GameObject Crow_prefab = null;
        public static GameObject Picture_prefab = null;
        public static GameObject Coffin_prefab = null;
        public static GameObject Suicidal_prefab = null;
        public static GameObject SmokeParticleSystem_prefab = null;
        public static GameObject mailboxSheet = null;
        public static GameObject envelopeObject = null;
        public static AudioClip  AcidBurnSound = null;

        public static List<Vector3> pills_positions = new List<Vector3> {
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

        public static List<Vector3[]> crows_positions = new List<Vector3[]> {
            new Vector3[2]{ new Vector3(-7.516591f, 4.490543f, 8.677464f), new Vector3(0.003401852f, 87.02156f, -0.0002028467f) },
            new Vector3[2]{ new Vector3(1.856678f, 9.4264f, 7.493578f), new Vector3(17.93412f, 274.3768f, 10.45374f) },
            new Vector3[2]{ new Vector3(58.8f, 3.197987f, -70.876f), new Vector3(349.5564f, 87.5478f, 0.6662462f) },
            new Vector3[2]{ new Vector3(32.70638f, 6.566193f, -41.20472f), new Vector3(0.006775226f, 172.4616f, 0.002704575f) },
            new Vector3[2]{ new Vector3(751.0732f, -2.711799f, -348.1628f), new Vector3(23.28657f, 145.5104f, 12.25799f) },
            new Vector3[2]{ new Vector3(1866.657f, -0.583223f, -810.8883f), new Vector3(1.390312f, 143.5693f, 12.79265f) },
            new Vector3[2]{ new Vector3(1923.895f, 9.179598f, -411.1647f), new Vector3(359.99f, 297.3772f, 0.001026476f) },
            new Vector3[2]{ new Vector3(1952.774f, 13.97002f, -229.0258f), new Vector3 (0.01081815f, 232.413f, 0.0006151822f) },
            new Vector3[2]{ new Vector3(1594.176f, 8.469599f, 658.263f), new Vector3(-4.64268E-05f, 343.1817f, 0.0001089085f) },
            new Vector3[2]{ new Vector3(1578.88f, 10.73002f, 653.0276f), new Vector3(-0.0001219711f, 348.5435f, 3.264716E-05f) },
            new Vector3[2]{ new Vector3(1551.507f, 7.826491f, 735.2065f), new Vector3(350.2102f, 28.64273f, 347.3693f) },
            new Vector3[2]{ new Vector3(1366.415f, 15.09954f, 798.6128f), new Vector3(0.03065068f, 297.5476f, -0.001512694f) },
            new Vector3[2]{ new Vector3(462.1749f, 9.42584f, 1317.943f), new Vector3 (-0.00287517f, 265.5215f, -0.004583559f) },
            new Vector3[2]{ new Vector3(-1548.195f, 7.158126f, 1178.941f), new Vector3 (354.6178f, 247.7073f, 26.39358f) },
            new Vector3[2]{ new Vector3 (-1429.306f, 8.151156f, 1147.188f), new Vector3 (0.9512718f, 154.2885f, 29.43955f) },
            new Vector3[2]{ new Vector3 (-1525.027f, 9.647693f, 1341.555f), new Vector3 (354.4455f, 245.7671f, 2.359069f) },
            new Vector3[2]{ new Vector3 (-1535.116f, 8.121346f, 1259.436f), new Vector3 (354.5245f, 272.0313f, 9.901129f) }
        };


        public static T LoadAsset<T>(AssetBundle bundle, string path) where T : UnityEngine.Object
        {
            try
            {
                var asset = bundle.LoadAsset<T>(path);
                if (asset == null) throw new NullReferenceException();
                return asset;
            }
            catch (Exception e) {
                ModConsole.Error($"Unable to load asset {path} from embedded resource;\n{e.GetFullMessage()}");
            }
            return null;
        }

        public static void LoadAllScreens(AssetBundle bundle)
        {
            // load all screens
            foreach (var v in bundle.GetAllAssetNames())
            {
                if (!v.Contains("screens/")) continue;
                mailScreens.Add(LoadAsset<Texture>(bundle, v));
            }

            // sort screens from 0 to *, for correct display this in the letter
            mailScreens.Sort(delegate(Texture item, Texture target) {
                return (int.Parse(item.name) < int.Parse(target.name)) ? -1 : 0;
            });
        }
    }
}