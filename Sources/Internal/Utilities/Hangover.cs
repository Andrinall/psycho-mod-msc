
using UnityEngine;


namespace Psycho.Internal
{
    class Hangover : MonoBehaviour
    {
        const float STRENGHT_PLUS = 0.004251007f;
        const float STRENGHT_MINUS = -0.004251007f; // defaults from game
        bool hangoverActive = false;


        void FixedUpdate()
        {
            if (!Logic.InHorror)
            {
                if (!hangoverActive) return;
                
                transform.localPosition = Vector3.zero;
                hangoverActive = false;
                return;
            }

            hangoverActive = true;
            transform.localPosition = new Vector3(
                Random.Range(STRENGHT_MINUS, STRENGHT_PLUS),
                Random.Range(STRENGHT_MINUS, STRENGHT_PLUS),
                Random.Range(STRENGHT_MINUS, STRENGHT_PLUS)
            );
        }
    }
}
