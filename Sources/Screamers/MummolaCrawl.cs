using HutongGames.PlayMaker;
using MSCLoader;
using Psycho.Internal;
using UnityEngine;


namespace Psycho.Screamers
{
    public sealed class MummolaCrawl : MonoBehaviour
    {
        Transform Char;
        Transform Head;

        Animation Anim;
        AnimationClip pigwalk;

        PlayMakerFSM _fsm;

        bool isDisabledChar = false;
        bool animPlayed = false;

        int HeadInterpolationFrames = 240;
        int elapsedFrames = 0;
        
        float TargetDistance = 0.1f;
        float MaxSpeed = 1;

        Vector3 StartPoint = new Vector3(-8.40377522f, 2.46443129f, 9.5422678f);
        Vector3 TargetPoint = new Vector3(-11.2399998f, 2.5150001f, 12.6759996f);
        Vector3 HeadEndRotation = new Vector3(80f, 270f, 0f);

        Transform player;
        Transform fpsCamera;
        Vector3 playerOrigPos;
        Vector3 playerOrigEuler;

        Vector3 cameraOrigPos;
        Vector3 cameraOrigEuler;

        void Awake()
        {
            _fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            
            player = GameObject.Find("PLAYER").transform;
            fpsCamera = Utils.GetGlobalVariable<FsmGameObject>("POV").Value.transform.parent;
            
            enabled = false;
        }

        void OnEnable()
        {
            transform.position = StartPoint;
            transform.eulerAngles = new Vector3(347.788879f, 331.232269f, 180f);

            Char = transform.Find("Char");
            Head = Char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");
            
            SetCameraPos();
            ResetHeadRotation();
            Char.gameObject.SetActive(true);
        }
        
        void OnDisable()
        {
            elapsedFrames = 0;
            animPlayed = false;
            
            Char.gameObject.SetActive(false);            
            ResetHeadRotation();
        }
        
        void FixedUpdate()
        {
            if (TargetPoint == null) return;
            if (animPlayed) return;

            if (CheckDistance())
            { 
                RotateHeadPivot();
                return;
            }

            MoveTowards();
        }

        bool CheckDistance() => (transform.position - TargetPoint).magnitude < TargetDistance;

        void MoveTowards() => transform.position = Vector3.MoveTowards(transform.position, TargetPoint, MaxSpeed * Time.deltaTime);

        void ResetHeadRotation() => Head.localEulerAngles = new Vector3(270f, 90f, 0f);

        void RotateHeadPivot()
        {
            if (elapsedFrames == HeadInterpolationFrames)
            {
                PlaySleepAnim();
                return;
            }

            float interpolationRatio = (float)elapsedFrames / HeadInterpolationFrames;
            Head.localEulerAngles = Vector3.Lerp(Head.localEulerAngles, HeadEndRotation, interpolationRatio);
            elapsedFrames++;
        }

        void PlaySleepAnim()
        {
            if (animPlayed) return;
            animPlayed = true;

            Logic.shizAnimPlayer.PlayAnimation("sleep_knockout", default, 4f, default, () =>
            {                
                this.enabled = false;
                ResetCameraPos();

                _fsm.enabled = true;
                _fsm.Fsm.Event(_fsm.GetGlobalTransition("SCREAMSTOP").FsmEvent); // work
            });
        }


        void SetCameraPos()
        {
            Utils.PrintDebug("SetCameraPos() called in MummolaCrawl");

            cameraOrigPos = fpsCamera.localPosition;
            cameraOrigEuler = fpsCamera.localEulerAngles;

            fpsCamera.LookAt(TargetPoint);
        }

        void ResetCameraPos()
        {
            fpsCamera.localPosition = cameraOrigPos;
            fpsCamera.localEulerAngles = cameraOrigEuler;
        }
    }
}
