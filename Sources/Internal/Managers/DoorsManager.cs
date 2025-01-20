
using System;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Psycho.Internal
{
    enum DOOR : int
    {
        SAUNA, BATHROOM, MIDDLEROOM,
        REAR, PANTRY, FRONT,
        WC, BEDROOM1, BEDROOM2
    }

    static class DoorsManager
    {
        static string[] houseDoorsPaths = new string[]
        {
            "SAUNA/DoorSauna",
            "BATHROOM/DoorBathroom",
            "MIDDLEROOM/DoorMiddleroom",
            "MIDDLEROOM/DoorRear",
            "KITCHEN/DoorPantry",
            "LIVINGROOM/DoorFront",
            "LIVINGROOM/DoorWC",
            "BEDROOM1/DoorBedroom1",
            "BEDROOM2/DoorBedroom2",
        };

        public static void AddDoorOpenCallback(string path, Action callback)
            => StateHook.Inject(GameObject.Find(path).transform.Find("Pivot/Handle").gameObject, "Use", "Open door", callback);

        public static bool CloseHouseDoor(DOOR door)
        {
            string fullPath = $"YARD/Building/${houseDoorsPaths[(int)door]}/Pivot/Handle";
            PlayMakerFSM _door = GameObject.Find(fullPath)?.GetPlayMaker("Use");
            if (_door == null)
                return false;

            return SetDoorStatus(_door, false);
        }

        public static bool OpenHouseDoor(DOOR door)
        {
            string fullPath = $"YARD/Building/${houseDoorsPaths[(int)door]}/Pivot/Handle";
            PlayMakerFSM _door = GameObject.Find(fullPath)?.GetPlayMaker("Use");
            if (_door == null)
                return false;

            return SetDoorStatus(_door, true);
        }

        public static bool CloseAllDoorsInHouse()
        {
            Transform _house = GameObject.Find("YARD/Building").transform;
            if (_house == null)
                return false;

            foreach (string path in houseDoorsPaths)
            {
                PlayMakerFSM _door = _house.Find($"{path}/Pivot/Handle")?.GetPlayMaker("Use");
                if (_door == null)
                    return false;

                if (!SetDoorStatus(_door, false))
                    return false;
            }

            return true;
        }

        static bool SetDoorStatus(PlayMakerFSM door, bool status)
        {
            FsmBool _open = door.GetVariable<FsmBool>("DoorOpen");
            if (_open == null)
                return false;

            _open.Value = !status;
            door.SendEvent("GLOBALEVENT");

            return true;
        }
    }
}
