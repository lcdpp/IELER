Shader "Unlit/Transparent Colored Eraser"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "white" {}
		_RendTex("Base (RGB), Alpha (A)", 2D) = "white" {}
		_LineColor1("LineColor1", Color) = (0,0,0,0)
		_LineColor2("LineColor2", Color) = (0,0,0,0)
		_LineColor3("LineColor3", Color) = (0,0,0,0)
		_LineColor4("LineColor4", Color) = (0,0,0,0)
		_LineColor5("LineColor5", Color) = (0,0,0,0)
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			//Offset - 1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _RendTex;
			half4 _LineColor1;
			half4 _LineColor2;
			half4 _LineColor3;
			half4 _LineColor4;
			half4 _LineColor5;

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			half4 frag(v2f IN) : COLOR
			{
				// Sample the texture
				half4 col = tex2D(_MainTex, IN.texcoord);
				half4 rnd = tex2D(_RendTex, IN.texcoord);

				half4 lineColorDiff1 = abs(col - _LineColor1);
				half4 lineColorDiff2 = abs(col - _LineColor2);
				half4 lineColorDiff3 = abs(col - _LineColor3);
				half4 lineColorDiff4 = abs(col - _LineColor4);
				half4 lineColorDiff5 = abs(col - _LineColor5);

				if ((col.r < 0.1) && (col.g < 0.1) && (col.b < 0.1) && (col.a > 0.5))
					rnd.rgb = half3(0, 0, 0);
				else if ((lineColorDiff1.r < 0.1) && (lineColorDiff1.g < 0.1) && (lineColorDiff1.b < 0.1) && (col.a > 0.5))
					rnd.rgb = half3(0, 0, 0);
				else if ((lineColorDiff2.r < 0.1) && (lineColorDiff2.g < 0.1) && (lineColorDiff2.b < 0.1) && (col.a > 0.5))
					rnd.rgb = half3(0, 0, 0);
				else if ((lineColorDiff3.r < 0.1) && (lineColorDiff3.g < 0.1) && (lineColorDiff3.b < 0.1) && (col.a > 0.5))
					rnd.rgb = half3(0, 0, 0);
				else if ((lineColorDiff4.r < 0.1) && (lineColorDiff4.g < 0.1) && (lineColorDiff4.b < 0.1) && (col.a > 0.5))
					rnd.rgb = half3(0, 0, 0);
				else if ((lineColorDiff5.r < 0.1) && (lineColorDiff5.g < 0.1) && (lineColorDiff5.b < 0.1) && (col.a > 0.5))
					rnd.rgb = half3(0, 0, 0);
				else
					rnd.rgb = IN.color;


				half4 ret = col * (1 - rnd.a) + rnd * rnd.a;
				ret.a = max(col.a, rnd.a);
				//col.a = rnd.a;
				return ret;
			}
			ENDCG
		}
	}
}