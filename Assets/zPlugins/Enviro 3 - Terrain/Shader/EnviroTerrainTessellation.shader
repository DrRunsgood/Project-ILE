Shader "Enviro3/EnviroTerrainTessellation"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector][Toggle(_HEIGHTBLEND_ON)] _HEIGHTBLEND("HEIGHTBLEND", Float) = 1
		[HideInInspector]_TerrainHolesTexture("_TerrainHolesTexture", 2D) = "white" {}
		[HideInInspector][Toggle(_STOCHASTIC_ON)] _STOCHASTIC("STOCHASTIC", Float) = 0
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Control2("Control2", 2D) = "black" {}
		[HideInInspector]_Control1("Control1", 2D) = "black" {}
		[HideInInspector]_SSSIntensity("SSS Intensity", Range( 0 , 5)) = 3
		[HideInInspector]_SSSScale("SSS Scale", Range( 0 , 1)) = 0.5
		[HideInInspector]_SSSDistortion("SSS Distortion", Range( 0 , 1)) = 0.9
		[HideInInspector]_SnowTiling("Snow Tiling", Float) = 0
		[HideInInspector]_AlbedoArray("AlbedoArray", 2DArray) = "white" {}
		[HideInInspector]_NormalArray("NormalArray", 2DArray) = "bump" {}
		[HideInInspector]_MaskArray("MaskArray", 2DArray) = "white" {}
		[HideInInspector]_LayerScaleOffset4("_LayerScaleOffset4", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset8("_LayerScaleOffset8", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset0("_LayerScaleOffset0", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset9("_LayerScaleOffset9", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset1("_LayerScaleOffset1", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset5("_LayerScaleOffset5", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset2("_LayerScaleOffset2", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset10("_LayerScaleOffset10", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset6("_LayerScaleOffset6", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset7("_LayerScaleOffset7", Vector) = (0,0,0,0)
		[HideInInspector]_LayerScaleOffset11("_LayerScaleOffset11", Vector) = (0,0,0,0)
		[HideInInspector]_EnviroRainIntensity("_EnviroRainIntensity", Float) = 0
		[HideInInspector]_LayerScaleOffset3("_LayerScaleOffset3", Vector) = (0,0,0,0)
		[HideInInspector]_RainFlowIntensity("Rain Flow Intensity", Range( 0 , 2)) = 1
		[HideInInspector]_RainFlowDistortionScale("Rain Flow Distortion Scale", Float) = 10
		[HideInInspector]_RainFlowDistortionStrenght("Rain Flow Distortion Strenght", Range( 0 , 0.25)) = 0.1
		[HideInInspector][Toggle(_RAIN_ON)] _Rain("Rain", Float) = 1
		[HideInInspector]_RainFlowTiling("Rain Flow Tiling", Float) = 5
		[HideInInspector][SingleLineTexture]_SnowMask("Snow Mask", 2D) = "black" {}
		[HideInInspector]_RainFlowStrength("Rain Flow Strength", Range( 0 , 1)) = 0.5
		[HideInInspector][Normal][SingleLineTexture]_SnowNormal("Snow Normal", 2D) = "white" {}
		[HideInInspector][SingleLineTexture]_SnowAlbedo("Snow Albedo", 2D) = "white" {}
		[HideInInspector]_SnowHeightBlending("Snow Height Blending", Range( 0 , 10)) = 0
		[HideInInspector]_SnowSlopePower("Snow Slope Power", Float) = 2
		[HideInInspector]_EnviroWetness("_EnviroWetness", Range( 0 , 1)) = 0
		[HideInInspector]_RainDropIntensity("RainDropIntensity", Float) = 5
		[HideInInspector]_SnowDisplacement("SnowDisplacement", Range( 0 , 1)) = 0.01
		[HideInInspector]_RainFlowSmoothnessBoost("RainFlowSmoothnessBoost", Range( 0 , 4)) = 2
		[HideInInspector]_RainDropSpeed("Rain Drop Speed", Range( 0 , 2)) = 1
		[HideInInspector][Toggle(_PUDDLES_ON)] _Puddles("Puddles", Float) = 0
		[HideInInspector]_PuddleColor("Puddle Color", Color) = (0.6037736,0.6037736,0.6037736,0.6666667)
		[HideInInspector]_DisplacementStrength("Displacement Strength", Range( 0 , 1)) = 0.05
		[HideInInspector]_EnviroSnow("_EnviroSnow", Range( 0 , 2)) = 0
		[HideInInspector]_TessellationMinDistance("Tessellation Min Distance", Float) = 10
		[HideInInspector]_RainDropTiling("RainDropTiling", Float) = 10
		[HideInInspector]_PuddleWaveTiling("Puddle Wave Tiling", Float) = 1
		[HideInInspector][Toggle(_SNOW_ON)] _Snow("Snow", Float) = 0
		[HideInInspector]_TessellationFactor("Tessellation Factor", Range( 1 , 32)) = 16
		[HideInInspector]_RainDistanceFade("RainDistanceFade", Range( 0 , 10)) = 5
		[HideInInspector]_PuddleWaveIntensity("Puddle Wave Intensity", Range( 0 , 2)) = 1
		[HideInInspector]_TessellationMaxDistance("Tessellation Max Distance", Float) = 30
		[HideInInspector]_WetnessBoost("WetnessBoost", Range( 0 , 1)) = 0.3
		[HideInInspector]_PuddleIntensity("Puddle Intensity", Range( 0 , 5)) = 1
		[HideInInspector]_PuddleCoverageNoise("Puddle Coverage Noise", Float) = 0.5
		[HideInInspector][Normal][SingleLineTexture]_WaveNormal("Wave Normal", 2D) = "white" {}
		[HideInInspector]_SnowMetallic("Snow Metallic", Float) = 0
		[HideInInspector]_SnowSmoothness("Snow Smoothness", Float) = 1
		[HideInInspector]_SnowNormalScale("Snow Normal Scale", Float) = 0
		[HideInInspector]_Metallic00("Metallic00", Vector) = (0,0,0,0)
		[HideInInspector]_Metallic02("Metallic02", Vector) = (0,0,0,0)
		[HideInInspector]_Metallic01("Metallic01", Vector) = (0,0,0,0)
		[HideInInspector]_Occlusion0("Occlusion0", Vector) = (0,0,0,0)
		[HideInInspector]_Occlusion2("Occlusion2", Vector) = (0,0,0,0)
		[HideInInspector]_Occlusion1("Occlusion1", Vector) = (0,0,0,0)
		[HideInInspector]_HeightBlending("HeightBlending", Range( 0 , 1)) = 0
		[HideInInspector]_MipDistanceBlending("MipDistanceBlending", Float) = 40
		[HideInInspector]_HeightBlendStrength("_HeightBlendStrength", Float) = 1
		[HideInInspector][KeywordEnum(_4,_8,_12)] _SPLATCOUNT("SPLATCOUNT", Float) = 0
		[HideInInspector][KeywordEnum(Fast,Balance,Quality)] _Quality("Quality", Float) = 0
		[HideInInspector]_Smoothness00("Smoothness00", Vector) = (0,0,0,0)
		[HideInInspector]_Smoothness01("Smoothness01", Vector) = (0,0,0,0)
		[HideInInspector]_Smoothness02("Smoothness02", Vector) = (0,0,0,0)
		[HideInInspector]_ColorTint10("_ColorTint10", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint0("_ColorTint0", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint1("_ColorTint1", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint2("_ColorTint2", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint3("_ColorTint3", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint4("_ColorTint4", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint5("_ColorTint5", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint6("_ColorTint6", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint7("_ColorTint7", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint8("_ColorTint8", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint9("_ColorTint9", Color) = (0,0,0,0)
		[HideInInspector]_ColorTint11("_ColorTint11", Color) = (0,0,0,0)
		[HideInInspector]_HeightContrast1("_HeightContrast1", Vector) = (0,0,0,0)
		[HideInInspector]_SamplingType1("_SamplingType1", Vector) = (0,0,0,0)
		[HideInInspector]_DisplacementMod1("_DisplacementMod1", Vector) = (0,0,0,0)
		[HideInInspector]_NormalScale01("_NormalScale01", Vector) = (0,0,0,0)
		[HideInInspector]_HeightContrast0("_HeightContrast0", Vector) = (0,0,0,0)
		[HideInInspector]_NormalScale00("_NormalScale00", Vector) = (0,0,0,0)
		[HideInInspector]_NormalScale02("_NormalScale02", Vector) = (0,0,0,0)
		[HideInInspector]_SamplingType0("_SamplingType0", Vector) = (0,0,0,0)
		[HideInInspector]_DisplacementMod0("_DisplacementMod0", Vector) = (0,0,0,0)
		[HideInInspector]_HeightContrast2("_HeightContrast2", Vector) = (0,0,0,0)
		[HideInInspector]_SamplingType2("_SamplingType2", Vector) = (0,0,0,0)
		[HideInInspector]_DisplacementMod2("_DisplacementMod2", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector][ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1
		[HideInInspector][ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		[HideInInspector][ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry-100" "UniversalMaterialType"="Lit" "TerrainCompatible"="True" "SplatCount"="12" }

		Cull Back
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _LIGHT_COOKIES
			#pragma multi_compile _ _FORWARD_PLUS

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_FORWARD

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#pragma multi_compile_local _RAIN_ON
			#pragma multi_compile_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
			#include "EnviroInclude.hlsl"


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float4 lightmapUVOrVertexSH : TEXCOORD1;
				half4 fogFactorAndVertexLight : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					float4 shadowCoord : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD7;
				#endif	
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD8;
				#endif
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_texcoord10 : TEXCOORD10;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D_ARRAY(_AlbedoArray);
			SAMPLER(sampler_AlbedoArray);
			TEXTURE2D(_SnowAlbedo);
			SAMPLER(sampler_SnowAlbedo);
			TEXTURE2D_ARRAY(_NormalArray);
			SAMPLER(sampler_NormalArray);
			TEXTURE2D(_WaveNormal);
			SAMPLER(sampler_WaveNormal);
			TEXTURE2D(_SnowNormal);
			SAMPLER(sampler_SnowNormal);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			
			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				float4 appendResult660 = (float4(cross( input.normalOS , float3(0,0,1) ) , -1.0));
				
				output.ase_texcoord9.xy = input.texcoord.xy;
				output.ase_texcoord10 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord9.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif
				input.normalOS = input.normalOS;
				input.tangentOS = appendResult660;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				output.tSpace0 = float4( normalInput.normalWS, vertexInput.positionWS.x );
				output.tSpace1 = float4( normalInput.tangentWS, vertexInput.positionWS.y );
				output.tSpace2 = float4( normalInput.bitangentWS, vertexInput.positionWS.z );

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV( input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy );
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				OUTPUT_SH4( vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir( vertexInput.positionWS ), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					output.lightmapUVOrVertexSH.zw = input.texcoord.xy;
					output.lightmapUVOrVertexSH.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );

				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( vertexInput.positionCS.z );
				#else
					half fogFactor = 0;
				#endif

				output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				output.texcoord1 = input.texcoord1;
				output.texcoord2 = input.texcoord2;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						, bool ase_vface : SV_IsFrontFace ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (input.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( input.tSpace0.xyz );
					float3 WorldTangent = input.tSpace1.xyz;
					float3 WorldBiTangent = input.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(input.tSpace0.w,input.tSpace1.w,input.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = input.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord9.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord9.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D( _Control1, sampler_Control1, uv_Control1 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord9.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D( _Control2, sampler_Control2, uv_Control2 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1066 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break896 = samplingType895;
				float2 appendResult69 = (float2(input.ase_texcoord10.xyz.x , input.ase_texcoord10.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float3 break485 = UV0100;
				float2 appendResult493 = (float2(break485.x , break485.y));
				float2 temp_output_5_0_g1057 = appendResult493;
				int temp_output_4_0_g1057 = (int)break485.z;
				float2 appendResult87 = (float2(input.ase_texcoord10.xyz.x , input.ase_texcoord10.xyz.z));
				float2 Mip101 = ( appendResult87 * ( 1.0 / max( 0.001 , _MipDistanceBlending ) ) );
				float2 temp_output_9_0_g1057 = Mip101;
				float2 temp_output_12_0_g1057 = ddx( temp_output_9_0_g1057 );
				float2 temp_output_13_0_g1057 = ddy( temp_output_9_0_g1057 );
				float4 tex2DArrayNode3_g1057 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1057,(float)temp_output_4_0_g1057, temp_output_12_0_g1057, temp_output_13_0_g1057 );
				float localStochasticTiling190_g1058 = ( 0.0 );
				float2 Input_UV317_g1058 = temp_output_5_0_g1057;
				float2 UV190_g1058 = Input_UV317_g1058;
				float2 UV1190_g1058 = float2( 0,0 );
				float2 UV2190_g1058 = float2( 0,0 );
				float2 UV3190_g1058 = float2( 0,0 );
				float W1190_g1058 = 0.0;
				float W2190_g1058 = 0.0;
				float W3190_g1058 = 0.0;
				StochasticTiling( UV190_g1058 , UV1190_g1058 , UV2190_g1058 , UV3190_g1058 , W1190_g1058 , W2190_g1058 , W3190_g1058 );
				float Input_Index330_g1058 = (float)temp_output_4_0_g1057;
				float2 temp_output_358_0_g1058 = temp_output_12_0_g1057;
				float2 temp_output_359_0_g1058 = temp_output_13_0_g1057;
				float4 Output_2DArray152_g1058 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W1190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W2190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W3190_g1058 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1057 = Output_2DArray152_g1058;
				#else
				float4 staticSwitch7_g1057 = tex2DArrayNode3_g1057;
				#endif
				float4 ifLocalVar17_g1057 = 0;
				UNITY_BRANCH 
				if( break896.x > 0.0 )
				ifLocalVar17_g1057 = staticSwitch7_g1057;
				else if( break896.x == 0.0 )
				ifLocalVar17_g1057 = tex2DArrayNode3_g1057;
				float localGetUVS795 = ( 0.0 );
				float4 in0795 = _ColorTint0;
				float4 in1795 = _ColorTint1;
				float4 in2795 = _ColorTint2;
				float4 in3795 = _ColorTint3;
				float4 in4795 = _ColorTint4;
				float4 in5795 = _ColorTint5;
				float4 in6795 = _ColorTint6;
				float4 in7795 = _ColorTint7;
				float4 in8795 = _ColorTint8;
				float4 in9795 = _ColorTint9;
				float4 in10795 = _ColorTint10;
				float4 in11795 = _ColorTint11;
				float4 index795 = SplatIndex44;
				float4 Out0795 = float4( 0,0,0,0 );
				float4 Out1795 = float4( 0,0,0,0 );
				float4 Out2795 = float4( 0,0,0,0 );
				float4 Out3795 = float4( 0,0,0,0 );
				{
				GetLayerUV(in0795,in1795,in2795,in3795,in4795,in5795,in6795,in7795,in8795,in9795,in10795,in11795,index795,Out0795,Out1795,Out2795,Out3795);
				}
				float4 Color0796 = Out0795;
				float4 temp_output_616_0 = ( ifLocalVar17_g1057 * Color0796 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch511 = temp_output_616_0;
				#else
				float4 staticSwitch511 = temp_output_616_0;
				#endif
				float4 temp_output_2_0_g1065 = staticSwitch511;
				float4 temp_output_18_0_g1066 = float4( (temp_output_2_0_g1065).rgb , 0.0 );
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float3 break487 = UV197;
				float2 appendResult492 = (float2(break487.x , break487.y));
				float2 temp_output_5_0_g1061 = appendResult492;
				int temp_output_4_0_g1061 = (int)break487.z;
				float2 temp_output_9_0_g1061 = Mip101;
				float2 temp_output_12_0_g1061 = ddx( temp_output_9_0_g1061 );
				float2 temp_output_13_0_g1061 = ddy( temp_output_9_0_g1061 );
				float4 tex2DArrayNode3_g1061 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1061,(float)temp_output_4_0_g1061, temp_output_12_0_g1061, temp_output_13_0_g1061 );
				float localStochasticTiling190_g1062 = ( 0.0 );
				float2 Input_UV317_g1062 = temp_output_5_0_g1061;
				float2 UV190_g1062 = Input_UV317_g1062;
				float2 UV1190_g1062 = float2( 0,0 );
				float2 UV2190_g1062 = float2( 0,0 );
				float2 UV3190_g1062 = float2( 0,0 );
				float W1190_g1062 = 0.0;
				float W2190_g1062 = 0.0;
				float W3190_g1062 = 0.0;
				StochasticTiling( UV190_g1062 , UV1190_g1062 , UV2190_g1062 , UV3190_g1062 , W1190_g1062 , W2190_g1062 , W3190_g1062 );
				float Input_Index330_g1062 = (float)temp_output_4_0_g1061;
				float2 temp_output_358_0_g1062 = temp_output_12_0_g1061;
				float2 temp_output_359_0_g1062 = temp_output_13_0_g1061;
				float4 Output_2DArray152_g1062 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W1190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W2190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W3190_g1062 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1061 = Output_2DArray152_g1062;
				#else
				float4 staticSwitch7_g1061 = tex2DArrayNode3_g1061;
				#endif
				float4 ifLocalVar17_g1061 = 0;
				UNITY_BRANCH 
				if( break896.y > 0.0 )
				ifLocalVar17_g1061 = staticSwitch7_g1061;
				else if( break896.y == 0.0 )
				ifLocalVar17_g1061 = tex2DArrayNode3_g1061;
				float4 Color1797 = Out1795;
				float4 temp_output_617_0 = ( ifLocalVar17_g1061 * Color1797 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch508 = temp_output_617_0;
				#else
				float4 staticSwitch508 = temp_output_617_0;
				#endif
				float4 temp_output_22_0_g1066 = staticSwitch508;
				float4 _Vector2 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float3 break488 = UV298;
				float2 appendResult495 = (float2(break488.x , break488.y));
				float2 temp_output_5_0_g1063 = appendResult495;
				int temp_output_4_0_g1063 = (int)break488.z;
				float2 temp_output_9_0_g1063 = Mip101;
				float2 temp_output_12_0_g1063 = ddx( temp_output_9_0_g1063 );
				float2 temp_output_13_0_g1063 = ddy( temp_output_9_0_g1063 );
				float4 tex2DArrayNode3_g1063 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1063,(float)temp_output_4_0_g1063, temp_output_12_0_g1063, temp_output_13_0_g1063 );
				float localStochasticTiling190_g1064 = ( 0.0 );
				float2 Input_UV317_g1064 = temp_output_5_0_g1063;
				float2 UV190_g1064 = Input_UV317_g1064;
				float2 UV1190_g1064 = float2( 0,0 );
				float2 UV2190_g1064 = float2( 0,0 );
				float2 UV3190_g1064 = float2( 0,0 );
				float W1190_g1064 = 0.0;
				float W2190_g1064 = 0.0;
				float W3190_g1064 = 0.0;
				StochasticTiling( UV190_g1064 , UV1190_g1064 , UV2190_g1064 , UV3190_g1064 , W1190_g1064 , W2190_g1064 , W3190_g1064 );
				float Input_Index330_g1064 = (float)temp_output_4_0_g1063;
				float2 temp_output_358_0_g1064 = temp_output_12_0_g1063;
				float2 temp_output_359_0_g1064 = temp_output_13_0_g1063;
				float4 Output_2DArray152_g1064 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W1190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W2190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W3190_g1064 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1063 = Output_2DArray152_g1064;
				#else
				float4 staticSwitch7_g1063 = tex2DArrayNode3_g1063;
				#endif
				float4 ifLocalVar17_g1063 = 0;
				UNITY_BRANCH 
				if( break896.z > 0.0 )
				ifLocalVar17_g1063 = staticSwitch7_g1063;
				else if( break896.z == 0.0 )
				ifLocalVar17_g1063 = tex2DArrayNode3_g1063;
				float4 Color2798 = Out2795;
				float4 temp_output_618_0 = ( ifLocalVar17_g1063 * Color2798 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch509 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch509 = temp_output_618_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch509 = temp_output_618_0;
				#else
				float4 staticSwitch509 = _Vector2;
				#endif
				float4 temp_output_23_0_g1066 = staticSwitch509;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float3 break486 = UV399;
				float2 appendResult491 = (float2(break486.x , break486.y));
				float2 temp_output_5_0_g1059 = appendResult491;
				int temp_output_4_0_g1059 = (int)break486.z;
				float2 temp_output_9_0_g1059 = Mip101;
				float2 temp_output_12_0_g1059 = ddx( temp_output_9_0_g1059 );
				float2 temp_output_13_0_g1059 = ddy( temp_output_9_0_g1059 );
				float4 tex2DArrayNode3_g1059 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1059,(float)temp_output_4_0_g1059, temp_output_12_0_g1059, temp_output_13_0_g1059 );
				float localStochasticTiling190_g1060 = ( 0.0 );
				float2 Input_UV317_g1060 = temp_output_5_0_g1059;
				float2 UV190_g1060 = Input_UV317_g1060;
				float2 UV1190_g1060 = float2( 0,0 );
				float2 UV2190_g1060 = float2( 0,0 );
				float2 UV3190_g1060 = float2( 0,0 );
				float W1190_g1060 = 0.0;
				float W2190_g1060 = 0.0;
				float W3190_g1060 = 0.0;
				StochasticTiling( UV190_g1060 , UV1190_g1060 , UV2190_g1060 , UV3190_g1060 , W1190_g1060 , W2190_g1060 , W3190_g1060 );
				float Input_Index330_g1060 = (float)temp_output_4_0_g1059;
				float2 temp_output_358_0_g1060 = temp_output_12_0_g1059;
				float2 temp_output_359_0_g1060 = temp_output_13_0_g1059;
				float4 Output_2DArray152_g1060 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W1190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W2190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W3190_g1060 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1059 = Output_2DArray152_g1060;
				#else
				float4 staticSwitch7_g1059 = tex2DArrayNode3_g1059;
				#endif
				float4 ifLocalVar17_g1059 = 0;
				UNITY_BRANCH 
				if( break896.w > 0.0 )
				ifLocalVar17_g1059 = staticSwitch7_g1059;
				else if( break896.w == 0.0 )
				ifLocalVar17_g1059 = tex2DArrayNode3_g1059;
				float4 Color3799 = Out3795;
				#if defined( _QUALITY_FAST )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch510 = ( ifLocalVar17_g1059 * Color3799 );
				#else
				float4 staticSwitch510 = _Vector2;
				#endif
				float4 temp_output_24_0_g1066 = staticSwitch510;
				float4 weightedBlendVar30_g1066 = temp_output_14_0_g1066;
				float4 weightedBlend30_g1066 = ( weightedBlendVar30_g1066.x*temp_output_18_0_g1066 + weightedBlendVar30_g1066.y*temp_output_22_0_g1066 + weightedBlendVar30_g1066.z*temp_output_23_0_g1066 + weightedBlendVar30_g1066.w*temp_output_24_0_g1066 );
				float4 break899 = samplingType895;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float2 temp_output_9_0_g11 = Mip101;
				float2 temp_output_12_0_g11 = ddx( temp_output_9_0_g11 );
				float2 temp_output_13_0_g11 = ddy( temp_output_9_0_g11 );
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, temp_output_12_0_g11, temp_output_13_0_g11 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float2 temp_output_358_0_g12 = temp_output_12_0_g11;
				float2 temp_output_359_0_g12 = temp_output_13_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float2 temp_output_9_0_g9 = Mip101;
				float2 temp_output_12_0_g9 = ddx( temp_output_9_0_g9 );
				float2 temp_output_13_0_g9 = ddy( temp_output_9_0_g9 );
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, temp_output_12_0_g9, temp_output_13_0_g9 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float2 temp_output_358_0_g10 = temp_output_12_0_g9;
				float2 temp_output_359_0_g10 = temp_output_13_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float2 temp_output_9_0_g7 = Mip101;
				float2 temp_output_12_0_g7 = ddx( temp_output_9_0_g7 );
				float2 temp_output_13_0_g7 = ddy( temp_output_9_0_g7 );
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, temp_output_12_0_g7, temp_output_13_0_g7 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float2 temp_output_358_0_g8 = temp_output_12_0_g7;
				float2 temp_output_359_0_g8 = temp_output_13_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float2 temp_output_9_0_g1 = Mip101;
				float2 temp_output_12_0_g1 = ddx( temp_output_9_0_g1 );
				float2 temp_output_13_0_g1 = ddy( temp_output_9_0_g1 );
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, temp_output_12_0_g1, temp_output_13_0_g1 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float2 temp_output_358_0_g6 = temp_output_12_0_g1;
				float2 temp_output_359_0_g6 = temp_output_13_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1066 = HeightRawCombined0199;
				float4 break15_g1066 = temp_output_14_0_g1066;
				float temp_output_53_0_g1066 = ( break13_g1066.x + break15_g1066.x );
				float temp_output_54_0_g1066 = ( break13_g1066.y + break15_g1066.y );
				float temp_output_55_0_g1066 = ( break13_g1066.z + break15_g1066.z );
				float temp_output_56_0_g1066 = ( break13_g1066.w + break15_g1066.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1066 = ( max( max( max( temp_output_53_0_g1066 , temp_output_54_0_g1066 ) , temp_output_55_0_g1066 ) , temp_output_56_0_g1066 ) - HeightBlending854 );
				float temp_output_63_0_g1066 = max( ( temp_output_53_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_67_0_g1066 = max( ( temp_output_54_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_71_0_g1066 = max( ( temp_output_55_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_73_0_g1066 = max( ( temp_output_56_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1066 = lerp( weightedBlend30_g1066 , ( ( ( temp_output_18_0_g1066 * temp_output_63_0_g1066 ) + ( temp_output_22_0_g1066 * temp_output_67_0_g1066 ) + ( temp_output_23_0_g1066 * temp_output_71_0_g1066 ) + ( temp_output_24_0_g1066 * temp_output_73_0_g1066 ) ) / ( temp_output_63_0_g1066 + temp_output_67_0_g1066 + temp_output_71_0_g1066 + temp_output_73_0_g1066 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1066 = lerpResult78_g1066;
				#else
				float4 staticSwitch77_g1066 = weightedBlend30_g1066;
				#endif
				float4 Albedo0520 = staticSwitch77_g1066;
				float4 appendResult179 = (float4(WorldPosition.x , WorldPosition.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Snow_Amount174 = _EnviroSnow;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( WorldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float4 lerpResult524 = lerp( float4( 1,1,1,0 ) , _PuddleColor , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch543 = ( Albedo0520 * lerpResult524 );
				#else
				float4 staticSwitch543 = Albedo0520;
				#endif
				float2 temp_cast_63 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord9.xy * temp_cast_63 + float2( 0,0 );
				float2 temp_output_5_0_g1067 = texCoord232;
				float localStochasticTiling2_g1068 = ( 0.0 );
				float2 Input_UV145_g1068 = temp_output_5_0_g1067;
				float2 UV2_g1068 = Input_UV145_g1068;
				float2 UV12_g1068 = float2( 0,0 );
				float2 UV22_g1068 = float2( 0,0 );
				float2 UV32_g1068 = float2( 0,0 );
				float W12_g1068 = 0.0;
				float W22_g1068 = 0.0;
				float W32_g1068 = 0.0;
				StochasticTiling( UV2_g1068 , UV12_g1068 , UV22_g1068 , UV32_g1068 , W12_g1068 , W22_g1068 , W32_g1068 );
				float2 temp_output_10_0_g1068 = ddx( Input_UV145_g1068 );
				float2 temp_output_12_0_g1068 = ddy( Input_UV145_g1068 );
				float4 Output_2D293_g1068 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV12_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W12_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV22_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W22_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV32_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W32_g1068 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1067 = Output_2D293_g1068;
				#else
				float4 staticSwitch7_g1067 = SAMPLE_TEXTURE2D( _SnowAlbedo, sampler_SnowAlbedo, temp_output_5_0_g1067 );
				#endif
				float4 Snow_Albedo522 = staticSwitch7_g1067;
				float4 temp_output_14_0_g1053 = SplatWeights198;
				float4 break898 = samplingType895;
				float2 temp_output_5_0_g1049 = UV0100.xy;
				float4 break391 = SplatIndex44;
				int temp_output_4_0_g1049 = (int)break391.x;
				float2 temp_output_9_0_g1049 = Mip101;
				float2 temp_output_12_0_g1049 = ddx( temp_output_9_0_g1049 );
				float2 temp_output_13_0_g1049 = ddy( temp_output_9_0_g1049 );
				float4 tex2DArrayNode3_g1049 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1049,(float)temp_output_4_0_g1049, temp_output_12_0_g1049, temp_output_13_0_g1049 );
				float localStochasticTiling190_g1050 = ( 0.0 );
				float2 Input_UV317_g1050 = temp_output_5_0_g1049;
				float2 UV190_g1050 = Input_UV317_g1050;
				float2 UV1190_g1050 = float2( 0,0 );
				float2 UV2190_g1050 = float2( 0,0 );
				float2 UV3190_g1050 = float2( 0,0 );
				float W1190_g1050 = 0.0;
				float W2190_g1050 = 0.0;
				float W3190_g1050 = 0.0;
				StochasticTiling( UV190_g1050 , UV1190_g1050 , UV2190_g1050 , UV3190_g1050 , W1190_g1050 , W2190_g1050 , W3190_g1050 );
				float Input_Index330_g1050 = (float)temp_output_4_0_g1049;
				float2 temp_output_358_0_g1050 = temp_output_12_0_g1049;
				float2 temp_output_359_0_g1050 = temp_output_13_0_g1049;
				float4 Output_2DArray152_g1050 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W1190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W2190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W3190_g1050 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1049 = Output_2DArray152_g1050;
				#else
				float4 staticSwitch7_g1049 = tex2DArrayNode3_g1049;
				#endif
				float4 ifLocalVar17_g1049 = 0;
				UNITY_BRANCH 
				if( break898.x > 0.0 )
				ifLocalVar17_g1049 = staticSwitch7_g1049;
				else if( break898.x == 0.0 )
				ifLocalVar17_g1049 = tex2DArrayNode3_g1049;
				float localGetLayerSettings368 = ( 0.0 );
				float4 in0368 = _NormalScale00;
				float4 in1368 = _NormalScale01;
				float4 in2368 = _NormalScale02;
				float4 index368 = SplatIndex44;
				float4 Out0368 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0368,in1368,in2368,index368,Out0368);
				}
				float4 NormalScales375 = Out0368;
				float4 break401 = NormalScales375;
				float3 unpack417 = UnpackNormalScale( ifLocalVar17_g1049, break401.x );
				unpack417.z = lerp( 1, unpack417.z, saturate(break401.x) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch433 = unpack417;
				#else
				float3 staticSwitch433 = unpack417;
				#endif
				float4 temp_output_18_0_g1053 = float4( staticSwitch433 , 0.0 );
				float2 temp_output_5_0_g1045 = UV197.xy;
				int temp_output_4_0_g1045 = (int)break391.y;
				float2 temp_output_9_0_g1045 = Mip101;
				float2 temp_output_12_0_g1045 = ddx( temp_output_9_0_g1045 );
				float2 temp_output_13_0_g1045 = ddy( temp_output_9_0_g1045 );
				float4 tex2DArrayNode3_g1045 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1045,(float)temp_output_4_0_g1045, temp_output_12_0_g1045, temp_output_13_0_g1045 );
				float localStochasticTiling190_g1046 = ( 0.0 );
				float2 Input_UV317_g1046 = temp_output_5_0_g1045;
				float2 UV190_g1046 = Input_UV317_g1046;
				float2 UV1190_g1046 = float2( 0,0 );
				float2 UV2190_g1046 = float2( 0,0 );
				float2 UV3190_g1046 = float2( 0,0 );
				float W1190_g1046 = 0.0;
				float W2190_g1046 = 0.0;
				float W3190_g1046 = 0.0;
				StochasticTiling( UV190_g1046 , UV1190_g1046 , UV2190_g1046 , UV3190_g1046 , W1190_g1046 , W2190_g1046 , W3190_g1046 );
				float Input_Index330_g1046 = (float)temp_output_4_0_g1045;
				float2 temp_output_358_0_g1046 = temp_output_12_0_g1045;
				float2 temp_output_359_0_g1046 = temp_output_13_0_g1045;
				float4 Output_2DArray152_g1046 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W1190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W2190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W3190_g1046 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1045 = Output_2DArray152_g1046;
				#else
				float4 staticSwitch7_g1045 = tex2DArrayNode3_g1045;
				#endif
				float4 ifLocalVar17_g1045 = 0;
				UNITY_BRANCH 
				if( break898.y > 0.0 )
				ifLocalVar17_g1045 = staticSwitch7_g1045;
				else if( break898.y == 0.0 )
				ifLocalVar17_g1045 = tex2DArrayNode3_g1045;
				float3 unpack416 = UnpackNormalScale( ifLocalVar17_g1045, break401.y );
				unpack416.z = lerp( 1, unpack416.z, saturate(break401.y) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch434 = unpack416;
				#else
				float3 staticSwitch434 = unpack416;
				#endif
				float4 temp_output_22_0_g1053 = float4( staticSwitch434 , 0.0 );
				float4 _Vector3 = float4(0,0,0,0);
				float2 temp_output_5_0_g1051 = UV298.xy;
				int temp_output_4_0_g1051 = (int)break391.z;
				float2 temp_output_9_0_g1051 = Mip101;
				float2 temp_output_12_0_g1051 = ddx( temp_output_9_0_g1051 );
				float2 temp_output_13_0_g1051 = ddy( temp_output_9_0_g1051 );
				float4 tex2DArrayNode3_g1051 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1051,(float)temp_output_4_0_g1051, temp_output_12_0_g1051, temp_output_13_0_g1051 );
				float localStochasticTiling190_g1052 = ( 0.0 );
				float2 Input_UV317_g1052 = temp_output_5_0_g1051;
				float2 UV190_g1052 = Input_UV317_g1052;
				float2 UV1190_g1052 = float2( 0,0 );
				float2 UV2190_g1052 = float2( 0,0 );
				float2 UV3190_g1052 = float2( 0,0 );
				float W1190_g1052 = 0.0;
				float W2190_g1052 = 0.0;
				float W3190_g1052 = 0.0;
				StochasticTiling( UV190_g1052 , UV1190_g1052 , UV2190_g1052 , UV3190_g1052 , W1190_g1052 , W2190_g1052 , W3190_g1052 );
				float Input_Index330_g1052 = (float)temp_output_4_0_g1051;
				float2 temp_output_358_0_g1052 = temp_output_12_0_g1051;
				float2 temp_output_359_0_g1052 = temp_output_13_0_g1051;
				float4 Output_2DArray152_g1052 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W1190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W2190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W3190_g1052 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1051 = Output_2DArray152_g1052;
				#else
				float4 staticSwitch7_g1051 = tex2DArrayNode3_g1051;
				#endif
				float4 ifLocalVar17_g1051 = 0;
				UNITY_BRANCH 
				if( break898.z > 0.0 )
				ifLocalVar17_g1051 = staticSwitch7_g1051;
				else if( break898.z == 0.0 )
				ifLocalVar17_g1051 = tex2DArrayNode3_g1051;
				float3 unpack414 = UnpackNormalScale( ifLocalVar17_g1051, break401.z );
				unpack414.z = lerp( 1, unpack414.z, saturate(break401.z) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch435 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#else
				float4 staticSwitch435 = _Vector3;
				#endif
				float4 temp_output_23_0_g1053 = staticSwitch435;
				float2 temp_output_5_0_g1047 = UV399.xy;
				int temp_output_4_0_g1047 = (int)break391.w;
				float2 temp_output_9_0_g1047 = Mip101;
				float2 temp_output_12_0_g1047 = ddx( temp_output_9_0_g1047 );
				float2 temp_output_13_0_g1047 = ddy( temp_output_9_0_g1047 );
				float4 tex2DArrayNode3_g1047 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1047,(float)temp_output_4_0_g1047, temp_output_12_0_g1047, temp_output_13_0_g1047 );
				float localStochasticTiling190_g1048 = ( 0.0 );
				float2 Input_UV317_g1048 = temp_output_5_0_g1047;
				float2 UV190_g1048 = Input_UV317_g1048;
				float2 UV1190_g1048 = float2( 0,0 );
				float2 UV2190_g1048 = float2( 0,0 );
				float2 UV3190_g1048 = float2( 0,0 );
				float W1190_g1048 = 0.0;
				float W2190_g1048 = 0.0;
				float W3190_g1048 = 0.0;
				StochasticTiling( UV190_g1048 , UV1190_g1048 , UV2190_g1048 , UV3190_g1048 , W1190_g1048 , W2190_g1048 , W3190_g1048 );
				float Input_Index330_g1048 = (float)temp_output_4_0_g1047;
				float2 temp_output_358_0_g1048 = temp_output_12_0_g1047;
				float2 temp_output_359_0_g1048 = temp_output_13_0_g1047;
				float4 Output_2DArray152_g1048 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W1190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W2190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W3190_g1048 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1047 = Output_2DArray152_g1048;
				#else
				float4 staticSwitch7_g1047 = tex2DArrayNode3_g1047;
				#endif
				float4 ifLocalVar17_g1047 = 0;
				UNITY_BRANCH 
				if( break898.w > 0.0 )
				ifLocalVar17_g1047 = staticSwitch7_g1047;
				else if( break898.w == 0.0 )
				ifLocalVar17_g1047 = tex2DArrayNode3_g1047;
				float3 unpack415 = UnpackNormalScale( ifLocalVar17_g1047, break401.w );
				unpack415.z = lerp( 1, unpack415.z, saturate(break401.w) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch436 = float4( unpack415 , 0.0 );
				#else
				float4 staticSwitch436 = _Vector3;
				#endif
				float4 temp_output_24_0_g1053 = staticSwitch436;
				float4 weightedBlendVar30_g1053 = temp_output_14_0_g1053;
				float4 weightedBlend30_g1053 = ( weightedBlendVar30_g1053.x*temp_output_18_0_g1053 + weightedBlendVar30_g1053.y*temp_output_22_0_g1053 + weightedBlendVar30_g1053.z*temp_output_23_0_g1053 + weightedBlendVar30_g1053.w*temp_output_24_0_g1053 );
				float4 break13_g1053 = HeightRawCombined0199;
				float4 break15_g1053 = temp_output_14_0_g1053;
				float temp_output_53_0_g1053 = ( break13_g1053.x + break15_g1053.x );
				float temp_output_54_0_g1053 = ( break13_g1053.y + break15_g1053.y );
				float temp_output_55_0_g1053 = ( break13_g1053.z + break15_g1053.z );
				float temp_output_56_0_g1053 = ( break13_g1053.w + break15_g1053.w );
				float temp_output_79_0_g1053 = ( max( max( max( temp_output_53_0_g1053 , temp_output_54_0_g1053 ) , temp_output_55_0_g1053 ) , temp_output_56_0_g1053 ) - HeightBlending854 );
				float temp_output_63_0_g1053 = max( ( temp_output_53_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_67_0_g1053 = max( ( temp_output_54_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_71_0_g1053 = max( ( temp_output_55_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_73_0_g1053 = max( ( temp_output_56_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float4 lerpResult78_g1053 = lerp( weightedBlend30_g1053 , ( ( ( temp_output_18_0_g1053 * temp_output_63_0_g1053 ) + ( temp_output_22_0_g1053 * temp_output_67_0_g1053 ) + ( temp_output_23_0_g1053 * temp_output_71_0_g1053 ) + ( temp_output_24_0_g1053 * temp_output_73_0_g1053 ) ) / ( temp_output_63_0_g1053 + temp_output_67_0_g1053 + temp_output_71_0_g1053 + temp_output_73_0_g1053 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1053 = lerpResult78_g1053;
				#else
				float4 staticSwitch77_g1053 = weightedBlend30_g1053;
				#endif
				float4 Normal0450 = staticSwitch77_g1053;
				float temp_output_395_0 = ( _TimeParameters.x * 0.05 );
				float2 appendResult379 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_397_0 = ( appendResult379 * _PuddleWaveTiling );
				float2 panner408 = ( temp_output_395_0 * float2( 1,0 ) + temp_output_397_0);
				float temp_output_406_0 = ( Puddle_Mask264 * ( _PuddleWaveIntensity * Wetness228 ) );
				float3 unpack420 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner408 ), temp_output_406_0 );
				unpack420.z = lerp( 1, unpack420.z, saturate(temp_output_406_0) );
				float2 panner407 = ( temp_output_395_0 * float2( 0,1 ) + temp_output_397_0);
				float3 unpack419 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner407 ), temp_output_406_0 );
				unpack419.z = lerp( 1, unpack419.z, saturate(temp_output_406_0) );
				float3 Puddle447 = BlendNormal( unpack420 , unpack419 );
				float4 lerpResult457 = lerp( Normal0450 , float4( Puddle447 , 0.0 ) , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch462 = lerpResult457;
				#else
				float4 staticSwitch462 = Normal0450;
				#endif
				float Rain_Intensity303 = _EnviroRainIntensity;
				float temp_output_325_0 = (1.0 + (( _RainFlowStrength * Rain_Intensity303 ) - 0.0) * (-1.0 - 1.0) / (1.0 - 0.0));
				float temp_output_306_0 = ( _TimeParameters.x * 0.05 );
				float4 transform287 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord10.xyz , 0.0 ));
				float2 appendResult298 = (float2(( transform287.z * 0.7 ) , ( transform287.y * 0.2 )));
				float2 panner313 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult298 * _RainFlowTiling ));
				float2 texCoord285 = input.ase_texcoord9.xy * float2( 10,10 ) + float2( 0,0 );
				float gradientNoise289 = UnityGradientNoise(texCoord285,_RainFlowDistortionScale);
				gradientNoise289 = gradientNoise289*0.5 + 0.5;
				float Distortion307 = ( gradientNoise289 * _RainFlowDistortionStrenght );
				float simpleNoise324 = SimpleNoise( ( panner313 + Distortion307 )*100.0 );
				simpleNoise324 = simpleNoise324*2 - 1;
				float smoothstepResult332 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise324);
				float temp_output_335_0 = ( ( ( WorldNormal.y - 0.95 ) * -1.0 ) * _RainFlowIntensity );
				float3 temp_cast_99 = (0.3).xxx;
				float3 break337 = ( abs( WorldNormal ) - temp_cast_99 );
				float lerpResult342 = lerp( 0.0 , ( smoothstepResult332 * temp_output_335_0 ) , break337.x);
				float4 transform286 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord10.xyz , 0.0 ));
				float2 appendResult299 = (float2(( transform286.x * 0.7 ) , ( transform286.y * 0.2 )));
				float2 panner312 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult299 * _RainFlowTiling ));
				float simpleNoise328 = SimpleNoise( ( panner312 + Distortion307 )*100.0 );
				simpleNoise328 = simpleNoise328*2 - 1;
				float smoothstepResult333 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise328);
				float lerpResult341 = lerp( 0.0 , ( smoothstepResult333 * temp_output_335_0 ) , break337.z);
				float Rain_Distance_Fade340 = ( 1.0 - sqrt( saturate( ( distance( WorldPosition , _WorldSpaceCameraPos ) / _RainDistanceFade ) ) ) );
				float temp_output_366_0 = saturate( ( ( lerpResult342 + lerpResult341 ) * Rain_Distance_Fade340 ) );
				float temp_output_373_0 = ddx( temp_output_366_0 );
				float temp_output_384_0 = ddy( temp_output_366_0 );
				float3 appendResult445 = (float3(temp_output_373_0 , temp_output_384_0 , sqrt( ( ( 1.0 - ( temp_output_373_0 * temp_output_373_0 ) ) - ( temp_output_384_0 * temp_output_384_0 ) ) )));
				float3 normalizeResult449 = normalize( appendResult445 );
				float3 RainFlow453 = normalizeResult449;
				float localRainRipples1_g1054 = ( 0.0 );
				float2 appendResult426 = (float2(WorldPosition.x , WorldPosition.z));
				float2 UV1_g1054 = ( appendResult426 * _RainDropTiling );
				float AngleOffset1_g1054 = 5.0;
				float lerpResult428 = lerp( 64.0 , 12.0 , Puddle_Mask264);
				float CellDensity1_g1054 = round( lerpResult428 );
				float Time1_g1054 = ( _TimeParameters.x * _RainDropSpeed );
				float temp_output_358_0 = ( _RainDropIntensity * 1.5 );
				float lerpResult365 = lerp( _RainDropIntensity , temp_output_358_0 , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float staticSwitch372 = lerpResult365;
				#else
				float staticSwitch372 = temp_output_358_0;
				#endif
				float switchResult422 = (((ase_vface>0)?(( ( ( WorldNormal.y - 0.7 ) * ( staticSwitch372 * Rain_Intensity303 ) ) * Rain_Distance_Fade340 )):(0.0)));
				float Strength1_g1054 = max( 0.0 , switchResult422 );
				float3 normal1_g1054 = float3( 0,0,0 );
				float Out1_g1054 = 0.0;
				float lerpResult440 = lerp( 5.0 , 8.0 , Puddle_Mask264);
				float pow1_g1054 = lerpResult440;
				float lerpResult439 = lerp( 1.0 , 0.0 , Puddle_Mask264);
				float sin1_g1054 = lerpResult439;
				{
				Rain(UV1_g1054,AngleOffset1_g1054,CellDensity1_g1054,Time1_g1054,Strength1_g1054,pow1_g1054,sin1_g1054,Out1_g1054,normal1_g1054);
				}
				float3 Rain_Drop452 = normal1_g1054;
				#ifdef _RAIN_ON
				float4 staticSwitch468 = float4( BlendNormal( staticSwitch462.xyz , BlendNormal( RainFlow453 , Rain_Drop452 ) ) , 0.0 );
				#else
				float4 staticSwitch468 = staticSwitch462;
				#endif
				float2 temp_output_5_0_g1055 = texCoord232;
				float localStochasticTiling2_g1056 = ( 0.0 );
				float2 Input_UV145_g1056 = temp_output_5_0_g1055;
				float2 UV2_g1056 = Input_UV145_g1056;
				float2 UV12_g1056 = float2( 0,0 );
				float2 UV22_g1056 = float2( 0,0 );
				float2 UV32_g1056 = float2( 0,0 );
				float W12_g1056 = 0.0;
				float W22_g1056 = 0.0;
				float W32_g1056 = 0.0;
				StochasticTiling( UV2_g1056 , UV12_g1056 , UV22_g1056 , UV32_g1056 , W12_g1056 , W22_g1056 , W32_g1056 );
				float2 temp_output_10_0_g1056 = ddx( Input_UV145_g1056 );
				float2 temp_output_12_0_g1056 = ddy( Input_UV145_g1056 );
				float4 Output_2D293_g1056 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV12_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W12_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV22_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W22_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV32_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W32_g1056 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1055 = Output_2D293_g1056;
				#else
				float4 staticSwitch7_g1055 = SAMPLE_TEXTURE2D( _SnowNormal, sampler_SnowNormal, temp_output_5_0_g1055 );
				#endif
				float3 unpack463 = UnpackNormalScale( staticSwitch7_g1055, _SnowNormalScale );
				unpack463.z = lerp( 1, unpack463.z, saturate(_SnowNormalScale) );
				float3 Snow_Normal465 = unpack463;
				float2 appendResult202 = (float2(WorldPosition.x , WorldPosition.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( WorldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord10.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord10.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float4 lerpResult470 = lerp( staticSwitch468 , float4( Snow_Normal465 , 0.0 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch471 = lerpResult470;
				#else
				float4 staticSwitch471 = staticSwitch468;
				#endif
				float4 Normal_Final472 = staticSwitch471;
				float3x3 ase_tangentToWorldFast = float3x3(WorldTangent.x,WorldBiTangent.x,WorldNormal.x,WorldTangent.y,WorldBiTangent.y,WorldNormal.y,WorldTangent.z,WorldBiTangent.z,WorldNormal.z);
				float3 tangentToWorldDir474 = mul( ase_tangentToWorldFast, Normal_Final472.xyz );
				float dotResult497 = dot( WorldViewDirection , -( _MainLightPosition.xyz + ( tangentToWorldDir474 * _SSSDistortion ) ) );
				float dotResult504 = dot( dotResult497 , _SSSScale );
				float SSS523 = ( saturate( dotResult504 ) * _SSSIntensity );
				float4 lerpResult553 = lerp( staticSwitch543 , ( Snow_Albedo522 + SSS523 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch562 = lerpResult553;
				#else
				float4 staticSwitch562 = staticSwitch543;
				#endif
				float4 Albedo_Final575 = ( staticSwitch562 + ( Wetness228 * -0.02 ) );
				float4 localClipHoles583 = ( Albedo_Final575 );
				float2 uv_TerrainHolesTexture = input.ase_texcoord9.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Hole583 = holeClipValue579;
				{
				clip(Hole583 == 0.0f ? -1 : 1);
				}
				float4 AlbedoCombined586 = localClipHoles583;
				
				float4 break668 = Normal_Final472;
				float3 appendResult671 = (float3(break668.x , break668.y , ( break668.z + 0.001 )));
				#ifdef _TERRAIN_INSTANCED_PERPIXEL_NORMAL
				float3 staticSwitch665 = appendResult671;
				#else
				float3 staticSwitch665 = appendResult671;
				#endif
				
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Metallic0569 = break245.x;
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float2 temp_output_10_0_g1044 = ddx( Input_UV145_g1044 );
				float2 temp_output_12_0_g1044 = ddy( Input_UV145_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowMask, sampler_SnowMask, UV12_g1044, temp_output_10_0_g1044, temp_output_12_0_g1044 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowMask, sampler_SnowMask, UV22_g1044, temp_output_10_0_g1044, temp_output_12_0_g1044 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowMask, sampler_SnowMask, UV32_g1044, temp_output_10_0_g1044, temp_output_12_0_g1044 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Metallic563 = ( break244.r + _SnowMetallic );
				float lerpResult577 = lerp( Metallic0569 , Snow_Metallic563 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch581 = lerpResult577;
				#else
				float staticSwitch581 = Metallic0569;
				#endif
				float Metallic_Final584 = staticSwitch581;
				
				float Smoothness0540 = break245.w;
				float Snow_Smoothness536 = ( break244.a * _SnowSmoothness );
				float lerpResult559 = lerp( Smoothness0540 , Snow_Smoothness536 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch564 = lerpResult559;
				#else
				float staticSwitch564 = Smoothness0540;
				#endif
				#ifdef _RAIN_ON
				float staticSwitch544 = ( Out1_g1054 * 0.2 );
				#else
				float staticSwitch544 = 0.0;
				#endif
				float RainDropSmoothness555 = staticSwitch544;
				#ifdef _RAIN_ON
				float staticSwitch545 = ( temp_output_366_0 * _RainFlowSmoothnessBoost );
				#else
				float staticSwitch545 = 0.0;
				#endif
				float RainFlowSmoothness557 = staticSwitch545;
				float Smoothness_Final585 = saturate( ( ( staticSwitch564 + ( ( ( _WetnessBoost * Wetness228 ) + saturate( ( Puddle_Mask264 - 0.2 ) ) ) * ( 1.0 - Snow_Blending247 ) ) ) + ( RainDropSmoothness555 + RainFlowSmoothness557 ) ) );
				
				float Occlusion0589 = break245.y;
				float Snow_Occlusion588 = break244.g;
				float lerpResult593 = lerp( Occlusion0589 , Snow_Occlusion588 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch594 = lerpResult593;
				#else
				float staticSwitch594 = Occlusion0589;
				#endif
				float Occlusion_Final595 = staticSwitch594;
				
				float Alpha1008 = holeClipValue579;
				

				float3 BaseColor = AlbedoCombined586.xyz;
				float3 Normal = staticSwitch665;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = Metallic_Final584;
				float Smoothness = Smoothness_Final585;
				float Occlusion = Occlusion_Final595;
				float Alpha = Alpha1008;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _CLEARCOAT
					float CoatMask = 0;
					float CoatSmoothness = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.positionCS = input.positionCS;
				inputData.viewDirectionWS = WorldViewDirection;

				#ifdef _NORMALMAP
						#if _NORMAL_DROPOFF_TS
							inputData.normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent, WorldBiTangent, WorldNormal));
						#elif _NORMAL_DROPOFF_OS
							inputData.normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							inputData.normalWS = Normal;
						#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = ShadowCoords;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				#else
					inputData.shadowCoord = float4(0, 0, 0, 0);
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = input.fogFactorAndVertexLight.x;
				#endif
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI( SH, GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask );
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
					#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				SurfaceData surfaceData;
				surfaceData.albedo              = BaseColor;
				surfaceData.metallic            = saturate(Metallic);
				surfaceData.specular            = Specular;
				surfaceData.smoothness          = saturate(Smoothness),
				surfaceData.occlusion           = Occlusion,
				surfaceData.emission            = Emission,
				surfaceData.alpha               = saturate(Alpha);
				surfaceData.normalTS            = Normal;
				surfaceData.clearCoatMask       = 0;
				surfaceData.clearCoatSmoothness = 1;

				#ifdef _CLEARCOAT
					surfaceData.clearCoatMask       = saturate(CoatMask);
					surfaceData.clearCoatSmoothness = saturate(CoatSmoothness);
				#endif

				#ifdef _DBUFFER
					ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
				#endif

				#ifdef _ASE_LIGHTING_SIMPLE
					half4 color = UniversalFragmentBlinnPhong( inputData, surfaceData);
				#else
					half4 color = UniversalFragmentPBR( inputData, surfaceData);
				#endif

				#ifdef ASE_TRANSMISSION
				{
					float shadow = _TransmissionShadow;

					#define SUM_LIGHT_TRANSMISSION(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 transmission = max( 0, -dot( inputData.normalWS, Light.direction ) ) * atten * Transmission;\
						color.rgb += BaseColor * transmission;

					SUM_LIGHT_TRANSMISSION( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSMISSION( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSMISSION( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_TRANSLUCENCY
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					#define SUM_LIGHT_TRANSLUCENCY(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 lightDir = Light.direction + inputData.normalWS * normal;\
						half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );\
						half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;\
						color.rgb += BaseColor * translucency * strength;

					SUM_LIGHT_TRANSLUCENCY( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSLUCENCY( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSLUCENCY( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_REFRACTION
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), input.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, input.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#include "EnviroInclude.hlsl"


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif				
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			

			float3 _LightDirection;
			float3 _LightPosition;

			PackedVaryings VertexFunction( Attributes input )
			{
				PackedVaryings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir(input.normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = positionCS;
				output.clipPosV = positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(	PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_TerrainHolesTexture = input.ase_texcoord3.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Alpha1008 = holeClipValue579;
				

				float Alpha = Alpha1008;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#include "EnviroInclude.hlsl"


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(	PackedVaryings input
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_TerrainHolesTexture = input.ase_texcoord3.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Alpha1008 = holeClipValue579;
				

				float Alpha = Alpha1008;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1

			#pragma shader_feature EDITOR_VISUALIZATION

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#pragma multi_compile_local _RAIN_ON
			#include "EnviroInclude.hlsl"


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef EDITOR_VISUALIZATION
					float4 VizUV : TEXCOORD2;
					float4 LightCoord : TEXCOORD3;
				#endif
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D_ARRAY(_AlbedoArray);
			SAMPLER(sampler_AlbedoArray);
			TEXTURE2D(_SnowAlbedo);
			SAMPLER(sampler_SnowAlbedo);
			TEXTURE2D_ARRAY(_NormalArray);
			SAMPLER(sampler_NormalArray);
			TEXTURE2D(_WaveNormal);
			SAMPLER(sampler_WaveNormal);
			TEXTURE2D(_SnowNormal);
			SAMPLER(sampler_SnowNormal);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			
			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.texcoord0.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.texcoord0.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.texcoord0.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.texcoord0.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				output.ase_texcoord6.xyz = ase_worldNormal;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord7.xyz = ase_worldTangent;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord8.xyz = ase_worldBitangent;
				
				output.ase_texcoord4.xy = input.texcoord0.xy;
				output.ase_texcoord5 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord4.zw = 0;
				output.ase_texcoord6.w = 0;
				output.ase_texcoord7.w = 0;
				output.ase_texcoord8.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = positionWS;
				#endif

				output.positionCS = MetaVertexPosition( input.positionOS, input.texcoord1.xy, input.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );

				#ifdef EDITOR_VISUALIZATION
					float2 VizUV = 0;
					float4 LightCoord = 0;
					UnityEditorVizData(input.positionOS.xyz, input.texcoord0.xy, input.texcoord1.xy, input.texcoord2.xy, VizUV, LightCoord);
					output.VizUV = float4(VizUV, 0, 0);
					output.LightCoord = LightCoord;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = output.positionCS;
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.texcoord0 = input.texcoord0;
				output.texcoord1 = input.texcoord1;
				output.texcoord2 = input.texcoord2;
				output.ase_tangent = input.ase_tangent;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.texcoord0 = patch[0].texcoord0 * bary.x + patch[1].texcoord0 * bary.y + patch[2].texcoord0 * bary.z;
				output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input , bool ase_vface : SV_IsFrontFace ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord4.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord4.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D( _Control1, sampler_Control1, uv_Control1 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord4.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D( _Control2, sampler_Control2, uv_Control2 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1066 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break896 = samplingType895;
				float2 appendResult69 = (float2(input.ase_texcoord5.xyz.x , input.ase_texcoord5.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float3 break485 = UV0100;
				float2 appendResult493 = (float2(break485.x , break485.y));
				float2 temp_output_5_0_g1057 = appendResult493;
				int temp_output_4_0_g1057 = (int)break485.z;
				float2 appendResult87 = (float2(input.ase_texcoord5.xyz.x , input.ase_texcoord5.xyz.z));
				float2 Mip101 = ( appendResult87 * ( 1.0 / max( 0.001 , _MipDistanceBlending ) ) );
				float2 temp_output_9_0_g1057 = Mip101;
				float2 temp_output_12_0_g1057 = ddx( temp_output_9_0_g1057 );
				float2 temp_output_13_0_g1057 = ddy( temp_output_9_0_g1057 );
				float4 tex2DArrayNode3_g1057 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1057,(float)temp_output_4_0_g1057, temp_output_12_0_g1057, temp_output_13_0_g1057 );
				float localStochasticTiling190_g1058 = ( 0.0 );
				float2 Input_UV317_g1058 = temp_output_5_0_g1057;
				float2 UV190_g1058 = Input_UV317_g1058;
				float2 UV1190_g1058 = float2( 0,0 );
				float2 UV2190_g1058 = float2( 0,0 );
				float2 UV3190_g1058 = float2( 0,0 );
				float W1190_g1058 = 0.0;
				float W2190_g1058 = 0.0;
				float W3190_g1058 = 0.0;
				StochasticTiling( UV190_g1058 , UV1190_g1058 , UV2190_g1058 , UV3190_g1058 , W1190_g1058 , W2190_g1058 , W3190_g1058 );
				float Input_Index330_g1058 = (float)temp_output_4_0_g1057;
				float2 temp_output_358_0_g1058 = temp_output_12_0_g1057;
				float2 temp_output_359_0_g1058 = temp_output_13_0_g1057;
				float4 Output_2DArray152_g1058 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W1190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W2190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W3190_g1058 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1057 = Output_2DArray152_g1058;
				#else
				float4 staticSwitch7_g1057 = tex2DArrayNode3_g1057;
				#endif
				float4 ifLocalVar17_g1057 = 0;
				UNITY_BRANCH 
				if( break896.x > 0.0 )
				ifLocalVar17_g1057 = staticSwitch7_g1057;
				else if( break896.x == 0.0 )
				ifLocalVar17_g1057 = tex2DArrayNode3_g1057;
				float localGetUVS795 = ( 0.0 );
				float4 in0795 = _ColorTint0;
				float4 in1795 = _ColorTint1;
				float4 in2795 = _ColorTint2;
				float4 in3795 = _ColorTint3;
				float4 in4795 = _ColorTint4;
				float4 in5795 = _ColorTint5;
				float4 in6795 = _ColorTint6;
				float4 in7795 = _ColorTint7;
				float4 in8795 = _ColorTint8;
				float4 in9795 = _ColorTint9;
				float4 in10795 = _ColorTint10;
				float4 in11795 = _ColorTint11;
				float4 index795 = SplatIndex44;
				float4 Out0795 = float4( 0,0,0,0 );
				float4 Out1795 = float4( 0,0,0,0 );
				float4 Out2795 = float4( 0,0,0,0 );
				float4 Out3795 = float4( 0,0,0,0 );
				{
				GetLayerUV(in0795,in1795,in2795,in3795,in4795,in5795,in6795,in7795,in8795,in9795,in10795,in11795,index795,Out0795,Out1795,Out2795,Out3795);
				}
				float4 Color0796 = Out0795;
				float4 temp_output_616_0 = ( ifLocalVar17_g1057 * Color0796 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch511 = temp_output_616_0;
				#else
				float4 staticSwitch511 = temp_output_616_0;
				#endif
				float4 temp_output_2_0_g1065 = staticSwitch511;
				float4 temp_output_18_0_g1066 = float4( (temp_output_2_0_g1065).rgb , 0.0 );
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float3 break487 = UV197;
				float2 appendResult492 = (float2(break487.x , break487.y));
				float2 temp_output_5_0_g1061 = appendResult492;
				int temp_output_4_0_g1061 = (int)break487.z;
				float2 temp_output_9_0_g1061 = Mip101;
				float2 temp_output_12_0_g1061 = ddx( temp_output_9_0_g1061 );
				float2 temp_output_13_0_g1061 = ddy( temp_output_9_0_g1061 );
				float4 tex2DArrayNode3_g1061 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1061,(float)temp_output_4_0_g1061, temp_output_12_0_g1061, temp_output_13_0_g1061 );
				float localStochasticTiling190_g1062 = ( 0.0 );
				float2 Input_UV317_g1062 = temp_output_5_0_g1061;
				float2 UV190_g1062 = Input_UV317_g1062;
				float2 UV1190_g1062 = float2( 0,0 );
				float2 UV2190_g1062 = float2( 0,0 );
				float2 UV3190_g1062 = float2( 0,0 );
				float W1190_g1062 = 0.0;
				float W2190_g1062 = 0.0;
				float W3190_g1062 = 0.0;
				StochasticTiling( UV190_g1062 , UV1190_g1062 , UV2190_g1062 , UV3190_g1062 , W1190_g1062 , W2190_g1062 , W3190_g1062 );
				float Input_Index330_g1062 = (float)temp_output_4_0_g1061;
				float2 temp_output_358_0_g1062 = temp_output_12_0_g1061;
				float2 temp_output_359_0_g1062 = temp_output_13_0_g1061;
				float4 Output_2DArray152_g1062 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W1190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W2190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W3190_g1062 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1061 = Output_2DArray152_g1062;
				#else
				float4 staticSwitch7_g1061 = tex2DArrayNode3_g1061;
				#endif
				float4 ifLocalVar17_g1061 = 0;
				UNITY_BRANCH 
				if( break896.y > 0.0 )
				ifLocalVar17_g1061 = staticSwitch7_g1061;
				else if( break896.y == 0.0 )
				ifLocalVar17_g1061 = tex2DArrayNode3_g1061;
				float4 Color1797 = Out1795;
				float4 temp_output_617_0 = ( ifLocalVar17_g1061 * Color1797 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch508 = temp_output_617_0;
				#else
				float4 staticSwitch508 = temp_output_617_0;
				#endif
				float4 temp_output_22_0_g1066 = staticSwitch508;
				float4 _Vector2 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float3 break488 = UV298;
				float2 appendResult495 = (float2(break488.x , break488.y));
				float2 temp_output_5_0_g1063 = appendResult495;
				int temp_output_4_0_g1063 = (int)break488.z;
				float2 temp_output_9_0_g1063 = Mip101;
				float2 temp_output_12_0_g1063 = ddx( temp_output_9_0_g1063 );
				float2 temp_output_13_0_g1063 = ddy( temp_output_9_0_g1063 );
				float4 tex2DArrayNode3_g1063 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1063,(float)temp_output_4_0_g1063, temp_output_12_0_g1063, temp_output_13_0_g1063 );
				float localStochasticTiling190_g1064 = ( 0.0 );
				float2 Input_UV317_g1064 = temp_output_5_0_g1063;
				float2 UV190_g1064 = Input_UV317_g1064;
				float2 UV1190_g1064 = float2( 0,0 );
				float2 UV2190_g1064 = float2( 0,0 );
				float2 UV3190_g1064 = float2( 0,0 );
				float W1190_g1064 = 0.0;
				float W2190_g1064 = 0.0;
				float W3190_g1064 = 0.0;
				StochasticTiling( UV190_g1064 , UV1190_g1064 , UV2190_g1064 , UV3190_g1064 , W1190_g1064 , W2190_g1064 , W3190_g1064 );
				float Input_Index330_g1064 = (float)temp_output_4_0_g1063;
				float2 temp_output_358_0_g1064 = temp_output_12_0_g1063;
				float2 temp_output_359_0_g1064 = temp_output_13_0_g1063;
				float4 Output_2DArray152_g1064 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W1190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W2190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W3190_g1064 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1063 = Output_2DArray152_g1064;
				#else
				float4 staticSwitch7_g1063 = tex2DArrayNode3_g1063;
				#endif
				float4 ifLocalVar17_g1063 = 0;
				UNITY_BRANCH 
				if( break896.z > 0.0 )
				ifLocalVar17_g1063 = staticSwitch7_g1063;
				else if( break896.z == 0.0 )
				ifLocalVar17_g1063 = tex2DArrayNode3_g1063;
				float4 Color2798 = Out2795;
				float4 temp_output_618_0 = ( ifLocalVar17_g1063 * Color2798 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch509 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch509 = temp_output_618_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch509 = temp_output_618_0;
				#else
				float4 staticSwitch509 = _Vector2;
				#endif
				float4 temp_output_23_0_g1066 = staticSwitch509;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float3 break486 = UV399;
				float2 appendResult491 = (float2(break486.x , break486.y));
				float2 temp_output_5_0_g1059 = appendResult491;
				int temp_output_4_0_g1059 = (int)break486.z;
				float2 temp_output_9_0_g1059 = Mip101;
				float2 temp_output_12_0_g1059 = ddx( temp_output_9_0_g1059 );
				float2 temp_output_13_0_g1059 = ddy( temp_output_9_0_g1059 );
				float4 tex2DArrayNode3_g1059 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1059,(float)temp_output_4_0_g1059, temp_output_12_0_g1059, temp_output_13_0_g1059 );
				float localStochasticTiling190_g1060 = ( 0.0 );
				float2 Input_UV317_g1060 = temp_output_5_0_g1059;
				float2 UV190_g1060 = Input_UV317_g1060;
				float2 UV1190_g1060 = float2( 0,0 );
				float2 UV2190_g1060 = float2( 0,0 );
				float2 UV3190_g1060 = float2( 0,0 );
				float W1190_g1060 = 0.0;
				float W2190_g1060 = 0.0;
				float W3190_g1060 = 0.0;
				StochasticTiling( UV190_g1060 , UV1190_g1060 , UV2190_g1060 , UV3190_g1060 , W1190_g1060 , W2190_g1060 , W3190_g1060 );
				float Input_Index330_g1060 = (float)temp_output_4_0_g1059;
				float2 temp_output_358_0_g1060 = temp_output_12_0_g1059;
				float2 temp_output_359_0_g1060 = temp_output_13_0_g1059;
				float4 Output_2DArray152_g1060 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W1190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W2190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W3190_g1060 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1059 = Output_2DArray152_g1060;
				#else
				float4 staticSwitch7_g1059 = tex2DArrayNode3_g1059;
				#endif
				float4 ifLocalVar17_g1059 = 0;
				UNITY_BRANCH 
				if( break896.w > 0.0 )
				ifLocalVar17_g1059 = staticSwitch7_g1059;
				else if( break896.w == 0.0 )
				ifLocalVar17_g1059 = tex2DArrayNode3_g1059;
				float4 Color3799 = Out3795;
				#if defined( _QUALITY_FAST )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch510 = ( ifLocalVar17_g1059 * Color3799 );
				#else
				float4 staticSwitch510 = _Vector2;
				#endif
				float4 temp_output_24_0_g1066 = staticSwitch510;
				float4 weightedBlendVar30_g1066 = temp_output_14_0_g1066;
				float4 weightedBlend30_g1066 = ( weightedBlendVar30_g1066.x*temp_output_18_0_g1066 + weightedBlendVar30_g1066.y*temp_output_22_0_g1066 + weightedBlendVar30_g1066.z*temp_output_23_0_g1066 + weightedBlendVar30_g1066.w*temp_output_24_0_g1066 );
				float4 break899 = samplingType895;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float2 temp_output_9_0_g11 = Mip101;
				float2 temp_output_12_0_g11 = ddx( temp_output_9_0_g11 );
				float2 temp_output_13_0_g11 = ddy( temp_output_9_0_g11 );
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, temp_output_12_0_g11, temp_output_13_0_g11 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float2 temp_output_358_0_g12 = temp_output_12_0_g11;
				float2 temp_output_359_0_g12 = temp_output_13_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float2 temp_output_9_0_g9 = Mip101;
				float2 temp_output_12_0_g9 = ddx( temp_output_9_0_g9 );
				float2 temp_output_13_0_g9 = ddy( temp_output_9_0_g9 );
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, temp_output_12_0_g9, temp_output_13_0_g9 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float2 temp_output_358_0_g10 = temp_output_12_0_g9;
				float2 temp_output_359_0_g10 = temp_output_13_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float2 temp_output_9_0_g7 = Mip101;
				float2 temp_output_12_0_g7 = ddx( temp_output_9_0_g7 );
				float2 temp_output_13_0_g7 = ddy( temp_output_9_0_g7 );
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, temp_output_12_0_g7, temp_output_13_0_g7 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float2 temp_output_358_0_g8 = temp_output_12_0_g7;
				float2 temp_output_359_0_g8 = temp_output_13_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float2 temp_output_9_0_g1 = Mip101;
				float2 temp_output_12_0_g1 = ddx( temp_output_9_0_g1 );
				float2 temp_output_13_0_g1 = ddy( temp_output_9_0_g1 );
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, temp_output_12_0_g1, temp_output_13_0_g1 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float2 temp_output_358_0_g6 = temp_output_12_0_g1;
				float2 temp_output_359_0_g6 = temp_output_13_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1066 = HeightRawCombined0199;
				float4 break15_g1066 = temp_output_14_0_g1066;
				float temp_output_53_0_g1066 = ( break13_g1066.x + break15_g1066.x );
				float temp_output_54_0_g1066 = ( break13_g1066.y + break15_g1066.y );
				float temp_output_55_0_g1066 = ( break13_g1066.z + break15_g1066.z );
				float temp_output_56_0_g1066 = ( break13_g1066.w + break15_g1066.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1066 = ( max( max( max( temp_output_53_0_g1066 , temp_output_54_0_g1066 ) , temp_output_55_0_g1066 ) , temp_output_56_0_g1066 ) - HeightBlending854 );
				float temp_output_63_0_g1066 = max( ( temp_output_53_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_67_0_g1066 = max( ( temp_output_54_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_71_0_g1066 = max( ( temp_output_55_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_73_0_g1066 = max( ( temp_output_56_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1066 = lerp( weightedBlend30_g1066 , ( ( ( temp_output_18_0_g1066 * temp_output_63_0_g1066 ) + ( temp_output_22_0_g1066 * temp_output_67_0_g1066 ) + ( temp_output_23_0_g1066 * temp_output_71_0_g1066 ) + ( temp_output_24_0_g1066 * temp_output_73_0_g1066 ) ) / ( temp_output_63_0_g1066 + temp_output_67_0_g1066 + temp_output_71_0_g1066 + temp_output_73_0_g1066 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1066 = lerpResult78_g1066;
				#else
				float4 staticSwitch77_g1066 = weightedBlend30_g1066;
				#endif
				float4 Albedo0520 = staticSwitch77_g1066;
				float3 ase_worldNormal = input.ase_texcoord6.xyz;
				float4 appendResult179 = (float4(WorldPosition.x , WorldPosition.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Snow_Amount174 = _EnviroSnow;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float4 lerpResult524 = lerp( float4( 1,1,1,0 ) , _PuddleColor , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch543 = ( Albedo0520 * lerpResult524 );
				#else
				float4 staticSwitch543 = Albedo0520;
				#endif
				float2 temp_cast_63 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord4.xy * temp_cast_63 + float2( 0,0 );
				float2 temp_output_5_0_g1067 = texCoord232;
				float localStochasticTiling2_g1068 = ( 0.0 );
				float2 Input_UV145_g1068 = temp_output_5_0_g1067;
				float2 UV2_g1068 = Input_UV145_g1068;
				float2 UV12_g1068 = float2( 0,0 );
				float2 UV22_g1068 = float2( 0,0 );
				float2 UV32_g1068 = float2( 0,0 );
				float W12_g1068 = 0.0;
				float W22_g1068 = 0.0;
				float W32_g1068 = 0.0;
				StochasticTiling( UV2_g1068 , UV12_g1068 , UV22_g1068 , UV32_g1068 , W12_g1068 , W22_g1068 , W32_g1068 );
				float2 temp_output_10_0_g1068 = ddx( Input_UV145_g1068 );
				float2 temp_output_12_0_g1068 = ddy( Input_UV145_g1068 );
				float4 Output_2D293_g1068 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV12_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W12_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV22_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W22_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV32_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W32_g1068 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1067 = Output_2D293_g1068;
				#else
				float4 staticSwitch7_g1067 = SAMPLE_TEXTURE2D( _SnowAlbedo, sampler_SnowAlbedo, temp_output_5_0_g1067 );
				#endif
				float4 Snow_Albedo522 = staticSwitch7_g1067;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float4 temp_output_14_0_g1053 = SplatWeights198;
				float4 break898 = samplingType895;
				float2 temp_output_5_0_g1049 = UV0100.xy;
				float4 break391 = SplatIndex44;
				int temp_output_4_0_g1049 = (int)break391.x;
				float2 temp_output_9_0_g1049 = Mip101;
				float2 temp_output_12_0_g1049 = ddx( temp_output_9_0_g1049 );
				float2 temp_output_13_0_g1049 = ddy( temp_output_9_0_g1049 );
				float4 tex2DArrayNode3_g1049 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1049,(float)temp_output_4_0_g1049, temp_output_12_0_g1049, temp_output_13_0_g1049 );
				float localStochasticTiling190_g1050 = ( 0.0 );
				float2 Input_UV317_g1050 = temp_output_5_0_g1049;
				float2 UV190_g1050 = Input_UV317_g1050;
				float2 UV1190_g1050 = float2( 0,0 );
				float2 UV2190_g1050 = float2( 0,0 );
				float2 UV3190_g1050 = float2( 0,0 );
				float W1190_g1050 = 0.0;
				float W2190_g1050 = 0.0;
				float W3190_g1050 = 0.0;
				StochasticTiling( UV190_g1050 , UV1190_g1050 , UV2190_g1050 , UV3190_g1050 , W1190_g1050 , W2190_g1050 , W3190_g1050 );
				float Input_Index330_g1050 = (float)temp_output_4_0_g1049;
				float2 temp_output_358_0_g1050 = temp_output_12_0_g1049;
				float2 temp_output_359_0_g1050 = temp_output_13_0_g1049;
				float4 Output_2DArray152_g1050 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W1190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W2190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W3190_g1050 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1049 = Output_2DArray152_g1050;
				#else
				float4 staticSwitch7_g1049 = tex2DArrayNode3_g1049;
				#endif
				float4 ifLocalVar17_g1049 = 0;
				UNITY_BRANCH 
				if( break898.x > 0.0 )
				ifLocalVar17_g1049 = staticSwitch7_g1049;
				else if( break898.x == 0.0 )
				ifLocalVar17_g1049 = tex2DArrayNode3_g1049;
				float localGetLayerSettings368 = ( 0.0 );
				float4 in0368 = _NormalScale00;
				float4 in1368 = _NormalScale01;
				float4 in2368 = _NormalScale02;
				float4 index368 = SplatIndex44;
				float4 Out0368 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0368,in1368,in2368,index368,Out0368);
				}
				float4 NormalScales375 = Out0368;
				float4 break401 = NormalScales375;
				float3 unpack417 = UnpackNormalScale( ifLocalVar17_g1049, break401.x );
				unpack417.z = lerp( 1, unpack417.z, saturate(break401.x) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch433 = unpack417;
				#else
				float3 staticSwitch433 = unpack417;
				#endif
				float4 temp_output_18_0_g1053 = float4( staticSwitch433 , 0.0 );
				float2 temp_output_5_0_g1045 = UV197.xy;
				int temp_output_4_0_g1045 = (int)break391.y;
				float2 temp_output_9_0_g1045 = Mip101;
				float2 temp_output_12_0_g1045 = ddx( temp_output_9_0_g1045 );
				float2 temp_output_13_0_g1045 = ddy( temp_output_9_0_g1045 );
				float4 tex2DArrayNode3_g1045 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1045,(float)temp_output_4_0_g1045, temp_output_12_0_g1045, temp_output_13_0_g1045 );
				float localStochasticTiling190_g1046 = ( 0.0 );
				float2 Input_UV317_g1046 = temp_output_5_0_g1045;
				float2 UV190_g1046 = Input_UV317_g1046;
				float2 UV1190_g1046 = float2( 0,0 );
				float2 UV2190_g1046 = float2( 0,0 );
				float2 UV3190_g1046 = float2( 0,0 );
				float W1190_g1046 = 0.0;
				float W2190_g1046 = 0.0;
				float W3190_g1046 = 0.0;
				StochasticTiling( UV190_g1046 , UV1190_g1046 , UV2190_g1046 , UV3190_g1046 , W1190_g1046 , W2190_g1046 , W3190_g1046 );
				float Input_Index330_g1046 = (float)temp_output_4_0_g1045;
				float2 temp_output_358_0_g1046 = temp_output_12_0_g1045;
				float2 temp_output_359_0_g1046 = temp_output_13_0_g1045;
				float4 Output_2DArray152_g1046 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W1190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W2190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W3190_g1046 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1045 = Output_2DArray152_g1046;
				#else
				float4 staticSwitch7_g1045 = tex2DArrayNode3_g1045;
				#endif
				float4 ifLocalVar17_g1045 = 0;
				UNITY_BRANCH 
				if( break898.y > 0.0 )
				ifLocalVar17_g1045 = staticSwitch7_g1045;
				else if( break898.y == 0.0 )
				ifLocalVar17_g1045 = tex2DArrayNode3_g1045;
				float3 unpack416 = UnpackNormalScale( ifLocalVar17_g1045, break401.y );
				unpack416.z = lerp( 1, unpack416.z, saturate(break401.y) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch434 = unpack416;
				#else
				float3 staticSwitch434 = unpack416;
				#endif
				float4 temp_output_22_0_g1053 = float4( staticSwitch434 , 0.0 );
				float4 _Vector3 = float4(0,0,0,0);
				float2 temp_output_5_0_g1051 = UV298.xy;
				int temp_output_4_0_g1051 = (int)break391.z;
				float2 temp_output_9_0_g1051 = Mip101;
				float2 temp_output_12_0_g1051 = ddx( temp_output_9_0_g1051 );
				float2 temp_output_13_0_g1051 = ddy( temp_output_9_0_g1051 );
				float4 tex2DArrayNode3_g1051 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1051,(float)temp_output_4_0_g1051, temp_output_12_0_g1051, temp_output_13_0_g1051 );
				float localStochasticTiling190_g1052 = ( 0.0 );
				float2 Input_UV317_g1052 = temp_output_5_0_g1051;
				float2 UV190_g1052 = Input_UV317_g1052;
				float2 UV1190_g1052 = float2( 0,0 );
				float2 UV2190_g1052 = float2( 0,0 );
				float2 UV3190_g1052 = float2( 0,0 );
				float W1190_g1052 = 0.0;
				float W2190_g1052 = 0.0;
				float W3190_g1052 = 0.0;
				StochasticTiling( UV190_g1052 , UV1190_g1052 , UV2190_g1052 , UV3190_g1052 , W1190_g1052 , W2190_g1052 , W3190_g1052 );
				float Input_Index330_g1052 = (float)temp_output_4_0_g1051;
				float2 temp_output_358_0_g1052 = temp_output_12_0_g1051;
				float2 temp_output_359_0_g1052 = temp_output_13_0_g1051;
				float4 Output_2DArray152_g1052 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W1190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W2190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W3190_g1052 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1051 = Output_2DArray152_g1052;
				#else
				float4 staticSwitch7_g1051 = tex2DArrayNode3_g1051;
				#endif
				float4 ifLocalVar17_g1051 = 0;
				UNITY_BRANCH 
				if( break898.z > 0.0 )
				ifLocalVar17_g1051 = staticSwitch7_g1051;
				else if( break898.z == 0.0 )
				ifLocalVar17_g1051 = tex2DArrayNode3_g1051;
				float3 unpack414 = UnpackNormalScale( ifLocalVar17_g1051, break401.z );
				unpack414.z = lerp( 1, unpack414.z, saturate(break401.z) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch435 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#else
				float4 staticSwitch435 = _Vector3;
				#endif
				float4 temp_output_23_0_g1053 = staticSwitch435;
				float2 temp_output_5_0_g1047 = UV399.xy;
				int temp_output_4_0_g1047 = (int)break391.w;
				float2 temp_output_9_0_g1047 = Mip101;
				float2 temp_output_12_0_g1047 = ddx( temp_output_9_0_g1047 );
				float2 temp_output_13_0_g1047 = ddy( temp_output_9_0_g1047 );
				float4 tex2DArrayNode3_g1047 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1047,(float)temp_output_4_0_g1047, temp_output_12_0_g1047, temp_output_13_0_g1047 );
				float localStochasticTiling190_g1048 = ( 0.0 );
				float2 Input_UV317_g1048 = temp_output_5_0_g1047;
				float2 UV190_g1048 = Input_UV317_g1048;
				float2 UV1190_g1048 = float2( 0,0 );
				float2 UV2190_g1048 = float2( 0,0 );
				float2 UV3190_g1048 = float2( 0,0 );
				float W1190_g1048 = 0.0;
				float W2190_g1048 = 0.0;
				float W3190_g1048 = 0.0;
				StochasticTiling( UV190_g1048 , UV1190_g1048 , UV2190_g1048 , UV3190_g1048 , W1190_g1048 , W2190_g1048 , W3190_g1048 );
				float Input_Index330_g1048 = (float)temp_output_4_0_g1047;
				float2 temp_output_358_0_g1048 = temp_output_12_0_g1047;
				float2 temp_output_359_0_g1048 = temp_output_13_0_g1047;
				float4 Output_2DArray152_g1048 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W1190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W2190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W3190_g1048 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1047 = Output_2DArray152_g1048;
				#else
				float4 staticSwitch7_g1047 = tex2DArrayNode3_g1047;
				#endif
				float4 ifLocalVar17_g1047 = 0;
				UNITY_BRANCH 
				if( break898.w > 0.0 )
				ifLocalVar17_g1047 = staticSwitch7_g1047;
				else if( break898.w == 0.0 )
				ifLocalVar17_g1047 = tex2DArrayNode3_g1047;
				float3 unpack415 = UnpackNormalScale( ifLocalVar17_g1047, break401.w );
				unpack415.z = lerp( 1, unpack415.z, saturate(break401.w) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch436 = float4( unpack415 , 0.0 );
				#else
				float4 staticSwitch436 = _Vector3;
				#endif
				float4 temp_output_24_0_g1053 = staticSwitch436;
				float4 weightedBlendVar30_g1053 = temp_output_14_0_g1053;
				float4 weightedBlend30_g1053 = ( weightedBlendVar30_g1053.x*temp_output_18_0_g1053 + weightedBlendVar30_g1053.y*temp_output_22_0_g1053 + weightedBlendVar30_g1053.z*temp_output_23_0_g1053 + weightedBlendVar30_g1053.w*temp_output_24_0_g1053 );
				float4 break13_g1053 = HeightRawCombined0199;
				float4 break15_g1053 = temp_output_14_0_g1053;
				float temp_output_53_0_g1053 = ( break13_g1053.x + break15_g1053.x );
				float temp_output_54_0_g1053 = ( break13_g1053.y + break15_g1053.y );
				float temp_output_55_0_g1053 = ( break13_g1053.z + break15_g1053.z );
				float temp_output_56_0_g1053 = ( break13_g1053.w + break15_g1053.w );
				float temp_output_79_0_g1053 = ( max( max( max( temp_output_53_0_g1053 , temp_output_54_0_g1053 ) , temp_output_55_0_g1053 ) , temp_output_56_0_g1053 ) - HeightBlending854 );
				float temp_output_63_0_g1053 = max( ( temp_output_53_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_67_0_g1053 = max( ( temp_output_54_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_71_0_g1053 = max( ( temp_output_55_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_73_0_g1053 = max( ( temp_output_56_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float4 lerpResult78_g1053 = lerp( weightedBlend30_g1053 , ( ( ( temp_output_18_0_g1053 * temp_output_63_0_g1053 ) + ( temp_output_22_0_g1053 * temp_output_67_0_g1053 ) + ( temp_output_23_0_g1053 * temp_output_71_0_g1053 ) + ( temp_output_24_0_g1053 * temp_output_73_0_g1053 ) ) / ( temp_output_63_0_g1053 + temp_output_67_0_g1053 + temp_output_71_0_g1053 + temp_output_73_0_g1053 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1053 = lerpResult78_g1053;
				#else
				float4 staticSwitch77_g1053 = weightedBlend30_g1053;
				#endif
				float4 Normal0450 = staticSwitch77_g1053;
				float temp_output_395_0 = ( _TimeParameters.x * 0.05 );
				float2 appendResult379 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_397_0 = ( appendResult379 * _PuddleWaveTiling );
				float2 panner408 = ( temp_output_395_0 * float2( 1,0 ) + temp_output_397_0);
				float temp_output_406_0 = ( Puddle_Mask264 * ( _PuddleWaveIntensity * Wetness228 ) );
				float3 unpack420 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner408 ), temp_output_406_0 );
				unpack420.z = lerp( 1, unpack420.z, saturate(temp_output_406_0) );
				float2 panner407 = ( temp_output_395_0 * float2( 0,1 ) + temp_output_397_0);
				float3 unpack419 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner407 ), temp_output_406_0 );
				unpack419.z = lerp( 1, unpack419.z, saturate(temp_output_406_0) );
				float3 Puddle447 = BlendNormal( unpack420 , unpack419 );
				float4 lerpResult457 = lerp( Normal0450 , float4( Puddle447 , 0.0 ) , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch462 = lerpResult457;
				#else
				float4 staticSwitch462 = Normal0450;
				#endif
				float Rain_Intensity303 = _EnviroRainIntensity;
				float temp_output_325_0 = (1.0 + (( _RainFlowStrength * Rain_Intensity303 ) - 0.0) * (-1.0 - 1.0) / (1.0 - 0.0));
				float temp_output_306_0 = ( _TimeParameters.x * 0.05 );
				float4 transform287 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord5.xyz , 0.0 ));
				float2 appendResult298 = (float2(( transform287.z * 0.7 ) , ( transform287.y * 0.2 )));
				float2 panner313 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult298 * _RainFlowTiling ));
				float2 texCoord285 = input.ase_texcoord4.xy * float2( 10,10 ) + float2( 0,0 );
				float gradientNoise289 = UnityGradientNoise(texCoord285,_RainFlowDistortionScale);
				gradientNoise289 = gradientNoise289*0.5 + 0.5;
				float Distortion307 = ( gradientNoise289 * _RainFlowDistortionStrenght );
				float simpleNoise324 = SimpleNoise( ( panner313 + Distortion307 )*100.0 );
				simpleNoise324 = simpleNoise324*2 - 1;
				float smoothstepResult332 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise324);
				float temp_output_335_0 = ( ( ( ase_worldNormal.y - 0.95 ) * -1.0 ) * _RainFlowIntensity );
				float3 temp_cast_99 = (0.3).xxx;
				float3 break337 = ( abs( ase_worldNormal ) - temp_cast_99 );
				float lerpResult342 = lerp( 0.0 , ( smoothstepResult332 * temp_output_335_0 ) , break337.x);
				float4 transform286 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord5.xyz , 0.0 ));
				float2 appendResult299 = (float2(( transform286.x * 0.7 ) , ( transform286.y * 0.2 )));
				float2 panner312 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult299 * _RainFlowTiling ));
				float simpleNoise328 = SimpleNoise( ( panner312 + Distortion307 )*100.0 );
				simpleNoise328 = simpleNoise328*2 - 1;
				float smoothstepResult333 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise328);
				float lerpResult341 = lerp( 0.0 , ( smoothstepResult333 * temp_output_335_0 ) , break337.z);
				float Rain_Distance_Fade340 = ( 1.0 - sqrt( saturate( ( distance( WorldPosition , _WorldSpaceCameraPos ) / _RainDistanceFade ) ) ) );
				float temp_output_366_0 = saturate( ( ( lerpResult342 + lerpResult341 ) * Rain_Distance_Fade340 ) );
				float temp_output_373_0 = ddx( temp_output_366_0 );
				float temp_output_384_0 = ddy( temp_output_366_0 );
				float3 appendResult445 = (float3(temp_output_373_0 , temp_output_384_0 , sqrt( ( ( 1.0 - ( temp_output_373_0 * temp_output_373_0 ) ) - ( temp_output_384_0 * temp_output_384_0 ) ) )));
				float3 normalizeResult449 = normalize( appendResult445 );
				float3 RainFlow453 = normalizeResult449;
				float localRainRipples1_g1054 = ( 0.0 );
				float2 appendResult426 = (float2(WorldPosition.x , WorldPosition.z));
				float2 UV1_g1054 = ( appendResult426 * _RainDropTiling );
				float AngleOffset1_g1054 = 5.0;
				float lerpResult428 = lerp( 64.0 , 12.0 , Puddle_Mask264);
				float CellDensity1_g1054 = round( lerpResult428 );
				float Time1_g1054 = ( _TimeParameters.x * _RainDropSpeed );
				float temp_output_358_0 = ( _RainDropIntensity * 1.5 );
				float lerpResult365 = lerp( _RainDropIntensity , temp_output_358_0 , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float staticSwitch372 = lerpResult365;
				#else
				float staticSwitch372 = temp_output_358_0;
				#endif
				float switchResult422 = (((ase_vface>0)?(( ( ( ase_worldNormal.y - 0.7 ) * ( staticSwitch372 * Rain_Intensity303 ) ) * Rain_Distance_Fade340 )):(0.0)));
				float Strength1_g1054 = max( 0.0 , switchResult422 );
				float3 normal1_g1054 = float3( 0,0,0 );
				float Out1_g1054 = 0.0;
				float lerpResult440 = lerp( 5.0 , 8.0 , Puddle_Mask264);
				float pow1_g1054 = lerpResult440;
				float lerpResult439 = lerp( 1.0 , 0.0 , Puddle_Mask264);
				float sin1_g1054 = lerpResult439;
				{
				Rain(UV1_g1054,AngleOffset1_g1054,CellDensity1_g1054,Time1_g1054,Strength1_g1054,pow1_g1054,sin1_g1054,Out1_g1054,normal1_g1054);
				}
				float3 Rain_Drop452 = normal1_g1054;
				#ifdef _RAIN_ON
				float4 staticSwitch468 = float4( BlendNormal( staticSwitch462.xyz , BlendNormal( RainFlow453 , Rain_Drop452 ) ) , 0.0 );
				#else
				float4 staticSwitch468 = staticSwitch462;
				#endif
				float2 temp_output_5_0_g1055 = texCoord232;
				float localStochasticTiling2_g1056 = ( 0.0 );
				float2 Input_UV145_g1056 = temp_output_5_0_g1055;
				float2 UV2_g1056 = Input_UV145_g1056;
				float2 UV12_g1056 = float2( 0,0 );
				float2 UV22_g1056 = float2( 0,0 );
				float2 UV32_g1056 = float2( 0,0 );
				float W12_g1056 = 0.0;
				float W22_g1056 = 0.0;
				float W32_g1056 = 0.0;
				StochasticTiling( UV2_g1056 , UV12_g1056 , UV22_g1056 , UV32_g1056 , W12_g1056 , W22_g1056 , W32_g1056 );
				float2 temp_output_10_0_g1056 = ddx( Input_UV145_g1056 );
				float2 temp_output_12_0_g1056 = ddy( Input_UV145_g1056 );
				float4 Output_2D293_g1056 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV12_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W12_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV22_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W22_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV32_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W32_g1056 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1055 = Output_2D293_g1056;
				#else
				float4 staticSwitch7_g1055 = SAMPLE_TEXTURE2D( _SnowNormal, sampler_SnowNormal, temp_output_5_0_g1055 );
				#endif
				float3 unpack463 = UnpackNormalScale( staticSwitch7_g1055, _SnowNormalScale );
				unpack463.z = lerp( 1, unpack463.z, saturate(_SnowNormalScale) );
				float3 Snow_Normal465 = unpack463;
				float2 appendResult202 = (float2(WorldPosition.x , WorldPosition.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord5.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord5.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float4 lerpResult470 = lerp( staticSwitch468 , float4( Snow_Normal465 , 0.0 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch471 = lerpResult470;
				#else
				float4 staticSwitch471 = staticSwitch468;
				#endif
				float4 Normal_Final472 = staticSwitch471;
				float3 ase_worldTangent = input.ase_texcoord7.xyz;
				float3 ase_worldBitangent = input.ase_texcoord8.xyz;
				float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
				float3 tangentToWorldDir474 = mul( ase_tangentToWorldFast, Normal_Final472.xyz );
				float dotResult497 = dot( ase_worldViewDir , -( _MainLightPosition.xyz + ( tangentToWorldDir474 * _SSSDistortion ) ) );
				float dotResult504 = dot( dotResult497 , _SSSScale );
				float SSS523 = ( saturate( dotResult504 ) * _SSSIntensity );
				float4 lerpResult553 = lerp( staticSwitch543 , ( Snow_Albedo522 + SSS523 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch562 = lerpResult553;
				#else
				float4 staticSwitch562 = staticSwitch543;
				#endif
				float4 Albedo_Final575 = ( staticSwitch562 + ( Wetness228 * -0.02 ) );
				float4 localClipHoles583 = ( Albedo_Final575 );
				float2 uv_TerrainHolesTexture = input.ase_texcoord4.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Hole583 = holeClipValue579;
				{
				clip(Hole583 == 0.0f ? -1 : 1);
				}
				float4 AlbedoCombined586 = localClipHoles583;
				
				float Alpha1008 = holeClipValue579;
				

				float3 BaseColor = AlbedoCombined586.xyz;
				float3 Emission = 0;
				float Alpha = Alpha1008;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = BaseColor;
				metaInput.Emission = Emission;
				#ifdef EDITOR_VISUALIZATION
					metaInput.VizUV = input.VizUV.xy;
					metaInput.LightCoord = input.LightCoord;
				#endif

				return UnityMetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#pragma multi_compile_local _RAIN_ON
			#include "EnviroInclude.hlsl"


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D_ARRAY(_AlbedoArray);
			SAMPLER(sampler_AlbedoArray);
			TEXTURE2D(_SnowAlbedo);
			SAMPLER(sampler_SnowAlbedo);
			TEXTURE2D_ARRAY(_NormalArray);
			SAMPLER(sampler_NormalArray);
			TEXTURE2D(_WaveNormal);
			SAMPLER(sampler_WaveNormal);
			TEXTURE2D(_SnowNormal);
			SAMPLER(sampler_SnowNormal);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			
			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_TRANSFER_INSTANCE_ID( input, output );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				output.ase_texcoord4.xyz = ase_worldNormal;
				float3 ase_worldTangent = TransformObjectToWorldDir(input.ase_tangent.xyz);
				output.ase_texcoord5.xyz = ase_worldTangent;
				float ase_vertexTangentSign = input.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				output.ase_texcoord6.xyz = ase_worldBitangent;
				
				output.ase_texcoord2.xy = input.ase_texcoord.xy;
				output.ase_texcoord3 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord2.zw = 0;
				output.ase_texcoord4.w = 0;
				output.ase_texcoord5.w = 0;
				output.ase_texcoord6.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				output.ase_tangent = input.ase_tangent;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				output.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input , bool ase_vface : SV_IsFrontFace ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord2.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord2.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D( _Control1, sampler_Control1, uv_Control1 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord2.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D( _Control2, sampler_Control2, uv_Control2 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1066 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break896 = samplingType895;
				float2 appendResult69 = (float2(input.ase_texcoord3.xyz.x , input.ase_texcoord3.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float3 break485 = UV0100;
				float2 appendResult493 = (float2(break485.x , break485.y));
				float2 temp_output_5_0_g1057 = appendResult493;
				int temp_output_4_0_g1057 = (int)break485.z;
				float2 appendResult87 = (float2(input.ase_texcoord3.xyz.x , input.ase_texcoord3.xyz.z));
				float2 Mip101 = ( appendResult87 * ( 1.0 / max( 0.001 , _MipDistanceBlending ) ) );
				float2 temp_output_9_0_g1057 = Mip101;
				float2 temp_output_12_0_g1057 = ddx( temp_output_9_0_g1057 );
				float2 temp_output_13_0_g1057 = ddy( temp_output_9_0_g1057 );
				float4 tex2DArrayNode3_g1057 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1057,(float)temp_output_4_0_g1057, temp_output_12_0_g1057, temp_output_13_0_g1057 );
				float localStochasticTiling190_g1058 = ( 0.0 );
				float2 Input_UV317_g1058 = temp_output_5_0_g1057;
				float2 UV190_g1058 = Input_UV317_g1058;
				float2 UV1190_g1058 = float2( 0,0 );
				float2 UV2190_g1058 = float2( 0,0 );
				float2 UV3190_g1058 = float2( 0,0 );
				float W1190_g1058 = 0.0;
				float W2190_g1058 = 0.0;
				float W3190_g1058 = 0.0;
				StochasticTiling( UV190_g1058 , UV1190_g1058 , UV2190_g1058 , UV3190_g1058 , W1190_g1058 , W2190_g1058 , W3190_g1058 );
				float Input_Index330_g1058 = (float)temp_output_4_0_g1057;
				float2 temp_output_358_0_g1058 = temp_output_12_0_g1057;
				float2 temp_output_359_0_g1058 = temp_output_13_0_g1057;
				float4 Output_2DArray152_g1058 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W1190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W2190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W3190_g1058 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1057 = Output_2DArray152_g1058;
				#else
				float4 staticSwitch7_g1057 = tex2DArrayNode3_g1057;
				#endif
				float4 ifLocalVar17_g1057 = 0;
				UNITY_BRANCH 
				if( break896.x > 0.0 )
				ifLocalVar17_g1057 = staticSwitch7_g1057;
				else if( break896.x == 0.0 )
				ifLocalVar17_g1057 = tex2DArrayNode3_g1057;
				float localGetUVS795 = ( 0.0 );
				float4 in0795 = _ColorTint0;
				float4 in1795 = _ColorTint1;
				float4 in2795 = _ColorTint2;
				float4 in3795 = _ColorTint3;
				float4 in4795 = _ColorTint4;
				float4 in5795 = _ColorTint5;
				float4 in6795 = _ColorTint6;
				float4 in7795 = _ColorTint7;
				float4 in8795 = _ColorTint8;
				float4 in9795 = _ColorTint9;
				float4 in10795 = _ColorTint10;
				float4 in11795 = _ColorTint11;
				float4 index795 = SplatIndex44;
				float4 Out0795 = float4( 0,0,0,0 );
				float4 Out1795 = float4( 0,0,0,0 );
				float4 Out2795 = float4( 0,0,0,0 );
				float4 Out3795 = float4( 0,0,0,0 );
				{
				GetLayerUV(in0795,in1795,in2795,in3795,in4795,in5795,in6795,in7795,in8795,in9795,in10795,in11795,index795,Out0795,Out1795,Out2795,Out3795);
				}
				float4 Color0796 = Out0795;
				float4 temp_output_616_0 = ( ifLocalVar17_g1057 * Color0796 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch511 = temp_output_616_0;
				#else
				float4 staticSwitch511 = temp_output_616_0;
				#endif
				float4 temp_output_2_0_g1065 = staticSwitch511;
				float4 temp_output_18_0_g1066 = float4( (temp_output_2_0_g1065).rgb , 0.0 );
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float3 break487 = UV197;
				float2 appendResult492 = (float2(break487.x , break487.y));
				float2 temp_output_5_0_g1061 = appendResult492;
				int temp_output_4_0_g1061 = (int)break487.z;
				float2 temp_output_9_0_g1061 = Mip101;
				float2 temp_output_12_0_g1061 = ddx( temp_output_9_0_g1061 );
				float2 temp_output_13_0_g1061 = ddy( temp_output_9_0_g1061 );
				float4 tex2DArrayNode3_g1061 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1061,(float)temp_output_4_0_g1061, temp_output_12_0_g1061, temp_output_13_0_g1061 );
				float localStochasticTiling190_g1062 = ( 0.0 );
				float2 Input_UV317_g1062 = temp_output_5_0_g1061;
				float2 UV190_g1062 = Input_UV317_g1062;
				float2 UV1190_g1062 = float2( 0,0 );
				float2 UV2190_g1062 = float2( 0,0 );
				float2 UV3190_g1062 = float2( 0,0 );
				float W1190_g1062 = 0.0;
				float W2190_g1062 = 0.0;
				float W3190_g1062 = 0.0;
				StochasticTiling( UV190_g1062 , UV1190_g1062 , UV2190_g1062 , UV3190_g1062 , W1190_g1062 , W2190_g1062 , W3190_g1062 );
				float Input_Index330_g1062 = (float)temp_output_4_0_g1061;
				float2 temp_output_358_0_g1062 = temp_output_12_0_g1061;
				float2 temp_output_359_0_g1062 = temp_output_13_0_g1061;
				float4 Output_2DArray152_g1062 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W1190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W2190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W3190_g1062 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1061 = Output_2DArray152_g1062;
				#else
				float4 staticSwitch7_g1061 = tex2DArrayNode3_g1061;
				#endif
				float4 ifLocalVar17_g1061 = 0;
				UNITY_BRANCH 
				if( break896.y > 0.0 )
				ifLocalVar17_g1061 = staticSwitch7_g1061;
				else if( break896.y == 0.0 )
				ifLocalVar17_g1061 = tex2DArrayNode3_g1061;
				float4 Color1797 = Out1795;
				float4 temp_output_617_0 = ( ifLocalVar17_g1061 * Color1797 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch508 = temp_output_617_0;
				#else
				float4 staticSwitch508 = temp_output_617_0;
				#endif
				float4 temp_output_22_0_g1066 = staticSwitch508;
				float4 _Vector2 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float3 break488 = UV298;
				float2 appendResult495 = (float2(break488.x , break488.y));
				float2 temp_output_5_0_g1063 = appendResult495;
				int temp_output_4_0_g1063 = (int)break488.z;
				float2 temp_output_9_0_g1063 = Mip101;
				float2 temp_output_12_0_g1063 = ddx( temp_output_9_0_g1063 );
				float2 temp_output_13_0_g1063 = ddy( temp_output_9_0_g1063 );
				float4 tex2DArrayNode3_g1063 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1063,(float)temp_output_4_0_g1063, temp_output_12_0_g1063, temp_output_13_0_g1063 );
				float localStochasticTiling190_g1064 = ( 0.0 );
				float2 Input_UV317_g1064 = temp_output_5_0_g1063;
				float2 UV190_g1064 = Input_UV317_g1064;
				float2 UV1190_g1064 = float2( 0,0 );
				float2 UV2190_g1064 = float2( 0,0 );
				float2 UV3190_g1064 = float2( 0,0 );
				float W1190_g1064 = 0.0;
				float W2190_g1064 = 0.0;
				float W3190_g1064 = 0.0;
				StochasticTiling( UV190_g1064 , UV1190_g1064 , UV2190_g1064 , UV3190_g1064 , W1190_g1064 , W2190_g1064 , W3190_g1064 );
				float Input_Index330_g1064 = (float)temp_output_4_0_g1063;
				float2 temp_output_358_0_g1064 = temp_output_12_0_g1063;
				float2 temp_output_359_0_g1064 = temp_output_13_0_g1063;
				float4 Output_2DArray152_g1064 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W1190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W2190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W3190_g1064 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1063 = Output_2DArray152_g1064;
				#else
				float4 staticSwitch7_g1063 = tex2DArrayNode3_g1063;
				#endif
				float4 ifLocalVar17_g1063 = 0;
				UNITY_BRANCH 
				if( break896.z > 0.0 )
				ifLocalVar17_g1063 = staticSwitch7_g1063;
				else if( break896.z == 0.0 )
				ifLocalVar17_g1063 = tex2DArrayNode3_g1063;
				float4 Color2798 = Out2795;
				float4 temp_output_618_0 = ( ifLocalVar17_g1063 * Color2798 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch509 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch509 = temp_output_618_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch509 = temp_output_618_0;
				#else
				float4 staticSwitch509 = _Vector2;
				#endif
				float4 temp_output_23_0_g1066 = staticSwitch509;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float3 break486 = UV399;
				float2 appendResult491 = (float2(break486.x , break486.y));
				float2 temp_output_5_0_g1059 = appendResult491;
				int temp_output_4_0_g1059 = (int)break486.z;
				float2 temp_output_9_0_g1059 = Mip101;
				float2 temp_output_12_0_g1059 = ddx( temp_output_9_0_g1059 );
				float2 temp_output_13_0_g1059 = ddy( temp_output_9_0_g1059 );
				float4 tex2DArrayNode3_g1059 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1059,(float)temp_output_4_0_g1059, temp_output_12_0_g1059, temp_output_13_0_g1059 );
				float localStochasticTiling190_g1060 = ( 0.0 );
				float2 Input_UV317_g1060 = temp_output_5_0_g1059;
				float2 UV190_g1060 = Input_UV317_g1060;
				float2 UV1190_g1060 = float2( 0,0 );
				float2 UV2190_g1060 = float2( 0,0 );
				float2 UV3190_g1060 = float2( 0,0 );
				float W1190_g1060 = 0.0;
				float W2190_g1060 = 0.0;
				float W3190_g1060 = 0.0;
				StochasticTiling( UV190_g1060 , UV1190_g1060 , UV2190_g1060 , UV3190_g1060 , W1190_g1060 , W2190_g1060 , W3190_g1060 );
				float Input_Index330_g1060 = (float)temp_output_4_0_g1059;
				float2 temp_output_358_0_g1060 = temp_output_12_0_g1059;
				float2 temp_output_359_0_g1060 = temp_output_13_0_g1059;
				float4 Output_2DArray152_g1060 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W1190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W2190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W3190_g1060 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1059 = Output_2DArray152_g1060;
				#else
				float4 staticSwitch7_g1059 = tex2DArrayNode3_g1059;
				#endif
				float4 ifLocalVar17_g1059 = 0;
				UNITY_BRANCH 
				if( break896.w > 0.0 )
				ifLocalVar17_g1059 = staticSwitch7_g1059;
				else if( break896.w == 0.0 )
				ifLocalVar17_g1059 = tex2DArrayNode3_g1059;
				float4 Color3799 = Out3795;
				#if defined( _QUALITY_FAST )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch510 = ( ifLocalVar17_g1059 * Color3799 );
				#else
				float4 staticSwitch510 = _Vector2;
				#endif
				float4 temp_output_24_0_g1066 = staticSwitch510;
				float4 weightedBlendVar30_g1066 = temp_output_14_0_g1066;
				float4 weightedBlend30_g1066 = ( weightedBlendVar30_g1066.x*temp_output_18_0_g1066 + weightedBlendVar30_g1066.y*temp_output_22_0_g1066 + weightedBlendVar30_g1066.z*temp_output_23_0_g1066 + weightedBlendVar30_g1066.w*temp_output_24_0_g1066 );
				float4 break899 = samplingType895;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float2 temp_output_9_0_g11 = Mip101;
				float2 temp_output_12_0_g11 = ddx( temp_output_9_0_g11 );
				float2 temp_output_13_0_g11 = ddy( temp_output_9_0_g11 );
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, temp_output_12_0_g11, temp_output_13_0_g11 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float2 temp_output_358_0_g12 = temp_output_12_0_g11;
				float2 temp_output_359_0_g12 = temp_output_13_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float2 temp_output_9_0_g9 = Mip101;
				float2 temp_output_12_0_g9 = ddx( temp_output_9_0_g9 );
				float2 temp_output_13_0_g9 = ddy( temp_output_9_0_g9 );
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, temp_output_12_0_g9, temp_output_13_0_g9 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float2 temp_output_358_0_g10 = temp_output_12_0_g9;
				float2 temp_output_359_0_g10 = temp_output_13_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float2 temp_output_9_0_g7 = Mip101;
				float2 temp_output_12_0_g7 = ddx( temp_output_9_0_g7 );
				float2 temp_output_13_0_g7 = ddy( temp_output_9_0_g7 );
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, temp_output_12_0_g7, temp_output_13_0_g7 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float2 temp_output_358_0_g8 = temp_output_12_0_g7;
				float2 temp_output_359_0_g8 = temp_output_13_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float2 temp_output_9_0_g1 = Mip101;
				float2 temp_output_12_0_g1 = ddx( temp_output_9_0_g1 );
				float2 temp_output_13_0_g1 = ddy( temp_output_9_0_g1 );
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, temp_output_12_0_g1, temp_output_13_0_g1 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float2 temp_output_358_0_g6 = temp_output_12_0_g1;
				float2 temp_output_359_0_g6 = temp_output_13_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1066 = HeightRawCombined0199;
				float4 break15_g1066 = temp_output_14_0_g1066;
				float temp_output_53_0_g1066 = ( break13_g1066.x + break15_g1066.x );
				float temp_output_54_0_g1066 = ( break13_g1066.y + break15_g1066.y );
				float temp_output_55_0_g1066 = ( break13_g1066.z + break15_g1066.z );
				float temp_output_56_0_g1066 = ( break13_g1066.w + break15_g1066.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1066 = ( max( max( max( temp_output_53_0_g1066 , temp_output_54_0_g1066 ) , temp_output_55_0_g1066 ) , temp_output_56_0_g1066 ) - HeightBlending854 );
				float temp_output_63_0_g1066 = max( ( temp_output_53_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_67_0_g1066 = max( ( temp_output_54_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_71_0_g1066 = max( ( temp_output_55_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_73_0_g1066 = max( ( temp_output_56_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1066 = lerp( weightedBlend30_g1066 , ( ( ( temp_output_18_0_g1066 * temp_output_63_0_g1066 ) + ( temp_output_22_0_g1066 * temp_output_67_0_g1066 ) + ( temp_output_23_0_g1066 * temp_output_71_0_g1066 ) + ( temp_output_24_0_g1066 * temp_output_73_0_g1066 ) ) / ( temp_output_63_0_g1066 + temp_output_67_0_g1066 + temp_output_71_0_g1066 + temp_output_73_0_g1066 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1066 = lerpResult78_g1066;
				#else
				float4 staticSwitch77_g1066 = weightedBlend30_g1066;
				#endif
				float4 Albedo0520 = staticSwitch77_g1066;
				float3 ase_worldNormal = input.ase_texcoord4.xyz;
				float4 appendResult179 = (float4(WorldPosition.x , WorldPosition.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Snow_Amount174 = _EnviroSnow;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float4 lerpResult524 = lerp( float4( 1,1,1,0 ) , _PuddleColor , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch543 = ( Albedo0520 * lerpResult524 );
				#else
				float4 staticSwitch543 = Albedo0520;
				#endif
				float2 temp_cast_63 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord2.xy * temp_cast_63 + float2( 0,0 );
				float2 temp_output_5_0_g1067 = texCoord232;
				float localStochasticTiling2_g1068 = ( 0.0 );
				float2 Input_UV145_g1068 = temp_output_5_0_g1067;
				float2 UV2_g1068 = Input_UV145_g1068;
				float2 UV12_g1068 = float2( 0,0 );
				float2 UV22_g1068 = float2( 0,0 );
				float2 UV32_g1068 = float2( 0,0 );
				float W12_g1068 = 0.0;
				float W22_g1068 = 0.0;
				float W32_g1068 = 0.0;
				StochasticTiling( UV2_g1068 , UV12_g1068 , UV22_g1068 , UV32_g1068 , W12_g1068 , W22_g1068 , W32_g1068 );
				float2 temp_output_10_0_g1068 = ddx( Input_UV145_g1068 );
				float2 temp_output_12_0_g1068 = ddy( Input_UV145_g1068 );
				float4 Output_2D293_g1068 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV12_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W12_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV22_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W22_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV32_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W32_g1068 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1067 = Output_2D293_g1068;
				#else
				float4 staticSwitch7_g1067 = SAMPLE_TEXTURE2D( _SnowAlbedo, sampler_SnowAlbedo, temp_output_5_0_g1067 );
				#endif
				float4 Snow_Albedo522 = staticSwitch7_g1067;
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float4 temp_output_14_0_g1053 = SplatWeights198;
				float4 break898 = samplingType895;
				float2 temp_output_5_0_g1049 = UV0100.xy;
				float4 break391 = SplatIndex44;
				int temp_output_4_0_g1049 = (int)break391.x;
				float2 temp_output_9_0_g1049 = Mip101;
				float2 temp_output_12_0_g1049 = ddx( temp_output_9_0_g1049 );
				float2 temp_output_13_0_g1049 = ddy( temp_output_9_0_g1049 );
				float4 tex2DArrayNode3_g1049 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1049,(float)temp_output_4_0_g1049, temp_output_12_0_g1049, temp_output_13_0_g1049 );
				float localStochasticTiling190_g1050 = ( 0.0 );
				float2 Input_UV317_g1050 = temp_output_5_0_g1049;
				float2 UV190_g1050 = Input_UV317_g1050;
				float2 UV1190_g1050 = float2( 0,0 );
				float2 UV2190_g1050 = float2( 0,0 );
				float2 UV3190_g1050 = float2( 0,0 );
				float W1190_g1050 = 0.0;
				float W2190_g1050 = 0.0;
				float W3190_g1050 = 0.0;
				StochasticTiling( UV190_g1050 , UV1190_g1050 , UV2190_g1050 , UV3190_g1050 , W1190_g1050 , W2190_g1050 , W3190_g1050 );
				float Input_Index330_g1050 = (float)temp_output_4_0_g1049;
				float2 temp_output_358_0_g1050 = temp_output_12_0_g1049;
				float2 temp_output_359_0_g1050 = temp_output_13_0_g1049;
				float4 Output_2DArray152_g1050 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W1190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W2190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W3190_g1050 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1049 = Output_2DArray152_g1050;
				#else
				float4 staticSwitch7_g1049 = tex2DArrayNode3_g1049;
				#endif
				float4 ifLocalVar17_g1049 = 0;
				UNITY_BRANCH 
				if( break898.x > 0.0 )
				ifLocalVar17_g1049 = staticSwitch7_g1049;
				else if( break898.x == 0.0 )
				ifLocalVar17_g1049 = tex2DArrayNode3_g1049;
				float localGetLayerSettings368 = ( 0.0 );
				float4 in0368 = _NormalScale00;
				float4 in1368 = _NormalScale01;
				float4 in2368 = _NormalScale02;
				float4 index368 = SplatIndex44;
				float4 Out0368 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0368,in1368,in2368,index368,Out0368);
				}
				float4 NormalScales375 = Out0368;
				float4 break401 = NormalScales375;
				float3 unpack417 = UnpackNormalScale( ifLocalVar17_g1049, break401.x );
				unpack417.z = lerp( 1, unpack417.z, saturate(break401.x) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch433 = unpack417;
				#else
				float3 staticSwitch433 = unpack417;
				#endif
				float4 temp_output_18_0_g1053 = float4( staticSwitch433 , 0.0 );
				float2 temp_output_5_0_g1045 = UV197.xy;
				int temp_output_4_0_g1045 = (int)break391.y;
				float2 temp_output_9_0_g1045 = Mip101;
				float2 temp_output_12_0_g1045 = ddx( temp_output_9_0_g1045 );
				float2 temp_output_13_0_g1045 = ddy( temp_output_9_0_g1045 );
				float4 tex2DArrayNode3_g1045 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1045,(float)temp_output_4_0_g1045, temp_output_12_0_g1045, temp_output_13_0_g1045 );
				float localStochasticTiling190_g1046 = ( 0.0 );
				float2 Input_UV317_g1046 = temp_output_5_0_g1045;
				float2 UV190_g1046 = Input_UV317_g1046;
				float2 UV1190_g1046 = float2( 0,0 );
				float2 UV2190_g1046 = float2( 0,0 );
				float2 UV3190_g1046 = float2( 0,0 );
				float W1190_g1046 = 0.0;
				float W2190_g1046 = 0.0;
				float W3190_g1046 = 0.0;
				StochasticTiling( UV190_g1046 , UV1190_g1046 , UV2190_g1046 , UV3190_g1046 , W1190_g1046 , W2190_g1046 , W3190_g1046 );
				float Input_Index330_g1046 = (float)temp_output_4_0_g1045;
				float2 temp_output_358_0_g1046 = temp_output_12_0_g1045;
				float2 temp_output_359_0_g1046 = temp_output_13_0_g1045;
				float4 Output_2DArray152_g1046 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W1190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W2190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W3190_g1046 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1045 = Output_2DArray152_g1046;
				#else
				float4 staticSwitch7_g1045 = tex2DArrayNode3_g1045;
				#endif
				float4 ifLocalVar17_g1045 = 0;
				UNITY_BRANCH 
				if( break898.y > 0.0 )
				ifLocalVar17_g1045 = staticSwitch7_g1045;
				else if( break898.y == 0.0 )
				ifLocalVar17_g1045 = tex2DArrayNode3_g1045;
				float3 unpack416 = UnpackNormalScale( ifLocalVar17_g1045, break401.y );
				unpack416.z = lerp( 1, unpack416.z, saturate(break401.y) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch434 = unpack416;
				#else
				float3 staticSwitch434 = unpack416;
				#endif
				float4 temp_output_22_0_g1053 = float4( staticSwitch434 , 0.0 );
				float4 _Vector3 = float4(0,0,0,0);
				float2 temp_output_5_0_g1051 = UV298.xy;
				int temp_output_4_0_g1051 = (int)break391.z;
				float2 temp_output_9_0_g1051 = Mip101;
				float2 temp_output_12_0_g1051 = ddx( temp_output_9_0_g1051 );
				float2 temp_output_13_0_g1051 = ddy( temp_output_9_0_g1051 );
				float4 tex2DArrayNode3_g1051 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1051,(float)temp_output_4_0_g1051, temp_output_12_0_g1051, temp_output_13_0_g1051 );
				float localStochasticTiling190_g1052 = ( 0.0 );
				float2 Input_UV317_g1052 = temp_output_5_0_g1051;
				float2 UV190_g1052 = Input_UV317_g1052;
				float2 UV1190_g1052 = float2( 0,0 );
				float2 UV2190_g1052 = float2( 0,0 );
				float2 UV3190_g1052 = float2( 0,0 );
				float W1190_g1052 = 0.0;
				float W2190_g1052 = 0.0;
				float W3190_g1052 = 0.0;
				StochasticTiling( UV190_g1052 , UV1190_g1052 , UV2190_g1052 , UV3190_g1052 , W1190_g1052 , W2190_g1052 , W3190_g1052 );
				float Input_Index330_g1052 = (float)temp_output_4_0_g1051;
				float2 temp_output_358_0_g1052 = temp_output_12_0_g1051;
				float2 temp_output_359_0_g1052 = temp_output_13_0_g1051;
				float4 Output_2DArray152_g1052 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W1190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W2190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W3190_g1052 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1051 = Output_2DArray152_g1052;
				#else
				float4 staticSwitch7_g1051 = tex2DArrayNode3_g1051;
				#endif
				float4 ifLocalVar17_g1051 = 0;
				UNITY_BRANCH 
				if( break898.z > 0.0 )
				ifLocalVar17_g1051 = staticSwitch7_g1051;
				else if( break898.z == 0.0 )
				ifLocalVar17_g1051 = tex2DArrayNode3_g1051;
				float3 unpack414 = UnpackNormalScale( ifLocalVar17_g1051, break401.z );
				unpack414.z = lerp( 1, unpack414.z, saturate(break401.z) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch435 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#else
				float4 staticSwitch435 = _Vector3;
				#endif
				float4 temp_output_23_0_g1053 = staticSwitch435;
				float2 temp_output_5_0_g1047 = UV399.xy;
				int temp_output_4_0_g1047 = (int)break391.w;
				float2 temp_output_9_0_g1047 = Mip101;
				float2 temp_output_12_0_g1047 = ddx( temp_output_9_0_g1047 );
				float2 temp_output_13_0_g1047 = ddy( temp_output_9_0_g1047 );
				float4 tex2DArrayNode3_g1047 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1047,(float)temp_output_4_0_g1047, temp_output_12_0_g1047, temp_output_13_0_g1047 );
				float localStochasticTiling190_g1048 = ( 0.0 );
				float2 Input_UV317_g1048 = temp_output_5_0_g1047;
				float2 UV190_g1048 = Input_UV317_g1048;
				float2 UV1190_g1048 = float2( 0,0 );
				float2 UV2190_g1048 = float2( 0,0 );
				float2 UV3190_g1048 = float2( 0,0 );
				float W1190_g1048 = 0.0;
				float W2190_g1048 = 0.0;
				float W3190_g1048 = 0.0;
				StochasticTiling( UV190_g1048 , UV1190_g1048 , UV2190_g1048 , UV3190_g1048 , W1190_g1048 , W2190_g1048 , W3190_g1048 );
				float Input_Index330_g1048 = (float)temp_output_4_0_g1047;
				float2 temp_output_358_0_g1048 = temp_output_12_0_g1047;
				float2 temp_output_359_0_g1048 = temp_output_13_0_g1047;
				float4 Output_2DArray152_g1048 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W1190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W2190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W3190_g1048 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1047 = Output_2DArray152_g1048;
				#else
				float4 staticSwitch7_g1047 = tex2DArrayNode3_g1047;
				#endif
				float4 ifLocalVar17_g1047 = 0;
				UNITY_BRANCH 
				if( break898.w > 0.0 )
				ifLocalVar17_g1047 = staticSwitch7_g1047;
				else if( break898.w == 0.0 )
				ifLocalVar17_g1047 = tex2DArrayNode3_g1047;
				float3 unpack415 = UnpackNormalScale( ifLocalVar17_g1047, break401.w );
				unpack415.z = lerp( 1, unpack415.z, saturate(break401.w) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch436 = float4( unpack415 , 0.0 );
				#else
				float4 staticSwitch436 = _Vector3;
				#endif
				float4 temp_output_24_0_g1053 = staticSwitch436;
				float4 weightedBlendVar30_g1053 = temp_output_14_0_g1053;
				float4 weightedBlend30_g1053 = ( weightedBlendVar30_g1053.x*temp_output_18_0_g1053 + weightedBlendVar30_g1053.y*temp_output_22_0_g1053 + weightedBlendVar30_g1053.z*temp_output_23_0_g1053 + weightedBlendVar30_g1053.w*temp_output_24_0_g1053 );
				float4 break13_g1053 = HeightRawCombined0199;
				float4 break15_g1053 = temp_output_14_0_g1053;
				float temp_output_53_0_g1053 = ( break13_g1053.x + break15_g1053.x );
				float temp_output_54_0_g1053 = ( break13_g1053.y + break15_g1053.y );
				float temp_output_55_0_g1053 = ( break13_g1053.z + break15_g1053.z );
				float temp_output_56_0_g1053 = ( break13_g1053.w + break15_g1053.w );
				float temp_output_79_0_g1053 = ( max( max( max( temp_output_53_0_g1053 , temp_output_54_0_g1053 ) , temp_output_55_0_g1053 ) , temp_output_56_0_g1053 ) - HeightBlending854 );
				float temp_output_63_0_g1053 = max( ( temp_output_53_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_67_0_g1053 = max( ( temp_output_54_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_71_0_g1053 = max( ( temp_output_55_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_73_0_g1053 = max( ( temp_output_56_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float4 lerpResult78_g1053 = lerp( weightedBlend30_g1053 , ( ( ( temp_output_18_0_g1053 * temp_output_63_0_g1053 ) + ( temp_output_22_0_g1053 * temp_output_67_0_g1053 ) + ( temp_output_23_0_g1053 * temp_output_71_0_g1053 ) + ( temp_output_24_0_g1053 * temp_output_73_0_g1053 ) ) / ( temp_output_63_0_g1053 + temp_output_67_0_g1053 + temp_output_71_0_g1053 + temp_output_73_0_g1053 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1053 = lerpResult78_g1053;
				#else
				float4 staticSwitch77_g1053 = weightedBlend30_g1053;
				#endif
				float4 Normal0450 = staticSwitch77_g1053;
				float temp_output_395_0 = ( _TimeParameters.x * 0.05 );
				float2 appendResult379 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_397_0 = ( appendResult379 * _PuddleWaveTiling );
				float2 panner408 = ( temp_output_395_0 * float2( 1,0 ) + temp_output_397_0);
				float temp_output_406_0 = ( Puddle_Mask264 * ( _PuddleWaveIntensity * Wetness228 ) );
				float3 unpack420 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner408 ), temp_output_406_0 );
				unpack420.z = lerp( 1, unpack420.z, saturate(temp_output_406_0) );
				float2 panner407 = ( temp_output_395_0 * float2( 0,1 ) + temp_output_397_0);
				float3 unpack419 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner407 ), temp_output_406_0 );
				unpack419.z = lerp( 1, unpack419.z, saturate(temp_output_406_0) );
				float3 Puddle447 = BlendNormal( unpack420 , unpack419 );
				float4 lerpResult457 = lerp( Normal0450 , float4( Puddle447 , 0.0 ) , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch462 = lerpResult457;
				#else
				float4 staticSwitch462 = Normal0450;
				#endif
				float Rain_Intensity303 = _EnviroRainIntensity;
				float temp_output_325_0 = (1.0 + (( _RainFlowStrength * Rain_Intensity303 ) - 0.0) * (-1.0 - 1.0) / (1.0 - 0.0));
				float temp_output_306_0 = ( _TimeParameters.x * 0.05 );
				float4 transform287 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord3.xyz , 0.0 ));
				float2 appendResult298 = (float2(( transform287.z * 0.7 ) , ( transform287.y * 0.2 )));
				float2 panner313 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult298 * _RainFlowTiling ));
				float2 texCoord285 = input.ase_texcoord2.xy * float2( 10,10 ) + float2( 0,0 );
				float gradientNoise289 = UnityGradientNoise(texCoord285,_RainFlowDistortionScale);
				gradientNoise289 = gradientNoise289*0.5 + 0.5;
				float Distortion307 = ( gradientNoise289 * _RainFlowDistortionStrenght );
				float simpleNoise324 = SimpleNoise( ( panner313 + Distortion307 )*100.0 );
				simpleNoise324 = simpleNoise324*2 - 1;
				float smoothstepResult332 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise324);
				float temp_output_335_0 = ( ( ( ase_worldNormal.y - 0.95 ) * -1.0 ) * _RainFlowIntensity );
				float3 temp_cast_99 = (0.3).xxx;
				float3 break337 = ( abs( ase_worldNormal ) - temp_cast_99 );
				float lerpResult342 = lerp( 0.0 , ( smoothstepResult332 * temp_output_335_0 ) , break337.x);
				float4 transform286 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord3.xyz , 0.0 ));
				float2 appendResult299 = (float2(( transform286.x * 0.7 ) , ( transform286.y * 0.2 )));
				float2 panner312 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult299 * _RainFlowTiling ));
				float simpleNoise328 = SimpleNoise( ( panner312 + Distortion307 )*100.0 );
				simpleNoise328 = simpleNoise328*2 - 1;
				float smoothstepResult333 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise328);
				float lerpResult341 = lerp( 0.0 , ( smoothstepResult333 * temp_output_335_0 ) , break337.z);
				float Rain_Distance_Fade340 = ( 1.0 - sqrt( saturate( ( distance( WorldPosition , _WorldSpaceCameraPos ) / _RainDistanceFade ) ) ) );
				float temp_output_366_0 = saturate( ( ( lerpResult342 + lerpResult341 ) * Rain_Distance_Fade340 ) );
				float temp_output_373_0 = ddx( temp_output_366_0 );
				float temp_output_384_0 = ddy( temp_output_366_0 );
				float3 appendResult445 = (float3(temp_output_373_0 , temp_output_384_0 , sqrt( ( ( 1.0 - ( temp_output_373_0 * temp_output_373_0 ) ) - ( temp_output_384_0 * temp_output_384_0 ) ) )));
				float3 normalizeResult449 = normalize( appendResult445 );
				float3 RainFlow453 = normalizeResult449;
				float localRainRipples1_g1054 = ( 0.0 );
				float2 appendResult426 = (float2(WorldPosition.x , WorldPosition.z));
				float2 UV1_g1054 = ( appendResult426 * _RainDropTiling );
				float AngleOffset1_g1054 = 5.0;
				float lerpResult428 = lerp( 64.0 , 12.0 , Puddle_Mask264);
				float CellDensity1_g1054 = round( lerpResult428 );
				float Time1_g1054 = ( _TimeParameters.x * _RainDropSpeed );
				float temp_output_358_0 = ( _RainDropIntensity * 1.5 );
				float lerpResult365 = lerp( _RainDropIntensity , temp_output_358_0 , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float staticSwitch372 = lerpResult365;
				#else
				float staticSwitch372 = temp_output_358_0;
				#endif
				float switchResult422 = (((ase_vface>0)?(( ( ( ase_worldNormal.y - 0.7 ) * ( staticSwitch372 * Rain_Intensity303 ) ) * Rain_Distance_Fade340 )):(0.0)));
				float Strength1_g1054 = max( 0.0 , switchResult422 );
				float3 normal1_g1054 = float3( 0,0,0 );
				float Out1_g1054 = 0.0;
				float lerpResult440 = lerp( 5.0 , 8.0 , Puddle_Mask264);
				float pow1_g1054 = lerpResult440;
				float lerpResult439 = lerp( 1.0 , 0.0 , Puddle_Mask264);
				float sin1_g1054 = lerpResult439;
				{
				Rain(UV1_g1054,AngleOffset1_g1054,CellDensity1_g1054,Time1_g1054,Strength1_g1054,pow1_g1054,sin1_g1054,Out1_g1054,normal1_g1054);
				}
				float3 Rain_Drop452 = normal1_g1054;
				#ifdef _RAIN_ON
				float4 staticSwitch468 = float4( BlendNormal( staticSwitch462.xyz , BlendNormal( RainFlow453 , Rain_Drop452 ) ) , 0.0 );
				#else
				float4 staticSwitch468 = staticSwitch462;
				#endif
				float2 temp_output_5_0_g1055 = texCoord232;
				float localStochasticTiling2_g1056 = ( 0.0 );
				float2 Input_UV145_g1056 = temp_output_5_0_g1055;
				float2 UV2_g1056 = Input_UV145_g1056;
				float2 UV12_g1056 = float2( 0,0 );
				float2 UV22_g1056 = float2( 0,0 );
				float2 UV32_g1056 = float2( 0,0 );
				float W12_g1056 = 0.0;
				float W22_g1056 = 0.0;
				float W32_g1056 = 0.0;
				StochasticTiling( UV2_g1056 , UV12_g1056 , UV22_g1056 , UV32_g1056 , W12_g1056 , W22_g1056 , W32_g1056 );
				float2 temp_output_10_0_g1056 = ddx( Input_UV145_g1056 );
				float2 temp_output_12_0_g1056 = ddy( Input_UV145_g1056 );
				float4 Output_2D293_g1056 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV12_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W12_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV22_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W22_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV32_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W32_g1056 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1055 = Output_2D293_g1056;
				#else
				float4 staticSwitch7_g1055 = SAMPLE_TEXTURE2D( _SnowNormal, sampler_SnowNormal, temp_output_5_0_g1055 );
				#endif
				float3 unpack463 = UnpackNormalScale( staticSwitch7_g1055, _SnowNormalScale );
				unpack463.z = lerp( 1, unpack463.z, saturate(_SnowNormalScale) );
				float3 Snow_Normal465 = unpack463;
				float2 appendResult202 = (float2(WorldPosition.x , WorldPosition.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord3.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord3.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float4 lerpResult470 = lerp( staticSwitch468 , float4( Snow_Normal465 , 0.0 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch471 = lerpResult470;
				#else
				float4 staticSwitch471 = staticSwitch468;
				#endif
				float4 Normal_Final472 = staticSwitch471;
				float3 ase_worldTangent = input.ase_texcoord5.xyz;
				float3 ase_worldBitangent = input.ase_texcoord6.xyz;
				float3x3 ase_tangentToWorldFast = float3x3(ase_worldTangent.x,ase_worldBitangent.x,ase_worldNormal.x,ase_worldTangent.y,ase_worldBitangent.y,ase_worldNormal.y,ase_worldTangent.z,ase_worldBitangent.z,ase_worldNormal.z);
				float3 tangentToWorldDir474 = mul( ase_tangentToWorldFast, Normal_Final472.xyz );
				float dotResult497 = dot( ase_worldViewDir , -( _MainLightPosition.xyz + ( tangentToWorldDir474 * _SSSDistortion ) ) );
				float dotResult504 = dot( dotResult497 , _SSSScale );
				float SSS523 = ( saturate( dotResult504 ) * _SSSIntensity );
				float4 lerpResult553 = lerp( staticSwitch543 , ( Snow_Albedo522 + SSS523 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch562 = lerpResult553;
				#else
				float4 staticSwitch562 = staticSwitch543;
				#endif
				float4 Albedo_Final575 = ( staticSwitch562 + ( Wetness228 * -0.02 ) );
				float4 localClipHoles583 = ( Albedo_Final575 );
				float2 uv_TerrainHolesTexture = input.ase_texcoord2.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Hole583 = holeClipValue579;
				{
				clip(Hole583 == 0.0f ? -1 : 1);
				}
				float4 AlbedoCombined586 = localClipHoles583;
				
				float Alpha1008 = holeClipValue579;
				

				float3 BaseColor = AlbedoCombined586.xyz;
				float Alpha = Alpha1008;
				float AlphaClipThreshold = 0.5;

				half4 color = half4(BaseColor, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
			//#define SHADERPASS SHADERPASS_DEPTHNORMALS

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#pragma multi_compile_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
			#pragma multi_compile_local _RAIN_ON
			#include "EnviroInclude.hlsl"


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float4 worldTangent : TEXCOORD2;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD3;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD4;
				#endif
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D_ARRAY(_NormalArray);
			SAMPLER(sampler_NormalArray);
			TEXTURE2D(_WaveNormal);
			SAMPLER(sampler_WaveNormal);
			TEXTURE2D(_SnowNormal);
			SAMPLER(sampler_SnowNormal);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			
			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				float4 appendResult660 = (float4(cross( input.normalOS , float3(0,0,1) ) , -1.0));
				
				output.ase_texcoord5.xy = input.ase_texcoord.xy;
				output.ase_texcoord6 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord5.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = appendResult660;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				float3 normalWS = TransformObjectToWorldNormal( input.normalOS );
				float4 tangentWS = float4( TransformObjectToWorldDir( input.tangentOS.xyz ), input.tangentOS.w );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					output.positionWS = vertexInput.positionWS;
				#endif

				output.worldNormal = normalWS;
				output.worldTangent = tangentWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag(	PackedVaryings input
						, out half4 outNormalWS : SV_Target0
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						, bool ase_vface : SV_IsFrontFace )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = input.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float3 WorldNormal = input.worldNormal;
				float4 WorldTangent = input.worldTangent;

				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = input.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord5.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord5.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D( _Control1, sampler_Control1, uv_Control1 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord5.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D( _Control2, sampler_Control2, uv_Control2 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1053 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break898 = samplingType895;
				float2 appendResult69 = (float2(input.ase_texcoord6.xyz.x , input.ase_texcoord6.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g1049 = UV0100.xy;
				float4 break391 = SplatIndex44;
				int temp_output_4_0_g1049 = (int)break391.x;
				float2 appendResult87 = (float2(input.ase_texcoord6.xyz.x , input.ase_texcoord6.xyz.z));
				float2 Mip101 = ( appendResult87 * ( 1.0 / max( 0.001 , _MipDistanceBlending ) ) );
				float2 temp_output_9_0_g1049 = Mip101;
				float2 temp_output_12_0_g1049 = ddx( temp_output_9_0_g1049 );
				float2 temp_output_13_0_g1049 = ddy( temp_output_9_0_g1049 );
				float4 tex2DArrayNode3_g1049 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1049,(float)temp_output_4_0_g1049, temp_output_12_0_g1049, temp_output_13_0_g1049 );
				float localStochasticTiling190_g1050 = ( 0.0 );
				float2 Input_UV317_g1050 = temp_output_5_0_g1049;
				float2 UV190_g1050 = Input_UV317_g1050;
				float2 UV1190_g1050 = float2( 0,0 );
				float2 UV2190_g1050 = float2( 0,0 );
				float2 UV3190_g1050 = float2( 0,0 );
				float W1190_g1050 = 0.0;
				float W2190_g1050 = 0.0;
				float W3190_g1050 = 0.0;
				StochasticTiling( UV190_g1050 , UV1190_g1050 , UV2190_g1050 , UV3190_g1050 , W1190_g1050 , W2190_g1050 , W3190_g1050 );
				float Input_Index330_g1050 = (float)temp_output_4_0_g1049;
				float2 temp_output_358_0_g1050 = temp_output_12_0_g1049;
				float2 temp_output_359_0_g1050 = temp_output_13_0_g1049;
				float4 Output_2DArray152_g1050 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W1190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W2190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W3190_g1050 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1049 = Output_2DArray152_g1050;
				#else
				float4 staticSwitch7_g1049 = tex2DArrayNode3_g1049;
				#endif
				float4 ifLocalVar17_g1049 = 0;
				UNITY_BRANCH 
				if( break898.x > 0.0 )
				ifLocalVar17_g1049 = staticSwitch7_g1049;
				else if( break898.x == 0.0 )
				ifLocalVar17_g1049 = tex2DArrayNode3_g1049;
				float localGetLayerSettings368 = ( 0.0 );
				float4 in0368 = _NormalScale00;
				float4 in1368 = _NormalScale01;
				float4 in2368 = _NormalScale02;
				float4 index368 = SplatIndex44;
				float4 Out0368 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0368,in1368,in2368,index368,Out0368);
				}
				float4 NormalScales375 = Out0368;
				float4 break401 = NormalScales375;
				float3 unpack417 = UnpackNormalScale( ifLocalVar17_g1049, break401.x );
				unpack417.z = lerp( 1, unpack417.z, saturate(break401.x) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch433 = unpack417;
				#else
				float3 staticSwitch433 = unpack417;
				#endif
				float4 temp_output_18_0_g1053 = float4( staticSwitch433 , 0.0 );
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g1045 = UV197.xy;
				int temp_output_4_0_g1045 = (int)break391.y;
				float2 temp_output_9_0_g1045 = Mip101;
				float2 temp_output_12_0_g1045 = ddx( temp_output_9_0_g1045 );
				float2 temp_output_13_0_g1045 = ddy( temp_output_9_0_g1045 );
				float4 tex2DArrayNode3_g1045 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1045,(float)temp_output_4_0_g1045, temp_output_12_0_g1045, temp_output_13_0_g1045 );
				float localStochasticTiling190_g1046 = ( 0.0 );
				float2 Input_UV317_g1046 = temp_output_5_0_g1045;
				float2 UV190_g1046 = Input_UV317_g1046;
				float2 UV1190_g1046 = float2( 0,0 );
				float2 UV2190_g1046 = float2( 0,0 );
				float2 UV3190_g1046 = float2( 0,0 );
				float W1190_g1046 = 0.0;
				float W2190_g1046 = 0.0;
				float W3190_g1046 = 0.0;
				StochasticTiling( UV190_g1046 , UV1190_g1046 , UV2190_g1046 , UV3190_g1046 , W1190_g1046 , W2190_g1046 , W3190_g1046 );
				float Input_Index330_g1046 = (float)temp_output_4_0_g1045;
				float2 temp_output_358_0_g1046 = temp_output_12_0_g1045;
				float2 temp_output_359_0_g1046 = temp_output_13_0_g1045;
				float4 Output_2DArray152_g1046 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W1190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W2190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W3190_g1046 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1045 = Output_2DArray152_g1046;
				#else
				float4 staticSwitch7_g1045 = tex2DArrayNode3_g1045;
				#endif
				float4 ifLocalVar17_g1045 = 0;
				UNITY_BRANCH 
				if( break898.y > 0.0 )
				ifLocalVar17_g1045 = staticSwitch7_g1045;
				else if( break898.y == 0.0 )
				ifLocalVar17_g1045 = tex2DArrayNode3_g1045;
				float3 unpack416 = UnpackNormalScale( ifLocalVar17_g1045, break401.y );
				unpack416.z = lerp( 1, unpack416.z, saturate(break401.y) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch434 = unpack416;
				#else
				float3 staticSwitch434 = unpack416;
				#endif
				float4 temp_output_22_0_g1053 = float4( staticSwitch434 , 0.0 );
				float4 _Vector3 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g1051 = UV298.xy;
				int temp_output_4_0_g1051 = (int)break391.z;
				float2 temp_output_9_0_g1051 = Mip101;
				float2 temp_output_12_0_g1051 = ddx( temp_output_9_0_g1051 );
				float2 temp_output_13_0_g1051 = ddy( temp_output_9_0_g1051 );
				float4 tex2DArrayNode3_g1051 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1051,(float)temp_output_4_0_g1051, temp_output_12_0_g1051, temp_output_13_0_g1051 );
				float localStochasticTiling190_g1052 = ( 0.0 );
				float2 Input_UV317_g1052 = temp_output_5_0_g1051;
				float2 UV190_g1052 = Input_UV317_g1052;
				float2 UV1190_g1052 = float2( 0,0 );
				float2 UV2190_g1052 = float2( 0,0 );
				float2 UV3190_g1052 = float2( 0,0 );
				float W1190_g1052 = 0.0;
				float W2190_g1052 = 0.0;
				float W3190_g1052 = 0.0;
				StochasticTiling( UV190_g1052 , UV1190_g1052 , UV2190_g1052 , UV3190_g1052 , W1190_g1052 , W2190_g1052 , W3190_g1052 );
				float Input_Index330_g1052 = (float)temp_output_4_0_g1051;
				float2 temp_output_358_0_g1052 = temp_output_12_0_g1051;
				float2 temp_output_359_0_g1052 = temp_output_13_0_g1051;
				float4 Output_2DArray152_g1052 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W1190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W2190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W3190_g1052 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1051 = Output_2DArray152_g1052;
				#else
				float4 staticSwitch7_g1051 = tex2DArrayNode3_g1051;
				#endif
				float4 ifLocalVar17_g1051 = 0;
				UNITY_BRANCH 
				if( break898.z > 0.0 )
				ifLocalVar17_g1051 = staticSwitch7_g1051;
				else if( break898.z == 0.0 )
				ifLocalVar17_g1051 = tex2DArrayNode3_g1051;
				float3 unpack414 = UnpackNormalScale( ifLocalVar17_g1051, break401.z );
				unpack414.z = lerp( 1, unpack414.z, saturate(break401.z) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch435 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#else
				float4 staticSwitch435 = _Vector3;
				#endif
				float4 temp_output_23_0_g1053 = staticSwitch435;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1047 = UV399.xy;
				int temp_output_4_0_g1047 = (int)break391.w;
				float2 temp_output_9_0_g1047 = Mip101;
				float2 temp_output_12_0_g1047 = ddx( temp_output_9_0_g1047 );
				float2 temp_output_13_0_g1047 = ddy( temp_output_9_0_g1047 );
				float4 tex2DArrayNode3_g1047 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1047,(float)temp_output_4_0_g1047, temp_output_12_0_g1047, temp_output_13_0_g1047 );
				float localStochasticTiling190_g1048 = ( 0.0 );
				float2 Input_UV317_g1048 = temp_output_5_0_g1047;
				float2 UV190_g1048 = Input_UV317_g1048;
				float2 UV1190_g1048 = float2( 0,0 );
				float2 UV2190_g1048 = float2( 0,0 );
				float2 UV3190_g1048 = float2( 0,0 );
				float W1190_g1048 = 0.0;
				float W2190_g1048 = 0.0;
				float W3190_g1048 = 0.0;
				StochasticTiling( UV190_g1048 , UV1190_g1048 , UV2190_g1048 , UV3190_g1048 , W1190_g1048 , W2190_g1048 , W3190_g1048 );
				float Input_Index330_g1048 = (float)temp_output_4_0_g1047;
				float2 temp_output_358_0_g1048 = temp_output_12_0_g1047;
				float2 temp_output_359_0_g1048 = temp_output_13_0_g1047;
				float4 Output_2DArray152_g1048 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W1190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W2190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W3190_g1048 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1047 = Output_2DArray152_g1048;
				#else
				float4 staticSwitch7_g1047 = tex2DArrayNode3_g1047;
				#endif
				float4 ifLocalVar17_g1047 = 0;
				UNITY_BRANCH 
				if( break898.w > 0.0 )
				ifLocalVar17_g1047 = staticSwitch7_g1047;
				else if( break898.w == 0.0 )
				ifLocalVar17_g1047 = tex2DArrayNode3_g1047;
				float3 unpack415 = UnpackNormalScale( ifLocalVar17_g1047, break401.w );
				unpack415.z = lerp( 1, unpack415.z, saturate(break401.w) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch436 = float4( unpack415 , 0.0 );
				#else
				float4 staticSwitch436 = _Vector3;
				#endif
				float4 temp_output_24_0_g1053 = staticSwitch436;
				float4 weightedBlendVar30_g1053 = temp_output_14_0_g1053;
				float4 weightedBlend30_g1053 = ( weightedBlendVar30_g1053.x*temp_output_18_0_g1053 + weightedBlendVar30_g1053.y*temp_output_22_0_g1053 + weightedBlendVar30_g1053.z*temp_output_23_0_g1053 + weightedBlendVar30_g1053.w*temp_output_24_0_g1053 );
				float4 break899 = samplingType895;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float2 temp_output_9_0_g11 = Mip101;
				float2 temp_output_12_0_g11 = ddx( temp_output_9_0_g11 );
				float2 temp_output_13_0_g11 = ddy( temp_output_9_0_g11 );
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, temp_output_12_0_g11, temp_output_13_0_g11 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float2 temp_output_358_0_g12 = temp_output_12_0_g11;
				float2 temp_output_359_0_g12 = temp_output_13_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float2 temp_output_9_0_g9 = Mip101;
				float2 temp_output_12_0_g9 = ddx( temp_output_9_0_g9 );
				float2 temp_output_13_0_g9 = ddy( temp_output_9_0_g9 );
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, temp_output_12_0_g9, temp_output_13_0_g9 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float2 temp_output_358_0_g10 = temp_output_12_0_g9;
				float2 temp_output_359_0_g10 = temp_output_13_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float2 temp_output_9_0_g7 = Mip101;
				float2 temp_output_12_0_g7 = ddx( temp_output_9_0_g7 );
				float2 temp_output_13_0_g7 = ddy( temp_output_9_0_g7 );
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, temp_output_12_0_g7, temp_output_13_0_g7 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float2 temp_output_358_0_g8 = temp_output_12_0_g7;
				float2 temp_output_359_0_g8 = temp_output_13_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float2 temp_output_9_0_g1 = Mip101;
				float2 temp_output_12_0_g1 = ddx( temp_output_9_0_g1 );
				float2 temp_output_13_0_g1 = ddy( temp_output_9_0_g1 );
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, temp_output_12_0_g1, temp_output_13_0_g1 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float2 temp_output_358_0_g6 = temp_output_12_0_g1;
				float2 temp_output_359_0_g6 = temp_output_13_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1053 = HeightRawCombined0199;
				float4 break15_g1053 = temp_output_14_0_g1053;
				float temp_output_53_0_g1053 = ( break13_g1053.x + break15_g1053.x );
				float temp_output_54_0_g1053 = ( break13_g1053.y + break15_g1053.y );
				float temp_output_55_0_g1053 = ( break13_g1053.z + break15_g1053.z );
				float temp_output_56_0_g1053 = ( break13_g1053.w + break15_g1053.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1053 = ( max( max( max( temp_output_53_0_g1053 , temp_output_54_0_g1053 ) , temp_output_55_0_g1053 ) , temp_output_56_0_g1053 ) - HeightBlending854 );
				float temp_output_63_0_g1053 = max( ( temp_output_53_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_67_0_g1053 = max( ( temp_output_54_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_71_0_g1053 = max( ( temp_output_55_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_73_0_g1053 = max( ( temp_output_56_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1053 = lerp( weightedBlend30_g1053 , ( ( ( temp_output_18_0_g1053 * temp_output_63_0_g1053 ) + ( temp_output_22_0_g1053 * temp_output_67_0_g1053 ) + ( temp_output_23_0_g1053 * temp_output_71_0_g1053 ) + ( temp_output_24_0_g1053 * temp_output_73_0_g1053 ) ) / ( temp_output_63_0_g1053 + temp_output_67_0_g1053 + temp_output_71_0_g1053 + temp_output_73_0_g1053 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1053 = lerpResult78_g1053;
				#else
				float4 staticSwitch77_g1053 = weightedBlend30_g1053;
				#endif
				float4 Normal0450 = staticSwitch77_g1053;
				float temp_output_395_0 = ( _TimeParameters.x * 0.05 );
				float2 appendResult379 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_397_0 = ( appendResult379 * _PuddleWaveTiling );
				float2 panner408 = ( temp_output_395_0 * float2( 1,0 ) + temp_output_397_0);
				float4 appendResult179 = (float4(WorldPosition.x , WorldPosition.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Snow_Amount174 = _EnviroSnow;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( WorldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float temp_output_406_0 = ( Puddle_Mask264 * ( _PuddleWaveIntensity * Wetness228 ) );
				float3 unpack420 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner408 ), temp_output_406_0 );
				unpack420.z = lerp( 1, unpack420.z, saturate(temp_output_406_0) );
				float2 panner407 = ( temp_output_395_0 * float2( 0,1 ) + temp_output_397_0);
				float3 unpack419 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner407 ), temp_output_406_0 );
				unpack419.z = lerp( 1, unpack419.z, saturate(temp_output_406_0) );
				float3 Puddle447 = BlendNormal( unpack420 , unpack419 );
				float4 lerpResult457 = lerp( Normal0450 , float4( Puddle447 , 0.0 ) , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch462 = lerpResult457;
				#else
				float4 staticSwitch462 = Normal0450;
				#endif
				float Rain_Intensity303 = _EnviroRainIntensity;
				float temp_output_325_0 = (1.0 + (( _RainFlowStrength * Rain_Intensity303 ) - 0.0) * (-1.0 - 1.0) / (1.0 - 0.0));
				float temp_output_306_0 = ( _TimeParameters.x * 0.05 );
				float4 transform287 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord6.xyz , 0.0 ));
				float2 appendResult298 = (float2(( transform287.z * 0.7 ) , ( transform287.y * 0.2 )));
				float2 panner313 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult298 * _RainFlowTiling ));
				float2 texCoord285 = input.ase_texcoord5.xy * float2( 10,10 ) + float2( 0,0 );
				float gradientNoise289 = UnityGradientNoise(texCoord285,_RainFlowDistortionScale);
				gradientNoise289 = gradientNoise289*0.5 + 0.5;
				float Distortion307 = ( gradientNoise289 * _RainFlowDistortionStrenght );
				float simpleNoise324 = SimpleNoise( ( panner313 + Distortion307 )*100.0 );
				simpleNoise324 = simpleNoise324*2 - 1;
				float smoothstepResult332 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise324);
				float temp_output_335_0 = ( ( ( WorldNormal.y - 0.95 ) * -1.0 ) * _RainFlowIntensity );
				float3 temp_cast_56 = (0.3).xxx;
				float3 break337 = ( abs( WorldNormal ) - temp_cast_56 );
				float lerpResult342 = lerp( 0.0 , ( smoothstepResult332 * temp_output_335_0 ) , break337.x);
				float4 transform286 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord6.xyz , 0.0 ));
				float2 appendResult299 = (float2(( transform286.x * 0.7 ) , ( transform286.y * 0.2 )));
				float2 panner312 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult299 * _RainFlowTiling ));
				float simpleNoise328 = SimpleNoise( ( panner312 + Distortion307 )*100.0 );
				simpleNoise328 = simpleNoise328*2 - 1;
				float smoothstepResult333 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise328);
				float lerpResult341 = lerp( 0.0 , ( smoothstepResult333 * temp_output_335_0 ) , break337.z);
				float Rain_Distance_Fade340 = ( 1.0 - sqrt( saturate( ( distance( WorldPosition , _WorldSpaceCameraPos ) / _RainDistanceFade ) ) ) );
				float temp_output_366_0 = saturate( ( ( lerpResult342 + lerpResult341 ) * Rain_Distance_Fade340 ) );
				float temp_output_373_0 = ddx( temp_output_366_0 );
				float temp_output_384_0 = ddy( temp_output_366_0 );
				float3 appendResult445 = (float3(temp_output_373_0 , temp_output_384_0 , sqrt( ( ( 1.0 - ( temp_output_373_0 * temp_output_373_0 ) ) - ( temp_output_384_0 * temp_output_384_0 ) ) )));
				float3 normalizeResult449 = normalize( appendResult445 );
				float3 RainFlow453 = normalizeResult449;
				float localRainRipples1_g1054 = ( 0.0 );
				float2 appendResult426 = (float2(WorldPosition.x , WorldPosition.z));
				float2 UV1_g1054 = ( appendResult426 * _RainDropTiling );
				float AngleOffset1_g1054 = 5.0;
				float lerpResult428 = lerp( 64.0 , 12.0 , Puddle_Mask264);
				float CellDensity1_g1054 = round( lerpResult428 );
				float Time1_g1054 = ( _TimeParameters.x * _RainDropSpeed );
				float temp_output_358_0 = ( _RainDropIntensity * 1.5 );
				float lerpResult365 = lerp( _RainDropIntensity , temp_output_358_0 , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float staticSwitch372 = lerpResult365;
				#else
				float staticSwitch372 = temp_output_358_0;
				#endif
				float switchResult422 = (((ase_vface>0)?(( ( ( WorldNormal.y - 0.7 ) * ( staticSwitch372 * Rain_Intensity303 ) ) * Rain_Distance_Fade340 )):(0.0)));
				float Strength1_g1054 = max( 0.0 , switchResult422 );
				float3 normal1_g1054 = float3( 0,0,0 );
				float Out1_g1054 = 0.0;
				float lerpResult440 = lerp( 5.0 , 8.0 , Puddle_Mask264);
				float pow1_g1054 = lerpResult440;
				float lerpResult439 = lerp( 1.0 , 0.0 , Puddle_Mask264);
				float sin1_g1054 = lerpResult439;
				{
				Rain(UV1_g1054,AngleOffset1_g1054,CellDensity1_g1054,Time1_g1054,Strength1_g1054,pow1_g1054,sin1_g1054,Out1_g1054,normal1_g1054);
				}
				float3 Rain_Drop452 = normal1_g1054;
				#ifdef _RAIN_ON
				float4 staticSwitch468 = float4( BlendNormal( staticSwitch462.xyz , BlendNormal( RainFlow453 , Rain_Drop452 ) ) , 0.0 );
				#else
				float4 staticSwitch468 = staticSwitch462;
				#endif
				float2 temp_cast_59 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord5.xy * temp_cast_59 + float2( 0,0 );
				float2 temp_output_5_0_g1055 = texCoord232;
				float localStochasticTiling2_g1056 = ( 0.0 );
				float2 Input_UV145_g1056 = temp_output_5_0_g1055;
				float2 UV2_g1056 = Input_UV145_g1056;
				float2 UV12_g1056 = float2( 0,0 );
				float2 UV22_g1056 = float2( 0,0 );
				float2 UV32_g1056 = float2( 0,0 );
				float W12_g1056 = 0.0;
				float W22_g1056 = 0.0;
				float W32_g1056 = 0.0;
				StochasticTiling( UV2_g1056 , UV12_g1056 , UV22_g1056 , UV32_g1056 , W12_g1056 , W22_g1056 , W32_g1056 );
				float2 temp_output_10_0_g1056 = ddx( Input_UV145_g1056 );
				float2 temp_output_12_0_g1056 = ddy( Input_UV145_g1056 );
				float4 Output_2D293_g1056 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV12_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W12_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV22_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W22_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV32_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W32_g1056 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1055 = Output_2D293_g1056;
				#else
				float4 staticSwitch7_g1055 = SAMPLE_TEXTURE2D( _SnowNormal, sampler_SnowNormal, temp_output_5_0_g1055 );
				#endif
				float3 unpack463 = UnpackNormalScale( staticSwitch7_g1055, _SnowNormalScale );
				unpack463.z = lerp( 1, unpack463.z, saturate(_SnowNormalScale) );
				float3 Snow_Normal465 = unpack463;
				float2 appendResult202 = (float2(WorldPosition.x , WorldPosition.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( WorldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord6.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord6.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float4 lerpResult470 = lerp( staticSwitch468 , float4( Snow_Normal465 , 0.0 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch471 = lerpResult470;
				#else
				float4 staticSwitch471 = staticSwitch468;
				#endif
				float4 Normal_Final472 = staticSwitch471;
				float4 break668 = Normal_Final472;
				float3 appendResult671 = (float3(break668.x , break668.y , ( break668.z + 0.001 )));
				#ifdef _TERRAIN_INSTANCED_PERPIXEL_NORMAL
				float3 staticSwitch665 = appendResult671;
				#else
				float3 staticSwitch665 = appendResult671;
				#endif
				
				float2 uv_TerrainHolesTexture = input.ase_texcoord5.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Alpha1008 = holeClipValue579;
				

				float3 Normal = staticSwitch665;
				float Alpha = Alpha1008;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(WorldNormal);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float crossSign = (WorldTangent.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
							float3 bitangent = crossSign * cross(WorldNormal.xyz, WorldTangent.xyz);
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent.xyz, bitangent, WorldNormal.xyz));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = WorldNormal;
					#endif
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
			#pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_GBUFFER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif
			
			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#pragma multi_compile_local _RAIN_ON
			#pragma multi_compile_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
			#include "EnviroInclude.hlsl"


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float4 lightmapUVOrVertexSH : TEXCOORD1;
				half4 fogFactorAndVertexLight : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
				float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD8;
				#endif
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_texcoord10 : TEXCOORD10;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D_ARRAY(_AlbedoArray);
			SAMPLER(sampler_AlbedoArray);
			TEXTURE2D(_SnowAlbedo);
			SAMPLER(sampler_SnowAlbedo);
			TEXTURE2D_ARRAY(_NormalArray);
			SAMPLER(sampler_NormalArray);
			TEXTURE2D(_WaveNormal);
			SAMPLER(sampler_WaveNormal);
			TEXTURE2D(_SnowNormal);
			SAMPLER(sampler_SnowNormal);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			
			float2 UnityGradientNoiseDir( float2 p )
			{
				p = fmod(p , 289);
				float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
				x = fmod( (34 * x + 1) * x , 289);
				x = frac( x / 41 ) * 2 - 1;
				return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
			}
			
			float UnityGradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 ip = floor( p );
				float2 fp = frac( p );
				float d00 = dot( UnityGradientNoiseDir( ip ), fp );
				float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
				float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
				float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
				fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
				return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				float4 appendResult660 = (float4(cross( input.normalOS , float3(0,0,1) ) , -1.0));
				
				output.ase_texcoord9.xy = input.texcoord.xy;
				output.ase_texcoord10 = input.positionOS;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord9.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = appendResult660;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				output.tSpace0 = float4( normalInput.normalWS, vertexInput.positionWS.x);
				output.tSpace1 = float4( normalInput.tangentWS, vertexInput.positionWS.y);
				output.tSpace2 = float4( normalInput.bitangentWS, vertexInput.positionWS.z);

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				OUTPUT_SH4( vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir( vertexInput.positionWS ), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					output.lightmapUVOrVertexSH.zw = input.texcoord.xy;
					output.lightmapUVOrVertexSH.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );

				output.fogFactorAndVertexLight = half4(0, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					output.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				output.positionCS = vertexInput.positionCS;
				output.clipPosV = vertexInput.positionCS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				output.texcoord1 = input.texcoord1;
				output.texcoord2 = input.texcoord2;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			FragmentOutput frag ( PackedVaryings input
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								, bool ase_vface : SV_IsFrontFace )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (input.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( input.tSpace0.xyz );
					float3 WorldTangent = input.tSpace1.xyz;
					float3 WorldBiTangent = input.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(input.tSpace0.w,input.tSpace1.w,input.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				float4 ClipPos = input.clipPosV;
				float4 ScreenPos = ComputeScreenPos( input.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = input.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#else
					ShadowCoords = float4(0, 0, 0, 0);
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord9.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D( _Control, sampler_Control, uv_Control );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord9.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D( _Control1, sampler_Control1, uv_Control1 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord9.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D( _Control2, sampler_Control2, uv_Control2 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1066 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break896 = samplingType895;
				float2 appendResult69 = (float2(input.ase_texcoord10.xyz.x , input.ase_texcoord10.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float3 break485 = UV0100;
				float2 appendResult493 = (float2(break485.x , break485.y));
				float2 temp_output_5_0_g1057 = appendResult493;
				int temp_output_4_0_g1057 = (int)break485.z;
				float2 appendResult87 = (float2(input.ase_texcoord10.xyz.x , input.ase_texcoord10.xyz.z));
				float2 Mip101 = ( appendResult87 * ( 1.0 / max( 0.001 , _MipDistanceBlending ) ) );
				float2 temp_output_9_0_g1057 = Mip101;
				float2 temp_output_12_0_g1057 = ddx( temp_output_9_0_g1057 );
				float2 temp_output_13_0_g1057 = ddy( temp_output_9_0_g1057 );
				float4 tex2DArrayNode3_g1057 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1057,(float)temp_output_4_0_g1057, temp_output_12_0_g1057, temp_output_13_0_g1057 );
				float localStochasticTiling190_g1058 = ( 0.0 );
				float2 Input_UV317_g1058 = temp_output_5_0_g1057;
				float2 UV190_g1058 = Input_UV317_g1058;
				float2 UV1190_g1058 = float2( 0,0 );
				float2 UV2190_g1058 = float2( 0,0 );
				float2 UV3190_g1058 = float2( 0,0 );
				float W1190_g1058 = 0.0;
				float W2190_g1058 = 0.0;
				float W3190_g1058 = 0.0;
				StochasticTiling( UV190_g1058 , UV1190_g1058 , UV2190_g1058 , UV3190_g1058 , W1190_g1058 , W2190_g1058 , W3190_g1058 );
				float Input_Index330_g1058 = (float)temp_output_4_0_g1057;
				float2 temp_output_358_0_g1058 = temp_output_12_0_g1057;
				float2 temp_output_359_0_g1058 = temp_output_13_0_g1057;
				float4 Output_2DArray152_g1058 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W1190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W2190_g1058 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1058,Input_Index330_g1058, temp_output_358_0_g1058, temp_output_359_0_g1058 ) * W3190_g1058 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1057 = Output_2DArray152_g1058;
				#else
				float4 staticSwitch7_g1057 = tex2DArrayNode3_g1057;
				#endif
				float4 ifLocalVar17_g1057 = 0;
				UNITY_BRANCH 
				if( break896.x > 0.0 )
				ifLocalVar17_g1057 = staticSwitch7_g1057;
				else if( break896.x == 0.0 )
				ifLocalVar17_g1057 = tex2DArrayNode3_g1057;
				float localGetUVS795 = ( 0.0 );
				float4 in0795 = _ColorTint0;
				float4 in1795 = _ColorTint1;
				float4 in2795 = _ColorTint2;
				float4 in3795 = _ColorTint3;
				float4 in4795 = _ColorTint4;
				float4 in5795 = _ColorTint5;
				float4 in6795 = _ColorTint6;
				float4 in7795 = _ColorTint7;
				float4 in8795 = _ColorTint8;
				float4 in9795 = _ColorTint9;
				float4 in10795 = _ColorTint10;
				float4 in11795 = _ColorTint11;
				float4 index795 = SplatIndex44;
				float4 Out0795 = float4( 0,0,0,0 );
				float4 Out1795 = float4( 0,0,0,0 );
				float4 Out2795 = float4( 0,0,0,0 );
				float4 Out3795 = float4( 0,0,0,0 );
				{
				GetLayerUV(in0795,in1795,in2795,in3795,in4795,in5795,in6795,in7795,in8795,in9795,in10795,in11795,index795,Out0795,Out1795,Out2795,Out3795);
				}
				float4 Color0796 = Out0795;
				float4 temp_output_616_0 = ( ifLocalVar17_g1057 * Color0796 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch511 = temp_output_616_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch511 = temp_output_616_0;
				#else
				float4 staticSwitch511 = temp_output_616_0;
				#endif
				float4 temp_output_2_0_g1065 = staticSwitch511;
				float4 temp_output_18_0_g1066 = float4( (temp_output_2_0_g1065).rgb , 0.0 );
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float3 break487 = UV197;
				float2 appendResult492 = (float2(break487.x , break487.y));
				float2 temp_output_5_0_g1061 = appendResult492;
				int temp_output_4_0_g1061 = (int)break487.z;
				float2 temp_output_9_0_g1061 = Mip101;
				float2 temp_output_12_0_g1061 = ddx( temp_output_9_0_g1061 );
				float2 temp_output_13_0_g1061 = ddy( temp_output_9_0_g1061 );
				float4 tex2DArrayNode3_g1061 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1061,(float)temp_output_4_0_g1061, temp_output_12_0_g1061, temp_output_13_0_g1061 );
				float localStochasticTiling190_g1062 = ( 0.0 );
				float2 Input_UV317_g1062 = temp_output_5_0_g1061;
				float2 UV190_g1062 = Input_UV317_g1062;
				float2 UV1190_g1062 = float2( 0,0 );
				float2 UV2190_g1062 = float2( 0,0 );
				float2 UV3190_g1062 = float2( 0,0 );
				float W1190_g1062 = 0.0;
				float W2190_g1062 = 0.0;
				float W3190_g1062 = 0.0;
				StochasticTiling( UV190_g1062 , UV1190_g1062 , UV2190_g1062 , UV3190_g1062 , W1190_g1062 , W2190_g1062 , W3190_g1062 );
				float Input_Index330_g1062 = (float)temp_output_4_0_g1061;
				float2 temp_output_358_0_g1062 = temp_output_12_0_g1061;
				float2 temp_output_359_0_g1062 = temp_output_13_0_g1061;
				float4 Output_2DArray152_g1062 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W1190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W2190_g1062 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1062,Input_Index330_g1062, temp_output_358_0_g1062, temp_output_359_0_g1062 ) * W3190_g1062 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1061 = Output_2DArray152_g1062;
				#else
				float4 staticSwitch7_g1061 = tex2DArrayNode3_g1061;
				#endif
				float4 ifLocalVar17_g1061 = 0;
				UNITY_BRANCH 
				if( break896.y > 0.0 )
				ifLocalVar17_g1061 = staticSwitch7_g1061;
				else if( break896.y == 0.0 )
				ifLocalVar17_g1061 = tex2DArrayNode3_g1061;
				float4 Color1797 = Out1795;
				float4 temp_output_617_0 = ( ifLocalVar17_g1061 * Color1797 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch508 = temp_output_617_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch508 = temp_output_617_0;
				#else
				float4 staticSwitch508 = temp_output_617_0;
				#endif
				float4 temp_output_22_0_g1066 = staticSwitch508;
				float4 _Vector2 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float3 break488 = UV298;
				float2 appendResult495 = (float2(break488.x , break488.y));
				float2 temp_output_5_0_g1063 = appendResult495;
				int temp_output_4_0_g1063 = (int)break488.z;
				float2 temp_output_9_0_g1063 = Mip101;
				float2 temp_output_12_0_g1063 = ddx( temp_output_9_0_g1063 );
				float2 temp_output_13_0_g1063 = ddy( temp_output_9_0_g1063 );
				float4 tex2DArrayNode3_g1063 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1063,(float)temp_output_4_0_g1063, temp_output_12_0_g1063, temp_output_13_0_g1063 );
				float localStochasticTiling190_g1064 = ( 0.0 );
				float2 Input_UV317_g1064 = temp_output_5_0_g1063;
				float2 UV190_g1064 = Input_UV317_g1064;
				float2 UV1190_g1064 = float2( 0,0 );
				float2 UV2190_g1064 = float2( 0,0 );
				float2 UV3190_g1064 = float2( 0,0 );
				float W1190_g1064 = 0.0;
				float W2190_g1064 = 0.0;
				float W3190_g1064 = 0.0;
				StochasticTiling( UV190_g1064 , UV1190_g1064 , UV2190_g1064 , UV3190_g1064 , W1190_g1064 , W2190_g1064 , W3190_g1064 );
				float Input_Index330_g1064 = (float)temp_output_4_0_g1063;
				float2 temp_output_358_0_g1064 = temp_output_12_0_g1063;
				float2 temp_output_359_0_g1064 = temp_output_13_0_g1063;
				float4 Output_2DArray152_g1064 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W1190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W2190_g1064 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1064,Input_Index330_g1064, temp_output_358_0_g1064, temp_output_359_0_g1064 ) * W3190_g1064 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1063 = Output_2DArray152_g1064;
				#else
				float4 staticSwitch7_g1063 = tex2DArrayNode3_g1063;
				#endif
				float4 ifLocalVar17_g1063 = 0;
				UNITY_BRANCH 
				if( break896.z > 0.0 )
				ifLocalVar17_g1063 = staticSwitch7_g1063;
				else if( break896.z == 0.0 )
				ifLocalVar17_g1063 = tex2DArrayNode3_g1063;
				float4 Color2798 = Out2795;
				float4 temp_output_618_0 = ( ifLocalVar17_g1063 * Color2798 );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch509 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch509 = temp_output_618_0;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch509 = temp_output_618_0;
				#else
				float4 staticSwitch509 = _Vector2;
				#endif
				float4 temp_output_23_0_g1066 = staticSwitch509;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float3 break486 = UV399;
				float2 appendResult491 = (float2(break486.x , break486.y));
				float2 temp_output_5_0_g1059 = appendResult491;
				int temp_output_4_0_g1059 = (int)break486.z;
				float2 temp_output_9_0_g1059 = Mip101;
				float2 temp_output_12_0_g1059 = ddx( temp_output_9_0_g1059 );
				float2 temp_output_13_0_g1059 = ddy( temp_output_9_0_g1059 );
				float4 tex2DArrayNode3_g1059 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, temp_output_5_0_g1059,(float)temp_output_4_0_g1059, temp_output_12_0_g1059, temp_output_13_0_g1059 );
				float localStochasticTiling190_g1060 = ( 0.0 );
				float2 Input_UV317_g1060 = temp_output_5_0_g1059;
				float2 UV190_g1060 = Input_UV317_g1060;
				float2 UV1190_g1060 = float2( 0,0 );
				float2 UV2190_g1060 = float2( 0,0 );
				float2 UV3190_g1060 = float2( 0,0 );
				float W1190_g1060 = 0.0;
				float W2190_g1060 = 0.0;
				float W3190_g1060 = 0.0;
				StochasticTiling( UV190_g1060 , UV1190_g1060 , UV2190_g1060 , UV3190_g1060 , W1190_g1060 , W2190_g1060 , W3190_g1060 );
				float Input_Index330_g1060 = (float)temp_output_4_0_g1059;
				float2 temp_output_358_0_g1060 = temp_output_12_0_g1059;
				float2 temp_output_359_0_g1060 = temp_output_13_0_g1059;
				float4 Output_2DArray152_g1060 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV1190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W1190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV2190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W2190_g1060 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _AlbedoArray, sampler_AlbedoArray, UV3190_g1060,Input_Index330_g1060, temp_output_358_0_g1060, temp_output_359_0_g1060 ) * W3190_g1060 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1059 = Output_2DArray152_g1060;
				#else
				float4 staticSwitch7_g1059 = tex2DArrayNode3_g1059;
				#endif
				float4 ifLocalVar17_g1059 = 0;
				UNITY_BRANCH 
				if( break896.w > 0.0 )
				ifLocalVar17_g1059 = staticSwitch7_g1059;
				else if( break896.w == 0.0 )
				ifLocalVar17_g1059 = tex2DArrayNode3_g1059;
				float4 Color3799 = Out3795;
				#if defined( _QUALITY_FAST )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch510 = _Vector2;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch510 = ( ifLocalVar17_g1059 * Color3799 );
				#else
				float4 staticSwitch510 = _Vector2;
				#endif
				float4 temp_output_24_0_g1066 = staticSwitch510;
				float4 weightedBlendVar30_g1066 = temp_output_14_0_g1066;
				float4 weightedBlend30_g1066 = ( weightedBlendVar30_g1066.x*temp_output_18_0_g1066 + weightedBlendVar30_g1066.y*temp_output_22_0_g1066 + weightedBlendVar30_g1066.z*temp_output_23_0_g1066 + weightedBlendVar30_g1066.w*temp_output_24_0_g1066 );
				float4 break899 = samplingType895;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float2 temp_output_9_0_g11 = Mip101;
				float2 temp_output_12_0_g11 = ddx( temp_output_9_0_g11 );
				float2 temp_output_13_0_g11 = ddy( temp_output_9_0_g11 );
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, temp_output_12_0_g11, temp_output_13_0_g11 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float2 temp_output_358_0_g12 = temp_output_12_0_g11;
				float2 temp_output_359_0_g12 = temp_output_13_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, temp_output_358_0_g12, temp_output_359_0_g12 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float2 temp_output_9_0_g9 = Mip101;
				float2 temp_output_12_0_g9 = ddx( temp_output_9_0_g9 );
				float2 temp_output_13_0_g9 = ddy( temp_output_9_0_g9 );
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, temp_output_12_0_g9, temp_output_13_0_g9 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float2 temp_output_358_0_g10 = temp_output_12_0_g9;
				float2 temp_output_359_0_g10 = temp_output_13_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, temp_output_358_0_g10, temp_output_359_0_g10 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float2 temp_output_9_0_g7 = Mip101;
				float2 temp_output_12_0_g7 = ddx( temp_output_9_0_g7 );
				float2 temp_output_13_0_g7 = ddy( temp_output_9_0_g7 );
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, temp_output_12_0_g7, temp_output_13_0_g7 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float2 temp_output_358_0_g8 = temp_output_12_0_g7;
				float2 temp_output_359_0_g8 = temp_output_13_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, temp_output_358_0_g8, temp_output_359_0_g8 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float2 temp_output_9_0_g1 = Mip101;
				float2 temp_output_12_0_g1 = ddx( temp_output_9_0_g1 );
				float2 temp_output_13_0_g1 = ddy( temp_output_9_0_g1 );
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, temp_output_12_0_g1, temp_output_13_0_g1 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float2 temp_output_358_0_g6 = temp_output_12_0_g1;
				float2 temp_output_359_0_g6 = temp_output_13_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, temp_output_358_0_g6, temp_output_359_0_g6 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1066 = HeightRawCombined0199;
				float4 break15_g1066 = temp_output_14_0_g1066;
				float temp_output_53_0_g1066 = ( break13_g1066.x + break15_g1066.x );
				float temp_output_54_0_g1066 = ( break13_g1066.y + break15_g1066.y );
				float temp_output_55_0_g1066 = ( break13_g1066.z + break15_g1066.z );
				float temp_output_56_0_g1066 = ( break13_g1066.w + break15_g1066.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1066 = ( max( max( max( temp_output_53_0_g1066 , temp_output_54_0_g1066 ) , temp_output_55_0_g1066 ) , temp_output_56_0_g1066 ) - HeightBlending854 );
				float temp_output_63_0_g1066 = max( ( temp_output_53_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_67_0_g1066 = max( ( temp_output_54_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_71_0_g1066 = max( ( temp_output_55_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float temp_output_73_0_g1066 = max( ( temp_output_56_0_g1066 - temp_output_79_0_g1066 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1066 = lerp( weightedBlend30_g1066 , ( ( ( temp_output_18_0_g1066 * temp_output_63_0_g1066 ) + ( temp_output_22_0_g1066 * temp_output_67_0_g1066 ) + ( temp_output_23_0_g1066 * temp_output_71_0_g1066 ) + ( temp_output_24_0_g1066 * temp_output_73_0_g1066 ) ) / ( temp_output_63_0_g1066 + temp_output_67_0_g1066 + temp_output_71_0_g1066 + temp_output_73_0_g1066 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1066 = lerpResult78_g1066;
				#else
				float4 staticSwitch77_g1066 = weightedBlend30_g1066;
				#endif
				float4 Albedo0520 = staticSwitch77_g1066;
				float4 appendResult179 = (float4(WorldPosition.x , WorldPosition.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Snow_Amount174 = _EnviroSnow;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( WorldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float4 lerpResult524 = lerp( float4( 1,1,1,0 ) , _PuddleColor , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch543 = ( Albedo0520 * lerpResult524 );
				#else
				float4 staticSwitch543 = Albedo0520;
				#endif
				float2 temp_cast_63 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord9.xy * temp_cast_63 + float2( 0,0 );
				float2 temp_output_5_0_g1067 = texCoord232;
				float localStochasticTiling2_g1068 = ( 0.0 );
				float2 Input_UV145_g1068 = temp_output_5_0_g1067;
				float2 UV2_g1068 = Input_UV145_g1068;
				float2 UV12_g1068 = float2( 0,0 );
				float2 UV22_g1068 = float2( 0,0 );
				float2 UV32_g1068 = float2( 0,0 );
				float W12_g1068 = 0.0;
				float W22_g1068 = 0.0;
				float W32_g1068 = 0.0;
				StochasticTiling( UV2_g1068 , UV12_g1068 , UV22_g1068 , UV32_g1068 , W12_g1068 , W22_g1068 , W32_g1068 );
				float2 temp_output_10_0_g1068 = ddx( Input_UV145_g1068 );
				float2 temp_output_12_0_g1068 = ddy( Input_UV145_g1068 );
				float4 Output_2D293_g1068 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV12_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W12_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV22_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W22_g1068 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowAlbedo, sampler_SnowAlbedo, UV32_g1068, temp_output_10_0_g1068, temp_output_12_0_g1068 ) * W32_g1068 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1067 = Output_2D293_g1068;
				#else
				float4 staticSwitch7_g1067 = SAMPLE_TEXTURE2D( _SnowAlbedo, sampler_SnowAlbedo, temp_output_5_0_g1067 );
				#endif
				float4 Snow_Albedo522 = staticSwitch7_g1067;
				float4 temp_output_14_0_g1053 = SplatWeights198;
				float4 break898 = samplingType895;
				float2 temp_output_5_0_g1049 = UV0100.xy;
				float4 break391 = SplatIndex44;
				int temp_output_4_0_g1049 = (int)break391.x;
				float2 temp_output_9_0_g1049 = Mip101;
				float2 temp_output_12_0_g1049 = ddx( temp_output_9_0_g1049 );
				float2 temp_output_13_0_g1049 = ddy( temp_output_9_0_g1049 );
				float4 tex2DArrayNode3_g1049 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1049,(float)temp_output_4_0_g1049, temp_output_12_0_g1049, temp_output_13_0_g1049 );
				float localStochasticTiling190_g1050 = ( 0.0 );
				float2 Input_UV317_g1050 = temp_output_5_0_g1049;
				float2 UV190_g1050 = Input_UV317_g1050;
				float2 UV1190_g1050 = float2( 0,0 );
				float2 UV2190_g1050 = float2( 0,0 );
				float2 UV3190_g1050 = float2( 0,0 );
				float W1190_g1050 = 0.0;
				float W2190_g1050 = 0.0;
				float W3190_g1050 = 0.0;
				StochasticTiling( UV190_g1050 , UV1190_g1050 , UV2190_g1050 , UV3190_g1050 , W1190_g1050 , W2190_g1050 , W3190_g1050 );
				float Input_Index330_g1050 = (float)temp_output_4_0_g1049;
				float2 temp_output_358_0_g1050 = temp_output_12_0_g1049;
				float2 temp_output_359_0_g1050 = temp_output_13_0_g1049;
				float4 Output_2DArray152_g1050 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W1190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W2190_g1050 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1050,Input_Index330_g1050, temp_output_358_0_g1050, temp_output_359_0_g1050 ) * W3190_g1050 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1049 = Output_2DArray152_g1050;
				#else
				float4 staticSwitch7_g1049 = tex2DArrayNode3_g1049;
				#endif
				float4 ifLocalVar17_g1049 = 0;
				UNITY_BRANCH 
				if( break898.x > 0.0 )
				ifLocalVar17_g1049 = staticSwitch7_g1049;
				else if( break898.x == 0.0 )
				ifLocalVar17_g1049 = tex2DArrayNode3_g1049;
				float localGetLayerSettings368 = ( 0.0 );
				float4 in0368 = _NormalScale00;
				float4 in1368 = _NormalScale01;
				float4 in2368 = _NormalScale02;
				float4 index368 = SplatIndex44;
				float4 Out0368 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0368,in1368,in2368,index368,Out0368);
				}
				float4 NormalScales375 = Out0368;
				float4 break401 = NormalScales375;
				float3 unpack417 = UnpackNormalScale( ifLocalVar17_g1049, break401.x );
				unpack417.z = lerp( 1, unpack417.z, saturate(break401.x) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch433 = unpack417;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch433 = unpack417;
				#else
				float3 staticSwitch433 = unpack417;
				#endif
				float4 temp_output_18_0_g1053 = float4( staticSwitch433 , 0.0 );
				float2 temp_output_5_0_g1045 = UV197.xy;
				int temp_output_4_0_g1045 = (int)break391.y;
				float2 temp_output_9_0_g1045 = Mip101;
				float2 temp_output_12_0_g1045 = ddx( temp_output_9_0_g1045 );
				float2 temp_output_13_0_g1045 = ddy( temp_output_9_0_g1045 );
				float4 tex2DArrayNode3_g1045 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1045,(float)temp_output_4_0_g1045, temp_output_12_0_g1045, temp_output_13_0_g1045 );
				float localStochasticTiling190_g1046 = ( 0.0 );
				float2 Input_UV317_g1046 = temp_output_5_0_g1045;
				float2 UV190_g1046 = Input_UV317_g1046;
				float2 UV1190_g1046 = float2( 0,0 );
				float2 UV2190_g1046 = float2( 0,0 );
				float2 UV3190_g1046 = float2( 0,0 );
				float W1190_g1046 = 0.0;
				float W2190_g1046 = 0.0;
				float W3190_g1046 = 0.0;
				StochasticTiling( UV190_g1046 , UV1190_g1046 , UV2190_g1046 , UV3190_g1046 , W1190_g1046 , W2190_g1046 , W3190_g1046 );
				float Input_Index330_g1046 = (float)temp_output_4_0_g1045;
				float2 temp_output_358_0_g1046 = temp_output_12_0_g1045;
				float2 temp_output_359_0_g1046 = temp_output_13_0_g1045;
				float4 Output_2DArray152_g1046 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W1190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W2190_g1046 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1046,Input_Index330_g1046, temp_output_358_0_g1046, temp_output_359_0_g1046 ) * W3190_g1046 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1045 = Output_2DArray152_g1046;
				#else
				float4 staticSwitch7_g1045 = tex2DArrayNode3_g1045;
				#endif
				float4 ifLocalVar17_g1045 = 0;
				UNITY_BRANCH 
				if( break898.y > 0.0 )
				ifLocalVar17_g1045 = staticSwitch7_g1045;
				else if( break898.y == 0.0 )
				ifLocalVar17_g1045 = tex2DArrayNode3_g1045;
				float3 unpack416 = UnpackNormalScale( ifLocalVar17_g1045, break401.y );
				unpack416.z = lerp( 1, unpack416.z, saturate(break401.y) );
				#if defined( _QUALITY_FAST )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_BALANCE )
				float3 staticSwitch434 = unpack416;
				#elif defined( _QUALITY_QUALITY )
				float3 staticSwitch434 = unpack416;
				#else
				float3 staticSwitch434 = unpack416;
				#endif
				float4 temp_output_22_0_g1053 = float4( staticSwitch434 , 0.0 );
				float4 _Vector3 = float4(0,0,0,0);
				float2 temp_output_5_0_g1051 = UV298.xy;
				int temp_output_4_0_g1051 = (int)break391.z;
				float2 temp_output_9_0_g1051 = Mip101;
				float2 temp_output_12_0_g1051 = ddx( temp_output_9_0_g1051 );
				float2 temp_output_13_0_g1051 = ddy( temp_output_9_0_g1051 );
				float4 tex2DArrayNode3_g1051 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1051,(float)temp_output_4_0_g1051, temp_output_12_0_g1051, temp_output_13_0_g1051 );
				float localStochasticTiling190_g1052 = ( 0.0 );
				float2 Input_UV317_g1052 = temp_output_5_0_g1051;
				float2 UV190_g1052 = Input_UV317_g1052;
				float2 UV1190_g1052 = float2( 0,0 );
				float2 UV2190_g1052 = float2( 0,0 );
				float2 UV3190_g1052 = float2( 0,0 );
				float W1190_g1052 = 0.0;
				float W2190_g1052 = 0.0;
				float W3190_g1052 = 0.0;
				StochasticTiling( UV190_g1052 , UV1190_g1052 , UV2190_g1052 , UV3190_g1052 , W1190_g1052 , W2190_g1052 , W3190_g1052 );
				float Input_Index330_g1052 = (float)temp_output_4_0_g1051;
				float2 temp_output_358_0_g1052 = temp_output_12_0_g1051;
				float2 temp_output_359_0_g1052 = temp_output_13_0_g1051;
				float4 Output_2DArray152_g1052 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W1190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W2190_g1052 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1052,Input_Index330_g1052, temp_output_358_0_g1052, temp_output_359_0_g1052 ) * W3190_g1052 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1051 = Output_2DArray152_g1052;
				#else
				float4 staticSwitch7_g1051 = tex2DArrayNode3_g1051;
				#endif
				float4 ifLocalVar17_g1051 = 0;
				UNITY_BRANCH 
				if( break898.z > 0.0 )
				ifLocalVar17_g1051 = staticSwitch7_g1051;
				else if( break898.z == 0.0 )
				ifLocalVar17_g1051 = tex2DArrayNode3_g1051;
				float3 unpack414 = UnpackNormalScale( ifLocalVar17_g1051, break401.z );
				unpack414.z = lerp( 1, unpack414.z, saturate(break401.z) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch435 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch435 = float4( unpack414 , 0.0 );
				#else
				float4 staticSwitch435 = _Vector3;
				#endif
				float4 temp_output_23_0_g1053 = staticSwitch435;
				float2 temp_output_5_0_g1047 = UV399.xy;
				int temp_output_4_0_g1047 = (int)break391.w;
				float2 temp_output_9_0_g1047 = Mip101;
				float2 temp_output_12_0_g1047 = ddx( temp_output_9_0_g1047 );
				float2 temp_output_13_0_g1047 = ddy( temp_output_9_0_g1047 );
				float4 tex2DArrayNode3_g1047 = SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, temp_output_5_0_g1047,(float)temp_output_4_0_g1047, temp_output_12_0_g1047, temp_output_13_0_g1047 );
				float localStochasticTiling190_g1048 = ( 0.0 );
				float2 Input_UV317_g1048 = temp_output_5_0_g1047;
				float2 UV190_g1048 = Input_UV317_g1048;
				float2 UV1190_g1048 = float2( 0,0 );
				float2 UV2190_g1048 = float2( 0,0 );
				float2 UV3190_g1048 = float2( 0,0 );
				float W1190_g1048 = 0.0;
				float W2190_g1048 = 0.0;
				float W3190_g1048 = 0.0;
				StochasticTiling( UV190_g1048 , UV1190_g1048 , UV2190_g1048 , UV3190_g1048 , W1190_g1048 , W2190_g1048 , W3190_g1048 );
				float Input_Index330_g1048 = (float)temp_output_4_0_g1047;
				float2 temp_output_358_0_g1048 = temp_output_12_0_g1047;
				float2 temp_output_359_0_g1048 = temp_output_13_0_g1047;
				float4 Output_2DArray152_g1048 = ( ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV1190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W1190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV2190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W2190_g1048 ) + ( SAMPLE_TEXTURE2D_ARRAY_GRAD( _NormalArray, sampler_NormalArray, UV3190_g1048,Input_Index330_g1048, temp_output_358_0_g1048, temp_output_359_0_g1048 ) * W3190_g1048 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1047 = Output_2DArray152_g1048;
				#else
				float4 staticSwitch7_g1047 = tex2DArrayNode3_g1047;
				#endif
				float4 ifLocalVar17_g1047 = 0;
				UNITY_BRANCH 
				if( break898.w > 0.0 )
				ifLocalVar17_g1047 = staticSwitch7_g1047;
				else if( break898.w == 0.0 )
				ifLocalVar17_g1047 = tex2DArrayNode3_g1047;
				float3 unpack415 = UnpackNormalScale( ifLocalVar17_g1047, break401.w );
				unpack415.z = lerp( 1, unpack415.z, saturate(break401.w) );
				#if defined( _QUALITY_FAST )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch436 = _Vector3;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch436 = float4( unpack415 , 0.0 );
				#else
				float4 staticSwitch436 = _Vector3;
				#endif
				float4 temp_output_24_0_g1053 = staticSwitch436;
				float4 weightedBlendVar30_g1053 = temp_output_14_0_g1053;
				float4 weightedBlend30_g1053 = ( weightedBlendVar30_g1053.x*temp_output_18_0_g1053 + weightedBlendVar30_g1053.y*temp_output_22_0_g1053 + weightedBlendVar30_g1053.z*temp_output_23_0_g1053 + weightedBlendVar30_g1053.w*temp_output_24_0_g1053 );
				float4 break13_g1053 = HeightRawCombined0199;
				float4 break15_g1053 = temp_output_14_0_g1053;
				float temp_output_53_0_g1053 = ( break13_g1053.x + break15_g1053.x );
				float temp_output_54_0_g1053 = ( break13_g1053.y + break15_g1053.y );
				float temp_output_55_0_g1053 = ( break13_g1053.z + break15_g1053.z );
				float temp_output_56_0_g1053 = ( break13_g1053.w + break15_g1053.w );
				float temp_output_79_0_g1053 = ( max( max( max( temp_output_53_0_g1053 , temp_output_54_0_g1053 ) , temp_output_55_0_g1053 ) , temp_output_56_0_g1053 ) - HeightBlending854 );
				float temp_output_63_0_g1053 = max( ( temp_output_53_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_67_0_g1053 = max( ( temp_output_54_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_71_0_g1053 = max( ( temp_output_55_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float temp_output_73_0_g1053 = max( ( temp_output_56_0_g1053 - temp_output_79_0_g1053 ) , 0.0 );
				float4 lerpResult78_g1053 = lerp( weightedBlend30_g1053 , ( ( ( temp_output_18_0_g1053 * temp_output_63_0_g1053 ) + ( temp_output_22_0_g1053 * temp_output_67_0_g1053 ) + ( temp_output_23_0_g1053 * temp_output_71_0_g1053 ) + ( temp_output_24_0_g1053 * temp_output_73_0_g1053 ) ) / ( temp_output_63_0_g1053 + temp_output_67_0_g1053 + temp_output_71_0_g1053 + temp_output_73_0_g1053 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1053 = lerpResult78_g1053;
				#else
				float4 staticSwitch77_g1053 = weightedBlend30_g1053;
				#endif
				float4 Normal0450 = staticSwitch77_g1053;
				float temp_output_395_0 = ( _TimeParameters.x * 0.05 );
				float2 appendResult379 = (float2(WorldPosition.x , WorldPosition.z));
				float2 temp_output_397_0 = ( appendResult379 * _PuddleWaveTiling );
				float2 panner408 = ( temp_output_395_0 * float2( 1,0 ) + temp_output_397_0);
				float temp_output_406_0 = ( Puddle_Mask264 * ( _PuddleWaveIntensity * Wetness228 ) );
				float3 unpack420 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner408 ), temp_output_406_0 );
				unpack420.z = lerp( 1, unpack420.z, saturate(temp_output_406_0) );
				float2 panner407 = ( temp_output_395_0 * float2( 0,1 ) + temp_output_397_0);
				float3 unpack419 = UnpackNormalScale( SAMPLE_TEXTURE2D( _WaveNormal, sampler_WaveNormal, panner407 ), temp_output_406_0 );
				unpack419.z = lerp( 1, unpack419.z, saturate(temp_output_406_0) );
				float3 Puddle447 = BlendNormal( unpack420 , unpack419 );
				float4 lerpResult457 = lerp( Normal0450 , float4( Puddle447 , 0.0 ) , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float4 staticSwitch462 = lerpResult457;
				#else
				float4 staticSwitch462 = Normal0450;
				#endif
				float Rain_Intensity303 = _EnviroRainIntensity;
				float temp_output_325_0 = (1.0 + (( _RainFlowStrength * Rain_Intensity303 ) - 0.0) * (-1.0 - 1.0) / (1.0 - 0.0));
				float temp_output_306_0 = ( _TimeParameters.x * 0.05 );
				float4 transform287 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord10.xyz , 0.0 ));
				float2 appendResult298 = (float2(( transform287.z * 0.7 ) , ( transform287.y * 0.2 )));
				float2 panner313 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult298 * _RainFlowTiling ));
				float2 texCoord285 = input.ase_texcoord9.xy * float2( 10,10 ) + float2( 0,0 );
				float gradientNoise289 = UnityGradientNoise(texCoord285,_RainFlowDistortionScale);
				gradientNoise289 = gradientNoise289*0.5 + 0.5;
				float Distortion307 = ( gradientNoise289 * _RainFlowDistortionStrenght );
				float simpleNoise324 = SimpleNoise( ( panner313 + Distortion307 )*100.0 );
				simpleNoise324 = simpleNoise324*2 - 1;
				float smoothstepResult332 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise324);
				float temp_output_335_0 = ( ( ( WorldNormal.y - 0.95 ) * -1.0 ) * _RainFlowIntensity );
				float3 temp_cast_99 = (0.3).xxx;
				float3 break337 = ( abs( WorldNormal ) - temp_cast_99 );
				float lerpResult342 = lerp( 0.0 , ( smoothstepResult332 * temp_output_335_0 ) , break337.x);
				float4 transform286 = mul(GetObjectToWorldMatrix(),float4( input.ase_texcoord10.xyz , 0.0 ));
				float2 appendResult299 = (float2(( transform286.x * 0.7 ) , ( transform286.y * 0.2 )));
				float2 panner312 = ( temp_output_306_0 * float2( 0,1 ) + ( appendResult299 * _RainFlowTiling ));
				float simpleNoise328 = SimpleNoise( ( panner312 + Distortion307 )*100.0 );
				simpleNoise328 = simpleNoise328*2 - 1;
				float smoothstepResult333 = smoothstep( temp_output_325_0 , 1.0 , simpleNoise328);
				float lerpResult341 = lerp( 0.0 , ( smoothstepResult333 * temp_output_335_0 ) , break337.z);
				float Rain_Distance_Fade340 = ( 1.0 - sqrt( saturate( ( distance( WorldPosition , _WorldSpaceCameraPos ) / _RainDistanceFade ) ) ) );
				float temp_output_366_0 = saturate( ( ( lerpResult342 + lerpResult341 ) * Rain_Distance_Fade340 ) );
				float temp_output_373_0 = ddx( temp_output_366_0 );
				float temp_output_384_0 = ddy( temp_output_366_0 );
				float3 appendResult445 = (float3(temp_output_373_0 , temp_output_384_0 , sqrt( ( ( 1.0 - ( temp_output_373_0 * temp_output_373_0 ) ) - ( temp_output_384_0 * temp_output_384_0 ) ) )));
				float3 normalizeResult449 = normalize( appendResult445 );
				float3 RainFlow453 = normalizeResult449;
				float localRainRipples1_g1054 = ( 0.0 );
				float2 appendResult426 = (float2(WorldPosition.x , WorldPosition.z));
				float2 UV1_g1054 = ( appendResult426 * _RainDropTiling );
				float AngleOffset1_g1054 = 5.0;
				float lerpResult428 = lerp( 64.0 , 12.0 , Puddle_Mask264);
				float CellDensity1_g1054 = round( lerpResult428 );
				float Time1_g1054 = ( _TimeParameters.x * _RainDropSpeed );
				float temp_output_358_0 = ( _RainDropIntensity * 1.5 );
				float lerpResult365 = lerp( _RainDropIntensity , temp_output_358_0 , Puddle_Mask264);
				#ifdef _PUDDLES_ON
				float staticSwitch372 = lerpResult365;
				#else
				float staticSwitch372 = temp_output_358_0;
				#endif
				float switchResult422 = (((ase_vface>0)?(( ( ( WorldNormal.y - 0.7 ) * ( staticSwitch372 * Rain_Intensity303 ) ) * Rain_Distance_Fade340 )):(0.0)));
				float Strength1_g1054 = max( 0.0 , switchResult422 );
				float3 normal1_g1054 = float3( 0,0,0 );
				float Out1_g1054 = 0.0;
				float lerpResult440 = lerp( 5.0 , 8.0 , Puddle_Mask264);
				float pow1_g1054 = lerpResult440;
				float lerpResult439 = lerp( 1.0 , 0.0 , Puddle_Mask264);
				float sin1_g1054 = lerpResult439;
				{
				Rain(UV1_g1054,AngleOffset1_g1054,CellDensity1_g1054,Time1_g1054,Strength1_g1054,pow1_g1054,sin1_g1054,Out1_g1054,normal1_g1054);
				}
				float3 Rain_Drop452 = normal1_g1054;
				#ifdef _RAIN_ON
				float4 staticSwitch468 = float4( BlendNormal( staticSwitch462.xyz , BlendNormal( RainFlow453 , Rain_Drop452 ) ) , 0.0 );
				#else
				float4 staticSwitch468 = staticSwitch462;
				#endif
				float2 temp_output_5_0_g1055 = texCoord232;
				float localStochasticTiling2_g1056 = ( 0.0 );
				float2 Input_UV145_g1056 = temp_output_5_0_g1055;
				float2 UV2_g1056 = Input_UV145_g1056;
				float2 UV12_g1056 = float2( 0,0 );
				float2 UV22_g1056 = float2( 0,0 );
				float2 UV32_g1056 = float2( 0,0 );
				float W12_g1056 = 0.0;
				float W22_g1056 = 0.0;
				float W32_g1056 = 0.0;
				StochasticTiling( UV2_g1056 , UV12_g1056 , UV22_g1056 , UV32_g1056 , W12_g1056 , W22_g1056 , W32_g1056 );
				float2 temp_output_10_0_g1056 = ddx( Input_UV145_g1056 );
				float2 temp_output_12_0_g1056 = ddy( Input_UV145_g1056 );
				float4 Output_2D293_g1056 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV12_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W12_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV22_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W22_g1056 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowNormal, sampler_SnowNormal, UV32_g1056, temp_output_10_0_g1056, temp_output_12_0_g1056 ) * W32_g1056 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1055 = Output_2D293_g1056;
				#else
				float4 staticSwitch7_g1055 = SAMPLE_TEXTURE2D( _SnowNormal, sampler_SnowNormal, temp_output_5_0_g1055 );
				#endif
				float3 unpack463 = UnpackNormalScale( staticSwitch7_g1055, _SnowNormalScale );
				unpack463.z = lerp( 1, unpack463.z, saturate(_SnowNormalScale) );
				float3 Snow_Normal465 = unpack463;
				float2 appendResult202 = (float2(WorldPosition.x , WorldPosition.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( WorldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord10.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.ase_texcoord10.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float4 lerpResult470 = lerp( staticSwitch468 , float4( Snow_Normal465 , 0.0 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch471 = lerpResult470;
				#else
				float4 staticSwitch471 = staticSwitch468;
				#endif
				float4 Normal_Final472 = staticSwitch471;
				float3x3 ase_tangentToWorldFast = float3x3(WorldTangent.x,WorldBiTangent.x,WorldNormal.x,WorldTangent.y,WorldBiTangent.y,WorldNormal.y,WorldTangent.z,WorldBiTangent.z,WorldNormal.z);
				float3 tangentToWorldDir474 = mul( ase_tangentToWorldFast, Normal_Final472.xyz );
				float dotResult497 = dot( WorldViewDirection , -( _MainLightPosition.xyz + ( tangentToWorldDir474 * _SSSDistortion ) ) );
				float dotResult504 = dot( dotResult497 , _SSSScale );
				float SSS523 = ( saturate( dotResult504 ) * _SSSIntensity );
				float4 lerpResult553 = lerp( staticSwitch543 , ( Snow_Albedo522 + SSS523 ) , Snow_Blending247);
				#ifdef _SNOW_ON
				float4 staticSwitch562 = lerpResult553;
				#else
				float4 staticSwitch562 = staticSwitch543;
				#endif
				float4 Albedo_Final575 = ( staticSwitch562 + ( Wetness228 * -0.02 ) );
				float4 localClipHoles583 = ( Albedo_Final575 );
				float2 uv_TerrainHolesTexture = input.ase_texcoord9.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Hole583 = holeClipValue579;
				{
				clip(Hole583 == 0.0f ? -1 : 1);
				}
				float4 AlbedoCombined586 = localClipHoles583;
				
				float4 break668 = Normal_Final472;
				float3 appendResult671 = (float3(break668.x , break668.y , ( break668.z + 0.001 )));
				#ifdef _TERRAIN_INSTANCED_PERPIXEL_NORMAL
				float3 staticSwitch665 = appendResult671;
				#else
				float3 staticSwitch665 = appendResult671;
				#endif
				
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Metallic0569 = break245.x;
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float2 temp_output_10_0_g1044 = ddx( Input_UV145_g1044 );
				float2 temp_output_12_0_g1044 = ddy( Input_UV145_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_GRAD( _SnowMask, sampler_SnowMask, UV12_g1044, temp_output_10_0_g1044, temp_output_12_0_g1044 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowMask, sampler_SnowMask, UV22_g1044, temp_output_10_0_g1044, temp_output_12_0_g1044 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_GRAD( _SnowMask, sampler_SnowMask, UV32_g1044, temp_output_10_0_g1044, temp_output_12_0_g1044 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Metallic563 = ( break244.r + _SnowMetallic );
				float lerpResult577 = lerp( Metallic0569 , Snow_Metallic563 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch581 = lerpResult577;
				#else
				float staticSwitch581 = Metallic0569;
				#endif
				float Metallic_Final584 = staticSwitch581;
				
				float Smoothness0540 = break245.w;
				float Snow_Smoothness536 = ( break244.a * _SnowSmoothness );
				float lerpResult559 = lerp( Smoothness0540 , Snow_Smoothness536 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch564 = lerpResult559;
				#else
				float staticSwitch564 = Smoothness0540;
				#endif
				#ifdef _RAIN_ON
				float staticSwitch544 = ( Out1_g1054 * 0.2 );
				#else
				float staticSwitch544 = 0.0;
				#endif
				float RainDropSmoothness555 = staticSwitch544;
				#ifdef _RAIN_ON
				float staticSwitch545 = ( temp_output_366_0 * _RainFlowSmoothnessBoost );
				#else
				float staticSwitch545 = 0.0;
				#endif
				float RainFlowSmoothness557 = staticSwitch545;
				float Smoothness_Final585 = saturate( ( ( staticSwitch564 + ( ( ( _WetnessBoost * Wetness228 ) + saturate( ( Puddle_Mask264 - 0.2 ) ) ) * ( 1.0 - Snow_Blending247 ) ) ) + ( RainDropSmoothness555 + RainFlowSmoothness557 ) ) );
				
				float Occlusion0589 = break245.y;
				float Snow_Occlusion588 = break244.g;
				float lerpResult593 = lerp( Occlusion0589 , Snow_Occlusion588 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch594 = lerpResult593;
				#else
				float staticSwitch594 = Occlusion0589;
				#endif
				float Occlusion_Final595 = staticSwitch594;
				
				float Alpha1008 = holeClipValue579;
				

				float3 BaseColor = AlbedoCombined586.xyz;
				float3 Normal = staticSwitch665;
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = Metallic_Final584;
				float Smoothness = Smoothness_Final585;
				float Occlusion = Occlusion_Final595;
				float Alpha = Alpha1008;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.positionCS = input.positionCS;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
						inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
						inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
						inputData.normalWS = Normal;
					#endif
				#else
					inputData.normalWS = WorldNormal;
				#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = SafeNormalize( WorldViewDirection );

				inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI( SH, GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask );
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
						#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				#ifdef _DBUFFER
					ApplyDecal(input.positionCS,
						BaseColor,
						Specular,
						inputData.normalWS,
						Metallic,
						Occlusion,
						Smoothness);
				#endif

				BRDFData brdfData;
				InitializeBRDFData
				(BaseColor, Metallic, Specular, Smoothness, Alpha, brdfData);

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
				half4 color;
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
				color.rgb = GlobalIllumination(brdfData, inputData.bakedGI, Occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb, Occlusion);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SCENESELECTIONPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#include "EnviroInclude.hlsl"


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );

				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 uv_TerrainHolesTexture = input.ase_texcoord.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Alpha1008 = holeClipValue579;
				

				surfaceDescription.Alpha = Alpha1008;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_TESSELLATION 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 170003
			#define ASE_USING_SAMPLING_MACROS 1


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(_ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

		    #define SCENEPICKINGPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_local _SNOW_ON
			#pragma multi_compile_local _HEIGHTBLEND_ON
			#pragma multi_compile_local _SPLATCOUNT__4 _SPLATCOUNT__8 _SPLATCOUNT__12
			#pragma multi_compile_local _QUALITY_FAST _QUALITY_BALANCE _QUALITY_QUALITY
			#pragma multi_compile_local _STOCHASTIC_ON
			#pragma multi_compile_local _PUDDLES_ON
			#include "EnviroInclude.hlsl"


			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DisplacementMod1;
			float4 _Metallic02;
			float4 _Occlusion0;
			float4 _Occlusion1;
			float4 _Occlusion2;
			float4 _DisplacementMod0;
			float4 _ColorTint10;
			float4 _DisplacementMod2;
			float4 _Smoothness00;
			float4 _Smoothness01;
			float4 _Smoothness02;
			float4 _HeightContrast0;
			float4 _HeightContrast1;
			float4 _HeightContrast2;
			float4 _ColorTint9;
			float4 _ColorTint8;
			float4 _ColorTint7;
			float4 _ColorTint6;
			float4 _ColorTint5;
			float4 _ColorTint4;
			float4 _ColorTint3;
			float4 _ColorTint2;
			float4 _Metallic01;
			float4 _Metallic00;
			float4 _LayerScaleOffset11;
			float4 _LayerScaleOffset10;
			float4 _NormalScale02;
			float4 _NormalScale01;
			float4 _TerrainHolesTexture_ST;
			float4 _NormalScale00;
			float4 _PuddleColor;
			float4 _ColorTint11;
			float4 _Control_ST;
			float4 _Control1_ST;
			float4 _Control2_ST;
			float4 _SamplingType0;
			float4 _ColorTint0;
			float4 _SamplingType1;
			float4 _LayerScaleOffset0;
			float4 _LayerScaleOffset1;
			float4 _LayerScaleOffset2;
			float4 _LayerScaleOffset3;
			float4 _LayerScaleOffset4;
			float4 _LayerScaleOffset5;
			float4 _LayerScaleOffset6;
			float4 _LayerScaleOffset7;
			float4 _LayerScaleOffset8;
			float4 _LayerScaleOffset9;
			float4 _SamplingType2;
			float4 _ColorTint1;
			float _TessellationMaxDistance;
			float _RainFlowDistortionStrenght;
			float _RainFlowIntensity;
			float _RainDistanceFade;
			float _RainDropTiling;
			float _RainDropSpeed;
			float _RainDropIntensity;
			float _SnowNormalScale;
			float _RainFlowTiling;
			float _SSSDistortion;
			float _SSSScale;
			float _SSSIntensity;
			float _SnowMetallic;
			float _SnowSmoothness;
			float _RainFlowDistortionScale;
			float _EnviroRainIntensity;
			float _SnowTiling;
			float _PuddleWaveIntensity;
			float _TessellationMinDistance;
			float _TessellationFactor;
			float _SnowDisplacement;
			float _EnviroSnow;
			float _SnowSlopePower;
			float _SnowHeightBlending;
			float _HeightBlendStrength;
			float _HeightBlending;
			float _WetnessBoost;
			float _DisplacementStrength;
			float _PuddleIntensity;
			float _PuddleCoverageNoise;
			float _EnviroWetness;
			float _MipDistanceBlending;
			float _PuddleWaveTiling;
			float _RainFlowStrength;
			float _RainFlowSmoothnessBoost;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			TEXTURE2D(_Control);
			SAMPLER(sampler_Control);
			TEXTURE2D(_Control1);
			SAMPLER(sampler_Control1);
			TEXTURE2D(_Control2);
			SAMPLER(sampler_Control2);
			TEXTURE2D_ARRAY(_MaskArray);
			SAMPLER(sampler_MaskArray);
			TEXTURE2D(_SnowMask);
			SAMPLER(sampler_SnowMask);
			TEXTURE2D(_TerrainHolesTexture);
			SAMPLER(sampler_TerrainHolesTexture);


			void GetSplats( float4 in0, float4 in1, float4 in2, out float4 Out1, out float4 Out0 )
			{
				GetSplatsWeights(in0,in1,in2,Out0,Out1);
			}
			
			void GetUVS( float4 in0, float4 in1, float4 in2, float4 in3, float4 in4, float4 in5, float4 in6, float4 in7, float4 in8, float4 in9, float4 in10, float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3 )
			{
				GetLayerUV(in0,in1,in2,in3,in4,in5,in6,in7,in8,in9,in10,in11,index,Out0,Out1,Out2,Out3);
			}
			
			void GetLayerSettings( float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0 )
			{
				GetLayerValue(in0,in1,in2,index,Out0);
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
			{
				float2 vertex1, vertex2, vertex3;
				// Scaling of the input
				float2 uv = UV * 3.464; // 2 * sqrt (3)
				// Skew input space into simplex triangle grid
				const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
				float2 skewedCoord = mul( gridToSkewedGrid, uv );
				// Compute local triangle vertex IDs and local barycentric coordinates
				int2 baseId = int2( floor( skewedCoord ) );
				float3 temp = float3( frac( skewedCoord ), 0 );
				temp.z = 1.0 - temp.x - temp.y;
				if ( temp.z > 0.0 )
				{
					W1 = temp.z;
					W2 = temp.y;
					W3 = temp.x;
					vertex1 = baseId;
					vertex2 = baseId + int2( 0, 1 );
					vertex3 = baseId + int2( 1, 0 );
				}
				else
				{
					W1 = -temp.z;
					W2 = 1.0 - temp.y;
					W3 = 1.0 - temp.x;
					vertex1 = baseId + int2( 1, 1 );
					vertex2 = baseId + int2( 1, 0 );
					vertex3 = baseId + int2( 0, 1 );
				}
				UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
				UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
				UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
				return;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 appendResult261 = (float3(_SnowDisplacement , _SnowDisplacement , _SnowDisplacement));
				float3 ase_worldNormal = TransformObjectToWorldNormal(input.normalOS);
				float3 ase_worldPos = TransformObjectToWorld( (input.positionOS).xyz );
				float2 appendResult202 = (float2(ase_worldPos.x , ase_worldPos.z));
				float simpleNoise216 = SimpleNoise( appendResult202*10.0 );
				float Snow_Amount174 = _EnviroSnow;
				float clampResult215 = clamp( Snow_Amount174 , 0.0 , 1.0 );
				float lerpResult231 = lerp( 0.0 , simpleNoise216 , clampResult215);
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float HeightMask238 = saturate(pow(((lerpResult231*( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) ))*4)+(( ( 2.0 * pow( normalizedWorldNormal.y , _SnowSlopePower ) ) * saturate( ( sqrt( ( input.positionOS.xyz.y * 0.1 ) ) * Snow_Amount174 ) ) )*2),_SnowHeightBlending));
				float Snow_Blending247 = saturate( HeightMask238 );
				float3 Snow_Displacement272 = ( ( appendResult261 * ase_worldNormal ) * ( Snow_Blending247 * Snow_Amount174 ) );
				#ifdef _SNOW_ON
				float3 staticSwitch279 = Snow_Displacement272;
				#else
				float3 staticSwitch279 = float3(0,0,0);
				#endif
				float localGetSplats43 = ( 0.0 );
				float2 uv_Control = input.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 SplatControl033 = SAMPLE_TEXTURE2D_LOD( _Control, sampler_Control, uv_Control, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch42 = SplatControl033;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch42 = SplatControl033;
				#else
				float4 staticSwitch42 = SplatControl033;
				#endif
				float4 in043 = staticSwitch42;
				float4 _Vector1 = float4(0,0,0,0);
				float2 uv_Control1 = input.ase_texcoord.xy * _Control1_ST.xy + _Control1_ST.zw;
				float4 SplatControl135 = SAMPLE_TEXTURE2D_LOD( _Control1, sampler_Control1, uv_Control1, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch41 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch41 = SplatControl135;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch41 = SplatControl135;
				#else
				float4 staticSwitch41 = _Vector1;
				#endif
				float4 in143 = staticSwitch41;
				float2 uv_Control2 = input.ase_texcoord.xy * _Control2_ST.xy + _Control2_ST.zw;
				float4 SplatControl234 = SAMPLE_TEXTURE2D_LOD( _Control2, sampler_Control2, uv_Control2, 0.0 );
				#if defined( _SPLATCOUNT__4 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__8 )
				float4 staticSwitch40 = _Vector1;
				#elif defined( _SPLATCOUNT__12 )
				float4 staticSwitch40 = SplatControl234;
				#else
				float4 staticSwitch40 = _Vector1;
				#endif
				float4 in243 = staticSwitch40;
				float4 Out143 = float4( 0,0,0,0 );
				float4 Out043 = float4( 0,0,0,0 );
				{
				GetSplatsWeights(in043,in143,in243,Out043,Out143);
				}
				float4 SplatWeights198 = Out143;
				float4 temp_output_14_0_g1042 = SplatWeights198;
				float localGetLayerSettings894 = ( 0.0 );
				float4 in0894 = _SamplingType0;
				float4 in1894 = _SamplingType1;
				float4 in2894 = _SamplingType2;
				float4 SplatIndex44 = Out043;
				float4 index894 = SplatIndex44;
				float4 Out0894 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0894,in1894,in2894,index894,Out0894);
				}
				float4 samplingType895 = Out0894;
				float4 break899 = samplingType895;
				float2 appendResult69 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float localGetUVS58 = ( 0.0 );
				float4 in058 = _LayerScaleOffset0;
				float4 in158 = _LayerScaleOffset1;
				float4 in258 = _LayerScaleOffset2;
				float4 in358 = _LayerScaleOffset3;
				float4 in458 = _LayerScaleOffset4;
				float4 in558 = _LayerScaleOffset5;
				float4 in658 = _LayerScaleOffset6;
				float4 in758 = _LayerScaleOffset7;
				float4 in858 = _LayerScaleOffset8;
				float4 in958 = _LayerScaleOffset9;
				float4 in1058 = _LayerScaleOffset10;
				float4 in1158 = _LayerScaleOffset11;
				float4 index58 = SplatIndex44;
				float4 Out058 = float4( 0,0,0,0 );
				float4 Out158 = float4( 0,0,0,0 );
				float4 Out258 = float4( 0,0,0,0 );
				float4 Out358 = float4( 0,0,0,0 );
				{
				GetLayerUV(in058,in158,in258,in358,in458,in558,in658,in758,in858,in958,in1058,in1158,index58,Out058,Out158,Out258,Out358);
				}
				float4 break63 = Out058;
				float2 appendResult65 = (float2(break63.x , break63.y));
				float2 appendResult73 = (float2(break63.z , break63.w));
				float4 break86 = SplatIndex44;
				float3 appendResult93 = (float3(( ( appendResult69 * appendResult65 ) + appendResult73 ) , break86.x));
				float3 UV0100 = appendResult93;
				float2 temp_output_5_0_g11 = UV0100.xy;
				float4 break102 = SplatIndex44;
				int temp_output_4_0_g11 = (int)break102.x;
				float4 tex2DArrayNode3_g11 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g11,(float)temp_output_4_0_g11, 1.0 );
				float localStochasticTiling190_g12 = ( 0.0 );
				float2 Input_UV317_g12 = temp_output_5_0_g11;
				float2 UV190_g12 = Input_UV317_g12;
				float2 UV1190_g12 = float2( 0,0 );
				float2 UV2190_g12 = float2( 0,0 );
				float2 UV3190_g12 = float2( 0,0 );
				float W1190_g12 = 0.0;
				float W2190_g12 = 0.0;
				float W3190_g12 = 0.0;
				StochasticTiling( UV190_g12 , UV1190_g12 , UV2190_g12 , UV3190_g12 , W1190_g12 , W2190_g12 , W3190_g12 );
				float Input_Index330_g12 = (float)temp_output_4_0_g11;
				float4 Output_2DArray152_g12 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g12,Input_Index330_g12, 0.0 ) * W1190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g12,Input_Index330_g12, 0.0 ) * W2190_g12 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g12,Input_Index330_g12, 0.0 ) * W3190_g12 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g11 = Output_2DArray152_g12;
				#else
				float4 staticSwitch7_g11 = tex2DArrayNode3_g11;
				#endif
				float4 ifLocalVar17_g11 = 0;
				UNITY_BRANCH 
				if( break899.x > 0.0 )
				ifLocalVar17_g11 = staticSwitch7_g11;
				else if( break899.x == 0.0 )
				ifLocalVar17_g11 = tex2DArrayNode3_g11;
				float4 break116 = ifLocalVar17_g11;
				float localGetLayerSettings163 = ( 0.0 );
				float4 in0163 = _Metallic00;
				float4 in1163 = _Metallic01;
				float4 in2163 = _Metallic02;
				float4 index163 = SplatIndex44;
				float4 Out0163 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0163,in1163,in2163,index163,Out0163);
				}
				float4 Metallic167 = Out0163;
				float4 break177 = Metallic167;
				float localGetLayerSettings728 = ( 0.0 );
				float4 in0728 = _Occlusion0;
				float4 in1728 = _Occlusion1;
				float4 in2728 = _Occlusion2;
				float4 index728 = SplatIndex44;
				float4 Out0728 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0728,in1728,in2728,index728,Out0728);
				}
				float4 Occlusion729 = Out0728;
				float4 break762 = Occlusion729;
				float HeightMap0119 = break116.b;
				float localGetLayerSettings977 = ( 0.0 );
				float4 in0977 = _DisplacementMod0;
				float4 in1977 = _DisplacementMod1;
				float4 in2977 = _DisplacementMod2;
				float4 index977 = SplatIndex44;
				float4 Out0977 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0977,in1977,in2977,index977,Out0977);
				}
				float4 displacementModifier978 = Out0977;
				float4 break982 = displacementModifier978;
				float localGetLayerSettings162 = ( 0.0 );
				float4 in0162 = _Smoothness00;
				float4 in1162 = _Smoothness01;
				float4 in2162 = _Smoothness02;
				float4 index162 = SplatIndex44;
				float4 Out0162 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0162,in1162,in2162,index162,Out0162);
				}
				float4 Smoothness166 = Out0162;
				float4 break178 = Smoothness166;
				float4 appendResult205 = (float4(( break116.r + break177.x ) , ( break116.g + break762.x ) , ( HeightMap0119 * break982.x ) , ( break116.a * break178.x )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch221 = appendResult205;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch221 = appendResult205;
				#else
				float4 staticSwitch221 = appendResult205;
				#endif
				float4 temp_output_18_0_g1042 = staticSwitch221;
				float4 break62 = Out158;
				float2 appendResult66 = (float2(break62.x , break62.y));
				float2 appendResult72 = (float2(break62.z , break62.w));
				float3 appendResult90 = (float3(( ( appendResult69 * appendResult66 ) + appendResult72 ) , break86.y));
				float3 UV197 = appendResult90;
				float2 temp_output_5_0_g9 = UV197.xy;
				int temp_output_4_0_g9 = (int)break102.y;
				float4 tex2DArrayNode3_g9 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g9,(float)temp_output_4_0_g9, 1.0 );
				float localStochasticTiling190_g10 = ( 0.0 );
				float2 Input_UV317_g10 = temp_output_5_0_g9;
				float2 UV190_g10 = Input_UV317_g10;
				float2 UV1190_g10 = float2( 0,0 );
				float2 UV2190_g10 = float2( 0,0 );
				float2 UV3190_g10 = float2( 0,0 );
				float W1190_g10 = 0.0;
				float W2190_g10 = 0.0;
				float W3190_g10 = 0.0;
				StochasticTiling( UV190_g10 , UV1190_g10 , UV2190_g10 , UV3190_g10 , W1190_g10 , W2190_g10 , W3190_g10 );
				float Input_Index330_g10 = (float)temp_output_4_0_g9;
				float4 Output_2DArray152_g10 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g10,Input_Index330_g10, 0.0 ) * W1190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g10,Input_Index330_g10, 0.0 ) * W2190_g10 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g10,Input_Index330_g10, 0.0 ) * W3190_g10 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g9 = Output_2DArray152_g10;
				#else
				float4 staticSwitch7_g9 = tex2DArrayNode3_g9;
				#endif
				float4 ifLocalVar17_g9 = 0;
				UNITY_BRANCH 
				if( break899.y > 0.0 )
				ifLocalVar17_g9 = staticSwitch7_g9;
				else if( break899.y == 0.0 )
				ifLocalVar17_g9 = tex2DArrayNode3_g9;
				float4 break115 = ifLocalVar17_g9;
				float HeightMap1118 = break115.b;
				float4 appendResult206 = (float4(( break177.y + break115.r ) , ( break115.g + break762.y ) , ( HeightMap1118 * break982.y ) , ( break115.a * break178.y )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch222 = appendResult206;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch222 = appendResult206;
				#else
				float4 staticSwitch222 = appendResult206;
				#endif
				float4 temp_output_22_0_g1042 = staticSwitch222;
				float4 _Vector4 = float4(0,0,0,0);
				float4 break60 = Out258;
				float2 appendResult67 = (float2(break60.x , break60.y));
				float2 appendResult79 = (float2(break60.z , break60.w));
				float3 appendResult91 = (float3(( ( appendResult69 * appendResult67 ) + appendResult79 ) , break86.z));
				float3 UV298 = appendResult91;
				float2 temp_output_5_0_g7 = UV298.xy;
				int temp_output_4_0_g7 = (int)break102.z;
				float4 tex2DArrayNode3_g7 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g7,(float)temp_output_4_0_g7, 1.0 );
				float localStochasticTiling190_g8 = ( 0.0 );
				float2 Input_UV317_g8 = temp_output_5_0_g7;
				float2 UV190_g8 = Input_UV317_g8;
				float2 UV1190_g8 = float2( 0,0 );
				float2 UV2190_g8 = float2( 0,0 );
				float2 UV3190_g8 = float2( 0,0 );
				float W1190_g8 = 0.0;
				float W2190_g8 = 0.0;
				float W3190_g8 = 0.0;
				StochasticTiling( UV190_g8 , UV1190_g8 , UV2190_g8 , UV3190_g8 , W1190_g8 , W2190_g8 , W3190_g8 );
				float Input_Index330_g8 = (float)temp_output_4_0_g7;
				float4 Output_2DArray152_g8 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g8,Input_Index330_g8, 0.0 ) * W1190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g8,Input_Index330_g8, 0.0 ) * W2190_g8 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g8,Input_Index330_g8, 0.0 ) * W3190_g8 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g7 = Output_2DArray152_g8;
				#else
				float4 staticSwitch7_g7 = tex2DArrayNode3_g7;
				#endif
				float4 ifLocalVar17_g7 = 0;
				UNITY_BRANCH 
				if( break899.z > 0.0 )
				ifLocalVar17_g7 = staticSwitch7_g7;
				else if( break899.z == 0.0 )
				ifLocalVar17_g7 = tex2DArrayNode3_g7;
				float4 break114 = ifLocalVar17_g7;
				float HeightMap2120 = break114.b;
				float4 appendResult207 = (float4(( break177.z + break114.r ) , ( break114.g + break762.z ) , ( HeightMap2120 * break982.z ) , ( break114.a * break178.z )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch223 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch223 = appendResult207;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch223 = appendResult207;
				#else
				float4 staticSwitch223 = _Vector4;
				#endif
				float4 temp_output_23_0_g1042 = staticSwitch223;
				float4 break61 = Out358;
				float2 appendResult68 = (float2(break61.x , break61.y));
				float2 appendResult78 = (float2(break61.z , break61.w));
				float3 appendResult92 = (float3(( ( appendResult69 * appendResult68 ) + appendResult78 ) , break86.w));
				float3 UV399 = appendResult92;
				float2 temp_output_5_0_g1 = UV399.xy;
				int temp_output_4_0_g1 = (int)break102.w;
				float4 tex2DArrayNode3_g1 = SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, temp_output_5_0_g1,(float)temp_output_4_0_g1, 1.0 );
				float localStochasticTiling190_g6 = ( 0.0 );
				float2 Input_UV317_g6 = temp_output_5_0_g1;
				float2 UV190_g6 = Input_UV317_g6;
				float2 UV1190_g6 = float2( 0,0 );
				float2 UV2190_g6 = float2( 0,0 );
				float2 UV3190_g6 = float2( 0,0 );
				float W1190_g6 = 0.0;
				float W2190_g6 = 0.0;
				float W3190_g6 = 0.0;
				StochasticTiling( UV190_g6 , UV1190_g6 , UV2190_g6 , UV3190_g6 , W1190_g6 , W2190_g6 , W3190_g6 );
				float Input_Index330_g6 = (float)temp_output_4_0_g1;
				float4 Output_2DArray152_g6 = ( ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV1190_g6,Input_Index330_g6, 0.0 ) * W1190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV2190_g6,Input_Index330_g6, 0.0 ) * W2190_g6 ) + ( SAMPLE_TEXTURE2D_ARRAY_LOD( _MaskArray, sampler_MaskArray, UV3190_g6,Input_Index330_g6, 0.0 ) * W3190_g6 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1 = Output_2DArray152_g6;
				#else
				float4 staticSwitch7_g1 = tex2DArrayNode3_g1;
				#endif
				float4 ifLocalVar17_g1 = 0;
				UNITY_BRANCH 
				if( break899.w > 0.0 )
				ifLocalVar17_g1 = staticSwitch7_g1;
				else if( break899.w == 0.0 )
				ifLocalVar17_g1 = tex2DArrayNode3_g1;
				float4 break113 = ifLocalVar17_g1;
				float HeightMap3117 = break113.b;
				float4 appendResult208 = (float4(( break177.w + break113.r ) , ( break113.g + break762.w ) , ( HeightMap3117 * break982.w ) , ( break113.a * break178.w )));
				#if defined( _QUALITY_FAST )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_BALANCE )
				float4 staticSwitch224 = _Vector4;
				#elif defined( _QUALITY_QUALITY )
				float4 staticSwitch224 = appendResult208;
				#else
				float4 staticSwitch224 = _Vector4;
				#endif
				float4 temp_output_24_0_g1042 = staticSwitch224;
				float4 weightedBlendVar30_g1042 = temp_output_14_0_g1042;
				float4 weightedBlend30_g1042 = ( weightedBlendVar30_g1042.x*temp_output_18_0_g1042 + weightedBlendVar30_g1042.y*temp_output_22_0_g1042 + weightedBlendVar30_g1042.z*temp_output_23_0_g1042 + weightedBlendVar30_g1042.w*temp_output_24_0_g1042 );
				float localGetLayerSettings820 = ( 0.0 );
				float4 in0820 = _HeightContrast0;
				float4 in1820 = _HeightContrast1;
				float4 in2820 = _HeightContrast2;
				float4 index820 = SplatIndex44;
				float4 Out0820 = float4( 0,0,0,0 );
				{
				GetLayerValue(in0820,in1820,in2820,index820,Out0820);
				}
				float4 HeightContrast824 = Out0820;
				float4 break834 = HeightContrast824;
				float temp_output_846_0 = ( HeightMap0119 * break834.x );
				#if defined( _QUALITY_FAST )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch158 = temp_output_846_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch158 = temp_output_846_0;
				#else
				float staticSwitch158 = temp_output_846_0;
				#endif
				float temp_output_847_0 = ( HeightMap1118 * break834.y );
				#if defined( _QUALITY_FAST )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch159 = temp_output_847_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch159 = temp_output_847_0;
				#else
				float staticSwitch159 = temp_output_847_0;
				#endif
				float temp_output_848_0 = ( HeightMap2120 * break834.z );
				#if defined( _QUALITY_FAST )
				float staticSwitch161 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch161 = temp_output_848_0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch161 = temp_output_848_0;
				#else
				float staticSwitch161 = 0.0;
				#endif
				#if defined( _QUALITY_FAST )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_BALANCE )
				float staticSwitch160 = 0.0;
				#elif defined( _QUALITY_QUALITY )
				float staticSwitch160 = ( HeightMap3117 * break834.w );
				#else
				float staticSwitch160 = 0.0;
				#endif
				float4 appendResult164 = (float4(staticSwitch158 , staticSwitch159 , staticSwitch161 , staticSwitch160));
				float4 HeightRawCombined0199 = saturate( pow( appendResult164 , 0.5 ) );
				float4 break13_g1042 = HeightRawCombined0199;
				float4 break15_g1042 = temp_output_14_0_g1042;
				float temp_output_53_0_g1042 = ( break13_g1042.x + break15_g1042.x );
				float temp_output_54_0_g1042 = ( break13_g1042.y + break15_g1042.y );
				float temp_output_55_0_g1042 = ( break13_g1042.z + break15_g1042.z );
				float temp_output_56_0_g1042 = ( break13_g1042.w + break15_g1042.w );
				float HeightBlending854 = _HeightBlendStrength;
				float temp_output_79_0_g1042 = ( max( max( max( temp_output_53_0_g1042 , temp_output_54_0_g1042 ) , temp_output_55_0_g1042 ) , temp_output_56_0_g1042 ) - HeightBlending854 );
				float temp_output_63_0_g1042 = max( ( temp_output_53_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_67_0_g1042 = max( ( temp_output_54_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_71_0_g1042 = max( ( temp_output_55_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float temp_output_73_0_g1042 = max( ( temp_output_56_0_g1042 - temp_output_79_0_g1042 ) , 0.0 );
				float Blending197 = _HeightBlending;
				float4 lerpResult78_g1042 = lerp( weightedBlend30_g1042 , ( ( ( temp_output_18_0_g1042 * temp_output_63_0_g1042 ) + ( temp_output_22_0_g1042 * temp_output_67_0_g1042 ) + ( temp_output_23_0_g1042 * temp_output_71_0_g1042 ) + ( temp_output_24_0_g1042 * temp_output_73_0_g1042 ) ) / ( temp_output_63_0_g1042 + temp_output_67_0_g1042 + temp_output_71_0_g1042 + temp_output_73_0_g1042 ) ) , Blending197);
				#ifdef _HEIGHTBLEND_ON
				float4 staticSwitch77_g1042 = lerpResult78_g1042;
				#else
				float4 staticSwitch77_g1042 = weightedBlend30_g1042;
				#endif
				float4 Mask0240 = staticSwitch77_g1042;
				float4 break245 = Mask0240;
				float Height0248 = break245.z;
				float2 temp_cast_20 = (_SnowTiling).xx;
				float2 texCoord232 = input.ase_texcoord.xy * temp_cast_20 + float2( 0,0 );
				float2 temp_output_5_0_g1043 = texCoord232;
				float localStochasticTiling2_g1044 = ( 0.0 );
				float2 Input_UV145_g1044 = temp_output_5_0_g1043;
				float2 UV2_g1044 = Input_UV145_g1044;
				float2 UV12_g1044 = float2( 0,0 );
				float2 UV22_g1044 = float2( 0,0 );
				float2 UV32_g1044 = float2( 0,0 );
				float W12_g1044 = 0.0;
				float W22_g1044 = 0.0;
				float W32_g1044 = 0.0;
				StochasticTiling( UV2_g1044 , UV12_g1044 , UV22_g1044 , UV32_g1044 , W12_g1044 , W22_g1044 , W32_g1044 );
				float4 Output_2D293_g1044 = ( ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV12_g1044, 0.0 ) * W12_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV22_g1044, 0.0 ) * W22_g1044 ) + ( SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, UV32_g1044, 0.0 ) * W32_g1044 ) );
				#ifdef _STOCHASTIC_ON
				float4 staticSwitch7_g1043 = Output_2D293_g1044;
				#else
				float4 staticSwitch7_g1043 = SAMPLE_TEXTURE2D_LOD( _SnowMask, sampler_SnowMask, temp_output_5_0_g1043, 0.0 );
				#endif
				float4 break244 = staticSwitch7_g1043;
				float Snow_Height249 = break244.b;
				float lerpResult257 = lerp( Height0248 , Snow_Height249 , Snow_Blending247);
				#ifdef _SNOW_ON
				float staticSwitch263 = lerpResult257;
				#else
				float staticSwitch263 = Height0248;
				#endif
				float Height_Final267 = staticSwitch263;
				float4 appendResult179 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float simpleNoise195 = SimpleNoise( appendResult179.xy*_PuddleCoverageNoise );
				simpleNoise195 = simpleNoise195*2 - 1;
				float Wetness228 = _EnviroWetness;
				#ifdef _PUDDLES_ON
				float staticSwitch258 = saturate( ( ( pow( ( ase_worldNormal.y - 0.99 ) , 0.4 ) * ( ( saturate( ( _PuddleIntensity * simpleNoise195 ) ) * saturate( ( 2.0 - Snow_Amount174 ) ) ) * Wetness228 ) ) * 8.0 ) );
				#else
				float staticSwitch258 = 0.0;
				#endif
				float Puddle_Mask264 = staticSwitch258;
				float3 DisplacementFinal282 = ( staticSwitch279 + ( input.normalOS * ( ( Height_Final267 * _DisplacementStrength ) * ( 1.0 - Puddle_Mask264 ) ) ) );
				
				output.ase_texcoord.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = DisplacementFinal282;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				output.positionCS = TransformWorldToHClip(positionWS);

				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.vertex = input.positionOS;
				output.normalOS = input.normalOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessellationFactor; float tessMin = _TessellationMinDistance; float tessMax = _TessellationMaxDistance;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].vertex, input[1].vertex, input[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float2 uv_TerrainHolesTexture = input.ase_texcoord.xy * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
				float holeClipValue579 = SAMPLE_TEXTURE2D( _TerrainHolesTexture, sampler_TerrainHolesTexture, uv_TerrainHolesTexture ).r;
				float Alpha1008 = holeClipValue579;
				

				surfaceDescription.Alpha = Alpha1008;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
						clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphLitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}