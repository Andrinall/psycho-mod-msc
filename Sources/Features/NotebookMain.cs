using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;
using Psycho.Extensions;
using Psycho.Handlers;


namespace Psycho.Features
{
    internal class NotebookMain : BookWithGUI
    {
        public static List<NotebookPage> Pages = new List<NotebookPage>()
        {
            new NotebookPage { index = 14, isDefaultPage = true }
        };

        int prevMax = 0;

        public override GameObject GUIPrefab => Globals.NotebookGUI_prefab;

        TextMesh NotebookGUIText;
        TextMesh NotebookGUIPage;

        Transform PlayerHand;
        PlayMakerFSM HandFsm;

        void OnDestroy() => MAX_PAGE = -1;
        void OnEnable() => MAX_PAGE = prevMax;
        void OnDisable()
        {
            prevMax = MAX_PAGE;
            MAX_PAGE = -1;
        }

        public override void AfterAwake()
        {
            PlayerHand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/ItemPivot").transform;
            HandFsm = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");

            NotebookGUIText = GUI.transform.Find("Text").GetComponent<TextMesh>();
            NotebookGUIPage = GUI.transform.Find("PageIndicator").GetComponent<TextMesh>();

            MAX_PAGE = Pages.Count - 1;
            CurrentPage = MAX_PAGE;

            EventsManager.OnLanguageChanged.AddListener(UpdatePageText);
        }

        public override void ObjectUsed()
        {
            if (PlayerHand.childCount == 0)
            {
                Utils.PrintDebug(eConsoleColors.GREEN, "Notebook opened");
                base.ObjectUsed();
                return;
            }

            Transform itemInHand = PlayerHand?.GetChild(0);
            if (itemInHand != null && itemInHand?.gameObject?.name?.Contains("Notebook Page") == true)
            {
                HandFsm?.CallGlobalTransition("DROP_PART");
                AddNewPage(itemInHand.gameObject);
                return;
            }
        }
       
        public override void PageSelected(bool next)
        {
            UpdatePageText();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"CurrentPage: {CurrentPage}; MAX_PAGE :{MAX_PAGE}");
        }

        public override void GUIOpened()
        {
            UpdatePageText();
            base.GUIOpened();
        }

        public static bool TryAddPage(NotebookPage page)
        {
            if (Pages.Any(v => v.index == page.index)) return false;
            Pages.Add(page);
            return true;
        }


        public void UpdatePageText()
        {
            if (CurrentPage < 0 || CurrentPage >= Pages.Count) return;

            NotebookPage page = Pages[CurrentPage];
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
            TryCreateFinalPage();
            SortPages();

            PlayPageTurn();
            Utils.PrintDebug(eConsoleColors.GREEN, $"Page added into notebook\nidx: {page.index}, default: {page.isDefaultPage}, final: {page.isFinalPage}, true: {page.isTruePage}");
            return true;
        }


        public int CalcTruePages() => Pages.Count(v => !v.isFinalPage && v.isTruePage);

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

        public void TryCreateFinalPage()
        {
            if (GetMaxPageIndex() == 15) return;
            if (Pages.Count < 14) return;

            int truePages = CalcTruePages();
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
            Destroy(GameObject.Find("COTTAGE/minigame(Clone)"));

            Utils.PrintDebug(eConsoleColors.GREEN, $"Final page added with ({truePages > 7} story)");
            Pages.ForEach(v => Utils.PrintDebug($"page[{v.index}]: true? {v.isTruePage}; default? {v.isDefaultPage}; final? {v.isFinalPage}"));
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

        public int GetMaxPageIndex()
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
