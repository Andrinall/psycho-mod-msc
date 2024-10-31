using System;
using System.Collections.Generic;

using MSCLoader;
using System.Linq;

using UnityEngine;
using HutongGames.PlayMaker;

namespace Psycho.Extensions
{
    public static class Extensions
    {
        public static void ClearActions(this Transform obj, string fsm, string state, int index = -1, int count = -1)
        {
            if (string.IsNullOrEmpty(fsm)) return;
            if (string.IsNullOrEmpty(state)) return;

            try
            {
                obj?.GetPlayMaker(fsm)?.GetState(state)?.ClearActions(index, count);
            }
            catch (Exception e)
            {
                ModConsole.Error($"[1] Failed to clears actions;\n{e.GetFullMessage()}");
            }
        }

        public static void ClearActions(this FsmState state, int index = -1, int count = -1)
        {
            try
            {
                var list = state.Actions.ToList();
                if (index == -1 && count == -1)
                    list.Clear();
                else if (index != -1 && count == -1)
                    list.RemoveRange(index, 1);
                else
                    list.RemoveRange(index, count);

                state.Actions = list.ToArray();
                state.SaveActions();
            }
            catch (Exception e)
            {
                ModConsole.Error($"[2] Failed to clears actions;\n{e.GetFullMessage()}");
            }
        }

        public static void AddEvent(this PlayMakerFSM fsm, string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            fsm.Fsm.Events = new List<FsmEvent>(fsm.Fsm.Events)
                { new FsmEvent(eventName) }.ToArray();
        }

        public static void CallGlobalTransition(this PlayMakerFSM fsm, string eventName)
        {
            fsm.enabled = true;
            fsm.Fsm.Event(fsm.GetGlobalTransition(eventName).FsmEvent);
        }

        public static bool IsPrefab(this Transform tempTrans)
        {
            if (tempTrans.gameObject.activeInHierarchy && !tempTrans.gameObject.activeSelf) return false;
            return tempTrans.root == tempTrans;
        }

        public static void IterateAllChilds(this Transform obj, Action<Transform> handler)
        {
            if (obj.childCount == 0) return;
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                handler?.Invoke(child);

                if (child.childCount == 0) continue;
                child.IterateAllChilds(handler);
            }
        }

        public static bool MoveTowards(this Transform transform, Vector3 targetPoint, float targetDistance, float maxSpeed)
        {
            if (_checkDistance(transform.position, targetPoint, targetDistance)) return true;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, maxSpeed * Time.deltaTime);
            return false;
        }

        public static bool MoveTowards(this Transform transform, Transform targetPoint, float targetDistance, float maxSpeed)
        {
            if (_checkDistance(transform.position, targetPoint.position, targetDistance)) return true;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, maxSpeed * Time.deltaTime);
            return false;
        }

        static bool _checkDistance(Vector3 p1, Vector3 p2, float target)
            => (p1 - p2).magnitude < target;
    }
}
