#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Assets/Shaders/HLSL/Noise.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

struct Attributes
{
    float4 position : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;  
};

struct Varyings
{
    float4 vertex : POSITION0;
    float3 normal : NORMAL;
    float3 positionWS : POSITIONT;
    float2 uv : TEXCOORD0;
    float4 shadowCoord : TEXCOORD1;    
};


// Get properties from the Unity Material
CBUFFER_START(UnityPerMaterial)
float3 _MainColor;
float4 _MainTex_ST;
float3 _ShadowColor;
int _Steps;
int _OutlineType;
float _OutlineThickness;
float4 _OutlineOffset;
float _Brightness;
float _Smoothness;
float _NoiseScale;
float _NoiseBias;
float3 _NoiseColor;
float3 _SpecularColor;
CBUFFER_END

Varyings Vert(Attributes i)
{
    Varyings o;
    o.vertex = TransformObjectToHClip(i.position.xyz);
    o.uv = TRANSFORM_TEX(i.uv, _MainTex);
    o.normal = TransformObjectToWorldNormal(i.normal.xyz);
    o.positionWS = TransformObjectToWorld(i.position.xyz);
    VertexPositionInputs vertexInput = GetVertexPositionInputs(i.position.xyz);
    o.shadowCoord = GetShadowCoord(vertexInput);
    return o;
}

float NDotL(float3 WorldNormal, float3 Direction)
{
    return saturate(dot(normalize(Direction), normalize(WorldNormal)));
}

float GetNoise(float2 uv)
{
    float2 st = uv * _NoiseScale;
    float n = cnoise(st);
    
    return n;
}

float GetStep(float value)
{
    float stepVal = 1.0 / _Steps;
    
    for (int i = _Steps; i >= 0; i--)
    {
        float target = stepVal * i;
        
        if (value > target)
        {
            return target;
        }
    }
    
    return 0;
}

half4 Frag(Varyings input) : SV_Target
{
    half4 mainTextureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
    
    half4 color = mainTextureColor;
    color.rgb *= _MainColor;
    
    Light mainLight = GetMainLight(input.shadowCoord);
    float shadowDistance = mainLight.distanceAttenuation;
    float shadow = mainLight.shadowAttenuation;
    float nDL = NDotL(input.normal, _MainLightPosition.xyz);
    float3 ambient = SampleSH(input.normal);

    float lightIntensity = GetStep(nDL);
    
    color.rgb *= lightIntensity * _MainLightColor.rgb * (shadowDistance * shadow)  + ambient;
    color.rgb = lerp(_ShadowColor, 1, color.rgb);

    half3 output = color.rgb;
    
    float n = GetNoise(input.uv);
    if (n > _NoiseBias)
    {
        n = 1;
    }
    else
    {
        n = 0;
    }
    
    output = lerp(output, output * _NoiseColor, n);
    output += _Brightness;
    
    return half4(output, 1);
}

// For outlines
Varyings InverseVert(Attributes IN)
{
    Varyings OUT;
    float4 clipPosition;
    
    if (_OutlineType == 0)
    {
        //Vertex Scaling
        float3 pos = IN.position.xyz * _OutlineThickness;
        clipPosition = TransformObjectToHClip(pos);
        clipPosition += _OutlineOffset;
        
        OUT.vertex = clipPosition;
        OUT.uv = IN.uv;
    
        return OUT;
    }
    else
    {  
        // Normal Based
        float4 clipPosition = TransformObjectToHClip(IN.position);
        float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, IN.normal));
        float2 offset = normalize(clipNormal.xy) * _OutlineThickness;
        clipPosition.xy += offset;
        
        OUT.vertex = clipPosition;
        OUT.uv = IN.uv;
    
        return OUT;
    }
}

Varyings InverseVertTwo(Attributes IN)
{
    Varyings OUT;
    float4 clipPosition;
    
    if (_OutlineType == 0)
    {
        // Vertex Scaling
        float3 pos = IN.position.xyz * (_OutlineThickness + 0.05);
        clipPosition = TransformObjectToHClip(pos);
        clipPosition += _OutlineOffset;
        
        OUT.vertex = clipPosition;
        OUT.uv = IN.uv;
    
        return OUT;
    }
    else
    {
        // Normal Based
        float4 clipPosition = TransformObjectToHClip(IN.position);
        float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, IN.normal));
        float2 offset = normalize(clipNormal.xy) * _OutlineThickness;
        clipPosition.xy += offset;
        
        OUT.vertex = clipPosition;
        OUT.uv = IN.uv;
    
        return OUT;
    }
}

// First outline color
half4 Outline(Varyings input) : SV_Target
{ 
    return 0;
}

// Second outline color
half4 OutlineTwo(Varyings input) : SV_Target
{
    return 1;
}
