
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using Harmony;
using UnityEngine;

using Psycho.Internal;


namespace Psycho.Patches
{
    internal static class PsychoPatches
    {
        [HarmonyPriority(800)]
        internal static IEnumerable<CodeInstruction> SleepTrigger_CalculateSleep(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            foreach (var code in list)
            {
                if (code.opcode == OpCodes.Ldc_R4 && (float)code.operand == 0)
                {
                    var value = Mathf.Clamp(Globals.PlayerFatigue?.Value ?? 0 - Logic.Value, 0f, 100f);
                    code.operand = value;
                }

                yield return code;
            }

            yield break;
        }
    }
}
