#if DEBUG
using MSCLoader;
using Psycho.Internal;

namespace Psycho.Commands
{
    internal class Phantom : ConsoleCommand
    {
        public override string Name => "ph";

        public override string Help => "";

        static bool active = false;

        public override void Run(string[] args)
        {
            active = !active;
            if (active)
                WorldManager.SpawnPhantomBehindPlayer();
            else
                WorldManager.ClonedPhantomTick(0);
        }
    }
}
#endif