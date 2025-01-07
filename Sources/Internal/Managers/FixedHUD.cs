
using System.Collections.Generic;

using UnityEngine;


namespace Psycho.Internal
{
    public enum eHUDCloneType { RECT, TEXT }

    public sealed class FixedHUD : CatchedComponent
    {
        readonly List<GameObject> LocalStruct = new List<GameObject>();
        readonly List<string> Blacklisted = new List<string> {
            "FPS", "Clutch", "Clutch 1", "DRMpink", "SpeedyHUD"
        };
        readonly List<string> DefaultElements = new List<string> {
            "Mortal", "Day", "Thrist",
            "Hunger", "Stress", "Urine",
            "Fatigue", "Dirty", "Health", "Money", "Jailtime"
        };

        Vector3 startPosition = Vector3.zero;

        static FixedHUD instance = null;

        protected override void Awaked()
        {
            instance = this;
        }

        protected override void Enabled()
        {
            startPosition = transform.Find("Mortal").localPosition;

            foreach(string element in DefaultElements)
            {
                GameObject t = transform.Find(element)?.gameObject;
                if (t == null) continue;
                LocalStruct.Add(t);
            }
        }

        protected override void Destroyed()
        {
            instance = null;
            LocalStruct.Clear();
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
        GameObject GetElementLocal(string name) => LocalStruct.Find(v => v.name == name);

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

            GameObject _hudElement = Instantiate(instance.transform.Find(cloneFrom == eHUDCloneType.RECT ? "Hunger" : "Money").gameObject);
            _hudElement.name = name;

            Transform _label = _hudElement.transform.Find("HUDLabel");
            _label.GetComponent<TextMesh>().text = name;
            _label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
            Destroy(_hudElement.GetComponentInChildren<PlayMakerFSM>());
            _hudElement.transform.SetParent(instance.transform, worldPositionStays: false);

            if (index > 0)
                instance.LocalStruct.Insert(index, _hudElement);
            else
                instance.LocalStruct.Add(_hudElement);

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

            return instance.LocalStruct.Exists(v =>
            {
                if (v != null) return v.name == name;
                instance.LocalStruct.Remove(v);
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
            if (index < 0 || index > instance.LocalStruct.Capacity) return;
            if (!IsElementExist(name)) return;

            GameObject _element = instance.GetElementLocal(name);
            if (_element == null) return;

            instance.LocalStruct.Remove(_element);
            instance.LocalStruct.Insert(index, _element);
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

            GameObject _element = instance.GetElementLocal(name);
            instance.LocalStruct.Remove(_element);
            Destroy(_element);
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
            Transform _label = instance.GetElement($"{name}/HUDLabel").transform;
            TextMesh _mesh = _label.GetComponent<TextMesh>();
            
            if (_mesh == null) return;
            _mesh.text = text;
            _label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
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
            GameObject _element = instance.GetElement($"{name}/Pivot/HUDBar");
            
            if (_element == null) return;
            _element.GetComponent<MeshRenderer>().material.color = color;
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
            GameObject _pivot = instance.GetElement($"{name}/Pivot");
            if (_pivot == null) return;

            _pivot.transform.localScale = scale.Clamp(0f, 1f); ;
        }

        /// <summary>
        /// Get element index by element name
        /// </summary>
        /// <param name="name">Name of element in HUD</param>
        /// <returns></returns>
        public static int GetIndexByName(string name)
            => instance?.LocalStruct?.FindIndex(v => v.name == name) ?? -1;

        /// <summary>
        /// Structurize positions of GUI/HUD childs
        /// </summary>
        public static void Structurize()
        {
            if (instance == null) return;
            int _inactiveItems = 0;

            var _clonedStruct = new List<GameObject>(instance.LocalStruct);
            foreach (GameObject child in _clonedStruct)
            {
                if (child?.gameObject == null)
                {
                    instance.LocalStruct.Remove(child);
                    continue;
                }

                if (!child.activeSelf)
                {
                    _inactiveItems++;
                    continue;
                }

                child.transform.localPosition = new Vector3(instance.startPosition.x,
                    instance.startPosition.y - (instance.LocalStruct.IndexOf(child) - _inactiveItems) * 0.4f
                );
            }

            if (instance.LocalStruct.Count == instance.transform.childCount) return;
            for (int i = 0; i < instance.transform.childCount; i++)
            {
                Transform child = instance.transform.GetChild(i);
                if (child?.gameObject == null) continue;
                if (instance.Blacklisted.Contains(child.name)) continue;
                if (instance.LocalStruct.FindIndex(v => v == child.gameObject) == -1)
                    instance.LocalStruct.Add(child.gameObject);
            }
        }
    }
}
