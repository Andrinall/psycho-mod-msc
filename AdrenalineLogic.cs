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

        private readonly string PAPER_TEXT_FI = "Mies kuoli\nsydänkohtaukseen";
        private readonly string DEATH_TEXT_HEARTH_ATTACK = "Man found\ndead of\nheart attack\nin region of\nAlivieska";

        private readonly float MIN_ADRENALINE = 0f;
        private readonly float MAX_ADRENALINE = 200f;


        private float DEFAULT_DECREASE = 0.18f;
        private float SPRINT_INCREASE = 0.28f;
        private float HIGHSPEED_INCREASE = 0.35f;
        private float FIGHT_INCREASE = 0.47f;
        private float WINDOW_BREAK_INCREASE = 6f;
        private float HOUSE_BURNING = 0.56f;
        private float TEIMO_PISS = 2.5f;
        private float GUARD_CATCH = 2.5f;
        private float FIGHTING = 8f;
        private float VENTTI_WIN = 5f;
        private float LOW_HP_LOSS_MOD = 0.01f;
        private float HIGH_HP_LOSS_MOD = 0.01f;

        private GameObject death;
        private PlayMakerFSM storeWindow;
        private PlayMakerFSM pubWindow;
        private PlayMakerFSM teimoFacePissTrigger;
        private PlayMakerFSM dancehallGuardReact;
        private PlayMakerFSM clubFighter;
        private PlayMakerFSM pubFighter;
        private PlayMakerFSM venttiWin;
        private FsmFloat playerMovementSpeed;
        private FsmString playerCurrentCar;
        private FsmBool houseBurning;

        public readonly List<Drivetrain> drivetrains = new List<Drivetrain>();

        public float value = 100f;
        public float lossRate = 1.2f;
        public bool lockDecrease = false;
        public float lockCooldown = 12000f; // 1 minute

        private List<string> fightStates = new List<string> { "State 1", "State 7", "State 10" };

        private List<string> venttiStates = new List<string> { "Lose" };


        public void OnEnable()
        {
            death = GameObject.Find("Systems").transform.Find("Death").gameObject;
            playerMovementSpeed = FsmVariables.GlobalVariables.FindFsmFloat("PlayerMovementSpeed");
            playerCurrentCar = FsmVariables.GlobalVariables.FindFsmString("PlayerCurrentVehicle");
            houseBurning = FsmVariables.GlobalVariables.FindFsmBool("HouseBurning");

            drivetrains.Insert((int)CARS.JONEZZ, GameObject.Find("JONNEZ ES(Clone)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.SATSUMA, GameObject.Find("SATSUMA(557kg, 248)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.FERNDALE, GameObject.Find("FERNDALE(1630kg)").GetComponent<Drivetrain>());
            drivetrains.Insert((int)CARS.HAYOSIKO, GameObject.Find("HAYOSIKO(1500kg, 250)").GetComponent<Drivetrain>());

            pubWindow = GameObject.Find("STORE/LOD/GFX_Pub/BreakableWindowsPub/BreakableWindowPub").GetComponents<PlayMakerFSM>().First(x => x.FsmName == "Break");
            storeWindow = GameObject.Find("STORE/LOD/GFX_Store/BreakableWindows/BreakableWindow").GetComponents<PlayMakerFSM>().First(x => x.FsmName == "Break");
            teimoFacePissTrigger = GameObject.Find("STORE/TeimoInShop/Pivot/FacePissTrigger").GetComponent<PlayMakerFSM>();
            dancehallGuardReact = GameObject.Find("DANCEHALL/Functions/GUARD/Guard").GetComponents<PlayMakerFSM>().First(x => x.FsmName == "React");
            clubFighter = GameObject.Find("DANCEHALL/Functions/FIGHTER/Fighter").GetComponents<PlayMakerFSM>().First(x => x.FsmName == "Hit");
            venttiWin = GameObject.Find("CABIN/Cabin/Ventti/Table/GameManager").GetComponents<PlayMakerFSM>().First(x => x.FsmName == "Lose");
        }

        public void FixedUpdate()
        {
            if (Health.hp < 30) SetLossRate(lossRate - LOW_HP_LOSS_MOD * Time.fixedDeltaTime);
            if (Health.hp > 80) SetLossRate(lossRate + HIGH_HP_LOSS_MOD * Time.fixedDeltaTime);

            if (!DecreaseIsLocked())
                value -= DEFAULT_DECREASE * lossRate * Time.fixedDeltaTime; // basic decrease adrenaline

            if (playerMovementSpeed.Value >= 3.5)
                IncreaseValue(SPRINT_INCREASE); // increase adrenaline while player sprinting

            if (houseBurning.Value == true)
                IncreaseValue(HOUSE_BURNING); // increase adrenaline while house is burning

            if (teimoFacePissTrigger.ActiveStateName == "State 2")
                IncreaseValue(TEIMO_PISS); // increase andrenaline from piss on teimo in shop

            CheckBreakWindow(); // increase adrenaline while breaking store/pub windows
            CheckHighSpeed(); // increase adrenaline while driving on high speed
            CheckDanceHallActions();
            CheckVenttiWin();

            SetAdrenaline(value); // set final adrenaline value of current checks iteration

            if (value <= MIN_ADRENALINE) Health.killCustom(DEATH_TEXT_HEARTH_ATTACK, PAPER_TEXT_FI);
            if (value >= MAX_ADRENALINE) Health.killCustom(DEATH_TEXT_HEARTH_ATTACK, PAPER_TEXT_FI);
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

        private void CheckBreakWindow()
        {
            if (storeWindow.ActiveStateName == "Shatter")
                IncreaseValue(WINDOW_BREAK_INCREASE);

            if (pubWindow.ActiveStateName == "Shatter")
                IncreaseValue(WINDOW_BREAK_INCREASE);
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

        private void CheckDanceHallActions()
        {
            if (dancehallGuardReact.ActiveStateName == "Catch")
                IncreaseValue(GUARD_CATCH);

            if (clubFighter != null)
            {
                if (clubFighter.ActiveStateName != "State 3" && clubFighter.ActiveStateName != "State 6")
                    ModConsole.Print(clubFighter.ActiveStateName);

                if (fightStates.Contains(clubFighter.ActiveStateName))
                    IncreaseValue(FIGHTING);
            }

            if (pubFighter != null)
            {

            }
        }

        private void CheckVenttiWin()
        {
            if (venttiWin != null)
            {
                if (venttiStates.Contains(venttiWin.ActiveStateName))
                    IncreaseValue(VENTTI_WIN);
            }
        }
    }
}
