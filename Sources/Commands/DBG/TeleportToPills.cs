#if DEBUG
using MSCLoader;
using UnityEngine;


namespace Psycho.Debug
{
    internal class TeleportToPills : ConsoleCommand
    {
        public override string Name => "ptp";
        public override string Help => "Teleport a player to pills;";


        public override void Run(string[] args)
        {
            if (Application.loadedLevelName != "GAME") return;
            if (!Logic.InHorror) return;
            if (Globals.Pills == null) return;

            GameObject.Find("PLAYER").transform.position = Globals.Pills.self.transform.position;
        }
    }
}
#endif