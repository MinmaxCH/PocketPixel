# 1.0.2-preview — URP 2D integration

- Auto-install and repair the feature on Universal and 2D Renderer assets, preserving existing GUIDs.
- Request camera colour in 2D so post-processing need not be enabled to allocate an intermediate target.
- Use colour-contrast outlines for sprites/tilemaps without requesting 3D depth or normal textures.
- Keep the existing Universal Renderer depth/normal outline path.
- Include renderer type and outline source in diagnostics.
- New preview downloads; original 1.0.1 packages and licence remain unchanged.
- Validation status: source/package checks only. Unity compilation, visual checks and player builds pending.

# Changelog

## 1.0.1 — 2026-10-25
- Removed dependence on the private URP renderer-list field name.
- Documented manual feature setup as a fallback; automatic shader assignment for manual setup.
- Added a local diagnostic report command, technical maintenance guide and troubleshooting.
- Included complete source and custom permissive license requiring PocketPixel - minmax.ch credits.

## 1.0.0 — 2026-10-25
- Scene-controlled four-color filter, five palettes, fractional outlines, direct Inspector.
- Root-level Pixel Filter prefab and editor-only selectable marker.
- Automatic URP renderer integration; off when no active filter object exists.
