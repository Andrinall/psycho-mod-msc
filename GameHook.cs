using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
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

        public static void InjectStateHook(GameObject gameObject, string stateName, Action hook)
        {
            FsmState stateFromGameObject = GetStateFromGameObject(gameObject, stateName);
            if (stateFromGameObject != null)
            {
                List<FsmStateAction> list = new List<FsmStateAction>(stateFromGameObject.Actions);
                FsmHookAction fsmHookAction = new FsmHookAction();
                fsmHookAction.hook = hook;
                list.Insert(0, fsmHookAction);
                stateFromGameObject.Actions = list.ToArray();
            }
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
    }
}
