using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;


namespace Psycho
{
    internal sealed class FliesChanger : MonoBehaviour
    {
        void OnEnable() => Change();

        public void Change()
        {
            try
            {
                var fliesfsm = transform.GetPlayMaker("Dirtiness");

                _changeAmbienceSoundsState();
                _createGlobalVariableIfNotExists();
                _changeActionsData(fliesfsm);
                _replaceAudioClips(fliesfsm);
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"Unable to change ambience&flies sounds after moving between worlds;\n{e.GetFullMessage()}");
            }
        }

        void _changeAmbienceSoundsState() => GameObject.Find("MAP/SoundAmbience")?.SetActive(!Logic.inHorror);

        void _createGlobalVariableIfNotExists()
        {
            if (FsmVariables.GlobalVariables.FloatVariables.Contains(Logic.psycho)) return;
            FsmVariables.GlobalVariables.FloatVariables =
                new List<FsmFloat>(FsmVariables.GlobalVariables.FloatVariables)
                { Logic.psycho }.ToArray();
        }

        void _changeActionsData(PlayMakerFSM _fsm)
        {
            var dirtiness = Utils.GetGlobalVariable<FsmFloat>("PlayerDirtiness");
            if (dirtiness == null) return;

            var clean = _fsm.GetState("Clean").Actions[6] as FloatCompare;
            clean.float1 = (Logic.inHorror ? Logic.psycho : dirtiness);

            var states = _sortStates(_fsm);

            var current = 1;
            var defaults = new float[5] { 60f, 90f, 120f, 150f, 180f };
            foreach (var state in states)
            {
                if (state == null) continue;
                if (!state.Name.Contains("Fly")) continue;
                if (state.Name == "Fly6") continue;

                var compare1 = (state.Actions[2] as FloatCompare);
                var compare2 = (state.Actions[3] as FloatCompare);
                if (Logic.inHorror)
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

            var fly6 = _fsm.GetState("Fly6");
            var compare6 = (fly6.Actions[2] as FloatCompare);
            compare6.float1 = Logic.psycho;
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
            var count = 0;
            foreach (var obj in _fsm.FsmVariables.GameObjectVariables)
            {
                var child = obj.Value;
                var fsm = child.GetPlayMaker("Move");
                var movee = fsm.GetState("Movee").Actions[0] as SetAudioClip;
                var stop = fsm.GetState("Stop").Actions[1] as SetAudioClip;

                var stopclip = stop.audioClip.Value as AudioClip;
                var moveeclip = movee.audioClip.Value as AudioClip;
                if (stopclip.name == "fly" && moveeclip.name == "fly2")
                {
                    Globals.flies_cached.Add(stopclip);
                    Globals.flies_cached.Add(moveeclip);
                }

                if (Logic.inHorror)
                {
                    var idx = Random.Range(0, Globals.horror_flies.Count);
                    var idx2 = Random.Range(0, Globals.horror_flies.Count);
                    movee.audioClip.Value = Globals.horror_flies[idx];
                    stop.audioClip.Value = Globals.horror_flies[idx2];
                    count += 2;
                    continue;
                }

                stop.audioClip.Value = Globals.flies_cached[count];
                movee.audioClip.Value = Globals.flies_cached[count + 1];
                count += 2;
            }
        }
    }
}
