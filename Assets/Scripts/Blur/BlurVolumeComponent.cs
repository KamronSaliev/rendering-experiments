using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments.Blur
{
    [Serializable]
    [VolumeComponentMenuForRenderPipeline("Custom/Blur", typeof(UniversalRenderPipeline))]
    public class BlurVolumeComponent : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter HorizontalBlur => _horizontalBlur;

        public ClampedFloatParameter VerticalBlur => _verticalBlur;

        [SerializeField] private ClampedFloatParameter _horizontalBlur = new(0.05f, 0, 0.5f);
        [SerializeField] private ClampedFloatParameter _verticalBlur = new(0.05f, 0, 0.5f);

        public bool IsActive()
        {
            return true;
        }

        public bool IsTileCompatible()
        {
            return true;
        }
    }
}