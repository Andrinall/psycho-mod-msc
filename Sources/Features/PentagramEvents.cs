
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;

using Object = UnityEngine.Object;


namespace Psycho.Features
{
    internal sealed class PentagramEvents : CatchedComponent
    {
        bool isEventCalled = false;
        bool isEventFinished = true;

        Pentagram penta;
        Vector3 itemSpawnPos, grandmaSoundOrigPos;

        AudioSource grandmaSound;
        GameObject fire, fpsCamera, grandma, tireStatus;
        Transform fusetable;

        PlayMakerFSM blindless, hangoverCamera, knockout;
        FsmFloat blindIntensity;

        Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
        List<FsmFloat> fuelLevels = new List<FsmFloat>();
        List<FsmGameObject> tireStatusList = new List<FsmGameObject>();

        Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

        static PentagramEvents instance;

        protected override void Awaked()
        {
            instance = this;

            penta = GetComponent<Pentagram>();
            itemSpawnPos = transform.position + new Vector3(0, 0, .15f);

            fpsCamera = Utils.GetGlobalVariable<FsmGameObject>("POV").Value;
            
            blindless = fpsCamera.GetPlayMaker("Blindness");
            blindIntensity = blindless.GetVariable<FsmFloat>("Intensity");

            hangoverCamera = fpsCamera.GetPlayMaker("Hangover");
            hangoverCamera.Fsm.InitData();
            hangoverCamera.AddEvent("PENTAEVENT");
            hangoverCamera.AddGlobalTransition("PENTAEVENT", "Randomize");

            knockout = GameObject.Find("Systems/KnockOut").GetComponent<PlayMakerFSM>();

            fusetable = GameObject.Find("YARD/Building/Dynamics/FuseTable/Fusetable").transform;

            grandma = GameObject.Instantiate(GameObject.Find("ChurchGrandma/GrannyHiker/Char"));
            grandma.name = "PentaGrannyChar";

            Transform _temp = grandma.transform;
            _temp.SetParent(transform, worldPositionStays: false);
            _temp.localPosition = new Vector3(0, 0.7f, 0);
            _temp.localEulerAngles = new Vector3(0, -166.109f, 0);
            _temp.localScale = new Vector3(3.603494f, 1.028313f, 3.999587f);

            Destroy(_temp.Find("RagDollCar").gameObject);
            Destroy(_temp.Find("HeadTarget").gameObject);
            Destroy(_temp.Find("HumanTriggerCrime").gameObject);
            grandma.SetActive(false);

            grandmaSound = GameObject.Find("MasterAudio/Mummo/mummo_angry2").GetComponent<AudioSource>();
            grandmaSoundOrigPos = grandmaSound.transform.position;

            fuelLevels.Add(_getFuelLevel("Database/DatabaseMechanics/FuelTank")); // satsuma
            fuelLevels.Add(_getFuelLevel("FERNDALE(1630kg)/FuelTank")); // ferndale
            fuelLevels.Add(_getFuelLevel("HAYOSIKO(1500kg, 250)/FuelTank")); // hayosiko
            fuelLevels.Add(_getFuelLevel("GIFU(750/450psi)/FuelTank")); // gifu
            fuelLevels.Add(_getFuelLevel("RCO_RUSCKO12(270)/FuelTank")); // ruscko
            fuelLevels.Add(_getFuelLevel("JONNEZ ES(Clone)/FuelTank")); // jonnez
            fuelLevels.Add(_getFuelLevel("KEKMET(350-400psi)/FuelTank")); // kekmet

            tireStatus = GameObject.Find("Database/PartsStatus/TireStatus");
            tireStatusList.Add(getInstalledTireRef("WheelFL"));
            tireStatusList.Add(getInstalledTireRef("WheelFR"));
            tireStatusList.Add(getInstalledTireRef("WheelRL"));
            tireStatusList.Add(getInstalledTireRef("WheelRR"));

            fire = GameObject.Instantiate(
                Resources.FindObjectsOfTypeAll<GameObject>()
                    .First(v => v.name == "garbage barrel(itemx)")?.transform
                    ?.Find("Fire")?.gameObject
            );

            fire.transform.SetParent(transform, worldPositionStays: false);
            fire.transform.position = itemSpawnPos; //transform.position;
            Object.Destroy(fire.GetComponent<PlayMakerFSM>());
            Object.Destroy(fire.transform.Find("GarbageTrigger").gameObject);
            Object.Destroy(fire.transform.Find("FireTrigger").gameObject);
            fire.SetActive(false);
           
            List<Transform> _list = Resources.FindObjectsOfTypeAll<Transform>().ToList();

            objects.Add("beercase", _findPrefab(_list, "beer case"));
            objects.Add("battery", _list.First(v =>
                v.IsPrefab()
                && v.gameObject.name == "battery"
                && v.GetComponents<PlayMakerFSM>().Length > 0).gameObject
            );
            objects.Add("fusesbox", _findPrefab(_list, "fusepackage0"));
            objects.Add("sparksbox", _findPrefab(_list, "sparkplugbox0"));
            objects.Add("coolant", _findPrefab(_list, "coolant0"));
            objects.Add("motoroil", _findPrefab(_list, "motoroil0"));
            objects.Add("brakefluid", _findPrefab(_list, "brakefluid0"));
            objects.Add("coffee", _findPrefab(_list, "groundcoffee0"));
            objects.Add("booze", _findPrefab(_list, "booze"));
            objects.Add("sausages", _findPrefab(_list, "sausages"));
            objects.Add("lightbulb", _findPrefab(_list, "lightbulb0"));
            objects.Add("macaronbox", _findPrefab(_list, "macaron box"));
            objects.Add("chips", _findPrefab(_list, "potato chips"));
            objects.Add("sugar", _findPrefab(_list, "sugar"));
            objects.Add("cigarettes", _findPrefab(_list, "cigarettes0"));

            if (objects.Any(v => v.Value == null))
                Utils.PrintDebug(eConsoleColors.RED, $"[PentagramEvents] null exists in objects dict");

            addSoundClip("cash", "MasterAudio/Store/cash_register_1");
            addSoundClip("saatana", "MasterAudio/Hangover/hangover02");
            addSoundClip("nausea", "MasterAudio/PlayerMisc/puke01");
            addSoundClip("yawning", "MasterAudio/PlayerMisc/yawn01");
            addSoundClip("jokkeangry", "MasterAudio/DrunkLifter/kilju_alc_shit");
            addSoundClip("belching", "MasterAudio/Burb/burb01");
            addSoundClip("removal", "MasterAudio/CarBuilding/disassemble");
            addSoundClip("accelerate", "MasterAudio/Muscle/start1");
            addSoundClip("aborted", "MasterAudio/Ruscko/shutoff");
            addSoundClip("thunder", "MAP/CloudSystem/Clouds/Thunder/GroundStrike");
        }

        void OnDestroy() => instance = null;

        void createMoneyEnvelope()
        {
            GameObject _obj = GameObject.Find("YARD/PlayerMailBox/EnvelopeInspection/envelopemesh");
            _obj.name = "Money(Clone)";
            _obj.transform.parent = null;
            _obj.transform.position = itemSpawnPos;
            _obj.MakePickable();

            BoxCollider _collider = _obj.AddComponent<BoxCollider>();
            _collider.size = new Vector3(.21f, .15f, .005f);

            Rigidbody _rb = _obj.AddComponent<Rigidbody>();
            _rb.mass = .5f;
            _rb.drag = 1;
            _rb.angularDrag = 1;
            _rb.useGravity = true;
            _rb.isKinematic = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rb.detectCollisions = true;


            _obj.SetupFSM("Use", new string[] { "FINISHED", "USE" }, "Wait player", (_fsm, events) =>
            {
                FsmFloat _playerMoney = Utils.GetGlobalVariable<FsmFloat>("PlayerMoney");
                FsmFloat _money = new FsmFloat() { Name = "Money", Value = 100f };
                _fsm.AddVariable(_money);

                FsmEvent _finished = events[0];
                FsmEvent _useEvent = events[1];

                FsmString _guiInteraction = Utils.GetGlobalVariable<FsmString>("GUIinteraction");
                FsmBool _guiUse = Utils.GetGlobalVariable<FsmBool>("GUIuse");

                FsmOwnerDefault _owner = new FsmOwnerDefault() { OwnerOption = OwnerDefaultOption.UseOwner };
                FsmInt[] _layerMask = new FsmInt[1] { new FsmInt() { Value = 19 } };

                FsmState _waitPlayer = new FsmState(_fsm.Fsm)
                {
                    Name = "Wait player",
                    Transitions = new FsmTransition[1] {
                        new FsmTransition()
                        {
                            FsmEvent = _finished,
                            ToState = "Wait button"
                        }
                    },
                    Actions = new FsmStateAction[3]
                    {
                        new SetStringValue()
                        { stringVariable = _guiInteraction, stringValue = "", everyFrame = false },

                        new SetBoolValue()
                        { boolVariable = _guiUse, boolValue = false, everyFrame = false },

                        new MousePickEvent()
                        {
                            GameObject = _owner, rayDistance = 1,
                            mouseOver = _finished, layerMask = _layerMask,
                            invertMask = false, everyFrame = true
                        }
                    }
                };
                _waitPlayer.SaveActions();

                FsmState _waitButton = new FsmState(_fsm.Fsm)
                {
                    Name = "Wait button",
                    Transitions = new FsmTransition[2] {
                        new FsmTransition() { FsmEvent = _finished, ToState = "Wait player" },
                        new FsmTransition() { FsmEvent = _useEvent, ToState = "Use item" }
                    },
                    Actions = new FsmStateAction[4]
                    {
                        new SetStringValue()
                        { stringVariable = _guiInteraction, stringValue = "RANDOM GIFT MONEY", everyFrame = true },

                        new SetBoolValue()
                        { boolVariable = _guiUse, boolValue = true, everyFrame = true },

                        new MousePickEvent()
                        {
                            GameObject = _owner, rayDistance = 1,
                            mouseOff = _finished, layerMask = _layerMask,
                            invertMask = false, everyFrame = true
                        },

                        new GetButtonDown() { buttonName = "Use", sendEvent = _useEvent, storeResult = false }
                    }
                };
                _waitButton.SaveActions();

                FsmState _useItem = new FsmState(_fsm.Fsm)
                {
                    Name = "Use item",
                    Actions = new FsmStateAction[5]
                    {
                        new RandomFloat()
                        { min = 200, max = 1000, storeResult = _money },

                        new SetStringValue()
                        { stringVariable = _guiInteraction, stringValue = "", everyFrame = false },

                        new SetBoolValue()
                        { boolVariable = _guiUse, boolValue = false, everyFrame = false },

                        new FloatAdd()
                        { floatVariable = _playerMoney, add = _money, everyFrame = false, perSecond = false },

                        new DestroySelf() { detachChildren = false }
                    }
                };
                _useItem.SaveActions();


                return new FsmState[3] { _waitPlayer, _waitButton, _useItem };
            });
        }

        void createSpiritBottle()
        {
            GameObject _spirit = (GameObject)Instantiate(objects["booze"], itemSpawnPos, Quaternion.Euler(Vector3.zero));
            _spirit.SetActive(false);

            Rigidbody _rb = _spirit.GetComponent<Rigidbody>();
            _rb.useGravity = true;
            _rb.isKinematic = false;

            Material _spiritmat = Resources.FindObjectsOfTypeAll<Material>()
                .Where(v => v.name?.Contains("bottle_spirit_label") == true)
                .First();

            MeshRenderer _renderer = _spirit.GetComponent<MeshRenderer>();
            List<Material> _materials = new List<Material>(_renderer.materials);
            _materials.RemoveAt(1);
            _materials.Add(Instantiate(_spiritmat));
            _renderer.materials = _materials.ToArray();

            PlayMakerFSM _fsm = _spirit.GetComponent<PlayMakerFSM>();
            _fsm.enabled = false;

            _fsm.Fsm.GlobalTransitions = new List<FsmTransition>().ToArray();

            List<FsmState> _states = new List<FsmState>(_fsm.Fsm.States);
            FsmState _playAnim = _states.First(v => v.Name == "Play anim");

            _playAnim.Transitions = new List<FsmTransition>()
            {
                new FsmTransition()
                {
                    FsmEvent = _fsm.FsmEvents.First(v => v.Name == "FINISHED"),
                    ToState = "Destroy self"
                }
            }.ToArray();

            List<FsmStateAction> _animActions = new List<FsmStateAction>(_playAnim.Actions);
            (_animActions[2] as SendEvent).sendEvent = fpsCamera.transform.Find("Drink")
                .GetComponent<PlayMakerFSM>().FsmEvents
                .First(v => v.Name == "DRINKSPIRIT");

            _states.Remove(_states.First(v => v.Name == "State 1"));
            _states.Remove(_states.First(v => v.Name == "State 2"));
            _states.Remove(_states.First(v => v.Name == "Load"));
            _states.Remove(_states.First(v => v.Name == "Save"));
            _states.Remove(_states.First(v => v.Name == "Destroy"));

            _fsm.Fsm.States = _states.ToArray();
            _fsm.Fsm.StartState = "Wait player 2";
            _fsm.Fsm.Start();
            _fsm.enabled = true;

            _spirit.name = "spirit(Clone)";
            _spirit.SetActive(true);
        }

        void addSoundClip(string name, string path)
            => sounds.Add(name, GameObject.Find(path).GetComponent<AudioSource>().clip);

        FsmGameObject getInstalledTireRef(string slot)
        {
            PlayMakerFSM _wheelSlot = tireStatus.GetPlayMaker(slot);
            Wheel _script = (Wheel)_wheelSlot.GetVariable<FsmObject>("WheelScript").Value;
            GameObject _wheelSlotObj = _script.gameObject;
            PlayMakerFSM _slotConditionHandler = _wheelSlotObj.GetPlayMaker("Condition");
            FsmGameObject _thisTire = _slotConditionHandler.GetVariable<FsmGameObject>("ThisTire");
            return _thisTire;
        }

        public static void TriggerEvent(string _event, bool byCommand = false)
        {
            if (instance == null) return;
            if (string.IsNullOrEmpty(_event)) return;
            if (instance.isEventCalled || !instance.isEventFinished) return;
            if (byCommand)
            {
                if (!Pentagram.InnerEvents.Any(v => v.Value.Contains(_event))) return;
                Utils.PrintDebug("[event] Activate called");

                instance.isEventCalled = true;
                instance.isEventFinished = false;
                instance._processEvents(_event);
                return;
            }

            if (!instance.penta.LightsEnabled) return;
            if (!Pentagram.InnerEvents.Any(v => v.Value.Contains(_event))) return;
            Utils.PrintDebug("[event] Activate called");

            instance.isEventCalled = true;
            instance.isEventFinished = false;
            instance.penta.MakeItemsUnPickable();

            instance._processEvents(_event);
        }

        GameObject _findPrefab(List<Transform> list, string find)
            => list.First(v => v?.IsPrefab() == true && v?.name == find)?.gameObject;

        void _finishEvent()
        {
            Utils.PrintDebug("[event] _finishEvent called");
            isEventCalled = false;
            isEventFinished = true;
            penta.SetCandlesFireActive(false, true);
        }

        void _abortEvent()
        {
            Utils.PrintDebug("[event] _abortEvent called");
            _playSound("aborted");
            _finishEvent();
        }

        void _playSound(string soundcase = "")
        {
            if (string.IsNullOrEmpty(soundcase)) return;

            if (!sounds.TryGetValue(soundcase, out AudioClip clip))
            {
                Utils.PrintDebug(eConsoleColors.RED, $"[event] Soundcase {soundcase} doesn't exist in sounds list");
                return;
            }

            AudioSource.PlayClipAtPoint(clip, Globals.Player.position, 1f);
        }

        void _destroyItems()
        {
            Utils.PrintDebug("[event] _destroyItems called");
            penta.DestroyItems();
        }

        void _processEvents(string _event)
        {
            Utils.PrintDebug($"[event] _processEvents called: {_event}");
            switch (_event)
            {
                // ▼ spawn letter "RANDOM GIFT MONEY"
                case "money":
                    _startEvent(() => createMoneyEnvelope(), "cash");
                    return;

                // ▼ fill all fuel tanks
                case "fuel":
                    _startEvent(() => fuelLevels.ForEach(v => v.Value = 300f), "accelerate");
                    return;

                // ▼ spoil all products
                case "spoil":
                    _startGrannyEvent(() =>
                    {
                        List<PlayMakerFSM> _list = Resources.FindObjectsOfTypeAll<PlayMakerFSM>()
                            .Where(v =>
                                v.FsmName == "Use"
                                && v.gameObject.name.Contains("(itemx)")
                                && v.FsmEvents.Any(ev => ev.Name == "BAD")
                            )
                            .ToList();

                        _list.ForEach(_item =>
                        {
                            Utils.PrintDebug($"[event] {_item.gameObject.name} spoiled");
                            _item.GetVariable<FsmFloat>("Condition").Value = .5f;
                            _item.SendEvent("UPDATE");
                            _item.SendEvent("BAD");
                        });
                    });
                    return; // ▼ "spoil all products"

                // ▼ activate hangover effect
                case "hangover":
                    _startEvent(() =>
                    {
                        hangoverCamera.GetVariable<FsmFloat>("HangoverStrenght").Value = 0.008251007f;
                        hangoverCamera.GetVariable<FsmFloat>("TimeLeft").Value = 55f;
                        hangoverCamera.CallGlobalTransition("PENTAEVENT");
                    }, "saatana");
                    return;

                // ▼ set fatigue to 110f
                case "fatigue":
                    _startEvent(() => Globals.PlayerFatigue.Value = 110f, "yawning");
                    return;

                // ▼ set hunger & thirst to 101f
                case "hunger":
                    _startEvent(() => {
                        Globals.PlayerHunger.Value = 101f;
                        Globals.PlayerThirst.Value = 101f;
                    }, "nausea");
                    return;

                // ▼ blow ALL fuses in electricity point
                case "blowfuses":
                    _startEvent(() =>
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Transform _fusePivot = fusetable.GetChild(i);
                            if (_fusePivot.childCount == 0) continue;
                            _fusePivot.GetChild(0).GetPlayMaker("Use").SendEvent("BLOWFUSE");
                        }
                    }, "thunder");
                    return;

                // ▼ knockout player like a Jani's hit
                case "knockout":
                    _startEvent(() => knockout.CallGlobalTransition("GLOBALEVENT"), "saatana");
                    return;

                // ▼ burst all tires on wheels where installed on satsuma
                case "bursttires":
                    _startEvent(() =>
                    {
                        tireStatusList.ForEach(v =>
                        {
                            if (v.Value == null) return;
                            
                            v.Value.GetPlayMaker("Use")
                                .GetVariable<FsmFloat>("TireHealth")
                                .Value = 0f;
                        });
                    }, "jokkeangry");
                    return;

                // ▼ empty all fuel tanks
                case "outoffuel":
                    _startEvent(() => fuelLevels.ForEach(v => v.Value = 0f), "removal");
                    return;

                // ▼ BLIND BY BEE
                case "blindless":
                    _startEvent(() =>
                    {
                        blindless.SendEvent("BLINDBEE");
                        blindIntensity.Value = 60f;
                    });
                    return;

                // ▼ spawn a spirit bottle (not booze, spirit)
                case "spirit":
                    _startEvent(() => createSpiritBottle(), "belching");
                    return;

                // ▼ spawn item from list (PentagramEvents.objects)
                case string _item when objects.Keys.Contains(_item):
                    _startEvent(() => _cloneItemFromPrefab(_item), "cash");
                    return;
            }
        }

        void _startEvent(Action func, string sound = "") => StartCoroutine(_eventCoroutine(func, sound));

        void _startGrannyEvent(Action func) => StartCoroutine(_eventGrannyCoroutine(func));

        void _cloneItemFromPrefab(string item)
        {
            if (!objects.TryGetValue(item, out GameObject _prefab))
            {
                Utils.PrintDebug(eConsoleColors.RED, $"[event] _spawnItem object {item} doesn't exist in pool");
                _abortEvent();
                return;
            }

            GameObject _cloned = (GameObject)Object.Instantiate(
                _prefab,
                itemSpawnPos,
                Quaternion.Euler(Vector3.zero)
            );

            Utils.PrintDebug($"[event] _spawnItem cloned item {_cloned}");
        }

        FsmFloat _getFuelLevel(string path)
            => GameObject.Find(path).GetPlayMaker("Data").GetVariable<FsmFloat>("FuelLevel");

        IEnumerator _playFireAnimation()
        {
            Utils.PrintDebug("[event] _playFireAnimation called");
            
            fire.SetActive(true);            
            yield return new WaitForSeconds(2f);

            try
            {
                _destroyItems();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, "Exception in PrentagramEvents._playFireAnimation()::_destroyItems()");
                ModConsole.Error(ex.GetFullMessage());
            }

            yield return new WaitForSeconds(2f);
            fire.SetActive(false);
        }

        IEnumerator _eventGrannyCoroutine(Action action)
        {
            Utils.PrintDebug("[event] _eventGrannyCoroutine called");
            fire.SetActive(true);
            
            yield return new WaitForSeconds(2f);
            _destroyItems();

            grandma.SetActive(true);
            grandmaSound.transform.position = transform.position;
            grandmaSound.Play();

            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Exception in {Utils.GetMethodPath(action.Method)}");
                ModConsole.Error(ex.GetFullMessage());
            }

            while (grandmaSound.isPlaying)
                yield return new WaitForSeconds(.1f);
            
            grandmaSound.transform.position = grandmaSoundOrigPos;
            grandma.SetActive(false);
            fire.SetActive(false);
            _finishEvent();
        }

        IEnumerator _eventCoroutine(Action action, string sound = "")
        {
            Utils.PrintDebug("[event] _eventCoroutine called");
            yield return StartCoroutine(_playFireAnimation());

            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, $"Exception in {Utils.GetMethodPath(action.Method)}");
                ModConsole.Error(ex.GetFullMessage());
            }

            if (!string.IsNullOrEmpty(sound))
                _playSound(sound);

            _finishEvent();
        }
    }
}
