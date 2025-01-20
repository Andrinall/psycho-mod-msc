
using UnityEngine;


namespace Psycho.Handlers
{
    class ItemRenamer : MonoBehaviour
    {
        public string TargetName;
        public string FinalName;


        void Update()
        {
            if (string.IsNullOrEmpty(TargetName)) return; 
            if (string.IsNullOrEmpty(FinalName)) return;
            if (name != TargetName) return;
            if (name == FinalName) return;

            name = FinalName;
        }
    }
}
