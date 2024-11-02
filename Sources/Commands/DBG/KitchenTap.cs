using MSCLoader;
using Psycho.Internal;

namespace Psycho.Commands
{
	internal class KitchenTap : ConsoleCommand
	{
		public override string Name => "Tap";

		public override string Help => "";

		static bool active = false;
		public override void Run(string[] args)
		{
			active = !active;
			WorldManager.SwitchKitchenShower(active);
		}
	}
}
