using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PocketPixel
{
    public sealed class PixelFilterFeature : FullScreenPassRendererFeature
    {
        public Shader filterShader;
        [Tooltip("2D Renderer: Konturen aus Farbkontrasten statt 3D-Tiefen/Normalen. Wird vom Installer automatisch gesetzt.")]
        public bool useColorOutlines;
        static readonly int ColorOutlinesId = Shader.PropertyToID("_ColorOutlines");
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
            instance.SetFloat(ColorOutlinesId, useColorOutlines ? 1 : 0);
            passMaterial = instance;
            fetchColorBuffer = true;
            injectionPoint = InjectionPoint.AfterRenderingPostProcessing;
            bindDepthStencilAttachment = false;
            // Renderer2D checks the Color input when deciding whether to allocate an
            // intermediate camera texture, even with camera post-processing disabled.
            // fetchColorBuffer samples the completed frame, including transparent sprites.
            // Never request 3D depth/normals textures from the 2D renderer.
            requirements = useColorOutlines ? ScriptableRenderPassInput.Color :
                filter.outlines && filter.outlineWidth > 0 ?
                ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal : ScriptableRenderPassInput.None;
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
