using System.Linq;

using MSCLoader;
using UnityEngine;

namespace Psycho
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

    // debug commands
#if DEBUG
    public sealed class SuicidalCmd : ConsoleCommand
    {
        public override string Name => "suicidal";

        public override string Help => "";

        public override void Run(string[] args) =>
            WorldManager.CreateSuicidal(GameObject.Find("PLAYER").transform.position, "SuicidalCustom");
    }

    public sealed class ChangeWorld : ConsoleCommand
    {
        public override string Name => "cworld";
        public override string Help => "change a player world";

        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0])) return;
            if (!int.TryParse(args[0], out int world)) return;
            if (world < 0 || world > 1) return;
            Logic.ChangeWorld((eWorldType)world);
        }
    }

    public sealed class Kill : ConsoleCommand
    {
        public override string Name => "kill";

        public override string Help => "";

        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0])) return;

            if (args[0] == "heart")
                Logic.Points = 200f;
            else if (args[0] == "train")
                Logic.KillUsingTrain();
        }
    }

    public sealed class TeleportToPills : ConsoleCommand
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

            player.transform.position = Globals.pills_positions.ElementAtOrDefault(index);
        }

        void PrintHelp()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"============ Pills TP ============\nUsage: atp index\nPosible indexes: from 0 to {Globals.pills_positions.Count - 1}\n===================================");
        }
    }
#endif
}
