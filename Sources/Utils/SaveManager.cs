using System;
using System.IO;
using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Features;


namespace Psycho.Internal
{
    internal sealed class SaveManager
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
                GameObject.Find("rooster_poster(Clone)")
                    .GetComponent<AngryRoosterPoster>().Applyed = BitConverter.ToBoolean(value, 11);

                Globals.LoadPool(value, 48);

                Utils.PrintDebug($"Value:{Logic.Value}; dead:{Logic.isDead}; env:{Logic.envelopeSpawned}; horror:{Logic.inHorror}");
                if (Logic.isDead)
                {
                    Logic.isDead = false;
                    Logic.ResetValue();
                    Logic.SetPoints(0);
                }

                if (!Logic.inHorror || Logic.envelopeSpawned)
                    goto SkipLoadPills;

                // spawn pills in needed
                PillsItem item = new PillsItem(0);
                item.ReadData(ref value, 12);
                item.self.SetActive(Logic.inHorror);
                Globals.pills_list.Add(item);

            SkipLoadPills:
                Utils.PrintDebug(eConsoleColors.GREEN, "Save Data Loaded!");
            }
            catch (Exception e)
            {
                ModConsole.Error("<color=red>Unable to load Save Data, resetting to default</color>");
                Utils.PrintDebug(eConsoleColors.RED, e.GetFullMessage());

                Logic.SetDefaultValues(); // reset data if savedata not loaded

                if (GameObject.Find("Picture(Clone)") == null) // spawn picture frame at default position if needed
                {
                    Globals.AddPentaItem(Globals.Picture_prefab,
                        new Vector3(-10.1421f, 0.2857685f, 6.501729f),
                        new Vector3(0.01392611f, 2.436693f, 89.99937f)
                    );
                }
            }
        }

        internal static void SaveData()
        {
            // save data
            byte[] array = new byte[80 + (Globals.penta_pool.Count * 90)];
            // [(values + picture + pills + empty space) + (penta pool len * penta pool alloc)]

            BitConverter.GetBytes(Logic.isDead).CopyTo(array, 0); // 1
            BitConverter.GetBytes(Logic.inHorror).CopyTo(array, 1); // 1
            BitConverter.GetBytes(Logic.envelopeSpawned).CopyTo(array, 2); // 1
            BitConverter.GetBytes(Logic.Value).CopyTo(array, 3); // 4
            BitConverter.GetBytes(Logic.Points).CopyTo(array, 7); // 4
            BitConverter.GetBytes(GameObject.Find("rooster_poster(Clone)").GetComponent<AngryRoosterPoster>().Applyed).CopyTo(array, 11);

            Globals.SavePool(ref array, 48);

            if (!Logic.inHorror || Logic.envelopeSpawned)
            {
                File.WriteAllBytes(_saveDataPath, array);
                return;
            }

            Globals.pills_list.First()?.WriteData(ref array, 12);
            File.WriteAllBytes(_saveDataPath, array);
        }

        internal static void RemoveFile()
            => File.Delete(_saveDataPath);
    }
}
