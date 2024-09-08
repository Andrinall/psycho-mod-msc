using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class GameHook
    {
        private class FsmHookAction : FsmStateAction
        {
            public Action hook;

            public override void OnEnter()
            {
                hook();
                this.Finish();
            }
        }

        internal static bool InjectStateHook(GameObject gameObject, string stateName, Action hook, bool placeLast = false)
        {
            FsmState stateFromGameObject = GetStateFromGameObject(gameObject, stateName);
            if (stateFromGameObject == null) return false;

            return InjectStateHook_internal(stateFromGameObject, hook, placeLast);
        }

        internal static bool InjectStateHook(GameObject gameObject, string FsmName, string stateName, Action hook, bool placeLast = false)
        {
            FsmState stateFromGameObject = GetStateFromGameObject(gameObject, FsmName, stateName);
            if (stateFromGameObject == null) return false;

            return InjectStateHook_internal(stateFromGameObject, hook, placeLast);
        }

        private static bool InjectStateHook_internal(FsmState stateFromGameObject, Action hook, bool placeLast = false)
        {
            List<FsmStateAction> list = new List<FsmStateAction>(stateFromGameObject.Actions);
            FsmHookAction fsmHookAction = new FsmHookAction();
            fsmHookAction.hook = hook;

            if (placeLast)
                list.Add(fsmHookAction);
            else
                list.Insert(0, fsmHookAction);

            stateFromGameObject.Actions = list.ToArray();

            return true;
        }

        private static FsmState GetStateFromGameObject(GameObject obj, string stateName)
        {
            PlayMakerFSM[] components = obj.GetComponents<PlayMakerFSM>();
            for (int i = 0; i < components.Length; i++)
            {
                FsmState val = components[i].FsmStates.FirstOrDefault(x => x.Name == stateName);
                if (val != null) return val;
            }
            return null;
        }

        private static FsmState GetStateFromGameObject(GameObject obj, string FsmName, string stateName)
        {
            PlayMakerFSM component = obj.GetComponents<PlayMakerFSM>().FirstOrDefault(v => v.FsmName == FsmName);
            FsmState val = component.GetState(stateName);
            if (val != null) return val;
            return null;
        }
    }
}
