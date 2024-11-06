using MSCLoader;
using UnityEngine;

using Psycho.Screamers;


namespace Psycho.Commands
{
    internal class Phone : ConsoleCommand
    {
        public override string Name => "phone";

        public override string Help => "";

        public override void Run(string[] args)
            => GameObject.Find("YARD/Building/LIVINGROOM/Telephone/Logic").GetComponent<PhoneRing>().enabled = true;
    }
}
