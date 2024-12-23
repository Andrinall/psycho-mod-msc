using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    public sealed class Sketchbook : BookWithGUI
    {
        protected override GameObject GUIPrefab => Globals.SketchbookGUI_prefab;

        protected override void AfterAwake()
            => MAX_PAGE = Globals.SketchbookPages.Count - 1;

        protected override void PageSelected(bool next)
            => Background.SetTexture("_MainTex", Globals.SketchbookPages[CurrentPage]);
    }
} 
