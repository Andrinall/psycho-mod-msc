using System;
using System.IO;
using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Features;


namespace Psycho.Internal
{
    internal static class SaveManager
    {
        static string _saveDataPath = Application.persistentDataPath + "\\Psycho.dat";

        internal static void LoadData()
        {
            // load saved data
            try
            {
                byte[] value = File.ReadAllBytes(_saveDataPath);
                if (value.Length == 0) throw new IndexOutOfRangeException("Bytes length in save file == 0");
                if (value.Length < 30) throw new IndexOutOfRangeException("Bytes length in save file < default length");

                Logic.isDead = BitConverter.ToBoolean(value, 0);
                Logic.inHorror = BitConverter.ToBoolean(value, 1);
                Logic.envelopeSpawned = BitConverter.ToBoolean(value, 2);
                Logic.SetValue(BitConverter.ToSingle(value, 3));
                Logic.SetPoints(BitConverter.ToSingle(value, 7));
                Logic.BeerBottlesDrunked = BitConverter.ToInt32(value, 11);
                Logic.lastDayMinigame = BitConverter.ToInt32(value, 15);
                Logic.numberOfSpawnedPages = BitConverter.ToInt32(value, 19);

                var rooster = GameObject.Find("rooster_poster(Clone)").GetComponent<AngryRoosterPoster>();
                rooster.Applyed = BitConverter.ToBoolean(value, 23);
                rooster.LastDayApplyed = BitConverter.ToInt32(value, 24);

                // spawn pills in needed
                if (Logic.inHorror && !Logic.envelopeSpawned)
                    Globals.pills = PillsItem.ReadData(ref value, 28);

                Utils.PrintDebug(eConsoleColors.YELLOW, $"Value:{Logic.Value}; dead:{Logic.isDead}; env:{Logic.envelopeSpawned}; horror:{Logic.inHorror}");
                
                if (Logic.isDead)
                {
                    Logic.isDead = false;
                    Logic.inHorror = false;
                    Logic.BeerBottlesDrunked = 0;
                    Logic.numberOfSpawnedPages = 0;
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
        }

        internal static void LoadNotebookPages(byte[] value)
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
                Logic.numberOfSpawnedPages = 0;
                Logic.lastDayMinigame = 0;
                Globals.Notebook?.ClearPages();

                ModConsole.Error("Error while loading notebook pages");
                ModConsole.Error($"{ex.GetFullMessage()}\n{ex.StackTrace}");
            }
        }

        internal static void SaveNotebookPages(ref byte[] array)
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
        }

        internal static void SaveData()
        {
            // save data
            byte[] array = new byte[80 + (ItemsPool.Length * 90) + 4 + (6 * Notebook.Pages.Count)];
            // [(values + picture + pills + empty space) + (penta pool len * penta pool alloc)]

            if (Globals.pills == null && Logic.inHorror)
                Logic.envelopeSpawned = true;

            BitConverter.GetBytes(Logic.isDead).CopyTo(array, 0); // 1
            BitConverter.GetBytes(Logic.inHorror).CopyTo(array, 1); // 1
            BitConverter.GetBytes(Logic.envelopeSpawned).CopyTo(array, 2); // 1
            BitConverter.GetBytes(Logic.Value).CopyTo(array, 3); // 4
            BitConverter.GetBytes(Logic.Points).CopyTo(array, 7); // 4
            BitConverter.GetBytes(Logic.BeerBottlesDrunked).CopyTo(array, 11);
            BitConverter.GetBytes(Logic.lastDayMinigame).CopyTo(array, 15);
            BitConverter.GetBytes(Logic.numberOfSpawnedPages).CopyTo(array, 19);

            var rooster = GameObject.Find("rooster_poster(Clone)").GetComponent<AngryRoosterPoster>();
            BitConverter.GetBytes(rooster.Applyed).CopyTo(array, 23);
            BitConverter.GetBytes(rooster.LastDayApplyed).CopyTo(array, 24);

            Utils.PrintDebug($"dead: [{Logic.isDead}]; horror: [{Logic.inHorror}]; envelope: [{Logic.envelopeSpawned}];\nvalue: [{Logic.Value}]; points: [{Logic.Points}]; bottles: [{Logic.BeerBottlesDrunked}]");

            if (Logic.inHorror && !Logic.envelopeSpawned)
            {
                Globals.pills?.WriteData(ref array, 28);
                Globals.pills = null;
            }

            ItemsPool.Save(ref array);
            SaveNotebookPages(ref array);
            File.WriteAllBytes(_saveDataPath, array);
        }

        internal static void RemoveFile()
            => File.Delete(_saveDataPath);
    }
}
