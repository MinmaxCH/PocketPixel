"""Build the 2D preview without changing Unity GUIDs or the original release.

Run from any directory: python tools/build_release.py
Uses the original 1.0.1 export only as a Unity package container template.
Every asset and .meta payload is replaced with the corresponding Source file.
"""
import gzip
import io
from pathlib import Path
import tarfile
import zipfile

ROOT = Path(__file__).resolve().parents[1]
VERSION = "1.0.2-preview"
PACKAGE = f"PocketPixel-{VERSION}.unitypackage"
ARCHIVE = f"PocketPixel-{VERSION}-komplett.zip"


def build():
    # Keep pathnames, GUID directories and meta files exactly as exported by Unity.
    with tarfile.open(ROOT / "PocketPixel-1.0.1.unitypackage", "r:gz") as original:
        members = original.getmembers()
        paths = {
            member.name.rsplit("/", 1)[0]: original.extractfile(member).read().decode("utf-8")
            for member in members if member.name.endswith("/pathname")
        }
        buffer = io.BytesIO()
        with tarfile.open(fileobj=buffer, mode="w", format=tarfile.USTAR_FORMAT) as output:
            for member in members:
                info = tarfile.TarInfo(member.name)
                info.type = member.type
                info.mode = member.mode
                if not member.isfile():
                    output.addfile(info)
                    continue
                folder, leaf = member.name.rsplit("/", 1)
                if leaf in ("asset", "asset.meta"):
                    source = ROOT / "Source" / paths[folder]
                    if leaf == "asset.meta":
                        source = Path(str(source) + ".meta")
                    data = source.read_bytes()
                else:
                    data = original.extractfile(member).read()
                info.size = len(data)
                output.addfile(info, io.BytesIO(data))
        with (ROOT / PACKAGE).open("wb") as target:
            with gzip.GzipFile(filename="", fileobj=target, mode="wb", mtime=0) as gz:
                gz.write(buffer.getvalue())

    # A clean versioned release folder, excluding local tooling and git metadata.
    names = ["README.md", "README.de.md", "LICENSE.txt", "TECHNICAL.md",
             "TROUBLESHOOTING.md", "CHANGELOG.md", "TEST-2D.md", PACKAGE]
    names += [str(p.relative_to(ROOT)) for directory in ("Source", "Previews")
              for p in (ROOT / directory).rglob("*") if p.is_file()]
    with zipfile.ZipFile(ROOT / ARCHIVE, "w", zipfile.ZIP_DEFLATED) as output:
        for name in sorted(names):
            info = zipfile.ZipInfo(f"{VERSION}/{name}")
            info.compress_type = zipfile.ZIP_DEFLATED
            output.writestr(info, (ROOT / name).read_bytes())
        for name in ("TEST-ENVIRONMENT.txt", "verification.json"):
            output.writestr(f"{VERSION}/Verification/1.0.1/{name}", (ROOT / name).read_bytes())
    print(PACKAGE)
    print(ARCHIVE)


if __name__ == "__main__":
    build()
