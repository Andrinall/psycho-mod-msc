
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class SuitcaseHandler : CatchedComponent
    {
        bool suitCaseSpawned = false;
        bool suitCaseGrabbed = false;

        Transform suitcase = null;


        protected override void OnFixedUpdate()
        {
            if (suitcase == null)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform _child = transform.GetChild(i);
                    if (_child.childCount == 0) continue;

                    suitcase = _child.Find("suitcase(itemx)");
                    suitCaseSpawned = true;
                }
                return;
            }

            if (!suitCaseSpawned) return;
            if (suitCaseGrabbed) return;

            if (suitcase?.parent?.gameObject?.name == "ItemPivot")
            {
                Logic.PlayerCommittedOffence("GRAB_SUITCASE");
                suitCaseGrabbed = true;
                suitcase.parent = null;
            }
        }
    }
}
