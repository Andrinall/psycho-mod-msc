using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class TVScreamer : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.FEAR;
        public override int ScreamerVariant => (int)ScreamFearType.TV;


        GameObject SwitchObj;
        GameObject Electricity;
        Transform NightProgram;

        PlayMakerFSM TVSwitch;
        PlayMakerFSM NightFSM;
        PlayMakerFSM MainSwitchUsable;

        AudioSource TVAudio;
        Texture ReplaceTexture;

        int elapsedFrames = 0;
        const int neededFrames = 200;

        bool ButtonAlreadyUsed = false;


        public override void InitScreamer()
        {
            SwitchObj = GameObject.Find("YARD/Building/LIVINGROOM/TV/Switch");
            MainSwitchUsable = GameObject.Find("YARD/Building/Dynamics/FuseTable/Fusetable/MainSwitch").GetComponent<PlayMakerFSM>();
            Electricity = GameObject.Find("YARD/Building/Dynamics/HouseElectricity/ElectricAppliances");

            TVSwitch = SwitchObj.GetComponent<PlayMakerFSM>();
            StateHook.Inject(SwitchObj, "Use", "Switch", _hook);

            TVSwitch.Fsm.InitData();
            TVSwitch.AddEvent("SCREAM_ON");
            TVSwitch.AddGlobalTransition("SCREAM_ON", "State 5");

            var state = TVSwitch.GetState("Switch");
            state.Transitions = new List<FsmTransition>(state.Transitions)
            {
                new FsmTransition
                {
                    FsmEvent = TVSwitch.FsmEvents.First(v=> v.Name == "FINISHED"),
                    ToState = "Wait player"
                }
            }.ToArray();

            NightProgram = transform.Find("NightProgram");
            NightFSM = NightProgram.GetComponent<PlayMakerFSM>();
            NightFSM.Fsm.InitData();

            ReplaceTexture = Globals.pictures[Globals.pictures.Count - 1];

            TVAudio = NightProgram.GetComponent<AudioSource>();
        }

        public override void TriggerScreamer()
        {
            if (!Electricity.activeSelf && WorldManager.GetElecCutoffTimer() >= 22000f)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "TV screamer triggered, but ElectricAppliances disabled (not payed)");
                base.Stop();
                return;
            }

            (TVSwitch.GetState("Switch").Actions[1] as BoolTest).Enabled = false;
            (TVSwitch.GetState("Close TV 2").Actions[8] as ActivateGameObject).Enabled = false;
            TVSwitch.CallGlobalTransition("SCREAM_ON");

            MainSwitchUsable.enabled = false;
        }

        public override void StopScreamer()
        {
            ButtonAlreadyUsed = false;
            MainSwitchUsable.enabled = true;

            (TVSwitch.GetState("Switch").Actions[1] as BoolTest).Enabled = true;
            (TVSwitch.GetState("Close TV 2").Actions[8] as ActivateGameObject).Enabled = true;
            TVSwitch.CallGlobalTransition("GLOBALEVENT");

            TVAudio.enabled = true;
            elapsedFrames = 0;
        }


        protected override void OnFixedUpdate()
        {
            if (!ButtonAlreadyUsed) return;
            if (elapsedFrames <= neededFrames)
            {
                elapsedFrames++;
                return;
            }

            base.Stop();
        }


        void _hook(PlayMakerFSM _fsm)
        {
            if (!ScreamerEnabled) return;
            
            _fsm.SendEvent("FINISHED");
            if (ButtonAlreadyUsed) return;

            ButtonAlreadyUsed = true;
            _setTexture();
            PlayAudioClip(Globals.TVScream_clip);
            Utils.PrintDebug(eConsoleColors.YELLOW, "Player pressed switch button. Show screamer.");
        }

        void _setTexture()
        {
            (NightFSM.GetState("State 4").Actions[0] as SetMaterialTexture)
                .material.Value.SetTexture("_MainTex", ReplaceTexture);
        }

        void PlayAudioClip(AudioClip clip)
        {
            TVAudio.enabled = false;
            AudioSource.PlayClipAtPoint(clip, transform.position);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"AudioClip({clip.name}) is played");
        }
    }
}
