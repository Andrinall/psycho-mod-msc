#if DEBUG
using MSCLoader;
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Commands
{
    public sealed class Suicidal : ConsoleCommand
    {
        public override string Name => "suicidal";

        public override string Help => "";

        public override void Run(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                WorldManager.CreateSuicidal(GameObject.Find("PLAYER").transform.position, "SuicidalCustom");
                return;
            }

            if (args[0] == "copy")
            {
                Utils.PrintDebug("SuicidalsCustom copy process");
                Transform _suicidals = GameObject.Find("SuicidalList").transform;

                for (int i = 0; i < _suicidals.childCount; i++)
                {
                    Transform _child = _suicidals.GetChild(i);
                    Utils.PrintDebug($"[{i}]: /[{_child.position.ToString()}]\\ /[{_child.eulerAngles.ToString()}]\\");
                }

                Utils.PrintDebug("SuicidalsCustom copy finished");
                return;
            }

            if (args[0] == "clear")
                Object.Destroy(GameObject.Find("SuicidalList"));
        }
    }
}
#endif