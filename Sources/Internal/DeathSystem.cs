using MSCLoader;
using System;

using UnityEngine;


namespace Psycho.Internal
{
    internal class DeathSystem : CatchedComponent
    {
        public static DeathSystem instance { get; private set; } = null;
        Transform death;

        public override void Awaked()
        {
            instance = this;
            death = transform.Find("Death");
            Utils.PrintDebug($"DeathSystem init ; {death}");
        }

        public override void Destroyed()
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
            if (Logic.isDead) return;

            Transform paper = death.Find($"GameOverScreen/Paper/{palette}");
            if (paper == null) return;

            try
            {

                death.gameObject.SetActive(true);
                paper.Find("TextFI").GetComponent<TextMesh>().text = fi;
                paper.Find("TextEN").GetComponent<TextMesh>().text = en;
            }
            catch (Exception e)
            {
                Utils.PrintDebug($"Error in KillCustom_internal!\n{e.GetFullMessage()}");
            }

            Logic.isDead = true;
        }
    }
}
