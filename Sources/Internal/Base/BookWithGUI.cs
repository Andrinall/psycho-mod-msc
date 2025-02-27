﻿
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    class BookWithGUI : CatchedComponent
    {
        protected int CurrentPage { get; set; } = 0;
        protected virtual int MAX_PAGE { get; set; } = 0;
        protected virtual int MIN_PAGE { get; set; } = 0;

        protected virtual GameObject GUIPrefab => null;
        protected GameObject GUI { get; private set; }
        protected GameObject ButtonNext { get; private set; }
        protected GameObject ButtonPrev { get; private set; }
        protected Material Background { get; private set; }


        RaycastHit hitInfo;

        GameObject guiCamera;
        GameObject originalCamera;

        PlayMakerFSM openMenu;
        FsmBool playerStop;
        FsmBool playerInMenu;
        FsmBool playerComputer;

        AudioClip pageTurn;

        int GUILayerMask => LayerMask.GetMask("Parts", "GUI");

        protected override void Awaked()
        {
            CurrentPage = 0;

            GUI = Instantiate(GUIPrefab);
            Transform guiTransform = GUI.transform;

            GUI.gameObject.name = "Sketchbook";
            guiTransform.SetParent(GameObject.Find("Sheets").transform);

            ButtonNext = guiTransform.Find("ButtonNext").gameObject;
            ButtonPrev = guiTransform.Find("ButtonPrevious").gameObject;
            Background = guiTransform.Find("Background").GetComponent<MeshRenderer>().material;

            guiCamera = guiTransform.Find("Cam").gameObject;

            openMenu = GameObject.Find("Systems/Options").GetPlayMaker("Open Menu");
            playerInMenu = Utils.GetGlobalVariable<FsmBool>("PlayerInMenu");
            playerComputer = Utils.GetGlobalVariable<FsmBool>("PlayerComputer");
            playerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            pageTurn = GameObject.Find("MasterAudio/GUI/page_turn").GetComponent<AudioSource>().clip;

            GUI.SetActive(false);
            AfterAwake();
        }

        protected override void OnUpdate()
        {
            if (Camera.main == null) return;
            if (GUI == null) return;

            if (GUI.activeSelf && Input.GetKeyUp(KeyCode.Escape))
            {
                ActivateGUI(false);
                return;
            }

            if (!Physics.Raycast(Globals.RayFromScreenPoint, out hitInfo, 1.5f, GUILayerMask)) return;
            if (hitInfo.collider == null) return;

            GameObject hitted = hitInfo.collider?.gameObject;
            if (GUI.activeSelf)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (hitted == ButtonNext)
                        SelectPage(true);
                    else if (hitted == ButtonPrev)
                        SelectPage(false);

                    return;
                }
            }

            if (hitted == gameObject && !GUI.activeSelf && cInput.GetKeyUp("Use"))
            {
                if (originalCamera == null)
                    originalCamera = Camera.main.gameObject;

                ObjectUsed();
                return;
            }
        }

        /// <summary>
        /// Activate object GUI
        /// </summary>
        /// <param name="state">State of gui active</param>
        public void ActivateGUI(bool state)
        {
            playerInMenu.Value = state;
            playerStop.Value = state;
            playerComputer.Value = state;
            openMenu.enabled = !state;
            GUI.SetActive(state);

            if (state)
                GUIOpened();
            else
                GUIClosed();

            SetMainCamera(state ? guiCamera : originalCamera);
        }

        /// <summary>
        /// Switch current selected page for +1 or -1 using bool param
        /// <code>
        /// obj.SelectPage(true) // to next page
        /// obj.SelectPage(false) // to prev page
        /// </code>
        /// </summary>
        /// <param name="next">Go to next page? (false - previous)</param>
        public void SelectPage(bool next)
        {
            CurrentPage += next ? 1 : -1;

            if (CurrentPage > MAX_PAGE)
                CurrentPage = MIN_PAGE;

            if (CurrentPage < MIN_PAGE)
                CurrentPage = MAX_PAGE;

            PlayPageTurn();
            PageSelected(next);
        }

        /// <summary>
        /// <b>(Callback for override)</b> Called after GUI instantiate & all fields filled with data
        /// </summary>
        protected virtual void AfterAwake() { }

        /// <summary>
        /// <b>(Callback for override)</b> Called after switch CurrentPage and PlayPageTurn();
        /// </summary>
        /// <param name="next">Go to next page? (false - previous)</param>
        protected virtual void PageSelected(bool next) { }

        /// <summary>
        /// <b>(Callback for override)</b> Called after apply activate gui state (true)
        /// </summary>
        protected virtual void GUIOpened() => Utils.PrintDebug(eConsoleColors.GREEN, $"GUI [{GUIPrefab?.name}] Opened");

        /// <summary>
        /// <b>(Callback for override)</b> Called after apply activate gui state (false)
        /// </summary>
        protected virtual void GUIClosed() => Utils.PrintDebug(eConsoleColors.GREEN, $"GUI [{GUIPrefab?.name}] Closed");

        /// <summary>
        /// <b>(Callback for override)</b> Called when object (where have this component) used by mouseOver & click `Use` button
        /// </summary>
        protected virtual void ObjectUsed()
        {
            PlayPageTurn();
            ActivateGUI(true);
        }

        /// <summary>
        /// Play page turn sound
        /// </summary>
        public void PlayPageTurn()
            => AudioSource.PlayClipAtPoint(pageTurn, Globals.Player.position);


        void SetMainCamera(GameObject newCamera)
        {
            if (Camera.main != null)
                Camera.main.gameObject.tag = "Untagged";

            newCamera.tag = "MainCamera";
        }

    }
}
