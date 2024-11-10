using UnityEngine;

namespace Psycho.Handlers
{
    [RequireComponent(typeof(Rigidbody))]
    internal sealed class ItemsGravityEnabler : MonoBehaviour
    {
        void Update()
        {
            if (transform.parent != null) return;
            
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            Destroy(this);
        }
    }
}
