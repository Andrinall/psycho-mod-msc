using MSCLoader;

using Psycho.Internal;


namespace Psycho.Commands
{
    public sealed class FixBrokenHUD : ConsoleCommand
    {
        public override string Name => "fhud";
        public override string Help => "Fixes a broken HUD line";

        public override void Run(string[] args)
        {
            if (Logic._hud == null)
            {
                Utils.PrintDebug(eConsoleColors.RED, "HUD not initialized");
                return;
            }

            Logic._hud.Structurize();
        }
    }
}
