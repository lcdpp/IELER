Shader "Unlit/Transparent Colored Eraser"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "white" {}
	_RendTex("Base (RGB), Alpha (A)", 2D) = "white" {}
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
		half4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
		half4 rnd = tex2D(_RendTex, IN.texcoord) * IN.color;
		
		if ((col.r < 0.1) && (col.g < 0.1) && (col.b < 0.1) && (col.a > 0.5))
			rnd.rgb = half3(0, 0, 0);
		else
			rnd.rgb = half3(1, 1, 1);

		half4 ret = col * (1 - rnd.a) + rnd * rnd.a;
		//col.a = rnd.a;
		return ret;
	}
		ENDCG
	}
	}
}