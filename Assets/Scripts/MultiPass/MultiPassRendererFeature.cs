using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments.MultiPass
{
    public class MultiPassRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private List<string> _lightModePasses;

        private MultiPass _multiPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_multiPass);
        }

        public override void Create()
        {
            _multiPass = new MultiPass(_lightModePasses);
        }
    }
}