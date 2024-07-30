Shader "URP/NoiseCha"

{
    Properties {
		_MainTex ("色彩贴图_MainTex", 2D) = "white" {}
		_ChaTex ("角色贴图_ChaTex", 2D) = "white" {}
		_MainTexMask ("镂空贴图（红绿图）_MainTexMask", 2D) = "white" {}
		
		[HDR]_ColorMaskR ("一层调色_ColorMaskR", Color) = (1, 1, 1, 1)
		[HDR]_ColorMaskG ("二层调色_ColorMaskR", Color) = (1, 1, 1, 1)
		[HDR]_ColorMaskB ("三层调色调色_ColorMaskB", Color) = (1, 1, 1, 1)
    	_Alpha0 ("一号层透明度_Alpha0", Range(0.0, 1.0)) = 1
    	_Alpha1 ("二号层透明度_Alpha1", Range(0.0, 1.0)) = 1
    	_Alpha2 ("三号层透明度_Alpha2", Range(0.0, 1.0)) = 1
		_UVoffset ("UV偏移_UVoffset", Float) = 0.02
    	_Maskoffset ("遮罩偏移_Maskoffset", Float) = 0.02
		_Posoffset ("顶点偏移_Posoffset", Vector) = (0,0.2,0,0)
    	//---------------------------------------
    	_OutlineWidth("边缘粗度", Range(0.0, 1.0)) = 1.0
    	[HDR]_Color ("整体校色_Color", Color) = (1, 1, 1, 1)
    }
	SubShader {

		Tags {"RenderPipeline" = "UniversalPipeline" "Queue"="Transparent" 
			"IgnoreProjector"="True" "RenderType"="Transparent" 
			"DisableBatching"="True"}
        //DisableBatching关闭批处理，因为批处理会合并所有相关的模型，而这些模型各自的模型空间就会丢失，而本shader需要在模型空间操作顶点位置实现顶点动画
		
		Pass {
			Tags { "LightMode"="UniversalForward" }
			
			ZWrite On
			Blend OneMinusDstColor One
			Cull Back
            //关闭剔除功能，可以同时显示前面与后面
			
			HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma vertex vert 
			#pragma fragment frag

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _ChaTex_ST;
            float4 _MainTexMask_ST;

			half4 _ColorMaskR;
			half4 _ColorMaskG;
			half4 _ColorMaskB;
			float _Alpha0;
			float _Alpha1;
			float _Alpha2;
			float _UVoffset;
			float _Maskoffset;
			float4 _Posoffset;

			//----------------
			float _OutlineWidth;
						half4 _Color;
            CBUFFER_END

			// sampler2D _MainTex;
			// sampler2D _ChaTex;
			// sampler2D _MainTexMask;
			
            TEXTURE2D(_MainTex);
            TEXTURE2D(_ChaTex);
            TEXTURE2D(_MainTexMask);
			SAMPLER(sampler_MainTex);
			SAMPLER(sampler_ChaTex);
			SAMPLER(sampler_MainTexMask);

            struct a2v {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
            	float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(a2v v) {
				v2f o;
				
				float4 offset;
				offset = _Posoffset;
				o.pos = TransformObjectToHClip(v.vertex + offset);
				float3 worldNormal = TransformObjectToWorldNormal(v.normal);
				float3 ndcNormal = normalize(TransformWorldToViewNormal(worldNormal)) * o.pos.w;
				
				float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));
				float aspect = abs(nearUpperRight.y / nearUpperRight.x);
				ndcNormal.x *= aspect;
				o.pos.xy += 0.01 * _OutlineWidth * ndcNormal.xy;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			half4 frag(v2f i) : SV_Target {
				return float4(_Color.rgb,1.0);
			} 
			
			ENDHLSL
		}
	}
    FallBack "Packages/com.unity.render-pipelines.universal/FallbackError"
}