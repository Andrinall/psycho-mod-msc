
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;


namespace Psycho.Screamers
{
    class PhoneScreamer : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.FEAR;
        public override int ScreamerVariant => (int)ScreamFearType.PHONE;


        GameObject ring;
        GameObject logic;
        GameObject phoneLogic;
        Transform player;

        PlayMakerFSM phoneCord;
        PlayMakerFSM ringFSM;
        FsmString topic;
        FsmBool phoneBillsPaid;

        AudioSource callScreamer;

        bool phonePaidStatusTemp = false;
        bool phoneHandlePickedUp = false;


        public override void InitScreamer()
        {
            ring = transform.Find("Logic/Ring").gameObject;
            ringFSM = ring.GetPlayMaker("Ring");
            topic = ringFSM.GetVariable<FsmString>("Topic");

            logic = transform.Find("Logic").gameObject;
            phoneLogic = transform.Find("Logic/PhoneLogic").gameObject;
            phoneBillsPaid = GameObject.Find("Systems/PhoneBills").GetPlayMaker("Data").GetVariable<FsmBool>("PhonePaid");
            player = GameObject.Find("PLAYER").transform;

            GameObject _useHandle = transform.Find("Logic/UseHandle").gameObject;
            StateHook.Inject(_useHandle, "Use", "Close phone", _closePhoneHook);
            StateHook.Inject(_useHandle, "Use", "Pick phone", _pickPhoneHook);

            phoneCord = GameObject.Find("YARD/Building/LIVINGROOM/Telephone/Cord/PhoneCordOut/Trigger").GetComponent<PlayMakerFSM>();
            phoneCord.AddEvent("SCREAMTRIGGER");
            phoneCord.AddGlobalTransition("SCREAMTRIGGER", "Position");

            _addAudios();
            _addRingStateForScreamer();
            _addEventAndTransitionToScreamerState();
        }

        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!phoneHandlePickedUp) return;

            WorldManager.ClonedPhantomTick(200, _phantomHideCallback);
        }


        public override void TriggerScreamer()
        {
            _enablePhoneCord();
            phoneLogic.SetActive(false);

            phonePaidStatusTemp = phoneBillsPaid.Value;
            phoneBillsPaid.Value = true;

            topic.Value = "SCREAMCALL";
            ring.SetActive(true);
            logic.SetActive(true);
        }


        public override void StopScreamer()
        {
            ring.SetActive(false);
            phoneLogic.SetActive(true);
            topic.Value = "";
            phoneBillsPaid.Value = phonePaidStatusTemp;
            phoneHandlePickedUp = false;
        }


        void _pickPhoneHook()
        {
            if (!ScreamerEnabled) return;
            
            phoneHandlePickedUp = true;
        }

        void _closePhoneHook()
        {
            if (!ScreamerEnabled) return;
            if (!phoneHandlePickedUp) return;

            WorldManager.ClonedPhantomTick(0, _phantomHideCallback);
            base.Stop();
        }


        void _spawnPhantomBehindPlayer()
        {
            callScreamer.Play();
            WorldManager.SpawnPhantomBehindPlayer();
        }


        void _phantomHideCallback()
        {
            callScreamer.Stop();
            base.Stop();
        }


        void _addAudios()
        {
            AudioSource _source = player.gameObject.AddComponent<AudioSource>();
            _source.clip = ResourcesStorage.ScreamCall_clip;
            _source.loop = false;
            _source.priority = 128;
            _source.volume = 1f;
            _source.pitch = 1f;
            _source.panStereo = 0;
            _source.spatialBlend = 1f;
            _source.reverbZoneMix = 1f;
            _source.dopplerLevel = 1f;
            _source.minDistance = 1.5f;
            _source.spread = 0;
            _source.maxDistance = 12f;
            callScreamer = _source;

            AudioSource _source2 = player.gameObject.AddComponent<AudioSource>();
            _source2.clip = ResourcesStorage.PhantomScream_clip;
            _source2.loop = false;
            _source2.priority = 128;
            _source2.volume = 1;
            _source2.pitch = 1;
            _source2.panStereo = 0;
            _source2.spatialBlend = 1;
            _source2.reverbZoneMix = 1;
            _source2.dopplerLevel = 1;
            _source2.spread = 0;
            _source2.minDistance = 1.5f;
            _source2.maxDistance = 12f;
            Globals.PhantomScream_source = _source2;
        }


        void _addRingStateForScreamer()
        {
            ringFSM.Fsm.InitData();

            FsmState _newState = new FsmState(ringFSM.Fsm.States.Last());
            _newState.Name = "Night screamer";
            (_newState.Actions[0] as MasterAudioPlaySound).variationName.Value = "night_screamer";
            (_newState.Actions[1] as SetStringValue).stringValue.Value = Locales.CALL_SCREMER_TEXT[Globals.CurrentLang]; //"I'm always watching you! I'm always with you! Behind you...";
            (_newState.Actions[2] as Wait).time.Value = 4f;
            ringFSM.Fsm.States = new List<FsmState>(ringFSM.Fsm.States) { _newState }.ToArray();

            StateHook.Inject(ring, "Ring", "Night screamer", _spawnPhantomBehindPlayer, 2);
        }


        void _addEventAndTransitionToScreamerState()
        {
            ringFSM.AddEvent("SCREAMCALL");

            FsmState _state = ringFSM.GetState("State 2");
            _state.Transitions = new List<FsmTransition>(_state.Transitions)
            {
                new FsmTransition
                {
                    FsmEvent = ringFSM.FsmEvents.First(v=> v.Name == "SCREAMCALL"),
                    ToState = "Night screamer"
                }
            }.ToArray();
        }


        void _ignoreBillsPaidStatus(PlayMakerFSM fsm)
        {
            if (!ScreamerEnabled) return;
            fsm.SendEvent("FINISHED");
        }


        void _enablePhoneCord()
        {
            GameObject _cord = phoneCord.gameObject.transform.parent.gameObject;
            if (_cord.activeSelf) return;

            _cord.SetActive(true);
            phoneCord.CallGlobalTransition("SCREAMTRIGGER");
        }
    }
}
