using System;
using System.Reflection;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using HarmonyLib;

namespace SPT.Ready.Patches;

internal static class PatchTargets
{
    internal static readonly MethodInfo LocationNext = RequireMethod(
        typeof(MainMenuShowOperation),
        nameof(MainMenuShowOperation.CG_method_77));

    internal static readonly MethodInfo ShowOfflineRaidSettings = RequireMethod(
        typeof(MainMenuShowOperation),
        nameof(MainMenuShowOperation.method_50));

    internal static readonly MethodInfo ShowConfirmation = RequireMethod(
        typeof(MainMenuShowOperation),
        nameof(MainMenuShowOperation.method_52));

    internal static readonly MethodInfo ContinueNext = RequireMethod(
        typeof(NextRouter),
        nameof(NextRouter.Continue),
        typeof(MainMenuShowOperation));

    internal static readonly MethodInfo PresentConfirmationOrStart = RequireMethod(
        typeof(NextRouter),
        nameof(NextRouter.ShowConfirmationOrStart),
        typeof(MatchMakerAcceptScreen.MatchmakerAcceptScreenController),
        typeof(EScreenState));

    private static MethodInfo RequireMethod(Type type, string name, params Type[] parameterTypes)
    {
        MethodInfo? method = AccessTools.Method(type, name, parameterTypes);
        return method ?? throw new MissingMethodException(type.FullName, name);
    }
}
