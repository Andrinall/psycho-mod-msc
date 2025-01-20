
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;


namespace Psycho.Handlers
{
    class FliesChanger : MonoBehaviour
    {
        void OnEnable() => Change();

        public void Change()
        {
            try
            {
                PlayMakerFSM _fliesfsm = transform.GetPlayMaker("Dirtiness");

                _changeAmbienceSoundsState();
                _createGlobalVariableIfNotExists();
                _replaceAudioClips(_fliesfsm);
                _changeActionsData(_fliesfsm);
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Unable to change ambience&flies sounds after moving between worlds;");
                ModConsole.Error(e.GetFullMessage());
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

        void _changeActionsData(PlayMakerFSM fsm)
        {
            FsmFloat _dirtiness = Utils.GetGlobalVariable<FsmFloat>("PlayerDirtiness");
            if (_dirtiness == null) return;

            FloatCompare _clean = fsm.GetState("Clean").Actions[6] as FloatCompare;
            _clean.float1 = (Logic.InHorror ? Logic.psycho : _dirtiness);

            FsmState[] _states = _sortStates(fsm);

            int _current = 1;
            float[] _defaults = new float[5] { 60f, 90f, 120f, 150f, 180f };

            foreach (FsmState _state in _states)
            {
                if (_state == null) continue;
                if (!_state.Name.Contains("Fly")) continue;
                if (_state.Name == "Fly6") continue;

                FloatCompare _compare1 = _state.Actions[2] as FloatCompare;
                FloatCompare _compare2 = _state.Actions[3] as FloatCompare;

                if (Logic.InHorror)
                {
                    _compare1.float1 = Logic.psycho;
                    _compare1.float2.Value = 18 * _current;

                    _compare2.float1 = Logic.psycho;
                    _compare2.float2.Value = _state.Name == "Fly1" ? 18 : _compare1.float2.Value - 30f;

                    _current++;
                    continue;
                }

                _compare1.float1 = _dirtiness;
                _compare1.float2.Value = _defaults[_current - 1];

                _compare2.float1 = _dirtiness;
                _compare2.float2.Value = _defaults[_current - 1] - 30f;

                _current++;
            }

            FsmState _fly6 = fsm.GetState("Fly6");
            FloatCompare _compare6 = (_fly6.Actions[2] as FloatCompare);
            _compare6.float1 = Logic.InHorror ? Logic.psycho : _dirtiness;
            _compare6.float2 = 90f;

        }

        FsmState[] _sortStates(PlayMakerFSM fsm)
        {
            var _states = new List<FsmState>(fsm.Fsm.States);
            _states.Sort((item, target) => {
                if (!int.TryParse(item.Name.Substring(3), out int i)) return -1;
                if (!int.TryParse(target.Name.Substring(3), out int t)) return 1;
                return i < t ? -1 : 0;
            });
            fsm.Fsm.States = _states.ToArray();
            return fsm.Fsm.States;
        }

        void _replaceAudioClips(PlayMakerFSM fsm)
        {
            int _count = 0;
            foreach (FsmGameObject _obj in fsm.FsmVariables.GameObjectVariables)
            {
                GameObject _child = _obj.Value;
                PlayMakerFSM _fsm = _child.GetPlayMaker("Move");
                SetAudioClip _movee = _fsm.GetState("Movee").Actions[0] as SetAudioClip;
                SetAudioClip _stop = _fsm.GetState("Stop").Actions[1] as SetAudioClip;

                AudioClip _stopclip = _stop.audioClip.Value as AudioClip;
                AudioClip _moveeclip = _movee.audioClip.Value as AudioClip;
                if (_stopclip.name == "fly" && _moveeclip.name == "fly2")
                {
                    Globals.CachedFliesSounds.Add(_stopclip);
                    Globals.CachedFliesSounds.Add(_moveeclip);
                }

                if (Logic.InHorror)
                {
                    int _idx = Random.Range(0, ResourcesStorage.HorrorFliesSounds.Count);
                    int _idx2 = Random.Range(0, ResourcesStorage.HorrorFliesSounds.Count);
                    _movee.audioClip.Value = ResourcesStorage.HorrorFliesSounds[_idx];
                    _stop.audioClip.Value = ResourcesStorage.HorrorFliesSounds[_idx2];
                    _count += 2;
                    continue;
                }

                _stop.audioClip.Value = Globals.CachedFliesSounds[_count];
                _movee.audioClip.Value = Globals.CachedFliesSounds[_count + 1];
                _count += 2;
            }
        }
    }
}
