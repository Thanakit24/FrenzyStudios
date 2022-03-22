Shader "Hyperspace Teleport/Evanish" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		[HDR]_AmbientColor ("Ambient", Color) = (0.2, 0.2, 0.2, 1)
		[NoScaleOffset]_FlowTex      ("Flow (RG)", 2D) = "black" {}
		[NoScaleOffset]_DissolveTex  ("Dissolve", 2D) = "white" {}
		_Exapnd     ("Expand", Range(0.01, 0.03)) = 0.01
		_Weight     ("Weight", Range(0, 1)) = 0
		_Strength   ("Strength", Range(0.001, 0.03)) = 0.01
		_Direction  ("Direction", Vector) = (0, 0, 0, 0)
		[HDR]_Color ("Color", Color) = (1, 1, 1, 1)
		_Glow       ("Glow", Range(0, 2)) = 1
		[NoScaleOffset]_ShapeTex ("Shape", 2D) = "white" {}
		_Radius                  ("Radius", Float) = 0.2
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Lighting.cginc"

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _AmbientColor;

	sampler2D _FlowTex, _ShapeTex, _DissolveTex;
	half4 _Direction, _Color;
	half _Strength, _Exapnd, _Weight, _Glow, _Radius;

	struct v2g
	{
		float4 locpos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
	};
	struct g2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		half4 color : COLOR;
		float3 normal : NORMAL;
	};
	v2g vert (appdata_base v)
	{
		v2g o;
		o.locpos = v.vertex;
		o.uv = v.texcoord;
		o.normal = UnityObjectToWorldNormal(v.normal);
		return o;
	}
	float random (float2 uv)                                                  { return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123); }
	float remap (float value, float from1, float to1, float from2, float to2) { return (value - from1) / (to1 - from1) * (to2 - from2) + from2; }
	float3 remapFlowTexture (float4 tex) {
		return float3(
			remap(tex.x, 0, 1, -1, 1),
			remap(tex.y, 0, 1, -1, 1),
			0);
	}
	[maxvertexcount(7)]
	void geom (triangle v2g IN[3], inout TriangleStream<g2f> triStream)
	{
		float2 avgUV = (IN[0].uv + IN[1].uv + IN[2].uv) / 3.0;
		float3 avgPos = (IN[0].locpos + IN[1].locpos + IN[2].locpos) / 3.0;
		float3 avgNormal = (IN[0].normal + IN[1].normal + IN[2].normal) / 3.0;

		float dsv = tex2Dlod(_DissolveTex, float4(avgUV, 0, 0)).r;
		float t = clamp(_Weight * 2 - dsv, 0, 1);

		float2 flowUV = mul(unity_ObjectToWorld, avgPos).xz;
		float3 flowVector = remapFlowTexture(tex2Dlod(_FlowTex, float4(flowUV, 0, 0)));

		float3 rndpos = avgPos + normalize(_Direction) * _Strength;
		rndpos += (flowVector.xyz * _Exapnd);

		float3 p = lerp(avgPos, rndpos, t);
		float radius = lerp(_Radius, 0, t);

		if (t > 0)
		{
			float3 right = UNITY_MATRIX_IT_MV[0].xyz;
			float3 up = UNITY_MATRIX_IT_MV[1].xyz;

			float halfS = 0.5 * radius;

			float4 v[4];
			v[0] = float4(p + halfS * right - halfS * up, 1.0);
			v[1] = float4(p + halfS * right + halfS * up, 1.0);
			v[2] = float4(p - halfS * right - halfS * up, 1.0);
			v[3] = float4(p - halfS * right + halfS * up, 1.0);

			g2f o;
			o.pos = UnityObjectToClipPos(v[0]);
			o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, float2(1.0, 0.0));
			o.color = 1.0;
			o.normal = avgNormal;
			triStream.Append(o);

			o.pos = UnityObjectToClipPos(v[1]);
			o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, float2(1.0, 1.0));
			o.color = 1.0;
			o.normal = avgNormal;
			triStream.Append(o);

			o.pos =UnityObjectToClipPos(v[2]);
			o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, float2(0.0, 0.0));
			o.color = 1.0;
			o.normal = avgNormal;
			triStream.Append(o);

			o.pos = UnityObjectToClipPos(v[3]);
			o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, float2(0.0, 1.0));
			o.color = 1.0;
			o.normal = avgNormal;
			triStream.Append(o);

			triStream.RestartStrip();
		}
		for (int i = 0; i < 3; i++)
		{
			g2f o;
			o.pos = UnityObjectToClipPos(IN[i].locpos);
			o.uv = TRANSFORM_TEX(IN[i].uv, _MainTex);
			o.color = 0.0;
			o.normal = IN[i].normal;
			triStream.Append(o); 
		}
		triStream.RestartStrip();
	}
	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Off
		Pass {
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#pragma multi_compile_fwdbase

			half4 frag (g2f input) : SV_Target
			{
				float3 N = normalize(input.normal);
				float ndl = dot(_WorldSpaceLightPos0, N);
				float4 litcol = ndl * _LightColor0;

				half4 col = tex2D(_MainTex, input.uv);
				col *= (_AmbientColor + litcol);
				col = lerp(col, _Color, input.color.x);

				float dsv = tex2D(_DissolveTex, input.uv).r;
				if (input.color.w == 0)
				{
					clip(dsv - 2.0 * _Weight);
				}
				else
				{
					float s = tex2D(_ShapeTex, input.uv).r;
					if (s < 0.5)
						discard;

					float brightness = input.color.w  * _Glow;
					col *= brightness + _Weight;
				}
				return col;
			}
			ENDCG
		}
		Pass {
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster

			half4 frag (g2f input) : SV_Target
			{
				half dsv = tex2D(_DissolveTex, input.uv).r;
				if (input.color.w == 0)
				{
					clip(dsv - 2.0 * _Weight);
				}
				else
				{
					half s = tex2D(_ShapeTex, input.uv).r;
					if (s < 0.5)
						discard;
				}
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	Fallback Off
}
