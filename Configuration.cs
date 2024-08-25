using System.Reflection;

namespace Adrenaline
{
    public class Configuration
    {
        public float MIN_LOSS_RATE = 0.6f;
        public float MAX_LOSS_RATE = 1.4f;

        public float DEFAULT_DECREASE = 0.18f;
        public float SPRINT_INCREASE = 0.3f;
        public float HIGHSPEED_INCREASE = 0.35f;
        public float FIGHT_INCREASE = 0.47f;
        public float WINDOW_BREAK_INCREASE = 6f;
        public float HOUSE_BURNING = 0.56f;
        public float TEIMO_PISS = 2.5f;
        public float GUARD_CATCH = 2.5f;
        public float VENTTI_WIN = 11f;
        public float JANNI_PETTERI_HIT = 45.5f;
        public float TEIMO_SWEAR = 3f;
        public float PISS_ON_DEVICES = 4f;
        public float SPARKS_WIRING = 3f;
        public float SPILL_SHIT = 1.1f;
        public float RALLY_PLAYER = 1f;


        public T GetFieldValue<T>(string name)
        {
            return (T)GetType().GetField(name).GetValue(this);
        }
    }
}
