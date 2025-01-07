
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


        GameObject particleDrink;
        GameObject switchObj;
        Transform pivot;

        PlayMakerFSM switchFSM;
        FsmBool switchOn;
        FsmBool playerStop;

        bool switched = false;


        public override void InitScreamer()
        {
            particleDrink = transform.Find("ParticleDrink").gameObject;
            switchObj = transform.Find("Trigger").gameObject;
            switchFSM = switchObj.GetComponent<PlayMakerFSM>();
            switchOn = switchFSM.GetVariable<FsmBool>("SwitchOn");

            pivot = transform.Find("Handle/Pivot");
            playerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            StateHook.Inject(switchObj, "Use", "OFF", _showerHook);
        }

        public override void TriggerScreamer()
        {
            pivot.localEulerAngles = new Vector3(-17f, 0f, 0f);
            switchOn.Value = true;
            particleDrink.SetActive(true);
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
            playerStop.Value = true;
        }


        void _phantomCallback()
        {
            switched = false;

            playerStop.Value = false;
            base.Stop();
        }
    }
}
