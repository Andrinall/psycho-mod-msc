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
            "Fatigue", "Dirty", "Money", "Jailtime"
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
        GameObject GetElement(string name) => transform.Find(name)?.gameObject;
        GameObject GetElementLocal(string name) => _struct.Find(v => v.name == name);
        
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

        public bool IsElementExist(string name) => _struct.Exists(v =>
        {
            if (v != null) return v.name == name;
            _struct.Remove(v);
            return false;
        });

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

        public void RemoveElement(string name)
        {
            if (!IsElementExist(name)) return;

            GameObject element = GetElementLocal(name);
            _struct.Remove(element);
            Destroy(element);
            Structurize();
        }

        public void HideElement(string name, bool hide)
        {
            if (!IsElementExist(name)) return;
            GetElementLocal(name).SetActive(!hide);
        }

        public void SetElementText(string name, string text)
        {
            if (!IsElementExist(name)) return;
            Transform label = GetElement($"{name}/HUDLabel").transform;
            label.GetComponent<TextMesh>().text = text;
            label.Find("HUDLabelShadow").GetComponent<TextMesh>().text = name;
        }

        public void SetElementColor(string name, Color color)
        {
            if (!IsElementExist(name)) return;
            GetElement($"{name}/Pivot/HUDBar")
                .GetComponent<MeshRenderer>()
                .material.color = color;
        }

        public void SetElementScale(string name, Vector3 scale)
        {
            if (!IsElementExist(name)) return;
            GetElement($"{name}/Pivot").transform.localScale = scale;
        }

        public int GetIndexByName(string name)
            => _struct.FindIndex(v => v.name == name);

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
