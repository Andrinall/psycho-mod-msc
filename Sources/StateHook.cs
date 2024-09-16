using System;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class StateHook : FsmHook
    {
        private class FsmHookAction : FsmStateAction
        {
            public Action hook;

            public override void OnEnter()
            {
                hook?.Invoke();
                base.Finish();
            }
        }

        internal static void Inject(GameObject gameObject, string stateName, Action hook)
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

        internal static void Inject(GameObject gameObject, string stateName, int index, Action hook)
        {
            try
            {
                FsmState playMakerState = gameObject.GetPlayMakerState(stateName);
                if (playMakerState == null) throw new NullReferenceException();

                List<FsmStateAction> list = new List<FsmStateAction>(playMakerState.Actions);
                FsmHookAction item = new FsmHookAction { hook = hook };
                InsertItemWithIndex(index, ref list, ref item);
                playMakerState.Actions = list.ToArray();
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        internal static void Inject(GameObject gameObject, string fsmName, string stateName, Action hook)
        {
            try
            {
                PlayMakerFSM playMaker = gameObject.GetPlayMaker(fsmName);
                if (playMaker == null)
                    return;
                    //throw new NullReferenceException($"Doesn't find FSM {fsmName} in object {gameObject.name}");

                FsmState playMakerState = gameObject.GetPlayMakerState(stateName);
                if (playMakerState == null)
                    return;
                    //throw new NullReferenceException($"Doesn't find state {stateName} in FSM {fsmName} in object {gameObject.name}");

                List<FsmStateAction> list = new List<FsmStateAction>(playMakerState.Actions);
                FsmHookAction item = new FsmHookAction { hook = hook };
                list.Insert(0, item);
                playMakerState.Actions = list.ToArray();
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        internal static void Inject(GameObject gameObject, string fsmName, string stateName, int index, Action hook)
        {
            try
            {
                PlayMakerFSM playMaker = gameObject.GetPlayMaker(fsmName);
                if (playMaker == null) throw new NullReferenceException();

                FsmState playMakerState = gameObject.GetPlayMakerState(stateName);
                if (playMakerState == null) throw new NullReferenceException();

                List<FsmStateAction> list = new List<FsmStateAction>(playMakerState.Actions);
                FsmHookAction item = new FsmHookAction { hook = hook };

                InsertItemWithIndex(index, ref list, ref item);
                playMakerState.Actions = list.ToArray();
            }
            catch
            {
                ModConsole.Error($"FsmInject: Cannot find FSM <b>{fsmName}</b>, state <b>{stateName}</b> in GameObject <b>{gameObject.name}</b>");
            }
        }

        private static void InsertItemWithIndex(int index, ref List<FsmStateAction> list, ref FsmHookAction item)
        {
            if (index > list.Count - 1)
                list.Add(item);
            else if (index == -1)
                list.Add(item);
            else if (index < 0)
                list.Insert(0, item);
            else
                list.Insert(index, item);
        }
    }
}
