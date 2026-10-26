# PocketPixel 1.0.1 — validation status

Editor language and package refresh: 2026-10-27. Editor UI scripts were updated; rendering scripts, shader, prefab settings, serialized component fields and existing Unity GUIDs were not changed.

## Supported scope

Unity 6, Universal Render Pipeline, **Universal Renderer (3D)**.
Built-in, HDRP and the URP 2D Renderer are not supported.
The presence of 2D sprites or an orthographic camera does not by itself change the renderer in use.

## Supplied test records

The original TEST-ENVIRONMENT.txt and verification.json were supplied with version 1.0.1.
They record Unity 6000.4.0f1 / URP 17.4.0, Windows Editor, Direct3D12 and Linear colour space.
The supplied checks cover image changes with the filter enabled, disabling/deleting the filter,
five palette differences and no reported shader errors. The accompanying report also records
idempotent installation on two renderer assets.

These reports have been preserved as supplied. They were not rerun during this Editor UI refresh.

## Checks for this refresh

- Runtime scripts, shader, prefabs and existing metadata are byte-identical to the previous 1.0.1 package.
- The Editor language selector and translations are new in this refresh; original rendering reports do not validate this UI change.
- All seven asset C# files and the standalone test file pass syntax parsing.
- The actual C# language adapter was compiled and exercised with Mono against API doubles: 39 assertions passed across normal, missing and changed Unity-language API scenarios. These are adapter tests, not a Unity Editor UI test. Reproduce with python tools/test_editor_language.py (requires Mono/mcs).
- All exported asset payloads and metadata match Source/Assets/PocketPixel.
- The existing licence files are unchanged; the Unity package retains its original bundled licence.
- Both language READMEs explain package import, confirming Import and adding the prefab to the scene.
- The complete ZIP contains the same refreshed Unity package, source, bilingual READMEs and guides.
- Local Markdown links, download filenames, package GUIDs and prefab script references are checked.
- ZIP and Unity package output can be reproduced with the standard-library scripts in tools/.

## Not independently verified in this refresh

A Unity Editor is not available in the preparation environment. In-Editor compilation, Inspector rendering,
Play Mode lifecycle, performance and a standalone player build have not been rerun here.
A build-target value in TEST-ENVIRONMENT.txt identifies the selected target; it is not evidence
that a player was built and tested. Other GPUs/platforms, XR, HDR output and dynamic resolution
remain unverified.

Before advertising player/platform compatibility, build and run a test scene on that target.
Check the effect, all palettes, toggling/deleting the object, shader inclusion, scene changes and
quality-level changes. Record actual results before broadening the documented support.
