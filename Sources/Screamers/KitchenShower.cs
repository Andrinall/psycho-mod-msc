using UnityEngine;

using MSCLoader;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Screamers
{
    public sealed class KitchenShower : MonoBehaviour
    {
        GameObject ParticleDrink;
        GameObject Switch;
        Transform Pivot;

        PlayMakerFSM SwitchFSM;
        FsmBool SwitchOn;
        FsmBool PlayerStop;

        bool switched = false;


        void Awake()
        {
            ParticleDrink = transform.Find("ParticleDrink").gameObject;
            Switch = transform.Find("Trigger").gameObject;
            SwitchFSM = Switch.GetComponent<PlayMakerFSM>();
            SwitchOn = SwitchFSM.GetVariable<FsmBool>("SwitchOn");

            Pivot = transform.Find("Handle/Pivot");
            PlayerStop = Utils.GetGlobalVariable<FsmBool>("PlayerStop");

            StateHook.Inject(Switch, "Use", "OFF", 0, _showerHook);
            enabled = false;

            EventsManager.OnScreamerTriggered.AddListener(TriggerScreamer);
        }


        void OnEnable()
        {
            Pivot.localEulerAngles = new Vector3(-17f, 0f, 0f);
            SwitchOn.Value = true;
            ParticleDrink.SetActive(true);
        }

        void OnDisable() => EventsManager.FinishScreamer(ScreamTimeType.FEAR, (int)ScreamFearType.WATERKITCHEN);

        void FixedUpdate()
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
