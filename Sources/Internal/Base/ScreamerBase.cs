
namespace Psycho.Internal
{
    internal abstract class ScreamerBase : CatchedComponent
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
