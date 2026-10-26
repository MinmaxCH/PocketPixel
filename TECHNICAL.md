# PocketPixel: technical guide

Version: 1.0.1. Read LICENSE.txt before distributing modifications.
This is complete editable source, not an obfuscated or compiled-only asset.
Unity and URP are required dependencies; the asset needs no network service or account at runtime.

## File map
- Runtime/PixelFilter.cs: scene component, per-object data, five palette definitions,
  active-instance registry and priority selection, material property writes.
- Runtime/PixelFilterFeature.cs: small URP integration layer derived from Unity's
  FullScreenPassRendererFeature; uses Unity's supplied RenderGraph implementation.
- Shaders/PixelFilter.shader: block sampling, luminance quantization, Bayer dithering,
  depth/normal outlines and fractional outline coverage. Includes URP/Core HLSL.
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
requirements, fetchColorBuffer and injectionPoint. The supplied release reports cover URP 17.4.0.
Do not claim automatic compatibility with unknown future versions. If Unity changes
these APIs, adapt only the feature adapter to the new public rendering API.
Keep the Unity-provided RenderGraph path; do not force legacy Compatibility Mode.
Automatic discovery uses SerializedObject and the public UniversalRendererData type.
If renderer serialization changes, use the documented manual installation fallback.

The ShaderLab name is PocketPixel/Four Colour Camera. Installer and feature fallback
use Shader.Find with this name; changing it requires changing both callers. A serialized
feature reference is required for build inclusion. Changing folder location is supported;
there are no fixed Assets paths in runtime or installation code.

## Editor language
The Inspector stores Automatic / English / German in the per-user EditorPrefs key
PocketPixel.Editor.Language. It is not a serialized component field.
PixelFilterLocalization.cs owns text selection and language detection. Automatic
reads UnityEditor.LocalizationDatabase.currentEditorLanguage only when editor
localization is enabled. This type is internal to Unity; the small read-only adapter
finds its properties at runtime, checks their types and falls back to English on
missing APIs or getter failures. It does not use Application.systemLanguage, OS
culture, private EditorPrefs keys or change Unity's language setting.
Manual English/German overrides bypass the detector. Labels and palette names are
display text only: the Palette enum values, serialized field names and GUIDs do not change.
Menu paths stay English for documentation consistency; messages and dialogs are translated.
No runtime localization package or assembly-definition changes are required.

## Behavioral invariants
- No active scene component = no queued filter pass, not merely a transparent pass.
- Disabling/destroying/unloading removes an instance from the registry.
- Prefab preview scenes are ignored. Highest priority wins across loaded scenes;
  equal priorities should be avoided. This is global, not a local blended volume.
- Preview/reflection cameras are skipped; only the final camera of a stack is filtered.
- The feature owns and disposes an internal transient material. Object settings never
  write to a shared material asset. Keep this isolation when modifying the effect.
- ApplyPalette copies values, so custom colors remain editable. Palette changes preserve
  non-color settings. Reset restores warm-yellow effect defaults.

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

## Supplied release verification
The original 1.0.1 reports describe checks in Unity 6000.4.0f1, URP 17.4.0 and the Windows/DX12 Editor:
C# and shader compilation, five palette renders, object-disable/component-disable/delete comparisons,
custom Inspector display, export and idempotent renderer discovery.
Play-mode lifecycle edge cases, standalone builds, other Unity versions, other GPUs,
mobile, WebGL, XR and HDR output remain unverified. The included report records
the supplied results; the checklist above also includes future validation tasks.
These tests were not rerun during the 2026-10-27 documentation refresh. See VALIDATION.md.

## Distribution
The supported scope is Universal Renderer (3D), not Renderer2D, HDRP or Built-in.
The package includes English and German READMEs; the guides are in English.
Run `python tools/build_release.py` and `python tools/check_release.py` from the repository
to rebuild and verify the download containers without changing asset GUIDs.

Export only Assets/PocketPixel with ExportPackageOptions.Recurse (not IncludeDependencies).
Keep GUIDs/.meta files. Include source + license + docs in the download. Never ship Library,
Temp, logs, the author's level, URP package source, or editor-automation credentials.
No third-party graphics/textures are included; Unity packages are dependencies supplied by users.
