using System.Linq;

using MSCLoader;
using UnityEngine;

using Psycho.Features;
using Psycho.Internal;


namespace Psycho.Commands
{
    internal class NotebookCMD : ConsoleCommand
    {
        public override string Name => "np";

        public override string Help => "";

        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0])) return;

            if (args[0].Contains("fill"))
            {
                char iter = args[0].ToCharArray().Last();
                switch (iter)
                {
                    case '1': // fill true
                        Fill(true);
                        break;
                    case '2': // fill false
                        Fill(false);
                        break; 
                    case '3': // fill random
                        Fill(default, true);
                        break;
                    default:
                        throw new System.NotImplementedException("Argument doesn't match in command implementation");
                }
                Globals.Notebook?.TryCreateFinalPage();
            }
            else if (args[0] == "clear")
                Globals.Notebook?.ClearPages();
        }

        void Fill(bool state, bool random = false)
        {
            Globals.Notebook?.ClearPages();
            for (int i = 1; i < 14; i++)
            {
                NotebookMain.Pages.Add(new NotebookPage
                {
                    index = i,
                    isTruePage = random ? (Random.Range(0, 2) == 1) : state
                });
            }
            Globals.Notebook?.SortPages();

            Utils.PrintDebug(eConsoleColors.GREEN, $"Pages list filled with (true? {state}) pages");
        }
    }
}
