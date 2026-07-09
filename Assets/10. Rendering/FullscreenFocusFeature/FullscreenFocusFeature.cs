using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class FullscreenFocusFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public bool showInSceneView = false;
    }

    [SerializeField] private Settings settings = new Settings();

    private FocusMaskPass pass;

    public override void Create()
    {
        pass = new FocusMaskPass();
        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        if (!settings.showInSceneView &&
            renderingData.cameraData.cameraType == CameraType.SceneView)
            return;

        pass.Setup(settings.material);
        pass.renderPassEvent = settings.renderPassEvent;
        renderer.EnqueuePass(pass);
    }

    private class FocusMaskPass : ScriptableRenderPass
    {
        private const string PassName = "Fullscreen Focus Mask";

        private Material material;

        public void Setup(Material material)
        {
            this.material = material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null)
                return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            TextureHandle source = resourceData.activeColorTexture;

            TextureDesc destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "_FullscreenFocusMaskResult";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            var blitParameters = new RenderGraphUtils.BlitMaterialParameters(
                source,
                destination,
                material,
                0
            );

            renderGraph.AddBlitPass(blitParameters, PassName);

            resourceData.cameraColor = destination;
        }
    }
}