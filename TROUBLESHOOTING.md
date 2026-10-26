# PocketPixel troubleshooting / Hilfe

Start: Tools > Pixel Filter > Diagnosebericht speichern. Add the exact first
compiler/shader error and a screenshot to any report. Do not send an entire
private project or secrets. The diagnostic command only writes a local file.

## Nothing happens
1. Use URP with a Universal Renderer or 2D Renderer, not Built-in/HDRP. The 2D path is new in 1.0.2-preview and still needs Unity validation.
2. Place Pixel Filter.prefab in the scene and enable its object and component.
3. Check competing filter objects and their priorities. Turn on Scene preview if needed.
4. Run Tools > Pixel Filter > Installieren oder reparieren outside Play mode.
5. Check the renderer actually used by the camera/quality level has an enabled Pixel Filter Feature.

## Manual installation if automatic discovery fails after a Unity update
Select the Universal Renderer Data or 2D Renderer Data asset referenced by your URP asset. In its
Inspector choose Add Renderer Feature > Pixel Filter Feature. Set Filter Shader
to the included Shaders/PixelFilter.shader. On a 2D Renderer, enable Use Color Outlines; leave it disabled on a Universal Renderer. Save the renderer asset.
Do not add a second feature when one already exists. The scene object still
controls the effect; the renderer feature is only its integration point.

## Compiler errors in another Unity version
Version 1.0.1 was tested in Unity 6000.4.0f1 / URP 17.4.0. The 1.0.2 preview has not yet been compiled or run in Unity. Other versions are
unverified, not guaranteed. URP is a required dependency and is not bundled.
Check the installed package's FullScreenPassRendererFeature API. Adapt
Runtime/PixelFilterFeature.cs first; do not rewrite palettes or prefabs.
Keep all .meta files and serialized field names to preserve existing scenes.
Read TECHNICAL.md for the complete design and compatibility boundaries.

## After updating from 1.0.1
Import 1.0.2-preview over the existing installation and let compilation finish.
Run Tools > Pixel Filter > Installieren oder reparieren outside Play Mode.
The diagnostic report should list renderer=Renderer2D for the game camera and
outlineSource=2D colour for its feature. Keep 3D features in 3D depth/normal mode.
If the effect fails, report the first Console error and the diagnostic report.

## Build differs from Editor
Verify the built scene contains the enabled filter, and every graphics-quality
renderer has the feature. Filter Shader must be assigned on that feature, so
Unity includes it in the build. A player build has not been tested in this release.

## Outlines / transparent objects / blurry pixels
On the Universal Renderer, outlines need depth and normals; transparent shaders often do not provide them.
On the 2D Renderer, outlines detect visible colour contrasts in the composited camera image.
They can outline internal texture details and cannot separate objects with identical visible colours.
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
