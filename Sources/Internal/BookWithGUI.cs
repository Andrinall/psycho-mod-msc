using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace Psycho.Internal
{
    internal abstract class BookWithGUI : CatchedComponent
    {
        public int CurrentPage { get; set; } = 0;
        public virtual int MAX_PAGE { get; set; } = 0;

        public virtual GameObject GUIPrefab => null;

        public RaycastHit hitInfo;

        public GameObject GUI;
        public GameObject ButtonNext;
        public GameObject ButtonPrev;
        public Material Background;

        GameObject GUICamera;
        GameObject OriginalCamera;

        PlayMakerFSM OpenMenu;
        FsmBool PlayerStop;
        FsmBool PlayerInMenu;
        FsmBool PlayerComputer;

        AudioClip PageTurn;
        Transform Player;

        internal override void Awaked()
        {
            CurrentPage = 0;

            GUI = Instantiate(GUIPrefab);
            Transform guiTransform = GUI.transform;

            GUI.gameObject.name = "Sketchbook";
            guiTransform.SetParent(GameObject.Find("Sheets").transform);

            ButtonNext = guiTransform.Find("ButtonNext").gameObject;
            ButtonPrev = guiTransform.Find("ButtonPrevious").gameObject;
            Background = guiTransform.Find("Background").GetComponent<MeshRenderer>().material;

            GUICamera = guiTransform.Find("Cam").gameObject;

            OpenMenu = GameObject.Find("Systems/Options").GetPlayMaker("Open Menu");
            PlayerInMenu = Utils.GetGlobalVariable<FsmBool>("PlayerInMenu");
            PlayerComputer = Utils.GetGlobalVariable<FsmBool>("PlayerComputer");
            PlayerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            PageTurn = GameObject.Find("MasterAudio/GUI/page_turn").GetComponent<AudioSource>().clip;
            Player = GameObject.Find("PLAYER").transform;

            GUI.SetActive(false);

            AfterAwake();
        }

        internal override void OnFixedUpdate()
        {
            if (Camera.main == null) return;
            if (GUI == null) return;

            if (GUI.activeSelf && Input.GetKeyUp(KeyCode.Escape))
            {
                ActivateGUI(false);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hitInfo, 1.5f, LayerMask.GetMask("Parts", "GUI"))) return;
            if (hitInfo.collider == null) return;

            GameObject hitted = hitInfo.collider?.gameObject;
            if (GUI.activeSelf)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (hitted == ButtonNext)
                    {
                        SelectPage(true);
                        return;
                    }

                    if (hitted == ButtonPrev)
                    {
                        SelectPage(false);
                        return;
                    }
                }
            }

            if (hitted == gameObject && !GUI.activeSelf && cInput.GetKeyDown("Use"))
            {
                if (OriginalCamera == null)
                    OriginalCamera = Camera.main.gameObject;

                ObjectUsed();
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">state of gui active</param>
        public void ActivateGUI(bool state)
        {
            PlayerInMenu.Value = state;
            PlayerStop.Value = state;
            PlayerComputer.Value = state;
            OpenMenu.enabled = !state;
            GUI.SetActive(state);

            SetMainCamera(state ? GUICamera : OriginalCamera);

            GUIActivated(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next">go to next page? (false - previous)</param>
        public void SelectPage(bool next)
        {
            CurrentPage += next ? 1 : -1;

            if (CurrentPage > MAX_PAGE) CurrentPage = 0;
            if (CurrentPage < 0) CurrentPage = MAX_PAGE;

            PlayPageTurn();
            PageSelected(next);
        }

        /// <summary>
        /// called after GUI instantiate & all fields filled with data
        /// </summary>
        public virtual void AfterAwake() { }

        /// <summary>
        /// called after switch CurrentPage and PlayPageTurn();
        /// </summary>
        /// <param name="next"></param>
        public virtual void PageSelected(bool next) { }

        /// <summary>
        /// called after apply activate gui state
        /// </summary>
        /// <param name="state">state of activate gui</param>
        public virtual void GUIActivated(bool state) { }

        /// <summary>
        /// called when object (where have this component) used by mouseOver & click `Use` button
        /// </summary>
        public virtual void ObjectUsed()
        {
            PlayPageTurn();
            ActivateGUI(true);
        }



        void SetMainCamera(GameObject newCamera)
        {
            if (Camera.main != null)
                Camera.main.gameObject.tag = "Untagged";

            newCamera.tag = "MainCamera";
        }

        public void PlayPageTurn()
            => AudioSource.PlayClipAtPoint(PageTurn, Player.position);
    }
}
