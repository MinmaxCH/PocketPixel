"""Check release payloads against source; does not replace tests in Unity."""
import hashlib
import io
from pathlib import Path
import re
import tarfile
import zipfile

from build_release import ROOT, VERSION, PACKAGE, ARCHIVE


def check():
    with tarfile.open(ROOT / "PocketPixel-1.0.1.unitypackage") as old:
        original = {m.name: old.extractfile(m).read() for m in old if m.isfile()}
    with tarfile.open(ROOT / PACKAGE) as new:
        packed = {m.name: new.extractfile(m).read() for m in new if m.isfile()}
    assert original.keys() == packed.keys(), "Unity package paths or GUIDs changed"
    asset_count = 0
    for name, path_bytes in packed.items():
        if not name.endswith("/pathname"):
            continue
        assert path_bytes == original[name], name
        path = path_bytes.decode("utf-8")
        assert path.startswith("Assets/PocketPixel"), path
        source = ROOT / "Source" / path
        prefix = name.rsplit("/", 1)[0]
        meta = packed[prefix + "/asset.meta"]
        assert meta == Path(str(source) + ".meta").read_bytes(), path
        assert meta == original[prefix + "/asset.meta"], "Original metadata changed: " + path
        assert re.search(rb"guid: ([a-f0-9]{32})", meta).group(1).decode() == prefix
        if prefix + "/asset" in packed:
            assert packed[prefix + "/asset"] == source.read_bytes(), path
            asset_count += 1
        else:
            assert source.is_dir(), path
    with zipfile.ZipFile(ROOT / ARCHIVE) as archive:
        assert archive.testzip() is None
        for name in archive.namelist():
            path = name.removeprefix(VERSION + "/")
            if path.startswith("Verification/1.0.1/"):
                path = path.removeprefix("Verification/1.0.1/")
            assert archive.read(name) == (ROOT / path).read_bytes(), name
        assert archive.read(f"{VERSION}/{PACKAGE}") == (ROOT / PACKAGE).read_bytes()
    # Compare licence bytes directly with the original Unity-exported licence.
    license_entry = next(name.rsplit("/", 1)[0] for name, b in original.items()
                         if name.endswith("/pathname") and b.decode().endswith("/LICENSE.txt"))
    assert original[license_entry + "/asset"] == (ROOT / "Source/Assets/PocketPixel/LICENSE.txt").read_bytes()
    for language in ("README.md", "README.de.md"):
        for link in re.findall(r"\]\(([^)]+)\)", (ROOT / language).read_text()):
            if not link.startswith(("http:", "https:", "#")):
                assert (ROOT / link).exists(), (language, link)
    # Reproducible packages: a second build must produce the same bytes.
    from build_release import build
    before = {n: hashlib.sha256((ROOT / n).read_bytes()).digest() for n in (PACKAGE, ARCHIVE)}
    build()
    assert all(hashlib.sha256((ROOT / n).read_bytes()).digest() == sha for n, sha in before.items())
    print(f"PASS: {asset_count} asset payloads, all original GUIDs/meta and licence bytes preserved;")
    print("ZIP/source equality, README local links and deterministic rebuild verified.")
    print("Unity compilation, visual rendering and player build: NOT RUN (Unity unavailable).")


if __name__ == "__main__":
    check()
