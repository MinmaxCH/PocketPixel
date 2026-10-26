"""Verify release contents, references and rebuilds. Does not run Unity."""
import hashlib
import io
import re
import tarfile
import zipfile

from build_release import ROOT, VERSION, PACKAGE, ARCHIVE, DOCS, exported_assets, build


def check():
    expected = exported_assets()
    with tarfile.open(ROOT / PACKAGE) as archive:
        members = [m for m in archive if m.isfile()]
        package = {m.name: archive.extractfile(m).read() for m in members}
        assert len(package) == len(members), "Duplicate tar members"
    assert len([n for n in package if n.endswith("/pathname")]) == len(expected)
    guids = {guid for _, _, _, guid in expected}
    for pathname, path, meta, guid in expected:
        assert pathname == "Assets/PocketPixel" or pathname.startswith("Assets/PocketPixel/"), pathname
        assert package[f"{guid}/pathname"] == pathname.encode(), pathname
        assert package[f"{guid}/asset.meta"] == meta.read_bytes(), pathname
        if path.is_file():
            assert package[f"{guid}/asset"] == path.read_bytes(), pathname
        else:
            assert f"{guid}/asset" not in package, pathname
    source = ROOT / "Source"
    for path in (source / "Assets/PocketPixel").rglob("*"):
        if path.is_file() and path.suffix != ".meta":
            assert path.with_name(path.name + ".meta").exists(), f"Unpackaged file: {path}"
        if path.suffix == ".prefab":
            for guid in re.findall(r"guid: ([a-f0-9]{32})", path.read_text()):
                assert guid in guids or guid.startswith("00000000"), (path, guid)
    with zipfile.ZipFile(ROOT / ARCHIVE) as archive:
        assert archive.testzip() is None
        names = archive.namelist()
        assert len(names) == len(set(names)), "Duplicate zip members"
        for name in names:
            assert name.startswith(VERSION + "/") and ".." not in name.split("/"), name
            relative = name[len(VERSION) + 1:]
            assert archive.read(name) == (ROOT / relative).read_bytes(), relative
        for doc in DOCS:
            assert f"{VERSION}/{doc}" in names, doc
        assert archive.read(f"{VERSION}/{PACKAGE}") == (ROOT / PACKAGE).read_bytes()
    # Markdown links must resolve in both the repository and installed asset folder.
    for parent in (ROOT, source / "Assets/PocketPixel"):
        for name in ("README.md", "README.de.md", "TECHNICAL.md", "TROUBLESHOOTING.md", "VALIDATION.md"):
            text = (parent / name).read_text()
            assert not re.search(r"repository.{0,30}privat|downloads require", text, re.I), name
            for target in re.findall(r"\]\(([^)]+)\)", text):
                if not target.startswith(("http:", "https:", "#")):
                    assert (parent / target.split("#", 1)[0]).exists(), (parent, name, target)
        for name in ("README.md", "README.de.md"):
            text = (parent / name).read_text()
            assert "Import Unity Package" in text and "Pixel Filter.prefab" in text
            assert "Hierarchy" in text and "Project" in text
    hashes = {n: hashlib.sha256((ROOT / n).read_bytes()).hexdigest() for n in (PACKAGE, ARCHIVE)}
    lines = (ROOT / "SHA256SUMS").read_text().splitlines()
    assert lines == [f"{hashes[n]}  {n}" for n in (PACKAGE, ARCHIVE)]
    build()
    assert all(hashlib.sha256((ROOT / n).read_bytes()).hexdigest() == digest for n, digest in hashes.items())
    print(f"PASS: {len(expected)} assets; payloads/meta/GUIDs, prefab references, ZIP contents,")
    print("bilingual README links, installation steps, checksums and reproducible builds.")
    print("Unity compilation, visual rendering and player build: NOT RUN.")


if __name__ == "__main__":
    check()
