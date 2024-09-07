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
                envelopeSheet.gameObject.name = "DoctorMail";
                envelopeSheet.transform.SetParent(GameObject.Find("Sheets").transform, worldPositionStays: false);

                mailboxEnvelope.SetActive(true);
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
            var action = (openad.Actions.ElementAt(1) as ActivateGameObject);
            action.gameObject.GameObject.Value = envelopeSheet;
            action.Owner = envelopeSheet;

            fsm.FsmVariables.FloatVariables = new List<FsmFloat> { }.ToArray();
            GameHook.InjectStateHook(envelopeSheet, "Setup", "State 2", () => mailboxEnvelope?.SetActive(false));

            mailboxEnvelope.SetActive(false);
            envelopeSheet.SetActive(false);
        }
    }
}
