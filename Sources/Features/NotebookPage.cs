
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    class NotebookPage
    {
        public int Index = -1;
        public bool IsTruePage = false;
        public bool IsDefaultPage = false;
        public bool IsFinalPage = false;

        public NotebookPage() { }
        public NotebookPage(NotebookPage parent)
        {
            Index = parent.Index;
            IsTruePage = parent.IsTruePage;
            IsDefaultPage = parent.IsDefaultPage;
            IsFinalPage = parent.IsFinalPage;
        }

        public override string ToString()
            => $"NotebookPage[idx:{Index}; isTrue:{IsTruePage}; isDefault:{IsDefaultPage}; isFinal:{IsFinalPage}]";
    }


    class NotebookPageComponent : CatchedComponent
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

        protected override void Disabled()
        {
            EventsManager.OnLanguageChanged.RemoveListener(UpdatePageText);
            Destroy(gameObject);
        }

        public void UpdatePageText()
            => pageText.text = Locales.PAGES[Page.Index - 1, Page.IsTruePage ? 0 : 1, Globals.CurrentLang];
    }
}
 