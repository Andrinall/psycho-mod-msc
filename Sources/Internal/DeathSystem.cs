
using System;

using MSCLoader;

using UnityEngine;


namespace Psycho.Internal
{
    public class DeathSystem : CatchedComponent
    {
        static DeathSystem instance = null;
        Transform death;

        protected override void Awaked()
        {
            instance = this;
            death = transform.Find("Death");
            Utils.PrintDebug($"DeathSystem init ; {death}");
        }

        protected override void Destroyed()
        {
            instance = null;
        }

        public static void KillCustom(string palette, string en, string fi)
        {
            if (instance == null) return;

            instance.KillCustom_internal(palette, en, fi);
        }


        void KillCustom_internal(string palette, string en, string fi)
        {
            if (Logic.IsDead) return;

            Transform _paper = death.Find($"GameOverScreen/Paper/{palette}");
            if (_paper == null) return;

            try
            {

                death.gameObject.SetActive(true);
                _paper.Find("TextFI").GetComponent<TextMesh>().text = fi;
                _paper.Find("TextEN").GetComponent<TextMesh>().text = en;
            }
            catch (Exception e)
            {
                Utils.PrintDebug($"Error in KillCustom_internal!\n{e.GetFullMessage()}");
            }

            Logic.IsDead = true;
        }
    }
}
