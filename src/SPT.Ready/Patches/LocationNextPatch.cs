using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SPT.Ready.Patches;

internal sealed class LocationNextPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() => PatchTargets.LocationNext;

    [PatchTranspiler]
    [HarmonyPriority(Priority.Last)]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return TranspilerHelpers.RouteLocationNext(
            instructions,
            PatchTargets.ShowOfflineRaidSettings,
            PatchTargets.ShowConfirmation,
            PatchTargets.ContinueNext,
            nameof(LocationNextPatch));
    }
}
