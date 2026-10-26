using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PocketPixel
{
    public sealed class PixelFilterFeature : FullScreenPassRendererFeature
    {
        public Shader filterShader;
        Material instance;
        public override void Create()
        {
            // Also supports adding this feature manually through URP's Inspector.
            // The installer persists this reference for shader inclusion in builds.
            if(filterShader == null) filterShader = Shader.Find("PocketPixel/Four Colour Camera");
            base.Create();
        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var filter = PixelFilter.Current;
            if (filter == null || filterShader == null) return;
            var camera = renderingData.cameraData;
            if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.SceneView) return;
            if (camera.isSceneViewCamera && !filter.showInSceneView) return;
            // Apply once to the final camera of a camera stack.
            if (!camera.resolveFinalTarget) return;
            if (instance == null) instance = CoreUtils.CreateEngineMaterial(filterShader);
            filter.WriteMaterial(instance);
            passMaterial = instance;
            fetchColorBuffer = true;
            injectionPoint = InjectionPoint.AfterRenderingPostProcessing;
            requirements = filter.outlines ? ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal : ScriptableRenderPassInput.None;
            base.AddRenderPasses(renderer, ref renderingData);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(instance);
            instance = null;
            passMaterial = null;
            base.Dispose(disposing);
        }
    }
}
