using MSCLoader;
using System;
using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class FixBrokenHUD : ConsoleCommand
    {
        public override string Name => "ahud";
        public override string Help => "Fixes a broken HUD line";

        public override void Run(string[] args)
        {
            if (AdrenalineLogic._hud == null)
            {
                Utils.PrintDebug(eConsoleColors.RED, "HUD not initialized");
                return;
            }

            AdrenalineLogic._hud.Structurize();
        }
    }

    // debug commands
#if DEBUG
    internal class TeleportToPills : ConsoleCommand
    {
        public override string Name => "atp";
        public override string Help => "Teleport a player to pills; [ atp index ]";

        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                PrintHelp();
                return;
            }

            if (!Int32.TryParse(args[0], out var index))
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

            player.transform.position = Globals.pills_positions.ElementAtOrDefault(index);
        }

        private void PrintHelp()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"========== Adrenaline TP ==========\nUsage: atp index\nPosible indexes: from 0 to {Globals.pills_positions.Count - 1}\n===================================");
        }
    }
#endif
}
