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


        public virtual void Awaked() {}
        public virtual void Enabled() {}
        public virtual void Disabled() {}
        public virtual void Destroyed() {}
        public virtual void OnUpdate() {}
        public virtual void OnFixedUpdate() {}


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
