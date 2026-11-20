"""Verify Crowbar's legacy PC VTF header repair, optionally decoding with VTFCmd."""

import argparse
from pathlib import Path
import shutil
from struct import pack, pack_into
import subprocess
import tempfile


def run(*command):
    return subprocess.run([str(value) for value in command], capture_output=True, encoding="utf-8", errors="replace", timeout=30)


def set_field(data, offset, kind, value):
    modified = bytearray(data)
    pack_into("<" + kind, modified, offset, value)
    return modified


def exercise(runner, root, version, source_data, length):
    source = root / f"valid-{version}.vtf"
    source.write_bytes(source_data)
    changed = root / f"header-changed-{version}.vtf"
    changed_data = set_field(source_data, 12, "I", length + 1)
    changed.write_bytes(changed_data)

    result = run(runner, source)
    assert result.returncode == 0 and result.stdout.strip() == "False", result
    result = run(runner, changed)
    assert result.returncode == 0 and result.stdout.strip() == "True", result
    restored = root / f"restored-{version}.vtf"
    result = run(runner, changed, restored)
    assert result.returncode == 0, result
    assert restored.read_bytes() == source_data
    assert changed.read_bytes() == changed_data

    for destination in (restored, changed):
        before = destination.read_bytes()
        result = run(runner, changed, destination)
        assert result.returncode == 1 and result.stdout.strip() == "IOException", result
        assert destination.read_bytes() == before

    unnecessary = root / "unnecessary.vtf"
    result = run(runner, source, unnecessary)
    assert result.returncode == 1 and not unnecessary.exists(), result
    print(f"{version}: original, altered length, repaired bytes and output collisions")
    return source, changed, restored


def run_fixtures(runner, root):
    header = bytearray(80)
    header[:4] = b"VTF\0"
    pack_into("<IIIHHIHH", header, 4, 7, 2, 80, 8, 4, 0x2200, 2, 0)
    pack_into("<iBiBBH", header, 52, 15, 4, 13, 4, 4, 1)
    # DXT1 4x4 thumbnail, then two DXT5 frames with four mips each.
    data = bytes(header) + bytes(range(8 + 2 * (32 + 16 + 16 + 16)))
    exercise(runner, root, "synthetic-7.2", data, 80)

    altered = set_field(data, 12, "I", 81)
    bad_cases = {
        "wrong-signature": b"VTFX" + altered[4:],
        "unsupported-version": set_field(altered, 8, "I", 3),
        "unsupported-format": set_field(altered, 52, "i", 7),
        "cubemap": set_field(altered, 20, "I", 0x4000),
        "volume": set_field(altered, 63, "H", 2),
        "short-header": altered[:73],
        "short-image": altered[:-1],
        "added-byte": altered[:80] + b"\0" + altered[80:],
        "wrong-mip-count": set_field(altered, 56, "B", 5),
        "incomplete-thumbnail": set_field(altered, 61, "B", 0),
    }
    for name, payload in bad_cases.items():
        source = root / f"{name}.vtf"
        target = root / "invalid-output.vtf"
        source.write_bytes(payload)
        result = run(runner, source, target)
        assert result.returncode == 1 and result.stdout.strip() == "InvalidDataException", result
        assert not target.exists() and source.read_bytes() == payload, name
    assert not list(root.glob(".crowbar-*.tmp"))
    print(f"{len(bad_cases)} invalid or unsupported inputs refused")


def verify_vtfcmd(runner, root, vtfcmd):
    tga = root / "texture.tga"
    tga.write_bytes(pack("<BBBHHBHHHHBB", 0, 0, 2, 0, 0, 0, 0, 0, 16, 8, 24, 0x20)
                    + bytes((x * 21 + y * 41) % 256 for x in range(16) for y in range(24)))
    for version, pixel_format, size in (("7.0", "RGBA8888", 64), ("7.1", "DXT1", 64), ("7.2", "DXT5", 80)):
        build = root / f"vtfcmd-{version}"
        build.mkdir()
        command = run(vtfcmd, "-file", tga, "-output", build,
                      "-version", version, "-format", pixel_format)
        raw = build / "texture.vtf"
        assert command.returncode == 0 and raw.exists(), command
        original, changed, repaired = exercise(runner, build, version, raw.read_bytes(), size)
        for image in (original, changed, repaired):
            convert = run(vtfcmd, "-file", image, "-output", build, "-exportformat", "png")
            png_file = image.with_suffix(".png")
            if image == changed:
                assert not png_file.exists(), convert
            else:
                assert convert.returncode == 0 and png_file.exists(), convert
        assert original.with_suffix(".png").read_bytes() == repaired.with_suffix(".png").read_bytes()
        print(f"{version} {pixel_format}: VTFCmd decodes the repaired and original PNG identically")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("crowbar", type=Path)
    parser.add_argument("--vtfcmd", type=Path)
    args = parser.parse_args()
    csc = Path(r"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe")
    crowbar = args.crowbar.resolve(strict=True)
    with tempfile.TemporaryDirectory(prefix="Crowbar-vtf-test-") as temporary:
        root = Path(temporary)
        shutil.copyfile(crowbar, root / crowbar.name)
        runner = root / "VtfHeaderSmoke.exe"
        result = run(csc, "/nologo", "/platform:x86", f"/reference:{crowbar}", f"/out:{runner}",
                     Path(__file__).with_name("VtfHeaderSmoke.cs").resolve())
        assert result.returncode == 0, result
        run_fixtures(runner, root)
        if args.vtfcmd:
            verify_vtfcmd(runner, root, args.vtfcmd.resolve(strict=True))


if __name__ == "__main__":
    main()

