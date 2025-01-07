
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    internal class NotebookPage
    {
        public int index = -1;
        public bool isTruePage = false;
        public bool isDefaultPage = false;
        public bool isFinalPage = false;

        public NotebookPage() { }
        public NotebookPage(NotebookPage parent)
        {
            index = parent.index;
            isTruePage = parent.isTruePage;
            isDefaultPage = parent.isDefaultPage;
            isFinalPage = parent.isFinalPage;
        }

        public override string ToString()
            => $"NotebookPage[idx:{index}; isTrue:{isTruePage}; isDefault:{isDefaultPage}; isFinal:{isFinalPage}]";
    }


    internal class NotebookPageComponent : CatchedComponent
    {
        public NotebookPage Page;

        TextMesh pageText;


        protected override void Awaked()
            => EventsManager.OnLanguageChanged.AddListener(UpdatePageText);

        protected override void Enabled()
        {
            Transform _text = transform.Find("Text");
            pageText = _text?.GetComponent<TextMesh>();

            MeshRenderer _renderer = _text.GetComponent<MeshRenderer>();
            Material _pageTextMat = _renderer.material;
            _pageTextMat.shader = Shader.Find("GUI/3D Text Shader");
            _pageTextMat.color = new Color(0.0353f, 0.1922f, 0.3882f);
        }

        protected override void Disabled() => Destroy(gameObject);

        public void UpdatePageText()
            => pageText.text = Locales.PAGES[Page.index - 1, Page.isTruePage ? 0 : 1, Globals.CurrentLang];
    }
}
 