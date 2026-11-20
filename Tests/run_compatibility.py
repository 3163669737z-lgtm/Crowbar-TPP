"""Generate tiny Source assets and test the Windows decompiler entry point."""

import argparse
import hashlib
from pathlib import Path
import shutil
from struct import pack_into
import subprocess
import tempfile

CHECKSUM = 0x1B2C3D4E
BONES = 408
BODY = BONES + 2 * 216
MODEL = BODY + 16
MESH = MODEL + 148
TEXTURE = MESH + 116
STRINGS = TEXTURE + 64


def put(data, offset, *integers):
    pack_into("<" + "i" * len(integers), data, offset, *integers)


def write_model(path, version=49, abnormal_names=False, unterminated=False, internal_name="hidden/mesh.mdl"):
    path.parent.mkdir(parents=True, exist_ok=True)
    data = bytearray(STRINGS)
    data[:4] = b"IDST"
    put(data, 4, version, CHECKSUM)
    name = internal_name.encode("ascii")
    data[12:12 + len(name)] = name
    put(data, 156, 2, BONES)
    put(data, 204, 1, TEXTURE)
    put(data, 232, 1, BODY)

    def add_string(value):
        start = len(data)
        data.extend(value)
        return start

    root = add_string(b"root\0")
    child = add_string(b'bad"\nname\0' if abnormal_names else b"child\0")
    group = add_string(b"\0" if abnormal_names else b"body\0")
    material = add_string(b"unit_test\0")
    if unterminated:
        child = add_string(b"missing terminator")

    put(data, BONES, root - BONES, -1)
    put(data, BONES + 216, child - BONES - 216, 0)
    for bone in (BONES, BONES + 216):
        pack_into("<f", data, bone + 56, 1.0)
        for component in (0, 5, 10):
            pack_into("<f", data, bone + 96 + component * 4, 1.0)
    put(data, BODY, group - BODY, 1, 1, MODEL - BODY)
    model_name = b"CON.smd" if abnormal_names else b"triangle.smd"
    data[MODEL:MODEL + len(model_name)] = model_name
    put(data, MODEL + 72, 1, MESH - MODEL, 3, 0, 0)
    put(data, MESH + 8, 3, 0)
    put(data, MESH + 52, 3)
    put(data, TEXTURE, material - TEXTURE)
    put(data, 76, len(data))
    path.write_bytes(data)
    return path


def write_pair(base, checksum=CHECKSUM, broken_vtx=False):
    base.parent.mkdir(parents=True, exist_ok=True)
    vvd = bytearray(64 + 3 * (48 + 16))
    vvd[:4] = b"IDSV"
    put(vvd, 4, 4, checksum, 1, *([3] * 8), 0, 64, 64, 208)
    for index, (x, y) in enumerate(((0.0, 0.0), (1.0, 0.0), (0.0, 1.0))):
        pack_into("<3f4B8f", vvd, 64 + index * 48, 1, 0, 0, 0, 0, 0, 1, x, y, 0, 0, 0, 1, x, y)

    vtx = bytearray(158)
    pack_into("<iiHHiiiiii", vtx, 0, 7, 24, 16, 3, 3, checksum, 1, 0, 1, 36)
    put(vtx, 36, 1, 8)
    put(vtx, 44, 1, 8)
    pack_into("<iif", vtx, 52, 1, 12, 0.0)
    pack_into("<iiB", vtx, 64, 1, 9, 0)
    pack_into("<iiiiiiB", vtx, 73, 3, 52, 3, 79, 1, 25, 0)
    pack_into("<iiiihBii", vtx, 98, 3, 0, 3, 0, 0, 1, 0, 0)
    for index in range(3):
        pack_into("<4BH3B", vtx, 125 + index * 9, 0, 0, 0, 1, index, 0, 0, 0)
    pack_into("<3H", vtx, 152, 0, 1, 9 if broken_vtx else 2)
    base.with_suffix(".vvd").write_bytes(vvd)
    base.with_suffix(".dx90.vtx").write_bytes(vtx)


def checked_run(runner, root, model, status, *markers, resource="", group=False, lock=None):
    output = root / ("output-" + model.stem)
    output.mkdir(exist_ok=True)
    input_files = [model] if model.is_file() else list(model.glob("*.mdl"))
    before = {path: hashlib.sha256(path.read_bytes()).digest() for path in input_files}
    command = [runner, model, output, resource, "group" if group else ""]
    if lock:
        command.append(lock)
    process = subprocess.run([str(value) for value in command], capture_output=True, timeout=50, check=True)
    logs = "\n".join(path.read_text(encoding="utf-8") for path in output.glob("*.txt"))
    actual = process.stdout.decode("ascii").strip()
    assert actual == status, (model, actual, status, logs)
    for marker in markers:
        assert marker in logs, (model, marker, logs)
    assert before == {path: hashlib.sha256(path.read_bytes()).digest() for path in input_files}
    assert not list(output.rglob(".crowbar-*.tmp"))
    print(f"{model.stem}: {status}")
    return output


def run_cases(runner, root):
    model = write_model(root / "normal" / "normal.mdl")
    write_pair(model)
    model.with_suffix(".dx11.vtx").write_bytes(b"\x07")
    output = checked_run(runner, root, model, "Success", "不足 36 字节", group=True)
    assert "triangles" in (output / "triangle.smd").read_text(encoding="utf-8")
    assert (output / "normal_lod.qci").is_file()

    model = write_model(root / "split" / "models" / "split.mdl", version=53)
    model.with_suffix(".vvd").write_bytes(b"IDSV")
    write_pair(model.parent / "hidden" / "mesh.mdl")
    checked_run(runner, root, model, "Success", "声明版本 v53", "读取布局 v49", "不足 64 字节")

    model = write_model(root / "altered" / "altered.mdl", version=1500000001)
    write_pair(model)
    checked_run(runner, root, model, "Success", "声明版本 v1500000001", "读取布局 v49")

    model = write_model(root / "missing" / "missing.mdl")
    output = checked_run(runner, root, model, "PartialSuccess", "未找到结构一致")
    assert "nodes" in (output / "missing_skeleton.smd").read_text(encoding="utf-8")
    assert not (output / "triangle.smd").exists()

    model = write_model(root / "checksum" / "checksum.mdl")
    write_pair(model, checksum=123)
    checked_run(runner, root, model, "PartialSuccess", "校验和与 MDL 不符")

    model = write_model(root / "bad-index" / "bad-index.mdl")
    write_pair(model, broken_vtx=True)
    checked_run(runner, root, model, "PartialSuccess", "VTX 索引超出")

    model = write_model(root / "ambiguous" / "ambiguous.mdl")
    home = root / "resources"
    write_pair(home / "mesh.mdl")
    write_pair(home / "hidden" / "mesh.mdl")
    checked_run(runner, root, model, "PartialSuccess", "有歧义", resource=home)
    checked_run(runner, root, model, "Success", "使用已验证的配套", resource=home / "hidden")

    model = write_model(root / "names" / "names.mdl", abnormal_names=True)
    write_pair(model)
    output = checked_run(runner, root, model, "Success", "bone[1]", "bodygroup[0]")
    qc_text = (output / "names.qc").read_text(encoding="utf-8")
    assert '"temp.smd"' in qc_text
    mesh_text = (output / "temp.smd").read_text(encoding="utf-8")
    assert '1 "bone_recovered_001" 0' in mesh_text

    model = write_model(root / "string" / "string.mdl", unterminated=True)
    write_pair(model)
    checked_run(runner, root, model, "PartialSuccess", "缺少字符串终止符", "bone[1]")

    model = write_model(root / "locked" / "locked.mdl")
    write_pair(model)
    output = root / "output-locked"
    output.mkdir()
    qc = output / "locked.qc"
    qc.write_text("previous QC", encoding="ascii")
    checked_run(runner, root, model, "PartialSuccess", "写出失败", lock=qc)
    assert qc.read_text(encoding="ascii") == "previous QC"
    assert (output / "triangle.smd").is_file()

    model = write_model(root / "qci" / "qci.mdl")
    write_pair(model)
    output = root / "output-qci"
    output.mkdir()
    (output / "qci_lod.qci").mkdir()
    checked_run(runner, root, model, "PartialSuccess", "qci_lod.qci", group=True)
    assert not (output / "qci.qc").exists()
    assert (output / "triangle.smd").is_file()

    model = write_model(root / "missing-ref" / "missing-ref.mdl")
    write_pair(model)
    output = root / "output-missing-ref"
    output.mkdir()
    (output / "triangle.smd").mkdir()
    checked_run(runner, root, model, "PartialSuccess", "本次未生成的 QC 引用")
    assert (output / "missing-ref.qc").is_file()

    folder = root / "batch"
    truncated = write_model(folder / "a_truncated.mdl")
    truncated.write_bytes(truncated.read_bytes()[:200])
    model = write_model(folder / "b_normal.mdl")
    write_pair(model)
    unknown = folder / "c_unsupported.mdl"
    unknown.write_bytes(b"IDST\x05\x00\x00\x00")
    output = checked_run(runner, root, folder, "PartialSuccess", "失败 1", "成功 1", "不支持 1")
    assert (output / "b_normal.qc").is_file()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("crowbar", type=Path, help="Path to the built Crowbar-TPP.exe")
    args = parser.parse_args()
    executable = args.crowbar.resolve(strict=True)
    csc = Path(r"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe")
    with tempfile.TemporaryDirectory(prefix="Crowbar-compat-test-") as temporary:
        root = Path(temporary)
        shutil.copyfile(executable, root / executable.name)
        runner = root / "DecompileSmoke.exe"
        subprocess.run([str(csc), "/nologo", "/platform:x86", f"/reference:{executable}",
                        f"/out:{runner}", str(Path(__file__).with_name("DecompileSmoke.cs"))], check=True)
        run_cases(runner, root)


if __name__ == "__main__":
    main()

