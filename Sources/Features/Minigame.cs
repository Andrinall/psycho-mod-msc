
using System.Linq;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Features
{
    internal class Minigame : CatchedComponent
    {
        GameObject TaroUsable;
        GameObject HousekeeperCard;
        GameObject PlayerCard;

        Material HousekeeperCardMat;
        Material PlayerCardMat;

        FsmInt GlobalDay;

        Camera camera;

        int HousekeeperCurrentCardNumber = 0;
        int PlayerCurrentCardNumber = 0;

        int lastUpdated = 0;

        const int MAX_PAGES = 13;
        const int MAX_CARD = 14;

        protected override void Awaked()
        {
            TaroUsable = transform.Find("TaroUsable/Handle").gameObject;
            HousekeeperCard = transform.Find("Cards/HousekeeperCard").gameObject;
            PlayerCard = transform.Find("Cards/PlayerCard").gameObject;

            HousekeeperCardMat = HousekeeperCard.GetComponent<MeshRenderer>().material;
            PlayerCardMat = PlayerCard.GetComponent<MeshRenderer>().material;

            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            GameObject _fireParticle = GameObject.Find("ITEMS/lantern(itemx)/light/particle");
            GameObject clonedFire = Instantiate(_fireParticle);
            clonedFire.transform.SetParent(transform.Find("Candle"), false);
            clonedFire.transform.localPosition = new Vector3(0, 0, 0.06f);
            
            if (Notebook.Pages.Count >= 14)
            {
                Destroy(gameObject);
                return;
            }
        }

        protected override void OnUpdate()
        {
            if (!CheckDayChangedAndUpdateHousekeeperCard()) return;
            if (PlayerCard?.activeSelf == true) return;
            if (Camera.main == null) return;

            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out info, 2f)) return;
            if (info.collider?.gameObject != TaroUsable) return;

            Utils.SetGUIUse(true, "GET TARO CARD");

            if (cInput.GetKeyDown("Use"))
            {
                Utils.SetGUIUse(false);
                PlayerGetsCard();
            }
        }

        internal void PlayerGetsCard()
        {
            int rndCard = Random.Range(0, MAX_CARD);
            PlayerCurrentCardNumber = rndCard + 1;

            PlayerCardMat.SetTexture("_MainTex", Globals.TaroCards[rndCard]);
            PlayerCard.SetActive(true);

            Utils.PrintDebug($"PlayerCurrentCardNumber {PlayerCurrentCardNumber}");
            if (PlayerCurrentCardNumber > 7) // get card clarity
            {
                int player = (PlayerCurrentCardNumber % 7);
                player = (player == 0) ? 7 : player;

                SpawnNewPage(player <= HousekeeperCurrentCardNumber);
                return;
            }

            PlayHousekeeperLaughing();
            Logic.LastDayMinigame = GlobalDay.Value;
        }

        internal void UpdateHousekeeperCard()
        {
            int rndCard = Random.Range(0, MAX_CARD / 2);
            HousekeeperCardMat.SetTexture("_MainTex", Globals.TaroCards[rndCard]);
            HousekeeperCurrentCardNumber = rndCard + 1;
            PlayerCard.SetActive(false);

            GameObject page = GameObject.Find("Notebook Page(Clone)");
            if (page?.transform?.parent == null)
                Destroy(page);

            if (Notebook.Pages.Count == 15 && page == null)
                Destroy(this);
        }

        bool CheckDayChangedAndUpdateHousekeeperCard()
        {
            if (GlobalDay == null) return false;
            if (Logic.LastDayMinigame == GlobalDay.Value) return false;
            if (lastUpdated != GlobalDay.Value)
            {
                UpdateHousekeeperCard();
                lastUpdated = GlobalDay.Value;
            }
            return true;
        }

        void SpawnNewPage(bool isFake)
        {
            int index = Notebook.GetMaxPageIndex();
            Utils.PrintDebug($"SpawnNewPage index {index}");
            if (index >= 13)
                return;

            GameObject pageObj = (GameObject)Instantiate(
                Globals.NotebookPage_prefab,
                PlayerCard.transform.position,
                Quaternion.Euler(Vector3.zero)
            );

            NotebookPageComponent component = pageObj.AddComponent<NotebookPageComponent>();
            component.page = new NotebookPage
            {
                index = index + 1,
                isTruePage = !isFake
            };
            component.enabled = true;
            component.UpdatePageText();
            pageObj.MakePickable();

            Utils.PrintDebug($"{(isFake ? "Fake" : "True")} Page spawned with index {index + 1}");
        }

        void PlayHousekeeperLaughing()
            => AudioSource.PlayClipAtPoint(Globals.HousekeeperLaughs_clip, transform.position);

        public static void Initialize()
        {
            GameObject bottlehide = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom/bottlehide");
            Vector3 bottlehidePos = bottlehide.transform.position;
            Vector3 bottlehideRot = bottlehide.transform.eulerAngles;
            Destroy(bottlehide);

            if (Notebook.Pages.Any(v => v.isFinalPage)) return;
            GameObject minigame = Object.Instantiate(Globals.CottageMinigame_prefab);
            minigame.transform.SetParent(GameObject.Find("COTTAGE").transform, false);
            minigame.AddComponent<Minigame>();
        }
    }
}
