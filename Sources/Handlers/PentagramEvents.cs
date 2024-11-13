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
        Vector3 itemSpawnPos;

        FsmFloat PlayerFatigue, PlayerHunger, BlindIntensity;
        GameObject Fire, MoneyObj, FPSCamera;
        Transform Fusetable;

        PlayMakerFSM Blindless, Hangover, Knockout;

        Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();


        internal override void Awaked()
        {
            itemSpawnPos = transform.position + new Vector3(0, 0, .15f);

            penta = GetComponent<Pentagram>();
            PlayerFatigue = Utils.GetGlobalVariable<FsmFloat>("PlayerFatigue");
            PlayerHunger = Utils.GetGlobalVariable<FsmFloat>("PlayerHunger");
            FPSCamera = Utils.GetGlobalVariable<FsmGameObject>("POV").Value;
            
            Blindless = FPSCamera.GetPlayMaker("Blindless");
            BlindIntensity = Blindless.GetVariable<FsmFloat>("Intensity");

            Hangover = FPSCamera.GetPlayMaker("Hangover");
            Hangover.AddGlobalTransition("HANGOVER", "Shake");

            Knockout = GameObject.Find("Systems/KnockOut").GetComponent<PlayMakerFSM>();

            Fusetable = GameObject.Find("YARD/Building/Dynamics/FuseTable/Fusetable").transform;

            Fire = GameObject.Instantiate(
                Resources.FindObjectsOfTypeAll<GameObject>()
                    .First(v=>v.name=="garbage barrel(itemx)")?.transform
                    ?.Find("Fire")?.gameObject
            );
            Utils.PrintDebug($"FIRE {Fire}");

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
            Utils.PrintDebug($"MoneyObj {MoneyObj}");

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
            Utils.PrintDebug("objects assigned");

            objects.Any(v =>
            {
                Utils.PrintDebug($"{v}; {v.Key} ; {v.Value}");
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
            Utils.PrintDebug("pass three checks");
            if (!penta.InnerEvents.Any(v => v.Value.Contains(_event))) return;
            Utils.PrintDebug("event in inner events");

            IsEventCalled = true;
            IsEventFinished = false;
            _processEvents(_event);
        }

        GameObject _findPrefab(List<Transform> list, string find)
        {
            Utils.PrintDebug($"_findPrefab find {find}");
            GameObject go = list.First(v => v != null && v?.IsPrefab() == true && v?.name == find)?.gameObject ?? null;
            Utils.PrintDebug($"_findPrefab go {go}");
            return go;
        }

        void _finishEvent()
        {
            IsEventCalled = false;
            IsEventFinished = true;
        }

        void _abortEvent()
        {
            _playSound("aborted");
            _finishEvent();
            Utils.PrintDebug("_abortEvent called");
        }

        void _playSound(string soundcase = "item")
        {
            switch (soundcase)
            {
                case "cash":
                    break;

                case "saatana":
                    break;

                case "belching":
                    break;

                case "aborted":
                    break;
            }
        }

        void _destroyItems()
        {
            penta.DestroyItems();
            Utils.PrintDebug("_destroyItems called");
        }

        void _processEvents(string _event)
        {
            Utils.PrintDebug("_processEvents called");
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
                    });
                    return; // ▼ "spawn letter "RANDOM GIFT MONEY""

                case "fuel":
                    throw new NotImplementedException($"Event {_event} are not implemented");

                case "spoil":
                    /*_startEvent(() =>
                    {
                        // spoil all products
                    },
                    null, // grandma monolog audio source
                    (b) =>
                    {
                        if (b)
                        {
                            // move church grandma to penta & activate
                            return;
                        }
                        // reset church grandma position
                    });*/
                    throw new NotImplementedException($"Event {_event} are not implemented");

                case "hangover":
                    _startEvent(() =>
                    {
                        Hangover.GetVariable<FsmFloat>("HangoverStrenght").Value = 2;
                        Hangover.GetVariable<FsmFloat>("HangoverStrenghtMinus").Value = -2;
                        Hangover.GetVariable<FsmFloat>("TimeLeft").Value = 10;
                        Hangover.SendEvent("HANGOVER");

                        _playSound("saaatana");
                    });
                    return; // ▼ "activate hangover"

                case "fatigue":
                    _startEvent(() => PlayerFatigue.Value = 110f);
                    return; // ▼ "set fatigue to 100"

                case "hunger":
                    _startEvent(() => PlayerHunger.Value = 110f);
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
                    });
                    return; // ▼ "blow ALL fuses"

                case "knockout":
                    //_startEvent(() => Knockout.SendEvent("GLOBALEVENT"));
                    return; // ▼ "knockout player"

                case "bursttires":
                    throw new NotImplementedException($"Event {_event} are not implemented");

                case "outoffuel":
                    throw new NotImplementedException($"Event {_event} are not implemented");

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

        void _startEvent(Action func)
            => StartCoroutine(_eventCoroutine(func));

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
            _playSound("cash");
            Utils.PrintDebug("_spawnItem sound played");
        }

        IEnumerator _playFireAnimation(Action onFinish)
        {
            Utils.PrintDebug($"_playFireAnimation start enumerator {Time.time}");
            Fire.SetActive(true);
            Utils.PrintDebug($"_playFireAnimation fire activated {Time.time}");
            yield return new WaitForSeconds(2f);

            Utils.PrintDebug($"_playFireAnimation wait for 2 seconds {Time.time}");

            /*if (!penta.CheckItems())
            {
                Utils.PrintDebug("_playFireAnimation any items does not match the prescription");
                _abortEvent();
                Fire.SetActive(false);
                yield break;
            }*/

            onFinish?.Invoke();
            yield return new WaitForSeconds(2f);
            Utils.PrintDebug($"_playFireAnimation wait for 2 seconds {Time.time}");
            Fire.SetActive(false);
            Utils.PrintDebug($"_playFireAnimation fire deactivated {Time.time}");
        }

        IEnumerator _eventCoroutine(Action _action, AudioSource audio = null, Action<bool> _audioAction = null)
        {
            Utils.PrintDebug($"_eventCoroutine started {Time.time}");
            yield return StartCoroutine(_playFireAnimation(_destroyItems));
            Utils.PrintDebug($"_eventCoroutine fire animation played {Time.time}");

            if (_audioAction != null)
            {
                Utils.PrintDebug($"_eventCoroutine audio action call first {Time.time}");
                _audioAction?.Invoke(true);
            }

            _action?.Invoke();

            if (audio != null)
            {
                Utils.PrintDebug($"_eventCoroutine play audio source {Time.time}");
                audio.Play();
                while (audio.isPlaying)
                    yield return new WaitForSeconds(.5f);

                Utils.PrintDebug($"_eventCoroutine audio source finish playing {Time.time}");
            }

            if (_audioAction != null)
            {
                Utils.PrintDebug($"_eventCoroutine audio action call second {Time.time}");
                _audioAction?.Invoke(false);
            }

            _finishEvent();
            Utils.PrintDebug($"_spawnItem event finished {Time.time}");
            yield break;
        }
    }
}
