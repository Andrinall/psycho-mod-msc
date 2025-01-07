
using MSCLoader;
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    [RequireComponent(typeof(AudioSource), typeof(MeshRenderer), typeof(BoxCollider))]
    internal sealed class AngryRoosterPoster : CatchedComponent
    {
        public bool Applyed = false;
        public int LastDayApplyed = 0;

        bool status = false; // false = IDLE, true = NEED CHIPS

        AudioSource angrySounds;
        AudioSource completeSound;

        MeshRenderer renderer;
        PlayMakerFSM hand;


        protected override void Awaked()
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            angrySounds = sources[0];
            completeSound = sources[1];
            angrySounds.enabled = false;

            renderer = GetComponent<MeshRenderer>();

            hand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
            Utils.PrintDebug(eConsoleColors.YELLOW, $"[RoosterPoster.Awaked()] day: {Globals.GlobalDay.Value % 7}; hours: {Globals.SUN_Hours.Value}; applyed: {Applyed}; lastDayApplyed: {LastDayApplyed}");
        }

        protected override void OnFixedUpdate()
        {
            int day = Globals.GlobalDay.Value % 7;
            if (Applyed && LastDayApplyed != Globals.GlobalDay.Value)
                Applyed = false;


            if (!status && !Applyed && day == 6 && Globals.SUN_Hours.Value > 4 && Globals.SUN_Hours.Value < 16)
                Activate(true);
            else if (status && day == 6 && (Globals.SUN_Hours.Value < 4 || Globals.SUN_Hours.Value > 16))
            {
                Activate(false);
                Applyed = false;
            }
            else if (status && day != 6)
            {
                Activate(false);
                Applyed = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!status) return;
            if (other?.gameObject?.name?.Contains("potato chips") != true) return;
            
            Transform parent = other.gameObject.transform.parent;
            if (parent == null) return;

            Vector3 pos = other.gameObject.transform.position;
            hand.CallGlobalTransition("DROP_PART");
            other.gameObject.GetComponent<PlayMakerFSM>().CallGlobalTransition("GARBAGE");

            Applyed = true;
            LastDayApplyed = Globals.GlobalDay.Value;
            Activate(false);
            ItemsPool.AddItem(ResourcesStorage.BlackEgg_prefab, pos, Vector3.zero);
        }

        void Activate(bool state)
        {
            Utils.PrintDebug($"AngryRoosterPoster.Activate({state}) called");
            
            status = state;
            angrySounds.enabled = state;

            renderer.materials[0].SetTexture(
                "_MainTex",
                state
                    ? renderer.materials[2].mainTexture
                    : renderer.materials[1].mainTexture
            );

            if (!state)
            {
                completeSound.Play();
            }
        }
    }
}
