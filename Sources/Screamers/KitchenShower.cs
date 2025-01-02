using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class KitchenShower : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.FEAR;
        public override int ScreamerVariant => (int)ScreamFearType.WATERKITCHEN;


        GameObject ParticleDrink;
        GameObject Switch;
        Transform Pivot;

        PlayMakerFSM SwitchFSM;
        FsmBool SwitchOn;
        FsmBool PlayerStop;

        bool switched = false;


        public override void InitScreamer()
        {
            ParticleDrink = transform.Find("ParticleDrink").gameObject;
            Switch = transform.Find("Trigger").gameObject;
            SwitchFSM = Switch.GetComponent<PlayMakerFSM>();
            SwitchOn = SwitchFSM.GetVariable<FsmBool>("SwitchOn");

            Pivot = transform.Find("Handle/Pivot");
            PlayerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            StateHook.Inject(Switch, "Use", "OFF", _showerHook);
        }

        public override void TriggerScreamer()
        {
            Pivot.localEulerAngles = new Vector3(-17f, 0f, 0f);
            SwitchOn.Value = true;
            ParticleDrink.SetActive(true);
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!switched) return;
            WorldManager.ClonedPhantomTick(200, _phantomCallback);
        }




        void _showerHook(PlayMakerFSM _)
        {
            if (!ScreamerEnabled) return;
            if (switched) return;

            SoundManager.StopScreamSound("kitchen_water");
            WorldManager.SpawnPhantomBehindPlayer();
            switched = true;
            PlayerStop.Value = true;
        }


        void _phantomCallback()
        {
            switched = false;

            PlayerStop.Value = false;
            base.Stop();
        }
    }
}
