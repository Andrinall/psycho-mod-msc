
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Net;


namespace Psycho.Internal
{
    internal sealed class HookInfo
    {
        public int Index = -1;
        public string FsmName = string.Empty;
        public string StateName = string.Empty;
    }


    public sealed class StateHook
    {
        internal static Dictionary<FsmState, List<HookInfo>> hooksList = new Dictionary<FsmState, List<HookInfo>>();

        public static void Inject(GameObject gameObject, string stateName, Action hook)
        {
            try
            {
                PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
                if (components.Length == 0) return;

                foreach (PlayMakerFSM fsm in components)
                {
                    if (fsm == null) continue;
                    fsm.InitializeFSM();

                    FsmState _state = fsm.GetState(stateName);
                    if (_state == null) continue;

                    if (!hooksList.ContainsKey(_state))
                        hooksList[_state] = new List<HookInfo>();

                    hooksList[_state].Add(new HookInfo
                    {
                        Index = Insert(0, fsm, _state, hook),
                        FsmName = fsm.Fsm.Name,
                        StateName = stateName
                    });
                }
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        public static void Inject(GameObject gameObject, string fsmName, string stateName, Action hook, int index = 0)
        {
            try
            {
                PlayMakerFSM playMaker = gameObject.GetPlayMaker(fsmName);
                
                if (playMaker == null)
                    throw new NullReferenceException();

                playMaker.InitializeFSM();

                FsmState playMakerState = playMaker.GetState(stateName);
                
                if (playMakerState == null)
                    throw new NullReferenceException();

                if (!hooksList.ContainsKey(playMakerState))
                    hooksList[playMakerState] = new List<HookInfo>();

                hooksList[playMakerState].Add(new HookInfo
                {
                    Index = Insert(index, playMaker, playMakerState, hook),
                    FsmName = fsmName,
                    StateName = stateName
                });
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>'{gameObject.transform.GetPath()}'</b>");
            }
        }
        
        public static void Inject(GameObject gameObject, string fsmName, string stateName, Action<PlayMakerFSM> hook, int index = 0)
        {
            try
            {
                PlayMakerFSM playMaker = gameObject.GetPlayMaker(fsmName);
                
                if (playMaker == null)
                    throw new NullReferenceException();

                playMaker.InitializeFSM();
                FsmState playMakerState = playMaker.GetState(stateName);
                
                if (playMakerState == null)
                    throw new NullReferenceException();

                if (!hooksList.ContainsKey(playMakerState))
                    hooksList[playMakerState] = new List<HookInfo>();

                hooksList[playMakerState].Add(new HookInfo
                {
                    Index = Insert(index, playMaker, playMakerState, hook),
                    FsmName = fsmName,
                    StateName = stateName
                });
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>'{gameObject.transform.GetPath()}'</b>");
            }
        }

        public static void DisposeHook(GameObject gameObject, string fsmName, string stateName, int index = 0)
        {
            if (hooksList.Count == 0) return;

            try
            {
                KeyValuePair<FsmState, List<HookInfo>> item = 
                    hooksList.FirstOrDefault(v => v.Value.Any(t => t.Index == index && t.FsmName == fsmName && t.StateName == stateName));

                if (item.Key == null) return;
                if (item.Value == null) return;

                FsmState _state = item.Key;
                HookInfo _hook = item.Value.Find(v => v.Index == index && v.FsmName == fsmName && v.StateName == stateName);

                List<FsmStateAction> _actions = new List<FsmStateAction>(_state.Actions);
                var _item = _actions.Find(v => Find(v, index));
                _actions.Remove(_actions.Find(v => Find(v, index)));
                _state.Actions = _actions.ToArray();
                _state.SaveActions();

                item.Value.Remove(_hook);
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, $"DisposeHook go: {gameObject}; fsmName: {fsmName}; stateName: {stateName}; index: {index}");
            }
        }

        internal static void DisposeAllHooks()
        {
            if (hooksList.Count == 0) return;
            foreach (KeyValuePair<FsmState, List<HookInfo>> item in hooksList)
            {
                if (item.Key == null) continue;
                if (item.Value == null) continue;
                if (!item.Key.Fsm.Initialized) continue;

                FsmState _state = item.Key;
                if (item.Value.Count == 0) continue;

                List<FsmStateAction> _actions = new List<FsmStateAction>(_state.Actions);
                foreach (HookInfo hook in item.Value)
                {
                    if (hook == null) continue;

                    int index = hook.Index;
                    _actions.Remove(_actions.Find(v => Find(v, index)));
                }

                _state.Actions = _actions.ToArray();
                _state.SaveActions();
            }
        }

        static bool Find(FsmStateAction v, int index)
        {
            if (v is FsmHookAction)
                return (v as FsmHookAction).index == index;
            if (v is FsmHookActionWithArg)
                return (v as FsmHookActionWithArg).index == index;

            return false;
        }


    static void AddNewHooksContainer(FsmState state)
        {
            if (!hooksList.ContainsKey(state))
                hooksList[state] = new List<HookInfo>();
        }

        static int Insert(int index, PlayMakerFSM fsm, FsmState state, Action hook)
        {
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
