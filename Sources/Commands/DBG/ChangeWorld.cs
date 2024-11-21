#if DEBUG
using MSCLoader;


namespace Psycho.Commands
{
    internal class ChangeWorld : ConsoleCommand
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
}
#endif