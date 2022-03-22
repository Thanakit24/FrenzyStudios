Shader "Hyperspace Teleport/ClipPlane" {
	Properties {
		_MainTex    ("Main", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic   ("Metallic", Range(0, 1)) = 0
		_NoiseTex   ("Noise", 2D) = "black" {}
		_ClipPlane  ("Clip Plane", Vector) = (0, 0, 0, 0)
		_Clip       ("Clip", Float) = 1
		_Halo       ("Halo", Float) = 0.5
		_HaloColor  ("Halo Color", Color) = (1, 1, 0, 1)
		_Bloom      ("Bloom", Float) = 1.5
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		#pragma surface surf Standard addshadow finalcolor:teleport
		#pragma target 3.0
		#pragma multi_compile HT_DIR_X HT_DIR_Y HT_DIR_Z
		#pragma multi_compile HT_FORWARD HT_BACKWARD
		#pragma multi_compile _ HT_NOISE

		sampler2D _MainTex, _NoiseTex;
		half _Bloom, _Clip, _Halo, _Metallic, _Glossiness;
		half4 _HaloColor;

		float HTClipFrag (float3 wldpos, float2 uv)
		{
#ifdef HT_DIR_X
			float dt = wldpos.x - _Clip;
#endif
#ifdef HT_DIR_Y
			float dt = wldpos.y - _Clip;
#endif
#ifdef HT_DIR_Z
			float dt = wldpos.z - _Clip;
#endif

#ifdef HT_NOISE
			float3 ns3 = tex2D(_NoiseTex, uv).rgb;
			float ns = (ns3.x + ns3.y + ns3.z) / 3.0;
			dt += ns;
#endif

			float s = step(abs(dt), _Halo);
			float f = max(dt, 0.0);

#ifdef HT_BACKWARD
			clip(f - 0.01);
#endif
#ifdef HT_FORWARD
			clip(0.01 - f);
#endif
			return s;
		}

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			HTClipFrag(IN.worldPos, IN.uv_MainTex);
			float4 tc = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tc.rgb;
			o.Alpha = tc.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		void teleport (Input IN, SurfaceOutputStandard o, inout fixed4 color)
		{
			float s = HTClipFrag(IN.worldPos, IN.uv_MainTex);
			color.rgb = lerp(color.rgb, _HaloColor.rgb * _Bloom, s);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
