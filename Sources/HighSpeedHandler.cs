using Harmony;
using UnityEngine;
using HutongGames.PlayMaker;
using MSCLoader;

namespace Adrenaline
{
    internal class HighSpeedHandler : MonoBehaviour
    {
        private string CarName;
        private Drivetrain drivetrain;
        private FsmString PlayerVehicle;
        private FsmBool RallyPlayerOnStage;
        private FsmBool SeatbeltLocked;
        private FsmBool Helmet;

        private void Start()
        {
            try
            {
                CarName = Utils.GetCarNameByObject(base.transform.gameObject);

                drivetrain = base.GetComponent<Drivetrain>();
                PlayerVehicle = Utils.GetGlobalVariable<FsmString>("PlayerCurrentVehicle");
                RallyPlayerOnStage = Utils.GetGlobalVariable<FsmBool>("RallyPlayerOnStage");
                SeatbeltLocked = Utils.GetGlobalVariable<FsmBool>("PlayerSeatbeltsOn");
                Helmet = Utils.GetGlobalVariable<FsmBool>("PlayerHelmet");
                Utils.PrintDebug(eConsoleColors.GREEN, $"HighSpeedHandler enabled for {CarName}");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to load HighSpeedHandler component for {CarName}\n{e.GetFullMessage()}");
            }
        }

        private void FixedUpdate()
        {
            if (PlayerVehicle.Value != CarName) return;
            
            var rally = RallyPlayerOnStage?.Value == true && CarName == "Satsuma";
            var RequiredSpeed = AdrenalineLogic.config.GetValueSafe("REQUIRED_SPEED_" + CarName);
            RequiredSpeed = (rally ? RequiredSpeed - 20f : RequiredSpeed);
            if (drivetrain.differentialSpeed <= RequiredSpeed) return;

            var value = AdrenalineLogic.config.GetValueSafe("HIGHSPEED_INCREASE");
            var helmet_debuff = AdrenalineLogic.config.GetValueSafe("HELMET_DECREASE");
            if (CarName == "Jonnez")
            {
                value += (Helmet.Value ? -helmet_debuff : helmet_debuff);
                AdrenalineLogic.IncreaseTimed(value);
                return;
            }

            var seatbelt_debuff = AdrenalineLogic.config.GetValueSafe("SEATBELT_DECREASE");
            var rallybuff = AdrenalineLogic.config.GetValueSafe("RALLY_PLAYER");
            value += (SeatbeltLocked.Value ? -seatbelt_debuff : seatbelt_debuff);
            value += (Helmet.Value ? -helmet_debuff : helmet_debuff);
            value += (rally ? rallybuff : 0f);

            AdrenalineLogic.IncreaseTimed(value);
        }
    }
}
