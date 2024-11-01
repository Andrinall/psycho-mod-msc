using MSCLoader;
using Psycho.Internal;

namespace Psycho.Commands
{
    internal class Shower : ConsoleCommand
    {
        public override string Name => "shower";

        public override string Help => "";

        static bool active = false;
        public override void Run(string[] args)
        {
            active = !active;
            WorldManager.SwitchBathroomShower(active);
        }
    }
}
