using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Internal;
using Psycho.Extensions;
using Object = UnityEngine.Object;


namespace Psycho.Features
{
    internal sealed class PentagramEvents : CatchedComponent
    {
        public bool IsEventCalled = false;
        public bool IsEventFinished = true;

        Pentagram penta;
        Vector3 itemSpawnPos, GrandmaSoundOrigPos;

        AudioSource GrandmaSound;
        GameObject Fire, MoneyObj, FPSCamera, Grandma, TireStatus;
        Transform Fusetable, Player;

        PlayMakerFSM Blindless, HangoverCamera, Knockout;
        FsmFloat PlayerFatigue, PlayerHunger, PlayerThirst, BlindIntensity;

        Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
        List<FsmFloat> fuellevels = new List<FsmFloat>();
        List<FsmGameObject> TireStatusList = new List<FsmGameObject>();

        Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();


        internal override void Awaked()
        {
            penta = GetComponent<Pentagram>();
            itemSpawnPos = transform.position + new Vector3(0, 0, .15f);

            GameObject PLAYER = GameObject.Find("PLAYER");
            Player = PLAYER.transform;
            PlayerFatigue = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue");
            PlayerHunger = Utils.GetGlobalVariable<FsmFloat>("PlayerHunger");
            PlayerThirst = Utils.GetGlobalVariable<FsmFloat>("PlayerThirst");
            FPSCamera = Utils.GetGlobalVariable<FsmGameObject>("POV").Value;
            
            Blindless = FPSCamera.GetPlayMaker("Blindness");
            BlindIntensity = Blindless.GetVariable<FsmFloat>("Intensity");

            HangoverCamera = FPSCamera.GetPlayMaker("Hangover");
            HangoverCamera.Fsm.InitData();
            HangoverCamera.AddEvent("PENTAEVENT");
            HangoverCamera.AddGlobalTransition("PENTAEVENT", "Shake");

            Knockout = GameObject.Find("Systems/KnockOut").GetComponent<PlayMakerFSM>();

            Fusetable = GameObject.Find("YARD/Building/Dynamics/FuseTable/Fusetable").transform;

            Grandma = GameObject.Instantiate(GameObject.Find("ChurchGrandma/GrannyHiker/Char"));
            Grandma.name = "PentaGrannyChar";

            Transform temp = Grandma.transform;
            temp.SetParent(transform, worldPositionStays: false);
            temp.localPosition = new Vector3(0, 0.7f, 0);
            temp.localEulerAngles = new Vector3(0, -166.109f, 0);
            temp.localScale = new Vector3(3.603494f, 1.028313f, 3.999587f);

            Destroy(temp.Find("RagDollCar").gameObject);
            Destroy(temp.Find("HeadTarget").gameObject);
            Destroy(temp.Find("HumanTriggerCrime").gameObject);
            Grandma.SetActive(false);

            GrandmaSound = GameObject.Find("MasterAudio/Mummo/mummo_angry2").GetComponent<AudioSource>();
            GrandmaSoundOrigPos = GrandmaSound.transform.position;

            fuellevels.Add(_getFuelLevel("Database/DatabaseMechanics/FuelTank")); // satsuma
            fuellevels.Add(_getFuelLevel("FERNDALE(1630kg)/FuelTank")); // ferndale
            fuellevels.Add(_getFuelLevel("HAYOSIKO(1500kg, 250)/FuelTank")); // hayosiko
            fuellevels.Add(_getFuelLevel("GIFU(750/450psi)/FuelTank")); // gifu
            fuellevels.Add(_getFuelLevel("RCO_RUSCKO12(270)/FuelTank")); // ruscko
            fuellevels.Add(_getFuelLevel("JONNEZ ES(Clone)/FuelTank")); // jonnez
            fuellevels.Add(_getFuelLevel("KEKMET(350-400psi)/FuelTank")); // kekmet

            TireStatus = GameObject.Find("Database/PartsStatus/TireStatus");
            TireStatusList.Add(getInstalledTireRef("WheelFL"));
            TireStatusList.Add(getInstalledTireRef("WheelFR"));
            TireStatusList.Add(getInstalledTireRef("WheelRL"));
            TireStatusList.Add(getInstalledTireRef("WheelRR"));

            Fire = GameObject.Instantiate(
                Resources.FindObjectsOfTypeAll<GameObject>()
                    .First(v => v.name == "garbage barrel(itemx)")?.transform
                    ?.Find("Fire")?.gameObject
            );

            Fire.transform.SetParent(transform, worldPositionStays: false);
            Fire.transform.position = itemSpawnPos; //transform.position;
            Object.Destroy(Fire.GetComponent<PlayMakerFSM>());
            Object.Destroy(Fire.transform.Find("GarbageTrigger").gameObject);
            Object.Destroy(Fire.transform.Find("FireTrigger").gameObject);
            Fire.SetActive(false);
           
            List<Transform> list = Resources.FindObjectsOfTypeAll<Transform>().ToList();

            objects.Add("beercase", _findPrefab(list, "beer case"));
            objects.Add("battery", list.First(v =>
                v.IsPrefab()
                && v.gameObject.name == "battery"
                && v.GetComponents<PlayMakerFSM>().Length > 0).gameObject
            );
            objects.Add("fusesbox", _findPrefab(list, "fusepackage0"));
            objects.Add("sparksbox", _findPrefab(list, "sparkplugbox0"));
            objects.Add("coolant", _findPrefab(list, "coolant0"));
            objects.Add("motoroil", _findPrefab(list, "motoroil0"));
            objects.Add("brakefluid", _findPrefab(list, "brakefluid0"));
            objects.Add("coffee", _findPrefab(list, "groundcoffee0"));
            objects.Add("booze", _findPrefab(list, "booze"));
            objects.Add("sausages", _findPrefab(list, "sausages"));
            objects.Add("lightbulb", _findPrefab(list, "lightbulb0"));
            objects.Add("macaronbox", _findPrefab(list, "macaron box"));
            objects.Add("chips", _findPrefab(list, "potato chips"));
            objects.Add("sugar", _findPrefab(list, "sugar"));
            objects.Add("cigarettes", _findPrefab(list, "cigarettes0"));

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

        void createMoneyEnvelope()
        {
            GameObject obj = GameObject.Find("YARD/PlayerMailBox/EnvelopeInspection/envelopemesh");
            obj.name = "Money(Clone)";
            obj.transform.parent = null;
            obj.transform.position = itemSpawnPos;
            obj.MakePickable();

            BoxCollider collider = obj.AddComponent<BoxCollider>();
            collider.size = new Vector3(.21f, .15f, .005f);

            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.mass = .5f;
            rb.drag = 1;
            rb.angularDrag = 1;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.detectCollisions = true;

            FsmFloat playerMoney = Utils.GetGlobalVariable<FsmFloat>("PlayerMoney");
            FsmFloat money = new FsmFloat() { Name = "Money", Value = 100f };

            PlayMakerFSM fsm = obj.AddComponent<PlayMakerFSM>();
            fsm.InitializeFSM();
            fsm.enabled = false;
            fsm.Fsm.Name = "Use";
            fsm.AddVariable(money);
            fsm.AddEvent("FINISHED");
            fsm.AddEvent("USE");

            FsmEvent finished = fsm.Fsm.Events[0];
            FsmEvent useEvent = fsm.Fsm.Events[1];

            FsmString GUIinteraction = Utils.GetGlobalVariable<FsmString>("GUIinteraction");
            FsmBool GUIuse = Utils.GetGlobalVariable<FsmBool>("GUIuse");

            FsmOwnerDefault owner = new FsmOwnerDefault() { OwnerOption = OwnerDefaultOption.UseOwner };
            FsmInt[] layerMask = new FsmInt[1] { new FsmInt() { Value = 19 } };

            FsmState waitPlayer = new FsmState(fsm.Fsm)
            {
                Name = "Wait player",
                Transitions = new FsmTransition[1] {
                    new FsmTransition()
                    {
                        FsmEvent = finished,
                        ToState = "Wait button"
                    }
                },
                Actions = new FsmStateAction[3]
                {
                    new SetStringValue()
                    { stringVariable = GUIinteraction, stringValue = "", everyFrame = false },

                    new SetBoolValue()
                    { boolVariable = GUIuse, boolValue = false, everyFrame = false },

                    new MousePickEvent()
                    {
                        GameObject = owner, rayDistance = 1,
                        mouseOver = finished, layerMask = layerMask,
                        invertMask = false, everyFrame = true
                    }
                }
            };
            waitPlayer.SaveActions();

            FsmState waitButton = new FsmState(fsm.Fsm)
            {
                Name = "Wait button",
                Transitions = new FsmTransition[2] {
                    new FsmTransition() { FsmEvent = finished, ToState = "Wait player" },
                    new FsmTransition() { FsmEvent = useEvent, ToState = "Use item" }
                },
                Actions = new FsmStateAction[4]
                {
                    new SetStringValue()
                    { stringVariable = GUIinteraction, stringValue = "RANDOM GIFT MONEY", everyFrame = true },

                    new SetBoolValue()
                    { boolVariable = GUIuse, boolValue = true, everyFrame = true },

                    new MousePickEvent()
                    {
                        GameObject = owner, rayDistance = 1,
                        mouseOff = finished, layerMask = layerMask,
                        invertMask = false, everyFrame = true
                    },

                    new GetButtonDown() { buttonName = "Use", sendEvent = useEvent, storeResult = false }
                }
            };
            waitButton.SaveActions();

            FsmState useItem = new FsmState(fsm.Fsm)
            {
                Name = "Use item",
                Actions = new FsmStateAction[5]
                {
                    new RandomFloat()
                    { min = 200, max = 1000, storeResult = money },

                    new SetStringValue()
                    { stringVariable = GUIinteraction, stringValue = "", everyFrame = false },

                    new SetBoolValue()
                    { boolVariable = GUIuse, boolValue = false, everyFrame = false },

                    new FloatAdd()
                    { floatVariable = playerMoney, add = money, everyFrame = false, perSecond = false },

                    new DestroySelf() { detachChildren = false }
                }
            };
            useItem.SaveActions();

            fsm.Fsm.States = new FsmState[3] { waitPlayer, waitButton, useItem };
            fsm.Fsm.StartState = "Wait player";
            fsm.Fsm.Start();
            fsm.enabled = true;
        }

        void createSpiritBottle()
        {
            GameObject spirit = (GameObject)Instantiate(objects["booze"], itemSpawnPos, Quaternion.Euler(Vector3.zero));
            spirit.SetActive(false);

            Rigidbody rb = spirit.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            Material spiritmat = Resources.FindObjectsOfTypeAll<Material>()
                .Where(v => v.name?.Contains("bottle_spirit_label") == true)
                .First();

            MeshRenderer renderer = spirit.GetComponent<MeshRenderer>();
            List<Material> materials = new List<Material>(renderer.materials);
            materials.RemoveAt(1);
            materials.Add(Instantiate(spiritmat));
            renderer.materials = materials.ToArray();

            PlayMakerFSM fsm = spirit.GetComponent<PlayMakerFSM>();
            fsm.enabled = false;

            fsm.Fsm.GlobalTransitions = new List<FsmTransition>().ToArray();

            List<FsmState> states = new List<FsmState>(fsm.Fsm.States);
            FsmState playAnim = states.First(v => v.Name == "Play anim");

            playAnim.Transitions = new List<FsmTransition>()
            {
                new FsmTransition()
                {
                    FsmEvent = fsm.FsmEvents.First(v => v.Name == "FINISHED"),
                    ToState = "Destroy self"
                }
            }.ToArray();

            List<FsmStateAction> animActions = new List<FsmStateAction>(playAnim.Actions);
            (animActions[2] as SendEvent).sendEvent = FPSCamera.transform.Find("Drink")
                .GetComponent<PlayMakerFSM>().FsmEvents
                .First(v => v.Name == "DRINKSPIRIT");

            states.Remove(states.First(v => v.Name == "State 1"));
            states.Remove(states.First(v => v.Name == "State 2"));
            states.Remove(states.First(v => v.Name == "Load"));
            states.Remove(states.First(v => v.Name == "Save"));
            states.Remove(states.First(v => v.Name == "Destroy"));
            
            fsm.Fsm.States = states.ToArray();
            fsm.Fsm.StartState = "Wait player 2";
            fsm.Fsm.Start();
            fsm.enabled = true;

            spirit.name = "spirit(Clone)";
            spirit.SetActive(true);
        }

        void addSoundClip(string name, string path)
            => sounds.Add(name, GameObject.Find(path).GetComponent<AudioSource>().clip);

        FsmGameObject getInstalledTireRef(string slot)
        {
            PlayMakerFSM WheelSlot = TireStatus.GetPlayMaker(slot);
            Wheel Script = (Wheel)WheelSlot.GetVariable<FsmObject>("WheelScript").Value;
            GameObject WheelSlotObj = Script.gameObject;
            PlayMakerFSM SlotConditionHandler = WheelSlotObj.GetPlayMaker("Condition");
            FsmGameObject ThisTire = SlotConditionHandler.GetVariable<FsmGameObject>("ThisTire");
            return ThisTire;
        }

        public void Activate(string _event)
        {
            if (!penta.GetCandlesFireActive()) return;
            if (IsEventCalled || !IsEventFinished) return;
            if (string.IsNullOrEmpty(_event)) return;
            if (!penta.InnerEvents.Any(v => v.Value.Contains(_event))) return;
            Utils.PrintDebug(eConsoleColors.YELLOW, "[event] Activate called");

            IsEventCalled = true;
            IsEventFinished = false;
            penta.MakeItemsUnPickable();

            _processEvents(_event);
        }

        GameObject _findPrefab(List<Transform> list, string find)
            => list.First(v => v?.IsPrefab() == true && v?.name == find)?.gameObject;

        void _finishEvent()
        {
            Utils.PrintDebug(eConsoleColors.GREEN, "[event] _finishEvent called");
            IsEventCalled = false;
            IsEventFinished = true;
            penta.SetCandlesFireActive(false, true);
        }

        void _abortEvent()
        {
            Utils.PrintDebug(eConsoleColors.RED, "[event] _abortEvent called");
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

            AudioSource.PlayClipAtPoint(clip, Player.position, 1f);
        }

        void _destroyItems()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, "[event] _destroyItems called");
            penta.DestroyItems();
        }

        void _processEvents(string _event)
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"[event] _processEvents called: {_event}");
            switch (_event)
            {
                case "money":
                    _startEvent(() => createMoneyEnvelope(), "cash");
                    return; // ▼ "spawn letter "RANDOM GIFT MONEY""

                case "fuel":
                    _startEvent(() => fuellevels.ForEach(v => v.Value = 300f), "accelerate");
                    return; // ▼ "fill all fuel tanks"

                case "spoil":
                    _startGrannyEvent(() =>
                    {
                        // spoil all products
                        List<PlayMakerFSM> list = Resources.FindObjectsOfTypeAll<PlayMakerFSM>()
                            .Where(v =>
                                v.FsmName == "Use"
                                && v.gameObject.name.Contains("(itemx)")
                                && v.FsmEvents.Any(ev => ev.Name == "BAD")
                            )
                            .ToList();

                        list.ForEach(item =>
                        {
                            Utils.PrintDebug(eConsoleColors.YELLOW, $"[event] {item.gameObject.name} spoiled");
                            item.GetVariable<FsmFloat>("Condition").Value = .5f;
                            item.SendEvent("UPDATE");
                            item.SendEvent("BAD");
                        });
                    });
                    return; // ▼ "spoil all products"

                case "hangover":
                    _startEvent(() =>
                    {
                        HangoverCamera.GetVariable<FsmFloat>("HangoverStrenght").Value = 2;
                        HangoverCamera.GetVariable<FsmFloat>("HangoverStrenghtMinus").Value = -2;
                        HangoverCamera.GetVariable<FsmFloat>("TimeLeft").Value = 10;
                        HangoverCamera.SendEvent("PENTAEVENT");
                    }, "saatana");
                    return; // ▼ "activate hangover"

                case "fatigue":
                    _startEvent(() => PlayerFatigue.Value = 110f, "yawning");
                    return; // ▼ "set fatigue to 100"

                case "hunger":
                    _startEvent(() => {
                        PlayerHunger.Value = 101f;
                        PlayerThirst.Value = 101f;
                    }, "nausea");
                    return; // ▼ "set hunger to 100"

                case "blowfuses":
                    _startEvent(() =>
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Transform fusePivot = Fusetable.GetChild(i);
                            if (fusePivot.childCount == 0) continue;
                            fusePivot.GetChild(0).GetPlayMaker("Use").SendEvent("BLOWFUSE");
                        }
                    }, "thunder");
                    return; // ▼ "blow ALL fuses"

                case "knockout":
                    _startEvent(() => Knockout.CallGlobalTransition("GLOBALEVENT"), "saatana");
                    return; // ▼ "knockout player like a by Jani hit"

                case "bursttires":
                    _startEvent(() =>
                    {
                        TireStatusList.ForEach(v =>
                        {
                            if (v.Value == null) return;
                            
                            v.Value.GetPlayMaker("Use")
                                .GetVariable<FsmFloat>("TireHealth")
                                .Value = 0f;
                        });
                    }, "jokkeangry");
                    return; // ▼ "burst all tires on wheels installed to satsuma"

                case "outoffuel":
                    _startEvent(() => fuellevels.ForEach(v => v.Value = 0f), "removal");
                    return; // ▼ "empty all fuel tanks"

                case "blindless":
                    _startEvent(() =>
                    {
                        Blindless.SendEvent("BLINDBEE");
                        BlindIntensity.Value = 60f;
                    });
                    return; // ▼ "BLIND BY BEE"

                case "spirit":
                    _startEvent(() => createSpiritBottle(), "belching");
                    return;

                case string item when objects.Keys.Contains(item):
                    _startEvent(() => _cloneItemFromPrefab(item), "cash");
                    return; // ▼ "spawn item"
            }
        }

        void _startEvent(Action func, string sound = "") => StartCoroutine(_eventCoroutine(func, sound));

        void _startGrannyEvent(Action func) => StartCoroutine(_eventGrannyCoroutine(func));

        void _cloneItemFromPrefab(string item)
        {
            if (!objects.TryGetValue(item, out GameObject prefab))
            {
                Utils.PrintDebug(eConsoleColors.RED, $"[event] _spawnItem object {item} doesn't exist in pool");
                _abortEvent();
                return;
            }

            GameObject cloned = (GameObject)Object.Instantiate(
                prefab,
                itemSpawnPos,
                Quaternion.Euler(Vector3.zero)
            );

            Utils.PrintDebug(eConsoleColors.GREEN, $"[event] _spawnItem cloned item {cloned}");
        }

        FsmFloat _getFuelLevel(string path)
            => GameObject.Find(path).GetPlayMaker("Data").GetVariable<FsmFloat>("FuelLevel");

        IEnumerator _playFireAnimation()
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, $"[event] _playFireAnimation called");
            
            Fire.SetActive(true);            
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
            Fire.SetActive(false);
        }

        IEnumerator _eventGrannyCoroutine(Action _action)
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, "[event] _eventGrannyCoroutine called");
            Fire.SetActive(true);
            
            yield return new WaitForSeconds(2f);
            _destroyItems();

            Grandma.SetActive(true);
            GrandmaSound.transform.position = transform.position;
            GrandmaSound.Play();

            try
            {
                _action?.Invoke();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, "Exception in PrentagramEvents._eventGrannyCoroutine()::_action.Invoke()");
                ModConsole.Error(ex.GetFullMessage());
            }

            while (GrandmaSound.isPlaying)
                yield return new WaitForSeconds(.1f);
            
            GrandmaSound.transform.position = GrandmaSoundOrigPos;
            Grandma.SetActive(false);
            Fire.SetActive(false);
            _finishEvent();
        }

        IEnumerator _eventCoroutine(Action _action, string sound = "")
        {
            Utils.PrintDebug(eConsoleColors.YELLOW, "[event] _eventCoroutine called");
            yield return StartCoroutine(_playFireAnimation());

            try
            {
                _action?.Invoke();
            }
            catch (Exception ex)
            {
                Utils.PrintDebug(eConsoleColors.RED, "Exception in PrentagramEvents._eventCoroutine()::_action.Invoke()");
                ModConsole.Error(ex.GetFullMessage());
            }
            
            if (!string.IsNullOrEmpty(sound))
                _playSound(sound);

            _finishEvent();
        }
    }
}
