
using UnityEngine;


namespace Psycho.Internal
{
    [RequireComponent(typeof(Rigidbody))]
    class ItemsGravityCrutch : MonoBehaviour
    {
        Rigidbody rb;

        void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        void Update()
        {
            if (transform.parent != null) return;
            
            rb.useGravity = true;
            rb.isKinematic = false;

            Destroy(this);
        }
    }
}
