using HutongGames.PlayMaker;
using StructuredHUD;
using System.Linq;
using UnityEngine;

namespace Adrenaline
{
    internal class GlobalHandler : MonoBehaviour
    {
        private FsmFloat PlayerMovementSpeed;
        private FsmBool HouseBurningState;
        private FsmBool RallyPlayerOnStage;

        private PlayMakerFSM JanniHit;
        private PlayMakerFSM PetteriHit;
        private PlayMakerFSM FireElectric;
        private PlayMakerFSM SpilledShit;

        private void OnEnable()
        {
            PlayerMovementSpeed = Utils.GetGlobalVariable<FsmFloat>("PlayerMovementSpeed");
            HouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
            RallyPlayerOnStage = Utils.GetGlobalVariable<FsmBool>("RallyPlayerOnStage");

            FixedHUD.AddElement(eHUDCloneType.RECT, "Adrenaline", "Money");
            Utils.PrintDebug("GlobalHandler enabled");
        }

        private void OnDestroy()
        {
            if (FixedHUD._struct.FirstOrDefault(v => v.name == "Adrenaline") != null)
            {
                Destroy(GameObject.Find("GUI/HUD/Adrenaline"));
                Utils.PrintDebug("GHandler destroyed; HUD element is " + (FixedHUD.RemoveElement("Adrenaline") ? "" : "not") + " removed");
            }
        }

        private void FixedUpdate()
        {
            Utils.CacheFSM(ref JanniHit, "NPC_CARS/Amikset/KYLAJANI/Driver/Animations", "Logic");
            Utils.CacheFSM(ref PetteriHit, "NPC_CARS/Amikset/AMIS2/Passengers 3/Animations", "Logic");
            Utils.CacheFSM(ref FireElectric, "SATSUMA(557kg, 248)/Wiring/FireElectric", "Init");
            Utils.CacheFSM(ref SpilledShit, "GIFU(750/450psi)/ShitTank", "SpillPump");

            AdrenalineLogic.Tick();

            if (PlayerMovementSpeed.Value >= 3.5)
            {
                AdrenalineLogic.IncreaseTimed(Configuration.SPRINT_INCREASE); // increase adrenaline while player sprinting
                Utils.PrintDebug("Value increased by player sprinting");
            }

            if (HouseBurningState.Value == true)
            {
                AdrenalineLogic.IncreaseOnce(Configuration.HOUSE_BURNING); // increase adrenaline while house is burning
                Utils.PrintDebug("Value increased by house burning");
            }

            if (JanniHit?.ActiveStateName == "Hit")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.JANNI_PETTERI_HIT);
                Utils.PrintDebug("Value increased by Janni hit player");
            }

            if (PetteriHit?.ActiveStateName == "Hit")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.JANNI_PETTERI_HIT);
                Utils.PrintDebug("Value increased by Petteri hit player");
            }

            if (FireElectric?.ActiveStateName == "Sparks")
            {
                AdrenalineLogic.IncreaseOnce(Configuration.SPARKS_WIRING);
                Utils.PrintDebug("Value increased by hit from electricity into satsuma");
            }

            if (SpilledShit?.ActiveStateName == "Spill grow")
            {
                AdrenalineLogic.IncreaseTimed(Configuration.SPILL_SHIT);
                Utils.PrintDebug("Value increased by spill grow");
            }
        }
    }
}
