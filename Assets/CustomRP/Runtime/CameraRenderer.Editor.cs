using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

partial  class CameraRenderer
{
    partial void DrawUnsurppotedShaders();
    partial void DrawGizmos();
    
    partial void PrepareForSceneWindow();
    partial void PrepareBuffer();

    #if UNITY_EDITOR
    static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    private static Material errorMaterial;

   partial void DrawUnsurppotedShaders()
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
            
        }

        var drawingSettings = 
            new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera))
            {
                overrideMaterial = errorMaterial
            };
            
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        var filteringSettings = FilteringSettings.defaultValue;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

   partial void DrawGizmos()
   {
       if (Handles.ShouldRenderGizmos())
       {
           context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
           context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
       }
   }

   partial void PrepareBuffer()
   {
       Profiler.BeginSample("Editor Only");
       buffer.name = camera.name;
       Profiler.EndSample();
   }
   
   partial void PrepareForSceneWindow()
   {
       if (camera.cameraType == CameraType.SceneView)
       {
           ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
       }
   }
#else
    string SamplerName => bufferName;
#endif
}
