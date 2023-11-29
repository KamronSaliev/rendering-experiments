using System;
using UnityEngine;

namespace RenderingExperiments.Blur
{
    [Serializable]
    public class BlurSettings
    {
        public float HorizontalBlur => _horizontalBlur;

        public float VerticalBlur => _verticalBlur;

        [SerializeField] [Range(0, 0.4f)] private float _horizontalBlur = 0.001f;
        [SerializeField] [Range(0, 0.4f)] private float _verticalBlur = 0.001f;
    }
}