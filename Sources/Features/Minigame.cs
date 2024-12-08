using System.Collections;

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

        internal override void Awaked()
        {
            TaroUsable = transform.Find("TaroUsable/Handle").gameObject;
            HousekeeperCard = transform.Find("Cards/HousekeeperCard").gameObject;
            PlayerCard = transform.Find("Cards/PlayerCard").gameObject;

            HousekeeperCardMat = HousekeeperCard.GetComponent<MeshRenderer>().material;
            PlayerCardMat = PlayerCard.GetComponent<MeshRenderer>().material;

            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");

            if (NotebookMain.Pages.Count >= 14)
            {
                Destroy(gameObject);
                return;
            }
        }

        internal override void OnFixedUpdate()
        {
            if (!CheckDayChanged()) return;
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
            
            if (PlayerCurrentCardNumber > 7) // get card clarity
            {
                int player = (PlayerCurrentCardNumber % 7);
                player = (player == 0) ? 7 : player;

                StartCoroutine(SpawnNewPage(player <= HousekeeperCurrentCardNumber));
                return;
            }

            PlayHousekeeperLaughing();
            Logic.lastDayMinigame = GlobalDay.Value;
        }

        internal void UpdateHousekeeperCard()
        {
            int rndCard = Random.Range(0, MAX_CARD / 2);
            HousekeeperCardMat.SetTexture("_MainTex", Globals.TaroCards[rndCard]);
            HousekeeperCurrentCardNumber = rndCard + 1;
            PlayerCard.SetActive(false);

            GameObject page = GameObject.Find("Notebook Page(Clone)");
            if (page?.transform?.parent == null)
            {
                Logic.numberOfSpawnedPages--;
                Destroy(page);
            }

            if (NotebookMain.Pages.Count == 15 && Logic.numberOfSpawnedPages == 13 && page == null)
                Destroy(this);
        }

        bool CheckDayChanged()
        {
            if (GlobalDay == null) return false;
            if (Logic.lastDayMinigame == GlobalDay.Value) return false;
            if (lastUpdated != GlobalDay.Value)
            {
                UpdateHousekeeperCard();
                lastUpdated = GlobalDay.Value;
            }
            return true;
        }

        IEnumerator SpawnNewPage(bool isFake)
        {
            if (Logic.numberOfSpawnedPages == MAX_PAGES)
            {
                yield return new WaitForSeconds(0.5f);
                PlayHousekeeperLaughing();
                yield break;
            }

            yield return new WaitForSeconds(1f);

            int index = Globals.Notebook?.GetMaxPageIndex() ?? - 1;
            if (index < 0)
                yield break;

            if (index >= 13)
            {
                Utils.PrintDebug(eConsoleColors.RED, "Notebook contains a max count of pages. Spawn new page aborted.");
                yield break;
            }

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

            Logic.numberOfSpawnedPages++;
            Utils.PrintDebug($"{(isFake ? "Fake" : "True")} Page spawned with index {index + 1}");
        }

        void PlayHousekeeperLaughing()
            => AudioSource.PlayClipAtPoint(Globals.HousekeeperLaughs_clip, transform.position);
    }
}
