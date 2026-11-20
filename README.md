# Crowbar-TPP

面向 GoldSource 和 Source 1 的 Windows 模组工具，可反编译 MDL、调用游戏的 StudioMDL 编译 QC，并管理游戏资源包。**TPP-009** 是本次公开版本，对应旧归档中的 TPP-09。

## 下载与运行

在 [TPP-009 发布页](https://github.com/3163669737z-lgtm/Crowbar-TPP/releases/tag/TPP-009) 下载 `Crowbar-TPP-TPP-009-Windows-x86.zip`。解压整个目录后运行 `Crowbar-TPP.exe`。这是便携包，运行需要 Windows 和 .NET Framework 4.8。请保留同目录的 `Crowbar-TPP.exe.config`，它用于高 DPI 设置。编译和预览功能依赖对应游戏或 SDK 的外部工具。

发布页也附有同一标签的 `Crowbar-TPP-TPP-009-source.zip` 和 SHA-256 校验文件。源码可直接在本仓库查看。

## 上游与贡献

- **Crowbar** 由 **ZeqMacaw** 创建，是原始项目。对照版本为 [Crowbar v0.74](https://github.com/ZeqMacaw/Crowbar/tree/0d46f3b6a694b74453db407c72c12a9685d8eb1d)。原项目主页为 [Steam 社区](https://steamcommunity.com/groups/CrowbarTool)。
- **Crowbar-Starfelll** 由 **Starfelll** 修改，是 Crowbar-TPP 的直接上游。对照快照为 [946e34e](https://github.com/Starfelll/Crowbar-Starfelll/tree/946e34e9a1d927aece5f621800f383e661ed837e)。
- **Crowbar-TPP** 的代码修改由 **ChatGPT（OpenAI）** 实现。**TPP** 负责监督修改进度和状况。上游贡献沿用原有署名。

分别查看[相对 Crowbar v0.74](CHANGELOG.md#相对-crowbar-v074)和[相对 Crowbar-Starfelll](CHANGELOG.md#相对-crowbar-starfelll)的变化。

项目沿用 [CC BY-SA 3.0 Unported](https://creativecommons.org/licenses/by-sa/3.0/) 许可。原始署名和转载条件见 [LICENSE.txt](LICENSE.txt)。

## 从源码构建

安装 Visual Studio 的「.NET 桌面开发」工作负载和 .NET Framework 4.8 Developer Pack。在 Developer Command Prompt 中构建 Release x86 主程序。

```powershell
msbuild Crowbar/Crowbar.vbproj /t:Build /p:Configuration=Release /p:Platform=x86
```

产物位于 `Crowbar/bin/x86/Release`。主项目使用源码树内的嵌入式辅助程序。如果还要单独构建 Launcher 和 SteamPipe 项目，需要 .NET Framework 4.0 参考程序集。

构建后可在 Windows 上用 Python 3 运行仓库自带的测试。

```powershell
py Tests/run_compatibility.py Crowbar/bin/x86/Release/Crowbar-TPP.exe
py Tests/run_vtf_header.py Crowbar/bin/x86/Release/Crowbar-TPP.exe
```

## MDL 反编译兼容性

在「反编译」中选择原始模型，版本选项保持「不覆盖版本」即可自动识别。带有外置动画、纹理或网格数据的模型需要在原目录中保留配套文件。

| 格式 | 新增支持 | 验证 |
| --- | --- | --- |
| Half-Life Alpha v4（`IDST`） | QC、参考网格与骨骼动画 SMD、嵌入 BMP。使用首个可用动画帧作为参考姿态。 | 8 个原始模型。动画旋转和根骨骼位移按样本布局解码，尚无原引擎逐帧对照。 |
| 007: Nightfire PC v14（`MDLZ`） | QC、参考网格与骨骼动画 SMD，含子网格材质与权重。游戏的外部 PNG 需从游戏资源中取得。 | PC 试玩版 273 个文件，含 61 个空占位文件。其余文件的 137,310 个三角形和材质顺序均与原模型数据一致。 |
| Half-Life Dreamcast v10（`IDST`） | 将嵌入的 RGB565 PowerVR 纹理导出为 BMP，支持线性、Morton 排列和 VQ。颜色超过 256 种时使用 RGB332 调色板。 | 3 种纹理排列的构造样本。 |
| Xash3D v10（`IDST`） | 按格式标志读取绑定矩阵、每顶点最多四个骨骼权重及 16 位浮点 UV。 | 带两个骨骼的构造样本。 |

格式参考与样本来源见 [Half-Life Alpha 模型集](https://gamebanana.com/mods/639190)、[Nightfire 试玩版](https://archive.org/details/007NightfireDemo)、[Nightfire MDL 读取器](https://github.com/noodlecollie/nightfire-model-compiler)、[PVR2MDL](https://github.com/supadupaplex/PVR2MDL) 和 [Xash3D FWGS MDL 导出器](https://github.com/FWGS/xash3d-fwgs/blob/master/utils/mdldec/smd.c)。

上游已经包含部分早期格式的读取器。此处列的是 TPP-009 完善且有样本覆盖的格式。Half-Life Beta v9、Counter-Strike Xbox 的骨架变体、Zeno Clash 的 VVD v5，以及 Apex Legends RMDL/RPak 和 Titanfall Xbox 360 v63 仍需要各自的格式适配与样本验证。

### 《求生之路 2》分离网格模组

部分模组的 MDL 标记为 v53，内部使用 Source v49 布局。Crowbar 根据文件头选择读取器，并用 MDL 的内部路径、校验和及数据表定位完整的 VVD 和 VTX。

解压模组时保留 `models` 目录结构，在「反编译」中选择主 MDL，版本设为「不覆盖版本」。用于验证的模组中，角色主文件是 `survivor_mechanic.mdl`，手臂主文件是 `v_arms_mechanic_new.mdl`。`1.mdl` 和 `3.mdl` 是网格配套文件。

网格文件默认使用 MDL 中可用的文件名。只有扩展名的子模型改用部件名，例如 `body.smd`、`hair.smd`。文件重名时加数字后缀，空槽继续导出为 `blank`。含换行、控制字符或引号的骨骼名称会替换为基于索引的名称，QC、SMD、VTA 和 VRD 使用同一套骨骼名称。缺失的面部表情名称会按索引生成。眼睑和成对表情遵循 StudioMDL 的命名规则。

动画 SMD 优先使用 MDL 动画描述表里的名称，保留大小写和 `@`。名称字段为空且只关联一个实际序列时，文件名采用序列名，例如 `CustomModel.smd`、`IDLE.smd`。QC 中的动画标识仍单独命名，以便与同名序列区分。`_corrective_animation.smd` 是生成的辅助文件。

为兼容原版 L4D2 编译器，此类模组的 `$bodygrouppreset` 信息以注释保留。实测手臂模型可用原版 `studiomdl.exe` 重新编译。角色 MDL 的材质表有 53 项，原版编译器在读取网格时达到 32 个材质的上限，使用 `nekomdl.exe` 可以完成编译。

`faceeffect` 的 5 个选项各自导出带表情的 DMX，并在 QC 的 `$bodygroup` 中按原顺序引用。仍会同时导出各选项的 SMD。用 NekoMDL 重新编译后，5 个选项的顶点位置、三角形和每个选项的 4 个表情目标均通过回读检查。角色重编译产物包含 53 项材质。其他部件的 VTA 仍可能报告顶点未匹配。QC 中的 `$includemodel` 及材质引用仍依赖对应的游戏或模组资源。

### VTF 文件头修复

在「反编译」中选择「修复 VTF 文件头…」，可检查已提取的 PC VTF 7.0–7.2 二维纹理。程序先核对缩略图和完整像素数据范围，再决定是否提供头长度修复副本。参见 [防反编译与加密复查](防反编译与加密复查.md)。
