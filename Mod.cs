using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace com.riskivr.MotionBlur;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalConfigProxy.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
internal class Mod : BaseUnityPlugin
{
    private readonly Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    internal static Mod Instance;
    internal static ManualLogSource logger;
    
    internal static ConfigEntry<float> Intensity;
    internal static ConfigEntry<int> SampleCount;
    internal static ConfigEntry<bool> Enabled;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;

        logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
        logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} is awake :3");
        
        harmony.PatchAll(typeof(StartOfRoundPatch));
        
        ConfigHelper.SkipAutoGen();
        
        Enabled = ConfigHelper.Bind("Motion Blur", "Enabled", defaultValue: true, false, "Enable Motion Blur");
        Intensity = ConfigHelper.Bind("Motion Blur", "Intensity", defaultValue: 1.2f, false, "Larger values results in longer exposures and increases the strength of the blur effect", new AcceptableValueRange<float>(0.1f, 2f));
        SampleCount = ConfigHelper.Bind("Motion Blur", "Sample Count", defaultValue: 12, false, "The number of sample points will affect the quality and performance of the blur effect", new AcceptableValueRange<int>(8, 32));
        
        Enabled.SettingChanged += (object sender, System.EventArgs e) => StartOfRoundPatch.ApplySettings();
        Intensity.SettingChanged += (object sender, System.EventArgs e) => StartOfRoundPatch.ApplySettings();
        SampleCount.SettingChanged += (object sender, System.EventArgs e) => StartOfRoundPatch.ApplySettings();
    }
}