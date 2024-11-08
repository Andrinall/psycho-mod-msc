using UnityEngine;

namespace Psycho.Internal
{
    public class ItemRenamer : MonoBehaviour
    {
        public string TargetName;
        public string FinalName;

        void Update()
        {
            if (string.IsNullOrEmpty(TargetName)) return; 
            if (string.IsNullOrEmpty(FinalName)) return;
            if (name != TargetName) return;
            if (name == FinalName) return;

            SetName(FinalName);
        }

        public virtual void SetName(string _name)
            => name = _name;
    }
}
