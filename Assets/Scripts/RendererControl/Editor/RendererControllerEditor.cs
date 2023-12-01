using System;
using System.Collections.Generic;
using RenderingExperiments.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments.RendererControl.Editor
{
    [CustomEditor(typeof(RendererControl))]
    public class RendererControllerEditor : UnityEditor.Editor
    {
        private SerializedProperty _rendererViewsProperty;
        private SerializedProperty _rendererViewTypeProperty;

        private RenderPipelineAsset _currentRenderPipelineAsset;
        private List<ScriptableRendererFeature> _currentRendererFeatures;

        private readonly Color _mainPropertyColor = Color.green;
        private readonly Color _defaultPropertyColor = Color.white;

        private const int SpacePixelCount = 10;
        private const int MainButtonHeight = 50;

        private void OnEnable()
        {
            _rendererViewsProperty = serializedObject.FindProperty("_rendererViews");
            _rendererViewTypeProperty = serializedObject.FindProperty("_rendererViewType");

            _currentRenderPipelineAsset = GraphicsSettings.currentRenderPipeline;
            _currentRendererFeatures = UniversalRenderPipelineExtensions.GetRendererFeatures();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_rendererViewsProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Get Render Views! The list is empty!", MessageType.Error);
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_rendererViewsProperty, new GUIContent("Renderer Views"), true);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(SpacePixelCount);
            
            if (GUILayout.Button("Get Renderer Views"))
            {
                GetRendererViews();
            }

            if (GUILayout.Button("Reset Renderer Views"))
            {
                ResetRendererViews();
            }
            
            GUILayout.Space(SpacePixelCount);
            GUI.color = _mainPropertyColor;
            
            EditorGUILayout.PropertyField(_rendererViewTypeProperty);
            
            GUILayout.Space(SpacePixelCount);

            if (GUILayout.Button("Enable Chosen Renderer View", GUILayout.Height(MainButtonHeight)))
            {
                var currentRendererFeatureIndex = _rendererViewTypeProperty.enumValueIndex;
                var currentRendererFeatureName = _rendererViewTypeProperty.enumNames[currentRendererFeatureIndex];
                EnableRendererFeature(currentRendererFeatureName);
            }

            GUI.color = _defaultPropertyColor;

            serializedObject.ApplyModifiedProperties();
        }

        private void GetRendererViews()
        {
            ResetRendererViews();

            foreach (var rendererFeature in _currentRendererFeatures)
            {
                foreach (var rendererViewTypeName in Enum.GetNames(typeof(RendererViewType)))
                {
                    if (!rendererFeature.name.Contains(rendererViewTypeName))
                    {
                        continue;
                    }

                    _rendererViewsProperty.arraySize++;
                    var element = _rendererViewsProperty.GetArrayElementAtIndex(_rendererViewsProperty.arraySize - 1);

                    var rendererViewType = (RendererViewType)Enum.Parse(typeof(RendererViewType), rendererViewTypeName);
                    var isActive = rendererFeature.isActive;

                    element.FindPropertyRelative("_rendererViewType").enumValueIndex = (int)rendererViewType;
                    element.FindPropertyRelative("_scriptableRendererFeature").objectReferenceValue = rendererFeature;
                    element.FindPropertyRelative("_isActive").boolValue = isActive;

                    Debug.Log($"Added {rendererViewTypeName}");
                }
            }
        }

        private void ResetRendererViews()
        {
            _rendererViewsProperty.ClearArray();

            Debug.LogWarning("Cleared renderer view instances");
        }

        private void EnableRendererFeature(string rendererFeatureName)
        {
            foreach (var rendererFeature in _currentRendererFeatures)
            {
                rendererFeature.SetActive(false);
            }

            foreach (var rendererFeature in _currentRendererFeatures)
            {
                if (rendererFeature.name != rendererFeatureName)
                {
                    continue;
                }

                rendererFeature.SetActive(!rendererFeature.isActive);

                Debug.Log($"Enabled Renderer Feature: {rendererFeature.name} {rendererFeature.isActive}");
                break;
            }

            EditorUtility.SetDirty(_currentRenderPipelineAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}