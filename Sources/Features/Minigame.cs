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

        FsmString GUIinteraction;
        FsmBool GUIuse;
        FsmInt GlobalDay;

        int HousekeeperCurrentCardNumber = 0;
        int PlayerCurrentCardNumber = 0;

        internal override void Awaked()
        {
            TaroUsable = transform.Find("TaroUsable/Handle").gameObject;
            HousekeeperCard = transform.Find("Cards/HousekeeperCard").gameObject;
            PlayerCard = transform.Find("Cards/PlayerCard").gameObject;
            Utils.PrintDebug($"{TaroUsable}, {PlayerCard}, {HousekeeperCard}");

            HousekeeperCardMat = HousekeeperCard.GetComponent<MeshRenderer>().material;
            PlayerCardMat = PlayerCard.GetComponent<MeshRenderer>().material;

            GUIinteraction = Utils.GetGlobalVariable<FsmString>("GUIinteraction");
            GUIuse = Utils.GetGlobalVariable<FsmBool>("GUIuse");
            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
        }

        internal override void OnFixedUpdate()
        {
            CheckDayChanged();

            if (PlayerCard.activeSelf) return;

            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out info, 2f)) return;
            if (info.collider?.gameObject != TaroUsable) return;

            GUIuse.Value = true;
            GUIinteraction.Value = "GET TARO CARD";

            if (cInput.GetKeyDown("Use"))
            {
                GUIuse.Value = false;
                GUIinteraction.Value = "";
                PlayerGetsCard();
            }
        }

        internal void PlayerGetsCard()
        {
            int rndCard = Random.Range(0, 14);
            PlayerCurrentCardNumber = rndCard + 1;

            PlayerCardMat.SetTexture("_MainTex", Globals.TaroCards[rndCard]);
            PlayerCard.SetActive(true);
            
            if (PlayerCurrentCardNumber > 7) // get card clarity
            {
                int player = (PlayerCurrentCardNumber % 7);
                player = (player == 0) ? 7 : player;

                SpawnNewPage(player <= HousekeeperCurrentCardNumber);
                return;
            }

            PlayHousekeeperLaughing();
        }

        internal void UpdateHousekeeperCard()
        {
            int rndCard = Random.Range(0, 7);
            HousekeeperCardMat.SetTexture("_MainTex", Globals.TaroCards[rndCard]);
            HousekeeperCurrentCardNumber = rndCard + 1;
            PlayerCard.SetActive(false);
        }

        void CheckDayChanged()
        {
            if (GlobalDay.Value != Logic.lastDayMinigame)
            {
                Logic.lastDayMinigame = GlobalDay.Value;
                UpdateHousekeeperCard();
            }
        }

        void SpawnNewPage(bool isFake)
        {
            if (isFake)
            {
                Utils.PrintDebug("Spawned fake page");
                return;
            }

            Utils.PrintDebug("Spawned true page");
        }

        void PlayHousekeeperLaughing()
            => AudioSource.PlayClipAtPoint(Globals.HousekeeperLaughs_clip, transform.position);
    }
}
