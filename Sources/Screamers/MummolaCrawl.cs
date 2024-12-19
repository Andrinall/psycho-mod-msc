using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MummolaCrawl : CatchedComponent
    {
        Transform Char;
        Transform Head;

        Animation Anim;
        AnimationClip PigWalk;

        PlayMakerFSM Fsm;

        bool AnimPlayed = false;

        int HeadInterpolationFrames = 240;
        int ElapsedFrames = 0;
        float InterpolationRatio => (float)ElapsedFrames / HeadInterpolationFrames;

        float TargetDistance = 0.1f;
        float MaxSpeed = 0.9f;

        Vector3 StartPoint = new Vector3(-8.40377522f, 2.46443129f, 9.5422678f);
        Vector3 TargetPoint = new Vector3(-11.2399998f, 2.5150001f, 12.6759996f);
        Vector3 HeadEndRotation = new Vector3(80f, 270f, 0f);

        Vector3[] cameraOrigs;


        public override void Awaked()
        {
            enabled = false;
            Fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Char = transform.Find("Char");
            Head = Char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");

            EventsManager.OnScreamerTriggered.AddListener(TriggerScreamer);
        }

        public override void Enabled()
        {
            Fsm.enabled = false;
            transform.position = StartPoint;
            transform.eulerAngles = new Vector3(347.788879f, 331.232269f, 180f);
            
            cameraOrigs = Utils.SetCameraLookAt(TargetPoint);
            ResetHeadRotation();
            Char.gameObject.SetActive(true);
            SoundManager.PlayHeartbeat(true);
        }

        public override void Disabled()
        {
            if (Char == null) return;

            ElapsedFrames = 0;
            AnimPlayed = false;

            Char.gameObject.SetActive(false);            
            ResetHeadRotation();
            SoundManager.PlayHeartbeat(false);
            EventsManager.FinishScreamer(ScreamTimeType.PARALYSIS, (int)ScreamParalysisType.GRANNY);
        }

        public override void OnFixedUpdate()
        {
            if (AnimPlayed) return;
            if (!transform.MoveTowards(TargetPoint, TargetDistance, MaxSpeed)) return;
            RotateHeadPivot();
        }

        void TriggerScreamer(ScreamTimeType type, int variation)
        {
            if (type != ScreamTimeType.PARALYSIS || (ScreamParalysisType)variation != ScreamParalysisType.GRANNY) return;

            enabled = true;
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
                    enabled = false;
                    Utils.ResetCameraLook(cameraOrigs);
                    Fsm.CallGlobalTransition("SCREAMSTOP");
                });
                return;
            }

            Head.localEulerAngles = Vector3.Lerp(Head.localEulerAngles, HeadEndRotation, InterpolationRatio);
            ElapsedFrames++;
        }
    }
}
