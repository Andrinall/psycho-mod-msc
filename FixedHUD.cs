using UnityEngine;
using System.Collections.Generic;
using System;

namespace Adrenaline
{
    internal enum eHUDCloneType { RECT, TEXT }
    internal class FixedHUD : MonoBehaviour
    {
        private readonly List<GameObject> _struct = new List<GameObject>();
        private readonly List<string> _blacklisted = new List<string> {
            "FPS", "Clutch", "Clutch 1", "DRMpink", "SpeedyHUD"
        };
        private readonly List<string> _default = new List<string> {
            "Mortal", "Day", "Thrist",
            "Hunger", "Stress", "Urine",
            "Fatigue", "Dirty", "Money", "Jailtime"
        };

        private Vector3 _start = Vector3.zero;

        private void OnEnable()
        {
            _start = base.transform.Find("Mortal").localPosition;

            foreach(string element in _default)
            {
                GameObject t = base.transform.Find(element)?.gameObject;
                if (t == null) continue;
                _struct.Add(t);
            }
        }
        
        public void AddElement(eHUDCloneType cloneFrom, string name, int index = -1)
        {
            if (name.Length == 0) return;

            GameObject hudElement = Instantiate(transform.Find(cloneFrom == eHUDCloneType.RECT ? "Hunger" : "Money").gameObject);
            hudElement.name = name;

            Transform label = hudElement.transform.Find("HUDLabel");
            label.GetComponent<TextMesh>().text = name;
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
            Destroy(hudElement.GetComponentInChildren<PlayMakerFSM>());
            hudElement.transform.SetParent(base.transform, worldPositionStays: false);

            Insert(hudElement, index);
            Structurize();
        }

        public bool IsElementExist(string name)
        {
            return _struct.Exists(v => v.name == name);
        }

        public GameObject GetElement(string name)
        {
            return base.transform.Find(name)?.gameObject;
        }

        public void MoveElement(string name, int index)
        {
            if (index < 0 || index > _struct.Capacity) return;

            GameObject temp = _struct.Find(v => v.name == name);
            if (temp?.gameObject == null) return;

            _struct.Remove(temp);
            _struct.Insert(index, temp);
            Structurize();
        }

        public void RemoveElement(string name)
        {
            if (!IsElementExist(name)) return;
            GameObject element = _struct.Find(v => v.name == name);
            _struct.Remove(element);
            Destroy(element);
            Structurize();
        }

        public void HideElement(string name, bool hide)
        {
            _struct.Find(v => v.name == name)?.SetActive(!hide);
        }

        public void SetElementText(string name, string text)
        {
            Transform label = _struct.Find(v => v.name == name).transform.Find("HUDLabel");
            label.GetComponent<TextMesh>().text = text;
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
        }

        public void SetElementColor(string name, Color color)
        {
            _struct.Find(v => v.name == name)
                .transform.Find("Pivot/HUDBar")
                .GetComponent<MeshRenderer>()
                .material.color = color;
        }

        public void SetElementScale(string name, Vector3 scale)
        {
            base.transform.Find(name + "/Pivot").localScale = scale;
        }

        public int GetIndexByName(string name)
        {
            return _struct.FindIndex(v => v.name == name);
        }

        private void Insert(GameObject element, int index = -1)
        {
            if (element?.gameObject == null) return;
            if (index > 0) _struct.Insert(index, element);
            else _struct.Add(element);
            
            Utils.PrintDebug(String.Format(
                "Element %s inserted into _struct with index %d",
                element.name, index
            ));
        }

        private void Structurize()
        {
            int inactive_items = 0;

            foreach (var child in _struct)
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

            if (_struct.Count == base.transform.childCount - 1) return;
            for (int i = 0; i < base.transform.childCount - 1; i++)
            {
                var child = base.transform.GetChild(i);
                if (child?.gameObject == null) continue;
                if (_blacklisted.Contains(child.name)) continue;
                if (_struct.FindIndex(v => v == child.gameObject) == -1)
                    _struct.Add(child.gameObject);
            }
        }
    }
}
