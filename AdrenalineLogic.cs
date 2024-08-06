using StructuredHUD;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using MSCLoader;
using System.Linq;
using HealthMod;
using HutongGames.PlayMaker.Actions;

namespace Adrenaline
{
    internal class AdrenalineLogic : MonoBehaviour
    {
        private enum CARS : int { JONEZZ = 0, SATSUMA = 1, FERNDALE = 2, HAYOSIKO = 3 };
        private readonly string[] CAR_NAMES = { "Jonezz", "Satsuma", "Ferndale", "Hayosiko" };
        private readonly float[] CAR_SPEEDS = { 70f, 120f, 110f, 110f };

        private readonly string PAPER_TEXT_FI = "";
        private readonly string DEATH_TEXT_HEARTH_ATTACK = "Man found dead\nwith hearth attack\nin region of\nAlivieska";

        private readonly float MIN_ADRENALINE = 0f;
        private readonly float MAX_ADRENALINE = 2f;


        private float DEFAULT_DECREASE = 0.18f;
        private float DEFAULT_SPRINT_INCREASE = 0.28f;
        private float DEFAULT_HIGHSPEED_INCREASE = 0.35f;
        private float DEFAULT_FIGHT_INCREASE = 0.47f;
        private float DEFAULT_WINDOW_BREAK_INCREASE = 5f;
        private float HOUSE_BURNING = 0.56f;
        private float TEIMO_PISS = 0.035f;
        private float LOW_HP = 0.018f;
        private float HIGH_HP = 0.018f;

        private GameObject death;
        private PlayMakerFSM storeWindow;
        private PlayMakerFSM pubWindow;
        private FsmFloat playerMovementSpeed;
        private FsmString playerCurrentCar;
        private FsmInt fight;
        private FsmBool houseBurning;
        private FsmBool teimoPiss;

        public readonly List<Drivetrain> drivetrains = new List<Drivetrain>();

        public float value = 100f;
        public float lossRate = 1.2f;
        public bool lockDecrease = false;
        public float lockCooldown = 12000f; // 1 minute


        public void OnEnable()
        {
            death = GameObject.Find("Systems").transform.Find("Death").gameObject;
            playerMovementSpeed = FsmVariables.GlobalVariables.FindFsmFloat("PlayerMovementSpeed");
            playerCurrentCar = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle");
            houseBurning = FsmVariables.GlobalVariables.FindFsmBool("HouseBurning");

            //teimoPiss = FsmVariables.GlobalVariables.FindFsmBool("STORE/TeimoInShop/Pivot/FacePissTrigger/Reaction/PissOn");

            drivetrains.Insert((int)CARS.JONEZZ, GameObject.Find("JONNEZ ES(Clone)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.SATSUMA, GameObject.Find("SATSUMA(557kg, 248)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.FERNDALE, GameObject.Find("FERNDALE(1630kg)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.HAYOSIKO, GameObject.Find("HAYOSIKO(1500kg, 250)").GetComponent<Drivetrain>());

            pubWindow = GameObject.Find("STORE/LOD/GFX_Pub/BreakableWindowsPub/BreakableWindowPub").GetComponents<PlayMakerFSM>().First(x => x.FsmName == "Break");
            storeWindow = GameObject.Find("STORE/LOD/GFX_Store/BreakableWindows/BreakableWindow").GetComponents<PlayMakerFSM>().First(x => x.FsmName == "Break");
            //fight = FsmVariables.GlobalVariables.FindFsmInt("DANCEHALL/Functions/FIGHTER/Fighter/Move/Anger");
            
        }

        public void FixedUpdate()
        {
            if (!DecreaseIsLocked()) value -= DEFAULT_DECREASE * lossRate * Time.fixedDeltaTime; // basic decrease adrenaline

            if (playerMovementSpeed.Value >= 3.5) value += DEFAULT_SPRINT_INCREASE * Time.fixedDeltaTime; // increase adrenaline while player sprinting

            if (houseBurning.Value == true) lossRate -= HOUSE_BURNING - Time.fixedDeltaTime;


            //if (teimoPiss.Value == true) lossRate -= TEIMO_PISS - Time.fixedDeltaTime;


            if (HealthMod.Health.hp <= 41) lossRate -= LOW_HP - Time.fixedDeltaTime;

            if (HealthMod.Health.hp >= 80) lossRate += HIGH_HP * Time.fixedDeltaTime;


            //if (fight.Value == 4) value += FIGHT * Time.fixedDeltaTime;

            CheckBreakWindow();
            CheckHighSpeed(); // increase adrenaline from driving on high speed
            SetAdrenaline(value);

            if (value <= 0) Health.killCustom(DEATH_TEXT_HEARTH_ATTACK, PAPER_TEXT_FI);
            if (value >= 200) Health.killCustom(DEATH_TEXT_HEARTH_ATTACK, PAPER_TEXT_FI);
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

        private void CheckBreakWindow()
        {
            ModConsole.Print(storeWindow.ActiveStateName);
            if (storeWindow.ActiveStateName == "Shatter")
                value += DEFAULT_WINDOW_BREAK_INCREASE * Time.fixedDeltaTime;

            if (pubWindow.ActiveStateName == "Shatter")
                value += DEFAULT_WINDOW_BREAK_INCREASE * Time.fixedDeltaTime;
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
    }
}
