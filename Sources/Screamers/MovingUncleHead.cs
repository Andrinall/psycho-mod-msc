using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MovingUncleHead : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.PARALYSIS;
        public override int ScreamerVariant => (int)ScreamParalysisType.KESSELI;

        const int neededFrames = 100;
        const float MaxSpeed = 0.26f;
        const float TargetDistance = 0.07f;

        readonly Vector3 TargetPoint = new Vector3(-12.035453796386719f, 0.24380475282669068f, 13.690605163574219f);


        Transform Head;
        Transform Char;
        PlayMakerFSM _fsm;

        Vector3 StartPoint;
        Vector3[] CameraOrigs;

        int elapsedFrames = 0;
        bool animPlayed = false;


        public override void InitScreamer()
        {
            _fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Char = transform.Find("Char");
            Head = Char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot/head");
            StartPoint = Head.position;

            var source = Head.gameObject.AddComponent<AudioSource>();
            source.clip = Globals.UncleScream_clip;
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
        }

        public override void TriggerScreamer()
        {
            _fsm.enabled = false;
            Head.position = StartPoint;
            CameraOrigs = Utils.SetCameraLookAt(Head.position);
            Char.gameObject.SetActive(true);
            Head.gameObject.SetActive(false);
        }

        public override void StopScreamer()
        {
            animPlayed = false;
            Char.gameObject.SetActive(false);
            elapsedFrames = 0;
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
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
                SoundManager.PlayHeartbeat(false);
                Utils.ResetCameraLook(CameraOrigs);
                _fsm.CallGlobalTransition("SCREAMSTOP");
                base.Stop();
            });
        }
    }
}