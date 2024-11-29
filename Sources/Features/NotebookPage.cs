using UnityEngine;

namespace Psycho.Sources.Features
{
    internal class NotebookPage : MonoBehaviour
    {
        public int currentPageIndex;
        public bool isDefaultPage;
        public bool isFinalPage;
        public bool isTruePage;

        TextMesh textMesh;
        NotebookMain Notebook;

        void Awake()
        {
            Notebook = GameObject.Find("").GetComponent<NotebookMain>();
        }

        public void UpdatePageText()
        {
            int lang = Globals.CurrentLang;

            if (isDefaultPage)
            {
                textMesh.text = Locales.DEFAULT_PAGE[lang];
                return;
            }

            if (isFinalPage)
            {
                bool isTrueFinal = Notebook.CalcTruePages() > 7;
                textMesh.text = Locales.FINAL_PAGE[isTrueFinal ? 0 : 1, lang];
                return;
            }

            textMesh.text = Locales.PAGES[currentPageIndex, isTruePage ? 0 : 1, Globals.CurrentLang];
        }
    }
}
