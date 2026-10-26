# PocketPixel: maintenance guide for developers and AI assistants

Version: 1.0.2-preview. Read LICENSE.txt before distributing modifications.
This is complete editable source, not an obfuscated or compiled-only asset.
No network service, account, paid plugin, Pipeline package, or AI tool is needed
to use it. The author's local CLI was only used during development.

## File map
- Runtime/PixelFilter.cs: scene component, per-object data, five palette definitions,
  active-instance registry and priority selection, material property writes.
- Runtime/PixelFilterFeature.cs: small URP integration layer derived from Unity's
  FullScreenPassRendererFeature; uses Unity's supplied RenderGraph implementation.
- Shaders/PixelFilter.shader: block sampling, luminance quantization, Bayer dithering,
  depth/normal outlines (3D), colour-contrast outlines (2D) and fractional outline coverage. Includes URP/Core HLSL.
- Editor/PixelFilterSetup.cs: discovers renderer asset references by serialized type
  in Graphics/Quality URP assets. Does not rely on m_RendererDataList's private name.
  Adds the feature idempotently, serializes the shader reference and saves assets.
- Editor/PixelFilterEditor.cs: direct Inspector controls, Undo and prefab override recording.
- Editor/PixelFilterGizmo.cs: pickable Scene-view marker; no runtime mesh/collider.
- Editor/PixelFilterDiagnostics.cs: local environment report without private paths.
- Pixel Filter.prefab: default warm-yellow object. Prefabs/: four other palettes.

## Public contracts and failure points
The Runtime code depends on UnityEngine, Core rendering and Universal rendering.
Editor-only code is in Editor/ or guarded with UNITY_EDITOR. FullScreenPassRendererFeature
must remain inheritable and expose AddRenderPasses, Create, Dispose, passMaterial,
requirements, fetchColorBuffer and injectionPoint. This is verified for URP 17.4.
Do not claim automatic compatibility with unknown future versions. If Unity changes
these APIs, adapt only the feature adapter to the new public rendering API.
Keep the Unity-provided RenderGraph path; do not force legacy Compatibility Mode.
Automatic discovery uses SerializedObject and the public UniversalRendererData and Renderer2DData types.
If renderer serialization changes, use the documented manual installation fallback.

The ShaderLab name is PocketPixel/Four Colour Camera. Installer and feature fallback
use Shader.Find with this name; changing it requires changing both callers. A serialized
feature reference is required for build inclusion. Changing folder location is supported;
there are no fixed Assets paths in runtime or installation code.

## Behavioral invariants
- No active scene component = no queued filter pass, not merely a transparent pass.
- Disabling/destroying/unloading removes an instance from the registry.
- Prefab preview scenes are ignored. Highest priority wins across loaded scenes;
  equal priorities should be avoided. This is global, not a local blended volume.
- Preview/reflection cameras are skipped; only the final camera of a stack is filtered.
- The feature owns and disposes an internal transient material. Object settings never
  write to a shared material asset. Keep this isolation when modifying the effect.
- SetPalette copies values, so custom colors remain editable. Palette changes preserve
  non-color settings. Reset restores warm-yellow effect defaults.

## 2D integration (preview, runtime validation pending)
The installer accepts UniversalRendererData and Renderer2DData references in every
Graphics/Quality URP asset. Each feature stores useColorOutlines for its owning renderer:
false for Universal, true for 2D. Repair updates existing features without adding duplicates.
For manual installation on a 2D Renderer, enable Use Color Outlines on the feature.

In 2D, requirements is Color, with fetchColorBuffer enabled. Renderer2D uses the Color
input to allocate an intermediate camera texture even with camera post-processing off.
The source is the completed camera colour, including transparent sprites, not an opaque-only texture.
Depth and Normal inputs are never requested in 2D. The shader's colour branch samples
neighbouring image colours; it does not sample scene depth or scene normals. RGB contrast
in display space is mapped with smoothstep(0.10, 0.25). Outlines follow visible image
boundaries (including sprite details), not object IDs or occluded sprite silhouettes.
The existing outlines toggle, colour and fractional width also apply in 2D.
The Universal Renderer retains its original depth/normal algorithm.

This preview was prepared without a local Unity Editor. Source/package checks do not
establish Unity compilation, RenderGraph execution or player compatibility. See TEST-2D.md
in the repository for the required acceptance checks, including post-processing off,
transparent sprites, Pixel Perfect Camera and a 3D regression check.

## Reproducing checks
1. Import into a fresh Universal-3D project, wait for compilation, inspect Console.
2. Verify repair twice doesn't add duplicate features; check every quality renderer.
3. Capture a lit cube/sphere scene without the filter, then with it. Images must differ.
4. Disable the object, disable the component, and delete the object separately.
   Each resulting image must match the original in a static deterministic scene.
5. Render all five palettes and inspect differences. Set width to 0, 0.25 and 0.5.
6. Select the prefab instance: verify controls, Undo, save/reload, colors and reset.
7. Enter/exit Play Mode with normal and disabled domain reload; repeat activation checks.
8. Build a test player for each platform you intend to advertise. Verify shader inclusion,
   scene switching and quality switching. Do not infer player support from Editor captures.

## Historical 1.0.1 verification (not a test of this preview)
Unity 6000.4.0f1, URP 17.4.0, Windows/DX12 Editor: C# and shader compilation,
five palette renders, object-disable/component-disable/delete comparisons, custom Inspector
display and export have been checked. 1.0.1 additionally checks idempotent renderer discovery.
Play-mode lifecycle edge cases, standalone builds, other Unity versions, other GPUs,
mobile, WebGL, XR and HDR output have NOT been certified. The included report records
the checks actually executed; the checklist above also includes future validation tasks.

## Distribution
Export only Assets/PocketPixel with ExportPackageOptions.Recurse (not IncludeDependencies).
Keep GUIDs/.meta files. Include source + license + docs in the download. Never ship Library,
Temp, logs, the author's level, URP package source, or editor-automation credentials.
No third-party graphics/textures are included; Unity packages are dependencies supplied by users.
