
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class Hangover : CatchedComponent
    {
        const float m_fHangoverStrenght = 0.004251007f;
        const float m_fHangoverStrenghtMinus = -0.004251007f; // defaults from game
        bool m_bHangoverActive = false;


        void FixedUpdate()
        {
            if (!Logic.InHorror)
            {
                if (!m_bHangoverActive) return;
                
                transform.localPosition = Vector3.zero;
                m_bHangoverActive = false;
                return;
            }

            m_bHangoverActive = true;
            transform.localPosition = new Vector3(
                Random.Range(m_fHangoverStrenghtMinus, m_fHangoverStrenght),
                Random.Range(m_fHangoverStrenghtMinus, m_fHangoverStrenght),
                Random.Range(m_fHangoverStrenghtMinus, m_fHangoverStrenght)
            );
        }
    }
}
