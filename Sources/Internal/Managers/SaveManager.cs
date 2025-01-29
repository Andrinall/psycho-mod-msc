
using MSCLoader;
using UnityEngine;

using Psycho.Features;


namespace Psycho.Internal
{
    static class SaveManager
    {
        public static bool Load(Mod mod)
        {
            try
            {
                SaveLoadData _data = SaveLoad.DeserializeClass<SaveLoadData>(mod, "globalData", true);

                if (Logic.GameFinished)
                {
                    Logic.DestroyAllObjects();
                    ResourcesStorage.UnloadAll();
                    Globals.Reset();
                    return true;
                }
#if DEBUG
                Utils.PrintDebug(eConsoleColors.YELLOW, _data.ToString());
#endif
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

                if (Logic.IsDead)
                {
                    Logic.SetDefaultValues();
                    Utils.PrintDebug(eConsoleColors.YELLOW, "Player is DEAD in Psycho mod; resetting progress.");
                }
                else
                {
                    ItemsPool.LoadData(_data.ItemsPoolData);
                    Notebook.Pages = _data.NotebookPagesPool;
                    
                    if (Logic.InHorror && !Logic.EnvelopeSpawned && _data.PillsIndex != -1)
                        PillsItem.TryCreatePills(_data.PillsIndex, _data.PillsPosition, _data.PillsEuler);

                    if (Logic.InHorror && _data.PillsIndex == -1)
                        Logic.EnvelopeSpawned = true;

                    if (!Logic.InHorror && _data.PillsIndex != -1)
                        PillsItem.Reset();
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
            var _rooster = GameObject.Find("rooster_poster(Clone)")?.GetComponent<AngryRoosterPoster>();
            var _pills = PillsItem.Self?.transform;

            SaveLoadData _data = new SaveLoadData
            {
                PsychoValue = Logic.Value,
                PsychoPoints = Logic.Points,

                InHorror = Logic.InHorror,

                IsDead = Logic.IsDead,
                GameFinished = Logic.GameFinished,

                BeerBottlesDrunked = Logic.BeerBottlesDrunked,
                LastDayMinigame = Logic.LastDayMinigame,

                RoosterPosterApplyed = AngryRoosterPoster.Applyed,
                RoosterPosterLastDayApplyed = AngryRoosterPoster.LastDayApplyed,

                EnvelopeSpawned = Logic.EnvelopeSpawned,

                PillsIndex = PillsItem.Index,
                PillsPosition = _pills?.position ?? Vector3.zero,
                PillsEuler = _pills?.eulerAngles ?? Vector3.zero,

                ItemsPoolData = ItemsPool.GetSaveData(),
                NotebookPagesPool = Notebook.Pages
            };

            SaveLoad.SerializeClass(mod, _data, "globalData", true);
#if DEBUG
            Utils.PrintDebug(_data.ToString());
#endif
            ModConsole.Log($"[{mod.Name}{{{mod.Version}}}]: <color=green>Save Data saved!</color>");
        }

        public static void RemoveData(Mod mod)
            => SaveLoad.DeleteValue(mod, "globalData");
    }
}
