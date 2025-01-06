
using System;
using System.Collections;

using MSCLoader;
using UnityEngine;


namespace Psycho.Internal
{
    [RequireComponent(typeof(Animation))]
    public sealed class ShizAnimPlayer : CatchedComponent
    {
        GameObject _eyesOrig;
        Animation _animationOrig;

        GameObject _eyes;
        Animation _animation;

        static ShizAnimPlayer instance = null;

        protected override void Awaked()
        {
            _eyesOrig = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/SleepEyes");
            
            if (_eyesOrig == null)
                throw new NullReferenceException("SleepEyes object not exists!");

            _eyes = Instantiate(_eyesOrig);
            _eyes.name = "HorrorEyes";
            _eyes.transform.SetParent(_eyesOrig.transform.parent, false);
            _eyes.transform.localPosition = Vector3.zero;
            _eyes.transform.localEulerAngles = Vector3.zero;
            _eyes.SetActive(true);


            _animation = _eyes.GetComponent<Animation>();
            _animation.Stop();

            _animationOrig = _eyesOrig.GetComponent<Animation>();
            _animationOrig.Stop();

            instance = this;
        }

        void OnDestroy() => instance = null;

        public static void PlayAnimation(string animation, float waitSeconds = 0f, PlayMode mode = PlayMode.StopAll, Action finish_callback = null)
        {
            if (instance == null) return;

            instance.PlayAnimation_internal(animation, waitSeconds, mode, finish_callback);
        }

        public static void PlayOriginalAnimation(string animation, float waitSeconds = 0f, PlayMode mode = PlayMode.StopAll, Action finish_callback = null)
        {
            if (instance == null) return;

            instance.PlayOrigAnimation_internal(animation, waitSeconds, mode, finish_callback);
        }

        void PlayAnimation_internal(string animation, float waitSeconds, PlayMode mode, Action finish_callback)
            => StartCoroutine(PlayAnimWaited(animation, waitSeconds, mode, finish_callback));

        void PlayOrigAnimation_internal(string animation, float waitSeconds, PlayMode mode, Action finish_callback)
            => StartCoroutine(PlayOrigAnimWaited(animation, waitSeconds, mode, finish_callback));

        IEnumerator PlayAnimWaited(string animation, float waitSeconds, PlayMode mode, Action finish_callback)
        {
            if (_animation == null)
            {
                ModConsole.Error("ShizAnimPlayer.m_oAnimation == null");
                yield break;
            }

            if (_eyes == null)
            {
                ModConsole.Error("ShizAnimPlayer._eyes == null");
                yield break;
            }

            _animation.Stop();
            _eyes.SetActive(true);

            _animation.Play(animation, mode);
            while (_animation.isPlaying)
                yield return new WaitForEndOfFrame();

            if (waitSeconds > 0f)
                yield return new WaitForSeconds(waitSeconds);

            _animation.Stop();
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


        IEnumerator PlayOrigAnimWaited(string animation, float waitSeconds, PlayMode mode, Action finish_callback)
        {
            if (_animationOrig == null)
            {
                ModConsole.Error("ShizAnimPlayer.m_oAnimation == null");
                yield break;
            }

            if (_eyesOrig == null)
            {
                ModConsole.Error("ShizAnimPlayer._eyes == null");
                yield break;
            }

            _animationOrig.Stop();
            _eyesOrig.SetActive(true);

            _animationOrig.Play(animation, mode);
            while (_animationOrig.isPlaying)
                yield return new WaitForEndOfFrame();

            if (waitSeconds > 0f)
                yield return new WaitForSeconds(waitSeconds);

            _animationOrig.Stop();
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
