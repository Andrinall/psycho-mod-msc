using UnityEngine;

using Psycho.Internal;
using System;


namespace Psycho.Handlers
{
    internal sealed class SuskiHelpHandler : CatchedComponent
    {
        protected override void Awaked()
        {
            GameObject _stages = transform.Find("SuskiStages")?.gameObject;
            if (_stages == null)
                throw new NullReferenceException("SuskiStages not exists!");

            StateHook.Inject(_stages, "Logic", "Enable note", SuskiMoved);
        }

        void SuskiMoved() => Logic.PlayerCompleteJob("SUSKI_HELP");
    }
}
