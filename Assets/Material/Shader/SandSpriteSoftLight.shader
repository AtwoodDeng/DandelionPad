Shader "Custom/SandSpriteSoftLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SandTex ("Sand Texture", 2D) = "white" {}
		_SandResize("Sand Resize",float) = 1.0
		_SandAlpha("Sand Alpha",Range(0,1.0)) = 0.5

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 vertex : SV_POSITION;
                float3 wpos : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.wpos = mul (_Object2World, v.vertex).xyz;
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _SandTex;
			float _SandAlpha;
			float _SandResize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 sandCol = tex2D(_SandTex, i.wpos.xy * _SandResize);
				// just invert the colors

				// adjust sand color
				sandCol.r = 0.5 + (sandCol.r - 0.5) * _SandAlpha;
				sandCol.g = 0.5 + (sandCol.g - 0.5) * _SandAlpha;
				sandCol.b = 0.5 + (sandCol.b - 0.5) * _SandAlpha;
				// sandCol *= _SandAlpha;

				fixed4 resCol=(0,0,0,0);

				// overlay
				// resCol.r = (sandCol.r < 0.5) ? (2 * col.r * sandCol.r / 1) : (1 - 2 * (1 - col.r) * (1 - sandCol.r) / 1);
				// resCol.g = (sandCol.g < 0.5) ? (2 * col.g * sandCol.g / 1) : (1 - 2 * (1 - col.g) * (1 - sandCol.g) / 1);
				// resCol.b = (sandCol.b < 0.5) ? (2 * col.b * sandCol.b / 1) : (1 - 2 * (1 - col.b) * (1 - sandCol.b) / 1);

				// soft light
				resCol.r = (sandCol.r < 0.5)? (2 * (( col.r * 0.5 ) + 0.25)) * (sandCol.r / 1) : 
         (1 - ( 2 * (1 - ( (col.r * 0.5 ) + 0.25 ) )  *  ( 1 - sandCol.r ) / 1 ));
				
				resCol.g = (sandCol.g < 0.5)? (2 * (( col.g * 0.5 ) + 0.25)) * (sandCol.g / 1) : 
         (1 - ( 2 * (1 - ( (col.g * 0.5 ) + 0.25 ) )  *  ( 1 - sandCol.g ) / 1 ));
				
				resCol.b = (sandCol.b < 0.5)? (2 * (( col.b * 0.5 ) + 0.25)) * (sandCol.b / 1) : 
         (1 - ( 2 * (1 - ( (col.b * 0.5 ) + 0.25 ) )  *  ( 1 - sandCol.b ) / 1 ));

				return resCol;
			}
			ENDCG
		}
	}
}
