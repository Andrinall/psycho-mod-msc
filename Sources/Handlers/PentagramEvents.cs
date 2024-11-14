using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using MSCLoader;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using Psycho.Objects;
using Psycho.Internal;
using Psycho.Extensions;
using Object = UnityEngine.Object;


namespace Psycho.Handlers
{
    internal sealed class PentagramEvents : CatchedComponent
    {
        public bool IsEventCalled = false;
        public bool IsEventFinished = true;

        Pentagram penta;
        Vector3 itemSpawnPos, GrandmaSoundOrigPos;

        AudioSource GrandmaSound;
        GameObject Fire, MoneyObj, FPSCamera, Grandma;
        Transform Fusetable, Player;

        PlayMakerFSM Blindless, HangoverCamera, Knockout;
        FsmFloat PlayerFatigue, PlayerHunger, PlayerThirst, BlindIntensity;

        Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
        List<FsmFloat> fuellevels = new List<FsmFloat>();


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

            // "SATSUMA(557kg, 248)/CarSimulation/Engine/Fuel"
            fuellevels.Add(_getFuelLevel("Database/DatabaseMechanics/FuelTank")); // satsuma
            fuellevels.Add(_getFuelLevel("FERNDALE(1630kg)/FuelTank")); // ferndale
            fuellevels.Add(_getFuelLevel("HAYOSIKO(1500kg, 250)/FuelTank")); // hayosiko
            fuellevels.Add(_getFuelLevel("GIFU(750/450psi)/FuelTank")); // gifu
            fuellevels.Add(_getFuelLevel("RCO_RUSCKO12(270)/FuelTank")); // ruscko

            Fire = GameObject.Instantiate(
                Resources.FindObjectsOfTypeAll<GameObject>()
                    .First(v=>v.name=="garbage barrel(itemx)")?.transform
                    ?.Find("Fire")?.gameObject
            );

            Fire.transform.SetParent(transform, worldPositionStays: false);
            Fire.transform.position = itemSpawnPos; //transform.position;
            Object.Destroy(Fire.GetComponent<PlayMakerFSM>());
            Object.Destroy(Fire.transform.Find("GarbageTrigger").gameObject);
            Object.Destroy(Fire.transform.Find("FireTrigger").gameObject);
            Fire.SetActive(false);
           
            List<Transform> list = Resources.FindObjectsOfTypeAll<Transform>().ToList();

            MoneyObj =
                GameObject.Find("MISC/holiday present(Clon2)/Parts/Money")
                ?? list.First(v => v != null && v?.gameObject?.name == "Money" && v?.parent == null)?.gameObject;

            objects.Add("beercase", _findPrefab(list, "beer case"));
            //objects.Add("battery", list.FirstOrDefault(v => v != null && v?.IsPrefab() == true && v?.name == "battety" && v?.GetComponent<PlayMakerFSM>() == null)?.gameObject);
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
            //objects.Add("spirit", _findPrefab(list, "spirit")); // ??!

            objects.Any(v =>
            {
                bool res = v.Value == null;
                if (res) Utils.PrintDebug(eConsoleColors.RED, $"{v.Key} in objects is null");
                return res;
            });

        }

        public void Activate(string _event)
        {
            Utils.PrintDebug("Activate called");
            if (IsEventCalled || !IsEventFinished) return;
            if (string.IsNullOrEmpty(_event)) return;
            if (!penta.InnerEvents.Any(v => v.Value.Contains(_event))) return;

            IsEventCalled = true;
            IsEventFinished = false;
            Utils.PrintDebug("_processEvents called");
            _processEvents(_event);
        }

        GameObject _findPrefab(List<Transform> list, string find)
            => list.First(v => v != null && v?.IsPrefab() == true && v?.name == find)?.gameObject ?? null;

        void _finishEvent()
        {
            Utils.PrintDebug("_finishEvent called");
            IsEventCalled = false;
            IsEventFinished = true;
        }

        void _abortEvent()
        {
            Utils.PrintDebug("_abortEvent called");
            _playSound("aborted");
            _finishEvent();
        }

        void _playSound(string soundcase = "item")
        {
            switch (soundcase)
            {
                case "cash":
                    break;

                case "saatana":
                    break;

                case "nausea":
                    break;

                case "belching":
                    break;
                
                case "jokkeangry":
                    break;

                case "accelerate":
                    break;

                case "removal":
                    break;

                case "thunder":
                    break;

                case "yawning":
                    break;

                case "aborted":
                    break;
            }
        }

        void _destroyItems()
        {
            Utils.PrintDebug("_destroyItems called");
            penta.DestroyItems();
        }

        void _processEvents(string _event)
        {
            switch (_event)
            {
                case "money":
                    _startEvent(() =>
                    {
                        GameObject cloned = GameObject.Instantiate(MoneyObj);
                        cloned.transform.parent = null;
                        cloned.transform.position = itemSpawnPos;

                        PlayMakerFSM fsm = cloned.GetComponent<PlayMakerFSM>();
                        FsmState state = fsm.GetState("State 1");

                        var actions = new List<FsmStateAction>(state.Actions);
                        actions.RemoveAt(4);
                        actions.Add(new DestroySelf() { detachChildren = true });

                        state.Actions = actions.ToArray();
                        state.SaveActions();

                        MoneyObj.SetActive(true);
                    }, "cash");
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
                            Utils.PrintDebug(eConsoleColors.YELLOW, $"{item.gameObject.name} spoiled");
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
                    }, "saaatana");
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
                    _startEvent(() => Knockout.CallGlobalTransition("GLOBALEVENT"));
                    return; // ▼ "knockout player like a by Jani hit"

                case "bursttires":
                    _startEvent(() => { }, "jokkeangry");
                    throw new NotImplementedException($"Event {_event} are not implemented");

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
                    _playSound("belching");
                    throw new NotImplementedException($"Event {_event} are not implemented");

                case string item when objects.Keys.Contains(item):
                    _startEvent(() => _cloneItemFromPrefab(item));
                    return; // ▼ "spawn item"
            }
        }

        void _startEvent(Action func, string sound = "") => StartCoroutine(_eventCoroutine(func, sound));

        void _startGrannyEvent(Action func) => StartCoroutine(_eventGrannyCoroutine(func));

        void _cloneItemFromPrefab(string item)
        {
            if (!objects.TryGetValue(item, out GameObject prefab))
            {
                Utils.PrintDebug($"_spawnItem object {item} doesn't exist in pool");
                _abortEvent();
                return;
            }

            GameObject cloned = (GameObject)Object.Instantiate(
                prefab,
                itemSpawnPos,
                Quaternion.Euler(Vector3.zero)
            );

            Utils.PrintDebug($"_spawnItem cloned item {cloned}");
        }

        FsmFloat _getFuelLevel(string path)
            => GameObject.Find(path).GetPlayMaker("Data").GetVariable<FsmFloat>("FuelLevel");

        IEnumerator _playFireAnimation(Action onFinish)
        {
            Utils.PrintDebug($"_playFireAnimation called");
            
            Fire.SetActive(true);            
            yield return new WaitForSeconds(2f);
            /*if (!penta.CheckItems())
            {
                Utils.PrintDebug("_playFireAnimation any items does not match the prescription");
                _abortEvent();
                Fire.SetActive(false);
                yield break;
            }*/
            onFinish?.Invoke();
            yield return new WaitForSeconds(2f);
            Fire.SetActive(false);
        }

        IEnumerator _eventGrannyCoroutine(Action _action)
        {
            Utils.PrintDebug("_eventGrannyCoroutine called");
            Fire.SetActive(true);
            Grandma.SetActive(true);
            GrandmaSound.transform.position = transform.position;
            GrandmaSound.Play();
            _action?.Invoke();

            while (GrandmaSound.isPlaying)
                yield return new WaitForSeconds(.1f);
            
            GrandmaSound.transform.position = GrandmaSoundOrigPos;
            Grandma.SetActive(false);
            Fire.SetActive(false);
            _finishEvent();
        }

        IEnumerator _eventCoroutine(Action _action, string sound = "")
        {
            Utils.PrintDebug("_eventCoroutine called");
            yield return StartCoroutine(_playFireAnimation(_destroyItems));
            _action?.Invoke();
            
            if (!string.IsNullOrEmpty(sound))
                _playSound(sound);

            _finishEvent();
        }
    }
}
