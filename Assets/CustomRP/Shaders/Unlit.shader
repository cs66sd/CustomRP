Shader "Custom RP/Unlit"
{
    Properties
    {
        //[可选：特性]变量名(Inspector上的文本,类型名) = 默认值
        //[optional: attribute] name("display text in Inspector", type name) = default value
        _BaseColor("Color",Color) = (1.0,1.0,1.0,1.0)
        _BaseMap("Texture", 2D) = "white"{}
        //混合模式使用的值，其值应该是枚举值，但是这里使用float
        //特性用于在Editor下更方便编辑
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Src Blend",Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("Dst Blend",Float) = 0
        //深度写入模式
        [Enum(Off,0,On,1)] _ZWrite("Z Write",Float) = 1
    }

    SubShader
    {
        Pass
        {
            //设置混合模式
            Blend [_SrcBlend] [_DstBlend]
            
            HLSLPROGRAM
            //这一指令会让Unity生成两个该Shader的变体，一个支持GPU Instancing，另一个不支持。
            #pragma multi_compile_instancing
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #include "UnlitPass.hlsl"
            ENDHLSL
        }
    }
}