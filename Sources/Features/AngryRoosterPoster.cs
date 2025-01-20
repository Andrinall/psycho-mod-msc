
using MSCLoader;
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    [RequireComponent(typeof(AudioSource), typeof(MeshRenderer), typeof(BoxCollider))]
    class AngryRoosterPoster : CatchedComponent
    {
        public static bool Applyed = false;
        public static int LastDayApplyed = 0;

        bool status = false; // false = IDLE, true = NEED CHIPS

        AudioSource angrySounds;
        AudioSource completeSound;

        MeshRenderer renderer;
        PlayMakerFSM hand;


        protected override void Awaked()
        {
            AudioSource[] _sources = GetComponents<AudioSource>();
            angrySounds = _sources[0];
            completeSound = _sources[1];
            angrySounds.enabled = false;

            renderer = GetComponent<MeshRenderer>();

            hand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand").GetPlayMaker("PickUp");
        }

        protected override void OnFixedUpdate()
        {
            int _day = Globals.GlobalDay.Value % 7;
            if (Applyed && LastDayApplyed != Globals.GlobalDay.Value)
                Applyed = false;


            if (!status && !Applyed && _day == 6 && Globals.SUN_Hours > 4 && Globals.SUN_Hours < 16)
                Activate(true);
            else if (status && _day == 6 && (Globals.SUN_Hours < 4 || Globals.SUN_Hours > 16))
            {
                Activate(false);
                Applyed = false;
            }
            else if (status && _day != 6)
            {
                Activate(false);
                Applyed = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!status) return;
            if (other?.gameObject?.name?.Contains("potato chips") != true) return;
            
            Transform _parent = other.gameObject.transform.parent;
            if (_parent == null) return;

            Vector3 _pos = other.gameObject.transform.position;
            hand.CallGlobalTransition("DROP_PART");
            other.gameObject.GetComponent<PlayMakerFSM>().CallGlobalTransition("GARBAGE");

            Applyed = true;
            LastDayApplyed = Globals.GlobalDay.Value;
            Activate(false);
            ItemsPool.AddItem(ResourcesStorage.BlackEgg_prefab, _pos, Vector3.zero);
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
