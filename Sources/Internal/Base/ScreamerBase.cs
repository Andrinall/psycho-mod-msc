
namespace Psycho.Internal
{
    enum ScreamTimeType : int
    {
        SOUNDS = 0,
        FEAR = 1,
        PARALYSIS = 2
    }

    enum ScreamSoundType : int
    {
        BEDROOM = 0,
        CRYFEMALE = 1,
        CRYKID = 2,
        KNOCK = 3,
        FOOTSTEPS = 4,
        GLASS1 = 5,
        GLASS2 = 6,
        WATER = 7
    }

    enum ScreamFearType : int
    {
        GRANNY = 0,
        SUICIDAL = 1,
        WATERKITCHEN = 2,
        WATERBATHROOM = 3,
        TV = 4,
        PHONE = 5
    }

    enum ScreamParalysisType : int
    {
        GRANNY = 0,
        HAND = 1,
        KESSELI = 2
    }

    abstract class ScreamerBase : CatchedComponent
    {
        public virtual ScreamTimeType ScreamerTime => 0;
        public virtual int ScreamerVariant => 0;

        protected bool ScreamerEnabled { get; private set; } = false;


        protected override void Awaked()
        {
            InitScreamer();
            EventsManager.OnScreamerTriggered.AddListener(Triggered);
        }

        void Triggered(ScreamTimeType type, int variation)
        {
            if (type != ScreamerTime || variation != ScreamerVariant) return;

            TriggerScreamer();
            ScreamerEnabled = true;
        }

        protected void Stop()
        {
            ScreamerEnabled = false;
            StopScreamer();
            EventsManager.FinishScreamer(ScreamerTime, ScreamerVariant);
        }

        public abstract void InitScreamer();
        public abstract void TriggerScreamer();
        public virtual void StopScreamer() { }
    }
}
