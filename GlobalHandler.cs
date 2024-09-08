using System.Linq;

using Harmony;
using MSCLoader;
using HealthMod;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class GlobalHandler : MonoBehaviour
    {
        private GameObject kiljuguy;
        private Transform HouseFire;

        private FsmFloat PlayerMovementSpeed;
        private FsmBool HouseBurningState;
        private FsmInt GlobalDay;

        private void Awake()
        {
            try
            {
                AdrenalineLogic._hud = GameObject.Find("GUI/HUD").AddComponent<FixedHUD>();
                AdrenalineLogic._hud.AddElement(eHUDCloneType.RECT, "Adrenaline", AdrenalineLogic._hud.GetIndexByName("Money"));
                Utils.PrintDebug(eConsoleColors.GREEN, "HUD Enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to init HUD bar");
            }

            try
            {
                PlayerMovementSpeed = Utils.GetGlobalVariable<FsmFloat>("PlayerMovementSpeed");
                HouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
                GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
                HouseFire = GameObject.Find("YARD/Building/HOUSEFIRE").transform;
                kiljuguy = GameObject.Find("KILJUGUY");

                if (kiljuguy.transform.childCount > 3)
                    kiljuguy.transform.Find("KiljuMurderer").gameObject.AddComponent<KiljuMurdererHandler>();

                var fridge_paper = GameObject.Find("fridge_paper");
                var fpsCamera = base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera");
                var drink = fpsCamera.Find("Drink").gameObject;
                GameHook.InjectStateHook(drink, "Activate 5", IncreaseByEnergyDrink);
                GameHook.InjectStateHook(drink, "Activate 7", IncreaseByCoffee);
                GameHook.InjectStateHook(drink, "HomeCoffee", IncreaseByCoffee);
                GameHook.InjectStateHook(fridge_paper, "Use", "Wait button", SetFridgePaperText, true);

                Utils.SetMaterial(fridge_paper, 0, "ATLAS_OFFICE(Clone)", Globals.atlas_texture, Vector2.zero, Vector2.one);
                Utils.PrintDebug(eConsoleColors.GREEN, "GlobalHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load GlobalHandler component");
            }

            try
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Loading component FITTAN::HighSpeedHandler");
                Resources.FindObjectsOfTypeAll<Transform>().Where(v => v.name == "FITTAN") // avoid error (scene contains two fittan objects)
                    .Where(v => v.GetComponents<PlayMakerFSM>().Length == 4).ToArray()[0].gameObject
                    .AddComponent<HighSpeedHandler>();
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load HighSpeedHandler component for FITTAN");
            }

            if (AdrenalineLogic.LastDayUpdated == -1)
                AdrenalineLogic.LastDayUpdated = GlobalDay.Value;
        }

        private void OnDestroy()
        {
            Destroy(AdrenalineLogic._hud);
        }

        private void FixedUpdate()
        {
            AdrenalineLogic.Tick();

            if (PlayerMovementSpeed.Value >= 3.5)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("SPRINT_INCREASE")); // increase adrenaline while player sprinting

            if (HouseBurningState.Value == true)
            {
                if (Vector3.Distance(HouseFire.position, transform.position) > 6f) return;
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("HOUSE_BURNING")); // increase adrenaline while house is burning
                return;
            }

            if (GlobalDay.Value != AdrenalineLogic.LastDayUpdated)
            {
                var mailFromDoctor = GameObject.Find("YARD/PlayerMailBox/EnvelopeDoctor");
                mailFromDoctor.SetActive(true);
                mailFromDoctor.GetComponent<PlayMakerFSM>().enabled = true;

                Utils.PrintDebug("Day updated from {0} to {1}", AdrenalineLogic.LastDayUpdated, GlobalDay.Value);
                AdrenalineLogic.LastDayUpdated = GlobalDay.Value;
            }
        }

        private void IncreaseByEnergyDrink()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("ENERGY_DRINK_INCREASE"));
            AdrenalineLogic.SetDecreaseLocked(true, 12000f);
            if (ModLoader.IsModPresent("Health")) Health.editHp(-4f);
        }

        private void IncreaseByCoffee()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("COFFEE_INCREASE"));
            AdrenalineLogic.SetDecreaseLocked(true, 12000f); // 1 minute
        }

        private void SetFridgePaperText()
        {
            Utils.GetGlobalVariable<FsmString>("GUIsubtitle").Value = "Hi! You are sick and need to keep your adrenaline high! Drink coffee, drive fast, risk your life, conflict with society.\nYou can buy an energy drink at the Nappo pub.You will die if your adrenaline drops to zero or gets too high.\nLove, Doctor.";
        }
    }
}
