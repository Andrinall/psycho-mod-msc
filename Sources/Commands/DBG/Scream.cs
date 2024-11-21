#if DEBUG
using MSCLoader;
using UnityEngine;

using Psycho.Screamers;


namespace Psycho.Commands
{
    internal class Scream : ConsoleCommand
    {
        ScreamsInitiator initiator;

        public override string Name => "scream";

        public override string Help => "";


        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0])) return;
            if (!initiator)
                initiator = GameObject.Find("YARD/Building/BEDROOM1/LOD_bedroom1/Sleep/SleepTrigger").GetComponent<ScreamsInitiator>();

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
                initiator.ApplyScreamer(type, res);
            else
                initiator.ApplyScreamer(type);
        }
    }
}
#endif