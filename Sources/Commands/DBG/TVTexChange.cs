using MSCLoader;
using UnityEngine;

using Psycho.Screamers;


namespace Psycho.Commands
{
    internal class TVTexChange : ConsoleCommand
    {
        public override string Name => "tv";

        public override string Help => "";

        public override void Run(string[] args)
            => GameObject.Find("YARD/Building/Dynamics/HouseElectricity/ElectricAppliances/TV_Programs")
                .GetComponent<TVScreamer>()
                .enabled = true;
    }
}
