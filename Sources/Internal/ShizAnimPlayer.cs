using System;
using System.Collections;

using MSCLoader;
using UnityEngine;


namespace Psycho.Internal
{
    [RequireComponent(typeof(Animation))]
    internal sealed class ShizAnimPlayer : CatchedComponent
    {
        GameObject _eyes = null;
        Animation m_oAnimation = null;


        public override void Awaked()
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
            if (m_oAnimation == null)
            {
                ModConsole.Error("ShizAnimPlayer.m_oAnimation == null");
                yield break;
            }

            if (_eyes == null)
            {
                ModConsole.Error("ShizAnimPlayer._eyes == null");
                yield break;
            }

            m_oAnimation.Stop();
            _eyes.SetActive(true);

            m_oAnimation.Play(animation, mode);
            while (m_oAnimation.isPlaying)
                yield return new WaitForEndOfFrame();

            if (waitSeconds > 0f)
                yield return new WaitForSeconds(waitSeconds);

            m_oAnimation.Stop();
            try
            {
                finish_callback?.Invoke();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Exception in {Utils.GetMethodPath(finish_callback.Method)}");
                ModConsole.Error(ex.GetFullMessage());
            }
        }
    }
}
