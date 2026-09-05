using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SPT.Ready.Patches;

internal static class TranspilerHelpers
{
    private const string SptScavLoaderType =
        "SPT.SinglePlayer.Patches.ScavMode.LoadOfflineRaidScreenPatch";

    private const string SptScavLoaderMethod = "LoadOfflineRaidScreenForScav";

    internal static IEnumerable<CodeInstruction> RouteLocationNext(
        IEnumerable<CodeInstruction> instructions,
        MethodInfo showOfflineRaidSettings,
        MethodInfo showConfirmation,
        MethodInfo continueNext,
        string patchName)
    {
        List<CodeInstruction> codes = instructions.ToList();
        int pmcRouteCount = 0;
        int scavRouteCount = 0;

        for (int index = 0; index < codes.Count; index++)
        {
            CodeInstruction instruction = codes[index];

            if (instruction.Calls(showOfflineRaidSettings))
            {
                ReplaceWithStaticCall(instruction, continueNext);
                pmcRouteCount++;
                continue;
            }

            if (instruction.Calls(showConfirmation))
            {
                ReplaceWithStaticCall(instruction, continueNext);
                scavRouteCount++;
                continue;
            }

            if (!IsSptScavOfflineLoader(instruction))
            {
                continue;
            }

            CodeInstruction loadOperation = new(OpCodes.Ldarg_0);
            instruction.MoveLabelsTo(loadOperation);
            instruction.MoveBlocksTo(loadOperation);
            codes.Insert(index, loadOperation);
            index++;

            ReplaceWithStaticCall(instruction, continueNext);
            scavRouteCount++;
        }

        if (pmcRouteCount != 1 || scavRouteCount != 1)
        {
            throw new InvalidOperationException(
                $"{patchName} expected one PMC route and one Scav route, " +
                $"but found {pmcRouteCount} PMC and {scavRouteCount} Scav routes.");
        }

        return codes;
    }

    internal static IEnumerable<CodeInstruction> ReplaceSingleCall(
        IEnumerable<CodeInstruction> instructions,
        Func<MethodInfo, bool> predicate,
        MethodInfo replacement,
        string expectedCall,
        string patchName)
    {
        List<CodeInstruction> codes = instructions.ToList();
        int replacementCount = 0;

        foreach (CodeInstruction instruction in codes)
        {
            if ((instruction.opcode != OpCodes.Call && instruction.opcode != OpCodes.Callvirt)
                || instruction.operand is not MethodInfo method
                || !predicate(method))
            {
                continue;
            }

            ReplaceWithStaticCall(instruction, replacement);
            replacementCount++;
        }

        if (replacementCount != 1)
        {
            throw new InvalidOperationException(
                $"{patchName} expected one call to {expectedCall}, " +
                $"but found {replacementCount}.");
        }

        return codes;
    }

    private static bool IsSptScavOfflineLoader(CodeInstruction instruction)
    {
        if (instruction.opcode != OpCodes.Call || instruction.operand is not MethodInfo method)
        {
            return false;
        }

        return method.IsStatic
            && method.Name == SptScavLoaderMethod
            && method.DeclaringType?.FullName == SptScavLoaderType
            && method.ReturnType == typeof(void)
            && method.GetParameters().Length == 0;
    }

    private static void ReplaceWithStaticCall(CodeInstruction instruction, MethodInfo replacement)
    {
        instruction.opcode = OpCodes.Call;
        instruction.operand = replacement;
    }
}
