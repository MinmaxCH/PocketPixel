"""Rebuild PocketPixel 1.0.1 downloads from Source using only Python's stdlib.

Run: python tools/build_release.py
This packages existing files and metadata; it does not compile or test Unity code.
"""
import gzip
import hashlib
import io
from pathlib import Path
import re
import tarfile
import zipfile

ROOT = Path(__file__).resolve().parents[1]
VERSION = "1.0.1"
PACKAGE = f"PocketPixel-{VERSION}.unitypackage"
ARCHIVE = f"PocketPixel-{VERSION}-komplett.zip"
DOCS = ("README.md", "README.de.md", "LICENSE", "LICENSE.txt", "CHANGELOG.md",
        "TECHNICAL.md", "TROUBLESHOOTING.md", "VALIDATION.md",
        "TEST-ENVIRONMENT.txt", "verification.json")


def exported_assets():
    source = ROOT / "Source"
    assets = []
    guids = set()
    for meta in sorted((source / "Assets").rglob("*.meta")):
        path = Path(str(meta)[:-5])
        if not path.exists():
            raise ValueError(f"Orphan metadata: {meta.relative_to(ROOT)}")
        match = re.search(rb"^guid: ([a-f0-9]{32})\s*$", meta.read_bytes(), re.M)
        if not match or match[1] in guids:
            raise ValueError(f"Missing or duplicate GUID: {meta.relative_to(ROOT)}")
        guids.add(match[1])
        assets.append((path.relative_to(source).as_posix(), path, meta, match[1].decode()))
    if not assets:
        raise ValueError("No asset metadata found")
    return assets


def add_file(output, name, data):
    info = tarfile.TarInfo(name)
    info.mode = 0o644
    info.size = len(data)
    output.addfile(info, io.BytesIO(data))


def build():
    buffer = io.BytesIO()
    with tarfile.open(fileobj=buffer, mode="w", format=tarfile.USTAR_FORMAT) as output:
        for pathname, path, meta, guid in exported_assets():
            folder = tarfile.TarInfo(guid)
            folder.type = tarfile.DIRTYPE
            folder.mode = 0o755
            output.addfile(folder)
            if path.is_file():
                add_file(output, f"{guid}/asset", path.read_bytes())
            add_file(output, f"{guid}/asset.meta", meta.read_bytes())
            add_file(output, f"{guid}/pathname", pathname.encode("utf-8"))
    with (ROOT / PACKAGE).open("wb") as target:
        with gzip.GzipFile(filename="", fileobj=target, mode="wb", mtime=0) as zipped:
            zipped.write(buffer.getvalue())

    names = list(DOCS) + [PACKAGE]
    names += [p.relative_to(ROOT).as_posix() for directory in ("Source", "Previews")
              for p in (ROOT / directory).rglob("*") if p.is_file()]
    names += ["tools/build_release.py", "tools/check_release.py"]
    with zipfile.ZipFile(ROOT / ARCHIVE, "w", zipfile.ZIP_DEFLATED) as output:
        for name in sorted(names):
            info = zipfile.ZipInfo(f"{VERSION}/{name}")
            info.compress_type = zipfile.ZIP_DEFLATED
            output.writestr(info, (ROOT / name).read_bytes())
    (ROOT / "SHA256SUMS").write_text("".join(
        f"{hashlib.sha256((ROOT / name).read_bytes()).hexdigest()}  {name}\n"
        for name in (PACKAGE, ARCHIVE)
    ))
    print(f"Built {PACKAGE} and {ARCHIVE}; wrote SHA256SUMS")


if __name__ == "__main__":
    build()
