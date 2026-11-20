# Crowbar-TPP 变更对照

Crowbar-TPP 的直接上游是 Starfelll 的 Crowbar-Starfelll。该分支基于 ZeqMacaw 的 Crowbar v0.74。本页以 [Crowbar v0.74 的源码](https://github.com/ZeqMacaw/Crowbar/tree/0d46f3b6a694b74453db407c72c12a9685d8eb1d)和 [Crowbar-Starfelll 的 946e34e 快照](https://github.com/Starfelll/Crowbar-Starfelll/tree/946e34e9a1d927aece5f621800f383e661ed837e)为基准。

## 相对 Crowbar v0.74

| 范围 | Starfelll 已做的修改 | Crowbar-TPP 后续变化 |
| --- | --- | --- |
| 桌面界面 | 调整编译、反编译和关于页的布局，编译与反编译日志采用深色配色。 | 补全简体中文界面、菜单、提示和进度日志，调整按钮、标签页和表格样式，适配每显示器高 DPI。应用改名为 Crowbar-TPP 并使用紫色图标。 |
| 工具调用 | 按 UTF-8 读取编译器输出，适配 HLMV++ 预览。 | 沿用外部编译器和预览器接口，补齐中文反馈，兼容原 Starfelll 设置目录。 |
| 反编译 | 改进 v49 的 UTF-8 名称读取，合并 corrective animation 的输出与引用。 | 延续这些改动，进一步处理中文及异常名称、旧 MDL 格式、L4D2 分离网格与导出、批量恢复和 VTF 文件头检查。具体功能见下一节。 |

[查看 Crowbar v0.74 与 Crowbar-TPP 的代码差异](https://github.com/3163669737z-lgtm/Crowbar-TPP/compare/0d46f3b6a694b74453db407c72c12a9685d8eb1d...v1.0.0)

## 相对 Crowbar-Starfelll

- **汉化与高 DPI。** 补全主要功能页、下拉菜单、提示和日志的简体中文。保留 MDL、QC、SMD 等格式名和命令标识。主程序由 .NET Framework 4.0 升至 4.8，以 PerMonitorV2 适配多显示器缩放。
- **MDL 名称。** 在模型名称等字段处理 UTF-8 和 GB18030，按字节读取固定长度字段。对空名、异常字符和 Windows 保留文件名使用安全的导出名称，让 QC 引用与写出的文件一致。
- **旧版和衍生 MDL。** 完善 Half-Life Alpha v4 与 007: Nightfire PC v14 的网格、动画和 QC 导出。加入 Half-Life Dreamcast v10 的 PowerVR 内嵌纹理导出，以及 Xash3D v10 的绑定矩阵、多骨骼权重和半精度 UV 读取。支持范围见 [MDL 兼容性表](README.md#mdl-反编译兼容性)。
- **L4D2 分离网格。** 区分声明为 v53、内部使用 v49 布局的模型与真正的 v53 模型。按内部路径、校验和及数据表寻找匹配的 VVD 和 VTX，支持指定配套资源目录。候选有冲突时列出位置，便于手动缩小范围。
- **L4D2 导出和重编译。** 在 QC、SMD、VTA、VRD 中同步异常骨骼名的替代名称，为缺名网格和表情生成有效名称。动画 SMD 优先使用原名，缺名且只关联一个序列时可借用序列名。对支持 DMX 导出的多选项表情体组，按原顺序保留各选项，同时导出 SMD。
- **反编译容错。** 对 Source v49 增加文件结构及表边界校验。外置网格缺失或不可用时，可按选项导出还能读取的骨架、动画和元数据。批量任务按模型记录结果并继续处理其余文件。QC/QCI 写出路径增加本地引用检查及安全写入。
- **VTF 文件头修复。** 通过「反编译」→「修复 VTF 文件头…」检查已经提取的 PC VTF 7.0–7.2 二维纹理。只有缩略图和完整像素数据均符合布局时，才允许另存仅修正头长度字段的副本。

[查看 Crowbar-Starfelll 与 Crowbar-TPP 的代码差异](https://github.com/3163669737z-lgtm/Crowbar-TPP/compare/946e34e9a1d927aece5f621800f383e661ed837e...v1.0.0)

Crowbar-TPP 改动由 **ChatGPT（OpenAI）** 实现，**TPP** 负责监督修改进度和状况。Crowbar 原作归功于 **ZeqMacaw**，直接上游改动归功于 **Starfelll**。
