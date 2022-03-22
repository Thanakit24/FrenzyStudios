Shader "Hyperspace Teleport/GeometryTriangle" {
	Properties {
		_MainTex          ("Main", 2D) = "white" {}
		_Smoothness       ("Smoothness", Range(0, 1)) = 0.5
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
		_EffectVector     ("Effect Vector", Vector) = (0, 0, 0, 0)
		_EffectPoint      ("Effect Point", Vector) = (0, 0, 0, 0)
		[HDR] _Emission1  ("Emission1", Color) = (0, 0, 0, 0)
		[HDR] _Emission2  ("Emission2", Color) = (0, 0, 0, 0)
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Noise.cginc"
	#include "UnityPBSLighting.cginc"

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _EffectVector;
	float3 _EffectPoint, _Emission1, _Emission2;
	half _Smoothness, _Metallic;

	struct v2g
	{
		float4 position : POSITION;
		float3 normal   : NORMAL;
		float2 texcoord : TEXCOORD0;
	};
	struct g2f
	{
		float4 position : SV_POSITION;
		float3 normal   : NORMAL;
		float2 texcoord : TEXCOORD0;
		half3  emission : TEXCOORD1;
		float3 worldPos : TEXCOORD2;
	};
	v2g vert (appdata_base v)
	{
		v2g o = (v2g)0;
		o.position = mul(unity_ObjectToWorld, v.vertex);
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}
	g2f VertexOutput (float3 wpos, half3 wnrm, float2 uv, half3 emission)
	{
		g2f o;
		o.position = UnityWorldToClipPos(float4(wpos, 1));
		o.texcoord = uv;
		o.emission = emission;
		o.normal = wnrm;
		o.worldPos = wpos;
		return o;
	}
	[maxvertexcount(6)]
	void geom (triangle v2g input[3], uint pid : SV_PrimitiveID, inout TriangleStream<g2f> outStream)
	{
		float3 p0 = input[0].position.xyz;
		float3 p1 = input[1].position.xyz;
		float3 p2 = input[2].position.xyz;

		float3 n0 = input[0].normal;
		float3 n1 = input[1].normal;
		float3 n2 = input[2].normal;

		float2 uv0 = input[0].texcoord;
		float2 uv1 = input[1].texcoord;
		float2 uv2 = input[2].texcoord;

		float3 center = (p0 + p1 + p2) / 3;

		float param = 1 - dot(_EffectVector.xyz, center) + _EffectVector.w;
		if (param < 0)
		{
			outStream.Append(VertexOutput(p0, n0, uv0, 0));
			outStream.Append(VertexOutput(p1, n1, uv1, 0));
			outStream.Append(VertexOutput(p2, n2, uv2, 0));
			outStream.RestartStrip();
			return;
		}
		if (param >= 1) return;

		uint seed = pid * 877;
		if (Random(seed) < 0.3)
		{
			// triangle vertices at the relay point
			float3 rp0 = center + snoise_grad(center * 1.3).xyz * 0.3;
			float3 rp1 = rp0 + RandomUnitVector(seed + 3) * 0.02;
			float3 rp2 = rp0 + RandomUnitVector(seed + 5) * 0.02;

			// vanishing point
			float3 rv = _EffectPoint + RandomVector(seed + 7) * 0.3;
			rv.y = center.y;

			// parameter value at the midpoint
			float m0 = 0.4 + Random(seed + 9) * 0.3;
			float m1 = m0 + (Random(seed + 10) - 0.5) * 0.2;
			float m2 = m0 + (Random(seed + 11) - 0.5) * 0.2;

			// initial inflation animation
			float3 t_p0 = p0 + (p0 - center) * 4 * smoothstep(0, 0.05, param);
			float3 t_p1 = p1 + (p1 - center) * 4 * smoothstep(0, 0.05, param);
			float3 t_p2 = p2 + (p2 - center) * 4 * smoothstep(0, 0.05, param);

			// move to the relay point.
			t_p0 = lerp(t_p0, rp0, smoothstep(0.05, m0, param));
			t_p1 = lerp(t_p1, rp1, smoothstep(0.05, m1, param));
			t_p2 = lerp(t_p2, rp2, smoothstep(0.05, m2, param));

			// move to the vanishing point.
			t_p0 = lerp(t_p0, rv, smoothstep(m0 * 0.75, 1, param));
			t_p1 = lerp(t_p1, rv, smoothstep(m1 * 0.75, 1, param));
			t_p2 = lerp(t_p2, rv, smoothstep(m2 * 0.75, 1, param));

			// recalculate the normal vector.
			float3 normal = normalize(cross(t_p1 - t_p0, t_p2 - t_p0));

			// material animation
			float3 em = lerp(_Emission1, _Emission2, Random(seed + 12));
			em *= smoothstep(0.2, 0.5, param);

			// vertex outputs
			outStream.Append(VertexOutput(t_p0, normal, uv0, em));
			outStream.Append(VertexOutput(t_p1, normal, uv1, em));
			outStream.Append(VertexOutput(t_p2, normal, uv2, em));
			outStream.RestartStrip();

			outStream.Append(VertexOutput(t_p0, -normal, uv0, em));
			outStream.Append(VertexOutput(t_p2, -normal, uv2, em));
			outStream.Append(VertexOutput(t_p1, -normal, uv1, em));
			outStream.RestartStrip();
		}
	}
	float4 frag (g2f input) : SV_Target
	{
		float3 N = normalize(input.normal);
		float3 L = _WorldSpaceLightPos0.xyz;
		float3 V = normalize(_WorldSpaceCameraPos - input.worldPos);

		float3 specularTint;
		float oneMinusReflectivity;
		half3 albedo = tex2D(_MainTex, input.texcoord).rgb;
		albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);
		
		UnityLight light;
		light.color = _LightColor0.rgb;
		light.dir = L;
		light.ndotl = DotClamped(N, L);
		UnityIndirect indirectLight;
		indirectLight.diffuse = 0;
		indirectLight.specular = 0;
		
		half3 ambient = ShadeSH9(float4(N, 1));
		
		return UNITY_BRDF_PBS(albedo + ambient, specularTint,
			oneMinusReflectivity, _Smoothness, N, V,
			light, indirectLight) + float4(input.emission, 1);
	}
	ENDCG
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma target 4.0
			ENDCG
		}
		Pass {
			Tags { "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma target 4.0
			ENDCG
		}
	}
	FallBack "Diffuse"
}
