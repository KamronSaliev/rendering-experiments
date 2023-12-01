using System.Collections.Generic;
using UnityEngine;

namespace RenderingExperiments.RendererControl
{
    public class RendererControl : MonoBehaviour
    {
        // This list of RendererView is populated and managed by the custom editor script
        // It is serialized to maintain the state within the Unity Editor
        [SerializeField] private List<RendererView> _rendererViews;

        // This RendererViewType field is used to control specific functionality within the custom editor script
        // It is serialized to allow changes from the Unity Editor
        [SerializeField] private RendererViewType _rendererViewType;
    }
}