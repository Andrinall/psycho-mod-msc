using System.Linq;
using System.Collections.Generic;

using Harmony;
using MSCLoader;
using HealthMod;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Adrenaline
{
    internal class GlobalHandler : MonoBehaviour
    {
        private GameObject kiljuguy;
        private Transform HouseFire;

        private FsmFloat PlayerMovementSpeed;
        private FsmBool HouseBurningState;
        private FsmInt GlobalDay;

        private void Awake()
        {
            try
            {
                AdrenalineLogic._hud = GameObject.Find("GUI/HUD").AddComponent<FixedHUD>();
                AdrenalineLogic._hud.AddElement(eHUDCloneType.RECT, "Adrenaline", AdrenalineLogic._hud.GetIndexByName("Money"));
                Utils.PrintDebug(eConsoleColors.GREEN, "HUD Enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to init HUD bar");
            }

            try
            {
                PlayerMovementSpeed = Utils.GetGlobalVariable<FsmFloat>("PlayerMovementSpeed");
                HouseBurningState = Utils.GetGlobalVariable<FsmBool>("HouseBurning");
                GlobalDay = Utils.GetGlobalVariable<FsmInt>("GlobalDay");
                HouseFire = GameObject.Find("YARD/Building/HOUSEFIRE").transform;
                kiljuguy = GameObject.Find("KILJUGUY");

                if (kiljuguy.transform.childCount > 3)
                    kiljuguy.transform.Find("KiljuMurderer").gameObject.AddComponent<KiljuMurdererHandler>();

                var fridge_paper = GameObject.Find("fridge_paper");
                var fpsCamera = base.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera");
                var drink = fpsCamera.Find("Drink").gameObject;
                GameHook.InjectStateHook(drink, "Activate 5", IncreaseByEnergyDrink);
                GameHook.InjectStateHook(drink, "Activate 7", IncreaseByCoffee);
                GameHook.InjectStateHook(drink, "HomeCoffee", IncreaseByCoffee);
                GameHook.InjectStateHook(fridge_paper, "Use", "Wait button", SetFridgePaperText, true);

                Utils.SetMaterial(fridge_paper, 0, "ATLAS_OFFICE(Clone)", AdrenalineLogic.atlas_texture, Vector2.zero, Vector2.one);
                Utils.PrintDebug(eConsoleColors.GREEN, "GlobalHandler enabled");
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load GlobalHandler component");
            }

            try
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Loading component FITTAN::HighSpeedHandler");
                Resources.FindObjectsOfTypeAll<Transform>().Where(v => v.name == "FITTAN") // avoid error (scene contains two fittan objects)
                    .Where(v => v.GetComponents<PlayMakerFSM>().Length == 4).ToArray()[0].gameObject
                    .AddComponent<HighSpeedHandler>();
            }
            catch
            {
                Utils.PrintDebug(eConsoleColors.RED, "Unable to load HighSpeedHandler component for FITTAN");
            }

            try
            {
                Utils.PrintDebug(eConsoleColors.YELLOW, "Loading AUDIO loops");

                var player = GameObject.Find("PLAYER");
                foreach (var clip in AdrenalineLogic.clips)
                {
                    if (clip.name == "heart_stop")
                    {
                        var src = GameObject.Find("Systems").AddComponent<AudioSource>();
                        src.clip = clip;
                        src.loop = false;
                        src.volume = 1.75f;
                        src.priority = 0;
                        AdrenalineLogic.audios.Add(src);
                        Utils.PrintDebug(eConsoleColors.GREEN, "AudioSource {0} created", src.clip.name);
                        continue;
                    }

                    var source = player.AddComponent<AudioSource>();
                    source.clip = clip;
                    source.loop = true;
                    source.volume = 2f;
                    AdrenalineLogic.audios.Add(source);
                    Utils.PrintDebug(eConsoleColors.GREEN, "AudioSource {0} created", source.clip.name);
                }

                Utils.PrintDebug("AUDIO loops loaded & started");
            }
            catch (System.Exception e)
            {
                Utils.PrintDebug("AUDIO loops loading failed: {0}", e.GetFullMessage());
            }

            try
            {
                SpawnAllPills();
                Utils.PrintDebug("Pills spawned");
            }
            catch
            {
                Utils.PrintDebug("Unable to spawn pills");
            }

            if (AdrenalineLogic.LastDayUpdated == -1)
                AdrenalineLogic.LastDayUpdated = GlobalDay.Value;
        }

        private void OnDestroy()
        {
            Destroy(AdrenalineLogic._hud);
        }

        private void FixedUpdate()
        {
            AdrenalineLogic.Tick();

            if (PlayerMovementSpeed.Value >= 3.5)
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("SPRINT_INCREASE")); // increase adrenaline while player sprinting

            if (HouseBurningState.Value == true)
            {
                if (Vector3.Distance(HouseFire.position, transform.position) > 6f) return;
                AdrenalineLogic.IncreaseTimed(AdrenalineLogic.config.GetValueSafe("HOUSE_BURNING")); // increase adrenaline while house is burning
                return;
            }

            if (GlobalDay.Value != AdrenalineLogic.LastDayUpdated)
            {
            TryRandom:
                var disabled = AdrenalineLogic.pills_list.Where(v => !v.activeSelf).ToList();
                var idx = Random.Range(0, disabled.Count);
                var element = disabled.ElementAtOrDefault(idx);
                if (element.activeSelf) goto TryRandom;
                element.SetActive(true);

                var mailFromDoctor = GameObject.Find("YARD/PlayerMailBox/EnvelopeDoctor");
                mailFromDoctor.SetActive(true);
                mailFromDoctor.GetComponent<PlayMakerFSM>().enabled = true;

                Utils.PrintDebug("Day updated from {0} to {1}", AdrenalineLogic.LastDayUpdated, GlobalDay.Value);
                AdrenalineLogic.LastDayUpdated = GlobalDay.Value;
            }
        }

        private void IncreaseByEnergyDrink()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("ENERGY_DRINK_INCREASE"));
            AdrenalineLogic.SetDecreaseLocked(true, 12000f);
            if (ModLoader.IsModPresent("Health")) Health.editHp(-4f);
        }

        private void IncreaseByCoffee()
        {
            AdrenalineLogic.IncreaseOnce(AdrenalineLogic.config.GetValueSafe("COFFEE_INCREASE"));
            AdrenalineLogic.SetDecreaseLocked(true, 12000f); // 1 minute
        }

        private void SetFridgePaperText()
        {
            Utils.GetGlobalVariable<FsmString>("GUIsubtitle").Value = "Hi! You are sick and need to keep your adrenaline high! Drink coffee, drive fast, risk your life, conflict with society.\nYou can buy an energy drink at the Nappo pub.You will die if your adrenaline drops to zero or gets too high.\nLove, Doctor.";
        }

        private void SpawnAllPills()
        {
            var list = new List<Vector3>
            {
                new Vector3(462.2887f, 9.311339f, 1320.133f),
                new Vector3(1353.568f, 5.9235f, 821.1407f),
                new Vector3(1396f, 4.59463f, 850.6066f),
                new Vector3(1360.994f, 7.37431f, 805.0872f),
                new Vector3(1373.052f, 9.094034f, 795.0477f),
                new Vector3(1366.354f, 10.09609f, 798.3608f),
                new Vector3(1363.531f, 11.67623f, 803.7286f),
                new Vector3(1444.064f, -4.52272f, 722.709f),
                new Vector3(1559.36f, 4.486989f, 719.0803f),
                new Vector3(1548.925f, 4.417933f, 741.9292f),
                new Vector3(1598.357f, 3.662708f, 645.6377f),
                new Vector3(1790.758f, 3.977903f, 556.7726f),
                new Vector3(1796.5f, 3.642186f, 40.22343f),
                new Vector3(2168.991f, -1.790677f, 61.94064f),
                new Vector3(1932.791f, 6.842865f, -223.3672f),
                new Vector3(1895.464f, 5.256322f, -411.0562f),
                new Vector3(1893.25f, -3.624627f, -781.93f),
                new Vector3(1064.38f, 5.352709f, -910.4662f),
                new Vector3(664.62f, 0.9491397f, -1219.598f),
                new Vector3(454.0934f, 2.124174f, -1322.424f),
                new Vector3(286.9083f, 0.7398606f, -1269.742f),
                new Vector3(105.4728f, 4.87885f, -1357.449f),
                new Vector3(-187.5422f, 2.033351f, -942.8172f),
                new Vector3(-1196.174f, 0.1953767f, -625.333f),
                new Vector3(-1311.078f, -1.490561f, -618.7657f),
                new Vector3(-1495.032f, 26.79742f, -349.7293f),
                new Vector3(-669.2556f, 7.799858f, -721.1388f),
                new Vector3(-1271.826f, 2.043516f, -961.0248f),
                new Vector3(-240.0972f, 2.171151f, -1061.969f),
                new Vector3(-2014.459f, 105.964f, -144.1324f),
                new Vector3(-2004.393f, 68.72627f, -103.7944f),
                new Vector3(-1766.836f, 6.30492f, -353.0125f),
                new Vector3(-1619.084f, 1.936041f, 481.3835f),
                new Vector3(-1722.345f, 3.172916f, 952.5532f),
                new Vector3(-1516.232f, 7.27427f, 1369.108f),
                new Vector3(-1517.137f, 3.151923f, 1241.793f),
                new Vector3(-1409.24f, 5.365961f, 1139.492f),
                new Vector3(-1387.699f, 3.106995f, 1261.727f),
                new Vector3(-1274.975f, -0.3430491f, 1084.578f),
                new Vector3(-1118.886f, 0.650418f, 1312.829f),
                new Vector3(-845.4117f, 1.910222f, 1332.593f),
                new Vector3(-335.2787f, 0.4752745f, 1372.508f),
                new Vector3(-164.7237f, -3.985549f, 1006.097f),
                new Vector3(-180.117f, -3.341475f, 1022.654f),
                new Vector3(-785.0475f, 7.041798f, 1710.269f),
                new Vector3(1469.62f, -4.294517f, 1070.209f),
                new Vector3(2187.362f, -0.6239323f, -454.2437f),
                new Vector3(1426.629f, -4.249069f, 751.5843f)
            };

            list.ForEach(v => {
                var item = new PillsItem(v).self;
                AdrenalineLogic.pills_list.Add(item);
                item.SetActive(false);
            });
        }
    }
}
