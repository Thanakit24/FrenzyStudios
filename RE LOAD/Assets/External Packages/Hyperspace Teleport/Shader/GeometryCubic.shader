Shader "Hyperspace Teleport/GeometryCubic" {
	Properties {
		_MainTex          ("Main", 2D) = "white" {}
		_Smoothness       ("Smoothness", Range(0, 1)) = 0.5
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
		_EffectVector     ("Effect Vector", Vector) = (0, 0, 0, 0)
		[HDR] _EdgeColor  ("Edge", Color) = (0, 0, 0, 1)
		_PullLength       ("Pull Length", Range(1, 128)) = 1
		_EmissionPower    ("Emission Power", Range(1, 5)) = 1
		_CubeScale        ("Cube Scale", Range(0.01, 0.5)) = 0.05
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Noise.cginc"
	#include "UnityPBSLighting.cginc"

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _EffectVector;
	float3 _EdgeColor;
	half _Smoothness, _Metallic, _PullLength, _EmissionPower, _CubeScale;

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
		float4 edge     : TEXCOORD1;
		float3 wldpos   : TEXCOORD2;
	};
	v2g vert (appdata_base v)
	{
		v2g o = (v2g)0;
		o.position = mul(unity_ObjectToWorld, v.vertex);
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}
	g2f VertexOutput (float3 wpos, half3 wnrm, float2 uv, float4 edge = 0.5, float channel = 0)
	{
		g2f o;
		o.position = UnityWorldToClipPos(float4(wpos, 1));
		o.normal = wnrm;
		o.texcoord = uv;
		o.edge = edge;
		o.wldpos = wpos;
		return o;
	}
	g2f CubeVertex (float3 wpos, half3 wnrm_cube, float2 uv, float2 bary_cube, float emission)
	{
		return VertexOutput(wpos, wnrm_cube, uv, float4(bary_cube, 0.5, emission), 1);
	}
	[maxvertexcount(24)]
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

		// deformation parameter
		float param = 1 - dot(_EffectVector.xyz, center) + _EffectVector.w;
		if (param < 0)
		{
			outStream.Append(VertexOutput(p0, n0, uv0));
			outStream.Append(VertexOutput(p1, n1, uv1));
			outStream.Append(VertexOutput(p2, n2, uv2));
			outStream.RestartStrip();
			return;
		}
		if (param >= 1) return;

		// convert vertex to cube
		uint seed = pid * 877;
		{
			// cube animation
			float rnd = Random(seed + 1); // random number, gradient noise
			float4 snoise = snoise_grad(float3(rnd * 2378.34, param * 0.8, 0));

			float move = saturate(param * 4 - 3); // stretch/move param
			move = move * move;

			float3 pos = center + snoise.xyz * 0.02; // cube position
			pos.y += move * rnd;

			float3 scale = float2(1 - move, 1 + move * _PullLength).xyx; // cube scale anim
			scale *= _CubeScale * saturate(1 + snoise.w * 2) * param;

			float edge = saturate(param * _EmissionPower);

			// cube points calculation
			float3 c_p0 = pos + (float3(-1, -1, -1) + 1) * scale;
			float3 c_p1 = pos + (float3(+1, -1, -1) + 1) * scale;
			float3 c_p2 = pos + (float3(-1, +1, -1) + 1) * scale;
			float3 c_p3 = pos + (float3(+1, +1, -1) + 1) * scale;
			float3 c_p4 = pos + (float3(-1, -1, +1) + 1) * scale;
			float3 c_p5 = pos + (float3(+1, -1, +1) + 1) * scale;
			float3 c_p6 = pos + (float3(-1, +1, +1) + 1) * scale;
			float3 c_p7 = pos + (float3(+1, +1, +1) + 1) * scale;

			// cube vertex outputs
			float3 c_n = float3(-1, 0, 0);
			outStream.Append(CubeVertex(c_p2, c_n, uv0, float2(0, 0), edge));
			outStream.Append(CubeVertex(c_p0, c_n, uv2, float2(1, 0), edge));
			outStream.Append(CubeVertex(c_p6, c_n, uv0, float2(0, 1), edge));
			outStream.Append(CubeVertex(c_p4, c_n, uv2, float2(1, 1), edge));
			outStream.RestartStrip();

			c_n = float3(1, 0, 0);
			outStream.Append(CubeVertex(c_p1, c_n, uv2, float2(0, 0), edge));
			outStream.Append(CubeVertex(c_p3, c_n, uv1, float2(1, 0), edge));
			outStream.Append(CubeVertex(c_p5, c_n, uv2, float2(0, 1), edge));
			outStream.Append(CubeVertex(c_p7, c_n, uv1, float2(1, 1), edge));
			outStream.RestartStrip();

			c_n = float3(0, -1, 0);
			outStream.Append(CubeVertex(c_p0, c_n, uv2, float2(0, 0), edge));
			outStream.Append(CubeVertex(c_p1, c_n, uv2, float2(1, 0), edge));
			outStream.Append(CubeVertex(c_p4, c_n, uv2, float2(0, 1), edge));
			outStream.Append(CubeVertex(c_p5, c_n, uv2, float2(1, 1), edge));
			outStream.RestartStrip();

			c_n = float3(0, 1, 0);
			outStream.Append(CubeVertex(c_p3, c_n, uv1, float2(0, 0), edge));
			outStream.Append(CubeVertex(c_p2, c_n, uv0, float2(1, 0), edge));
			outStream.Append(CubeVertex(c_p7, c_n, uv1, float2(0, 1), edge));
			outStream.Append(CubeVertex(c_p6, c_n, uv0, float2(1, 1), edge));
			outStream.RestartStrip();

			c_n = float3(0, 0, -1);
			outStream.Append(CubeVertex(c_p1, c_n, uv2, float2(0, 0), edge));
			outStream.Append(CubeVertex(c_p0, c_n, uv2, float2(1, 0), edge));
			outStream.Append(CubeVertex(c_p3, c_n, uv1, float2(0, 1), edge));
			outStream.Append(CubeVertex(c_p2, c_n, uv0, float2(1, 1), edge));
			outStream.RestartStrip();

			c_n = float3(0, 0, 1);
			outStream.Append(CubeVertex(c_p4, c_n, uv2, float2(0, 0), edge));
			outStream.Append(CubeVertex(c_p5, c_n, uv2, float2(1, 0), edge));
			outStream.Append(CubeVertex(c_p6, c_n, uv0, float2(0, 1), edge));
			outStream.Append(CubeVertex(c_p7, c_n, uv1, float2(1, 1), edge));
			outStream.RestartStrip();
		}
	}
	float4 frag (g2f input) : SV_Target
	{
		float3 N = normalize(input.normal);
		float3 L = _WorldSpaceLightPos0.xyz;
		float3 V = normalize(_WorldSpaceCameraPos - input.wldpos);

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
		
		// fixed-width edges with using screen space derivatives of barycentric coordinates.
		float3 bcc = input.edge.xyz;
		float3 fw = fwidth(bcc);
		float3 edge3 = min(smoothstep(fw / 2, fw, bcc), smoothstep(fw / 2, fw, 1 - bcc));
		float edge = 1 - min(min(edge3.x, edge3.y), edge3.z);

		float4 emission = float4(_EdgeColor * input.edge.w * edge, 1);
		return UNITY_BRDF_PBS(albedo + ambient, specularTint,
			oneMinusReflectivity, _Smoothness, N, V,
			light, indirectLight) + emission;
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
