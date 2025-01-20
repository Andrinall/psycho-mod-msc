
using System;
using System.Collections;

using MSCLoader;
using UnityEngine;


namespace Psycho.Internal
{
    [RequireComponent(typeof(Animation))]
    class ShizAnimPlayer : CatchedComponent
    {
        GameObject eyesOrig;
        Animation animationOrig;

        GameObject eyes;
        Animation animation;

        static ShizAnimPlayer instance = null;

        protected override void Awaked()
        {
            eyesOrig = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/SleepEyes");
            
            if (eyesOrig == null)
                throw new NullReferenceException("SleepEyes object not exists!");

            eyes = Instantiate(eyesOrig);
            eyes.name = "HorrorEyes";
            eyes.transform.SetParent(eyesOrig.transform.parent, false);
            eyes.transform.localPosition = Vector3.zero;
            eyes.transform.localEulerAngles = Vector3.zero;
            eyes.SetActive(true);


            animation = eyes.GetComponent<Animation>();
            animation.Stop();

            animationOrig = eyesOrig.GetComponent<Animation>();
            animationOrig.Stop();

            instance = this;
        }

        void OnDestroy() => instance = null;

        public static void PlayAnimation(string animation, float waitSeconds = 0f, PlayMode mode = PlayMode.StopAll, Action finish_callback = null)
            => instance?.PlayAnimation_internal(animation, waitSeconds, mode, finish_callback);

        public static void PlayOriginalAnimation(string animation, float waitSeconds = 0f, PlayMode mode = PlayMode.StopAll, Action finish_callback = null)
            => instance?.PlayOrigAnimation_internal(animation, waitSeconds, mode, finish_callback);

        void PlayAnimation_internal(string animation, float waitSeconds, PlayMode mode, Action finish_callback)
            => StartCoroutine(PlayAnimWaited(animation, waitSeconds, mode, finish_callback));

        void PlayOrigAnimation_internal(string animation, float waitSeconds, PlayMode mode, Action finish_callback)
            => StartCoroutine(PlayOrigAnimWaited(animation, waitSeconds, mode, finish_callback));

        IEnumerator PlayAnimWaited(string animationName, float waitSeconds, PlayMode mode, Action finish_callback)
        {
            if (animation == null)
            {
                ModConsole.Warning("ShizAnimPlayer.m_oAnimation == null");
                yield break;
            }

            if (eyes == null)
            {
                ModConsole.Warning("ShizAnimPlayer._eyes == null");
                yield break;
            }

            eyes.SetActive(true);
            animation.Stop();

            animation.Play(animationName, mode);
            if (!animation.isPlaying)
                ModConsole.Warning("ShizAnimPlayer !animation.isPlaying");

            while (animation.isPlaying)
                yield return new WaitForEndOfFrame();

            if (waitSeconds > 0f)
                yield return new WaitForSeconds(waitSeconds);

            animation.Stop(animationName);
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
            if (animationOrig == null)
            {
                ModConsole.Warning("ShizAnimPlayer.m_oAnimation == null");
                yield break;
            }

            if (eyesOrig == null)
            {
                ModConsole.Warning("ShizAnimPlayer._eyes == null");
                yield break;
            }

            animationOrig.Stop();
            eyesOrig.SetActive(true);

            animationOrig.Play(animation, mode);
            while (animationOrig.isPlaying)
                yield return new WaitForEndOfFrame();

            if (waitSeconds > 0f)
                yield return new WaitForSeconds(waitSeconds);

            animationOrig.Stop();
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
