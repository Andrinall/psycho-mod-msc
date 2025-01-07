
using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho
{
    public static class PlayMakerExtensions
    {
        public static FsmEvent AddEvent(this PlayMakerFSM fsm, string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return null;
            InitializeFSM(fsm.Fsm);

            FsmEvent _event = new FsmEvent(eventName);
            fsm.Fsm.Events = new List<FsmEvent>(fsm.Fsm.Events) { _event }.ToArray();
            return _event;
        }

        public static void CallGlobalTransition(this PlayMakerFSM fsm, string eventName)
        {
            fsm.enabled = true;
            fsm.Fsm.Event(fsm.GetGlobalTransition(eventName)?.FsmEvent);
        }

        public static void ClearFsmActions(this Transform obj, string fsm, string state, int index = -1, int count = -1)
        {
            if (string.IsNullOrEmpty(fsm)) return;
            if (string.IsNullOrEmpty(state)) return;

            try
            {
                PlayMakerFSM _fsm = obj?.GetPlayMaker(fsm);
                InitializeFSM(_fsm.Fsm);

                _fsm?.GetState(state)?.ClearActions(index, count);
            }
            catch (Exception ex)
            {
                ModConsole.Error("Clearing fsm actions failed;");
                ModConsole.Error(ex.GetFullMessage());
            }
        }

        public static void ClearActions(this FsmState state, int index = -1, int count = -1)
        {
            try
            {
                InitializeFSM(state.Fsm);

                List<FsmStateAction> _list = state.Actions.ToList();
                if (index == -1 && count == -1)
                    _list.Clear();
                else if (index != -1 && count == -1)
                    _list.RemoveRange(index, 1);
                else
                    _list.RemoveRange(index, count);

                state.Actions = _list.ToArray();
                state.SaveActions();
            }
            catch (Exception e)
            {
                ModConsole.Error("Failed to clears actions;");
                ModConsole.Error(e.GetFullMessage());
            }
        }

        static void InitializeFSM(Fsm fsm)
        {
            if (!fsm.Initialized) return;
            fsm.InitData();
        }
    }
}
