using UnityEngine;

using Psycho.Internal;
using Psycho.Extensions;


namespace Psycho.Screamers
{
    public sealed class MummolaCrawl : MonoBehaviour
    {
        Transform Char;
        Transform Head;

        Animation Anim;
        AnimationClip PigWalk;

        PlayMakerFSM Fsm;

        bool AnimPlayed = false;

        int HeadInterpolationFrames = 240;
        int ElapsedFrames = 0;
        
        float TargetDistance = 0.1f;
        float MaxSpeed = 0.9f;

        Vector3 StartPoint = new Vector3(-8.40377522f, 2.46443129f, 9.5422678f);
        Vector3 TargetPoint = new Vector3(-11.2399998f, 2.5150001f, 12.6759996f);
        Vector3 HeadEndRotation = new Vector3(80f, 270f, 0f);

        Vector3[] cameraOrigs;


        void Awake()
        {
            Fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Char = transform.Find("Char");
            Head = Char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");
            enabled = false;
        }

        void OnEnable()
        {
            Fsm.enabled = false;
            transform.position = StartPoint;
            transform.eulerAngles = new Vector3(347.788879f, 331.232269f, 180f);
            
            cameraOrigs = Utils.SetCameraLookAt(TargetPoint);
            ResetHeadRotation();
            Char.gameObject.SetActive(true);
            SoundManager.PlayHeartbeat(true);
            WorldManager.ShowCrows(false);
        }
        
        void OnDisable()
        {
            ElapsedFrames = 0;
            AnimPlayed = false;
            
            Char.gameObject.SetActive(false);            
            ResetHeadRotation();
            SoundManager.PlayHeartbeat(false);
            WorldManager.ShowCrows(true);
        }
        
        void FixedUpdate()
        {
            if (AnimPlayed) return;
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
                    enabled = false;
                    Utils.ResetCameraLook(cameraOrigs);
                    Fsm.CallGlobalTransition("SCREAMSTOP");
                });
                return;
            }

            float interpolationRatio = (float)ElapsedFrames / HeadInterpolationFrames;
            Head.localEulerAngles = Vector3.Lerp(Head.localEulerAngles, HeadEndRotation, interpolationRatio);
            ElapsedFrames++;
        }
    }
}
