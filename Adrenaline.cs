using MSCLoader;
using System.Threading;
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

        public static GameObject death;

        public static string deathTextMinAdrenaline = "Young male\nfound dead\nwith out of\nadrenaline in\n region of\nAlivieska";

        internal static Settings lossRateSet = new Settings("adrlossrate", "Adrenaline Loss Rate", 1f, updateSettings);

        public override void ModSetup()
        {
            SetupFunction(Setup.OnNewGame, Mod_NewGame);
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.OnSave, Mod_Save);
        }

        private void Mod_NewGame()
        {
            AdrenalineLogic.value = 100f;
        }

        private void Mod_OnLoad()
        {
            // load value and loss rate
            // ...

            death = GameObject.Find("Systems").transform.Find("Death").gameObject;

            AdrenalineBar.Setup();
            AdrenalineLogic.Set(100f);

            GameObject.Find("PLAYER").AddComponent<AdrenalineLogic>();

            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }

        private void Mod_Save()
        {
            // save value and loss rate
        }

        public static void Kill()
        {
            death.SetActive(true);
            death.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmBool("Fatigue").Value = true;

            death.transform.Find("GameOverScreen/Paper/Fatigue/TextEN").GetComponent<TextMesh>().text = deathTextMinAdrenaline;
            death.transform.Find("GameOverScreen/Paper/Fatigue/TextFI").GetComponent<TextMesh>().text = deathTextMinAdrenaline;
        }

        internal static void updateSettings()
        {
            AdrenalineLogic.lossRate = Mathf.Clamp((float)lossRateSet.Value, 0.5f, 3f);
        }
    }
}