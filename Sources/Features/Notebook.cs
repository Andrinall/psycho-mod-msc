
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Features
{
    class Notebook : BookWithGUI
    {
        public static Dictionary<int, NotebookPage> Pages = new Dictionary<int, NotebookPage>()
        {
            [14] = new NotebookPage { Index = 14, IsDefaultPage = true }
        };

        protected override GameObject GUIPrefab => ResourcesStorage.NotebookGUI_prefab;

        TextMesh notebookGUIText;
        TextMesh notebookGUIPage;

        Transform itemPivot;
        PlayMakerFSM handFsm;

        Transform ItemInHand => itemPivot.childCount > 0 ? itemPivot.GetChild(0) : null;

        protected override void AfterAwake()
        {
            itemPivot = Utils.GetGlobalVariable<FsmGameObject>("ItemPivot").Value.transform;
            handFsm = itemPivot.parent.Find("Hand").GetPlayMaker("PickUp");

            notebookGUIText = GUI.transform.Find("Text").GetComponent<TextMesh>();
            notebookGUIPage = GUI.transform.Find("PageIndicator").GetComponent<TextMesh>();

            MAX_PAGE = Pages.Count - 1;
            CurrentPage = MAX_PAGE;

            EventsManager.OnLanguageChanged.AddListener(UpdatePageTextLang);
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
            UpdatePageText(next);
            Utils.PrintDebug($"CurrentPage: {CurrentPage}; MAX_PAGE :{MAX_PAGE}");
        }

        protected override void GUIOpened()
        {
            UpdatePageText();
            base.GUIOpened();
        }

        public static bool TryAddPage(NotebookPage page)
        {
            int _index = page.Index;
            if (IsPageExists(_index))
                return false;

            Pages[_index] = page;            
            return true;
        }

        public static bool IsPageExists(int index)
            => Pages.ContainsKey(index);

        public void UpdatePageTextLang() => UpdatePageText();

        void UpdatePageText(bool next = true)
        {
            if (!Pages.ContainsKey(CurrentPage))
            {
                MAX_PAGE = Pages.Keys.Max();
                MIN_PAGE = Pages.Keys.Min();

                int _maxPage = GetMaxPageIndex();
                CurrentPage = next ? MAX_PAGE : (CurrentPage < 0 ? MAX_PAGE : (_maxPage == 0 ? MIN_PAGE : _maxPage));
            }

            NotebookPage _page = Pages[CurrentPage];
            if (_page == null)
                return;

            bool _isTrueEnding = _page.IsTruePage;
            if (_page.Index == 15) // final page
            {
                notebookGUIText.text = Locales.FINAL_PAGE[_isTrueEnding ? 0 : 1, Globals.CurrentLang];
                Background.SetTexture("_MainTex",
                    _isTrueEnding ? ResourcesStorage.NotebookPages_texture : ResourcesStorage.NotebookFinalPage_texture
                );
            }
            else if (_page.Index == 14) // default
            {
                notebookGUIText.text = Locales.DEFAULT_PAGE[Globals.CurrentLang];
                Background.SetTexture("_MainTex", ResourcesStorage.NotebookStartPage_texture);
            }
            else if (_page.Index < 14)
            {
                notebookGUIText.text = Locales.PAGES[_page.Index - 1, _isTrueEnding ? 0 : 1, Globals.CurrentLang];
                Background.SetTexture("_MainTex", ResourcesStorage.NotebookPages_texture);
            }

            notebookGUIPage.text = $"Page {_page.Index}";
        }

        bool AddNewPage(GameObject pageObj)
        {
            if (pageObj == null) return false;

            int _maxPage = GetMaxPageIndex();
            if (_maxPage == 15) return false;

            NotebookPageComponent _newPage = pageObj.GetComponent<NotebookPageComponent>();
            NotebookPage _page = new NotebookPage(_newPage.Page);
            Destroy(pageObj);

            Pages[_page.Index] = _page;
            CreateFinalPage();

            if (Pages.Values.Any(v => v.IsFinalPage) && GameObject.Find("Postcard(Clone)") == null)
                Postcard.Initialize();

            PlayPageTurn();
            Utils.PrintDebug($"{_page} added into notebook");

            MIN_PAGE = Pages.Keys.Min();
            MAX_PAGE = Pages.Keys.Max();
            return true;
        }


        public int GetCountOfTruePages()
            => Pages.Values.Count(v => !v.IsFinalPage && !v.IsDefaultPage && v.IsTruePage);

        public void ClearPages()
        {
            Pages.Clear();
            AddDefaultPage();
            MAX_PAGE = 14;
            CurrentPage = MAX_PAGE;
        }

        public void CreateFinalPage()
        {
            int _maxPage = GetMaxPageIndex();
            if (_maxPage != 13) return;

            int _truePages = GetCountOfTruePages();
            bool _isTrueStory = (_truePages > 7);

            TryAddPage(new NotebookPage
            {
                Index = 15,
                IsTruePage = _isTrueStory,
                IsFinalPage = true
            });

            if (_isTrueStory)
                Postcard.Initialize();

            if (Pages.ContainsKey(14))
                Pages.Remove(14);

            GameObject.Find("COTTAGE/minigame(Clone)")?.SetActive(false);
            Utils.PrintDebug($"Final page added with ({_truePages > 7} story)");
        }

        public static void AddDefaultPage()
            => Pages[14] = new NotebookPage { Index = 14, IsDefaultPage = true };

        public static int GetMaxPageIndex()
        {
            try
            {
                return Pages.Values.Max(v => (v.IsDefaultPage || v.IsFinalPage) ? 0 : v.Index);
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }
    }
}
