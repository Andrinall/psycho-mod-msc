using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class SuitcaseHandler : CatchedComponent
    {
        bool m_bSuitCaseSpawned = false;
        bool m_bSuitCaseGrabbed = false;

        Transform _suitcase = null;


        public override void OnFixedUpdate()
        {
            if (_suitcase == null)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.childCount == 0) continue;

                    _suitcase = child.Find("suitcase(itemx)");
                    m_bSuitCaseSpawned = true;
                }
                return;
            }

            if (!m_bSuitCaseSpawned) return;
            if (m_bSuitCaseGrabbed) return;

            if (_suitcase?.parent?.gameObject?.name == "ItemPivot")
            {
                Logic.PlayerCommittedOffence("GRAB_SUITCASE");
                m_bSuitCaseGrabbed = true;
                _suitcase.parent = null;
            }
        }
    }
}
