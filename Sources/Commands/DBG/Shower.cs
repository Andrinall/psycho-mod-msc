#if DEBUG
using MSCLoader;

using Psycho.Screamers;
using UnityEngine;


namespace Psycho.Commands
{
    internal class Shower : ConsoleCommand
    {
        public override string Name => "shower";

        public override string Help => "";

        public override void Run(string[] args)
        {
            GameObject.Find("YARD/Building/BATHROOM/Shower")
                .GetComponent<BathroomShower>()
                .enabled = true;
        }
    }
}
#endif