
using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    internal static class Electricity
    {
        public static float GetCutoffTimer()
        {
            GameObject _bills = GameObject.Find("Systems/ElectricityBills");
            PlayMakerFSM _data = _bills?.GetComponent<PlayMakerFSM>();
            FsmFloat _waitCutoff = _data?.GetVariable<FsmFloat>("WaitCutoff");

            return _waitCutoff?.Value ?? 0f;
        }

        public static bool GetSwitchState()
        {
            return GameObject.Find("Systems/ElectricityBills")
                ?.GetComponent<PlayMakerFSM>()
                ?.GetVariable<FsmBool>("MainSwitch")
                ?.Value ?? false;
        }

        public static void SetSwitchState(bool state)
        {
            Transform _fuseTable = GameObject.Find("YARD/Building/Dynamics/FuseTable")?.transform;
            if (_fuseTable == null) return;
            _fuseTable.Find("ElectricShockPoint")?.gameObject?.SetActive(state);

            Transform _mainSwitch = _fuseTable.Find("Fusetable/MainSwitch");
            if (_mainSwitch == null) return;

            // states Wait Player -> Wait Button -> Switch
            FsmBool _switchState = _mainSwitch?.GetComponent<PlayMakerFSM>()?.GetVariable<FsmBool>("Switch");
            if (_switchState == null) return;
            _switchState.Value = state;

            // states Position -> OFF
            FsmBool _elecSwitch = GameObject.Find("Systems/ElectricityBills")
                ?.GetComponent<PlayMakerFSM>()
                ?.GetVariable<FsmBool>("MainSwitch");

            if (_elecSwitch == null) return;
            _elecSwitch.Value = state;

            Transform _switchPivot = _mainSwitch.Find("Pivot");
            if (_switchPivot == null) return;
            _switchPivot.localEulerAngles = new Vector3(state ? 335f : 25f, 0);
        }
    }
}
