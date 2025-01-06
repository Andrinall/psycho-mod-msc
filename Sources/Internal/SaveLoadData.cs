

using Psycho.Features;
using System.Collections.Generic;
using UnityEngine;

namespace Psycho.Internal
{
    internal sealed class SaveLoadData
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
        public List<NotebookPage> NotebookPagesPool = default;
    }

    internal sealed class ItemsPoolSaveLoadData
    {
        public string Name = string.Empty;
        public Vector3 Position = Vector3.zero;
        public Vector3 Euler = Vector3.zero;
    }
}
