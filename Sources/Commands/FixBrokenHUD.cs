using MSCLoader;

using Psycho.Internal;


namespace Psycho.Commands
{
    class FixBrokenHUD : ConsoleCommand
    {
        public override string Name => "fhud";
        public override string Help => "Fixes a broken HUD line";


        public override void Run(string[] args)
            => FixedHUD.Structurize();
    }
}
