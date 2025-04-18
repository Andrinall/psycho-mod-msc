﻿
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    class HookInfo
    {
        public int Index = -1;
        public string FsmName = string.Empty;
        public string StateName = string.Empty;
    }


    class StateHook
    {
        public static Dictionary<FsmState, List<HookInfo>> hooksList = new Dictionary<FsmState, List<HookInfo>>();

        public static void Inject(GameObject gameObject, string stateName, Action hook)
        {
            try
            {
                PlayMakerFSM[] _components = gameObject.GetComponents<PlayMakerFSM>();
                if (_components.Length == 0) return;

                foreach (PlayMakerFSM _fsm in _components)
                {
                    if (_fsm == null) continue;
                    _fsm.InitializeFSM();

                    FsmState _state = _fsm.GetState(stateName);
                    if (_state == null) continue;

                    AddNewHooksContainer(_state);

                    int finalIndex = Insert(0, _fsm, _state, hook);
                    if (finalIndex == -1) return;

                    hooksList[_state].Add(new HookInfo
                    {
                        Index = finalIndex,
                        FsmName = _fsm.Fsm.Name,
                        StateName = stateName
                    });
                }
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, $"FsmInject: Cannot find state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        public static void Inject(GameObject gameObject, string fsmName, string stateName, Action hook, int index = 0)
        {
            try
            {
                PlayMakerFSM _playMaker = gameObject.GetPlayMaker(fsmName);
                
                if (_playMaker == null)
                    throw new NullReferenceException();

                _playMaker.InitializeFSM();

                FsmState _playMakerState = _playMaker.GetState(stateName);
                
                if (_playMakerState == null)
                    throw new NullReferenceException();

                AddNewHooksContainer(_playMakerState);

                int finalIndex = Insert(index, _playMaker, _playMakerState, hook);
                if (finalIndex == -1) return;

                hooksList[_playMakerState].Add(new HookInfo
                {
                    Index = finalIndex,
                    FsmName = fsmName,
                    StateName = stateName
                });
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, $"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>'{gameObject.transform.GetPath()}'</b>");
            }
        }
        
        public static void Inject(GameObject gameObject, string fsmName, string stateName, Action<PlayMakerFSM> hook, int index = 0)
        {
            try
            {
                PlayMakerFSM _playMaker = gameObject.GetPlayMaker(fsmName);
                
                if (_playMaker == null)
                    throw new NullReferenceException();

                _playMaker.InitializeFSM();
                FsmState _playMakerState = _playMaker.GetState(stateName);
                
                if (_playMakerState == null)
                    throw new NullReferenceException();

                AddNewHooksContainer(_playMakerState);

                int finalIndex = Insert(index, _playMaker, _playMakerState, hook);
                if (finalIndex == -1) return;

                hooksList[_playMakerState].Add(new HookInfo
                {
                    Index = finalIndex,
                    FsmName = fsmName,
                    StateName = stateName
                });
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, $"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>'{gameObject.transform.GetPath()}'</b>");
            }
        }

        public static void DisposeHook(GameObject gameObject, string fsmName, string stateName, int index = 0)
        {
            if (hooksList.Count == 0) return;

            try
            {
                KeyValuePair<FsmState, List<HookInfo>> _item = 
                    hooksList.FirstOrDefault(v => v.Value.Any(t => t.Index == index && t.FsmName == fsmName && t.StateName == stateName));

                if (_item.Key == null) return;
                if (_item.Value == null) return;

                FsmState _state = _item.Key;
                HookInfo _hook = _item.Value.Find(v => v.Index == index && v.FsmName == fsmName && v.StateName == stateName);

                List<FsmStateAction> _actions = new List<FsmStateAction>(_state.Actions);
                var _findedAction = _actions.Find(v => Find(v, index));
                _actions.Remove(_actions.Find(v => Find(v, index)));
                _state.Actions = _actions.ToArray();
                _state.SaveActions();

                _item.Value.Remove(_hook);
            }
            catch (Exception ex)
            {
                ModConsole.Error($"DisposeHook error go: {gameObject}; fsmName: {fsmName}; stateName: {stateName}; index: {index}");
                ModConsole.Error(ex.GetFullMessage());
            }
        }

        public static void DisposeAllHooks()
        {
            if (hooksList.Count == 0) return;
            foreach (KeyValuePair<FsmState, List<HookInfo>> _item in hooksList)
            {
                if (_item.Key?.Fsm == null) continue;
                if (_item.Value == null) continue;
                if (!_item.Key.Fsm.Initialized) continue;

                FsmState _state = _item.Key;
                if (_state.Name == "State 3" && _state.Fsm.Name == "Button") return;
                if (_state.Name == "Take photo" && _state.Fsm.Name == "Activate Dead Body") return;
                if (_item.Value.Count == 0) continue;

                List<FsmStateAction> _actions = new List<FsmStateAction>(_state.Actions);
                foreach (HookInfo _hook in _item.Value)
                {
                    if (_hook == null) continue;

                    int _index = _hook.Index;
                    _actions.Remove(_actions.Find(v => Find(v, _index)));
                }

                _state.Actions = _actions.ToArray();
                _state.SaveActions();
            }
        }

        static bool Find(FsmStateAction v, int index)
        {
            if (v == null) return false;

            if (v is FsmHookAction)
                return (v as FsmHookAction).index == index;
            if (v is FsmHookActionWithArg)
                return (v as FsmHookActionWithArg).index == index;

            return false;
        }


        static void AddNewHooksContainer(FsmState state)
        {
            if (state == null) return;

            if (!hooksList.ContainsKey(state))
                hooksList[state] = new List<HookInfo>();
        }

        static int Insert(int index, PlayMakerFSM fsm, FsmState state, Action hook)
        {
            if (fsm == null || state == null || hook == null) return -1;

            FsmHookAction _item = new FsmHookAction
            {
                index = index,
                state = state.Name,
                hook = hook,
                component = fsm,
                method = Utils.GetMethodPath(hook.Method)
            };

            return InsertItemWithIndex(index, state, ref _item);
        }

        static int Insert(int index, PlayMakerFSM fsm, FsmState state, Action<PlayMakerFSM> hook)
        {
            if (fsm == null || state == null || hook == null) return -1;

            FsmHookActionWithArg _item = new FsmHookActionWithArg
            {
                index = index,
                state = state.Name,
                hook = hook,
                component = fsm,
                method = Utils.GetMethodPath(hook.Method)
            };

            return InsertItemWithIndex(index, state, ref _item);
        }

        static int InsertItemWithIndex<T>(int index, FsmState state, ref T item) where T : FsmStateAction
        {
            List<FsmStateAction> _actions = new List<FsmStateAction>(state.Actions);

            if (index > _actions.Count - 1 || index < 0)
            {
                _actions.Add(item);
                index = _actions.Count - 1;
            }
            else _actions.Insert(index, item);

            state.Actions = _actions.ToArray();
            state.SaveActions();

            return index;
        }
    }
}
