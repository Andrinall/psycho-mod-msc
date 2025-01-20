
using UnityEngine;

namespace Psycho
{
    static class VectorExtensions
    {
        public static Vector3 Clamp(this Vector3 obj, float min, float max)
            => new Vector3(
                Mathf.Clamp(obj.x, min, max),
                Mathf.Clamp(obj.y, min, max),
                Mathf.Clamp(obj.z, min, max)
            );
    }
}
