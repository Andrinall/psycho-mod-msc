using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    public sealed class StateHook
    {
        public static void Inject(GameObject gameObject, string stateName, Action hook)
        {
            try
            {
                PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
                if (components.Length == 0) return;

                foreach (PlayMakerFSM fsm in components)
                {
                    if (fsm.GetState(stateName) == null) continue;
                    gameObject.FsmInject(fsm.Fsm.Name, stateName, hook);
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
                if (playMaker == null) throw new NullReferenceException();

                playMaker.Fsm.InitData();
                FsmState playMakerState = gameObject.GetPlayMakerState(stateName);
                if (playMakerState == null) throw new NullReferenceException();

                var list = new List<FsmStateAction>(playMakerState.Actions);
                FsmHookAction item = new FsmHookAction
                {
                    hook = hook,
                    state = stateName,
                    method = Utils.GetMethodPath(hook.Method),
                    index = index,
                    component = playMaker
                };

                InsertItemWithIndex(index, ref list, ref item);
                playMakerState.Actions = list.ToArray();
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
                if (playMaker == null) throw new NullReferenceException();

                playMaker.Fsm.InitData();
                FsmState playMakerState = gameObject.GetPlayMakerState(stateName);
                if (playMakerState == null) throw new NullReferenceException();

                var list = new List<FsmStateAction>(playMakerState.Actions);
                FsmHookActionWithArg item = new FsmHookActionWithArg
                {
                    hook = hook,
                    state = stateName,
                    method = Utils.GetMethodPath(hook.Method),
                    index = index,
                    component = playMaker
                };

                InsertItemWithIndex(index, ref list, ref item);
                playMakerState.Actions = list.ToArray();
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>'{gameObject.transform.GetPath()}'</b>");
            }
        }

        static void InsertItemWithIndex<T>(int index, ref List<FsmStateAction> list, ref T item) where T : FsmStateAction
        {
            if (index > list.Count - 1 || index == -1)
                list.Add(item);
            else if (index < 0)
                list.Insert(0, item);
            else
                list.Insert(index, item);
        }
    }
}
