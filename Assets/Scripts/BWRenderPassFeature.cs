using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Custom ScriptableRendererFeature for a black and white post-processing effect
public class BWRenderPassFeature : ScriptableRendererFeature
{
    private BWPass bwPass;

    // Called when the feature is created
    public override void Create()
    {
        // Create an instance of BWPass for rendering
        bwPass = new BWPass();
    }

    // Called to add render passes to the renderer
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Enqueue the custom render pass
        renderer.EnqueuePass(bwPass);
    }

    // Custom render pass class
    class BWPass : ScriptableRenderPass
    {
        Material _mat; // Material for black and white effect
        int bwId = Shader.PropertyToID("_Temp"); // Property ID for the temporary render target
        RenderTargetIdentifier src, bw; // Render target identifiers

        public BWPass()
        {
            // Create the material for the black and white effect
            if (!_mat)
            {
                _mat = CoreUtils.CreateEngineMaterial("Custom Post-Processing/B&W Post-Processing");
            }
            // Set the render pass event to execute before post-processing
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // Called when setting up the camera for rendering
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Get the camera target descriptor
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            // Set the source render target identifier
            src = renderingData.cameraData.renderer.cameraColorTargetHandle;
            // Create a temporary render target and get its identifier
            cmd.GetTemporaryRT(bwId, desc, FilterMode.Bilinear);
            bw = new RenderTargetIdentifier(bwId);
        }

        // Execute the custom render pass
        public override void Execute(ScriptableRenderContext context, ref RenderingData render)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("BWRenderPassFeature");
            // Access the volume stack and get the BlackAndWhitePostProcess component
            VolumeStack volumes = VolumeManager.instance.stack;
            BlackAndWhitePostProcess bwPP = volumes.GetComponent<BlackAndWhitePostProcess>();

            // Check if the black and white post-processing is active
            if (bwPP.IsActive())
            {
                // Set the blend intensity in the material
                _mat.SetFloat("_blend", (float)bwPP.blendIntensity);
                // Apply the black and white effect to the temporary render target
                Blit(commandBuffer, src, bw, _mat, 0);
                // Blit the result back to the source render target
                Blit(commandBuffer, bw, src);
            }

            // Execute the command buffer
            context.ExecuteCommandBuffer(commandBuffer);
            // Release the command buffer
            CommandBufferPool.Release(commandBuffer);
        }

        // Called when cleaning up the camera after rendering
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Release the temporary render target
            cmd.ReleaseTemporaryRT(bwId);
        }
    }
}
