#if DEBUG
using MSCLoader;
using UnityEngine;

using Psycho.Features;


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

                case "event":
                    if (args.Length == 1) return;
                    if (string.IsNullOrEmpty(args[1])) return;
                    penta.gameObject.GetComponent<PentagramEvents>().Activate(args[1]);
                    break;

                case "item":
                    if (args.Length == 1) return;
                    if (string.IsNullOrEmpty(args[1])) return;
                    SpawnItem(args[1]);
                    break;

                case "items":
                    foreach (string itemname in penta.recipe)
                        SpawnItem(itemname);
                    break;
            }
        }

        void SpawnItem(string item)
        {
            Transform player = GameObject.Find("PLAYER").transform;
            Vector3 pos = player.position;

            switch (item)
            {
                case "churchcandle":
                    Globals.AddPentaItem(Globals.Candle_prefab, pos, Vector3.zero);
                    break;
                case "fernflower":
                    Globals.AddPentaItem(Globals.FernFlower_prefab, pos, Vector3.zero);
                    break;
                case "mushroom":
                    Globals.AddPentaItem(Globals.Mushroom_prefab, pos, Vector3.zero);
                    break;
                case "blackegg":
                    Globals.AddPentaItem(Globals.BlackEgg_prefab, pos, Vector3.zero);
                    break;
                case "walnut": 
                    Globals.AddPentaItem(Globals.Walnut_prefab, pos, Vector3.zero);
                    break;

                default:
                    return;
            }
            ModConsole.Print($"{item} spawned on player pos");
        }
    }
}
#endif