#if DEBUG
using MSCLoader;
using UnityEngine;

using Psycho.Features;
using Object = UnityEngine.Object;


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

                    Transform player = GameObject.Find("PLAYER").transform;
                    GameObject item;

                    switch (args[1])
                    {
                        case "candle":
                            item = (GameObject)Object.Instantiate(Globals.Candle_prefab, player.position, Quaternion.Euler(Vector3.zero));
                            item.GetComponent<MeshRenderer>().materials[0].color = new Color(161 / 255, 37 / 255, 44 / 255);
                            item.transform.localScale = new Vector3(.25f, .2f, .36f);
                            break;
                        case "flower":
                            item = (GameObject)Object.Instantiate(Globals.FernFlower_prefab, player.position, Quaternion.Euler(Vector3.zero));
                            item.transform.localScale = new Vector3(.78f, .78f, .78f);
                            break;
                        case "mushroom":
                            item = (GameObject)Object.Instantiate(Globals.Mushroom_prefab, player.position, Quaternion.Euler(Vector3.zero));
                            break;
                        case "egg":
                            item = (GameObject)Object.Instantiate(Globals.BlackEgg_prefab, player.position, Quaternion.Euler(Vector3.zero));
                            break;
                        case "nut":
                            item = (GameObject)Object.Instantiate(Globals.Nut_prefab, player.position, Quaternion.Euler(Vector3.zero));
                            break;
                        
                        default:
                            return;
                    }

                    if (item != null) item.MakePickable();

                    ModConsole.Print($"{args[1]} spawned on player pos");
                    break;
            }
        }
    }
}
#endif