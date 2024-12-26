using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class PhoneRing : CatchedComponent
    {
        GameObject Ring;
        GameObject _logic;
        GameObject Callers;
        GameObject PhoneLogic;
        Transform Player;

        PlayMakerFSM PhoneCord;
        PlayMakerFSM RingFSM;
        FsmString Topic;
        FsmBool PhoneBillsPaid;

        AudioSource CallScreamer;

        short neededFrames = 400;
        bool phonePaidStatusTemp = false;


        protected override void Awaked()
        {
            EventsManager.OnScreamerTriggered.AddListener(TriggerScreamer);

            Ring = transform.Find("Logic/Ring").gameObject;
            _logic = transform.Find("Logic").gameObject;
            Callers = GameObject.Find("MasterAudio/Callers");
            PhoneLogic = transform.Find("Logic/PhoneLogic").gameObject;
            PhoneBillsPaid = GameObject.Find("Systems/PhoneBills").GetPlayMaker("Data").GetVariable<FsmBool>("PhonePaid");
            Player = GameObject.Find("PLAYER").transform;

            StateHook.Inject(transform.Find("Logic/UseHandle").gameObject, "Use", "Close phone", _closePhoneHook);
            
            enabled = false;
            RingFSM = Ring.GetPlayMaker("Ring");
            Topic = RingFSM.GetVariable<FsmString>("Topic");

            PhoneCord = GameObject.Find("YARD/Building/LIVINGROOM/Telephone/Cord/PhoneCordOut/Trigger").GetComponent<PlayMakerFSM>();
            PhoneCord.AddEvent("SCREAMTRIGGER");
            PhoneCord.AddGlobalTransition("SCREAMTRIGGER", "Position");

            _addAudios();
            _addRingStateForScreamer();
            _addEventAndTransitionToScreamerState();

            Utils.PrintDebug("Phone ring awaked");
        }

        protected override void Enabled()
        {            
            _enablePhoneCord();
            PhoneLogic.SetActive(false);

            phonePaidStatusTemp = PhoneBillsPaid.Value;
            PhoneBillsPaid.Value = true;

            Topic.Value = "SCREAMCALL";
            Ring.SetActive(true);

            _logic.SetActive(true);
        }

        protected override void Disabled()
        {
            if (CallScreamer == null) return;

            Ring.SetActive(false);
            PhoneLogic.SetActive(true);
            Topic.Value = "";
            PhoneBillsPaid.Value = phonePaidStatusTemp;
            EventsManager.FinishScreamer(ScreamTimeType.FEAR, (int)ScreamFearType.PHONE);
        }

        protected override void OnFixedUpdate()
            => WorldManager.ClonedPhantomTick(200, _phantomHideCallback);


        public void TriggerScreamer(ScreamTimeType type, int variation)
        {
            if (type != ScreamTimeType.FEAR || variation != (int)ScreamFearType.PHONE) return;

            enabled = true;
        }

        void _closePhoneHook()
           => WorldManager.ClonedPhantomTick(0, _phantomHideCallback);

        void _addAudios()
        {
            var source = Player.gameObject.AddComponent<AudioSource>();
            source.clip = Globals.ScreamCall_clip;
            source.loop = false;
            source.priority = 128;
            source.volume = 1f;
            source.pitch = 1f;
            source.panStereo = 0;
            source.spatialBlend = 1f;
            source.reverbZoneMix = 1f;
            source.dopplerLevel = 1f;
            source.minDistance = 1.5f;
            source.spread = 0;
            source.maxDistance = 12f;
            CallScreamer = source;

            var source2 = Player.gameObject.AddComponent<AudioSource>();
            source2.clip = Globals.PhantomScream_clip;
            source2.loop = false;
            source2.priority = 128;
            source2.volume = 1;
            source2.pitch = 1;
            source2.panStereo = 0;
            source2.spatialBlend = 1;
            source2.reverbZoneMix = 1;
            source2.dopplerLevel = 1;
            source2.spread = 0;
            source2.minDistance = 1.5f;
            source2.maxDistance = 12f;
            Globals.PhantomScream_source = source2;
        }

        void _addRingStateForScreamer()
        {
            RingFSM.Fsm.InitData();

            FsmState newState = new FsmState(RingFSM.Fsm.States.Last());
            newState.Name = "Night screamer";
            (newState.Actions[0] as MasterAudioPlaySound).variationName.Value = "night_screamer";
            (newState.Actions[1] as SetStringValue).stringValue.Value = Locales.CALL_SCREMER_TEXT[Globals.CurrentLang]; //"I'm always watching you! I'm always with you! Behind you...";
            (newState.Actions[2] as Wait).time.Value = 4f;
            RingFSM.Fsm.States = new List<FsmState>(RingFSM.Fsm.States) { newState }.ToArray();

            StateHook.Inject(Ring, "Ring", "Night screamer", _spawnPhantomBehindPlayer, 2);
        }

        void _addEventAndTransitionToScreamerState()
        {
            RingFSM.AddEvent("SCREAMCALL");

            var state = RingFSM.GetState("State 2");
            state.Transitions = new List<FsmTransition>(state.Transitions)
            {
                new FsmTransition
                {
                    FsmEvent = RingFSM.FsmEvents.First(v=> v.Name == "SCREAMCALL"),
                    ToState = "Night screamer"
                }
            }.ToArray();
        }

        void _ignoreBillsPaidStatus(PlayMakerFSM fsm)
        {
            if (!enabled) return;

            fsm.SendEvent("FINISHED");
        }

        void _spawnPhantomBehindPlayer()
        {
            CallScreamer.Play();
            WorldManager.SpawnPhantomBehindPlayer();
        }

        void _phantomHideCallback()
        {
            CallScreamer.Stop();
            enabled = false;
        }

        void _enablePhoneCord()
        {
            PhoneCord.gameObject.transform.parent.gameObject.SetActive(true);
            PhoneCord.CallGlobalTransition("SCREAMTRIGGER");
            
            Utils.PrintDebug(eConsoleColors.YELLOW, "Phone cord enabled.");
        }
    }
}
