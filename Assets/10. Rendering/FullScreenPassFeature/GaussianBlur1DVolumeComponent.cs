using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Post-processing Custom/GaussianBlur1D")]
[VolumeRequiresRendererFeatures(typeof(GaussianBlur1DRendererFeature))]
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
[DisplayInfo(name = "GaussianBlur1D")]
public sealed class GaussianBlur1DVolumeComponent : VolumeComponent, IPostProcessComponent
{
    [Tooltip("Blur radius. 0~1 normalized, remapped in renderer feature.")]
    public ClampedFloatParameter radius = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Final dimming amount. Applied only once in the final composite pass.")]
    public ClampedFloatParameter dimmed = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Compound blur mask amount. 0 = uniform blur, 1 = fully masked blur.")]
    public ClampedFloatParameter compound = new ClampedFloatParameter(0f, 0f, 1f);

    public bool IsActive()
    {
        return active && (radius.value > 0f || dimmed.value > 0f || compound.value > 0f);
    }
}