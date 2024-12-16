using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;


namespace Psycho.Features
{
    public sealed class MailBoxEnvelope : CatchedComponent
    {
        public GameObject MailboxEnvelope;
        public GameObject EnvelopeSheet;

        bool m_bInstalled = false;


        public override void Enabled()
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
                (fsm.GetState("State 2").Actions[1] as SetStringValue)
                    .stringValue.Value = "Strange Letter";

                (fsm.GetState("Open ad").Actions.Last() as ActivateGameObject)
                    .gameObject.GameObject.Value = EnvelopeSheet;

                fsm.FsmVariables.FloatVariables = new List<FsmFloat>().ToArray();
                if (!Logic.envelopeSpawned)
                    MailboxEnvelope.SetActive(false);

                StateHook.Inject(MailboxEnvelope, "Use", "Open ad", CreateRandomPills, -1);
                StateHook.Inject(EnvelopeSheet, "Setup", "State 2", DisableStrangeLetter, -1);
                EnvelopeSheet.SetActive(false);

                Globals.mailboxSheet = EnvelopeSheet;
                Globals.envelopeObject = MailboxEnvelope;

                if (Globals.pills != null)
                    SetBackgroundScreenForLetter(Globals.pills.index);

                m_bInstalled = true;
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Error in MailBoxEnvelope:OnEnable(): {e.GetFullMessage()}");
            }
        }

        void DisableStrangeLetter()
        {
            MailboxEnvelope.SetActive(false);
            Logic.envelopeSpawned = false;
        }

        void CreateRandomPills()
        {
            try
            {
                if (Globals.pills != null)
                {
                    Destroy(Globals.pills.self);
                    Globals.pills = null;
                    Utils.PrintDebug(eConsoleColors.YELLOW, "Removed previous pills");
                }

                int idx = UnityEngine.Random.Range(0, Globals.pills_positions.Count - 1);
                Globals.pills = new PillsItem(idx, Globals.pills_positions[idx]);

                SetBackgroundScreenForLetter(idx);
                Utils.PrintDebug(eConsoleColors.GREEN, $"Generated pills: {idx}");
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to create a random pills;\n{e.GetFullMessage()}");
            }
        }

        void SetBackgroundScreenForLetter(int index)
        {
            Transform image = EnvelopeSheet.transform.Find("Background/Image");
            Texture newTexture = Globals.mailScreens.Find(v => v.name == index.ToString());
            image.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", newTexture);
            Utils.PrintDebug($"Sheets/DoctorMail/Background/Image screen updated to {newTexture.name} idx");
        }
    }
}