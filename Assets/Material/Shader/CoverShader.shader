Shader "Custom/CoverShader" {
 //  SubShader {
 //        Tags { "RenderType" = "Transparent" "Queue" = "Background"}
       
 //        ZWrite Off
 //        ZTest Always
 //        Blend SrcAlpha OneMinusSrcAlpha
       
 //        Pass {
 //        	Color(0,0,0,0)
 //        }
	// }

    // SubShader{
    //     ColorMask 0
    //     Pass {}
    // }

      SubShader {
    // draw after all opaque objects (queue = 2001):
    Tags { "Queue"="Overlay" }
    Pass {
      Blend Zero One // keep the image behind it
    }
  } 
}