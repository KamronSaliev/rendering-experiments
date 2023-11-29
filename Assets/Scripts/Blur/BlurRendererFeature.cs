using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments.Blur
{
    public class BlurRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private Shader _shader;
        [SerializeField] private BlurSettings _settings;
        
        private Material _material;
        private BlurRenderPass _renderPass;

        public override void Create()
        {
            if (_shader == null)
            {
                return;
            }

            _material = new Material(_shader);
            _renderPass = new BlurRenderPass(_material, _settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(_renderPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _renderPass.Dispose();
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(_material);
            }
            else
            {
                DestroyImmediate(_material);
            }
#else
                Destroy(material);
#endif
        }
    }
}