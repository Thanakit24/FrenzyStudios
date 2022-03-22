Shader "Hyperspace Teleport/TransparentCullOff" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_Alpha ("Alpha", Float) = 0.5
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Pass {
			Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Alpha;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			float4 frag (v2f i) : SV_TARGET
			{
				float4 tc = tex2D(_MainTex, i.tex);
				return float4(tc.rgb, tc.a * _Alpha);
			}
			ENDCG
		}
	}
	FallBack Off
}