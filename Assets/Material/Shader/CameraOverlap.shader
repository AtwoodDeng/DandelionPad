Shader "Custom/Camera Overlap"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_OverTex ("Overlap Texture" , 2D) = "white" {}
		_Resize("Texture Resize",float) = 1.0
		_Darkness("Darkness",Range(0,1.0)) = 0.5
		_ChangeRate("Change Rate",Range(0,1)) = 0.5

	}


	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _OverTex;
			float _Darkness;
			float _Resize;
			float _ChangeRate;
			float _OffsetX;
			float _OffsetY;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				float2 offset = ( 0 , 0 );
				offset.x = _OffsetX;
				offset.y = _OffsetY;
				fixed4 overlapCol = tex2D(_OverTex, i.uv * _Resize + offset.xy );

//				overlapCol.r = clamp( overlapCol.r * _Darkness , 0 , 1 );
//				overlapCol.g = clamp( overlapCol.g * _Darkness , 0 , 1 );
//				overlapCol.b = clamp( overlapCol.b * _Darkness , 0 , 1 );
				overlapCol.r = lerp( overlapCol.r , col.r , _Darkness );
				overlapCol.g = lerp( overlapCol.g , col.g , _Darkness );
				overlapCol.b = lerp( overlapCol.b , col.b , _Darkness );

				fixed4 resCol=(0,0,0,1);

				// overlay
				resCol.r = clamp( (overlapCol.r < 0.5) ? (2 * col.r * overlapCol.r ) : (1 - 2 * (1 - col.r) * (1 - overlapCol.r) ) , 0 , 1 );
				resCol.g = clamp( (overlapCol.g < 0.5) ? (2 * col.g * overlapCol.g ) : (1 - 2 * (1 - col.g) * (1 - overlapCol.g) ) , 0 , 1 );
				resCol.b = clamp( (overlapCol.b < 0.5) ? (2 * col.b * overlapCol.b ) : (1 - 2 * (1 - col.b) * (1 - overlapCol.b) ) , 0 , 1 );

				resCol.r = clamp( (- resCol.r + col.r) * _ChangeRate + col.r , 0 , 1 );
				resCol.g = clamp( (- resCol.g + col.g) * _ChangeRate + col.g , 0 , 1 );
				resCol.b = clamp( (- resCol.b + col.b) * _ChangeRate + col.b , 0 , 1 );

				return resCol;
			}
			ENDCG
		}
	}

}
