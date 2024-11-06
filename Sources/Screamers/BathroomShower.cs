using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    public sealed class BathroomShower : MonoBehaviour
    {
        GameObject TapDrink;
        EllipsoidParticleEmitter TapParticle;

        Transform ShowerPower;

        FsmBool ShowerSwitch;
        FsmBool ValveSwitch;
        FsmBool Valve;
        FsmBool PlayerStop;

        bool switched = false;

        void Awake()
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

            StateHook.Inject(transform.Find("Valve").gameObject, "Switch", "OFF", 0, _showerHook);
            enabled = false;
        }

        void OnEnable()
            => SwitchValve(true);

        void FixedUpdate()
        {
            if (!switched) return;
            WorldManager.ClonedPhantomTick(200, _phantomCallback);
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
