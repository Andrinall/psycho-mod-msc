using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using Psycho.Internal;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace Psycho.Objects
{
    public sealed class MailBoxEnvelope : MonoBehaviour
    {
        public GameObject MailboxEnvelope;
        public GameObject EnvelopeSheet;

        bool m_bInstalled = false;

        void OnEnable()
        {
            if (m_bInstalled) return;

            try
            {
                MailboxEnvelope = Instantiate(transform.Find("EnvelopeInspection").gameObject);
                MailboxEnvelope.gameObject.name = "EnvelopeDoctor";
                MailboxEnvelope.transform.SetParent(GameObject.Find("YARD/PlayerMailBox").transform, worldPositionStays: false);
                MailboxEnvelope.transform.localPosition = new Vector3(-0.0685f, -0.007f, 0.1076f);
                MailboxEnvelope.GetComponent<CapsuleCollider>().radius = 0.04f;

                GameObject sheets = FindObjectsOfType<GameObject>().First(v => v.name == "Sheets");
                EnvelopeSheet = Instantiate(sheets.transform.Find("InspectionAD").gameObject);
                EnvelopeSheet.name = "DoctorMail";
                EnvelopeSheet.transform.SetParent(sheets.transform, worldPositionStays: false);
                EnvelopeSheet.SetActive(true);

                Transform old_back = EnvelopeSheet.transform.GetChild(1);
                old_back.parent = null;
                Destroy(old_back.gameObject);

                GameObject _background = Instantiate(Globals.Background_prefab);
                _background.transform.SetParent(EnvelopeSheet.transform, worldPositionStays: false);
                _background.transform.localPosition = new Vector3(0, 0.002f, 0.126f);
                _background.name = "Background";
                _background.layer = 14;
                _background.transform.GetChild(0).gameObject.layer = 14;

                MailboxEnvelope.SetActive(true);
                MailboxEnvelope.GetComponent<PlayMakerFSM>().enabled = true;

                PlayMakerFSM fsm = MailboxEnvelope.GetPlayMaker("Use");
                (fsm.GetState("State 2").Actions.ElementAtOrDefault(1) as SetStringValue)
                    .stringValue.Value = "Strange Letter";

                (fsm.GetState("Open ad").Actions.Last() as ActivateGameObject)
                    .gameObject.GameObject.Value = EnvelopeSheet;

                fsm.FsmVariables.FloatVariables = new List<FsmFloat>().ToArray();
                if (!Logic.envelopeSpawned)
                    MailboxEnvelope.SetActive(false);

                StateHook.Inject(MailboxEnvelope, "Use", "Open ad", -1, _ => Utils.CreateRandomPills());
                StateHook.Inject(EnvelopeSheet, "Setup", "State 2", _ => {
                    MailboxEnvelope.SetActive(false);
                    Logic.envelopeSpawned = false;
                });
                EnvelopeSheet.SetActive(false);

                Globals.mailboxSheet = EnvelopeSheet;
                Globals.envelopeObject = MailboxEnvelope;
                
                m_bInstalled = true;
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"error in MailBoxEnvelope:OnEnable(): {e.GetFullMessage()}");
            }
        }
    }
}