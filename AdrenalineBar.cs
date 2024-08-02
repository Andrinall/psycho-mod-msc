using UnityEngine;

namespace Adrenaline
{
    internal class AdrenalineBar
    {
        internal static Material Mat;
        internal static Transform HUD;
        internal static Transform Bar;

        public static void Init()
        {
            HUD = GameObject.Find("GUI/HUD").transform;
            HUD.Find("Money").transform.localPosition = new Vector3(-11.5f, 6.4f);
            Transform clone = Object.Instantiate(HUD.Find("Stress"));
            Transform label = clone.Find("HUDLabel");
            clone.name = "Adrenaline";
            clone.parent = HUD;
            label.GetComponent<TextMesh>().text = "Adrenaline";
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = "Adrenaline";
            clone.localPosition = new Vector3(-11.5f, 6.8f);
            Object.Destroy(clone.GetComponentInChildren<PlayMakerFSM>());

            Bar = GameObject.Find("GUI/HUD/Adrenaline/Pivot").transform;
            Mat = Bar.Find("HUDBar").GetComponent<MeshRenderer>().material;

            Mat.color = Color.white;
        }

        public static void Set(int value)
        {
            Bar.localScale = new Vector3(Mathf.Clamp(value / 100f, 0f, 1f), 1f);
        }
    }
}
