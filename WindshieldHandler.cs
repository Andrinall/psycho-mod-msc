using HutongGames.PlayMaker;
using UnityEngine;

namespace Adrenaline
{
    internal class WindshieldHandler : MonoBehaviour
    {
        private FsmBool Damaged;
        private Drivetrain drivetrain;

        private void OnEnable()
        {
            Damaged = base.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmBool("Damaged");
            drivetrain = base.transform.parent.parent.gameObject.GetComponent<Drivetrain>();
        }

        private void FixedUpdate()
        {
            if (Damaged.Value && drivetrain.differentialSpeed >= 45f)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.BROKEN_WINDSHIELD_INCREASE);
        }
    }
}
