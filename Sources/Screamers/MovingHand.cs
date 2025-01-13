
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MovingHand : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.PARALYSIS;
        public override int ScreamerVariant => (int)ScreamParalysisType.HAND;


        const float TARGET_DISTANCE_FIRST = 0.1f;
        const float MAX_SPEED_FIRST = 0.1f;

        const float TARGET_DISTANCE_SECOND = 0.05f;
        const float MAX_SPEED_SECOND = 10f;

        const int AWAIT_FRAMES_COUNT = 120;

        readonly Vector3 CameraTargetPoint = new Vector3(-11.703310012817383f, 2.7082486152648927f, 13.318896293640137f);
        readonly Vector3 TargetPointFirst = new Vector3(-11.942817687988282f, 1.1628656387329102f, 14.70871639251709f);
        readonly Vector3 TargetPointSecond = new Vector3(-11.962800979614258f, 0.44349291920661929f, 14.862831115722657f);


        Transform armature;
        Transform rigged;
        PlayMakerFSM fsm;

        Vector3 startPoint;
        Vector3[] cameraOrigs;

        int elapsedFrames = 0;
        byte movingStage = 0;
        bool animPlayed = false;


        public override void InitScreamer()
        {
            fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            armature = transform.Find("Armature");
            rigged = transform.Find("hand_rigged");
            
            startPoint = armature.position;
        }

        public override void TriggerScreamer()
        {
            fsm.enabled = false;
            armature.position = startPoint;
            cameraOrigs = Utils.SetCameraLookAt(CameraTargetPoint);

            armature.gameObject.SetActive(true);
            rigged.gameObject.SetActive(true);
            SoundManager.PlayHeartbeat(true);
        }

        public override void StopScreamer()
        {
            fsm.enabled = true;
            animPlayed = false;
            movingStage = 0;
            elapsedFrames = 0;

            SoundManager.PlayHeartbeat(false);
            Utils.ResetCameraLook(cameraOrigs);
            fsm.CallGlobalTransition("SCREAMSTOP");

            armature.gameObject.SetActive(false);
            rigged.gameObject.SetActive(false);
        }

        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (animPlayed) return;

            if (movingStage == 0)
            {
                if (!armature.MoveTowards(TargetPointFirst, TARGET_DISTANCE_FIRST, MAX_SPEED_FIRST)) return;
                movingStage++;
                return;
            }

            if (movingStage == 1)
            {
                if (!Utils.WaitFrames(ref elapsedFrames, AWAIT_FRAMES_COUNT)) return;
                if (!armature.MoveTowards(TargetPointSecond, TARGET_DISTANCE_SECOND, MAX_SPEED_SECOND)) return;
                movingStage++;
                return;
            }

            if (movingStage == 2)
            {
                animPlayed = true;
                AudioSource.PlayClipAtPoint(ResourcesStorage.HandDroppedToFace_clip, transform.position, 1f);
                ShizAnimPlayer.PlayOriginalAnimation("sleep_knockout", 4f, default, base.Stop);
            }
        }
    }
}
