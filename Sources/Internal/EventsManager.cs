using UnityEngine.Events;


namespace Psycho.Internal
{
    public sealed class ScreamerEvent : UnityEvent<ScreamTimeType, int> { } // crutch

    public class EventsManager
    {
        public static UnityEvent OnLanguageChanged = new UnityEvent();

        public static UnityEvent<ScreamTimeType, int> OnScreamerTriggered = new ScreamerEvent();
        public static UnityEvent OnScreamerFinished = new UnityEvent();


        public static void ChangeLanguage(int lang)
        {
            Globals.CurrentLang = lang;
            Psycho.lang.Instance.Name = lang == 0 ? "Language select" : "Выбор языка";

            OnLanguageChanged.Invoke();
        }

        public static void TriggerNightScreamer(ScreamTimeType type, int variation)
        {
            if (!Psycho.IsLoaded) return;

            Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer triggered [{type} : {GetScreamerVariant(type, variation)}]");
            OnScreamerTriggered.Invoke(type, variation);
        }

        public static void FinishScreamer(ScreamTimeType type, int variation)
        {
            if (!Psycho.IsLoaded) return;

            Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer finished! [{type} : {GetScreamerVariant(type, variation)}]");
            OnScreamerFinished.Invoke();
        }

        

        
        static string GetScreamerVariant(ScreamTimeType type, int variation)
        {
            return (type == ScreamTimeType.SOUNDS ? ((ScreamSoundType)variation).ToString() : (type == ScreamTimeType.FEAR ? ((ScreamFearType)variation).ToString() : ((ScreamParalysisType)variation).ToString()));
        }
    }
}
