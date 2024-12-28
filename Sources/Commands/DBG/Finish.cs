#if DEBUG
using MSCLoader;

namespace Psycho.Debug
{
    internal class Finish : ConsoleCommand
    {
        public override string Name => "finish";

        public override string Help => "finish shiz game";


        public override void Run(string[] args)
            => Logic.FinishShizGame();
    }
}
#endif