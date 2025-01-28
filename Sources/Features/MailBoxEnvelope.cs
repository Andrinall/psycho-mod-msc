
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
    class MailBoxEnvelope : CatchedComponent
    {
        GameObject mailboxEnvelope;
        GameObject envelopeSheet;

        bool installed = false;
        
        static MailBoxEnvelope instance = null;


        protected override void Enabled()
        {
            if (installed) return;
            instance = this;

            mailboxEnvelope = Instantiate(transform.Find("EnvelopeInspection").gameObject);
            mailboxEnvelope.gameObject.name = "EnvelopeDoctor";
            mailboxEnvelope.transform.SetParent(GameObject.Find("YARD/PlayerMailBox").transform, false);
            mailboxEnvelope.transform.localPosition = new Vector3(-0.0685f, -0.007f, 0.1076f);
            mailboxEnvelope.GetComponent<CapsuleCollider>().radius = 0.04f;

            GameObject _sheets = FindObjectsOfType<GameObject>().First(v => v.name == "Sheets");
            envelopeSheet = Instantiate(_sheets.transform.Find("InspectionAD").gameObject);
            envelopeSheet.name = "DoctorMail";
            envelopeSheet.transform.SetParent(_sheets.transform, false);
            envelopeSheet.SetActive(true);

            Transform _oldBackground = envelopeSheet.transform.GetChild(1);
            Destroy(_oldBackground.gameObject);

            GameObject _background = Instantiate(ResourcesStorage.Background_prefab);
            _background.transform.SetParent(envelopeSheet.transform, false);
            _background.transform.localPosition = new Vector3(0, 0.002f, 0.126f);
            _background.name = "Background";
            _background.layer = 14;
            _background.transform.GetChild(0).gameObject.layer = 14;

            mailboxEnvelope.SetActive(true);
            mailboxEnvelope.GetComponent<PlayMakerFSM>().enabled = true;

            PlayMakerFSM _fsm = mailboxEnvelope.GetPlayMaker("Use");
            (_fsm.GetState("State 2").Actions[1] as SetStringValue)
                .stringValue.Value = "Strange Letter";

            (_fsm.GetState("Open ad").Actions.Last() as ActivateGameObject)
                .gameObject.GameObject.Value = envelopeSheet;

            _fsm.FsmVariables.FloatVariables = new List<FsmFloat>().ToArray();
            if (!Logic.EnvelopeSpawned)
                mailboxEnvelope.SetActive(false);

            StateHook.Inject(mailboxEnvelope, "Use", "Open ad", CreateRandomPills, -1);
            StateHook.Inject(envelopeSheet, "Setup", "State 2", DisableStrangeLetter, -1);
            envelopeSheet.SetActive(false);

            Globals.MailboxSheet = envelopeSheet;
            Globals.EnvelopeObject = mailboxEnvelope;

            if (PillsItem.Index != -1)
                SetBackgroundScreenForLetter(PillsItem.Index);

            installed = true;
        }

        protected override void Destroyed() => instance = null;

        void DisableStrangeLetter()
        {
            mailboxEnvelope.SetActive(false);
            Logic.EnvelopeSpawned = false;
        }

        void CreateRandomPills()
        {
            try
            {
                if (PillsItem.Self != null)
                {
                    Destroy(PillsItem.Self);
                    PillsItem.Self = null;
                    Utils.PrintDebug(eConsoleColors.YELLOW, "Removed previous pills");
                }

                int _idx = UnityEngine.Random.Range(0, Globals.PillsPositions.Count - 1);
                PillsItem.TryCreatePills(_idx, Globals.PillsPositions[_idx], Vector3.zero);

                SetBackgroundScreenForLetter(_idx);
                Utils.PrintDebug($"Generated pills: {_idx}");
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Failed to create a random pills;");
                ModConsole.Error(e.GetFullMessage());
            }
        }

        public static void SetBackgroundScreenForLetter(int index)
        {
            if (instance == null) return;

            Transform _image = instance.envelopeSheet.transform.Find("Background/Image");
            Texture _newTexture = ResourcesStorage.MailScreens.Find(v => v.name == index.ToString());
            _image.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", _newTexture);
            Utils.PrintDebug($"Sheets/DoctorMail/Background/Image screen updated to {_newTexture.name} idx");
        }
    }
}