
using System;

using MSCLoader;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    class FsmHookActionWithArg : FsmStateAction
    {
        public Action<PlayMakerFSM> hook;
        public PlayMakerFSM component;
        public string method;
        public string state;
        public int index;


        public override void OnEnter()
        {
            try
            {
                hook?.Invoke(component);
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED,
$@"Error in StateHook action delegate!
[{component.transform.GetPath()}]
[fsm {component.Fsm.Name}][state {state}][index {index}]
[method {method}]"
                );
                ModConsole.Error(e.GetFullMessage());
            }

            base.Finish();
        }
    }

    class FsmHookAction : FsmStateAction
    {
        public Action hook;
        public PlayMakerFSM component;
        public string method;
        public string state;
        public int index;


        public override void OnEnter()
        {
            try
            {
                hook?.Invoke();
            }
            catch (Exception e)
            {
                Utils.PrintDebug(eConsoleColors.RED,
$@"Error in StateHook action delegate!
[{component.transform.GetPath()}]
[fsm {component.Fsm.Name}][state {state}][index {index}]
[method {method}]"
                );
                ModConsole.Error(e.GetFullMessage());
            }

            base.Finish();
        }
    }
}
