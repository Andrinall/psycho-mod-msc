using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace Adrenaline
{
    internal class MailBoxEnvelope : MonoBehaviour
    {
        private GameObject mailboxEnvelope;
        private GameObject envelopeSheet;
        private bool installed = false;

        private void Awake()
        {
            try
            {
                mailboxEnvelope = Instantiate(base.transform.Find("EnvelopeInspection").gameObject);
                mailboxEnvelope.gameObject.name = "EnvelopeDoctor";
                mailboxEnvelope.transform.SetParent(GameObject.Find("YARD/PlayerMailBox").transform, worldPositionStays: false);
                mailboxEnvelope.transform.localPosition = new Vector3(-0.0685f, -0.007f, 0.1076f);
                mailboxEnvelope.GetComponent<CapsuleCollider>().radius = 0.04f;

                var sheets = FindObjectsOfType<GameObject>().First(v => v.name == "Sheets");
                envelopeSheet = Instantiate(sheets.transform.Find("InspectionAD").gameObject);
                envelopeSheet.name = "DoctorMail";
                envelopeSheet.transform.SetParent(sheets.transform, worldPositionStays: false);
                envelopeSheet.SetActive(true);

                var old_back = envelopeSheet.transform.GetChild(1);
                old_back.parent = null;
                Destroy(old_back.gameObject);

                var _background = Instantiate(Globals.background);
                _background.transform.SetParent(envelopeSheet.transform, worldPositionStays: false);
                _background.transform.localPosition = new Vector3(0, 0.002f, 0.126f);
                _background.name = "Background";
                _background.layer = 14;
                _background.transform.GetChild(0).gameObject.layer = 14;

                mailboxEnvelope.SetActive(true);
                mailboxEnvelope.GetComponent<PlayMakerFSM>().enabled = true;
                Utils.PrintDebug(eConsoleColors.GREEN, "MailBoxEnvelope component loaded");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Error while loading MailBoxEnvelope component");
            }
        }

        private void Start()
        {
            var fsm = mailboxEnvelope.GetPlayMaker("Use");
            var state2 = fsm.GetState("State 2");
            (state2.Actions.ElementAt(1) as SetStringValue).stringValue.Value = "Mail from Doctor";
            
            var openad = fsm.GetState("Open ad");
            var action = openad.Actions.ElementAt(1) as ActivateGameObject;
            action.gameObject.GameObject.Value = envelopeSheet;
            action.Owner = envelopeSheet;

            fsm.FsmVariables.FloatVariables = new List<FsmFloat> { }.ToArray();
            mailboxEnvelope.SetActive(false);
        }

        private void OnEnable()
        {
            if (installed) return;

            StateHook.Inject(mailboxEnvelope, "Use", "Open ad", Utils.CreateRandomPills);
            StateHook.Inject(envelopeSheet, "Setup", "State 2", () => mailboxEnvelope?.SetActive(false));
            envelopeSheet.SetActive(false);
            installed = true;
            Utils.PrintDebug(eConsoleColors.YELLOW, "MailBoxEnvelope enabled");
        }
    }
}