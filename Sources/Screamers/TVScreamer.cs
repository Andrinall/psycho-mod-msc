using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;
using Psycho.Extensions;


namespace Psycho.Screamers
{
    public sealed class TVScreamer : MonoBehaviour
    {
        GameObject SwitchObj;
        Transform NightProgram;

        PlayMakerFSM TVSwitch;
        PlayMakerFSM NightFSM;

        AudioSource TVAudio;

        Texture ReplaceTexture;
        Texture OrigTexture;

        AudioClip OrigAudioClip;

        bool ScreamEnabled = false;
        bool fullEnable = false;

        int elapsedFrames = 0;
        int neededFrames = 200;


        void Awake()
        {
            SwitchObj = GameObject.Find("YARD/Building/LIVINGROOM/TV/Switch");

            TVSwitch = SwitchObj.GetComponent<PlayMakerFSM>();
            StateHook.Inject(SwitchObj, "Use", "Switch", 0, _hook);

            TVSwitch.Fsm.InitData();
            TVSwitch.AddEvent("SCREAM_ON");
            TVSwitch.AddGlobalTransition("SCREAM_ON", "State 5");

            var state = TVSwitch.GetState("Switch");
            state.Transitions = new List<FsmTransition>(state.Transitions)
            {
                new FsmTransition
                {
                    FsmEvent = TVSwitch.FsmEvents.First(v=> v.Name == "FINISHED"),
                    ToState = "Wait player"
                }
            }.ToArray();

            NightProgram = transform.Find("NightProgram");
            NightFSM = NightProgram.GetComponent<PlayMakerFSM>();
            NightFSM.Fsm.InitData();

            ReplaceTexture = Globals.pictures[Globals.pictures.Count - 1];
            OrigTexture = (NightFSM.GetState("State 4").Actions[0] as SetMaterialTexture).texture.Value;

            TVAudio = NightProgram.GetComponent<AudioSource>();
            OrigAudioClip = TVAudio.clip;

            enabled = false;
        }

        void OnEnable()
        {
            (TVSwitch.GetState("Switch").Actions[1] as BoolTest).Enabled = false;
            (TVSwitch.GetState("Close TV 2").Actions[8] as ActivateGameObject).Enabled = false;
            TVSwitch.CallGlobalTransition("SCREAM_ON");

            fullEnable = true;
            WorldManager.ShowCrows(false);
            Utils.PrintDebug(eConsoleColors.GREEN, "TVScreamer enabled");
        }

        void OnDisable()
        {
            if (!fullEnable) return;

            (TVSwitch.GetState("Switch").Actions[1] as BoolTest).Enabled = true;
            (TVSwitch.GetState("Close TV 2").Actions[8] as ActivateGameObject).Enabled = true;
            TVSwitch.CallGlobalTransition("GLOBALEVENT");

            _setAudioClip(OrigAudioClip);

            elapsedFrames = 0;
            ScreamEnabled = false;
            fullEnable = false;

            WorldManager.ShowCrows(true);
            Utils.PrintDebug(eConsoleColors.RED, "TVScreamer disabled");
        }

        void FixedUpdate()
        {
            if (!ScreamEnabled) return;
            if (elapsedFrames <= neededFrames)
            {
                elapsedFrames++;
                return;
            }

            enabled = false;
            Utils.PrintDebug(eConsoleColors.YELLOW, "elapsed == needed; TVSwitch enabled, component disabled");
        }

        void _hook(PlayMakerFSM _fsm)
        {
            if (!fullEnable) return;
            
            _fsm.SendEvent("FINISHED");
            if (ScreamEnabled) return;
            
            _setTexture();
            _setAudioClip(Globals.TVScreamSound);
            ScreamEnabled = true;

            Utils.PrintDebug(eConsoleColors.YELLOW, "TVScreamer hook called; screamer enabled;");
        }

        void _setTexture()
        {
            (NightFSM.GetState("State 4").Actions[0] as SetMaterialTexture)
                .material.Value.SetTexture("_MainTex", ReplaceTexture);

            Utils.PrintDebug(eConsoleColors.YELLOW, "Texture setted");
        }

        void _setAudioClip(AudioClip clip)
        {
            TVAudio.Stop();
            TVAudio.clip = clip;
            TVAudio.Play();
            Utils.PrintDebug(eConsoleColors.YELLOW, $"Audio clip setted with name {clip.name}");
        }
    }
}
