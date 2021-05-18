Shader "LimitlessGlitch/TwirlEffect_URP"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	TEXTURE2D(_MainTex);
	SAMPLER(sampler_MainTex);

	#pragma shader_feature WAVEEFFECT_ON
	uniform float speed = 43.0;
	uniform float effectRadius = .52;
	uniform float verticleDensity = 8.0;
	uniform float amount;
	float2 mouse;

	struct Attributes
	{
		float4 positionOS       : POSITION;
		float2 uv               : TEXCOORD0;
	};

	struct Varyings
	{
		float2 uv        : TEXCOORD0;
		float4 vertex : SV_POSITION;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings vert(Attributes input)
	{
		Varyings output = (Varyings)0;
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
		output.vertex = vertexInput.positionCS;
		output.uv = input.uv;
		return output;
	}

	float4 Frag(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
		float effectAngle = amount * PI * sin(_Time.x * speed);
		float2 center = float2(0.5, 0.5) + mouse;
		center = center == float2(0., 0.) ? float2(.5, .5) : center;
		float2 uv = input.uv.xy - center;
		float offsetX;
		#if WAVEEFFECT_ON
		offsetX = sin(uv.y * verticleDensity + _Time.x * speed) * amount;
		#else
		offsetX = effectAngle;
		#endif
		float angle = atan2(uv.y, uv.x) + (offsetX)*smoothstep(effectRadius, 0., length(uv * float2(_ScreenParams.x / _ScreenParams.y, 1.)));
		float radius = length(uv);
		mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(radius * cos(angle), radius * sin(angle)) + center);
		return mask; 
	}

		ENDHLSL

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
			LOD 200
			Pass
		{
			HLSLPROGRAM

				#pragma vertex vert
				#pragma fragment Frag

			ENDHLSL
		}
	}
	FallBack "Diffuse"
}