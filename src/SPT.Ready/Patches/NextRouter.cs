using System;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;

namespace SPT.Ready.Patches;

internal static class NextRouter
{
    [ThreadStatic]
    private static int _directStartDepth;

    private static bool DirectStartActive => _directStartDepth > 0;

    public static void Continue(MainMenuShowOperation operation)
    {
        bool startImmediately = Plugin.StartRaidImmediately.Value;

        if (startImmediately)
        {
            _directStartDepth++;
        }

        try
        {
            operation.method_52();
        }
        finally
        {
            if (startImmediately)
            {
                _directStartDepth--;
            }
        }
    }

    public static void ShowConfirmationOrStart(
        MatchMakerAcceptScreen.MatchmakerAcceptScreenController controller,
        EScreenState screenState)
    {
        if (DirectStartActive)
        {
            controller.ShowNextScreen();
            return;
        }

        controller.ShowScreen(screenState);
    }
}
