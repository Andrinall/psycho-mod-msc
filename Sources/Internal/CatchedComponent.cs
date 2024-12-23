using System;

using MSCLoader;
using UnityEngine;


namespace Psycho.Internal
{
    public class CatchedComponent : MonoBehaviour
    {
        void Awake() => _callVirtual(Awaked);
        void OnEnable() => _callVirtual(Enabled);
        void OnDisable() => _callVirtual(Disabled);
        void OnDestroy() => _callVirtual(Destroyed);
        void Update() => _callVirtual(OnUpdate);
        void FixedUpdate() => _callVirtual(OnFixedUpdate);


        protected virtual void Awaked() {}
        protected virtual void Enabled() {}
        protected virtual void Disabled() {}
        protected virtual void Destroyed() {}
        protected virtual void OnUpdate() {}
        protected virtual void OnFixedUpdate() {}


        void _callVirtual(Action method)
        {
            try
            {
                method?.Invoke();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Exception in {transform.GetPath()} [ {Utils.GetMethodPath(method.Method)}() ]");
                ModConsole.Error($"{ex.GetFullMessage()}\n{ex.StackTrace}");
            }
        }
    }
}
