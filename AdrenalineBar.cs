using UnityEngine;

namespace Adrenaline
{
    internal class AdrenalineBar
    {
        private static Material Mat;
        private static Transform HUD;
        private static Transform Bar;

        private const float MIN_ADRENALINE = 0f;
        private const float MAX_ADRENALINE = 2f;

        public static void Setup()
        {
            HUD = GameObject.Find("GUI/HUD").transform;
            Transform clone = Object.Instantiate(HUD.Find("Stress"));
            Transform label = clone.Find("HUDLabel");
            clone.name = "Adrenaline";
            clone.parent = HUD;
            label.GetComponent<TextMesh>().text = "Adrenaline";
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = "Adrenaline";
            Object.Destroy(clone.GetComponentInChildren<PlayMakerFSM>());

            Bar = GameObject.Find("GUI/HUD/Adrenaline/Pivot").transform;
            Mat = Bar.Find("HUDBar").GetComponent<MeshRenderer>().material;
        }

        public static void Set(float value)
        {
            var clamped = Mathf.Clamp(value / 100f, MIN_ADRENALINE, MAX_ADRENALINE);
            Bar.localScale = new Vector3(clamped, 1f);
            if (clamped <= 0.1f || clamped >= 1.75f) Mat.color = Color.red;
            else Mat.color = Color.white;
        }
    }
}
