// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - supports tint color

Shader "Unlit/Crosshair" {

	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (0.5,0.5,0.5,0.5)
        //_Offset("Clip",Vector) = (0.0,0.0,0.0,0.0)
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent+5000"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZTest Always
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
				};
	
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
				sampler2D _ClipTex;
                float4 _Offset;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					if (i.texcoord.x < _Offset.x || i.texcoord.y > (1-_Offset.y) 
                    || i.texcoord.x > (1- _Offset.z) || i.texcoord.y < _Offset.w)
					{
                        half4 colr = half4(0,0,0,0);
                        return colr;
					}
					fixed4 col = tex2D(_MainTex, i.texcoord) * i.color * _Color * 2.0;
					return col;
				}
			ENDCG
		}
	}
	
	
}

