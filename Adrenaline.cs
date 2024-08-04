using MSCLoader;
using StructuredHUD;
using UnityEngine;

namespace Adrenaline
{
    public class Adrenaline : Mod
    {
        public override string ID => "com.adrenaline.mod";
        public override string Name => "Adrenaline";
        public override string Author => "Andrinall,@racer";
        public override string Version => "0.0.1";
        public override string Description => "";

        internal AdrenalineLogic logic;

        public float tempValue = 100f;
        public float tempLoss = 1f;

        public override void OnNewGame()
        {
            tempValue = 100f;
            tempLoss = 1f;
            
            if(logic != null)
            {
                logic.value = 100f;
                logic.lossRate = 1f;
            }
        }

        public override void OnLoad()
        {
            // load value and loss rate
            // ...

            FixedHUD.AddElement(eHUDCloneType.RECT, "Adrenaline", "Money");

            logic = GameObject.Find("PLAYER").AddComponent<AdrenalineLogic>();
            logic.value = tempValue;
            logic.lossRate = tempLoss;

            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        public override void OnGUI()
        {
            FixedHUD.Structurize();
        }

        public override void OnSave()
        {
            // save value and loss rate
        }

        internal static void updateSettings()
        {
            //AdrenalineLogic.lossRate = Mathf.Clamp((float)lossRateSet.Value, 0.5f, 3f);
        }
    }
}