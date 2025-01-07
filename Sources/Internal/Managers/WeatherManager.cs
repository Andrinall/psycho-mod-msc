
using System;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;


namespace Psycho.Internal
{
    internal static class WeatherManager
    {
        public static void StopCloudsOrRandomize()
        {
            try
            {
                bool _state = !Logic.InHorror;
                GameObject _clouds = GameObject.Find("MAP/CloudSystem/Clouds");
                PlayMakerFSM _cloudsFsm = _clouds.GetPlayMaker("Weather");
                FsmState _cloudsMove = _cloudsFsm.GetState("Move clouds");

                FloatAdd _action1 = (_cloudsMove.Actions[1] as FloatAdd);
                SetPosition _action2 = (_cloudsMove.Actions[2] as SetPosition);

                _action1.everyFrame = _state;
                _action1.perSecond = _state;

                _action2.everyFrame = _state;
                _action2.lateUpdate = _state;

                if (!_state)
                    _clouds.transform.position = Vector3.zero;
                else
                    _cloudsFsm.SendEvent("RANDOMIZE");
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to change clouds after moving between worlds;\n{e.GetFullMessage()}");
            }
        }
    }
}
