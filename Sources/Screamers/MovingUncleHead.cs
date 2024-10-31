using UnityEngine;

using Psycho.Internal;
using Psycho.Extensions;


namespace Psycho.Screamers
{
    public sealed class MovingUncleHead : MonoBehaviour
    {
        Transform Head;
        Transform Char;
        PlayMakerFSM _fsm;

        Vector3 StartPoint;
        Vector3 TargetPoint = new Vector3(-12.035453796386719f, 0.24380475282669068f, 13.690605163574219f);

        Vector3[] CameraOrigs;

        float TargetDistance = 0.1f;
        float MaxSpeed = 0.52f;

        bool animPlayed = false;

        void Awake()
        {
            _fsm = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<PlayMakerFSM>();
            Char = transform.Find("Char");
            Head = Char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot/head");
            StartPoint = Head.position;
            enabled = false;
        }

        void OnEnable()
        {
            Head.position = StartPoint;
            CameraOrigs = Utils.SetCameraLookAt(Head.position);
            Char.gameObject.SetActive(true);
        }

        void OnDisable()
        {
            animPlayed = false;
            Char.gameObject.SetActive(false);
        }

        void FixedUpdate()
        {
            if (animPlayed) return;
            if (!Head.MoveTowards(TargetPoint, TargetDistance, MaxSpeed)) return;

            Utils.PlayScreamSleepAnim(ref animPlayed, () =>
            {
                this.enabled = false;
                Utils.ResetCameraLook(CameraOrigs);
                _fsm.CallGlobalTransition("SCREAMSTOP");
            });
        }
    }
}