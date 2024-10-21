using System;
using System.Collections;

using UnityEngine;

namespace Psycho
{
    [RequireComponent(typeof(Animation))]
    public sealed class ShizAnimPlayer : MonoBehaviour
    {
        GameObject _eyes = null;
        Animation m_oAnimation = null;


        void OnEnable()
        {
            _eyes = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/SleepEyes");
            m_oAnimation = _eyes.GetComponent<Animation>();
        }

        public void PlayAnimation(
            string animation,
            bool disable = false,
            float waitSeconds = 0f,
            PlayMode mode = PlayMode.StopAll,
            Action finish_callback = null
        ) => StartCoroutine(PlayAnimWaited(animation, disable, waitSeconds, mode, finish_callback));

        IEnumerator PlayAnimWaited(
            string animation,
            bool disable,
            float waitSeconds,
            PlayMode mode,
            Action finish_callback
        ) {
            if (m_oAnimation.isPlaying)
                yield return new WaitForSeconds(1f);

            if (!_eyes.activeSelf)
                _eyes.SetActive(true);

            m_oAnimation.Play(animation, mode);
            while (m_oAnimation.isPlaying)
                yield return new WaitForEndOfFrame();

            if (waitSeconds > 0f)
                yield return new WaitForSeconds(waitSeconds);

            if (disable && _eyes.activeSelf)
                _eyes.SetActive(false);

            finish_callback?.Invoke();
        }
    }
}
