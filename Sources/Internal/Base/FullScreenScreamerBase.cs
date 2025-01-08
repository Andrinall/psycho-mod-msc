
using UnityEngine;


namespace Psycho.Internal
{
    internal abstract class FullScreenScreamerBase : CatchedComponent
    {
        GameObject hud;

        bool screamerEnabled = false;


        protected override void OnFixedUpdate()
        {
            if (!screamerEnabled) return;
            if (SoundManager.IsFullScreenScreamerSoundPlaying()) return;

            Globals.FullScreenScreamer.SetActive(false);
            SoundManager.FullScreenScreamerSoundsSource.Stop();

            hud?.SetActive(true);
            screamerEnabled = false;
        }

        public void ShowScreamer()
        {
            if (Psycho.ShowFullScreenScreamers.GetValue() == false) return;
            if (screamerEnabled) return;
            if (!Logic.IsFullScreenScreamerAvailableForTrigger()) return;

            float _random = Random.Range(0f, 100f);
            if (_random < 5f || _random > 15f)
                return;
            
            if (hud == null)
                hud = GameObject.Find("GUI/HUD");
            
            Logic.EnableRandomFullScreenScreamer();
            hud.SetActive(false);

            Globals.PlayerFatigue.Value = Mathf.Clamp(Globals.PlayerFatigue.Value - 10f, -20f, 300f);
            Globals.PlayerStress.Value = Mathf.Clamp(Globals.PlayerStress.Value + 10f, -100f, 300f);

            screamerEnabled = true;
        }
    }
}
