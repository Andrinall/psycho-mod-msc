using MSCLoader;
using UnityEngine;

namespace Adrenaline
{
    internal class AdrenalineLogic : MonoBehaviour
    {
        public static float value = 100f;
        public static float lossRate = 1f;

        public void OnEnable()
        {

        }

        public void FixedUpdate()
        {
            value -= 0.51f * lossRate * Time.fixedDeltaTime;
            AdrenalineBar.Set(value);
            ModConsole.Print(value.ToString());

            if (value <= 0)
                Adrenaline.Kill();
            if (value >= 200)
                Adrenaline.Kill();
        }

        public static void Set(float value_)
        {
            AdrenalineBar.Set(value_);
            value = value_;
        }
    }
}
