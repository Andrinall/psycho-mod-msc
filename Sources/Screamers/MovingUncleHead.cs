using UnityEngine;

using Psycho.Internal;
using Psycho.Extensions;


namespace Psycho.Screamers
{
    public sealed class MovingUncleHead : MonoBehaviour
    {
        Transform Head;
        Transform Char;
        PlayMakerFSM _fsm;

        AudioSource ScreamSound;

        Vector3 StartPoint;
        Vector3 TargetPoint = new Vector3(-12.035453796386719f, 0.24380475282669068f, 13.690605163574219f);

        Vector3[] CameraOrigs;

        float TargetDistance = 0.07f;
        float MaxSpeed = 0.26f;

        int elapsedFrames = 0;
        int neededFrames = 100;

        bool animPlayed = false;

        void Awake()
        {
            _fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Char = transform.Find("Char");
            Head = Char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot/head");
            StartPoint = Head.position;

            var source = Head.gameObject.AddComponent<AudioSource>();
            source.clip = Globals.UncleScreamSound;
            source.playOnAwake = true;
            source.loop = false;
            source.priority = 128;
            source.volume = 1f;
            source.pitch = 1f;
            source.panStereo = 0;
            source.spatialBlend = 1f;
            source.reverbZoneMix = 1f;
            source.dopplerLevel = 1f;
            source.minDistance = 1.5f;
            source.spread = 0;
            source.maxDistance = 12f;
            ScreamSound = source;

            enabled = false;
        }

        void OnEnable()
        {
            _fsm.enabled = false;
            Head.position = StartPoint;
            CameraOrigs = Utils.SetCameraLookAt(Head.position);
            Char.gameObject.SetActive(true);
            Head.gameObject.SetActive(false);
            WorldManager.ShowCrows(false);
        }
        
        void OnDisable()
        {
            animPlayed = false;
            Char.gameObject.SetActive(false);
            elapsedFrames = 0;
            WorldManager.ShowCrows(true);
        }

        void FixedUpdate()
        {
            if (elapsedFrames < neededFrames)
            {
                elapsedFrames++;
                if (elapsedFrames == neededFrames)
                {
                    Head.gameObject.SetActive(true);
                    SoundManager.PlayHeartbeat(true);
                }
                return;
            }

            if (animPlayed) return;
            if (!Head.MoveTowards(TargetPoint, TargetDistance, MaxSpeed)) return;

            Utils.PlayScreamSleepAnim(ref animPlayed, () =>
            {
                _fsm.enabled = true;
                enabled = false;
                SoundManager.PlayHeartbeat(false);
                Utils.ResetCameraLook(CameraOrigs);
                _fsm.CallGlobalTransition("SCREAMSTOP");
            });
        }
    }
}