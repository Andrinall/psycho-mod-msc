using MSCLoader;
using Psycho.Objects;
using UnityEngine;

namespace Psycho.Commands
{
    internal class Ferns : ConsoleCommand
    {
        public override string Name => "ferns";

        public override string Help => "";

        public override void Run(string[] args)
        {
            GameObject.Find("Ferns(Clone)")
                .GetComponent<FernFlowerSpawner>()
                .SpawnRandomFlower(true);
        }
    }
}
