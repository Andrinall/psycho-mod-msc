
using System;

using MSCLoader;
using UnityEngine;

using Psycho.Internal;


namespace Psycho
{
    static class TransformExtensions
    {
        public static bool IsPrefab(this Transform obj)
        {
            if (obj.gameObject.activeInHierarchy && !obj.gameObject.activeSelf) return false;
            return obj.root == obj;
        }

        public static void IterateAllChilds(this Transform obj, Action<Transform> handler)
        {
            if (obj.childCount == 0) return;
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform _child = obj.GetChild(i);
                try
                {
                    handler?.Invoke(_child);
                }
                catch (Exception ex)
                {
                    Utils.PrintDebug($"Error in IterateAllChilds handler delegate ({handler.Method.DeclaringType.Name}");
                    ModConsole.Error(ex.GetFullMessage());
                }

                if (_child.childCount == 0) continue;
                _child.IterateAllChilds(handler);
            }
        }

        public static bool MoveTowards(this Transform obj, Vector3 targetPoint, float targetDistance, float maxSpeed)
        {
            if (_checkDistance(obj.position, targetPoint, targetDistance)) return true;
            obj.position = Vector3.MoveTowards(obj.position, targetPoint, maxSpeed * Time.deltaTime);
            return false;
        }

        public static bool MoveTowards(this Transform obj, Transform targetPoint, float targetDistance, float maxSpeed)
        {
            if (_checkDistance(obj.position, targetPoint.position, targetDistance)) return true;
            obj.position = Vector3.MoveTowards(obj.position, targetPoint.position, maxSpeed * Time.deltaTime);
            return false;
        }

        public static string GetPath(this Transform obj)
        {
            if (obj.parent == null)
                return obj.name;
            return obj.parent.GetPath() + "/" + obj.name;
        }

        static bool _checkDistance(Vector3 p1, Vector3 p2, float target)
            => (p1 - p2).magnitude < target;
    }
}
