
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    internal abstract class FullScreenScreamerBase : CatchedComponent
    {
        bool screamerEnabled = false;
        GameObject hud;
        FsmFloat fatigue;
        FsmFloat stress;

        protected override void OnFixedUpdate()
        {
            if (!screamerEnabled) return;
            if (SoundManager.IsFullScreenScreamerSoundPlaying()) return;

            Globals.FullScreenScreamer.SetActive(false);
            SoundManager.FullScreenScreamerSoundsSource.Stop();

            hud?.SetActive(true);
            screamerEnabled = false;
        }

        public void TryShowScreamer()
        {
            if (screamerEnabled) return;
            if (!Logic.IsFullScreenScreamerAvailableForTrigger()) return;

            float _random = Random.Range(0f, 100f);
            if (_random < 5f || _random > 15f)
                return;
            
            if (hud == null)
                hud = GameObject.Find("GUI/HUD");

            if (fatigue == null)
                fatigue = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue");

            if (stress == null)
                stress = Utils.GetGlobalVariable<FsmFloat>("PlayerStress");
            
            Logic.EnableRandomFullScreenScreamer();
            hud.SetActive(false);

            fatigue.Value = Mathf.Clamp(fatigue.Value - 10f, -20f, 300f);
            stress.Value = Mathf.Clamp(stress.Value + 10f, -100f, 300f);

            screamerEnabled = true;
        }
    }
}
