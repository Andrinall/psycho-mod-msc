#if DEBUG
using MSCLoader;

using Psycho.Screamers;
using UnityEngine;


namespace Psycho.Commands
{
	internal class KitchenTap : ConsoleCommand
	{
		public override string Name => "Tap";

		public override string Help => "";

		public override void Run(string[] args)
		{
            GameObject.Find("YARD/Building/KITCHEN/KitchenWaterTap")
                    .GetComponent<KitchenShower>()
                    .enabled = true;
        }
	}
}
#endif