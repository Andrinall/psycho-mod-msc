using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class VenttiGameHandler : MonoBehaviour
    {
        private PlayMakerFSM VenttiGame;

        private void OnEnable()
        {
            VenttiGame = base.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == "Use");
            Utils.PrintDebug("VenttiGameHandler enabled");
        }

        private void FixedUpdate()
        {
            if (VenttiGame?.ActiveStateName == "Lose")
            {
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.VENTTI_WIN);
                Utils.PrintDebug("Value increased by losing in ventti game");
            }
        }
    }
}
