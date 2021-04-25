Shader "Unlit/Texture And Color"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Tex", 2D) = "white" {}

		[Header(Hardware settings)]
		[Enum(UnityEngine.Rendering.CullMode)] HARDWARE_CullMode ("Cull faces", Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)] HARDWARE_BlendSrc ("Blend Source", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] HARDWARE_BlendDst ("Blend Destination", Float) = 10

		[Header(Stencil)]
		HARDWARE_StencilRef ("Stencil REF", Range(0, 255)) = 0
		HARDWARE_ReadMask ("Stencil Read Mask", Range(0, 255)) = 255
		HARDWARE_WriteMask ("Stencil Write Mask", Range(0, 255)) = 255

		[Enum(UnityEngine.Rendering.CompareFunction)] HARDWARE_StencilComp ("Stencil comparison", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] HARDWARE_StencilPass ("Stencil Pass", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] HARDWARE_StencilFail ("Stencil Fail", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] HARDWARE_StencilZFail ("Stencil Z Fail", Float) = 0

		[Header(ZWrite)]
		[Enum(On, 1, Off, 0)] HARDWARE_ZWrite ("Depth write", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] HARDWARE_ZTest("Depth test", Float) = 4
		HARDWARE_Offset ("Hardware Offset", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100

		Pass
		{
			Cull [HARDWARE_CullMode]
			ZWrite [HARDWARE_ZWrite]
			ZTest [HARDWARE_ZTest]
			Blend [HARDWARE_BlendSrc] [HARDWARE_BlendDst]
			Offset [HARDWARE_Offset], -1

			Stencil
			{
				Ref [HARDWARE_StencilRef]
				Comp [HARDWARE_StencilComp]

				Pass [HARDWARE_StencilPass]
				Fail [HARDWARE_StencilFail]
				ZFail [HARDWARE_StencilZFail]
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX (v.uv, _MainTex);
				return o;
			}

			half4 frag (v2f i) : SV_Target
			{
				// sample the texture
				half4 col = tex2D(_MainTex, i.uv);
				col *= i.color * _Color;
				return col;
			}
			ENDCG
		}
	}
}