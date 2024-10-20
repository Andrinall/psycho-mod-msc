using UnityEngine;

namespace Psycho
{
    internal class ItemRenamer : MonoBehaviour
    {
        public string TargetName = "";
        public string FinalName;

        void Update()
        {
            if (name != TargetName) return;
            if (name == FinalName) return;

            name = FinalName;
        }
    }
}
