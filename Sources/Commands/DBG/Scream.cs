#if DEBUG
using MSCLoader;

using Psycho.Internal;


namespace Psycho.Debug
{
    internal class Scream : ConsoleCommand
    {
        public override string Name => "scream";

        public override string Help => "";


        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0])) return;

            switch (args[0])
            {
                case "sound":
                    ApplyScreamer(args, ScreamTimeType.SOUNDS);
                    break;
                case "fear":
                    ApplyScreamer(args, ScreamTimeType.FEAR);
                    break;
                case "prls":
                    ApplyScreamer(args, ScreamTimeType.PARALYSIS);
                    break;
            }
        }

        void ApplyScreamer(string[] args, ScreamTimeType type)
        {
            if (args.Length > 1 && int.TryParse(args[1], out int res))
                EventsManager.TriggerNightScreamer(type, res);
            else
                EventsManager.TriggerNightScreamer(type, 0);
        }
    }
}
#endif