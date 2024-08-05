using StructuredHUD;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using MSCLoader;

namespace Adrenaline
{
    internal class AdrenalineLogic : MonoBehaviour
    {
        private enum CARS : int { JONEZZ = 0, SATSUMA = 1, FERNDALE = 2, HAYOSIKO = 3 };
        private readonly string[] CAR_NAMES = { "Jonezz", "Satsuma", "Ferndale", "Hayosiko" };
        private readonly float[] CAR_SPEEDS = { 70f, 120f, 110f, 110f };

        private readonly string deathTextMinAdrenaline = "Young male\nfound dead of\nhearth attack in\nregion of Alivieska";

        private readonly float MIN_ADRENALINE = 0f;
        private readonly float MAX_ADRENALINE = 2f;


        private float DEFAULT_DECREASE = 0.18f;
        private float DEFAULT_SPRINT_INCREASE = 0.28f;
        private float DEFAULT_HIGHSPEED_INCREASE = 0.35f;
        private float FIGHT = 0.47f;

        private GameObject death;
        private FsmFloat playerMovementSpeed;
        private FsmString playerCurrentCar;
        private FsmBool breakWindowShop;
        private FsmBool breakWindowPub;
        private FsmInt fight;

        public readonly List<Drivetrain> drivetrains = new List<Drivetrain>();

        public float value = 100f;
        public float lossRate = 1f;
        public bool lockDecrease = false;
        public float lockCooldown = 12000f; // 1 minute


        public void OnEnable()
        {
            death = GameObject.Find("Systems").transform.Find("Death").gameObject;
            playerMovementSpeed = FsmVariables.GlobalVariables.FindFsmFloat("PlayerMovementSpeed");
            playerCurrentCar = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle");

            drivetrains.Insert((int)CARS.JONEZZ, GameObject.Find("JONNEZ ES(Clone)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.SATSUMA, GameObject.Find("SATSUMA(557kg, 248)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.FERNDALE, GameObject.Find("FERNDALE(1630kg)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.HAYOSIKO, GameObject.Find("HAYOSIKO(1500kg, 250)").GetComponent<Drivetrain>());

            breakWindowPub = FsmVariables.GlobalVariables.FindFsmBool("STORE/LOD/GFX_Pub/BreakableWindowsPub/BreakableWindowPub/Break/Open2");
            breakWindowShop = FsmVariables.GlobalVariables.FindFsmBool("STORE/LOD/GFX_Store/BreakableWindows/BreakableWindow/Break/Open2");
            fight = FsmVariables.GlobalVariables.FindFsmBool("DANCEHALL/Functions/FIGHTER/Fighter/Move/Anger");
        }

        public void FixedUpdate()
        {
            if (breakWindowShop.Value == true)
            {
                value += 25f;
            }

            if (breakWindowPub.Value == true)
            {
                value += 25f;
            }

            if (!DecreaseIsLocked())
                value -= DEFAULT_DECREASE * lossRate * Time.fixedDeltaTime; // basic decrease adrenaline

            if (playerMovementSpeed.Value >= 3.5)
                value += DEFAULT_SPRINT_INCREASE * Time.fixedDeltaTime; // increase adrenaline while player sprinting

            if (fight.Value == 4) value += FIGHT * Time.fixedDeltaTime; /* Задокументировал
                                                                             а теперь в деньгах */
                                           

            CheckHighSpeed(); // increase adrenaline from driving on high speed
            SetAdrenaline(value);

            if (value <= 0) Kill();
            if (value >= 200) Kill();
        }

        public void SetAdrenaline(float value_)
        {
            var clamped = Mathf.Clamp(value_ / 100f, MIN_ADRENALINE, MAX_ADRENALINE);
            value = value_;

            FixedHUD.SetElementScale("Adrenaline", new Vector3(clamped, 1f));
            FixedHUD.SetElementColor("Adrenaline", (clamped <= 0.15f || clamped >= 1.75f) ? Color.red : Color.white);
        }

        private void CheckHighSpeed()
        {
            foreach (Drivetrain car in drivetrains)
            {
                if (car == null) continue;

                int idx = drivetrains.IndexOf(car);
                if (idx == -1) continue;
                if (playerCurrentCar.Value != CAR_NAMES[idx]) continue;
                if (car.differentialSpeed > CAR_SPEEDS[idx])
                    value += DEFAULT_HIGHSPEED_INCREASE * Time.fixedDeltaTime;
            }
        }

        private void SetDecreaseLock(float time)
        {
            if (lockDecrease) return;
            lockDecrease = true;
            lockCooldown = time;
        }

        private bool DecreaseIsLocked()
        {
            if (!lockDecrease) return false;
            
            lockCooldown -= Time.fixedDeltaTime;
            if (lockCooldown >= 0) return true;

            lockDecrease = false; // disable decrease lock
            return false;
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
