using MSCLoader;
using System.Collections.Generic;
using UnityEngine;

namespace Adrenaline
{
    internal class AdrenalineBar
    {
        internal static Material Mat;
        internal static Transform HUD;
        internal static Transform Bar;

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
            clone.localPosition = new Vector3(-11.5f, 6.8f);
            Object.Destroy(clone.GetComponentInChildren<PlayMakerFSM>());

            Bar = GameObject.Find("GUI/HUD/Adrenaline/Pivot").transform;
            Mat = Bar.Find("HUDBar").GetComponent<MeshRenderer>().material;

            Mat.color = Color.white;
        }

        private void StructurizeHUD()
        {
            List<string> labels = new List<string> { "Hunger", "Urine", "Stress", "Dirtiness", "Adrenaline" };
            if (ModLoader.IsModPresent("")) labels.Add("Temperature");
            if (ModLoader.IsModPresent("")) labels.Add("Health");
        }

        public static void Set(int value)
        {
            Bar.localScale = new Vector3(Mathf.Clamp(value / 100f, MIN_ADRENALINE, MAX_ADRENALINE), 1f);
        }
    }
}
