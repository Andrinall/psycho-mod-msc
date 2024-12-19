using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MovingUncleHead : CatchedComponent
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


        public override void Awaked()
        {
            enabled = false;

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
            ScreamSound = source;

            EventsManager.OnScreamerTriggered.AddListener(TriggerScreamer);
        }

        public override void Enabled()
        {
            _fsm.enabled = false;
            Head.position = StartPoint;
            CameraOrigs = Utils.SetCameraLookAt(Head.position);
            Char.gameObject.SetActive(true);
            Head.gameObject.SetActive(false);
        }

        public override void Disabled()
        {
            if (Char == null) return;

            animPlayed = false;
            Char.gameObject.SetActive(false);
            elapsedFrames = 0;
            EventsManager.FinishScreamer(ScreamTimeType.PARALYSIS, (int)ScreamParalysisType.KESSELI);
        }

        public override void OnFixedUpdate()
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

        void TriggerScreamer(ScreamTimeType type, int variation)
        {
            if (type != ScreamTimeType.PARALYSIS || (ScreamParalysisType)variation != ScreamParalysisType.KESSELI) return;

            enabled = true;
        }
    }
}