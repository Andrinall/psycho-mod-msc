
using System;

using MSCLoader;
using UnityEngine;

using Psycho.Features;


namespace Psycho.Internal
{
    internal static class SaveManager
    {
        public static bool TryLoad(Mod mod)
        {
            try
            {
                SaveLoadData _data = SaveLoad.DeserializeClass<SaveLoadData>(mod, "psychoData", true);

                if (Logic.GameFinished)
                {
                    Logic.DestroyAllObjects();
                    Utils.FreeResources();
                    return true;
                }

                Logic.IsDead = _data.IsDead;
                Logic.InHorror = _data.InHorror;
                Logic.SetValue(_data.PsychoValue);
                Logic.SetPoints(_data.PsychoPoints);
                Logic.BeerBottlesDrunked = _data.BeerBottlesDrunked;
                Logic.LastDayMinigame = _data.LastDayMinigame;

                var rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
                if (rooster)
                {
                    rooster.Applyed = _data.RoosterPosterApplyed;
                    rooster.LastDayApplyed = _data.RoosterPosterLastDayApplyed;
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

                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
                return true;
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load save data;");
                return false;
            }
        }

        public static void SaveData(Mod mod)
        {
            var rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
            var pills = Globals.Pills?.self?.transform;

            SaveLoad.SerializeClass(mod, new SaveLoadData
            {
                GameFinished = Logic.GameFinished,

                IsDead = Logic.IsDead,
                InHorror = Logic.InHorror,

                PsychoValue = Logic.Value,
                PsychoPoints = Logic.Points,

                BeerBottlesDrunked = Logic.BeerBottlesDrunked,
                LastDayMinigame = Logic.LastDayMinigame,

                RoosterPosterApplyed = rooster?.Applyed ?? false,
                RoosterPosterLastDayApplyed = rooster?.LastDayApplyed ?? 0,

                EnvelopeSpawned = Logic.EnvelopeSpawned,

                PillsPosition = pills == null ? Vector3.zero : pills.position,
                PillsEuler = pills == null ? Vector3.zero : pills.eulerAngles,

                ItemsPoolData = ItemsPool.GetSaveData(),
                NotebookPagesPool = Notebook.Pages
            }, "psychoData", true);
        }

        public static void RemoveData(Mod mod)
            => SaveLoad.DeleteValue(mod, "psychoData");

        /*static string _saveDataPath = Application.persistentDataPath + "\\Psycho.dat";*/

        /*internal static void LoadData()
        {
            // load saved data
            try
            {
                byte[] value = File.ReadAllBytes(_saveDataPath);
                if (value.Length == 0) throw new IndexOutOfRangeException("Bytes length in save file == 0");
                if (value.Length < 30) throw new IndexOutOfRangeException("Bytes length in save file < default length");

                Logic.IsDead = BitConverter.ToBoolean(value, 0);
                Logic.InHorror = BitConverter.ToBoolean(value, 1);
                Logic.GameFinished = BitConverter.ToBoolean(value, 2);
                Logic.EnvelopeSpawned = BitConverter.ToBoolean(value, 3);
                Logic.SetValue(BitConverter.ToSingle(value, 4));
                Logic.SetPoints(BitConverter.ToSingle(value, 8));
                Logic.BeerBottlesDrunked = BitConverter.ToInt32(value, 12);
                Logic.LastDayMinigame = BitConverter.ToInt32(value, 16);
                //Logic.NumberOfSpawnedPages = BitConverter.ToInt32(value, 20);

                var rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
                if (rooster)
                {
                    rooster.Applyed = BitConverter.ToBoolean(value, 24);
                    rooster.LastDayApplyed = BitConverter.ToInt32(value, 25);
                }

                // spawn pills in needed
                if (Logic.InHorror && !Logic.EnvelopeSpawned)
                    Globals.Pills = PillsItem.ReadData(ref value, 29);

                Utils.PrintDebug(eConsoleColors.YELLOW, $"Value:{Logic.Value}; dead:{Logic.IsDead}; env:{Logic.EnvelopeSpawned}; horror:{Logic.InHorror}");
                Utils.PrintDebug(eConsoleColors.YELLOW, $"lastDayMinigame: {Logic.LastDayMinigame}; globalDay: {Utils.GetGlobalVariable<FsmInt>("GlobalDay")}");
                
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

                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
            }
            catch (Exception e)
            {
                ModConsole.Error("Unable to load Save Data, resetting to default");
                Utils.PrintDebug(eConsoleColors.RED, e.GetFullMessage());

                Logic.SetDefaultValues(); // reset data if savedata not loaded

                if (GameObject.Find("Picture(Clone)") == null) // spawn picture frame at default position if needed
                {
                    ItemsPool.AddItem(Globals.Picture_prefab,
                        new Vector3(-10.1421f, 0.2857685f, 6.501729f),
                        new Vector3(0.01392611f, 2.436693f, 89.99937f)
                    );
                }

                if (GameObject.Find("Notebook(Clone)") == null)
                {
                    GameObject Notebook = ItemsPool.AddItem(Globals.Notebook_prefab,
                        new Vector3(-2.007682f, 0.04279194f, 7.669019f),
                        new Vector3(90f, 247.8114f, 0f)
                    );
                    Globals.Notebook = Notebook.AddComponent<Notebook>();
                    Notebook.MakePickable();
                }

            }
            
            if (GameObject.Find("Sketchbook(Clone)") == null)
            {
                GameObject Sketchbook = (GameObject)UnityEngine.Object.Instantiate(Globals.Sketchbook_prefab,
                    new Vector3(-4.179454f, -0.08283298f, 7.728132f),
                    Quaternion.Euler(new Vector3(90f, 44.45397f, 0f))
                );
                Sketchbook.AddComponent<Sketchbook>();
                Sketchbook.MakePickable();
            }
        }*/

        /*internal static void LoadNotebookPages(byte[] value)
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
                        index = idx,
                        isTruePage = isTrue,
                        isFinalPage = isFinal
                    });

                    offset += 6;
                }

                if (!Notebook.Pages.Any(v => v.isFinalPage))
                    Notebook.AddDefaultPage();

                Globals.Notebook?.SortPages();

                Utils.PrintDebug(eConsoleColors.GREEN, "Notebook Pages Loaded!");
                Notebook.Pages.ForEach(v => Utils.PrintDebug(eConsoleColors.YELLOW, $"page[{v.index}]: true? {v.isTruePage}; default? {v.isDefaultPage}; final? {v.isFinalPage}"));
            }
            catch (Exception ex)
            {
                Logic.LastDayMinigame = 0;
                Globals.Notebook?.ClearPages();

                ModConsole.Error("Error while loading notebook pages");
                ModConsole.Error($"{ex.GetFullMessage()}\n{ex.StackTrace}");
            }
        }*/

        /*internal static void SaveNotebookPages(ref byte[] array)
        {
            int itemsPoolSize = ItemsPool.GetSizeInSave(array);
            int offset = ItemsPool.base_offset + itemsPoolSize + 4; // (items pool base offset) + (items pool size) + (spacing)
            int pagesCount = Notebook.Pages.Count - (Notebook.Pages.Any(v => v.isDefaultPage) ? 1 : 0);

            BitConverter.GetBytes(pagesCount).CopyTo(array, offset);
            offset += 4;

            foreach (NotebookPage item in Notebook.Pages)
            {
                if (item.isDefaultPage) continue;

                BitConverter.GetBytes(item.index).CopyTo(array, offset);
                BitConverter.GetBytes(item.isTruePage).CopyTo(array, offset + 4);
                BitConverter.GetBytes(item.isFinalPage).CopyTo(array, offset + 5);
                offset += 6;
            }

            Notebook.Pages.Clear();
        }*/

        /*internal static void SaveData()
        {
            // save data
            byte[] array = new byte[80 + (ItemsPool.Length * 90) + 4 + (6 * Notebook.Pages.Count)];
            // [(values + picture + pills + empty space) + (penta pool len * penta pool alloc)]
            BitConverter.GetBytes(Logic.GameFinished).CopyTo(array, 2);

            if (Logic.GameFinished)
            {
                File.WriteAllBytes(_saveDataPath, array);
                return;
            }

            if (Globals.Pills == null && Logic.InHorror)
                Logic.EnvelopeSpawned = true;

            BitConverter.GetBytes(Logic.IsDead).CopyTo(array, 0);
            BitConverter.GetBytes(Logic.InHorror).CopyTo(array, 1);
            
            BitConverter.GetBytes(Logic.EnvelopeSpawned).CopyTo(array, 3);
            BitConverter.GetBytes(Logic.Value).CopyTo(array, 4);
            BitConverter.GetBytes(Logic.Points).CopyTo(array, 8);
            BitConverter.GetBytes(Logic.BeerBottlesDrunked).CopyTo(array, 12);
            BitConverter.GetBytes(Logic.LastDayMinigame).CopyTo(array, 16);
            //BitConverter.GetBytes(Logic.NumberOfSpawnedPages).CopyTo(array, 20);

            var rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
            if (rooster)
            {
                BitConverter.GetBytes(rooster.Applyed).CopyTo(array, 24);
                BitConverter.GetBytes(rooster.LastDayApplyed).CopyTo(array, 25);
            }

            Utils.PrintDebug($"dead: [{Logic.IsDead}]; horror: [{Logic.InHorror}]; envelope: [{Logic.EnvelopeSpawned}];\nvalue: [{Logic.Value}]; points: [{Logic.Points}]; bottles: [{Logic.BeerBottlesDrunked}]");

            if (Logic.InHorror && !Logic.EnvelopeSpawned)
            {
                Globals.Pills?.WriteData(ref array, 29);
                Globals.Pills = null;
            }

            ItemsPool.Save(ref array);
            SaveNotebookPages(ref array);
            File.WriteAllBytes(_saveDataPath, array);
        }*/

        /*internal static void RemoveFile()
            => File.Delete(_saveDataPath);*/
    }
}
