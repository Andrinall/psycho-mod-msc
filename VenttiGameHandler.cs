using UnityEngine;

namespace Adrenaline
{
    internal class VenttiGameHandler : MonoBehaviour
    {
        private GameObject Ventti;
        private PlayMakerFSM VenttiGame;

        private void OnEnable()
        {
            Utils.PrintDebug("VenttiGameHandler enabled");
            Ventti = base.transform.Find("Ventti").gameObject;
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref VenttiGame, ref Ventti, "Table/GameManager", "Use");

            if (VenttiGame?.ActiveStateName == "Lose")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.VENTTI_WIN);
                Utils.PrintDebug("Value increased by losing in ventti game");
            }
        }
    }
}
