Shader "Custom/ToonMaster"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        [HDR]_MainColor("Main Color", Color) = (1, 1, 1, 0)
        _DetailMask("Detail Mask Texture", 2D) = "black" {}
        _ShadowTex("Shadow Texture", 2D) = "white" {}
        _ShadowColor("Shadow Color", Color) = (1, 1, 1, 0)
        _Brightness("Brightness", Range(-1, 1)) = 0
        _OutlineThickness("Outline Thickness", Float) = 1
        _OutlineType("Outline Type", Integer) = 0
        _OutlineOffset("Outline Offset", Vector) = (0, 0, 0, 0)
        _Steps("Steps", Integer) = 2
        _Smoothness("Smoothness", Range(0, 1)) = 1
        [HDR]_SpecularColor("Specular Color", Color) = (1, 1, 1, 0)
        [Header(Noise)]
        _NoiseScale("Noise Scale", Float) = 1
        _NoiseBias("Noise Bias", Range(0, 1)) = 0.5
        _NoiseColor("Noise Color", Color) = (1, 1, 1, 0)
    }
    SubShader
    {

        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        Pass
        {
            Name "UniversalForward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #include "Assets/Shaders/HLSL/Unlit.hlsl"
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma vertex Vert;
            #pragma fragment Frag;
            ENDHLSL
        }

        // Outline pass
        Pass
        {
            Name "Outline"
            Tags
            {
                "LightMode" = "Outline"
            }
            Cull Front
            Blend One Zero
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #include "Assets/Shaders/HLSL/Unlit.hlsl"
            #pragma vertex InverseVert;
            #pragma fragment Outline;
            ENDHLSL
        }

        // Additional pass for light calculations
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #include "Assets/Shaders/HLSL/Unlit.hlsl"
            #pragma vertex Vert;
            #pragma fragment Frag;
            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }
            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            // Universal Pipeline keywords
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            ENDHLSL
        }
    }
}