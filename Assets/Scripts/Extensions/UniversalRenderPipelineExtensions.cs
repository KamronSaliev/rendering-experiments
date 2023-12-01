#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments.Extensions
{
    public static class UniversalRenderPipelineExtensions
    {
        private static readonly FieldInfo RenderDataListFieldInfo;

        private const string RenderDataListFieldName = "m_RendererDataList";

        static UniversalRenderPipelineExtensions()
        {
            var pipelineAssetType = typeof(UniversalRenderPipelineAsset);
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            RenderDataListFieldInfo = pipelineAssetType.GetField(RenderDataListFieldName, flags);
        }

        public static ScriptableRendererData[] GetRendererDataList(UniversalRenderPipelineAsset asset = null)
        {
            if (asset == null)
            {
                asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            }

            return (ScriptableRendererData[])RenderDataListFieldInfo.GetValue(asset);
        }

        public static List<ScriptableRendererFeature> GetRendererFeatures(int rendererDataIndex = 0)
        {
            var rendererDataList = GetRendererDataList();
            return rendererDataList[rendererDataIndex].rendererFeatures;
        }

        public static T GetRendererFeature<T>() where T : ScriptableRendererFeature
        {
            var renderDataList = GetRendererDataList();
            if (renderDataList == null || renderDataList.Length == 0)
            {
                return null;
            }

            foreach (var renderData in renderDataList)
            {
                foreach (var rendererFeature in renderData.rendererFeatures)
                {
                    if (rendererFeature is T feature)
                    {
                        return feature;
                    }
                }
            }

            return null;
        }

        public static ScriptableRendererFeature GetRendererFeature(string typeName)
        {
            var renderDataList = GetRendererDataList();
            if (renderDataList == null || renderDataList.Length == 0)
            {
                return null;
            }

            foreach (var renderData in renderDataList)
            {
                foreach (var rendererFeature in renderData.rendererFeatures)
                {
                    if (rendererFeature == null)
                    {
                        continue;
                    }

                    if (rendererFeature.GetType().Name.Contains(typeName))
                    {
                        return rendererFeature;
                    }
                }
            }

            return null;
        }

        public static bool IsRendererFeatureActive<T>(bool defaultValue = false) where T : ScriptableRendererFeature
        {
            var feature = GetRendererFeature<T>();
            if (feature == null)
            {
                return defaultValue;
            }

            return feature.isActive;
        }

        public static bool IsRendererFeatureActive(string typeName, bool defaultValue = false)
        {
            var feature = GetRendererFeature(typeName);
            if (feature == null)
            {
                return defaultValue;
            }

            return feature.isActive;
        }

        public static void SetRendererFeatureActive<T>(bool active) where T : ScriptableRendererFeature
        {
            var feature = GetRendererFeature<T>();
            if (feature == null)
            {
                return;
            }

            feature.SetActive(active);
        }

        public static void SetRendererFeatureActive(string typeName, bool active)
        {
            var feature = GetRendererFeature(typeName);
            if (feature == null)
            {
                return;
            }

            feature.SetActive(active);
        }
    }
}
#endif