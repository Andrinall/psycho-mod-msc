#if DEBUG
using MSCLoader;


namespace Psycho.Debug
{
    internal class Kill : ConsoleCommand
    {
        public override string Name => "kill";

        public override string Help => "";


        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0])) return;

            if (args[0] == "heart")
                Logic.SetValue(100f);
            else if (args[0] == "train")
                Logic.KillUsingTrain();
        }
    }
}
#endif