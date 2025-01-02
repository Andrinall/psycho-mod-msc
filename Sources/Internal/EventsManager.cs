using MSCLoader;

using UnityEngine;
using UnityEngine.Events;


namespace Psycho.Internal
{
    public sealed class ScreamerEvent : UnityEvent<ScreamTimeType, int> { } // crutch

    public static class EventsManager
    {
        public static UnityEvent OnLanguageChanged { get; private set; } = new UnityEvent();

        public static UnityEvent<ScreamTimeType, int> OnScreamerTriggered { get; private set; } = new ScreamerEvent();
        public static UnityEvent OnScreamerFinished { get; private set; } = new UnityEvent();


        public static void ChangeLanguage(int lang)
        {
            Globals.CurrentLang = lang;

            if (Application.loadedLevelName != "GAME") return;
            
            TextMesh postcardText = GameObject.Find("Postcard(Clone)")?.transform?.Find("Text")?.GetComponent<TextMesh>();
            if (postcardText != null)
                postcardText.text = Locales.POSTCARD_TEXT[Globals.CurrentLang];

            OnLanguageChanged.Invoke();
        }

        public static void TriggerNightScreamer(ScreamTimeType type, int variation)
        {
            if (!Psycho.IsLoaded) return;

            try
            {
                if (type == ScreamTimeType.FEAR && variation == (int)ScreamFearType.TV)
                    WorldManager.SetElecMeterState(true);
                else
                    WorldManager.SetElecMeterState(false);

                WorldManager.ShowCrows(false);
                Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer triggered [{type} : {GetScreamerVariantName(type, variation)}]");

                OnScreamerTriggered.Invoke(type, variation);
            }
            catch (System.Exception ex)
            {
                ModConsole.Error($"{ex.GetFullMessage()}\n{ex.StackTrace}");
            }
        }

        public static void FinishScreamer(ScreamTimeType type, int variation)
        {
            if (!Psycho.IsLoaded) return;
            
            WorldManager.SetElecMeterState(true);
            WorldManager.ShowCrows(true);

            OnScreamerFinished.Invoke();
            Utils.PrintDebug(eConsoleColors.GREEN, $"Screamer finished! [{type} : {GetScreamerVariantName(type, variation)}]");
        }

        
        public static void UnSubscribeAll()
        {
            OnLanguageChanged.RemoveAllListeners();
            OnScreamerTriggered.RemoveAllListeners();
            OnScreamerFinished.RemoveAllListeners();
        }

        
        static string GetScreamerVariantName(ScreamTimeType type, int variation)
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
