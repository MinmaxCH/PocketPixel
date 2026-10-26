# PocketPixel troubleshooting / Hilfe

Use Tools > Pixel Filter > Diagnosebericht speichern to save a diagnostic report.
Include the first compiler/shader error and a screenshot when reporting an issue.
The command writes a local text file; it does not upload anything.

## Package import
Drag PocketPixel-1.0.1.unitypackage from your file browser into the Project window
(Assets area), then click Import in the Import Unity Package window and wait for
compilation. Alternatively use Assets > Import Package > Custom Package.
The complete ZIP is not required for this step. Importing the files alone does not
activate the filter: drag Assets/PocketPixel/Pixel Filter.prefab into the Hierarchy.

## Nothing happens
1. Use URP with a Universal Renderer, not Built-in/HDRP/2D Renderer.
2. Place Pixel Filter.prefab in the scene and enable its object and component.
3. Check competing filter objects and their priorities. Turn on Scene preview if needed.
4. Run Tools > Pixel Filter > Installieren oder reparieren outside Play mode.
5. Check the renderer actually used by the camera/quality level has an enabled Pixel Filter Feature.

## Manual installation if automatic discovery fails after a Unity update
Select the Universal Renderer Data asset referenced by your URP asset. In its
Inspector choose Add Renderer Feature > Pixel Filter Feature. Set Filter Shader
to the included Shaders/PixelFilter.shader. Save the renderer asset.
Do not add a second feature when one already exists. The scene object still
controls the effect; the renderer feature is only its integration point.

## Compiler errors in another Unity version
The supplied 1.0.1 reports cover Unity 6000.4.0f1 / URP 17.4.0 in the Windows Editor. Other versions are
unverified, not guaranteed. URP is a required dependency and is not bundled.
Check the installed package's FullScreenPassRendererFeature API. Adapt
Runtime/PixelFilterFeature.cs first; do not rewrite palettes or prefabs.
Keep all .meta files and serialized field names to preserve existing scenes.
Read TECHNICAL.md for the complete design and compatibility boundaries.

## Build differs from Editor
Verify the built scene contains the enabled filter, and every graphics-quality
renderer has the feature. Filter Shader must be assigned on that feature, so
Unity includes it in the build. Standalone player compatibility is not verified by the supplied release reports. See VALIDATION.md.

## Outlines / transparent objects / blurry pixels
Outlines need depth and normals; transparent shaders often don't provide them.
Try disabling outlines to isolate this. Disable AA/upscalers after the filter
when crisp pixels are required. HDR output, XR and dynamic resolution are untested.

## Removing PocketPixel
First remove Pixel Filter Feature from each configured URP renderer. Remove
scene filter objects, then delete Assets/PocketPixel. Removing only the folder
leaves missing feature-script references on renderer assets.

## Updates
Back up the project or commit changes first. Import the newer unitypackage over
the same folder. Do not regenerate .meta files. Keep custom prefab variants in
your own folder so updating the supplied defaults does not replace them.
