using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;


namespace Psycho.Features
{
    [RequireComponent(typeof(AudioSource), typeof(MeshRenderer), typeof(BoxCollider))]
    internal sealed class AngryRoosterPoster : CatchedComponent
    {
        bool Status = false; // false = IDLE, true = NEED CHIPS
        public bool Applyed = false;
        public int LastDayApplyed = 0;

        AudioSource AngrySounds;
        AudioSource CompleteSound;

        FsmInt GlobalDay;
        FsmFloat SUN_hours;

        MeshRenderer renderer;

        PlayMakerFSM Hand;


        protected override void Awaked()
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            AngrySounds = sources[0];
            AngrySounds.enabled = false;

            CompleteSound = sources[1];

            renderer = GetComponent<MeshRenderer>();

            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
            SUN_hours = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock").GetVariable<FsmFloat>("Hours");
            Hand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
            Utils.PrintDebug($"rooster awaked day: {GlobalDay.Value % 7}, hours: {SUN_hours.Value}, applyed");
        }

        protected override void OnFixedUpdate()
        {

            int day = GlobalDay.Value % 7;
            if (Applyed && LastDayApplyed != GlobalDay.Value)
                Applyed = false;


            if (!Status && !Applyed && day == 6 && SUN_hours.Value > 4 && SUN_hours.Value < 16)
                Activate(true);
            else if (Status && day == 6 && (SUN_hours.Value < 4 || SUN_hours.Value > 16))
            {
                Activate(false);
                Applyed = false;
            }
            else if (Status && day != 6)
            {
                Activate(false);
                Applyed = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!Status) return;
            if (other?.gameObject?.name?.Contains("potato chips") != true) return;
            
            Transform parent = other.gameObject.transform.parent;
            if (parent == null) return;

            Vector3 pos = other.gameObject.transform.position;
            Hand.CallGlobalTransition("DROP_PART");
            other.gameObject.GetComponent<PlayMakerFSM>().CallGlobalTransition("GARBAGE");

            Applyed = true;
            LastDayApplyed = GlobalDay.Value;
            Activate(false);
            ItemsPool.AddItem(Globals.BlackEgg_prefab, pos, Vector3.zero);
        }

        void Activate(bool state)
        {
            Utils.PrintDebug($"AngryRoosterPoster.Activate({state}) called");
            
            Status = state;
            AngrySounds.enabled = state;

            renderer.materials[0].SetTexture(
                "_MainTex",
                state
                    ? renderer.materials[2].mainTexture
                    : renderer.materials[1].mainTexture
            );

            if (!state)
            {
                CompleteSound.Play();
            }
        }
    }
}
