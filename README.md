# Crowbar-TPP

面向 GoldSource 和 Source 1 的 Windows 模组工具，可反编译 MDL、调用游戏的 StudioMDL 编译 QC，并管理游戏资源包。

Crowbar-TPP is a Windows MDL decompiler and modding toolkit for GoldSource and Source 1. It adds Left 4 Dead 2 model compatibility, VTF header repair, Simplified Chinese UI, and per-monitor DPI scaling.

## 下载与运行

在 [GitHub 发布页](https://github.com/3163669737z-lgtm/Crowbar-TPP/releases/latest) 下载 Windows x86 便携包。解压整个目录后运行 `Crowbar-TPP.exe`。运行需要 Windows 和 .NET Framework 4.8。请保留同目录的 `Crowbar-TPP.exe.config`，它用于高 DPI 设置。编译和预览功能依赖对应游戏或 SDK 的外部工具。

发布页也附有同一标签的源码 ZIP 和 SHA-256 校验文件。源码可直接在本仓库查看。

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

| 格式 | 新增支持 |
| --- | --- |
| Half-Life Alpha v4（`IDST`） | 导出 QC、参考网格与骨骼动画 SMD、嵌入纹理 BMP。使用首个可用动画帧作为参考姿态。 |
| 007: Nightfire PC v14（`MDLZ`） | 导出 QC、参考网格与骨骼动画 SMD，保留子网格材质与权重。外部 PNG 需从游戏资源中取得。 |
| Half-Life Dreamcast v10（`IDST`） | 将嵌入的 RGB565 PowerVR 纹理导出为 BMP，支持线性、Morton 排列和 VQ。颜色超过 256 种时使用 RGB332 调色板。 |
| Xash3D v10（`IDST`） | 按格式标志读取绑定矩阵、每顶点最多四个骨骼权重及 16 位浮点 UV。 |

格式参考包括 [Nightfire MDL 读取器](https://github.com/noodlecollie/nightfire-model-compiler)、[PVR2MDL](https://github.com/supadupaplex/PVR2MDL) 和 [Xash3D FWGS MDL 导出器](https://github.com/FWGS/xash3d-fwgs/blob/master/utils/mdldec/smd.c)。

Half-Life Beta v9、Counter-Strike Xbox 的骨架变体、Zeno Clash 的 VVD v5、Apex Legends RMDL/RPak 和 Titanfall Xbox 360 v63 仍需要各自的格式适配。

### 《求生之路 2》分离网格与导出

- **分离网格识别。** 自动识别声明为 v53 但采用 Source v49 布局的 MDL，按内部路径、校验和及数据表匹配外置 VVD 和 VTX，也支持指定配套资源目录。
- **网格、骨骼和表情命名。** 保留可用的原始网格名称，缺名时使用部件名，重名时添加后缀。QC、SMD、VTA 和 VRD 共用修正后的骨骼名称，缺失的表情名称按索引生成。
- **动画文件命名。** 动画 SMD 优先采用 MDL 中的原名。缺名且只关联一个序列时，可借用该序列名。QC 中的动画标识单独命名。
- **体组与重编译。** 对支持 DMX 导出的多选项表情体组，按原顺序在 QC 的 `$bodygroup` 中引用带表情的 DMX，同时保留 SMD。分离网格模型的 `$bodygrouppreset` 以注释保留，便于使用原版 L4D2 编译器。

### VTF 文件头修复

在「反编译」中选择「修复 VTF 文件头…」，可检查已提取的 PC VTF 7.0–7.2 二维纹理。缩略图和完整像素数据符合布局时，可另存仅修正头长度字段的副本。
