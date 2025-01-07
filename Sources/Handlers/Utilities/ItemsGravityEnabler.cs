
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    [RequireComponent(typeof(Rigidbody))]
    internal sealed class ItemsGravityEnabler : CatchedComponent
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
