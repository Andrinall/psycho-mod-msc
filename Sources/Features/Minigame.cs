
using System.Linq;
using System.Collections;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    class Minigame : CatchedComponent
    {
        GameObject taroUsable;
        GameObject housekeeperCard;
        GameObject playerCard;

        Material housekeeperCardMat;
        Material playerCardMat;

        int housekeeperCurrentCardNumber = 0;
        int playerCurrentCardNumber = 0;

        int lastUpdated = 0;

        const int MAX_PAGES = 13;
        const int MAX_CARD = 14;

        protected override void Awaked()
        {
            taroUsable = transform.Find("TaroUsable/Handle").gameObject;
            housekeeperCard = transform.Find("Cards/HousekeeperCard").gameObject;
            playerCard = transform.Find("Cards/PlayerCard").gameObject;

            housekeeperCardMat = housekeeperCard.GetComponent<MeshRenderer>().material;
            playerCardMat = playerCard.GetComponent<MeshRenderer>().material;

            GameObject _fireParticle = GameObject.Find("ITEMS/lantern(itemx)/light/particle");
            GameObject _clonedFire = Instantiate(_fireParticle);
            _clonedFire.transform.SetParent(transform.Find("Candle"), false);
            _clonedFire.transform.localPosition = new Vector3(0, 0, 0.06f);
            
            if (Notebook.Pages.Count >= 14)
            {
                Destroy(gameObject);
                return;
            }
        }

        protected override void OnUpdate()
        {
            if (!CheckDayChangedAndUpdateHousekeeperCard()) return;
            if (playerCard?.activeSelf == true) return;
            if (Camera.main == null) return;

            RaycastHit _info;
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(_ray, out _info, 2f)) return;
            if (_info.collider?.gameObject != taroUsable) return;

            Utils.SetGUIUse(true, "GET TARO CARD");

            if (cInput.GetKeyDown("Use"))
            {
                Utils.SetGUIUse(false);
                PlayerGetsCard();
            }
        }

        public void PlayerGetsCard()
        {
            int _randomCard = Random.Range(0, MAX_CARD);
            playerCurrentCardNumber = _randomCard + 1;

            playerCardMat.SetTexture("_MainTex", ResourcesStorage.TaroCardsTextures[_randomCard]);
            playerCard.SetActive(true);

            Utils.PrintDebug($"PlayerCurrentCardNumber {playerCurrentCardNumber}");
            if (playerCurrentCardNumber > 7) // get card clarity
            {
                int _player = (playerCurrentCardNumber % 7);
                _player = (_player == 0) ? 7 : _player;

                StartCoroutine(SpawnNewPage(_player <= housekeeperCurrentCardNumber));
                return;
            }

            PlayHousekeeperLaughing();
            Logic.LastDayMinigame = Globals.GlobalDay.Value;
        }

        public void UpdateHousekeeperCard()
        {
            int _randomCard = Random.Range(0, MAX_CARD / 2);
            housekeeperCardMat.SetTexture("_MainTex", ResourcesStorage.TaroCardsTextures[_randomCard]);
            housekeeperCurrentCardNumber = _randomCard + 1;
            playerCard.SetActive(false);

            GameObject _page = GameObject.Find("Notebook Page(Clone)");
            if (_page?.transform?.parent == null)
                Destroy(_page);

            if (Notebook.Pages.Count == 15 && _page == null)
                Destroy(this);
        }

        bool CheckDayChangedAndUpdateHousekeeperCard()
        {
            if (Logic.LastDayMinigame == Globals.GlobalDay.Value) return false;
            if (lastUpdated != Globals.GlobalDay.Value)
            {
                UpdateHousekeeperCard();
                lastUpdated = Globals.GlobalDay.Value;
            }
            return true;
        }

        IEnumerator SpawnNewPage(bool isFake)
        {
            int _index = Notebook.GetMaxPageIndex();
            if (_index >= 13)
            {
                PlayHousekeeperLaughing();
                yield return new WaitForSeconds(0.5f);

                gameObject.SetActive(false);
                yield break;
            }

            Utils.PrintDebug($"SpawnNewPage index {_index}");
            yield return new WaitForSeconds(1f);

            GameObject _pageObj = (GameObject)Instantiate(
                ResourcesStorage.NotebookPage_prefab,
                playerCard.transform.position,
                Quaternion.Euler(Vector3.zero)
            );

            NotebookPageComponent _component = _pageObj.AddComponent<NotebookPageComponent>();
            _component.Page = new NotebookPage
            {
                index = _index + 1,
                isTruePage = !isFake
            };
            _component.enabled = true;
            _component.UpdatePageText();
            _pageObj.MakePickable();

            Utils.PrintDebug($"{(isFake ? "Fake" : "True")} Page spawned with index {_index + 1}");
        }

        void PlayHousekeeperLaughing()
            => AudioSource.PlayClipAtPoint(ResourcesStorage.HousekeeperLaughs_clip, transform.position);

        public static void Initialize()
        {
            GameObject _bottlehide = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom/bottlehide");
            Vector3 _bottlehidePos = _bottlehide.transform.position;
            Vector3 _bottlehideRot = _bottlehide.transform.eulerAngles;
            Destroy(_bottlehide);

            if (Notebook.Pages.Any(v => v.isFinalPage)) return;
            GameObject _minigame = Object.Instantiate(ResourcesStorage.CottageMinigame_prefab);
            _minigame.transform.SetParent(GameObject.Find("COTTAGE").transform, false);
            _minigame.AddComponent<Minigame>();
        }
    }
}
