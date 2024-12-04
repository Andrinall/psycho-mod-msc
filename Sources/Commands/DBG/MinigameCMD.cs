using MSCLoader;
using UnityEngine;

using Psycho.Features;
using Psycho.Internal;

namespace Psycho.Commands
{
    internal class MinigameCMD : ConsoleCommand
    {
        public override string Name => "mini";
        public override string Help => "";

        Features.Minigame minigame;

        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0])) return;

            if (minigame == null) minigame = GameObject.Find("COTTAGE/minigame(Clone)").GetComponent<Minigame>();

            if (args[0] == "up")
            {
                minigame.PlayerGetsCard();
                return;
            }
            else if (args[0] == "uh")
            {
                Logic.lastDayMinigame--;
                minigame.UpdateHousekeeperCard();
                return;
            }
            else Utils.PrintDebug("Undefined command argument");
        }
    }
}
