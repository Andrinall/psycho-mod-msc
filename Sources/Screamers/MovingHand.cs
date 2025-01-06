
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MovingHand : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.PARALYSIS;
        public override int ScreamerVariant => (int)ScreamParalysisType.HAND;


        const float TargetDistanceFirst = 0.1f;
        const float MaxSpeedFirst = 0.1f;

        const float TargetDistanceSecond = 0.05f;
        const float MaxSpeedSecond = 10f;

        const int awaitFramesCount = 120;

        readonly Vector3 CameraTargetPoint = new Vector3(-11.703310012817383f, 2.7082486152648927f, 13.318896293640137f);
        readonly Vector3 TargetPointFirst = new Vector3(-11.942817687988282f, 1.1628656387329102f, 14.70871639251709f);
        readonly Vector3 TargetPointSecond = new Vector3(-11.962800979614258f, 0.44349291920661929f, 14.862831115722657f);


        Transform Armature;
        Transform Rigged;
        PlayMakerFSM _fsm;

        Vector3 StartPoint;
        Vector3[] CameraOrigs;

        int elapsedFrames = 0;
        byte movingStage = 0;
        bool animPlayed = false;


        public override void InitScreamer()
        {
            _fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Armature = transform.Find("Armature");
            Rigged = transform.Find("hand_rigged");
            
            StartPoint = Armature.position;
        }

        public override void TriggerScreamer()
        {
            _fsm.enabled = false;
            Armature.position = StartPoint;
            CameraOrigs = Utils.SetCameraLookAt(CameraTargetPoint);

            Armature.gameObject.SetActive(true);
            Rigged.gameObject.SetActive(true);
            SoundManager.PlayHeartbeat(true);
        }

        public override void StopScreamer()
        {
            animPlayed = false;
            movingStage = 0;
            elapsedFrames = 0;

            Armature.gameObject.SetActive(false);
            Rigged.gameObject.SetActive(false);
        }

        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (animPlayed) return;

            if (movingStage == 0)
            {
                if (!Armature.MoveTowards(TargetPointFirst, TargetDistanceFirst, MaxSpeedFirst)) return;
                movingStage++;
                return;
            }

            if (movingStage == 1)
            {
                if (elapsedFrames < awaitFramesCount)
                {
                    elapsedFrames++;
                    return;
                }

                if (!Armature.MoveTowards(TargetPointSecond, TargetDistanceSecond, MaxSpeedSecond)) return;
                movingStage++;
                return;
            }

            if (movingStage == 2)
            {
                AudioSource.PlayClipAtPoint(Globals.HandDroppedToFace_clip, transform.position, 1f);

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
}
