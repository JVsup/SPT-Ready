using System;
using System.Collections.Generic;
using System.Reflection;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SPT.Ready.Patches;

internal sealed class ConfirmationRoutingPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() => PatchTargets.ShowConfirmation;

    [PatchTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return TranspilerHelpers.ReplaceSingleCall(
            instructions,
            IsConfirmationPresentation,
            PatchTargets.PresentConfirmationOrStart,
            "the inherited Confirmation ShowScreen(EScreenState) call",
            nameof(ConfirmationRoutingPatch));
    }

    private static bool IsConfirmationPresentation(MethodInfo method)
    {
        ParameterInfo[] parameters = method.GetParameters();
        Type? declaringType = method.DeclaringType;

        return method.Name == "ShowScreen"
            && method.ReturnType == typeof(void)
            && parameters.Length == 1
            && parameters[0].ParameterType == typeof(EScreenState)
            && declaringType != null
            && declaringType.IsAssignableFrom(
                typeof(MatchMakerAcceptScreen.MatchmakerAcceptScreenController));
    }
}
