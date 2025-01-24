
using System;
using System.IO;
using System.Linq;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Features;
using System.Security.Cryptography;


namespace Psycho.Internal
{
    static class SaveManager
    {
        public static bool TryLoad(Mod mod)
        {
            try
            {
                if (File.Exists(_saveDataPath))
                {
                    Utils.PrintDebug(eConsoleColors.YELLOW, "Try loading old save data");
                    OldSaveLoaded = LoadData();
                    
                    Utils.PrintDebug(eConsoleColors.YELLOW, $"Load old save data; Status - {OldSaveLoaded}");
                    if (OldSaveLoaded)
                        return true;

                    Utils.PrintDebug(eConsoleColors.YELLOW, "Try loading new save data");
                }

                SaveLoadData _data = SaveLoad.DeserializeClass<SaveLoadData>(mod, "globalData", true);

                if (Logic.GameFinished)
                {
                    Logic.DestroyAllObjects();
                    ResourcesStorage.UnloadAll();
                    Globals.Reset();
                    return true;
                }

                Logic.IsDead = _data.IsDead;
                Logic.InHorror = _data.InHorror;
                Logic.SetValue(_data.PsychoValue);
                Logic.SetPoints(_data.PsychoPoints);
                Logic.BeerBottlesDrunked = _data.BeerBottlesDrunked;
                Logic.LastDayMinigame = _data.LastDayMinigame;

                var _rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
                if (_rooster)
                {
                    AngryRoosterPoster.Applyed = _data.RoosterPosterApplyed;
                    AngryRoosterPoster.LastDayApplyed = _data.RoosterPosterLastDayApplyed;
                }

                Logic.EnvelopeSpawned = _data.EnvelopeSpawned;

                if (Logic.InHorror && !Logic.EnvelopeSpawned)
                    Globals.Pills = new PillsItem(_data.PillsPosition, _data.PillsEuler);

                if (Logic.IsDead)
                {
                    Logic.IsDead = false;
                    Logic.InHorror = false;
                    Logic.BeerBottlesDrunked = 0;
                    Logic.ResetValue();
                    Logic.SetPoints(0);
                    Utils.PrintDebug(eConsoleColors.YELLOW, "Player is DEAD in Psycho mod; resetting progress.");
                }
                else
                {
                    ItemsPool.LoadData(_data.ItemsPoolData);
                    Notebook.Pages = _data.NotebookPagesPool;
                }

                ModConsole.Print($"[{mod.Name}{{{mod.Version}}}]: <color=green>Save Data Loaded!</color>");
                return true;
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Unable to load save data.\nWill be created after saving.");
                return false;
            }
        }
        
        public static void SaveData(Mod mod)
        {
            if (OldSaveLoaded)
                File.Delete(_saveDataPath);

            var _rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
            var _pills = Globals.Pills?.self?.transform;

            SaveLoad.SerializeClass(mod, new SaveLoadData
            {
                GameFinished = Logic.GameFinished,

                IsDead = Logic.IsDead,
                InHorror = Logic.InHorror,

                PsychoValue = Logic.Value,
                PsychoPoints = Logic.Points,

                BeerBottlesDrunked = Logic.BeerBottlesDrunked,
                LastDayMinigame = Logic.LastDayMinigame,

                RoosterPosterApplyed = AngryRoosterPoster.Applyed,
                RoosterPosterLastDayApplyed = AngryRoosterPoster.LastDayApplyed,

                EnvelopeSpawned = Logic.EnvelopeSpawned,

                PillsPosition = _pills?.position ?? Vector3.zero,
                PillsEuler = _pills?.eulerAngles ?? Vector3.zero,

                ItemsPoolData = ItemsPool.GetSaveData(),
                NotebookPagesPool = Notebook.Pages
            }, "globalData", true);

            ModConsole.Log($"[{mod.Name}{{{mod.Version}}}]: <color=green>Save Data saved!</color>");
        }

        public static void RemoveData(Mod mod)
            => SaveLoad.DeleteValue(mod, "globalData");


        /// ===================================
        ///       BACKWARD COMPABILITY
        /// ===================================

        static string _saveDataPath = Application.persistentDataPath + "\\Psycho.dat";
        public static bool OldSaveLoaded = false;

        public static bool LoadData()
        {
            // load saved data
            try
            {
                byte[] value = File.ReadAllBytes(_saveDataPath);
                if (value.Length == 0) throw new IndexOutOfRangeException("Bytes length in save file == 0");
                if (value.Length < 30) throw new IndexOutOfRangeException("Bytes length in save file < default length");

                Logic.IsDead = BitConverter.ToBoolean(value, 0);
                Logic.InHorror = BitConverter.ToBoolean(value, 1);
                Logic.EnvelopeSpawned = BitConverter.ToBoolean(value, 2);
                Logic.SetValue(BitConverter.ToSingle(value, 3));
                Logic.SetPoints(BitConverter.ToSingle(value, 7));
                Logic.BeerBottlesDrunked = BitConverter.ToInt32(value, 11);
                Logic.LastDayMinigame = BitConverter.ToInt32(value, 15);

                var rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
                if (rooster)
                {
                    AngryRoosterPoster.Applyed = BitConverter.ToBoolean(value, 23);
                    AngryRoosterPoster.LastDayApplyed = BitConverter.ToInt32(value, 24);
                }

                // spawn pills in needed
                if (Logic.InHorror && !Logic.EnvelopeSpawned)
                    Globals.Pills = PillsItem.ReadData(ref value, 28);

                Utils.PrintDebug($"Value:{Logic.Value}; dead:{Logic.IsDead}; env:{Logic.EnvelopeSpawned}; horror:{Logic.InHorror}");
                Utils.PrintDebug($"lastDayMinigame: {Logic.LastDayMinigame}; globalDay: {Utils.GetGlobalVariable<FsmInt>("GlobalDay")}");
                
                if (Logic.IsDead)
                {
                    Logic.IsDead = false;
                    Logic.InHorror = false;
                    Logic.BeerBottlesDrunked = 0;
                    Logic.ResetValue();
                    Logic.SetPoints(0);
                }
                else
                {
                    ItemsPool.Load(value);
                    LoadNotebookPages(value);
                }

                if (GameObject.Find("Sketchbook(Clone)") == null)
                {
                    GameObject Sketchbook = (GameObject)UnityEngine.Object.Instantiate(ResourcesStorage.Sketchbook_prefab,
                        new Vector3(-4.179454f, -0.08283298f, 7.728132f),
                        Quaternion.Euler(new Vector3(90f, 44.45397f, 0f))
                    );
                    Sketchbook.AddComponent<Sketchbook>();
                    Sketchbook.MakePickable();
                }

                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
                return true;
            }
            catch (Exception e)
            {
                ModConsole.Warning("Unable to load Save Data, resetting to default");

                Logic.SetDefaultValues(); // reset data if savedata not loaded

                if (GameObject.Find("Picture(Clone)") == null) // spawn picture frame at default position if needed
                {
                    ItemsPool.AddItem(ResourcesStorage.Picture_prefab,
                        new Vector3(-10.1421f, 0.2857685f, 6.501729f),
                        new Vector3(0.01392611f, 2.436693f, 89.99937f)
                    );
                }

                if (GameObject.Find("Notebook(Clone)") == null)
                {
                    GameObject Notebook = ItemsPool.AddItem(ResourcesStorage.Notebook_prefab,
                        new Vector3(-2.007682f, 0.04279194f, 7.669019f),
                        new Vector3(90f, 247.8114f, 0f)
                    );
                    Globals.Notebook = Notebook.AddComponent<Notebook>();
                    Notebook.MakePickable();
                }

                if (GameObject.Find("Sketchbook(Clone)") == null)
                {
                    GameObject Sketchbook = (GameObject)UnityEngine.Object.Instantiate(ResourcesStorage.Sketchbook_prefab,
                        new Vector3(-4.179454f, -0.08283298f, 7.728132f),
                        Quaternion.Euler(new Vector3(90f, 44.45397f, 0f))
                    );
                    Sketchbook.AddComponent<Sketchbook>();
                    Sketchbook.MakePickable();
                }

                return false;
            }
        }

        public static void LoadNotebookPages(byte[] value)
        {
            Notebook.Pages.Clear();

            try
            {
                int itemsPoolSize = ItemsPool.GetSizeInSave(value);
                int offset = ItemsPool.base_offset + itemsPoolSize + 4; // (items pool base offset) + (items pool size) + (spacing)
                int pagesCount = BitConverter.ToInt32(value, offset);
                offset += 4;

                for (int i = 0; i < pagesCount; i++)
                {
                    int idx = BitConverter.ToInt32(value, offset);
                    if (idx <= 0 || idx > 15) continue;

                    bool isTrue = BitConverter.ToBoolean(value, offset + 4);
                    bool isFinal = BitConverter.ToBoolean(value, offset + 5);

                    bool result = Notebook.TryAddPage(new NotebookPage
                    {
                        Index = idx,
                        IsTruePage = isTrue,
                        IsFinalPage = isFinal
                    });

                    offset += 6;
                }

                if (!Notebook.Pages.Values.Any(v => v.IsFinalPage))
                    Notebook.AddDefaultPage();
                else
                {
                    if (GameObject.Find("Postcard(Clone)") == null)
                        Postcard.Initialize();
                }

                Utils.PrintDebug(eConsoleColors.GREEN, "Notebook Pages Loaded!");

                foreach (var _page in Notebook.Pages.Values)
                    Utils.PrintDebug($"page[{_page.Index}]: true? {_page.IsTruePage}; default? {_page.IsDefaultPage}; final? {_page.IsFinalPage}");
            }
            catch (Exception ex)
            {
                Logic.LastDayMinigame = 0;
                Globals.Notebook?.ClearPages();

                Utils.PrintDebug(eConsoleColors.RED, "Error while loading notebook pages.");
                ModConsole.Error($"{ex.GetFullMessage()}\n{ex.StackTrace}");
            }
        }
    }
}
