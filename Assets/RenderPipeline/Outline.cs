using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Vector2 = UnityEngine.Vector2;

public class Outline : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Renderer sRdrLiquid;
        
        public RTHandle colorRTTemp = null, depthRTTemp = null;
        public  RTHandle colorLiquidRTTemp = null, depthLiquidRTTemp = null;


        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ScriptableRenderer sRdr = renderingData.cameraData.renderer;
            RTHandle camColorHandle = sRdr.cameraColorTargetHandle;
            RTHandle camDepthHandle = sRdr.cameraDepthTargetHandle;
            RenderTextureDescriptor colorDesc = camColorHandle.rt.descriptor;
            RenderTextureDescriptor depthDesc = camDepthHandle.rt.descriptor;
            
            //之前是CommandBuffer.GetTemporaryRT
            RenderingUtils.ReAllocateIfNeeded(ref colorRTTemp, colorDesc);
            RenderingUtils.ReAllocateIfNeeded(ref depthRTTemp, colorDesc);
            
            RenderingUtils.ReAllocateIfNeeded(ref colorLiquidRTTemp, colorDesc);
            RenderingUtils.ReAllocateIfNeeded(ref depthLiquidRTTemp, colorDesc);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            context.ExecuteCommandBuffer(cmd);
            ScriptableRenderer sRdrCamera = renderingData.cameraData.renderer;
            RTHandle tempCamColHandle = sRdrCamera.cameraColorTargetHandle;
            
            //CoreUtils.SetRenderTarget（设置SV_Targe和SV_Depth）——>设置内部材质参数——>cmd.DrawMesh绘制全屏网格
            Vector2 viewScale = tempCamColHandle.useScaling
                ? new Vector2(tempCamColHandle.rtHandleProperties.rtHandleScale.x,
                    tempCamColHandle.rtHandleProperties.rtHandleScale.y)
                : Vector2.one;
            CoreUtils.SetRenderTarget(cmd, colorRTTemp, depthRTTemp, ClearFlag.All, Color.clear);
            Blitter.BlitColorAndDepth(cmd, sRdrCamera.cameraColorTargetHandle, sRdrCamera.cameraDepthTargetHandle,
                viewScale, 0, true);
            
            //指定材质
            CoreUtils.SetRenderTarget(cmd, colorLiquidRTTemp, depthLiquidRTTemp, ClearFlag.All, Color.clear);
            cmd.DrawRenderer(sRdrLiquid, sRdrLiquid.sharedMaterial);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


