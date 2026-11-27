# Crowbar-TPP

[下载最新版本](https://github.com/3163669737z-lgtm/Crowbar-TPP/releases/latest) · [版本变更对照](CHANGELOG.md) · [许可协议](LICENSE.txt)

## 主要改动

- **简体中文与高 DPI 适配**：补全中文界面、菜单与日志输出，升级至 **.NET Framework 4.8** 并支持每显示器高 DPI 缩放。
- **Left 4 Dead 2 分离网格与导出**：自动识别 v53 声明、v49 布局的分离网格并匹配外置 **VVD** / **VTX**；修正缺失、重名或异常的网格、骨骼、表情与动画名称；多选项表情体组导出 **DMX** 并保留 **SMD**，注释保留 `$bodygrouppreset` 便于 **StudioMDL** / **NekoMDL** 重编译。
- **Source v49 Delta 动作还原**：反编译含 **Delta** 增量序列的模型时自动还原为绝对姿态 **SMD**，可直接导入 **Blender** 编辑，并生成 `_corrective_animation.smd` 以支持通过 **StudioMDL** / **NekoMDL** 无损重编译。
- **VTF 文件头修复**：在「反编译」中内置「修复 VTF 文件头…」，检查并另存修正头长度后的 **PC VTF 7.0–7.2** 二维纹理副本。
- **新增 MDL 反编译格式**：

| 格式 | 新增支持 |
| --- | --- |
| Half-Life Alpha v4 | 导出 QC、参考网格、骨骼动画 SMD 及内嵌 BMP 纹理 |
| 007: Nightfire PC v14 | 导出 QC、参考网格与骨骼动画 SMD，保留子网格材质与权重 |
| Half-Life Dreamcast v10 | 导出内嵌 PowerVR 纹理为 BMP |
| Xash3D v10 | 读取绑定矩阵、多骨骼权重及 16 位浮点 UV |

## 上游与许可

- 原作：[Crowbar v0.74](https://github.com/ZeqMacaw/Crowbar/tree/0d46f3b6a694b74453db407c72c12a9685d8eb1d) — ZeqMacaw · [Steam 社区](https://steamcommunity.com/groups/CrowbarTool)
- 上游：[Crowbar-Starfelll](https://github.com/Starfelll/Crowbar-Starfelll/tree/946e34e9a1d927aece5f621800f383e661ed837e) — Starfelll
- 贡献：代码修改由 ChatGPT 实现，TPP 负责监督进度与状况，详见 [CHANGELOG.md](CHANGELOG.md)
- 许可：[CC BY-SA 3.0 Unported](https://creativecommons.org/licenses/by-sa/3.0/)，详见 [LICENSE.txt](LICENSE.txt)
