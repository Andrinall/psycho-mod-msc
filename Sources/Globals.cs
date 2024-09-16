using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    internal static class Globals
    {
        internal static List<PillsItem> pills_list = new List<PillsItem> { };
        internal static List<Texture> poster_textures = new List<Texture> { };
        internal static List<Texture> mailScreens = new List<Texture> { };
        internal static List<AudioSource> audios = new List<AudioSource> { };
        internal static List<AudioClip> clips = new List<AudioClip> { };
        internal static GameObject background = null;
        internal static GameObject pills = null;
        internal static GameObject poster = null;
        internal static Texture can_texture = null;
        internal static Texture atlas_texture = null;
        internal static Mesh empty_cup = null;
        internal static Mesh coffee_cup = null;
        internal static List<Vector3> pills_positions = new List<Vector3> {
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

        internal static Dictionary<string, string> localization = new Dictionary<string, string>
        {
            ["LOSS_RATE_SPEED"] = "Модификатор скорости пассивного уменьшения",
            ["DEFAULT_DECREASE"] = "Базовое уменьшение адреналина",
            ["SPRINT_INCREASE"] = "Увеличение от бега",
            ["HIGHSPEED_INCREASE"] = "Увеличение от езды на большой скорости",
            ["BROKEN_WINDSHIELD_INCREASE"] = "Увеличение при езде без лобового стекла",
            ["FIGHT_INCREASE"] = "Увеличение во время драки",
            ["WINDOW_BREAK_INCREASE"] = "Увеличение за разбивание окон (магазин, паб)",
            ["HOUSE_BURNING"] = "Увеличение во время пожара в доме",
            ["TEIMO_PISS"] = "Увеличение за обоссывание Теймо",
            ["GUARD_CATCH"] = "Увеличение при попытках охранника поймать игрока",
            ["VENTTI_WIN"] = "Увеличение адреналина при поражениях в игре со свином",
            ["JANNI_PETTERI_HIT"] = "Увеличение за нокаут от NPC (Janni и Petteri)",
            ["TEIMO_SWEAR"] = "Увеличение при ругани Теймо на персонажа",
            ["PISS_ON_DEVICES"] = "Увеличение за обоссывание приборов в доме(TV)",
            ["SPARKS_WIRING"] = "Увеличение при замыкании проводки Satsuma",
            ["SPILL_SHIT"] = "Увеличение при сливе говна в неположенном месте (crime)",
            ["RALLY_PLAYER"] = "Увеличение при участии в ралли",
            ["MURDER_WALKING"] = "Увеличение при приследовании мужиком с топором",
            ["COFFEE_INCREASE"] = "Увеличение от употребления кофе",
            ["ENERGY_DRINK_INCREASE"] = "Увеличение от употребления энергетика",
            ["CRASH_INCREASE"] = "Увеличение за получение урона в аварии",
            ["DRIVEBY_INCREASE"] = "Увеличение при сбитии NPC (зрители ралли)",
            ["MURDERER_THREAT"] = "Увеличение за уклонение от удара топором",
            ["MURDERER_HIT"] = "Увеличение за удар по мужику с топором",
            ["SLEEP_DECREASE"] = "Уменьшение адреналина при сне",
            ["HELMET_DECREASE"] = "Дебафф при езде на скорости со <b>шлемом</b>",
            ["SEATBELT_DECREASE"] = "Дебафф при езде с пристёгнутым ремнём",
            ["SMOKING_DECREASE"] = "Уменьшение от курения",

            ["REQUIRED_SPEED_Jonnez"] = "Мин.скорость для прибавки при езде на Jonezz",
            ["REQUIRED_SPEED_Satsuma"] = "Мин.скорость для прибавки при езде в Satsuma",
            ["REQUIRED_SPEED_Ferndale"] = "Мин.скорость для прибавки при езде в Ferndale",
            ["REQUIRED_SPEED_Hayosiko"] = "Мин.скорость для прибавки при езде в Hayosiko",
            ["REQUIRED_SPEED_Fittan"] = "Мин.скорость для прибавки при езде в Fittan",
            ["REQUIRED_SPEED_Gifu"] = "Мин.скорость для прибавки при езде в Gifu",

            ["REQUIRED_CRASH_SPEED"] = "Мин.скорость для прибавки от аварии",
            ["REQUIRED_WINDSHIELD_SPEED"] = "Мин.скорость для прибавки при езде без лобаша"
        };

        internal static T LoadAsset<T>(AssetBundle bundle, string path) where T : UnityEngine.Object
        {
            try
            {
                var asset = bundle.LoadAsset<T>(path);
                if (asset == null) throw new NullReferenceException();
                return asset;
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to load asset {path} from embedded resource\n{e.GetFullMessage()}");
                return null;
            }
        }

        internal static void LoadAllPosters(AssetBundle bundle)
        {
            foreach (var v in bundle.GetAllAssetNames()) {
                if (!v.Contains("textures/poster")) continue;
                poster_textures.Add(LoadAsset<Texture>(bundle, v));
            }
        }

        internal static void LoadAllScreens(AssetBundle bundle)
        {
            foreach (var v in bundle.GetAllAssetNames())
            {
                if (!v.Contains("screens/")) continue;
                mailScreens.Add(LoadAsset<Texture>(bundle, v));
            }

            // sort screens from 0 to *, for correct display this in the email
            mailScreens.Sort(delegate(Texture item, Texture target) {
                return (Int32.Parse(item.name) < Int32.Parse(target.name)) ? -1 : 0;
            });
        }

        internal static void LoadAllSounds(AssetBundle bundle)
        {
            try
            {
                var player = GameObject.Find("PLAYER");
                var systems = GameObject.Find("Systems");

                foreach (var v in bundle.GetAllAssetNames())
                {
                    if (!v.Contains("audio/heart_")) continue;
                    var clip = LoadAsset<AudioClip>(bundle, v);
                    clips.Add(clip);

                    if (clip.name == "heart_stop")
                    {
                        var src = systems.AddComponent<AudioSource>();
                        src.clip = clip;
                        src.loop = false;
                        src.volume = 1.75f;
                        src.priority = 0;
                        audios.Add(src);
                        Utils.PrintDebug(eConsoleColors.GREEN, $"AudioSource {src.clip.name} created");
                        continue;
                    }

                    var source = player.AddComponent<AudioSource>();
                    source.clip = clip;
                    source.loop = true;
                    source.volume = 2f;
                    audios.Add(source);
                    Utils.PrintDebug(eConsoleColors.GREEN, $"AudioSource {source.clip.name} created");
                }
            }
            catch (Exception e)
            {
                Utils.PrintDebug($"AUDIO loops loading failed\n{e.GetFullMessage()}");
            }
        }
    }
}
