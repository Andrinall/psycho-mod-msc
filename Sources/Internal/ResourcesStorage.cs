
using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;


namespace Psycho.Internal
{
    internal static class ResourcesStorage
    {
        public static GameObject Pentagram_prefab = null;
        public static GameObject RoosterPoster_prefab = null;
        public static GameObject Ferns_prefab = null;
        public static GameObject Candle_prefab = null;
        public static GameObject FernFlower_prefab = null;
        public static GameObject Nut_prefab = null;
        public static GameObject BlackEgg_prefab = null;
        public static GameObject Mushroom_prefab = null;
        public static GameObject Walnut_prefab = null;

        public static GameObject Background_prefab = null;
        public static GameObject Pills_prefab = null;
        public static GameObject CrowsList_prefab = null;
        public static GameObject SuicidalsList_prefab = null;
        public static GameObject Picture_prefab = null;
        public static GameObject Coffin_prefab = null;
        public static GameObject CottageMinigame_prefab = null;

        public static GameObject SmokeParticleSystem_prefab = null;
        public static GameObject FullScreenScreamer_prefab = null;

        public static GameObject Notebook_prefab = null;
        public static GameObject NotebookPage_prefab = null;
        public static GameObject NotebookGUI_prefab = null;

        public static GameObject Sketchbook_prefab = null;
        public static GameObject SketchbookGUI_prefab = null;

        public static GameObject Postcard_prefab = null;

        public static Texture NotebookPages_texture = null;
        public static Texture NotebookStartPage_texture = null;
        public static Texture NotebookFinalPage_texture = null;
        public static Texture NewsPaper_texture = null;

        public static AudioClip Heartbeat_clip = null;
        public static AudioClip HeartStop_clip = null;

        public static AudioClip AcidBurn_clip = null;
        public static AudioClip ScreamCall_clip = null;
        public static AudioClip PhantomScream_clip = null;
        public static AudioClip TVScream_clip = null;
        public static AudioClip UncleScream_clip = null;
        public static AudioClip HousekeeperLaughs_clip = null;

        public static AudioClip JokkeSpawned_clip = null;
        public static AudioClip GrannyCrawlScreamer_clip = null;
        public static AudioClip HandDroppedToFace_clip = null;

        public static AudioClip UBV_psy_clip = null;
        public static AudioClip HouseAmbient_clip = null;
        public static AudioClip IslandAmbient_clip = null;
        public static AudioClip DingonbiisiAmbient_clip = null;
        public static AudioClip GlobalAmbient_clip = null;
        public static AudioClip GlobalPsychoAmbient_clip = null;

        public static AudioClip FinishGame_clip = null;


        public static readonly List<Texture> Replaces = new List<Texture> { };
        public static readonly List<Texture> IndependentlyTextures = new List<Texture> { };

        public static readonly List<Texture> MailScreens = new List<Texture> { };
        public static readonly List<Texture> TaroCardsTextures = new List<Texture> { };
        public static readonly List<Texture> Pictures = new List<Texture> { };
        public static readonly List<Texture> SketchbookPages = new List<Texture> { };
        public static readonly List<Texture> FullScreenScreamerTextures = new List<Texture> { };

        public static readonly List<AudioClip> HorrorFliesSounds = new List<AudioClip> { };

        public static readonly Dictionary<int, ModelData> ModelsReplaces = new Dictionary<int, ModelData> { };


        public static void LoadFromBundle(string bundlePath)
        {
            AssetBundle _bundle = LoadAssets.LoadBundle(bundlePath);

            Pentagram_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/penta.prefab");
            RoosterPoster_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/rooster_poster.prefab");
            Ferns_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/ferns.prefab");
            Candle_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/churchcandle.prefab");
            FernFlower_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/fernflower.prefab");
            Nut_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/nut.prefab");
            BlackEgg_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/blackegg.prefab");
            Mushroom_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/mushroom.prefab");
            Walnut_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/walnut.prefab");

            Background_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/background.prefab");
            Pills_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/pills.prefab");
            CrowsList_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/crowslist.prefab");
            SuicidalsList_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/customsuicidals.prefab");
            Coffin_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/coffin.prefab");
            Picture_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/picture.prefab");
            CottageMinigame_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/minigame.prefab");

            SmokeParticleSystem_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/smoke.prefab");
            FullScreenScreamer_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/fullscreenscreamer.prefab");

            Notebook_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/notebook.prefab");
            NotebookPage_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/notebook page.prefab");
            NotebookGUI_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/notebookgui.prefab");

            Sketchbook_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/sketchbook.prefab");
            SketchbookGUI_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/sketchbookgui.prefab");

            Postcard_prefab = LoadAsset<GameObject>(_bundle, "assets/prefabs/postcard.prefab");

            NotebookPages_texture = LoadAsset<Texture>(_bundle, "assets/textures/page(1-13)(notebook).png");
            NotebookStartPage_texture = LoadAsset<Texture>(_bundle, "assets/textures/page14.png");
            NotebookFinalPage_texture = LoadAsset<Texture>(_bundle, "assets/textures/page15(false).png");
            NewsPaper_texture = LoadAsset<Texture>(_bundle, "assets/textures/newspaper.png");

            Heartbeat_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/heartbeat.wav");
            HeartStop_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/heart_stop.wav");

            AcidBurn_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/acid_burn.mp3");
            ScreamCall_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/screamcall.wav");
            PhantomScream_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/phantomscream.mp3");
            TVScream_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/tvscreamer.mp3");
            UncleScream_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/uncle_screamer.mp3");
            HousekeeperLaughs_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/housekeeper_laughs.wav");

            JokkeSpawned_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/jokkespawnedsound.mp3");
            GrannyCrawlScreamer_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/grannycrawlscreamer.mp3");
            HandDroppedToFace_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/handdroppedtoface.mp3");

            UBV_psy_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/ubv_psy.mp3");
            HouseAmbient_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/ambient/home_night_all.mp3");
            IslandAmbient_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/ambient/island_all.mp3");
            DingonbiisiAmbient_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/ambient/penta_all.mp3");
            GlobalAmbient_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/ambient/real_night_emb.mp3");
            GlobalPsychoAmbient_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/ambient/psycho_emb.mp3");

            FinishGame_clip = LoadAsset<AudioClip>(_bundle, "assets/audio/wining.mp3");


            // load all replaces
            Transform _building = GameObject.Find("YARD/Building").transform;
            foreach (string path in _bundle.GetAllAssetNames())
            {
                if (path.Contains("assets/replaces")) // load texture & sound replaces
                {
                    if (path.Contains("/horror"))
                        Replaces.Add(LoadAsset<Texture>(_bundle, path));
                    else if (path.Contains("/sounds")) // replaces for flies sounds in horror world
                        HorrorFliesSounds.Add(LoadAsset<AudioClip>(_bundle, path));
                    else if (path.Contains("/allworlds")) // load texture used independently of world
                        IndependentlyTextures.Add(LoadAsset<Texture>(_bundle, path));
                }
                else if (path.Contains("assets/pictures")) // load textures for picture in frame
                    Pictures.Add(LoadAsset<Texture>(_bundle, path));
                else if (path.Contains("assets/audio/screamers"))
                { // load sounds for night screamer
                    string _item = path.Replace("assets/audio/screamers/", "").Replace(".mp3", "");
                    GameObject _emptyPoint = new GameObject($"ScreamPoint({_item})");
                    AudioSource _source = _emptyPoint.AddComponent<AudioSource>();
                    _source.clip = LoadAsset<AudioClip>(_bundle, path);
                    _source.loop = true;
                    _source.volume = path.Contains("crying") ? 0.4f : 0.9f;
                    _source.priority = 0;
                    _source.rolloffMode = AudioRolloffMode.Logarithmic;
                    _source.minDistance = 1.5f;
                    _source.maxDistance = 12;
                    _source.spatialBlend = 1;
                    _source.spread = 0;
                    _source.dopplerLevel = 1;

                    _emptyPoint.transform.SetParent(_building, worldPositionStays: false);
                    _emptyPoint.transform.position = Globals.NightScreamersPointsPos[_item];
                    SoundManager.ScreamPoints.Add(_source);
                }
                else if (path.Contains("assets/audio/fullscreenscreamers"))
                    SoundManager.FullScreenScreamersSounds.Add(LoadAsset<AudioClip>(_bundle, path));
                else if (path.Contains("screens/"))
                    MailScreens.Add(LoadAsset<Texture>(_bundle, path));
                else if (path.Contains("textures/taro"))
                    TaroCardsTextures.Add(LoadAsset<Texture>(_bundle, path));
                else if (path.Contains("textures/album"))
                    SketchbookPages.Add(LoadAsset<Texture>(_bundle, path));
                else if (path.Contains("textures/fullscreenscreamers"))
                    FullScreenScreamerTextures.Add(LoadAsset<Texture>(_bundle, path));
            }

            // load smoking replaces
            Texture _cig_texture = LoadAsset<Texture>(_bundle, "assets/replaces/smoking/hand.png");
            ModelsReplaces.Add("cigarette_filter".GetHashCode(), new ModelData
            {
                path = "Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Filter",
                mesh = LoadAsset<Mesh>(_bundle, "assets/replaces/smoking/cigarette_filter.obj"),
                texture = _cig_texture
            });

            ModelsReplaces.Add("cigarette_shaft".GetHashCode(), new ModelData
            {
                path = "Armature/Bone/Bone_001/Bone_008/Bone_009/Bone_019/Bone_020/Cigarette/Shaft",
                mesh = LoadAsset<Mesh>(_bundle, "assets/replaces/smoking/cigarette_shaft.obj"),
                texture = _cig_texture
            });

            MailScreens.Sort(delegate (Texture item, Texture target) {
                return (int.Parse(item.name) < int.Parse(target.name)) ? -1 : 0;
            });

            _bundle.Unload(false);
        }


        public static void UnloadAll()
        {
            foreach (var _field in typeof(Globals).GetFields())
            {
                string _fieldName = _field.Name;
                string _fieldType = _field.FieldType.Name;
                if (!_fieldName.Contains("_clip") && !_fieldName.Contains("_prefab") && !_fieldName.Contains("_texture")) continue;

                switch (_fieldType)
                {
                    case "GameObject":
                        Resources.UnloadAsset((GameObject)_field.GetValue(null));
                        _field.SetValue(null, null);
                        break;
                    case "AudioClip":
                        Resources.UnloadAsset((AudioClip)_field.GetValue(null));
                        _field.SetValue(null, null);
                        break;
                    case "Texture":
                        Resources.UnloadAsset((Texture)_field.GetValue(null));
                        _field.SetValue(null, null);
                        break;
                }
            }

            foreach (var _item in ModelsReplaces)
            {
                Resources.UnloadAsset(_item.Value.mesh);
                Resources.UnloadAsset(_item.Value.texture);
            }

            ModelsReplaces.Clear();
            FullScreenScreamerTextures.Clear();
        }


        static T LoadAsset<T>(AssetBundle bundle, string path) where T : UnityEngine.Object
        {
            try
            {
                T _asset = bundle.LoadAsset<T>(path);
                if (_asset == null) throw new NullReferenceException();
                return _asset;
            }
            catch (Exception e)
            {
                ModConsole.Error($"Unable to load asset {path} from embedded resource;\n{e.GetFullMessage()}");
            }
            return null;
        }
    }
}
