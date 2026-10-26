using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PocketPixel.Editor
{
    [InitializeOnLoad]
    public static class PixelFilterSetup
    {
        static PixelFilterSetup() { EditorApplication.delayCall += AutoInstall; }
        static void AutoInstall()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) { EditorApplication.delayCall += AutoInstall; return; }
            if (!EditorApplication.isPlayingOrWillChangePlaymode) Install(false);
        }

        [MenuItem("Tools/Pixel Filter/Installieren oder reparieren")]
        public static void InstallMenu() => Install(true);

        public static int Install(bool report)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return 0;
            var shader=Shader.Find("PocketPixel/Four Colour Camera");
            if(shader==null) return 0;
            var pipelines=new HashSet<UniversalRenderPipelineAsset>();
            if(GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset primary) pipelines.Add(primary);
            for(int i=0;i<QualitySettings.names.Length;i++)
                if(QualitySettings.GetRenderPipelineAssetAt(i) is UniversalRenderPipelineAsset quality) pipelines.Add(quality);
            var renderers=new HashSet<ScriptableRendererData>();
            foreach(var pipeline in pipelines)
            {
                // Discover renderer references by type, not Unity's private field names.
                var serialized=new SerializedObject(pipeline);
                var property=serialized.GetIterator();
                while(property.Next(true))
                    if(property.propertyType==SerializedPropertyType.ObjectReference &&
                        property.objectReferenceValue is ScriptableRendererData renderer &&
                        (renderer is UniversalRendererData || renderer is Renderer2DData))
                        renderers.Add(renderer);
            }
            bool changed=false;
            foreach(var renderer in renderers)
            {
                PixelFilterFeature feature=null;
                foreach(var entry in renderer.rendererFeatures) if(entry is PixelFilterFeature match) {feature=match;break;}
                if(feature==null)
                {
                    feature=ScriptableObject.CreateInstance<PixelFilterFeature>();feature.name="Pixel Filter (Scene Object)";
                    AssetDatabase.AddObjectToAsset(feature,renderer); renderer.rendererFeatures.Add(feature); changed=true;
                    feature.filterShader=shader; feature.useColorOutlines=renderer is Renderer2DData;
                    feature.SetActive(true); feature.Create();
                    EditorUtility.SetDirty(feature); renderer.SetDirty(); EditorUtility.SetDirty(renderer);
                }
                else if(feature.filterShader!=shader || !feature.isActive || feature.useColorOutlines!=(renderer is Renderer2DData))
                {
                    feature.filterShader=shader;feature.useColorOutlines=renderer is Renderer2DData;
                    feature.SetActive(true);EditorUtility.SetDirty(feature);renderer.SetDirty();EditorUtility.SetDirty(renderer);changed=true;
                }
            }
            if(changed) AssetDatabase.SaveAssets();
            if(report) Debug.Log(renderers.Count>0 ? $"Pixel Filter bereit: {renderers.Count} Renderer (Universal / 2D). Ziehe ein Pixel-Filter-Prefab in die Szene." : "Kein kompatibler Renderer gefunden. URP mit Universal Renderer oder 2D Renderer erforderlich. Manuelle Einrichtung: Renderer-Asset > Add Renderer Feature > Pixel Filter Feature; Shader PocketPixel/Four Colour Camera zuweisen; auf einem 2D Renderer Use Color Outlines aktivieren. Siehe TROUBLESHOOTING.md.");
            return renderers.Count;
        }

        [MenuItem("GameObject/Effects/Pixel Filter",false,10)]
        public static void CreateObject()
        {
            Install(false);
            var go=new GameObject("Pixel Filter"); Undo.RegisterCreatedObjectUndo(go,"Create Pixel Filter");
            go.AddComponent<PixelFilter>(); Selection.activeGameObject=go;
        }
    }
}
