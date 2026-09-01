using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Post-processing Custom/GaussianBlur1D")]
[VolumeRequiresRendererFeatures(typeof(GaussianBlur1DRendererFeature))]
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
[DisplayInfo(name = "GaussianBlur1D")]
public sealed class GaussianBlur1DVolumeComponent : VolumeComponent, IPostProcessComponent
{
    [Tooltip("Blur radius. 0~1 normalized, remapped in the renderer feature.")]
    public ClampedFloatParameter radius = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Final dimming amount. Applied once in the final composite pass.")]
    public ClampedFloatParameter dimmed = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Compound masking amount. 0 = uniform full-screen blur, 1 = edge-masked blur. Applied only in the final composite pass.")]
    public ClampedFloatParameter compound = new ClampedFloatParameter(0f, 0f, 1f);

    public bool IsActive()
    {
        // Compound modifies an existing blur; it should not activate the pass by itself.
        return active && (radius.value > 0f || dimmed.value > 0f);
    }
}
