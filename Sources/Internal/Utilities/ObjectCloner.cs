
using MSCLoader;
using UnityEngine;

using Psycho.Screamers;
using System.Linq;


namespace Psycho.Internal
{
    static class ObjectCloner
    {
        static GameObject ClonedGrannyHiker;
        public static AnimationClip PigWalkAnimation;
        public static GameObject ClonedPhantom;

        public static void CopyGrannyHiker()
        {
            GameObject _hiker = GameObject.Find("ChurchGrandma/GrannyHiker");
            ClonedGrannyHiker = GameObject.Instantiate(_hiker);

            Transform _char = ClonedGrannyHiker.transform.Find("Char");
            Transform _head = _char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot");

            Object.Destroy(ClonedGrannyHiker.transform.GetPlayMaker("Logic"));
            Object.Destroy(ClonedGrannyHiker.transform.Find("Ray").gameObject);
            Object.Destroy(ClonedGrannyHiker.transform.Find("RagDoll2").gameObject);
            Object.Destroy(_head.GetPlayMaker("Look"));
            Object.Destroy(_char.Find("HeadTarget/LookAt").GetPlayMaker("Random"));
            Object.Destroy(_char.Find("RagDollCar").gameObject);
            Object.Destroy(_char.Find("HeadTarget").gameObject);
            Object.Destroy(_char.Find("HumanTriggerCrime").gameObject);
            _char.gameObject.SetActive(false);

            ClonedGrannyHiker.transform.parent = null;
            ClonedGrannyHiker.name = "GrannyScreamHiker";

            Animation _animation = _char.Find("skeleton").GetComponent<Animation>();
            if (!_animation.GetClip("venttipig_pig_walk"))
                _animation.AddClip(PigWalkAnimation, "venttipig_pig_walk");

            _animation.clip = _animation.GetClip("venttipig_pig_walk");
            _animation.playAutomatically = true;
            _animation.Play("venttipig_pig_walk", PlayMode.StopAll);

            if (ClonedGrannyHiker.GetComponent<MummolaCrawl>() == null)
                ClonedGrannyHiker.AddComponent<MummolaCrawl>();

            ClonedGrannyHiker.gameObject.SetActive(true);
        }

        public static void CopyScreamHand()
        {
            GameObject _handMilk = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Drink/Hand/HandMilk");
            Transform _bedroom = GameObject.Find("YARD/Building/BEDROOM1").transform;
            Transform _screamHand = Object.Instantiate(_handMilk).transform;
            Object.Destroy(_screamHand.Find("Milk").gameObject);

            _screamHand.SetParent(_bedroom, worldPositionStays: false);
            _screamHand.gameObject.name = "ScreamHand";

            _screamHand.Find("Armature").gameObject.SetActive(false);
            _screamHand.Find("hand_rigged").gameObject.SetActive(false);

            _screamHand.IterateAllChilds(v => v.gameObject.layer = 0);

            _screamHand.Find("Armature").localEulerAngles = new Vector3(-90, 0, 0);
            _screamHand.position = new Vector3(-12.00460433959961f, 1.1982498168945313f, 15.551212310791016f);
            _screamHand.eulerAngles = new Vector3(0.4349295198917389f, 0.05798640847206116f, 0.02807953953742981f);
            _screamHand.localScale = new Vector3(2f, 2f, 2f);

            if (_screamHand.gameObject.GetComponent<MovingHand>() == null)
                _screamHand.gameObject.AddComponent<MovingHand>();

            _screamHand.gameObject.SetActive(true);
        }

        public static void CopyUncleChar()
        {
            GameObject _uncleOrig = GameObject.Find("YARD/UNCLE/UncleWalking/Uncle");
            Transform _uncleClone = GameObject.Instantiate(_uncleOrig).transform;

            Transform _char = _uncleClone.Find("Char");
            Object.Destroy(_uncleClone.Find("Ray").gameObject);
            Object.Destroy(_char.Find("OriginalPos").gameObject);
            Object.Destroy(_char.Find("LookTarget").gameObject);
            Object.Destroy(_char.Find("HumanCollider").gameObject);
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot").GetComponent<PlayMakerFSM>());
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/HeadPivot/head/Smoking").GetComponent<PlayMakerFSM>());
            Object.Destroy(_char.Find("skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right/PayMoney").gameObject);
            _char.gameObject.SetActive(false);

            _uncleClone.SetParent(GameObject.Find("YARD/Building/BEDROOM1").transform, worldPositionStays: false);
            _char.position = new Vector3(-11.72816f, 0.3139997f, 11.2811f);
            _char.eulerAngles = new Vector3(0.0f, 270f, 0.0f);

            _uncleClone.gameObject.name = "ScreamUncle";

            if (_uncleClone.gameObject.GetComponent<MovingUncleHead>() == null)
                _uncleClone.gameObject.AddComponent<MovingUncleHead>();

            _uncleClone.gameObject.SetActive(true);
        }

        public static void CopySuicidal(GameObject cloned)
        {
            Transform _suicidal = GameObject.Instantiate(cloned.transform.GetChild(0).gameObject).transform;
            Transform _livingroom = GameObject.Find("YARD/Building/LIVINGROOM/LOD_livingroom").transform;

            _suicidal.SetParent(_livingroom, worldPositionStays: false);
            _suicidal.position = new Vector3(-1451.8280029296875f, -3.5810000896453859f, -1057.7840576171875f);
            _suicidal.localPosition = Vector3.zero;
            _suicidal.gameObject.SetActive(false);
        }

        public static void CopyVenttiAnimation()
        {
            if (PigWalkAnimation) return;

            Transform _pigMain = GameObject.Find("CABIN/Cabin/Ventti/PIG").transform;
            Transform _pigSkeleton = _pigMain.transform.Find("VenttiPig/Pivot/Char/skeleton");

            AnimationClip _clip = _pigSkeleton
                ? _pigSkeleton.GetComponent<Animation>().GetClip("venttipig_pig_walk")
                : Resources.FindObjectsOfTypeAll<AnimationClip>().Where(clip => clip.name == "venttipig_pig_walk").First();

            if (!_clip)
                throw new System.NullReferenceException("Cannot find a venttipig_pig_walk AnimationClip");

            PigWalkAnimation = AnimationClip.Instantiate(_clip);
        }

        public static void ActivateDINGONBIISIMiscThing3Permanently()
        {
            GameObject _obj = GameObject.Find("MAP/Buildings/DINGONBIISI");
            if (_obj == null) return;

            Transform _house = _obj?.transform;
            _house.GetPlayMaker("Clock").GetState("Off").ClearActions();

            Transform _misc = _house.transform.Find("Misc");
            _misc.gameObject.SetActive(true);

            Transform _thing3 = _misc.Find("Thing3");
            _thing3.gameObject.SetActive(true);
            _thing3.GetPlayMaker("Distance").GetState("Random").ClearActions(0, 1);

            Transform _mover = _thing3.Find("Mover");
            _mover.gameObject.SetActive(true);
            _mover.GetPlayMaker("Position").GetState("State 4").ClearActions();

            ClonedPhantom = GameObject.Instantiate(_mover.gameObject);
            Object.Destroy(ClonedPhantom.GetComponent<PlayMakerFSM>());
            ClonedPhantom.SetActive(false);

            ClonedPhantom.transform.parent = null;
            ClonedPhantom.transform.position = Vector3.zero;
            ClonedPhantom.transform.eulerAngles = Vector3.zero;
        }
    }
}
