using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed class GaussianBlur1DRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private Material m_HorizontalMaterial;
    [SerializeField] private Material m_VerticalMaterial;

    // Final composite only: original + blurred + compound mask + dimming.
    [SerializeField] private Material m_CompositeMaterial;

    [SerializeField, Range(1, 4)]
    private int m_Downsample = 4;

    private CustomPostRenderPass m_FullScreenPass;

    public override void Create()
    {
        if (m_HorizontalMaterial != null &&
            m_VerticalMaterial != null &&
            m_CompositeMaterial != null)
        {
            m_FullScreenPass = new CustomPostRenderPass(
                name,
                m_HorizontalMaterial,
                m_VerticalMaterial,
                m_CompositeMaterial,
                m_Downsample
            );
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_HorizontalMaterial == null ||
            m_VerticalMaterial == null ||
            m_CompositeMaterial == null ||
            m_FullScreenPass == null)
            return;

        if (renderingData.cameraData.cameraType == CameraType.Preview ||
            renderingData.cameraData.cameraType == CameraType.Reflection)
            return;

        GaussianBlur1DVolumeComponent volume =
            VolumeManager.instance.stack?.GetComponent<GaussianBlur1DVolumeComponent>();

        if (volume == null || !volume.IsActive())
            return;

        m_FullScreenPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        // This pass samples the camera color, so request a readable intermediate color texture.
        m_FullScreenPass.ConfigureInput(ScriptableRenderPassInput.Color);
        renderer.EnqueuePass(m_FullScreenPass);
    }

    protected override void Dispose(bool disposing)
    {
        m_FullScreenPass?.Dispose();
    }

    private class CustomPostRenderPass : ScriptableRenderPass
    {
        private readonly Material m_HorizontalMaterial;
        private readonly Material m_VerticalMaterial;
        private readonly Material m_CompositeMaterial;
        private readonly int m_Downsample;

#if URP_COMPATIBILITY_MODE
        private RTHandle m_Original;
        private RTHandle m_TempPing;
        private RTHandle m_TempPong;
#endif

        private static readonly MaterialPropertyBlock s_SharedPropertyBlock = new MaterialPropertyBlock();

        private static readonly int kBlitTexturePropertyId = Shader.PropertyToID("_BlitTexture");
        private static readonly int kOriginalTexturePropertyId = Shader.PropertyToID("_OriginalTexture");
        private static readonly int kBlitScaleBiasPropertyId = Shader.PropertyToID("_BlitScaleBias");
        private static readonly int kRadiusPropertyId = Shader.PropertyToID("_Radius");
        private static readonly int kDimmedPropertyId = Shader.PropertyToID("_Dimmed");
        private static readonly int kCompoundPropertyId = Shader.PropertyToID("_Compound");

        public CustomPostRenderPass(
            string passName,
            Material horizontalMaterial,
            Material verticalMaterial,
            Material compositeMaterial,
            int downsample)
        {
            profilingSampler = new ProfilingSampler(passName);
            m_HorizontalMaterial = horizontalMaterial;
            m_VerticalMaterial = verticalMaterial;
            m_CompositeMaterial = compositeMaterial;
            m_Downsample = Mathf.Max(1, downsample);
            requiresIntermediateTexture = true;
        }

        private class BlurPassData
        {
            public Material material;
            public TextureHandle inputTexture;
            public float radius;
        }

        private class CopyPassData
        {
            public TextureHandle inputTexture;
        }

        private class CompositePassData
        {
            public Material material;
            public TextureHandle blurredTexture;
            public TextureHandle originalTexture;
            public float dimmed;
            public float compound;
        }

        private static float GetProcessedRadius(float normalizedRadius)
        {
            normalizedRadius = Mathf.Clamp01(normalizedRadius);
            float easedRadius = Mathf.SmoothStep(0f, 1f, normalizedRadius);
            return easedRadius * 7f;
        }

        private static float GetProcessedDimmed(float normalizedDimmed)
        {
            return Mathf.Clamp01(normalizedDimmed);
        }

        private static float GetProcessedCompound(float normalizedCompound)
        {
            return Mathf.Clamp01(normalizedCompound);
        }

        private static void ExecuteBlurPass(BlurPassData data, RasterGraphContext context)
        {
            s_SharedPropertyBlock.Clear();
            s_SharedPropertyBlock.SetTexture(kBlitTexturePropertyId, data.inputTexture);
            s_SharedPropertyBlock.SetVector(kBlitScaleBiasPropertyId, new Vector4(1, 1, 0, 0));
            s_SharedPropertyBlock.SetFloat(kRadiusPropertyId, data.radius);

            context.cmd.DrawProcedural(
                Matrix4x4.identity,
                data.material,
                0,
                MeshTopology.Triangles,
                3,
                1,
                s_SharedPropertyBlock
            );
        }

        private static void ExecuteCopyPass(CopyPassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(
                context.cmd,
                data.inputTexture,
                new Vector4(1, 1, 0, 0),
                0.0f,
                false
            );
        }

        private static void ExecuteCompositePass(CompositePassData data, RasterGraphContext context)
        {
            s_SharedPropertyBlock.Clear();
            s_SharedPropertyBlock.SetTexture(kBlitTexturePropertyId, data.blurredTexture);
            s_SharedPropertyBlock.SetTexture(kOriginalTexturePropertyId, data.originalTexture);
            s_SharedPropertyBlock.SetVector(kBlitScaleBiasPropertyId, new Vector4(1, 1, 0, 0));
            s_SharedPropertyBlock.SetFloat(kDimmedPropertyId, data.dimmed);
            s_SharedPropertyBlock.SetFloat(kCompoundPropertyId, data.compound);

            context.cmd.DrawProcedural(
                Matrix4x4.identity,
                data.material,
                0,
                MeshTopology.Triangles,
                3,
                1,
                s_SharedPropertyBlock
            );
        }

#if URP_COMPATIBILITY_MODE
        [System.Obsolete]
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ResetTarget();

            RenderTextureDescriptor fullResDesc = renderingData.cameraData.cameraTargetDescriptor;
            fullResDesc.msaaSamples = 1;
            fullResDesc.depthBufferBits = 0;

            RenderingUtils.ReAllocateHandleIfNeeded(
                ref m_Original,
                fullResDesc,
                name: "_GaussianBlurOriginal"
            );

            RenderTextureDescriptor lowResDesc = GetLowResDescriptor(fullResDesc);

            RenderingUtils.ReAllocateHandleIfNeeded(
                ref m_TempPing,
                lowResDesc,
                name: "_GaussianBlurPing"
            );

            RenderingUtils.ReAllocateHandleIfNeeded(
                ref m_TempPong,
                lowResDesc,
                name: "_GaussianBlurPong"
            );
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            GaussianBlur1DVolumeComponent volume =
                VolumeManager.instance.stack?.GetComponent<GaussianBlur1DVolumeComponent>();

            if (volume == null || !volume.IsActive())
                return;

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, profilingSampler))
            {
                RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

                float processedRadius = GetProcessedRadius(volume.radius.value);
                float processedDimmed = GetProcessedDimmed(volume.dimmed.value);
                float processedCompound = GetProcessedCompound(volume.compound.value);

                // Preserve the full-resolution original before any blur work.
                Blitter.BlitCameraTexture(cmd, source, m_Original);

                RTHandle finalBlurred = m_Original;

                if (processedRadius > 0f)
                {
                    // Downsample from the preserved original.
                    Blitter.BlitCameraTexture(cmd, m_Original, m_TempPing);

                    m_HorizontalMaterial.SetFloat(kRadiusPropertyId, processedRadius);
                    m_VerticalMaterial.SetFloat(kRadiusPropertyId, processedRadius);

                    Blitter.BlitCameraTexture(cmd, m_TempPing, m_TempPong, m_HorizontalMaterial, 0);
                    Blitter.BlitCameraTexture(cmd, m_TempPong, m_TempPing, m_VerticalMaterial, 0);

                    Blitter.BlitCameraTexture(cmd, m_TempPing, m_TempPong, m_HorizontalMaterial, 0);
                    Blitter.BlitCameraTexture(cmd, m_TempPong, m_TempPing, m_VerticalMaterial, 0);

                    Blitter.BlitCameraTexture(cmd, m_TempPing, m_TempPong, m_HorizontalMaterial, 0);
                    Blitter.BlitCameraTexture(cmd, m_TempPong, m_TempPing, m_VerticalMaterial, 0);

                    finalBlurred = m_TempPing;
                }

                // Blitter binds finalBlurred to _BlitTexture.
                // The original texture and scalar controls are supplied separately.
                m_CompositeMaterial.SetTexture(kOriginalTexturePropertyId, m_Original);
                m_CompositeMaterial.SetFloat(kCompoundPropertyId, processedCompound);
                m_CompositeMaterial.SetFloat(kDimmedPropertyId, processedDimmed);

                Blitter.BlitCameraTexture(cmd, finalBlurred, source, m_CompositeMaterial, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        private RenderTextureDescriptor GetLowResDescriptor(RenderTextureDescriptor desc)
        {
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            desc.width = Mathf.Max(1, desc.width / m_Downsample);
            desc.height = Mathf.Max(1, desc.height / m_Downsample);
            return desc;
        }
#endif

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void Dispose()
        {
#if URP_COMPATIBILITY_MODE
            m_Original?.Release();
            m_TempPing?.Release();
            m_TempPong?.Release();
#endif
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            GaussianBlur1DVolumeComponent volume =
                VolumeManager.instance.stack?.GetComponent<GaussianBlur1DVolumeComponent>();

            if (volume == null || !volume.IsActive())
                return;

            float processedRadius = GetProcessedRadius(volume.radius.value);
            float processedDimmed = GetProcessedDimmed(volume.dimmed.value);
            float processedCompound = GetProcessedCompound(volume.compound.value);

            UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();

            // A back buffer cannot be sampled as a normal input texture in this raster pass.
            // ConfigureInput(Color) normally forces an intermediate color target; this guard
            // prevents an invalid read if the renderer still exposes the back buffer here.
            if (resourcesData.isActiveTargetBackBuffer)
                return;

            TextureHandle source = resourcesData.activeColorTexture;

            // Keep a full-resolution copy of the original camera color. The final composite
            // reads this copy while writing back to the active camera color target.
            var fullResDesc = renderGraph.GetTextureDesc(source);
            fullResDesc.clearBuffer = false;
            fullResDesc.msaaSamples = MSAASamples.None;
            fullResDesc.depthBufferBits = 0;
            fullResDesc.name = "_GaussianBlurOriginal";
            TextureHandle original = renderGraph.CreateTexture(fullResDesc);

            using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>(
                "GaussianBlur Copy Original",
                out var passData))
            {
                passData.inputTexture = source;

                builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                builder.SetRenderAttachment(original, 0, AccessFlags.Write);
                builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) =>
                    ExecuteCopyPass(data, context));
            }

            TextureHandle finalBlurred = original;

            if (processedRadius > 0f)
            {
                var lowResDesc = fullResDesc;
                lowResDesc.width = Mathf.Max(1, lowResDesc.width / m_Downsample);
                lowResDesc.height = Mathf.Max(1, lowResDesc.height / m_Downsample);

                lowResDesc.name = "_GaussianBlurPing";
                TextureHandle ping = renderGraph.CreateTexture(lowResDesc);

                lowResDesc.name = "_GaussianBlurPong";
                TextureHandle pong = renderGraph.CreateTexture(lowResDesc);

                // Downsample: full-resolution original -> low-resolution ping.
                using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>(
                    "GaussianBlur Downsample",
                    out var passData))
                {
                    passData.inputTexture = original;

                    builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(ping, 0, AccessFlags.Write);
                    builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) =>
                        ExecuteCopyPass(data, context));
                }

                AddBlurPass(renderGraph, "GaussianBlur Horizontal 1", m_HorizontalMaterial, ping, pong, processedRadius);
                AddBlurPass(renderGraph, "GaussianBlur Vertical 1", m_VerticalMaterial, pong, ping, processedRadius);
                AddBlurPass(renderGraph, "GaussianBlur Horizontal 2", m_HorizontalMaterial, ping, pong, processedRadius);
                AddBlurPass(renderGraph, "GaussianBlur Vertical 2", m_VerticalMaterial, pong, ping, processedRadius);
                AddBlurPass(renderGraph, "GaussianBlur Horizontal 3", m_HorizontalMaterial, ping, pong, processedRadius);
                AddBlurPass(renderGraph, "GaussianBlur Vertical 3", m_VerticalMaterial, pong, ping, processedRadius);

                finalBlurred = ping;
            }

            // Final composite: read preserved original + blurred image, write only to source.
            // Compound must be applied here, not inside each Gaussian pass.
            using (var builder = renderGraph.AddRasterRenderPass<CompositePassData>(
                "GaussianBlur Composite",
                out var passData))
            {
                passData.material = m_CompositeMaterial;
                passData.blurredTexture = finalBlurred;
                passData.originalTexture = original;
                passData.dimmed = processedDimmed;
                passData.compound = processedCompound;

                builder.UseTexture(passData.originalTexture, AccessFlags.Read);
                if (processedRadius > 0f)
                    builder.UseTexture(passData.blurredTexture, AccessFlags.Read);

                builder.SetRenderAttachment(source, 0, AccessFlags.Write);
                builder.SetRenderFunc((CompositePassData data, RasterGraphContext context) =>
                    ExecuteCompositePass(data, context));
            }
        }

        private void AddBlurPass(
            RenderGraph renderGraph,
            string passName,
            Material material,
            TextureHandle input,
            TextureHandle output,
            float radius)
        {
            using (var builder = renderGraph.AddRasterRenderPass<BlurPassData>(passName, out var passData))
            {
                passData.material = material;
                passData.inputTexture = input;
                passData.radius = radius;

                builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                builder.SetRenderAttachment(output, 0, AccessFlags.Write);
                builder.SetRenderFunc((BlurPassData data, RasterGraphContext context) =>
                    ExecuteBlurPass(data, context));
            }
        }
    }
}
