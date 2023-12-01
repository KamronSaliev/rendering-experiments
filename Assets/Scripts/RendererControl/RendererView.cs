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
        [SerializeField] private bool _isActive;

        public RendererView
        (
            RendererViewType rendererViewType,
            ScriptableRendererFeature scriptableRendererFeature,
            bool isActive
        )
        {
            _rendererViewType = rendererViewType;
            _scriptableRendererFeature = scriptableRendererFeature;
            _isActive = isActive;
        }
    }
}
