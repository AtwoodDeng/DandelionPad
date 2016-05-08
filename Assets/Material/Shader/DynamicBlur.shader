Shader "Custom/DynamicBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", COLOR) = (1,1,1,1)
		_CoverTex ("Cover" , 2D) = "white" {}
        _FadePos("Fade Position" , Range(0,1)) = 0.5
        _FadeRange("Fade Range" , Range(0,1)) = 0.1
        _BlurAmount("Blur Amount" , float) = 0
        // _CoverRec("Cover Rect" , Vector) = (0,0,0,0)
	}



	CGINCLUDE

	#include "UnityCG.cginc"

     struct v2f_blur_hor
     {
         float4  pos : SV_POSITION;
         float2  uv : TEXCOORD0;
     };


     struct v2f_blur_ver
     {
         float4  pos : SV_POSITION;
         float2  uv : TEXCOORD0;
     };

	struct v2f_cover
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};


    sampler2D _GrabTexture : register(s0);
	sampler2D _MainTex;
	sampler2D _CoverTex;
	float4 _MainTex_ST;
	fixed4 _Color;
	float _FadePos;
	float _FadeRange;
	float4 _CoverRec[25];
	int _TemIndex;
	int _CountNum;
    float _BlurAmount;
    float4 _GrabTexture_ST;


    v2f_blur_hor vert_blur_hor (appdata_base v)
     {
         v2f_blur_hor o;
         o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
         o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
         return o;
     }
     
     half4 frag_blur_hor (v2f_blur_hor i) : COLOR
     {
         half4 sum = half4(0,0,0,0);
         float blurDistance = _BlurAmount * 0.01f;
		 				
         sum += tex2D(_MainTex, float2(i.uv.x - 5.0 * blurDistance, i.uv.y)) * 0.025;
         sum += tex2D(_MainTex, float2(i.uv.x - 4.0 * blurDistance, i.uv.y)) * 0.05;
         sum += tex2D(_MainTex, float2(i.uv.x - 3.0 * blurDistance, i.uv.y)) * 0.09;
         sum += tex2D(_MainTex, float2(i.uv.x - 2.0 * blurDistance, i.uv.y)) * 0.12;
         sum += tex2D(_MainTex, float2(i.uv.x - blurDistance, i.uv.y)) * 0.15;
         sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * 0.16;
         sum += tex2D(_MainTex, float2(i.uv.x + blurDistance, i.uv.y)) * 0.15;
         sum += tex2D(_MainTex, float2(i.uv.x + 2.0 * blurDistance, i.uv.y)) * 0.12;
         sum += tex2D(_MainTex, float2(i.uv.x + 3.0 * blurDistance, i.uv.y)) * 0.09;
         sum += tex2D(_MainTex, float2(i.uv.x + 4.0 * blurDistance, i.uv.y)) * 0.05;
         sum += tex2D(_MainTex, float2(i.uv.x + 5.0 * blurDistance, i.uv.y)) * 0.025;


         return sum;
     }

     v2f_blur_ver vert_blur_ver (appdata_base v)
     {
         v2f_blur_ver o;
         o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
         o.uv = TRANSFORM_TEX(v.texcoord, _GrabTexture);
         return o;
     }

     half4 frag_blur_ver (v2f_blur_ver i) : COLOR
     {

         half4 sum = half4(0,0,0,0);

         float blurDistance = _BlurAmount * 0.01f;


         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 5.0 * blurDistance)) * 0.025;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 4.0 * blurDistance)) * 0.05;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 3.0 * blurDistance)) * 0.09;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 2.0 * blurDistance)) * 0.12;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - blurDistance)) * 0.15;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y)) * 0.16;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + blurDistance)) * 0.15;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 2.0 * blurDistance)) * 0.12;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 3.0 * blurDistance)) * 0.09;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 4.0 * blurDistance)) * 0.05;
         sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 5.0 * blurDistance)) * 0.025;

         return sum;
     }

	v2f_cover vert_cover (appdata v)
	{
		v2f_cover o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o; 
	}

	 
	fixed4 frag_cover (v2f_cover i) : SV_Target
	{
		// sample the texture
		fixed4 col = tex2D(_GrabTexture, i.uv) * _Color;

		return col;

		float coverA = 0;
		int k = 0;
		for(; k < _CountNum ; ++ k )
		{   
			float2 coverPos = i.uv;
			 coverPos.x += _CoverRec[k].x;
			 coverPos.x = 0.5 + ( coverPos.x - 0.5 ) * _CoverRec[k].z;
			 coverPos.y += _CoverRec[k].y;
			 coverPos.y = 0.5 + ( coverPos.y - 0.5 ) * _CoverRec[k].w;
//					 coverPos.x = lerp(coverPos.x,0,1);
//					 coverPos.y = lerp(coverPos.y,0,1);
			  
			fixed4 coverCol = tex2D(_CoverTex, coverPos);

			coverA = lerp( coverA , 1 , coverCol.a );
		}

		col.a *= coverA;

		if ( i.uv.y < _FadePos) 
		{
			col.a = 0;
		}else if ( i.uv.y < _FadePos + _FadeRange)
		{
			float a = 1 - (  _FadePos + _FadeRange - i.uv.y ) / _FadeRange;
			// col.a = clamp( a , 0 , col.a * coverCol.a );
			col.a *= a;
		}
		return col ;
	}

	ENDCG

Subshader {
	    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	    Blend SrcAlpha OneMinusSrcAlpha
	    Cull Off
	    LOD 200
	 Pass {     
	 		Name "BlurHor"
	      CGPROGRAM
	      
	      #pragma vertex vert_blur_hor
	      #pragma fragment frag_blur_hor
	      
	      ENDCG
	  }

//	 Pass {     
//
//	      CGPROGRAM
//	      
//	      #pragma vertex vert_blur_ver
//	      #pragma fragment frag_blur_ver
//	      
//	      ENDCG
//	  }

	GrabPass { "BlurHor" }

	 Pass {     

	      CGPROGRAM
	      
	      #pragma vertex vert_cover
	      #pragma fragment frag_cover
	      
	      ENDCG
	  }

  }



//	SubShader
//	{
//
//	    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
//	    Blend SrcAlpha OneMinusSrcAlpha
//	    Cull Off
//	    LOD 200
//
//
//	            // Horizontal blur pass
//         Pass
//         {
//             Blend SrcAlpha OneMinusSrcAlpha 
//             Name "HorizontalBlur"
// 
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag            
//             #include "UnityCG.cginc"
//             
//             sampler2D _MainTex;
//             
//             struct v2f
//             {
//                 float4  pos : SV_POSITION;
//                 float2  uv : TEXCOORD0;
//             };
//             
//             float4 _MainTex_ST;
//             float _BlurAmount;
//             
//             v2f vert (appdata_base v)
//             {
//                 v2f o;
//                 o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                 o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
//                 return o;
//             }
//             
//             half4 frag (v2f i) : COLOR
//             {
//                 half4 sum = half4(0,0,0,0);
//                 float blurDistance = _BlurAmount * 0.01f;
// 				 				
//                 sum += tex2D(_MainTex, float2(i.uv.x - 5.0 * blurDistance, i.uv.y)) * 0.025;
//                 sum += tex2D(_MainTex, float2(i.uv.x - 4.0 * blurDistance, i.uv.y)) * 0.05;
//                 sum += tex2D(_MainTex, float2(i.uv.x - 3.0 * blurDistance, i.uv.y)) * 0.09;
//                 sum += tex2D(_MainTex, float2(i.uv.x - 2.0 * blurDistance, i.uv.y)) * 0.12;
//                 sum += tex2D(_MainTex, float2(i.uv.x - blurDistance, i.uv.y)) * 0.15;
//                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * 0.16;
//                 sum += tex2D(_MainTex, float2(i.uv.x + blurDistance, i.uv.y)) * 0.15;
//                 sum += tex2D(_MainTex, float2(i.uv.x + 2.0 * blurDistance, i.uv.y)) * 0.12;
//                 sum += tex2D(_MainTex, float2(i.uv.x + 3.0 * blurDistance, i.uv.y)) * 0.09;
//                 sum += tex2D(_MainTex, float2(i.uv.x + 4.0 * blurDistance, i.uv.y)) * 0.05;
//                 sum += tex2D(_MainTex, float2(i.uv.x + 5.0 * blurDistance, i.uv.y)) * 0.025;
//
// 
//                 return sum;
//             }
//             ENDCG
//         }
//         
//         GrabPass { }
// 
//         // Vertical blur pass
//         Pass
//         {
//             Blend SrcAlpha OneMinusSrcAlpha
//             Name "VerticalBlur"
//                         
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag            
//             #include "UnityCG.cginc"
// 
//             sampler2D _GrabTexture : register(s0);
// 
//
//
// 
//             float4 _GrabTexture_ST;
//             float _BlurAmount;
// 
//             v2f vert (appdata_base v)
//             {
//                 v2f o;
//                 o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                 o.uv = TRANSFORM_TEX(v.texcoord, _GrabTexture);
//                 return o;
//             }
// 
//             half4 frag (v2f i) : COLOR
//             {
// 
//                 half4 sum = half4(0,0,0,0);
//
//                 float blurDistance = _BlurAmount * 0.01f;
//
//
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 5.0 * blurDistance)) * 0.025;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 4.0 * blurDistance)) * 0.05;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 3.0 * blurDistance)) * 0.09;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 2.0 * blurDistance)) * 0.12;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - blurDistance)) * 0.15;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y)) * 0.16;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + blurDistance)) * 0.15;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 2.0 * blurDistance)) * 0.12;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 3.0 * blurDistance)) * 0.09;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 4.0 * blurDistance)) * 0.05;
//                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 5.0 * blurDistance)) * 0.025;
// 
//                 return sum;
//             }
//             ENDCG
//         }
//
//         GrabPass {} 
//
//
//		// cover
//		Pass
//		{
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#include "UnityCG.cginc"
//
//			struct appdata
//			{
//				float4 vertex : POSITION;
//				float2 uv : TEXCOORD0;
//			};
//
//			struct v2f
//			{
//				float2 uv : TEXCOORD0;
//				float4 vertex : SV_POSITION;
//			};
//
//            sampler2D _GrabTexture : register(s0);
//			sampler2D _MainTex;
//			sampler2D _CoverTex;
//			float4 _MainTex_ST;
//			fixed4 _Color;
//			float _FadePos;
//			float _FadeRange;
//			float4 _CoverRec[25];
//			int _TemIndex;
//			int _CountNum;
////			float4 _CoverRec;
//
//
////			StructuredBuffer<float4> _CoverRec;
//
//			v2f vert (appdata v)
//			{
//				v2f o;
//				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//				return o; 
//			}
//
////			fixed AlphaAdd( float2 uv , float4 rec )
////			{
////				fixed pos = uv + rec.xy;
////				fixed4 col = tex2D( _CoverTex, pos);
////				return col.a * 0.04;
////			} 
//			 
//			fixed4 frag (v2f i) : SV_Target
//			{
//				// sample the texture
//				fixed4 col = tex2D(_GrabTexture, i.uv) * _Color;
//
////				fixed4 coverCol = tex2D( _CoverTex , i.uv + _CoverRec.xy);
////				col.a *= coverCol.a;
//
//
//				// col.a += AlphaAdd( i.uv , _CoverRec[0]);
//
//				float coverA = 0;
//				int k = 0;
//				for(; k < _CountNum ; ++ k )
//				{   
//					float2 coverPos = i.uv;
//					 coverPos.x += _CoverRec[k].x;
//					 coverPos.x = 0.5 + ( coverPos.x - 0.5 ) * _CoverRec[k].z;
//					 coverPos.y += _CoverRec[k].y;
//					 coverPos.y = 0.5 + ( coverPos.y - 0.5 ) * _CoverRec[k].w;
////					 coverPos.x = lerp(coverPos.x,0,1);
////					 coverPos.y = lerp(coverPos.y,0,1);
//					  
//					fixed4 coverCol = tex2D(_CoverTex, coverPos);
//
//					coverA = lerp( coverA , 1 , coverCol.a );
//				}
//
//				col.a *= coverA;
//
//				if ( i.uv.y < _FadePos) 
//				{
//					col.a = 0;
//				}else if ( i.uv.y < _FadePos + _FadeRange)
//				{
//					float a = 1 - (  _FadePos + _FadeRange - i.uv.y ) / _FadeRange;
//					// col.a = clamp( a , 0 , col.a * coverCol.a );
//					col.a *= a;
//				}
//				return col ;
//			}
//
//			ENDCG
//		}
//	}
Fallback "Diffuse"
}
