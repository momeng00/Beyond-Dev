using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Post-processing Custom/GaussianBlur1D")]
[VolumeRequiresRendererFeatures(typeof(GaussianBlur1DRendererFeature))]
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
[DisplayInfo(name = "GaussianBlur1D")]
public sealed class GaussianBlur1DVolumeComponent : VolumeComponent, IPostProcessComponent
{
    [Tooltip("Blur radius.")]
    public ClampedFloatParameter radius = new ClampedFloatParameter(0f, 0f, 1f);

    public bool IsActive()
    {
        return active && radius.value > 0f;
    }
}