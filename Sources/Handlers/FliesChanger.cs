
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;


namespace Psycho.Handlers
{
    internal sealed class FliesChanger : CatchedComponent
    {
        void OnEnable() => Change();

        public void Change()
        {
            try
            {
                PlayMakerFSM fliesfsm = transform.GetPlayMaker("Dirtiness");

                _changeAmbienceSoundsState();
                _createGlobalVariableIfNotExists();
                _replaceAudioClips(fliesfsm);
                _changeActionsData(fliesfsm);
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Unable to change ambience&flies sounds after moving between worlds;\n{e.GetFullMessage()}");
            }
        }

        void _changeAmbienceSoundsState() => GameObject.Find("MAP/SoundAmbience")?.SetActive(!Logic.InHorror);

        void _createGlobalVariableIfNotExists()
        {
            if (FsmVariables.GlobalVariables.FloatVariables.Contains(Logic.psycho)) return;
            FsmVariables.GlobalVariables.FloatVariables =
                new List<FsmFloat>(FsmVariables.GlobalVariables.FloatVariables)
                { Logic.psycho }.ToArray();
        }

        void _changeActionsData(PlayMakerFSM _fsm)
        {
            FsmFloat dirtiness = Utils.GetGlobalVariable<FsmFloat>("PlayerDirtiness");
            if (dirtiness == null) return;

            FloatCompare clean = _fsm.GetState("Clean").Actions[6] as FloatCompare;
            clean.float1 = (Logic.InHorror ? Logic.psycho : dirtiness);

            FsmState[] states = _sortStates(_fsm);

            int current = 1;
            float[] defaults = new float[5] { 60f, 90f, 120f, 150f, 180f };

            foreach (FsmState state in states)
            {
                if (state == null) continue;
                if (!state.Name.Contains("Fly")) continue;
                if (state.Name == "Fly6") continue;

                FloatCompare compare1 = state.Actions[2] as FloatCompare;
                FloatCompare compare2 = state.Actions[3] as FloatCompare;

                if (Logic.InHorror)
                {
                    compare1.float1 = Logic.psycho;
                    compare1.float2.Value = 18 * current;

                    compare2.float1 = Logic.psycho;
                    compare2.float2.Value = state.Name == "Fly1" ? 18 : compare1.float2.Value - 30f;

                    current++;
                    continue;
                }

                compare1.float1 = dirtiness;
                compare1.float2.Value = defaults[current - 1];

                compare2.float1 = dirtiness;
                compare2.float2.Value = defaults[current - 1] - 30f;

                current++;
            }

            FsmState fly6 = _fsm.GetState("Fly6");
            FloatCompare compare6 = (fly6.Actions[2] as FloatCompare);
            compare6.float1 = Logic.InHorror ? Logic.psycho : dirtiness;
            compare6.float2 = 90f;

        }

        FsmState[] _sortStates(PlayMakerFSM _fsm)
        {
            var states = new List<FsmState>(_fsm.Fsm.States);
            states.Sort((item, target) => {
                if (!int.TryParse(item.Name.Substring(3), out int i)) return -1;
                if (!int.TryParse(target.Name.Substring(3), out int t)) return 1;
                return i < t ? -1 : 0;
            });
            _fsm.Fsm.States = states.ToArray();
            return _fsm.Fsm.States;
        }

        void _replaceAudioClips(PlayMakerFSM _fsm)
        {
            int count = 0;
            foreach (FsmGameObject obj in _fsm.FsmVariables.GameObjectVariables)
            {
                GameObject child = obj.Value;
                PlayMakerFSM fsm = child.GetPlayMaker("Move");
                SetAudioClip movee = fsm.GetState("Movee").Actions[0] as SetAudioClip;
                SetAudioClip stop = fsm.GetState("Stop").Actions[1] as SetAudioClip;

                AudioClip stopclip = stop.audioClip.Value as AudioClip;
                AudioClip moveeclip = movee.audioClip.Value as AudioClip;
                if (stopclip.name == "fly" && moveeclip.name == "fly2")
                {
                    Globals.FliesCached.Add(stopclip);
                    Globals.FliesCached.Add(moveeclip);
                }

                if (Logic.InHorror)
                {
                    int idx = Random.Range(0, Globals.HorrorFlies.Count);
                    int idx2 = Random.Range(0, Globals.HorrorFlies.Count);
                    movee.audioClip.Value = Globals.HorrorFlies[idx];
                    stop.audioClip.Value = Globals.HorrorFlies[idx2];
                    count += 2;
                    continue;
                }

                stop.audioClip.Value = Globals.FliesCached[count];
                movee.audioClip.Value = Globals.FliesCached[count + 1];
                count += 2;
            }
        }
    }
}
