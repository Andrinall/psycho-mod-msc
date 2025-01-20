
using MSCLoader;

using UnityEngine;
using UnityEngine.Events;


namespace Psycho.Internal
{
    class ScreamerEvent : UnityEvent<ScreamTimeType, int> { } // crutch

    static class EventsManager
    {
        public static UnityEvent OnLanguageChanged { get; private set; } = new UnityEvent();

        public static UnityEvent<ScreamTimeType, int> OnScreamerTriggered { get; private set; } = new ScreamerEvent();
        public static UnityEvent OnScreamerFinished { get; private set; } = new UnityEvent();


        public static void ChangeLanguage(int lang)
        {
            Globals.CurrentLang = lang;

            if (Application.loadedLevelName != "GAME") return;
            
            TextMesh _postcardText = GameObject.Find("Postcard(Clone)")?.transform?.Find("Text")?.GetComponent<TextMesh>();
            if (_postcardText != null)
                _postcardText.text = Locales.POSTCARD_TEXT[Globals.CurrentLang];

            OnLanguageChanged.Invoke();
        }

        public static void TriggerNightScreamer(ScreamTimeType type, int variation)
        {
            if (!Psycho.IsLoaded) return;

            try
            {
                DoorsManager.CloseAllDoorsInHouse();

                if (type == ScreamTimeType.FEAR && variation == (int)ScreamFearType.TV)
                    Electricity.SetSwitchState(true);
                else
                    Electricity.SetSwitchState(false);

                Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer triggered [{type} : {_getScreamerVariantName(type, variation)}]");

                OnScreamerTriggered.Invoke(type, variation);
            }
            catch (System.Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to trigger night screamer.");
                ModConsole.Error($"{ex.GetFullMessage()}\n{ex.StackTrace}");
            }
        }

        public static void FinishScreamer(ScreamTimeType type, int variation)
        {
            if (!Psycho.IsLoaded) return;
            
            Electricity.SetSwitchState(true);

            OnScreamerFinished.Invoke();
            Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer finished! [{type} : {_getScreamerVariantName(type, variation)}]");
        }

        
        public static void UnSubscribeAll()
        {
            OnLanguageChanged.RemoveAllListeners();
            OnScreamerTriggered.RemoveAllListeners();
            OnScreamerFinished.RemoveAllListeners();
        }

        
        static string _getScreamerVariantName(ScreamTimeType type, int variation)
        {
            switch (type)
            {
                case ScreamTimeType.SOUNDS:
                    return ((ScreamSoundType)variation).ToString();
                case ScreamTimeType.FEAR:
                    return ((ScreamFearType)variation).ToString();
                case ScreamTimeType.PARALYSIS:
                    return ((ScreamParalysisType)variation).ToString();
                default:
                    return string.Empty;
            }
        }
    }
}
