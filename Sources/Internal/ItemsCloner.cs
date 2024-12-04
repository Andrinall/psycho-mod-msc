using System;

using MSCLoader;
using UnityEngine;


namespace Psycho.Internal
{
    internal static class ItemsCloner
    {
        internal static GameObject CloneItem(string path)
        {
            try
            {
                GameObject orig = GameObject.Find(path);
                GameObject cloned = GameObject.Instantiate(orig);

                cloned.GetComponent<PlayMakerFSM>().Fsm.InitData();

                if (cloned.transform?.parent?.name == "ITEMS")
                    return cloned;

                cloned.transform.SetParent(GameObject.Find("ITEMS").transform, worldPositionStays: false);
                return cloned;
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Failed to clone item with path {{{path}}}");
                ModConsole.Error(ex.GetFullMessage());
                return null;
            }
        }
    }
}
