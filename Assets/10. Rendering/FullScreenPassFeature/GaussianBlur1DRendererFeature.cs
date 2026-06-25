using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed class GaussianBlur1DRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private Material m_HorizontalMaterial;
    [SerializeField] private Material m_VerticalMaterial;

    // 마지막 1회만 dimmed를 적용하는 합성용 머티리얼
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

        GaussianBlur1DVolumeComponent volume = VolumeManager.instance.stack?.GetComponent<GaussianBlur1DVolumeComponent>();
        if (volume == null || !volume.IsActive())
            return;

        m_FullScreenPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        m_FullScreenPass.ConfigureInput(ScriptableRenderPassInput.None);
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
        private RTHandle m_TempPing;
        private RTHandle m_TempPong;
#endif

        private static readonly MaterialPropertyBlock s_SharedPropertyBlock = new MaterialPropertyBlock();

        private static readonly int kBlitTexturePropertyId = Shader.PropertyToID("_BlitTexture");
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
            public float compound;
        }

        private class CopyPassData
        {
            public TextureHandle inputTexture;
        }

        private class CompositePassData
        {
            public Material material;
            public TextureHandle inputTexture;
            public float dimmed;
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

        private static void ExecuteCopyPass(CopyPassData data, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), 0.0f, false);
        }

        private static void ExecuteCompositePass(CompositePassData data, RasterGraphContext context)
        {
            s_SharedPropertyBlock.Clear();
            s_SharedPropertyBlock.SetTexture(kBlitTexturePropertyId, data.inputTexture);
            s_SharedPropertyBlock.SetVector(kBlitScaleBiasPropertyId, new Vector4(1, 1, 0, 0));
            s_SharedPropertyBlock.SetFloat(kDimmedPropertyId, data.dimmed);

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

            var desc = GetCompatibleDescriptor(renderingData.cameraData.cameraTargetDescriptor);

            RenderingUtils.ReAllocateHandleIfNeeded(
                ref m_TempPing,
                desc,
                name: "_GaussianBlurPing"
            );

            RenderingUtils.ReAllocateHandleIfNeeded(
                ref m_TempPong,
                desc,
                name: "_GaussianBlurPong"
            );
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var volume = VolumeManager.instance.stack?.GetComponent<GaussianBlur1DVolumeComponent>();
            if (volume == null || !volume.IsActive())
                return;

            var cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, profilingSampler))
            {
                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

                float processedRadius = GetProcessedRadius(volume.radius.value);
                float processedDimmed = GetProcessedDimmed(volume.dimmed.value);
                float processedCompound = GetProcessedCompound(volume.compound.value);

                m_HorizontalMaterial.SetFloat(kRadiusPropertyId, processedRadius);
                m_VerticalMaterial.SetFloat(kRadiusPropertyId, processedRadius);

                m_HorizontalMaterial.SetFloat(kCompoundPropertyId, processedCompound);
                m_VerticalMaterial.SetFloat(kCompoundPropertyId, processedCompound);

                m_CompositeMaterial.SetFloat(kDimmedPropertyId, processedDimmed);

                // 1. Downsample copy
                Blitter.BlitCameraTexture(cmd, source, m_TempPing);

                // radius가 0보다 크면 블러 수행
                if (processedRadius > 0f)
                {
                    // 2. Horizontal blur 1
                    Blitter.BlitCameraTexture(cmd, m_TempPing, m_TempPong, m_HorizontalMaterial, 0);

                    // 3. Vertical blur 1
                    Blitter.BlitCameraTexture(cmd, m_TempPong, m_TempPing, m_VerticalMaterial, 0);

                    // 4. Horizontal blur 2
                    Blitter.BlitCameraTexture(cmd, m_TempPing, m_TempPong, m_HorizontalMaterial, 0);

                    // 5. Vertical blur 2
                    Blitter.BlitCameraTexture(cmd, m_TempPong, m_TempPing, m_VerticalMaterial, 0);

                    // 6. Horizontal blur 3
                    Blitter.BlitCameraTexture(cmd, m_TempPing, m_TempPong, m_HorizontalMaterial, 0);

                    // 7. Vertical blur 3
                    Blitter.BlitCameraTexture(cmd, m_TempPong, m_TempPing, m_VerticalMaterial, 0);
                }

                // 8. Final composite
                // 블러가 있었다면 m_TempPing, 없었다면 다운샘플된 m_TempPing 그대로 사용
                Blitter.BlitCameraTexture(cmd, m_TempPing, source, m_CompositeMaterial, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        private RenderTextureDescriptor GetCompatibleDescriptor(RenderTextureDescriptor desc)
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
            m_TempPing?.Release();
            m_TempPong?.Release();
#endif
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var volume = VolumeManager.instance.stack?.GetComponent<GaussianBlur1DVolumeComponent>();
            if (volume == null || !volume.IsActive())
                return;

            float processedRadius = GetProcessedRadius(volume.radius.value);
            float processedDimmed = GetProcessedDimmed(volume.dimmed.value);
            float processedCompound = GetProcessedCompound(volume.compound.value);

            UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();
            TextureHandle source = resourcesData.cameraColor;

            var desc = renderGraph.GetTextureDesc(source);
            desc.clearBuffer = false;
            desc.msaaSamples = MSAASamples.None;
            desc.depthBufferBits = 0;
            desc.width = Mathf.Max(1, desc.width / m_Downsample);
            desc.height = Mathf.Max(1, desc.height / m_Downsample);

            desc.name = "_GaussianBlurPing";
            TextureHandle ping = renderGraph.CreateTexture(desc);

            desc.name = "_GaussianBlurPong";
            TextureHandle pong = renderGraph.CreateTexture(desc);

            // 1. Downsample copy: full-res source -> low-res ping
            using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("GaussianBlur Downsample", out var passData))
            {
                passData.inputTexture = source;

                builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                builder.SetRenderAttachment(ping, 0, AccessFlags.Write);
                builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) => ExecuteCopyPass(data, context));
            }

            TextureHandle finalBlurred = ping;

            if (processedRadius > 0f)
            {
                // 2. Horizontal blur 1: ping -> pong
                using (var builder = renderGraph.AddRasterRenderPass<BlurPassData>("GaussianBlur Horizontal 1", out var passData))
                {
                    passData.material = m_HorizontalMaterial;
                    passData.inputTexture = ping;
                    passData.radius = processedRadius;
                    passData.compound = processedCompound;

                    builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(pong, 0, AccessFlags.Write);
                    builder.SetRenderFunc((BlurPassData data, RasterGraphContext context) => ExecuteBlurPass(data, context));
                }

                // 3. Vertical blur 1: pong -> ping
                using (var builder = renderGraph.AddRasterRenderPass<BlurPassData>("GaussianBlur Vertical 1", out var passData))
                {
                    passData.material = m_VerticalMaterial;
                    passData.inputTexture = pong;
                    passData.radius = processedRadius;
                    passData.compound = processedCompound;

                    builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(ping, 0, AccessFlags.Write);
                    builder.SetRenderFunc((BlurPassData data, RasterGraphContext context) => ExecuteBlurPass(data, context));
                }

                // 4. Horizontal blur 2: ping -> pong
                using (var builder = renderGraph.AddRasterRenderPass<BlurPassData>("GaussianBlur Horizontal 2", out var passData))
                {
                    passData.material = m_HorizontalMaterial;
                    passData.inputTexture = ping;
                    passData.radius = processedRadius;
                    passData.compound = processedCompound;

                    builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(pong, 0, AccessFlags.Write);
                    builder.SetRenderFunc((BlurPassData data, RasterGraphContext context) => ExecuteBlurPass(data, context));
                }

                // 5. Vertical blur 2: pong -> ping
                using (var builder = renderGraph.AddRasterRenderPass<BlurPassData>("GaussianBlur Vertical 2", out var passData))
                {
                    passData.material = m_VerticalMaterial;
                    passData.inputTexture = pong;
                    passData.radius = processedRadius;
                    passData.compound = processedCompound;

                    builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(ping, 0, AccessFlags.Write);
                    builder.SetRenderFunc((BlurPassData data, RasterGraphContext context) => ExecuteBlurPass(data, context));
                }

                // 6. Horizontal blur 3: ping -> pong
                using (var builder = renderGraph.AddRasterRenderPass<BlurPassData>("GaussianBlur Horizontal 3", out var passData))
                {
                    passData.material = m_HorizontalMaterial;
                    passData.inputTexture = ping;
                    passData.radius = processedRadius;
                    passData.compound = processedCompound;

                    builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(pong, 0, AccessFlags.Write);
                    builder.SetRenderFunc((BlurPassData data, RasterGraphContext context) => ExecuteBlurPass(data, context));
                }

                // 7. Vertical blur 3: pong -> ping
                using (var builder = renderGraph.AddRasterRenderPass<BlurPassData>("GaussianBlur Vertical 3", out var passData))
                {
                    passData.material = m_VerticalMaterial;
                    passData.inputTexture = pong;
                    passData.radius = processedRadius;
                    passData.compound = processedCompound;

                    builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(ping, 0, AccessFlags.Write);
                    builder.SetRenderFunc((BlurPassData data, RasterGraphContext context) => ExecuteBlurPass(data, context));
                }

                finalBlurred = ping;
            }

            // 8. Final composite: low-res result -> full-res source
            using (var builder = renderGraph.AddRasterRenderPass<CompositePassData>("GaussianBlur Composite", out var passData))
            {
                passData.material = m_CompositeMaterial;
                passData.inputTexture = finalBlurred;
                passData.dimmed = processedDimmed;

                builder.UseTexture(passData.inputTexture, AccessFlags.Read);
                builder.SetRenderAttachment(source, 0, AccessFlags.Write);
                builder.SetRenderFunc((CompositePassData data, RasterGraphContext context) => ExecuteCompositePass(data, context));
            }
        }
    }
}