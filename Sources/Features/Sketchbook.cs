using UnityEngine;

using Psycho.Internal;


namespace Psycho.Features
{
    internal sealed class Sketchbook : BookWithGUI
    {
        public override GameObject GUIPrefab => Globals.SketchbookGUI_prefab;

        public override void AfterAwake()
            => MAX_PAGE = Globals.SketchbookPages.Count - 1;

        public override void PageSelected(bool next)
            => Background.SetTexture("_MainTex", Globals.SketchbookPages[CurrentPage]);
    }
} 
