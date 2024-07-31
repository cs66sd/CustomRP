using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private const string bufferName = "Lighting";

    private static int dirLightColorId = Shader.PropertyToID("_DirectionalLightColo4"),
        dirLightDirId = Shader.PropertyToID("_DirectionLightDirection");

    private CommandBuffer buffer = new CommandBuffer()
    {
        name = bufferName
    };

    public void Setup(ScriptableRenderContext context)
    {
        buffer.BeginSample(bufferName);
        SetupDirLight();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
    }

    void SetupDirLight()
    {
        Light light = RenderSettings.sun;
        buffer.SetGlobalVector(dirLightColorId, light.color.linear * light.intensity);
        buffer.SetGlobalVector(dirLightDirId, -light.transform.forward);
    }
}
