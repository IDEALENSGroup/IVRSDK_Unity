Shader "IVR/Unlit/HandVisul"
{
	Properties
	{
        _Color ("Color",COLOR) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#define _DISPLAY_RADIUS .25
            #define _TOUCH_FEATHER 8
//            #define _TOUCHPAD_RADIUS .3027
//            #define _TOUCHPAD_CENTER half2(.441, .5)
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
                
                half4 _color : TEXCOORD1;
                half2 touchVector : TEXCOORD2;
			};

            half4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
            half4 _ToucInfo;
//            float _isTouching;

            half4 _TouchPadColor;
            half4 _ShaerDate;//x,y:center;z:radius;w:istouching

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                if(_ShaerDate.w != 0)
                {
                    half2 touchPosition = ((v.uv - _ShaerDate.xy)/_ShaerDate.z - _ToucInfo.xy);
                    half scaledInput = .25;
                    half bounced = 2 * (2 * scaledInput - scaledInput*scaledInput -.4375);
                    o.touchVector = (2-bounced)*( 1/_DISPLAY_RADIUS ) *touchPosition;
                    o._color = _TouchPadColor;
                 }
                 else
                 {
                    o._color = _Color;
                 }
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 texcol = tex2D(_MainTex, i.uv);
                half len = saturate(8*(1-length(i.touchVector)) );
                i._color = i._color *len;
                half3 tintColor = (i._color.rgb + (1-i._color.a) * _Color.rgb);

                texcol.rgb =  texcol.rgb * tintColor;

				return texcol;
			}
			ENDCG
		}
	}
}
