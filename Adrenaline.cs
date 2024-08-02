using MSCLoader;

namespace Adrenaline
{
    public class Adrenaline : Mod
    {
        public override string ID => "com.adrenaline.mod";
        public override string Name => "Adrenaline";
        public override string Author => "Andrinall,@racer";
        public override string Version => "0.0.1";
        public override string Description => "";

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
        }

        private void Mod_OnLoad()
        {
            AdrenalineBar.Init();
            AdrenalineBar.Set(72);

            ModConsole.Print("[Adrenaline]: <color=green>Successfully loaded!</color>");
        }
    }
}