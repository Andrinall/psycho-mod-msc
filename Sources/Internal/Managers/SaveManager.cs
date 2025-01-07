
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
                    _rooster.Applyed = _data.RoosterPosterApplyed;
                    _rooster.LastDayApplyed = _data.RoosterPosterLastDayApplyed;
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
            catch
            {
                ModConsole.Print("Unable to load save data");
                return false;
            }
        }

        public static void SaveData(Mod mod)
        {
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

                RoosterPosterApplyed = _rooster?.Applyed ?? false,
                RoosterPosterLastDayApplyed = _rooster?.LastDayApplyed ?? 0,

                EnvelopeSpawned = Logic.EnvelopeSpawned,

                PillsPosition = _pills == null ? Vector3.zero : _pills.position,
                PillsEuler = _pills == null ? Vector3.zero : _pills.eulerAngles,

                ItemsPoolData = ItemsPool.GetSaveData(),
                NotebookPagesPool = Notebook.Pages
            }, "psychoData", true);
        }

        public static void RemoveData(Mod mod)
            => SaveLoad.DeleteValue(mod, "psychoData");
    }
}
