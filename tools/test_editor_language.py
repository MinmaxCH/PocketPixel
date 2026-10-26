"""Run real C# language-adapter tests against API doubles; does not run Unity.

Requires Mono and mcs. Optional --runtime and --compiler accept local tool paths.
"""
import argparse
from pathlib import Path
import shutil
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--runtime", default=shutil.which("mono"))
    parser.add_argument("--compiler", default=shutil.which("mcs"))
    args = parser.parse_args()
    if not args.runtime or not args.compiler:
        parser.error("Mono/mcs are required for these standalone tests; they are not Unity dependencies.")
    files = [ROOT / "Source/Assets/PocketPixel/Editor/PixelFilterLocalization.cs",
             ROOT / "tests/EditorLanguageTests.cs"]
    with tempfile.TemporaryDirectory(prefix="pocketpixel-language-") as temp:
        for name, define in [("normal", "API_PRESENT"), ("missing", "API_MISSING"), ("changed", "API_WRONG_TYPE")]:
            binary = Path(temp) / (name + ".exe")
            compiler = [args.compiler]
            if args.compiler.endswith(".exe"):
                compiler.insert(0, args.runtime)
            subprocess.run(compiler + ["-nologo", "-define:" + define, "-out:" + str(binary)] + [str(f) for f in files], check=True)
            subprocess.run([args.runtime, str(binary)], check=True)
    print("PASS: normal, missing and changed Unity language API scenarios. Unity Editor UI not run.")


if __name__ == "__main__":
    main()
