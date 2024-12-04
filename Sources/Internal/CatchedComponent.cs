using System;

using MSCLoader;
using UnityEngine;

using Psycho.Extensions;


namespace Psycho.Internal
{
    internal class CatchedComponent : MonoBehaviour
    {
        internal virtual bool tested => false;

        void Awake() => _callVirtual(Awaked, "Awake");
        void OnEnable() => _callVirtual(Enabled, "OnEnable");
        void OnDisable() => _callVirtual(Disabled, "OnDisable");
        void OnDestroy() => _callVirtual(Destroyed, "OnDestroy");
        void Update() => _callVirtual(OnUpdate, "Update");
        void FixedUpdate() => _callVirtual(OnFixedUpdate, "FixedUpdate");


        internal virtual void Awaked() {}
        internal virtual void Enabled() {}
        internal virtual void Disabled() {}
        internal virtual void Destroyed() {}
        internal virtual void OnUpdate() {}
        internal virtual void OnFixedUpdate() {}


        void _callVirtual(Action method, string methodName)
        {
            try
            {
                method?.Invoke();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Exception in {transform.GetPath()}::{GetType()?.Name}::{methodName}();");
                ModConsole.Error($"{ex.GetFullMessage()}\n{ex.StackTrace}");
            }
        }
    }
}
