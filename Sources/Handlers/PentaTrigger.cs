using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class PentaTrigger : MonoBehaviour
    {
        internal GameObject Item = null;
        internal bool IsItemIn = false;

        GameObject player;
        GameObject mover;

        void Awake()
        {
            player = GameObject.Find("PLAYER");
            mover = GameObject.Find("MAP/Buildings/DINGONBIISI/.../.../Mover");
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsItemIn) return;
            if (!_checkObjects(other)) return;

            IsItemIn = true;
            Item = other.gameObject;
        }

        void OnTriggerExit(Collider other)
        {

        }

        bool _checkObjects(Collider other)
            => other.gameObject.name.Contains("(penta_item)");
    }
}
