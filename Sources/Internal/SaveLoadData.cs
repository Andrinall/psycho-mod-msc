
using System.Collections.Generic;

using UnityEngine;

using Psycho.Features;


namespace Psycho.Internal
{
    class SaveLoadData
    {
        public bool GameFinished = false;

        public bool IsDead = false;
        public bool InHorror = false;

        public float PsychoValue = 100f;
        public float PsychoPoints = 0f;

        public int BeerBottlesDrunked = 0;
        public int LastDayMinigame = 0;

        public bool RoosterPosterApplyed = false;
        public int RoosterPosterLastDayApplyed = 0;

        public bool EnvelopeSpawned = false;

        public Vector3 PillsPosition = Vector3.zero;
        public Vector3 PillsEuler = Vector3.zero;

        public List<ItemsPoolSaveLoadData> ItemsPoolData = default;
        public Dictionary<int, NotebookPage> NotebookPagesPool = default;
    }

    class ItemsPoolSaveLoadData
    {
        public string Name = string.Empty;
        public Vector3 Position = Vector3.zero;
        public Vector3 Euler = Vector3.zero;
    }
}
