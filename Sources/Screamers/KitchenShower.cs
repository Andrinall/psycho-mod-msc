using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class KitchenShower : CatchedComponent
    {
        GameObject ParticleDrink;
        GameObject Switch;
        Transform Pivot;

        PlayMakerFSM SwitchFSM;
        FsmBool SwitchOn;
        FsmBool PlayerStop;

        bool switched = false;


        protected override void Awaked()
        {
            enabled = false;
            ParticleDrink = transform.Find("ParticleDrink").gameObject;
            Switch = transform.Find("Trigger").gameObject;
            SwitchFSM = Switch.GetComponent<PlayMakerFSM>();
            SwitchOn = SwitchFSM.GetVariable<FsmBool>("SwitchOn");

            Pivot = transform.Find("Handle/Pivot");
            PlayerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            StateHook.Inject(Switch, "Use", "OFF", _showerHook);
            EventsManager.OnScreamerTriggered.AddListener(TriggerScreamer);
        }


        protected override void Enabled()
        {
            Pivot.localEulerAngles = new Vector3(-17f, 0f, 0f);
            SwitchOn.Value = true;
            ParticleDrink.SetActive(true);
        }

        protected override void Disabled()
        {
            if (ParticleDrink == null) return;
            EventsManager.FinishScreamer(ScreamTimeType.FEAR, (int)ScreamFearType.WATERKITCHEN);
        }

        protected override void OnFixedUpdate()
        {
            if (!switched) return;
            WorldManager.ClonedPhantomTick(200, _phantomCallback);
        }

        void TriggerScreamer(ScreamTimeType type, int variation)
        {
            if (type != ScreamTimeType.FEAR || (ScreamFearType)variation != ScreamFearType.WATERKITCHEN) return;

            enabled = true;
        }


        void _showerHook(PlayMakerFSM _)
        {
            SoundManager.StopScreamSound("kitchen_water");
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
