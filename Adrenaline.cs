using MSCLoader;
using MSCLoader.Helper;
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

        public float tempValue = 54f;
        public float tempLoss = 1.2f;

       

        public override void OnNewGame()
        {
            tempValue = 54f;
            tempLoss = 1.2f;
            
            if(logic != null)
            {
                logic.value = 100f;
                logic.lossRate = 1.2f;
            }
        }

        public override void OnLoad()
        {
            // load value and loss rate
            // ...
            
            

            if (GameObject.Find("GUI/HUD/Adrenaline") == null)
            {
                FixedHUD.AddElement(eHUDCloneType.RECT, "Adrenaline", "Money");
            }

            if (logic == null)
                logic = GameObject.Find("PLAYER").AddComponent<AdrenalineLogic>();
                logic.value = tempValue;
                logic.lossRate = tempLoss;

            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        public override void OnSave()
        {
            
        }

        public override void FixedUpdate()
        {


        }
    }
}