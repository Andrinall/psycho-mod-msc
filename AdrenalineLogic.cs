using StructuredHUD;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using System.Linq;
using HealthMod;
using MSCLoader;

namespace Adrenaline
{
    internal class AdrenalineLogic : MonoBehaviour
    {
        private enum CARS : int { JONEZZ = 0, SATSUMA = 1, FERNDALE = 2, HAYOSIKO = 3 };
        private readonly string[] CAR_NAMES = { "Jonezz", "Satsuma", "Ferndale", "Hayosiko" };
        private readonly float[] CAR_SPEEDS = { 70f, 120f, 110f, 110f };

        private readonly string PAPER_TEXT_FI = "Mies kuoli\nsydänkohtaukseen";
        private readonly string DEATH_TEXT_HEARTH_ATTACK = "Man found\ndead of\nheart attack\nin region of\nAlivieska";

        private readonly float MIN_ADRENALINE = 0f;
        private readonly float MAX_ADRENALINE = 200f;

        private float DEFAULT_DECREASE = 0.18f;
        private float SPRINT_INCREASE = 0.3f;
        private float HIGHSPEED_INCREASE = 0.35f;
        private float FIGHT_INCREASE = 0.47f;
        private float WINDOW_BREAK_INCREASE = 6f;
        private float HOUSE_BURNING = 0.56f;
        private float TEIMO_PISS = 2.5f;
        private float GUARD_CATCH = 2.5f;
        private float VENTTI_WIN = 11f;
        private float LOW_HP_LOSS = 0.01f;
        private float HIGH_HP_LOSS = 0.01f;

        private PlayMakerFSM storeWindow;
        private PlayMakerFSM pubWindow;
        private PlayMakerFSM teimoFacePissTrigger;
        private PlayMakerFSM clubGuard;
        private PlayMakerFSM clubFighter;
        private PlayMakerFSM pubFighter;
        private PlayMakerFSM venttiGame;

        private FsmFloat playerMovementSpeed;
        private FsmString playerCurrentCar;
        private FsmBool houseBurning;

        public readonly List<Drivetrain> drivetrains = new List<Drivetrain>();

        public float value = 100f;
        public float lossRate = 1.2f;
        public bool lockDecrease = false;
        public float lockCooldown = 12000f; // 1 minute

        private List<string> fightStates = new List<string> { "State 1", "State 7", "State 10" };


        public void OnEnable()
        {
            playerMovementSpeed = FsmVariables.GlobalVariables.FindFsmFloat("PlayerMovementSpeed");
            playerCurrentCar = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle");
            houseBurning = FsmVariables.GlobalVariables.FindFsmBool("HouseBurning");

            drivetrains.Insert((int)CARS.JONEZZ, GameObject.Find("JONNEZ ES(Clone)")?.GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.SATSUMA, GameObject.Find("SATSUMA(557kg, 248)")?.GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.FERNDALE, GameObject.Find("FERNDALE(1630kg)")?.GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.HAYOSIKO, GameObject.Find("HAYOSIKO(1500kg, 250)")?.GetComponent<Drivetrain>());
        }

        public void FixedUpdate()
        {
            if (Health.hp < 30) SetLossRate(lossRate - LOW_HP_LOSS * Time.fixedDeltaTime);
            if (Health.hp > 80) SetLossRate(lossRate + HIGH_HP_LOSS * Time.fixedDeltaTime);

            if (!DecreaseIsLocked())
                value -= DEFAULT_DECREASE * lossRate * Time.fixedDeltaTime; // basic decrease adrenaline

            if (playerMovementSpeed.Value >= 3.5)
                IncreaseValue(SPRINT_INCREASE); // increase adrenaline while player sprinting

            if (houseBurning.Value == true)
                IncreaseValue(HOUSE_BURNING); // increase adrenaline while house is burning

            CheckHighSpeed(); // increase adrenaline while driving on high speed
            CheckStoreActions(); // increase adrenaline while breaking store window or piss on teimo
            CheckPubActions(); // increase adrenaline while breaking pub store or fighting
            CheckDanceHallActions(); // increase adrenaline while fighting or guart trying to catch player
            CheckVenttiWin(); // increase adrenaline for every defeat in ventti game

            SetAdrenaline(value); // set final adrenaline value of current checks iteration

            if ((value <= MIN_ADRENALINE || value >= MAX_ADRENALINE) && Health.damage(100f))
                Health.killCustom(DEATH_TEXT_HEARTH_ATTACK, PAPER_TEXT_FI);
        }

        public void SetAdrenaline(float value_)
        {
            var clamped = Mathf.Clamp(value_ / 100f, 0f, 1f);

            FixedHUD.SetElementScale("Adrenaline", new Vector3(clamped, 1f));
            FixedHUD.SetElementColor("Adrenaline", (clamped <= 0.15f || clamped >= 1.75f) ? Color.red : Color.white);
        }

        public void SetLossRate(float value_)
        {
            lossRate = Mathf.Clamp(value_, 0.6f, 1.4f);
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

        private void IncreaseValue(float val)
        {
            value += val * Time.fixedDeltaTime;
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
                    IncreaseValue(HIGHSPEED_INCREASE);
            }
        }

        private void CheckStoreActions()
        {
            CacheFSM(ref storeWindow, "STORE/LOD/GFX_Store/BreakableWindows/BreakableWindow", "Break");
            CacheFSM(ref teimoFacePissTrigger, "STORE/TeimoInShop/Pivot/FacePissTrigger", "Reaction", true);

            if (storeWindow?.ActiveStateName == "Shatter")
                IncreaseValue(WINDOW_BREAK_INCREASE);
            
            if (teimoFacePissTrigger?.ActiveStateName == "State 2")
                IncreaseValue(TEIMO_PISS);
        }

        private void CheckPubActions()
        {
            CacheFSM(ref pubWindow, "STORE/LOD/GFX_Pub/BreakableWindowsPub/BreakableWindowPub", "Break");

            if (pubWindow?.ActiveStateName == "Shatter")
                IncreaseValue(WINDOW_BREAK_INCREASE);

            //if (pubFighter != null) { }
        }

        private void CheckDanceHallActions()
        {
            CacheFSM(ref clubGuard, "DANCEHALL/Functions/GUARD/Guard", "React");
            CacheFSM(ref clubFighter, "DANCEHALL/Functions/FIGHTER/Fighter", "Hit");
            
            if (clubGuard?.ActiveStateName == "Catch")
                IncreaseValue(GUARD_CATCH);

            if (fightStates.Contains(clubFighter?.ActiveStateName))
                IncreaseValue(FIGHT_INCREASE);
        }

        private void CheckVenttiWin()
        {
            CacheFSM(ref venttiGame, "CABIN/Cabin/Ventti/Table/GameManager", "Use");

            if (venttiGame?.ActiveStateName == "Lose")
                IncreaseValue(VENTTI_WIN);
        }

        private void CacheFSM(ref PlayMakerFSM obj, string path, string fsm, bool single = false)
        {
            if (obj?.gameObject == null)
            {
                if (single)
                    obj = GameObject.Find(path)?.GetComponent<PlayMakerFSM>();
                else
                    obj = GameObject.Find(path)?.GetComponents<PlayMakerFSM>().FirstOrDefault(x => x.FsmName == fsm);

                if (obj?.gameObject != null) ModConsole.Print("Cached FSM: " + path + " | " + fsm);
            }
        }
    }
}
