
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    class MummolaCrawl : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.PARALYSIS;
        public override int ScreamerVariant => (int)ScreamParalysisType.GRANNY;


        const int NEEDED_FRAMES = 240;
        const float TARGET_DISTANCE = 0.1f;
        const float MAX_SPEED = 0.9f;

        readonly Vector3 StartPoint = new Vector3(-8.40377522f, 2.46443129f, 9.5422678f);
        readonly Vector3 TargetPoint = new Vector3(-11.2399998f, 2.5150001f, 12.6759996f);
        readonly Vector3 HeadEndRotation = new Vector3(80f, 270f, 0f);

        Transform charTransform;
        Transform headTransform;

        PlayMakerFSM fsm;

        Vector3[] cameraOrigs;

        int elapsedFrames = 0;
        bool animPlayed = false;


        float InterpolationRatio => (float)elapsedFrames / NEEDED_FRAMES;


        public override void InitScreamer()
        {
            fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            charTransform = transform.Find("Char");
            headTransform = charTransform.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");
        }

        public override void TriggerScreamer()
        {
            fsm.enabled = false;
            transform.position = StartPoint;
            transform.eulerAngles = new Vector3(347.788879f, 331.232269f, 180f);

            cameraOrigs = Utils.SetCameraLookAt(TargetPoint);
            ResetHeadRotation();
            charTransform.gameObject.SetActive(true);
            SoundManager.PlayHeartbeat(true);

            AudioSource.PlayClipAtPoint(ResourcesStorage.GrannyCrawlScreamer_clip, transform.position, 1f);
        }

        public override void StopScreamer()
        {
            fsm.enabled = true;
            elapsedFrames = 0;
            animPlayed = false;
            
            Utils.ResetCameraLook(cameraOrigs);
            fsm.CallGlobalTransition("SCREAMSTOP");

            charTransform.gameObject.SetActive(false);
            ResetHeadRotation();
            SoundManager.PlayHeartbeat(false);
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!transform.MoveTowards(TargetPoint, TARGET_DISTANCE, MAX_SPEED)) return;
            RotateHeadPivot();
        }

        void ResetHeadRotation()
            => headTransform.localEulerAngles = new Vector3(270f, 90f, 0f);


        void RotateHeadPivot()
        {
            if (animPlayed) return;

            if (elapsedFrames == NEEDED_FRAMES)
            {
                animPlayed = true;
                ShizAnimPlayer.PlayOriginalAnimation("sleep_knockout", 4f, default, base.Stop);
                return;
            }

            headTransform.localEulerAngles = Vector3.Lerp(headTransform.localEulerAngles, HeadEndRotation, InterpolationRatio);
            elapsedFrames++;
        }
    }
}
