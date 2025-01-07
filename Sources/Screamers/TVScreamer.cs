
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

        const int NEEDED_FRAMES = 200;

        GameObject switchObj;
        GameObject electricity;
        Transform nightProgram;

        PlayMakerFSM tvSwitch;
        PlayMakerFSM nightFSM;
        PlayMakerFSM mainSwitchUsable;

        AudioSource tvAudio;
        Texture replaceTexture;

        int elapsedFrames = 0;
        bool buttonAlreadyUsed = false;


        public override void InitScreamer()
        {
            switchObj = GameObject.Find("YARD/Building/LIVINGROOM/TV/Switch");
            mainSwitchUsable = GameObject.Find("YARD/Building/Dynamics/FuseTable/Fusetable/MainSwitch").GetComponent<PlayMakerFSM>();
            electricity = GameObject.Find("YARD/Building/Dynamics/HouseElectricity/ElectricAppliances");

            tvSwitch = switchObj.GetComponent<PlayMakerFSM>();
            StateHook.Inject(switchObj, "Use", "Switch", _hook);

            tvSwitch.Fsm.InitData();
            tvSwitch.AddEvent("SCREAM_ON");
            tvSwitch.AddGlobalTransition("SCREAM_ON", "State 5");

            var state = tvSwitch.GetState("Switch");
            state.Transitions = new List<FsmTransition>(state.Transitions)
            {
                new FsmTransition
                {
                    FsmEvent = tvSwitch.FsmEvents.First(v=> v.Name == "FINISHED"),
                    ToState = "Wait player"
                }
            }.ToArray();

            nightProgram = transform.Find("NightProgram");
            nightFSM = nightProgram.GetComponent<PlayMakerFSM>();
            nightFSM.Fsm.InitData();

            replaceTexture = ResourcesStorage.Pictures[ResourcesStorage.Pictures.Count - 1];

            tvAudio = nightProgram.GetComponent<AudioSource>();
        }

        public override void TriggerScreamer()
        {
            if (!electricity.activeSelf && WorldManager.GetElecCutoffTimer() >= 22000f)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "TV screamer triggered, but ElectricAppliances disabled (not payed)");
                base.Stop();
                return;
            }

            (tvSwitch.GetState("Switch").Actions[1] as BoolTest).Enabled = false;
            (tvSwitch.GetState("Close TV 2").Actions[8] as ActivateGameObject).Enabled = false;
            tvSwitch.CallGlobalTransition("SCREAM_ON");

            mainSwitchUsable.enabled = false;
        }

        public override void StopScreamer()
        {
            buttonAlreadyUsed = false;
            mainSwitchUsable.enabled = true;

            (tvSwitch.GetState("Switch").Actions[1] as BoolTest).Enabled = true;
            (tvSwitch.GetState("Close TV 2").Actions[8] as ActivateGameObject).Enabled = true;
            tvSwitch.CallGlobalTransition("GLOBALEVENT");

            tvAudio.enabled = true;
            elapsedFrames = 0;
        }


        protected override void OnFixedUpdate()
        {
            if (!buttonAlreadyUsed) return;
            if (elapsedFrames <= NEEDED_FRAMES)
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
            if (buttonAlreadyUsed) return;

            buttonAlreadyUsed = true;
            _setTexture();
            PlayAudioClip(ResourcesStorage.TVScream_clip);
            Utils.PrintDebug(eConsoleColors.YELLOW, "Player pressed switch button. Show screamer.");
        }

        void _setTexture()
        {
            (nightFSM.GetState("State 4").Actions[0] as SetMaterialTexture)
                .material.Value.SetTexture("_MainTex", replaceTexture);
        }

        void PlayAudioClip(AudioClip clip)
        {
            tvAudio.enabled = false;
            AudioSource.PlayClipAtPoint(clip, transform.position);
            Utils.PrintDebug(eConsoleColors.YELLOW, $"AudioClip({clip.name}) is played");
        }
    }
}
