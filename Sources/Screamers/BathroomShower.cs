
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class BathroomShower : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.FEAR;
        public override int ScreamerVariant => (int)ScreamFearType.WATERBATHROOM;


        GameObject tapDrink;
        EllipsoidParticleEmitter tapParticle;

        Transform showerPower;

        FsmBool showerSwitch;
        FsmBool valveSwitch;
        FsmBool valve;
        FsmBool playerStop;

        bool switched = false;


        public override void InitScreamer()
        {
            tapDrink = transform.Find("TapDrink").gameObject;
            tapParticle = transform.Find("TapParticle").GetComponent<EllipsoidParticleEmitter>();
            showerPower = transform.parent.Find("LOD_bathroom/Power/shower_power");

            PlayMakerFSM _switchFSM = transform.Find("Switch").GetComponent<PlayMakerFSM>();
            PlayMakerFSM _valveFSM = transform.Find("Valve").GetComponent<PlayMakerFSM>();
            valve = _valveFSM.GetVariable<FsmBool>("Valve");

            showerSwitch = _switchFSM.GetVariable<FsmBool>("ShowerSwitch");
            valveSwitch = _switchFSM.GetVariable<FsmBool>("Valve");
            playerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            StateHook.Inject(transform.Find("Valve").gameObject, "Switch", "OFF", _showerHook);
        }

        public override void TriggerScreamer() => OpenValve();

        public override void StopScreamer()
        {
            switched = false;
            playerStop.Value = false;
        }

        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!switched) return;
            WorldManager.ClonedPhantomTick(200, _phantomCallback);
        }


        void OpenValve()
        {
            showerSwitch.Value = true;
            valveSwitch.Value = true;
            tapParticle.emit = true;

            showerPower.localEulerAngles = new Vector3(30f, 0f, 0f);

            tapDrink.SetActive(true);
            valve.Value = true;
        }


        void _showerHook(PlayMakerFSM _)
        {
            if (!ScreamerEnabled) return;
            if (switched) return;
            
            WorldManager.SpawnPhantomBehindPlayer();
            
            switched = true;
            playerStop.Value = true;
        }

        void _phantomCallback() => base.Stop();
    }
}
