using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Custom RenderRP")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    [SerializeField] private bool useDynamicBatching = false, useGPUInstancing = false, useSRPBatcher = false;
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher);
    }
}
