
using UnityEngine;

using Psycho.Handlers;
using Psycho.Internal;


namespace Psycho.Features
{
    class Postcard
    {
        public static GameObject self = null;

        public static void Initialize()
        {
            if (self != null) return;

            GameObject _cloned = ItemsPool.AddItem(ResourcesStorage.Postcard_prefab);
            _cloned.transform.SetParent(GameObject.Find("YARD/PlayerMailBox").transform, false);
            _cloned.AddComponent<ItemsGravityCrutch>();

            Shader _text3D = Shader.Instantiate(Shader.Find("GUI/3D Text Shader"));
            Transform _text = _cloned.transform.Find("Text");

            Material _material = _text.GetComponent<MeshRenderer>().material;
            _material.shader = _text3D;
            _material.color = new Color(0.0353f, 0.1922f, 0.3882f);

            TextMesh _textMesh = _text.GetComponent<TextMesh>();
            _textMesh.text = Locales.POSTCARD_TEXT[Globals.CurrentLang];

            self = _cloned;
        }

        public static void Initialize(GameObject cloned)
        {
            if (self != null) return;

            Shader _text3D = Shader.Instantiate(Shader.Find("GUI/3D Text Shader"));
            Transform _text = cloned.transform.Find("Text");

            Material _material = _text.GetComponent<MeshRenderer>().material;
            _material.shader = _text3D;
            _material.color = new Color(0.0353f, 0.1922f, 0.3882f);

            TextMesh _textMesh = _text.GetComponent<TextMesh>();
            _textMesh.text = Locales.POSTCARD_TEXT[Globals.CurrentLang];

            self = cloned;

            cloned.AddComponent<ItemsGravityCrutch>();
        }
    }
}
