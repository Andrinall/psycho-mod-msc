using UnityEngine;
using HutongGames.PlayMaker;
using System.Linq;
using MSCLoader;
using Harmony;
using HealthMod;

namespace Adrenaline
{
    internal class GlobalHandler : MonoBehaviour
    {
        private FsmFloat PlayerMovementSpeed;
        private FsmBool HouseBurningState;
        private FsmBool RallyPlayerOnStage;

        private PlayMakerFSM kiljuguy;

        private void OnEnable()
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
                RallyPlayerOnStage = Utils.GetGlobalVariable<FsmBool>("RallyPlayerOnStage");

                var fpsCamera = base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera");
                var drink = fpsCamera.Find("Drink").gameObject;
                GameHook.InjectStateHook(drink, "Activate 5", IncreaseByEnergyDrink);
                GameHook.InjectStateHook(drink, "Activate 7", IncreaseByCoffee);
                GameHook.InjectStateHook(drink, "HomeCoffee", IncreaseByCoffee);

                var fridge_paper = GameObject.Find("fridge_paper");
                GameHook.InjectStateHook(fridge_paper, "Use", "Wait button", SetFridgePaperText, true);
                
                var renderer = fridge_paper?.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    var mat = renderer.materials.ElementAt(0);

                    mat.name = "ATLAS_OFFICE(Clone)";
                    mat.mainTexture = AdrenalineLogic.atlas_texture;
                }

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
        }

        private void OnDestroy()
        {
            Object.Destroy(AdrenalineLogic._hud);
            AdrenalineLogic._hud = null;
        }

        private void FixedUpdate()
        {
            AdrenalineLogic.Tick();

            Utils.CacheFSM(ref kiljuguy, "KILJUGUY/KiljuMurderer", "Move");

            if (PlayerMovementSpeed?.Value >= 3.5)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("SPRINT_INCREASE").Value); // increase adrenaline while player sprinting

            if (HouseBurningState?.Value == true)
                AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("HOUSE_BURNING").Value); // increase adrenaline while house is burning

            if (kiljuguy?.ActiveStateName == "Walking")
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("MURDER_WALKING").Value); // ??
        }

        private void IncreaseByEnergyDrink()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("COFFEE_INCREASE").Value);
            AdrenalineLogic.SetDecreaseLocked(true, 12000f);
            if (ModLoader.IsModPresent("Health")) Health.editHp(-2f);
        }

        private void IncreaseByCoffee()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("COFFEE_INCREASE").Value);
            AdrenalineLogic.SetDecreaseLocked(true, 12000f); // 1 minute
        }

        private void SetFridgePaperText()
        {
            Utils.GetGlobalVariable<FsmString>("GUIsubtitle").Value = "Hi! You are sick and need to keep your adrenaline high! Drink coffee, drive fast, risk your life, conflict with society.\nYou can buy an energy drink at the Nappo pub.You will die if your adrenaline drops to zero or gets too high.\nLove, Doctor.";
        }
    }
}
