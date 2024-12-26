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
        public NotebookPage page;

        TextMesh pageText;


        protected override void Awaked()
            => EventsManager.OnLanguageChanged.AddListener(UpdatePageText);

        protected override void Enabled()
        {
            Transform text = transform.Find("Text");
            pageText = text?.GetComponent<TextMesh>();

            MeshRenderer renderer = text.GetComponent<MeshRenderer>();
            Material pageTextMat = renderer.material;
            pageTextMat.shader = Shader.Find("GUI/3D Text Shader");
            pageTextMat.color = new Color(0.0353f, 0.1922f, 0.3882f);
        }

        protected override void Disabled() => Destroy(gameObject);

        public void UpdatePageText()
            => pageText.text = Locales.PAGES[page.index - 1, page.isTruePage ? 0 : 1, Globals.CurrentLang];
    }
}
 