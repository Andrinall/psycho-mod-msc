#if DEBUG
using MSCLoader;
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Debug
{
    internal class TeleportToPills : ConsoleCommand
    {
        public override string Name => "ptp";
        public override string Help => "Teleport a player to pills; [ ptp index ]";


        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                PrintHelp();
                return;
            }

            if (!int.TryParse(args[0], out var index))
            {
                PrintHelp();
                return;
            }

            if (index < 0 || index > Globals.pills_positions.Count - 1)
            {
                PrintHelp();
                return;
            }

            var player = GameObject.Find("PLAYER");
            if (player == null)
            {
                Utils.PrintDebug(eConsoleColors.RED, "The game scene is not initialized.\nThe command is not used from the menu!");
                return;
            }

            player.transform.position = Globals.pills_positions[index];
        }

        void PrintHelp()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"============ Pills TP ============\nUsage: atp index\nPosible indexes: from 0 to {Globals.pills_positions.Count - 1}\n===================================");
        }
    }
}
#endif