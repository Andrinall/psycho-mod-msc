
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MummolaCrawl : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.PARALYSIS;
        public override int ScreamerVariant => (int)ScreamParalysisType.GRANNY;


        int HeadInterpolationFrames = 240;
        float TargetDistance = 0.1f;
        float MaxSpeed = 0.9f;

        Vector3 StartPoint = new Vector3(-8.40377522f, 2.46443129f, 9.5422678f);
        Vector3 TargetPoint = new Vector3(-11.2399998f, 2.5150001f, 12.6759996f);
        Vector3 HeadEndRotation = new Vector3(80f, 270f, 0f);

        Transform Char;
        Transform Head;

        PlayMakerFSM Fsm;

        Vector3[] cameraOrigs;

        int ElapsedFrames = 0;
        bool AnimPlayed = false;


        float InterpolationRatio => (float)ElapsedFrames / HeadInterpolationFrames;


        public override void InitScreamer()
        {
            Fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Char = transform.Find("Char");
            Head = Char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");
        }

        public override void TriggerScreamer()
        {
            Fsm.enabled = false;
            transform.position = StartPoint;
            transform.eulerAngles = new Vector3(347.788879f, 331.232269f, 180f);

            cameraOrigs = Utils.SetCameraLookAt(TargetPoint);
            ResetHeadRotation();
            Char.gameObject.SetActive(true);
            SoundManager.PlayHeartbeat(true);

            AudioSource.PlayClipAtPoint(Globals.GrannyCrawlScreamer_clip, transform.position, 1f);
        }

        public override void StopScreamer()
        {
            ElapsedFrames = 0;

            Char.gameObject.SetActive(false);
            ResetHeadRotation();
            SoundManager.PlayHeartbeat(false);
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!transform.MoveTowards(TargetPoint, TargetDistance, MaxSpeed)) return;
            RotateHeadPivot();
        }

        void ResetHeadRotation()
            => Head.localEulerAngles = new Vector3(270f, 90f, 0f);


        void RotateHeadPivot()
        {
            if (ElapsedFrames == HeadInterpolationFrames)
            {
                Utils.PlayScreamSleepAnim(ref AnimPlayed, () =>
                {
                    Fsm.enabled = true;
                    Utils.ResetCameraLook(cameraOrigs);
                    Fsm.CallGlobalTransition("SCREAMSTOP");
                    base.Stop();
                });
                return;
            }

            Head.localEulerAngles = Vector3.Lerp(Head.localEulerAngles, HeadEndRotation, InterpolationRatio);
            ElapsedFrames++;
        }
    }
}
