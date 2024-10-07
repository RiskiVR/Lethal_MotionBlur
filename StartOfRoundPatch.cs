using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace com.riskivr.MotionBlur;
[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch
{
    static Volume postprocess;
    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void StartPatch()
    {
        HDRenderPipelineAsset assetHDRP = QualitySettings.renderPipeline as HDRenderPipelineAsset;
        if (assetHDRP == null) return;
        
        RenderPipelineSettings settings = assetHDRP.currentPlatformRenderPipelineSettings;
        settings.supportMotionVectors = true;
        assetHDRP.currentPlatformRenderPipelineSettings = settings;
        
        postprocess = GameObject.Find("Systems/Rendering/VolumeMain").GetComponent<Volume>();
        ApplySettings();
    }
    public static void ApplySettings()
    {
        if (postprocess == null) return;
        if (postprocess.profile.TryGet(out UnityEngine.Rendering.HighDefinition.MotionBlur mb))
        {
            mb.intensity.value = Mod.Intensity.Value;
            mb.sampleCount = Mod.SampleCount.Value;
            mb.active = Mod.Enabled.Value;
        }
        Mod.logger.LogInfo("Settings Applied! :3");
    }
}