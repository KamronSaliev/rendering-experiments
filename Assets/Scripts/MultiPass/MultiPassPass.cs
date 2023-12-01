using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderingExperiments.MultiPass
{
    public class MultiPassPass : ScriptableRenderPass
    {
        private readonly List<ShaderTagId> _shaderTags;

        public MultiPassPass(List<string> tags)
        {
            _shaderTags = new List<ShaderTagId>();
            
            foreach (var tag in tags)
            {
                _shaderTags.Add(new ShaderTagId(tag));
            }

            renderPassEvent = RenderPassEvent.AfterRenderingOpaques - 1;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("Render Multi Pass")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                // Get the opaque rendering filter settings
                var filteringSettings = FilteringSettings.defaultValue;

                foreach (var pass in _shaderTags)
                {
                    var drawingSettings = CreateDrawingSettings(pass, ref renderingData, SortingCriteria.CommonOpaque);
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
                }
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }
}