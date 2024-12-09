using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class BathroomShower : CatchedComponent
    {
        GameObject TapDrink;
        EllipsoidParticleEmitter TapParticle;

        Transform ShowerPower;

        FsmBool ShowerSwitch;
        FsmBool ValveSwitch;
        FsmBool Valve;
        FsmBool PlayerStop;

        bool switched = false;


        internal override void Awaked()
        {
            enabled = false;
            TapDrink = transform.Find("TapDrink").gameObject;
            TapParticle = transform.Find("TapParticle").GetComponent<EllipsoidParticleEmitter>();
            ShowerPower = transform.parent.Find("LOD_bathroom/Power/shower_power");

            PlayMakerFSM SwitchFSM = transform.Find("Switch").GetComponent<PlayMakerFSM>();
            PlayMakerFSM ValveFSM = transform.Find("Valve").GetComponent<PlayMakerFSM>();
            Valve = ValveFSM.GetVariable<FsmBool>("Valve");

            ShowerSwitch = SwitchFSM.GetVariable<FsmBool>("ShowerSwitch");
            ValveSwitch = SwitchFSM.GetVariable<FsmBool>("Valve");
            PlayerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            StateHook.Inject(transform.Find("Valve").gameObject, "Switch", "OFF", 0, _showerHook);
            EventsManager.OnScreamerTriggered.AddListener(TriggerScreamer);
        }

        internal override void Enabled() => SwitchValve(true);

        internal override void Disabled()
            => EventsManager.FinishScreamer(ScreamTimeType.FEAR, (int)ScreamFearType.WATERBATHROOM);

        internal override void OnFixedUpdate()
        {
            if (!switched) return;
            WorldManager.ClonedPhantomTick(200, _phantomCallback);
        }

        void TriggerScreamer(ScreamTimeType type, int variation)
        {
            if (type != ScreamTimeType.FEAR || (ScreamFearType)variation != ScreamFearType.WATERBATHROOM) return;

            enabled = true;
        }

        void SwitchValve(bool state)
        {
            ShowerSwitch.Value = state;
            ValveSwitch.Value = state;
            TapParticle.emit = state;

            ShowerPower.localEulerAngles = new Vector3(30f, 0f, 0f);

            TapDrink.SetActive(state);
            Valve.Value = state;
        }


        void _showerHook(PlayMakerFSM _)
        {
            if (!enabled) return;
            if (switched) return;
            
            WorldManager.SpawnPhantomBehindPlayer();
            
            switched = true;
            PlayerStop.Value = true;
        }

        void _phantomCallback()
        {
            PlayerStop.Value = false;
            switched = false;
            enabled = false;
        }
    }
}
