
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Screamers
{
    internal sealed class MovingUncleHead : ScreamerBase
    {
        public override ScreamTimeType ScreamerTime => ScreamTimeType.PARALYSIS;
        public override int ScreamerVariant => (int)ScreamParalysisType.KESSELI;

        const int NEEDED_FRAMES = 100;
        const float MAX_SPEED = 0.26f;
        const float TARGET_DISTANCE = 0.07f;

        readonly Vector3 TargetPoint = new Vector3(-12.035453796386719f, 0.24380475282669068f, 13.690605163574219f);


        GameObject headObj;
        GameObject charObj;
        PlayMakerFSM fsm;

        Vector3 startPoint;
        Vector3[] cameraOrigs;

        int elapsedFrames = 0;
        bool animPlayed = false;


        public override void InitScreamer()
        {
            fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            charObj = transform.Find("Char").gameObject;
            headObj = charObj.transform.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot/head").gameObject;
            startPoint = headObj.transform.position;

            var _source = headObj.AddComponent<AudioSource>();
            _source.clip = ResourcesStorage.UncleScream_clip;
            _source.playOnAwake = true;
            _source.loop = false;
            _source.priority = 128;
            _source.volume = 1f;
            _source.pitch = 1f;
            _source.panStereo = 0;
            _source.spatialBlend = 1f;
            _source.reverbZoneMix = 1f;
            _source.dopplerLevel = 1f;
            _source.minDistance = 1.5f;
            _source.spread = 0;
            _source.maxDistance = 12f;
        }

        public override void TriggerScreamer()
        {
            fsm.enabled = false;
            headObj.transform.position = startPoint;
            cameraOrigs = Utils.SetCameraLookAt(headObj.transform.position);
            charObj.SetActive(true);
            headObj.SetActive(false);
        }

        public override void StopScreamer()
        {
            fsm.enabled = true;
            animPlayed = false;
            elapsedFrames = 0;

            SoundManager.PlayHeartbeat(false);
            Utils.ResetCameraLook(cameraOrigs);
            fsm.CallGlobalTransition("SCREAMSTOP");
            charObj.SetActive(false);
        }


        protected override void OnFixedUpdate()
        {
            if (!ScreamerEnabled) return;
            if (!Utils.WaitFrames(ref elapsedFrames, NEEDED_FRAMES))
            {
                if (elapsedFrames == NEEDED_FRAMES - 1)
                {
                    headObj.SetActive(true);
                    SoundManager.PlayHeartbeat(true);
                }
                return;
            }

            if (animPlayed) return;
            if (!headObj.transform.MoveTowards(TargetPoint, TARGET_DISTANCE, MAX_SPEED)) return;

            animPlayed = true;
            ShizAnimPlayer.PlayOriginalAnimation("sleep_knockout", 4f, default, base.Stop);
        }
    }
}