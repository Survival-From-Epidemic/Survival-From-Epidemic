using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KawaseBlur
{
    public class KawaseBlur : ScriptableRendererFeature
    {
        public KawaseBlurSettings settings = new();

        private CustomRenderPass _scriptablePass;

        public override void Create()
        {
            _scriptablePass = new CustomRenderPass("KawaseBlur")
            {
                blurMaterial = settings.blurMaterial,
                passes = settings.blurPasses,
                downsample = settings.downsample,
                copyToFramebuffer = settings.copyToFramebuffer,
                targetName = settings.targetName,
                renderPassEvent = settings.renderPassEvent
            };
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            var src = renderer.cameraColorTargetHandle;
            _scriptablePass.Setup(src);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_scriptablePass);
        }

        [Serializable]
        public class KawaseBlurSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            public Material blurMaterial;

            [Range(2, 15)] public int blurPasses = 1;

            [Range(1, 4)] public int downsample = 1;

            public bool copyToFramebuffer;
            public string targetName = "_blurTexture";
        }

        private class CustomRenderPass : ScriptableRenderPass
        {
            private readonly string profilerTag;
            public Material blurMaterial;
            public bool copyToFramebuffer;
            public int downsample;
            public int passes;
            public string targetName;

            private int tmpId1;
            private int tmpId2;

            private RenderTargetIdentifier tmpRT1;
            private RenderTargetIdentifier tmpRT2;

            public CustomRenderPass(string profilerTag)
            {
                this.profilerTag = profilerTag;
            }

            private RenderTargetIdentifier source { get; set; }

            public void Setup(RenderTargetIdentifier source)
            {
                this.source = source;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                var width = cameraTextureDescriptor.width / downsample;
                var height = cameraTextureDescriptor.height / downsample;

                tmpId1 = Shader.PropertyToID("tmpBlurRT1");
                tmpId2 = Shader.PropertyToID("tmpBlurRT2");
                cmd.GetTemporaryRT(tmpId1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
                cmd.GetTemporaryRT(tmpId2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

                tmpRT1 = new RenderTargetIdentifier(tmpId1);
                tmpRT2 = new RenderTargetIdentifier(tmpId2);

                ConfigureTarget(tmpRT1);
                ConfigureTarget(tmpRT2);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(profilerTag);

                var opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;

                // first pass
                // cmd.GetTemporaryRT(tmpId1, opaqueDesc, FilterMode.Bilinear);
                cmd.SetGlobalFloat("_offset", 1.5f);
                cmd.Blit(source, tmpRT1, blurMaterial);

                for (var i = 1; i < passes - 1; i++)
                {
                    cmd.SetGlobalFloat("_offset", 0.5f + i);
                    cmd.Blit(tmpRT1, tmpRT2, blurMaterial);

                    // pingpong
                    var rttmp = tmpRT1;
                    tmpRT1 = tmpRT2;
                    tmpRT2 = rttmp;
                }

                // final pass
                cmd.SetGlobalFloat("_offset", 0.5f + passes - 1f);
                if (copyToFramebuffer)
                {
                    cmd.Blit(tmpRT1, source, blurMaterial);
                }
                else
                {
                    cmd.Blit(tmpRT1, tmpRT2, blurMaterial);
                    cmd.SetGlobalTexture(targetName, tmpRT2);
                }

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
            }
        }
    }
}