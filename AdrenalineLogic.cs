using StructuredHUD;
using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;
using System.Linq;

namespace Adrenaline
{
    internal class AdrenalineLogic : MonoBehaviour
    {
        private const float MIN_ADRENALINE = 0f;
        private const float MAX_ADRENALINE = 2f;
        private const float DEFAULT_DECREASE = 0.18f;
        private const float DEFAULT_SPRINT_INCREASE = 0.28f;

        private GameObject death;
        private FsmFloat playerMovementSpeed;
        private PlayMakerFSM coffeeCup;

        private string deathTextMinAdrenaline = "Young male\nfound dead of\nhearth attack in\nregion of Alivieska";

        public float value = 100f;
        public float lossRate = 1f;
        public bool lockDecrease = false;
        public float lockCooldown = 12000f; // 1 minute

        public void OnEnable()
        {
            death = GameObject.Find("Systems").transform.Find("Death").gameObject;
            playerMovementSpeed = FsmVariables.GlobalVariables.FindFsmFloat("PlayerMovementSpeed");
            coffeeCup = GameObject.Find("Coffee").GetComponents<PlayMakerFSM>().FirstOrDefault(x => x.FsmName == "Use");
        }

        public void FixedUpdate()
        {
            // drunk coffee handler
            if (coffeeCup.ActiveStateName == "State 2" && !lockDecrease)
            {
                value += 5f;
                lockDecrease = true;
            }

            if (lockDecrease)
            {
                lockCooldown -= Time.fixedDeltaTime;
                if (lockCooldown <= 0)
                {
                    lockDecrease = false; // disable decrease lock
                    lockCooldown = 12000f;
                }
            }
            else
            {
                // basic decrease adrenaline
                value -= DEFAULT_DECREASE * lossRate * Time.fixedDeltaTime;
            }
            
            // increase adrenaline while player sprinting
            if (playerMovementSpeed.Value >= 3.5) value += DEFAULT_SPRINT_INCREASE * Time.fixedDeltaTime;

            Set(value);

            if (value <= 0)
                Kill();
            if (value >= 200)
                Kill();
        }

        public void Set(float value_)
        {
            var clamped = Mathf.Clamp(value_ / 100f, MIN_ADRENALINE, MAX_ADRENALINE);
            FixedHUD.SetElementScale("Adrenaline", new Vector3(clamped, 1f));
            FixedHUD.SetElementColor("Adrenaline", (clamped <= 0.15f || clamped >= 1.75f) ? Color.red : Color.white);

            value = clamped;
        }

        public void Kill()
        {
            death.SetActive(true);
            death.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmBool("Fatigue").Value = true;

            death.transform.Find("GameOverScreen/Paper/Fatigue/TextEN").GetComponent<TextMesh>().text = deathTextMinAdrenaline;
            death.transform.Find("GameOverScreen/Paper/Fatigue/TextFI").GetComponent<TextMesh>().text = deathTextMinAdrenaline;
        }
    }
}
