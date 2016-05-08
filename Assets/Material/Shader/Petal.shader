Shader "Custom/Petal"
{
	Properties
	{
		_Color("Color",COLOR) = (1,1,1,1)
		_Alpha("Alpha",Range(0,1)) = 0.5
		_Bold("Bold Amount",Range(0,0.1)) = 0.01

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "IgnoreProjector"="True"}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

 			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _Bold;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.xyz += v.normal * _Bold;
				o.uv = v.texcoord.xy;
				return o;
			}
			
			fixed4 _Color;
			float _Alpha;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 resCol = _Color;
				// resCol.a = resCol.a * _Alpha;

				return resCol;
			}

			
			ENDCG
		}
	}
}
