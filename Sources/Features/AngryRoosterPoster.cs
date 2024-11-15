using UnityEngine;
using HutongGames.PlayMaker;

using Psycho.Internal;
using MSCLoader;


namespace Psycho.Features
{
    [RequireComponent(typeof(AudioSource), typeof(MeshRenderer), typeof(BoxCollider))]
    internal sealed class AngryRoosterPoster : CatchedComponent
    {
        bool Status = false; // false = IDLE, true = NEED CHIPS
        bool Applyed = false;

        AudioSource AngrySounds;
        AudioSource CompleteSound;

        FsmInt GlobalDay;
        FsmFloat SUN_hours;

        MeshRenderer renderer;

        internal override void Awaked()
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            AngrySounds = sources[0];
            AngrySounds.enabled = false;

            CompleteSound = sources[1];

            renderer = GetComponent<MeshRenderer>();

            GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
            SUN_hours = GameObject.Find("MAP/SUN/Pivot/SUN").GetPlayMaker("Clock").GetVariable<FsmFloat>("Hours");
        }

        void FixedUpdate()
        {
            int day = GlobalDay.Value % 7;
            if (!Status && !Applyed && day == 6 && SUN_hours.Value > 4 && SUN_hours.Value < 16)
                Activate(true);
            else if (Status && day != 6)
            {
                Applyed = false;
                Activate(false);
            }
            else if (Status && day == 6 && (SUN_hours.Value < 4 || SUN_hours.Value > 16))
                Activate(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!Status) return;
            if (other?.gameObject?.name?.Contains("potato chips") != true) return;
            
            Transform parent = other.gameObject.transform.parent;
            if (parent == null) return;

            Vector3 pos = other.gameObject.transform.position;
            Destroy(other.gameObject);

            GameObject clone = (GameObject)Instantiate(Globals.BlackEgg_prefab, pos, Quaternion.Euler(Vector3.zero));
            clone.MakePickable();

            Applyed = true;
            Activate(false);
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

            if (!state) CompleteSound.Play();
        }
    }
}
