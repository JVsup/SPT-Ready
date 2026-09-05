using BepInEx;
using BepInEx.Configuration;
using SPT.Ready.Patches;

namespace SPT.Ready;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency("com.SPT.core", "4.1.4")]
[BepInDependency("com.SPT.singleplayer", "4.1.4")]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.jvsup.ready";
    public const string PluginName = "SPT Ready";
    public const string PluginVersion = "4.1.0";

    internal static ConfigEntry<bool> StartRaidImmediately { get; private set; } = null!;

    private void Awake()
    {
        StartRaidImmediately = Config.Bind(
            "General",
            "Start raid immediately",
            false,
            "When enabled, Next on Select Location starts the raid without displaying Confirmation.");

        new LocationNextPatch().Enable();
        new ConfirmationRoutingPatch().Enable();

        Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }
}
