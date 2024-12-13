using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    internal sealed class StateHook : FsmHook
    {
        internal sealed class FsmHookAction : FsmStateAction
        {
            public Action<PlayMakerFSM> hook;
            public PlayMakerFSM component;
            public string path;


            public override void OnEnter()
            {
                try
                {
                    hook?.Invoke(component);
                }
                catch (Exception e)
                {
                    ModConsole.Error($"Error in StateHook action delegate: {path}");
                    ModConsole.Error(e.GetFullMessage());
                }

                base.Finish();
            }
        }

        public static void Inject(GameObject gameObject, string stateName, Action hook)
        {
            try
            {
                FsmInject(gameObject, stateName, hook);
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        public static void Inject(GameObject gameObject, string fsmName, string stateName, Action<PlayMakerFSM> hook)
        {
            try
            {
                PlayMakerFSM playMaker = gameObject.GetPlayMaker(fsmName);
                if (playMaker == null) return;

                playMaker.Fsm.InitData();
                FsmState playMakerState = gameObject.GetPlayMakerState(stateName);
                if (playMakerState == null) return;

                var list = new List<FsmStateAction>(playMakerState.Actions);
                FsmHookAction item = new FsmHookAction {
                    hook = hook,
                    path = $"obj `{gameObject.name}` | fsm `{fsmName}` | state `{stateName}`",
                    component = playMaker
                };
                list.Insert(0, item);
                playMakerState.Actions = list.ToArray();
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        public static void Inject(GameObject gameObject, string fsmName, string stateName, int index, Action<PlayMakerFSM> hook)
        {
            try
            {
                PlayMakerFSM playMaker = gameObject.GetPlayMaker(fsmName);
                if (playMaker == null) throw new NullReferenceException();

                playMaker.Fsm.InitData();
                FsmState playMakerState = gameObject.GetPlayMakerState(stateName);
                if (playMakerState == null) throw new NullReferenceException();

                var list = new List<FsmStateAction>(playMakerState.Actions);
                FsmHookAction item = new FsmHookAction {
                    hook = hook,
                    path = $"obj `{gameObject.name}` | fsm `{fsmName}` | state `{stateName}` [{index}]",
                    component = playMaker
                };

                InsertItemWithIndex(index, ref list, ref item);
                playMakerState.Actions = list.ToArray();
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        static void InsertItemWithIndex(int index, ref List<FsmStateAction> list, ref FsmHookAction item)
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
