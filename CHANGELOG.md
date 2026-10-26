# Changelog

## 1.0.1 Editor language update — 2026-10-27
- Added per-user Automatic / English / Deutsch selection in the Pixel Filter Inspector.
- Localized Inspector labels, palette display names, help text, tooltips, buttons and installer messages.
- Automatic reads the Unity Editor language through an isolated compatibility adapter; unavailable or untranslated languages fall back to English.
- Standardized menu paths to Tools > Pixel Filter > Install or Repair and Save Diagnostic Report.
- Added language information to diagnostics; report field names remain English.
- Kept rendering code, shader, serialized component fields, prefab settings and existing GUIDs unchanged.

## 1.0.1 documentation refresh — 2026-10-27
- Clarified URP 3D / Universal Renderer scope and the recorded Editor-only test environment.
- Added drag-and-drop import instructions, the Import confirmation and the required scene prefab step in both languages.
- Included English and German READMEs in the Unity package and complete ZIP.
- Aligned the technical/troubleshooting guides and documented validation limits.
- Rebuilt downloads from the same source; no C# code, shader, prefab settings or existing GUIDs changed.
- Preserved the existing licence files and supplied test reports unchanged.

## 1.0.1 — 2026-10-25
- Removed dependence on the private URP renderer-list field name.
- Documented manual feature setup as a fallback; automatic shader assignment for manual setup.
- Added a local diagnostic report command, technical maintenance guide and troubleshooting.
- Included complete source and custom permissive license requiring PocketPixel - minmax.ch credits.

## 1.0.0 — 2026-10-25
- Scene-controlled four-color filter, five palettes, fractional outlines, direct Inspector.
- Root-level Pixel Filter prefab and editor-only selectable marker.
- Automatic URP renderer integration; off when no active filter object exists.
