
using System.Collections.Generic;

using UnityEngine;

using Psycho.Features;


namespace Psycho.Internal
{
    class SaveLoadData
    {
        public float PsychoValue = 100f;
        public float PsychoPoints = 0f;

        public bool InHorror = false;

        public bool IsDead = false;
        public bool GameFinished = false;

        public int BeerBottlesDrunked = 0;
        public int LastDayMinigame = 0;

        public bool RoosterPosterApplyed = false;
        public int RoosterPosterLastDayApplyed = 0;
         
        public bool EnvelopeSpawned = false;
        
        public int PillsIndex = -1;
        public Vector3 PillsPosition = Vector3.zero;
        public Vector3 PillsEuler = Vector3.zero;

        public List<ItemsPoolSaveLoadData> ItemsPoolData = default;
        public Dictionary<int, NotebookPage> NotebookPagesPool = default;

        public override string ToString()
            => $"v[{PsychoValue}];p[{PsychoPoints}];ih[{InHorror}];id[{IsDead}];gf[{GameFinished}];bbd[{BeerBottlesDrunked}];ldm[{LastDayMinigame}];rpa[{RoosterPosterApplyed}];rplda[{RoosterPosterLastDayApplyed}];es[{EnvelopeSpawned}];pi[{PillsIndex}];pp[{PillsPosition}];pe[{PillsEuler}];ipd[{ItemsPoolData}];npp[{NotebookPagesPool}]";
    }

    class ItemsPoolSaveLoadData
    {
        public string Name = string.Empty;
        public Vector3 Position = Vector3.zero;
        public Vector3 Euler = Vector3.zero;

        public override string ToString() => $"n[{Name}];p[{Position}];e[{Euler}]";
    }
}
