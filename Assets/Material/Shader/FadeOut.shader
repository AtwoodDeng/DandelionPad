Shader "Custom/FadeOut"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", COLOR) = (1,1,1,1)
        _FadePos("Fade Position" , Range(0,1)) = 0.5
        _FadeRange("Fade Range" , Range(0,1)) = 0.1
	}
	SubShader
	{

	    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	    Blend SrcAlpha OneMinusSrcAlpha
	    Cull Off
	    LOD 200

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _FadePos;
			float _FadeRange;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				if ( i.uv.y < _FadePos) 
				{
					col.a = 0;
				}else if ( i.uv.y < _FadePos + _FadeRange)
				{
					float a = 1 - (  _FadePos + _FadeRange - i.uv.y ) / _FadeRange;
					col.a = clamp( a , 0 , col.a);
				}
				return col ;
			}
			ENDCG
		}
	}
}
