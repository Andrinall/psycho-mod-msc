
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Features
{
    internal sealed class Notebook : BookWithGUI
    {
        public static List<NotebookPage> Pages = new List<NotebookPage>()
        {
            new NotebookPage { index = 14, isDefaultPage = true }
        };

        protected override GameObject GUIPrefab => ResourcesStorage.NotebookGUI_prefab;

        TextMesh notebookGUIText;
        TextMesh notebookGUIPage;

        Transform itemPivot;
        PlayMakerFSM handFsm;

        Transform ItemInHand => itemPivot.childCount > 0 ? itemPivot.GetChild(0) : null;

        int prevMax = 0;

        protected override void Destroyed() => MAX_PAGE = -1;

        protected override void Enabled() => MAX_PAGE = prevMax;

        protected override void Disabled()
        {
            prevMax = MAX_PAGE;
            MAX_PAGE = -1;
        }

        protected override void AfterAwake()
        {
            itemPivot = Utils.GetGlobalVariable<FsmGameObject>("ItemPivot").Value.transform;
            handFsm = itemPivot.parent.Find("Hand").GetPlayMaker("PickUp");

            notebookGUIText = GUI.transform.Find("Text").GetComponent<TextMesh>();
            notebookGUIPage = GUI.transform.Find("PageIndicator").GetComponent<TextMesh>();

            MAX_PAGE = Pages.Count - 1;
            CurrentPage = MAX_PAGE;

            EventsManager.OnLanguageChanged.AddListener(UpdatePageText);
        }

        protected override void ObjectUsed()
        {
            Transform _item = ItemInHand;

            if (_item != null && _item?.name?.Contains("Notebook Page") == true)
            {
                handFsm.CallGlobalTransition("DROP_PART");
                AddNewPage(_item.gameObject);
            }
            else
            {
                base.ObjectUsed();
            }
        }

        protected override void PageSelected(bool next)
        {
            UpdatePageText();
            Utils.PrintDebug($"CurrentPage: {CurrentPage}; MAX_PAGE :{MAX_PAGE}");
        }

        protected override void GUIOpened()
        {
            UpdatePageText();
            base.GUIOpened();
        }

        public static bool TryAddPage(NotebookPage page)
        {
            if (IsPageExists(page.index)) return false;

            Pages.Add(page);
            return true;
        }

        public static bool IsPageExists(int index)
            => Pages.Any(v => v.index == index);

        public void UpdatePageText()
        {
            if (CurrentPage < 0 || CurrentPage >= Pages.Count) return;

            NotebookPage _page = Pages.ElementAt(CurrentPage);
            if (_page == null) return;

            bool _isTrueEnding = _page.isTruePage;
            if (_page.index == 15) // final page
            {
                notebookGUIText.text = Locales.FINAL_PAGE[_isTrueEnding ? 0 : 1, Globals.CurrentLang];
                Background.SetTexture("_MainTex",
                    _isTrueEnding ? ResourcesStorage.NotebookPages_texture : ResourcesStorage.NotebookFinalPage_texture
                );
            }
            else if (_page.index == 14) // default
            {
                notebookGUIText.text = Locales.DEFAULT_PAGE[Globals.CurrentLang];
                Background.SetTexture("_MainTex", ResourcesStorage.NotebookStartPage_texture);
            }
            else if (_page.index < 14)
            {
                notebookGUIText.text = Locales.PAGES[_page.index - 1, _isTrueEnding ? 0 : 1, Globals.CurrentLang];
                Background.SetTexture("_MainTex", ResourcesStorage.NotebookPages_texture);
            }

            notebookGUIPage.text = $"Page {_page.index}";
            MAX_PAGE = Pages.Count - 1;
        }

        bool AddNewPage(GameObject pageObj)
        {
            if (pageObj == null) return false;
            if (Pages.Count > 13) return false;

            NotebookPageComponent _newPage = pageObj.GetComponent<NotebookPageComponent>();
            NotebookPage _page = new NotebookPage(_newPage.Page);
            Destroy(pageObj);

            Pages.Add(_page);
            CreateFinalPage();
            SortPages();

            PlayPageTurn();
            Utils.PrintDebug($"{_page} added into notebook");
            return true;
        }


        public int GetCountOfTruePages()
            => Pages.Count(v => !v.isFinalPage && v.isTruePage);

        public void ClearPages()
        {
            Pages.Clear();
            AddDefaultPage();
            MAX_PAGE = 0;
            CurrentPage = MAX_PAGE;
        }

        public void SortPages()
        {
            Pages.Sort((v, t) => (v.index < t.index) ? -1 : 1);
            MAX_PAGE = Pages.Count - 1;

            if (Pages.Any(v => v.isFinalPage) && GameObject.Find("Postcard(Clone)") == null)
                SpawnPostcard();
        }

        public void CreateFinalPage()
        {
            if (GetMaxPageIndex() == 15) return;
            if (Pages.Count < 14) return;

            int _truePages = GetCountOfTruePages();
            bool _isTrueStory = (_truePages > 7);

            TryAddPage(new NotebookPage
            {
                index = 15,
                isTruePage = _isTrueStory,
                isFinalPage = true
            });

            if (_isTrueStory)
                SpawnPostcard();

            Pages.Remove(Pages.First(v => v.isDefaultPage));
            GameObject.Find("COTTAGE/minigame(Clone)").SetActive(false);

            Utils.PrintDebug($"Final page added with ({_truePages > 7} story)");
            Pages.ForEach(v => Utils.PrintDebug(v.ToString()));
        }

        public static void AddDefaultPage()
            => Pages.Add(new NotebookPage { index = 14, isDefaultPage = true });

        void SpawnPostcard()
        {
            Postcard.Initialize();
        }

        public static int GetMaxPageIndex()
        {
            int _max = 0;
            foreach (NotebookPage _page in Pages)
            {
                if (_page.isDefaultPage || _page.isFinalPage) continue;

                if (_page.index > _max)
                    _max = _page.index;
            }
            return _max;
        }
    }
}
