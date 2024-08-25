using UnityEngine;

namespace Adrenaline
{
    internal class VenttiGameHandler : MonoBehaviour
    {
        private PlayMakerFSM VenttiGame;

        private void OnEnable()
        {
            Utils.PrintDebug("VenttiGameHandler enabled");
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref VenttiGame, "CABIN/Cabin/Ventti/Table/GameManager", "Use");

            if (VenttiGame?.ActiveStateName == "Lose")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.VENTTI_WIN);
                Utils.PrintDebug("Value increased by losing in ventti game");
            }
        }
    }
}
