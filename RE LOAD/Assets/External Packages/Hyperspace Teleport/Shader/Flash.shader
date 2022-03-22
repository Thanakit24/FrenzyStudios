Shader "Hyperspace Teleport/Flash" {
	Properties {
		_MainTex       ("Main", 2D) = "white" {}
		_Dissolve      ("Dissolve", Float) = 0
		_Direction     ("Direction", Vector) = (0, 1, 0, 0)
		_PivotOffset   ("Pivot Offset", Vector) = (0, 0, 0, 0)
		_Distortion    ("Distortion", Float) = 0.001
		[HDR]_Emission ("Emission", Color) = (1, 1, 1, 1)
		_NoiseSize     ("Noise size", Float) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		#pragma surface surf Standard addshadow vertex:vert
		#pragma target 3.0
		sampler2D _MainTex;
		float _Dissolve, _Distortion, _NoiseSize;
		fixed4 _Emission;
		float3 _Direction, _PivotOffset;
		
		struct Input
		{
			float2 uv_MainTex;
			float3 wldpos;
		};
		float random (float2 v)
		{
			return frac(sin(dot(v, float2(12.9898, 78.233))) * 43758.5453123);
		}
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.wldpos = mul(unity_ObjectToWorld, v.vertex.xyz + _PivotOffset.xyz);
			half f = ((dot(o.wldpos, float3(0, -1, 0)) + 1) / 2) - _Dissolve;
			float s = step(f, random(floor(o.uv_MainTex * _NoiseSize) * _Dissolve));
			v.vertex.xyz += _Direction * s * random(v.vertex.xy) * abs(f) * _Distortion;
		}
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half f = ((dot(IN.wldpos, float3(0, 1, 0)) + 1) / 2) - _Dissolve;
			float squares = random(floor(IN.uv_MainTex * _NoiseSize));
			float s = step(squares * _Dissolve, f);
			clip(s - 0.01);

			float4 tc = tex2D(_MainTex, IN.uv_MainTex);
			half ring = step(squares, _Dissolve);
			o.Albedo = tc.rgb;
			o.Emission = _Emission * ring;
			o.Alpha = tc.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
