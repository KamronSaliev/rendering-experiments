using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments
{
    public class BlurRenderPass : ScriptableRenderPass
    {
        private readonly BlurSettings _defaultSettings;
        private readonly Material _material;
        private RenderTextureDescriptor _blurTextureDescriptor;
        private RTHandle _blurTextureHandle;
        
        private static readonly int HorizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
        private static readonly int VerticalBlurId = Shader.PropertyToID("_VerticalBlur");

        public BlurRenderPass(Material material, BlurSettings defaultSettings)
        {
            _material = material;
            _defaultSettings = defaultSettings;

            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
            
            _blurTextureDescriptor = new RenderTextureDescriptor
            (
                Screen.width,
                Screen.height,
                RenderTextureFormat.Default,
                0
            );
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            _blurTextureDescriptor.width = cameraTextureDescriptor.width;
            _blurTextureDescriptor.height = cameraTextureDescriptor.height;
            RenderingUtils.ReAllocateIfNeeded(ref _blurTextureHandle, _blurTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("Custom Post Process Effects")))
            {
                var cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
                
                var volumeComponent = VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>();
                var horizontalBlur = volumeComponent.HorizontalBlur.overrideState
                    ? volumeComponent.HorizontalBlur.value
                    : _defaultSettings.HorizontalBlur;
                _material.SetFloat(HorizontalBlurId, horizontalBlur);

                var verticalBlur = volumeComponent.VerticalBlur.overrideState
                    ? volumeComponent.VerticalBlur.value
                    : _defaultSettings.VerticalBlur;
                _material.SetFloat(VerticalBlurId, verticalBlur);

                Blit(cmd, cameraTargetHandle, _blurTextureHandle, _material);
                Blit(cmd, _blurTextureHandle, cameraTargetHandle, _material, 1);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Object.Destroy(_material);
            }
            else
            {
                Object.DestroyImmediate(_material);
            }
#else
                Object.Destroy(material);
#endif

            _blurTextureHandle?.Release();
        }
    }
}