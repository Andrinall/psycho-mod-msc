using System;
using System.Linq;
using System.Collections.Generic;

using MSCLoader;
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

        public static string GetPath(this Transform current)
        {
            if (current.parent == null)
                return current.name;
            return current.parent.GetPath() + "/" + current.name;
        }


        public static FsmEvent AddEvent(this PlayMakerFSM fsm, string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return null;

            FsmEvent event_ = new FsmEvent(eventName);
            fsm.Fsm.Events = new List<FsmEvent>(fsm.Fsm.Events){event_}.ToArray();
            return event_;
        }

        public static void CallGlobalTransition(this PlayMakerFSM fsm, string eventName)
        {
            fsm.enabled = true;
            fsm.Fsm.Event(fsm.GetGlobalTransition(eventName).FsmEvent);
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

        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            float itemWeightIndex = (float)new System.Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }
        
        public static Vector3 Clamp(this Vector3 self, float min, float max)
            => new Vector3(
                Mathf.Clamp(self.x, min, max),
                Mathf.Clamp(self.y, min, max),
                Mathf.Clamp(self.z, min, max)
            );

        public static Vector3 Clamp(this Vector3 self, Vector3 min, Vector3 max)
            => new Vector3(
                Mathf.Clamp(self.x, min.x, max.x),
                Mathf.Clamp(self.y, min.y, max.y),
                Mathf.Clamp(self.z, min.z, max.z)
            );

        public static void CopyBytes(this Vector3 self, ref byte[] array, ref int offset)
        {
            BitConverter.GetBytes(self.x).CopyTo(array, offset);
            BitConverter.GetBytes(self.y).CopyTo(array, offset + 4);
            BitConverter.GetBytes(self.z).CopyTo(array, offset + 8);
            offset += 12;
        }

        public static Vector3 GetFromBytes(this Vector3 self, byte[] array, ref int offset)
        {
            float x = BitConverter.ToSingle(array, offset);
            float y = BitConverter.ToSingle(array, offset + 4);
            float z = BitConverter.ToSingle(array, offset + 8);
            offset += 12;
            return new Vector3(x, y, z);
        }

        public static void CopyBytes(this string self, ref byte[] array, ref int offset)
        {
            int len = self.Length;
            BitConverter.GetBytes(len).CopyTo(array, offset);
            offset += 4;
            
            char[] chars = self.ToCharArray();
            foreach (char itc in chars)
            {
                BitConverter.GetBytes(itc).CopyTo(array, offset);
                offset += 2;
            }
        }

        public static string GetFromBytes(this string self, byte[] array, ref int offset)
        {
            int len = BitConverter.ToInt32(array, offset);
            char[] chars = new char[len];

            offset += 4;

            for (int i = 0; i < len; i++)
            {
                chars[i] = BitConverter.ToChar(array, offset);
                offset += 2;
            }

            return new string(chars);
        }

        static bool _checkDistance(Vector3 p1, Vector3 p2, float target)
            => (p1 - p2).magnitude < target;

    }
}
