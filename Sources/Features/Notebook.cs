
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using Psycho.Handlers;


namespace Psycho.Features
{
    internal sealed class Notebook : BookWithGUI
    {
        public static List<NotebookPage> Pages = new List<NotebookPage>()
        {
            new NotebookPage { index = 14, isDefaultPage = true }
        };

        protected override GameObject GUIPrefab => Globals.NotebookGUI_prefab;

        TextMesh NotebookGUIText;
        TextMesh NotebookGUIPage;

        Transform ItemPivot;
        PlayMakerFSM HandFsm;

        Transform ItemInHand => ItemPivot.childCount > 0 ? ItemPivot.GetChild(0) : null;

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
            ItemPivot = Utils.GetGlobalVariable<FsmGameObject>("ItemPivot").Value.transform;
            HandFsm = ItemPivot.parent.Find("Hand").GetPlayMaker("PickUp");

            NotebookGUIText = GUI.transform.Find("Text").GetComponent<TextMesh>();
            NotebookGUIPage = GUI.transform.Find("PageIndicator").GetComponent<TextMesh>();

            MAX_PAGE = Pages.Count - 1;
            CurrentPage = MAX_PAGE;

            EventsManager.OnLanguageChanged.AddListener(UpdatePageText);
        }

        protected override void ObjectUsed()
        {
            Transform item = ItemInHand;

            if (item != null && item?.name?.Contains("Notebook Page") == true)
            {
                HandFsm.CallGlobalTransition("DROP_PART");
                AddNewPage(item.gameObject);
            }
            else
            {
                base.ObjectUsed();
            }
        }

        protected override void PageSelected(bool next)
        {
            UpdatePageText();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"CurrentPage: {CurrentPage}; MAX_PAGE :{MAX_PAGE}");
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

            NotebookPage page = Pages.ElementAt(CurrentPage);
            if (page == null) return;

            bool isTrueEnding = page.isTruePage;
            if (page.index == 15) // final page
            {
                NotebookGUIText.text = Locales.FINAL_PAGE[isTrueEnding ? 0 : 1, Globals.CurrentLang];
                Background.SetTexture("_MainTex",
                    isTrueEnding ? Globals.NotebookPages_texture : Globals.NotebookFinalPage_texture
                );
            }
            else if (page.index == 14) // default
            {
                NotebookGUIText.text = Locales.DEFAULT_PAGE[Globals.CurrentLang];
                Background.SetTexture("_MainTex", Globals.NotebookStartPage_texture);
            }
            else if (page.index < 14)
            {
                NotebookGUIText.text = Locales.PAGES[page.index - 1, isTrueEnding ? 0 : 1, Globals.CurrentLang];
                Background.SetTexture("_MainTex", Globals.NotebookPages_texture);
            }

            NotebookGUIPage.text = $"Page {page.index}";
            MAX_PAGE = Pages.Count - 1;
        }

        bool AddNewPage(GameObject pageObj)
        {
            if (pageObj == null) return false;
            if (Pages.Count > 13) return false;

            NotebookPageComponent newPage = pageObj.GetComponent<NotebookPageComponent>();
            NotebookPage page = new NotebookPage(newPage.page);
            Destroy(pageObj);

            Pages.Add(page);
            CreateFinalPage();
            SortPages();

            PlayPageTurn();
            Utils.PrintDebug(eConsoleColors.GREEN, $"{page} added into notebook");
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

            int truePages = GetCountOfTruePages();
            bool isTrueStory = (truePages > 7);

            TryAddPage(new NotebookPage
            {
                index = 15,
                isTruePage = isTrueStory,
                isFinalPage = true
            });

            if (isTrueStory)
                SpawnPostcard();

            Pages.Remove(Pages.First(v => v.isDefaultPage));
            GameObject.Find("COTTAGE/minigame(Clone)").SetActive(false);

            Utils.PrintDebug(eConsoleColors.GREEN, $"Final page added with ({truePages > 7} story)");
            Pages.ForEach(v => Utils.PrintDebug(v.ToString()));
        }

        public static void AddDefaultPage()
            => Pages.Add(new NotebookPage { index = 14, isDefaultPage = true });

        void SpawnPostcard()
        {
            GameObject postcard = ItemsPool.AddItem(Globals.Postcard_prefab);
            postcard.transform.SetParent(GameObject.Find("YARD/PlayerMailBox").transform, false);
            postcard.AddComponent<ItemsGravityEnabler>();
            Utils.InitPostcard(postcard);
        }

        public static int GetMaxPageIndex()
        {
            int max = 0;
            foreach (NotebookPage page in Pages)
            {
                if (page.isDefaultPage || page.isFinalPage) continue;

                if (page.index > max)
                    max = page.index;
            }
            return max;
        }
    }
}
