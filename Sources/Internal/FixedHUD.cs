using Psycho.Extensions;
using System.Collections.Generic;

using UnityEngine;


namespace Psycho.Internal
{
    public enum eHUDCloneType { RECT, TEXT }

    public sealed class FixedHUD : MonoBehaviour
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


        private void OnEnable()
        {
            _start = transform.Find("Mortal").localPosition;

            foreach(string element in _default)
            {
                GameObject t = transform.Find(element)?.gameObject;
                if (t == null) continue;
                _struct.Add(t);
            }
        }

        void OnDestroy() => _struct.Clear();

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
        public void AddElement(eHUDCloneType cloneFrom, string name, int index = -1)
        {
            if (name.Length == 0) return;
            if (IsElementExist(name)) return;

            GameObject hudElement = Instantiate(transform.Find(cloneFrom == eHUDCloneType.RECT ? "Hunger" : "Money").gameObject);
            hudElement.name = name;

            Transform label = hudElement.transform.Find("HUDLabel");
            label.GetComponent<TextMesh>().text = name;
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
            Destroy(hudElement.GetComponentInChildren<PlayMakerFSM>());
            hudElement.transform.SetParent(transform, worldPositionStays: false);

            if (index > 0)
                _struct.Insert(index, hudElement);
            else
                _struct.Add(hudElement);

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
        public void AddElement(eHUDCloneType cloneFrom, string name, string parent)
            => AddElement(cloneFrom, name, GetIndexByName(parent));

        /// <summary>
        /// Check if element exists by element name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsElementExist(string name) => _struct.Exists(v =>
        {
            if (v != null) return v.name == name;
            _struct.Remove(v);
            return false;
        });

        /// <summary>
        /// Move element in local storage and change position<br/>
        /// of element based on position in storage
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="index">New index (<) maximum storage size</param>
        public void MoveElement(string name, int index)
        {
            if (index < 0 || index > _struct.Capacity) return;
            if (!IsElementExist(name)) return;

            GameObject element = GetElementLocal(name);
            if (element == null) return;

            _struct.Remove(element);
            _struct.Insert(index, element);
            Structurize();
        }

        /// <summary>
        /// Remove element from storage & HUD
        /// </summary>
        /// <param name="name">Name of element to destroy</param>
        public void RemoveElement(string name)
        {
            if (!IsElementExist(name)) return;

            GameObject element = GetElementLocal(name);
            _struct.Remove(element);
            Destroy(element);
            Structurize();
        }

        /// <summary>
        /// Set selected element active
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="hide">State of visible</param>
        public void HideElement(string name, bool hide)
        {
            if (!IsElementExist(name)) return;
            GetElementLocal(name).SetActive(!hide);
        }

        /// <summary>
        /// Set element TextMesh.text if used eHUDCloneType.TEXT for AddElement
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="text">New element text</param>
        public void SetElementText(string name, string text)
        {
            if (!IsElementExist(name)) return;
            Transform label = GetElement($"{name}/HUDLabel").transform;
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
        public void SetElementColor(string name, Color color)
        {
            if (!IsElementExist(name)) return;
            GameObject element = GetElement($"{name}/Pivot/HUDBar");
            
            if (element == null) return;
            element.GetComponent<MeshRenderer>().material.color = color;
        }

        /// <summary>
        /// Set pivot scale for RECT element.<br/>
        /// Automatically clamped to 0-1 range.
        /// </summary>
        /// <param name="name">Name of element</param>
        /// <param name="scale">New pivot scale</param>
        public void SetElementScale(string name, Vector3 scale)
        {
            if (!IsElementExist(name)) return;
            GameObject pivot = GetElement($"{name}/Pivot");
            if (pivot == null) return;

            pivot.transform.localScale = scale.Clamp(0f, 1f); ;
        }

        /// <summary>
        /// Get element index by element name
        /// </summary>
        /// <param name="name">Name of element in HUD</param>
        /// <returns></returns>
        public int GetIndexByName(string name)
            => _struct.FindIndex(v => v.name == name);

        /// <summary>
        /// Structurize positions of GUI/HUD childs
        /// </summary>
        public void Structurize()
        {
            int inactive_items = 0;

            foreach (GameObject child in _struct)
            {
                if (child?.gameObject == null)
                {
                    _struct.Remove(child);
                    continue;
                }

                if (!child.activeSelf)
                {
                    inactive_items++;
                    continue;
                }

                child.transform.localPosition = new Vector3(_start.x,
                    _start.y - (_struct.IndexOf(child) - inactive_items) * 0.4f
                );
            }

            if (_struct.Count == transform.childCount) return;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child?.gameObject == null) continue;
                if (_blacklisted.Contains(child.name)) continue;
                if (_struct.FindIndex(v => v == child.gameObject) == -1)
                    _struct.Add(child.gameObject);
            }
        }
    }
}
