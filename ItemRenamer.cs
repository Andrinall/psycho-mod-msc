using UnityEngine;

namespace Adrenaline
{
    internal class ItemRenamer : MonoBehaviour
    {
        public string TargetName = "";
        public string FinalName;

        private void Update()
        {
            if (base.name != TargetName) return;
            if (base.name == FinalName) return;

            base.name = FinalName;
        }
    }
}
