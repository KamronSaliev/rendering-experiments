using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments.RendererControl
{
    [Serializable]
    public class RendererView
    {
        // Used for serialization
        [SerializeField] private RendererViewType _rendererViewType;
        [SerializeField] private ScriptableRendererFeature _scriptableRendererFeature;

        public RendererView
        (
            RendererViewType rendererViewType,
            ScriptableRendererFeature scriptableRendererFeature
        )
        {
            _rendererViewType = rendererViewType;
            _scriptableRendererFeature = scriptableRendererFeature;
        }
    }
}
