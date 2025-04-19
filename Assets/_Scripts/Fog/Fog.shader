Shader "Hidden/URP/Fog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "FogPass"
            ZWrite Off Cull Off ZTest Always

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex  Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ─────────── textures ───────────
            TEXTURE2D_X(_MainTex);            SAMPLER(sampler_MainTex);
            TEXTURE2D_X(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

            // ─────────── uniforms ───────────
            float4   _MainTex_ST;
            float4x4 _FrustumCornersRay;
            float4   _FogColorHorizon, _FogColorZenith;
            float    _FogDensity, _FogOffset;
            int      _HeightFogMode;
            float    _HeightFogAmount, _HeightFogFalloff, _GradientHeightFactor;
            float    _GradientBias;

            // ─────────── enum values ─────────
            #define FALLOFF_LINEAR      0
            #define FALLOFF_POWER       1
            #define FALLOFF_SMOOTHSTEP  2
            #define FALLOFF_EXPSQR      3

            struct Attributes { float3 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings Vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv         = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 ReconstructViewRay(float2 uv)
            {
                float3 tl = _FrustumCornersRay[0].xyz;
                float3 tr = _FrustumCornersRay[1].xyz;
                float3 bl = _FrustumCornersRay[2].xyz;
                float3 br = _FrustumCornersRay[3].xyz;
                return lerp(lerp(bl, tl, uv.y), lerp(br, tr, uv.y), uv.x);
            }

            float4 Frag (Varyings i) : SV_Target
            {
                // sample colour & depth
                float4 col      = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv);
                float  rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv).r;

                // linear depth values
                float linear01 = Linear01Depth(rawDepth, _ZBufferParams);
                float viewDist = LinearEyeDepth(rawDepth, _ZBufferParams);
                bool  isSky    = linear01 >= 0.999;

                // distance fog factor
                float distFog = exp2(-pow(max(0, viewDist - _FogOffset) * _FogDensity, 2));

                // height fog
                float3 viewRay = ReconstructViewRay(i.uv);
                float  upDot   = saturate(max(0, normalize(viewRay).y));
                float  falloff = max(0.01, _HeightFogFalloff);

                float heightRed =
                      (_HeightFogMode == FALLOFF_LINEAR)     ? upDot :
                      (_HeightFogMode == FALLOFF_POWER)      ? pow(upDot, falloff) :
                      (_HeightFogMode == FALLOFF_SMOOTHSTEP) ? smoothstep(0,1, pow(upDot, falloff)) :
                                                               1 - exp2(-pow(upDot * falloff, 2));

                // vertical gradient with bias
                float gradLerp = saturate(pow(upDot + _GradientBias,
                                               max(0.01, _GradientHeightFactor)));
                float4 fogCol  = lerp(_FogColorHorizon, _FogColorZenith, gradLerp);

                // combine
                float4 distCol  = lerp(fogCol, col, saturate(distFog));
                float4 finalCol = lerp(distCol, col, saturate(heightRed * _HeightFogAmount));

                return isSky ? lerp(fogCol, col, saturate(heightRed * _HeightFogAmount))
                             : finalCol;
            }
            ENDHLSL
        }
    }
    Fallback Off
}
