using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PocketPixel.Editor
{
    public static class PixelFilterDiagnostics
    {
        public static string CreateReport()
        {
            var text=new StringBuilder();
            text.AppendLine("PocketPixel 1.0.1 diagnostic report");
            text.AppendLine("Editor UI mode: "+PixelFilterLocalization.Language);
            text.AppendLine("Effective UI language: "+(PixelFilterLocalization.IsGerman ? "German" : "English"));
            text.AppendLine("Unity: "+Application.unityVersion);
            var package=UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(UniversalRenderPipelineAsset).Assembly);
            text.AppendLine("URP: "+(package?.version ?? "unknown"));
            text.AppendLine("Build target: "+EditorUserBuildSettings.activeBuildTarget);
            text.AppendLine("Graphics API: "+SystemInfo.graphicsDeviceType);
            text.AppendLine("Color space: "+QualitySettings.activeColorSpace);
            text.AppendLine("Pipeline type: "+(GraphicsSettings.currentRenderPipeline?.GetType().FullName ?? "Built-in"));
            text.AppendLine("Quality index: "+QualitySettings.GetQualityLevel());
            var shader=Shader.Find("PocketPixel/Four Colour Camera");
            text.AppendLine("Shader found: "+(shader!=null));
            if(shader!=null) {text.AppendLine("Shader supported: "+shader.isSupported);text.AppendLine("Shader has errors: "+ShaderUtil.ShaderHasError(shader));}
            var filters=Object.FindObjectsByType<PixelFilter>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            text.AppendLine("Loaded filter objects: "+filters.Length);
            var current=PixelFilter.Current;
            text.AppendLine("Active winning filter: "+(current!=null));
            if(current!=null)
            {
                text.AppendLine("Priority: "+current.priority);
                text.AppendLine("Scene preview: "+current.showInSceneView);
                text.AppendLine("Pixel height: "+current.pixelHeight);
                text.AppendLine("Outline: "+current.outlines+" / width "+current.outlineWidth);
            }
            var features=Resources.FindObjectsOfTypeAll<PixelFilterFeature>();
            text.AppendLine("Loaded renderer features: "+features.Length);
            foreach(var feature in features) text.AppendLine("Feature: active="+feature.isActive+", shaderAssigned="+(feature.filterShader!=null));
            text.AppendLine("This report omits file paths, object names, usernames and full console logs. Review before sharing.");
            return text.ToString();
        }

        [MenuItem("Tools/Pixel Filter/Save Diagnostic Report")]
        static void SaveReport()
        {
            var path=EditorUtility.SaveFilePanel(PixelFilterLocalization.Text("Pixel Filter diagnostics", "Pixel Filter Diagnose"),"","PocketPixel-diagnostics","txt");
            if(!string.IsNullOrEmpty(path)) System.IO.File.WriteAllText(path,CreateReport());
        }
    }
}
