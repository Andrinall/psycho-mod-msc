using UnityEngine;

namespace Psycho.Internal
{
    internal sealed class ItemRenamer : MonoBehaviour
    {
        internal string TargetName;
        internal string FinalName;


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
