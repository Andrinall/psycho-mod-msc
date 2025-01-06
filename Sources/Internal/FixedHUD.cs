
using System.Collections.Generic;

using UnityEngine;


namespace Psycho.Internal
{
    public enum eHUDCloneType { RECT, TEXT }

    public sealed class FixedHUD : CatchedComponent
    {
        readonly List<GameObject> _struct = new List<GameObject>();
        readonly List<string> _blacklisted = new List<string> {
            "FPS", "Clutch", "Clutch 1", "DRMpink", "SpeedyHUD"
        };
        readonly List<string> _default = new List<string> {
            "Mortal", "Day", "Thrist",
            "Hunger", "Stress", "Urine",
            "Fatigue", "Dirty", "Health", "Money", "Jailtime"
        };

        Vector3 _start = Vector3.zero;

        public static FixedHUD instance { get; private set; } = null;

        protected override void Awaked()
        {
            instance = this;
        }

        protected override void Enabled()
        {
            _start = transform.Find("Mortal").localPosition;

            foreach(string element in _default)
            {
                GameObject t = transform.Find(element)?.gameObject;
                if (t == null) continue;
                _struct.Add(t);
            }
        }

        protected override void Destroyed()
        {
            instance = null;
            _struct.Clear();
        }

        /// <summary>
        /// Get element in "GUI/HUD/..."
        /// </summary>
        /// <param name="name">GameObject element name</param>
        /// <returns>GameObject which finds or null</returns>
        GameObject GetElement(string name) => transform.Find(name)?.gameObject;

        /// <summary>
        /// Get element in class local storage (constructed from sorted "GUI/HUD")<br/>
        /// </summary>
        /// <param name="name">GameObject element name</param>
        /// <returns>GameObject which finds or null</returns>
        GameObject GetElementLocal(string name) => _struct.Find(v => v.name == name);

        /// <summary>
        /// Add element to "GUI/HUD" && local storage, cloned from selected parent
        /// </summary>
        /// <param name="cloneFrom">Clone RECT or TEXT field of HUD ?</param>
        /// <param name="name">Name for new HUD element</param>
        /// <param name="index">This parameter selects a parent.<br/>Use GetIndexByName(string name) for this, where name is element name in HUD</param>
        /// <example>
        /// GameObject guiHud = GameObject.Find("GUI/HUD");
        /// FixedHUD hud = guiHud.GetComponent<FixedHUD>() ?? guiHud.AddComponent<FixedHUD>();
        /// hud.AddElement(eHUDCloneType.RECT, "NewHUDRect", hud.GetIndexByName("Money"));
        /// hud.Structurize();
        /// </example>
        public static void AddElement(eHUDCloneType cloneFrom, string name, int index = -1)
        {
            if (instance == null) return;
            if (name.Length == 0) return;
            if (IsElementExist(name)) return;

            GameObject hudElement = Instantiate(instance.transform.Find(cloneFrom == eHUDCloneType.RECT ? "Hunger" : "Money").gameObject);
            hudElement.name = name;

            Transform label = hudElement.transform.Find("HUDLabel");
            label.GetComponent<TextMesh>().text = name;
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
            Destroy(hudElement.GetComponentInChildren<PlayMakerFSM>());
            hudElement.transform.SetParent(instance.transform, worldPositionStays: false);

            if (index > 0)
                instance._struct.Insert(index, hudElement);
            else
                instance._struct.Add(hudElement);

            Structurize();
        }

        /// <summary>
        /// Add element to GUI/HUD && local storage
        /// </summary>
        /// <param name="cloneFrom">Clone RECT or TEXT field of HUD ?</param>
        /// <param name="name">Name for new HUD element</param>
        /// <param name="parent">This parameter selects a parent (use name of element in HUD)</param>
        /// <example>
        /// GameObject guiHud = GameObject.Find("GUI/HUD");
        /// FixedHUD hud = guiHud.GetComponent<FixedHUD>() ?? guiHud.AddComponent<FixedHUD>();
        /// hud.AddElement(eHUDCloneType.RECT, "NewHUDRect", "Money");
        /// hud.Structurize();
        /// </example>
        public static void AddElement(eHUDCloneType cloneFrom, string name, string parent)
        {
            if (instance == null) return;
            AddElement(cloneFrom, name, GetIndexByName(parent));
        }

        /// <summary>
        /// Check if element exists by element name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsElementExist(string name)
        {
            if (instance == null) return false;

            return instance._struct.Exists(v =>
            {
                if (v != null) return v.name == name;
                instance._struct.Remove(v);
                return false;
            });
        }

        /// <summary>
        /// Move element in local storage and change position<br/>
        /// of element based on position in storage
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="index">New index (<) maximum storage size</param>
        public static void MoveElement(string name, int index)
        {
            if (instance == null) return;
            if (index < 0 || index > instance._struct.Capacity) return;
            if (!IsElementExist(name)) return;

            GameObject element = instance.GetElementLocal(name);
            if (element == null) return;

            instance._struct.Remove(element);
            instance._struct.Insert(index, element);
            Structurize();
        }

        /// <summary>
        /// Remove element from storage & HUD
        /// </summary>
        /// <param name="name">Name of element to destroy</param>
        public static void RemoveElement(string name)
        {
            if (instance == null) return;
            if (!IsElementExist(name)) return;

            GameObject element = instance.GetElementLocal(name);
            instance._struct.Remove(element);
            Destroy(element);
            Structurize();
        }

        /// <summary>
        /// Set selected element active
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="hide">State of visible</param>
        public static void HideElement(string name, bool hide)
        {
            if (instance == null) return;
            if (!IsElementExist(name)) return;
            instance.GetElementLocal(name)?.SetActive(!hide);
        }

        /// <summary>
        /// Set element TextMesh.text if used eHUDCloneType.TEXT for AddElement
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="text">New element text</param>
        public static void SetElementText(string name, string text)
        {
            if (instance == null) return;
            if (!IsElementExist(name)) return;
            Transform label = instance.GetElement($"{name}/HUDLabel").transform;
            TextMesh mesh = label.GetComponent<TextMesh>();
            
            if (mesh == null) return;
            mesh.text = text;
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
        }

        /// <summary>
        /// Set element color if used eHUDCloneType.RECT for AddElement
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="color">New element color</param>
        public static void SetElementColor(string name, Color color)
        {
            if (instance == null) return;
            if (!IsElementExist(name)) return;
            GameObject element = instance.GetElement($"{name}/Pivot/HUDBar");
            
            if (element == null) return;
            element.GetComponent<MeshRenderer>().material.color = color;
        }

        /// <summary>
        /// Set pivot scale for RECT element.<br/>
        /// Automatically clamped to 0-1 range.
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="scale">New pivot scale</param>
        public static void SetElementScale(string name, Vector3 scale)
        {
            if (instance == null) return;
            if (!IsElementExist(name)) return;
            GameObject pivot = instance.GetElement($"{name}/Pivot");
            if (pivot == null) return;

            pivot.transform.localScale = scale.Clamp(0f, 1f); ;
        }

        /// <summary>
        /// Get element index by element name
        /// </summary>
        /// <param name="name">Name of element in HUD</param>
        /// <returns></returns>
        public static int GetIndexByName(string name)
            => instance?._struct?.FindIndex(v => v.name == name) ?? -1;

        /// <summary>
        /// Structurize positions of GUI/HUD childs
        /// </summary>
        public static void Structurize()
        {
            if (instance == null) return;
            int inactive_items = 0;

            var temp = new List<GameObject>(instance._struct);
            foreach (GameObject child in temp)
            {
                if (child?.gameObject == null)
                {
                    instance._struct.Remove(child);
                    continue;
                }

                if (!child.activeSelf)
                {
                    inactive_items++;
                    continue;
                }

                child.transform.localPosition = new Vector3(instance._start.x,
                    instance._start.y - (instance._struct.IndexOf(child) - inactive_items) * 0.4f
                );
            }

            if (instance._struct.Count == instance.transform.childCount) return;
            for (int i = 0; i < instance.transform.childCount; i++)
            {
                Transform child = instance.transform.GetChild(i);
                if (child?.gameObject == null) continue;
                if (instance._blacklisted.Contains(child.name)) continue;
                if (instance._struct.FindIndex(v => v == child.gameObject) == -1)
                    instance._struct.Add(child.gameObject);
            }
        }
    }
}
