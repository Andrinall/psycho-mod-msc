using System;

using MSCLoader;
using UnityEngine;

using Psycho.Objects;


namespace Psycho.Commands
{
    internal class Penta : ConsoleCommand
    {
        public override string Name => "penta";

        public override string Help => "";

        Pentagram penta;

        public override void Run(string[] args)
        {
            if (args.Length == 0) return;
            if (string.IsNullOrEmpty(args[0])) return;

            if (!penta)
                penta = GameObject.Find("Penta(Clone)").GetComponent<Pentagram>();

            switch (args[0])
            {
                case "light":
                    penta.SetCandlesFireActive(!penta.GetCandlesFireActive());
                    break;

                case "visible":
                    penta.gameObject.SetActive(!penta.gameObject.activeSelf);
                    break;

                case "spawn":
                    throw new NotImplementedException();
            }
        }
    }
}
