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

        public static bool operator ==(NotebookPage o1, NotebookPage o2)
        {
            return o1.index == o2.index;
        }

        public static bool operator !=(NotebookPage o1, NotebookPage o2)
        {
            return o1.index != o2.index;
        }

        public override bool Equals(object other)
        {
            if (!(other is NotebookPage)) return false;
            return (other as NotebookPage).index == this.index;
        }

        public override int GetHashCode()
        {
            return index.GetHashCode()
                ^ (isTruePage.GetHashCode() << 2)
                / (isDefaultPage.GetHashCode() << 4)
                + (isFinalPage.GetHashCode() << 6);
        }
    }


    internal class NotebookPageComponent : MonoBehaviour
    {
        public NotebookPage page;

        NotebookMain notebook;
        TextMesh pageText;
        Material pageTextMat;

        
        void Awake()
        {
            EventsManager.OnLanguageChanged.AddListener(UpdatePageText);
        }

        void OnEnable()
        {
            notebook = GameObject.Find("Notebook(Clone)")?.GetComponent<NotebookMain>();

            Transform text = transform.Find("Text");
            pageText = text?.GetComponent<TextMesh>();

            MeshRenderer renderer = text.GetComponent<MeshRenderer>();
            pageTextMat = renderer.material;
            pageTextMat.shader = Instantiate(Shader.Find("GUI/3D Text Shader"));
            pageTextMat.color = new Color(0.0353f, 0.1922f, 0.3882f);
        }

        void OnDisable() => Destroy(gameObject);

        public void UpdatePageText()
            => pageText.text = Locales.PAGES[page.index - 1, page.isTruePage ? 0 : 1, Globals.CurrentLang];
    }
}
 