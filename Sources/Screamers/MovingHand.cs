using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MovingHand : CatchedComponent
    {
        Transform Armature;
        Transform Rigged;
        PlayMakerFSM _fsm;

        Vector3 StartPoint;
        Vector3 CameraTargetPoint = new Vector3(-11.703310012817383f, 2.7082486152648927f, 13.318896293640137f);

        Vector3 TargetPointFirst = new Vector3(-11.942817687988282f, 1.1628656387329102f, 14.70871639251709f);
        float TargetDistanceFirst = 0.1f;
        float MaxSpeedFirst = 0.1f;

        Vector3 TargetPointSecond = new Vector3(-11.962800979614258f, 0.44349291920661929f, 14.862831115722657f);
        float TargetDistanceSecond = 0.05f;
        float MaxSpeedSecond = 10f;
        
        byte movingStage = 0;
        bool animPlayed = false;

        int elapsedFrames = 0;
        int awaitFramesCount = 120;

        Vector3[] CameraOrigs;


        protected override void Awaked()
        {
            enabled = false;

            _fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Armature = transform.Find("Armature");
            Rigged = transform.Find("hand_rigged");
            
            StartPoint = Armature.position;

            EventsManager.OnScreamerTriggered.AddListener(TriggerScreamer);
        }

        protected override void Enabled()
        {
            _fsm.enabled = false;
            Armature.position = StartPoint;
            CameraOrigs = Utils.SetCameraLookAt(CameraTargetPoint);

            Armature.gameObject.SetActive(true);
            Rigged.gameObject.SetActive(true);
            SoundManager.PlayHeartbeat(true);
        }

        protected override void Disabled()
        {
            animPlayed = false;
            movingStage = 0;
            elapsedFrames = 0;

            Armature.gameObject.SetActive(false);
            Rigged.gameObject.SetActive(false);
            EventsManager.FinishScreamer(ScreamTimeType.PARALYSIS, (int)ScreamParalysisType.HAND);
        }

        protected override void OnFixedUpdate()
        {
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

        void TriggerScreamer(ScreamTimeType type, int variation)
        {
            if (type != ScreamTimeType.PARALYSIS || (ScreamParalysisType)variation != ScreamParalysisType.HAND) return;
            Utils.PrintDebug("Trigger screamer in MovingHand");

            enabled = true;
        }
    }
}
