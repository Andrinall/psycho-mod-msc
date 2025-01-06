
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


        GameObject TapDrink;
        EllipsoidParticleEmitter TapParticle;

        Transform ShowerPower;

        FsmBool ShowerSwitch;
        FsmBool ValveSwitch;
        FsmBool Valve;
        FsmBool PlayerStop;

        bool switched = false;


        public override void InitScreamer()
        {
            TapDrink = transform.Find("TapDrink").gameObject;
            TapParticle = transform.Find("TapParticle").GetComponent<EllipsoidParticleEmitter>();
            ShowerPower = transform.parent.Find("LOD_bathroom/Power/shower_power");

            PlayMakerFSM SwitchFSM = transform.Find("Switch").GetComponent<PlayMakerFSM>();
            PlayMakerFSM ValveFSM = transform.Find("Valve").GetComponent<PlayMakerFSM>();
            Valve = ValveFSM.GetVariable<FsmBool>("Valve");

            ShowerSwitch = SwitchFSM.GetVariable<FsmBool>("ShowerSwitch");
            ValveSwitch = SwitchFSM.GetVariable<FsmBool>("Valve");
            PlayerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            StateHook.Inject(transform.Find("Valve").gameObject, "Switch", "OFF", _showerHook);
        }

        public override void TriggerScreamer() => OpenValve();

        public override void StopScreamer()
        {
            switched = false;
            PlayerStop.Value = false;
        }

        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!switched) return;
            WorldManager.ClonedPhantomTick(200, _phantomCallback);
        }


        void OpenValve()
        {
            ShowerSwitch.Value = true;
            ValveSwitch.Value = true;
            TapParticle.emit = true;

            ShowerPower.localEulerAngles = new Vector3(30f, 0f, 0f);

            TapDrink.SetActive(true);
            Valve.Value = true;
        }


        void _showerHook(PlayMakerFSM _)
        {
            if (!ScreamerEnabled) return;
            if (switched) return;
            
            WorldManager.SpawnPhantomBehindPlayer();
            
            switched = true;
            PlayerStop.Value = true;
        }

        void _phantomCallback() => base.Stop();
    }
}
