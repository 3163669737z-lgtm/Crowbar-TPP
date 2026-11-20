<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DecompileUserControl
	Inherits BaseUserControl

	''UserControl overrides dispose to clean up the component list.
	'<System.Diagnostics.DebuggerNonUserCode()>
	'Protected Overrides Sub Dispose(ByVal disposing As Boolean)
	'	Try
	'		If disposing AndAlso components IsNot Nothing Then
	'			components.Dispose()
	'		End If
	'	Finally
	'		MyBase.Dispose(disposing)
	'	End Try
	'End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.DebugInfoCheckBox = New System.Windows.Forms.CheckBox()
        Me.LogFileCheckBox = New System.Windows.Forms.CheckBox()
        Me.DeclareSequenceQciCheckBox = New System.Windows.Forms.CheckBox()
        Me.FormatForStricterImportersCheckBox = New System.Windows.Forms.CheckBox()
        Me.UseMixedCaseForKeywordsCheckBox = New System.Windows.Forms.CheckBox()
        Me.RemovePathFromMaterialFileNamesCheckBox = New System.Windows.Forms.CheckBox()
        Me.UseNonValveUvConversionCheckBox = New System.Windows.Forms.CheckBox()
        Me.OverrideMdlVersionLabel = New System.Windows.Forms.Label()
        Me.OverrideMdlVersionComboBox = New System.Windows.Forms.ComboBox()
        Me.PrefixMeshFileNamesWithModelNameCheckBox = New System.Windows.Forms.CheckBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DecompileComboBox = New System.Windows.Forms.ComboBox()
        Me.MdlPathFileNameTextBox = New Crowbar.TextBoxEx()
        Me.BrowseForMdlPathFolderOrFileNameButton = New System.Windows.Forms.Button()
        Me.GotoMdlButton = New System.Windows.Forms.Button()
        Me.GotoOutputPathButton = New System.Windows.Forms.Button()
        Me.BrowseForOutputPathButton = New System.Windows.Forms.Button()
        Me.OutputPathTextBox = New Crowbar.TextBoxEx()
        Me.OutputSubfolderTextBox = New Crowbar.TextBoxEx()
        Me.OutputPathComboBox = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.UseDefaultOutputSubfolderButton = New System.Windows.Forms.Button()
        Me.Options_LogSplitContainer = New Crowbar.SplitContainerEx()
        Me.DecompileOptionsPanel = New System.Windows.Forms.Panel()
        Me.ReCreateFilesGroupBox = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox = New System.Windows.Forms.CheckBox()
        Me.SkinFamilyOnSingleLineCheckBox = New System.Windows.Forms.CheckBox()
        Me.TextureBmpFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.DecompileOptionsUseDefaultsButton = New System.Windows.Forms.Button()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.IncludeDefineBoneLinesCheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupIntoQciFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.PlaceInAnimsSubfolderCheckBox = New System.Windows.Forms.CheckBox()
        Me.LodMeshSmdFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.ProceduralBonesVrdFileCheckBox = New System.Windows.Forms.CheckBox()
        Me.BoneAnimationSmdFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.VertexAnimationVtaFileCheckBox = New System.Windows.Forms.CheckBox()
        Me.PhysicsMeshSmdFileCheckBox = New System.Windows.Forms.CheckBox()
        Me.ReferenceMeshSmdFileCheckBox = New System.Windows.Forms.CheckBox()
        Me.QcFileCheckBox = New System.Windows.Forms.CheckBox()
        Me.OptionsGroupBox = New System.Windows.Forms.GroupBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.FolderForEachModelCheckBox = New System.Windows.Forms.CheckBox()
        Me.DecompilerLogTextBox = New Crowbar.RichTextBoxEx()
        Me.DecompileButtonsPanel = New System.Windows.Forms.Panel()
        Me.DecompileButton = New System.Windows.Forms.Button()
        Me.SkipCurrentModelButton = New System.Windows.Forms.Button()
        Me.CancelDecompileButton = New System.Windows.Forms.Button()
        Me.UseAllInCompileButton = New System.Windows.Forms.Button()
        Me.MeshResourceFolderButton = New System.Windows.Forms.Button()
        Me.VtfHeaderButton = New System.Windows.Forms.Button()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.DecompiledFilesComboBox = New System.Windows.Forms.ComboBox()
        Me.UseInEditButton = New System.Windows.Forms.Button()
        Me.UseInCompileButton = New System.Windows.Forms.Button()
        Me.GotoDecompiledFileButton = New System.Windows.Forms.Button()
        Me.Panel2.SuspendLayout()
        CType(Me.Options_LogSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Options_LogSplitContainer.Panel1.SuspendLayout()
        Me.Options_LogSplitContainer.Panel2.SuspendLayout()
        Me.Options_LogSplitContainer.SuspendLayout()
        Me.DecompileOptionsPanel.SuspendLayout()
        Me.ReCreateFilesGroupBox.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.OptionsGroupBox.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.DecompileButtonsPanel.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'DebugInfoCheckBox
        '
        Me.DebugInfoCheckBox.AutoSize = True
        Me.DebugInfoCheckBox.Location = New System.Drawing.Point(4, 145)
        Me.DebugInfoCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DebugInfoCheckBox.Name = "DebugInfoCheckBox"
        Me.DebugInfoCheckBox.Size = New System.Drawing.Size(213, 20)
        Me.DebugInfoCheckBox.TabIndex = 14
        Me.DebugInfoCheckBox.Text = "反编译信息注释和文件"
        Me.ToolTip1.SetToolTip(Me.DebugInfoCheckBox, "写入包含反编译信息的注释和附加文件，便于调试。")
        Me.DebugInfoCheckBox.UseVisualStyleBackColor = True
        '
        'LogFileCheckBox
        '
        Me.LogFileCheckBox.AutoSize = True
        Me.LogFileCheckBox.Location = New System.Drawing.Point(4, 117)
        Me.LogFileCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.LogFileCheckBox.Name = "LogFileCheckBox"
        Me.LogFileCheckBox.Size = New System.Drawing.Size(116, 20)
        Me.LogFileCheckBox.TabIndex = 13
        Me.LogFileCheckBox.Text = "将日志写入文件"
        Me.ToolTip1.SetToolTip(Me.LogFileCheckBox, "将反编译日志写入文件。")
        Me.LogFileCheckBox.UseVisualStyleBackColor = True
        '
        'DeclareSequenceQciCheckBox
        '
        Me.DeclareSequenceQciCheckBox.AutoSize = True
        Me.DeclareSequenceQciCheckBox.Location = New System.Drawing.Point(4, 202)
        Me.DeclareSequenceQciCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DeclareSequenceQciCheckBox.Name = "DeclareSequenceQciCheckBox"
        Me.DeclareSequenceQciCheckBox.Size = New System.Drawing.Size(163, 20)
        Me.DeclareSequenceQciCheckBox.TabIndex = 40
        Me.DeclareSequenceQciCheckBox.Text = "$DeclareSequence QCI file"
        Me.ToolTip1.SetToolTip(Me.DeclareSequenceQciCheckBox, "为 MDL 文件中的每个序列写入包含 $DeclareSequence 行的 QCI 文件。有助于让多人游戏中的序列保持正确顺序。")
        Me.DeclareSequenceQciCheckBox.UseVisualStyleBackColor = True
        '
        'FormatForStricterImportersCheckBox
        '
        Me.FormatForStricterImportersCheckBox.AutoSize = True
        Me.FormatForStricterImportersCheckBox.Location = New System.Drawing.Point(4, 60)
        Me.FormatForStricterImportersCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.FormatForStricterImportersCheckBox.Name = "FormatForStricterImportersCheckBox"
        Me.FormatForStricterImportersCheckBox.Size = New System.Drawing.Size(173, 20)
        Me.FormatForStricterImportersCheckBox.TabIndex = 12
        Me.FormatForStricterImportersCheckBox.Text = "面向更严格导入器的格式"
        Me.ToolTip1.SetToolTip(Me.FormatForStricterImportersCheckBox, "以某些导入器所期望的格式写入反编译文件，但这种格式不太易读。")
        Me.FormatForStricterImportersCheckBox.UseVisualStyleBackColor = True
        '
        'UseMixedCaseForKeywordsCheckBox
        '
        Me.UseMixedCaseForKeywordsCheckBox.AutoSize = True
        Me.UseMixedCaseForKeywordsCheckBox.Location = New System.Drawing.Point(23, 117)
        Me.UseMixedCaseForKeywordsCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.UseMixedCaseForKeywordsCheckBox.Name = "UseMixedCaseForKeywordsCheckBox"
        Me.UseMixedCaseForKeywordsCheckBox.Size = New System.Drawing.Size(223, 20)
        Me.UseMixedCaseForKeywordsCheckBox.TabIndex = 42
        Me.UseMixedCaseForKeywordsCheckBox.Text = "关键字使用混合大小写（Source）"
        Me.ToolTip1.SetToolTip(Me.UseMixedCaseForKeywordsCheckBox, "$CommandLikeThis instead of $commandlikethis")
        Me.UseMixedCaseForKeywordsCheckBox.UseVisualStyleBackColor = True
        '
        'RemovePathFromMaterialFileNamesCheckBox
        '
        Me.RemovePathFromMaterialFileNamesCheckBox.AutoSize = True
        Me.RemovePathFromMaterialFileNamesCheckBox.Location = New System.Drawing.Point(23, 174)
        Me.RemovePathFromMaterialFileNamesCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.RemovePathFromMaterialFileNamesCheckBox.Name = "RemovePathFromMaterialFileNamesCheckBox"
        Me.RemovePathFromMaterialFileNamesCheckBox.Size = New System.Drawing.Size(224, 20)
        Me.RemovePathFromMaterialFileNamesCheckBox.TabIndex = 41
        Me.RemovePathFromMaterialFileNamesCheckBox.Text = "从材质文件名中移除路径"
        Me.ToolTip1.SetToolTip(Me.RemovePathFromMaterialFileNamesCheckBox, "即使在 SMD 中存储了路径，也只写入文件名。这可能会与 QC 文件中的 $CDMaterials 产生问题。")
        Me.RemovePathFromMaterialFileNamesCheckBox.UseVisualStyleBackColor = True
        '
        'UseNonValveUvConversionCheckBox
        '
        Me.UseNonValveUvConversionCheckBox.AutoSize = True
        Me.UseNonValveUvConversionCheckBox.Location = New System.Drawing.Point(23, 202)
        Me.UseNonValveUvConversionCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.UseNonValveUvConversionCheckBox.Name = "UseNonValveUvConversionCheckBox"
        Me.UseNonValveUvConversionCheckBox.Size = New System.Drawing.Size(253, 20)
        Me.UseNonValveUvConversionCheckBox.TabIndex = 44
        Me.UseNonValveUvConversionCheckBox.Text = "使用非 Valve UV 转换（GoldSource）"
        Me.ToolTip1.SetToolTip(Me.UseNonValveUvConversionCheckBox, "[ u=s/width ] and [ v=1-(t/height) ] instead of Valve's [ u=s/(width-1) ] and [ v" &
        "=1-(t/(height-1)) ]")
        Me.UseNonValveUvConversionCheckBox.UseVisualStyleBackColor = True
        '
        'OverrideMdlVersionLabel
        '
        Me.OverrideMdlVersionLabel.AutoSize = True
        Me.OverrideMdlVersionLabel.Location = New System.Drawing.Point(4, 249)
        Me.OverrideMdlVersionLabel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.OverrideMdlVersionLabel.Name = "OverrideMdlVersionLabel"
        Me.OverrideMdlVersionLabel.Size = New System.Drawing.Size(124, 16)
        Me.OverrideMdlVersionLabel.TabIndex = 46
        Me.OverrideMdlVersionLabel.Text = "覆盖 MDL 版本："
        Me.ToolTip1.SetToolTip(Me.OverrideMdlVersionLabel, "基于此所选版本反编译，而不是 MDL 文件中存储的版本。")
        '
        'OverrideMdlVersionComboBox
        '
        Me.OverrideMdlVersionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.OverrideMdlVersionComboBox.FormattingEnabled = True
        Me.OverrideMdlVersionComboBox.Location = New System.Drawing.Point(144, 245)
        Me.OverrideMdlVersionComboBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.OverrideMdlVersionComboBox.Name = "OverrideMdlVersionComboBox"
        Me.OverrideMdlVersionComboBox.Size = New System.Drawing.Size(128, 24)
        Me.OverrideMdlVersionComboBox.TabIndex = 45
        Me.ToolTip1.SetToolTip(Me.OverrideMdlVersionComboBox, "基于此所选版本反编译，而不是 MDL 文件中存储的版本。")
        '
        'PrefixMeshFileNamesWithModelNameCheckBox
        '
        Me.PrefixMeshFileNamesWithModelNameCheckBox.AutoSize = True
        Me.PrefixMeshFileNamesWithModelNameCheckBox.Location = New System.Drawing.Point(4, 32)
        Me.PrefixMeshFileNamesWithModelNameCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.PrefixMeshFileNamesWithModelNameCheckBox.Name = "PrefixMeshFileNamesWithModelNameCheckBox"
        Me.PrefixMeshFileNamesWithModelNameCheckBox.Size = New System.Drawing.Size(237, 20)
        Me.PrefixMeshFileNamesWithModelNameCheckBox.TabIndex = 47
        Me.PrefixMeshFileNamesWithModelNameCheckBox.Text = "在网格文件名前加模型名前缀"
        Me.ToolTip1.SetToolTip(Me.PrefixMeshFileNamesWithModelNameCheckBox, "避免文件名冲突。")
        Me.PrefixMeshFileNamesWithModelNameCheckBox.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.DecompileComboBox)
        Me.Panel2.Controls.Add(Me.MdlPathFileNameTextBox)
        Me.Panel2.Controls.Add(Me.BrowseForMdlPathFolderOrFileNameButton)
        Me.Panel2.Controls.Add(Me.GotoMdlButton)
        Me.Panel2.Controls.Add(Me.GotoOutputPathButton)
        Me.Panel2.Controls.Add(Me.BrowseForOutputPathButton)
        Me.Panel2.Controls.Add(Me.OutputPathTextBox)
        Me.Panel2.Controls.Add(Me.OutputSubfolderTextBox)
        Me.Panel2.Controls.Add(Me.OutputPathComboBox)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.UseDefaultOutputSubfolderButton)
        Me.Panel2.Controls.Add(Me.Options_LogSplitContainer)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(2)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(905, 660)
        Me.Panel2.TabIndex = 8
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(4, 10)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(75, 16)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "MDL 输入："
        '
        'DecompileComboBox
        '
        Me.DecompileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.DecompileComboBox.FormattingEnabled = True
        Me.DecompileComboBox.Location = New System.Drawing.Point(85, 5)
        Me.DecompileComboBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DecompileComboBox.Name = "DecompileComboBox"
        Me.DecompileComboBox.Size = New System.Drawing.Size(163, 24)
        Me.DecompileComboBox.TabIndex = 1
        '
        'MdlPathFileNameTextBox
        '
        Me.MdlPathFileNameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MdlPathFileNameTextBox.CueBannerText = ""
        Me.MdlPathFileNameTextBox.Location = New System.Drawing.Point(255, 4)
        Me.MdlPathFileNameTextBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MdlPathFileNameTextBox.Name = "MdlPathFileNameTextBox"
        Me.MdlPathFileNameTextBox.Size = New System.Drawing.Size(507, 21)
        Me.MdlPathFileNameTextBox.TabIndex = 1
        '
        'BrowseForMdlPathFolderOrFileNameButton
        '
        Me.BrowseForMdlPathFolderOrFileNameButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BrowseForMdlPathFolderOrFileNameButton.Location = New System.Drawing.Point(770, 4)
        Me.BrowseForMdlPathFolderOrFileNameButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.BrowseForMdlPathFolderOrFileNameButton.Name = "BrowseForMdlPathFolderOrFileNameButton"
        Me.BrowseForMdlPathFolderOrFileNameButton.Size = New System.Drawing.Size(75, 28)
        Me.BrowseForMdlPathFolderOrFileNameButton.TabIndex = 2
        Me.BrowseForMdlPathFolderOrFileNameButton.Text = "浏览..."
        Me.BrowseForMdlPathFolderOrFileNameButton.UseVisualStyleBackColor = True
        '
        'GotoMdlButton
        '
        Me.GotoMdlButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GotoMdlButton.Location = New System.Drawing.Point(852, 4)
        Me.GotoMdlButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GotoMdlButton.Name = "GotoMdlButton"
        Me.GotoMdlButton.Size = New System.Drawing.Size(50, 28)
        Me.GotoMdlButton.TabIndex = 3
        Me.GotoMdlButton.Text = "转到"
        Me.GotoMdlButton.UseVisualStyleBackColor = True
        '
        'GotoOutputPathButton
        '
        Me.GotoOutputPathButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GotoOutputPathButton.Location = New System.Drawing.Point(852, 39)
        Me.GotoOutputPathButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GotoOutputPathButton.Name = "GotoOutputPathButton"
        Me.GotoOutputPathButton.Size = New System.Drawing.Size(50, 28)
        Me.GotoOutputPathButton.TabIndex = 18
        Me.GotoOutputPathButton.Text = "转到"
        Me.GotoOutputPathButton.UseVisualStyleBackColor = True
        '
        'BrowseForOutputPathButton
        '
        Me.BrowseForOutputPathButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BrowseForOutputPathButton.Enabled = False
        Me.BrowseForOutputPathButton.Location = New System.Drawing.Point(770, 39)
        Me.BrowseForOutputPathButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.BrowseForOutputPathButton.Name = "BrowseForOutputPathButton"
        Me.BrowseForOutputPathButton.Size = New System.Drawing.Size(75, 28)
        Me.BrowseForOutputPathButton.TabIndex = 17
        Me.BrowseForOutputPathButton.Text = "浏览..."
        Me.BrowseForOutputPathButton.UseVisualStyleBackColor = True
        '
        'OutputPathTextBox
        '
        Me.OutputPathTextBox.AllowDrop = True
        Me.OutputPathTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OutputPathTextBox.CueBannerText = ""
        Me.OutputPathTextBox.Location = New System.Drawing.Point(255, 39)
        Me.OutputPathTextBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.OutputPathTextBox.Name = "OutputPathTextBox"
        Me.OutputPathTextBox.Size = New System.Drawing.Size(507, 21)
        Me.OutputPathTextBox.TabIndex = 16
        '
        'OutputSubfolderTextBox
        '
        Me.OutputSubfolderTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OutputSubfolderTextBox.CueBannerText = ""
        Me.OutputSubfolderTextBox.Location = New System.Drawing.Point(255, 39)
        Me.OutputSubfolderTextBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.OutputSubfolderTextBox.Name = "OutputSubfolderTextBox"
        Me.OutputSubfolderTextBox.Size = New System.Drawing.Size(507, 21)
        Me.OutputSubfolderTextBox.TabIndex = 20
        Me.OutputSubfolderTextBox.Visible = False
        '
        'OutputPathComboBox
        '
        Me.OutputPathComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.OutputPathComboBox.FormattingEnabled = True
        Me.OutputPathComboBox.Location = New System.Drawing.Point(85, 41)
        Me.OutputPathComboBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.OutputPathComboBox.Name = "OutputPathComboBox"
        Me.OutputPathComboBox.Size = New System.Drawing.Size(163, 24)
        Me.OutputPathComboBox.TabIndex = 14
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(4, 46)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(72, 16)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "输出到："
        '
        'UseDefaultOutputSubfolderButton
        '
        Me.UseDefaultOutputSubfolderButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UseDefaultOutputSubfolderButton.Location = New System.Drawing.Point(770, 39)
        Me.UseDefaultOutputSubfolderButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.UseDefaultOutputSubfolderButton.Name = "UseDefaultOutputSubfolderButton"
        Me.UseDefaultOutputSubfolderButton.Size = New System.Drawing.Size(132, 28)
        Me.UseDefaultOutputSubfolderButton.TabIndex = 19
        Me.UseDefaultOutputSubfolderButton.Text = "使用默认值"
        Me.UseDefaultOutputSubfolderButton.UseVisualStyleBackColor = True
        '
        'Options_LogSplitContainer
        '
        Me.Options_LogSplitContainer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Options_LogSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.Options_LogSplitContainer.Location = New System.Drawing.Point(4, 75)
        Me.Options_LogSplitContainer.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Options_LogSplitContainer.Name = "Options_LogSplitContainer"
        Me.Options_LogSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'Options_LogSplitContainer.Panel1
        '
        Me.Options_LogSplitContainer.Panel1.Controls.Add(Me.DecompileOptionsPanel)
        Me.Options_LogSplitContainer.Panel1MinSize = 45
        '
        'Options_LogSplitContainer.Panel2
        '
        Me.Options_LogSplitContainer.Panel2.Controls.Add(Me.DecompilerLogTextBox)
        Me.Options_LogSplitContainer.Panel2.Controls.Add(Me.DecompileButtonsPanel)
        Me.Options_LogSplitContainer.Panel2.Controls.Add(Me.Panel4)
        Me.Options_LogSplitContainer.Panel2MinSize = 45
        Me.Options_LogSplitContainer.Size = New System.Drawing.Size(898, 581)
        Me.Options_LogSplitContainer.SplitterDistance = 250
        Me.Options_LogSplitContainer.SplitterWidth = 5
        Me.Options_LogSplitContainer.TabIndex = 12
        '
        'DecompileOptionsPanel
        '
        Me.DecompileOptionsPanel.Controls.Add(Me.ReCreateFilesGroupBox)
        Me.DecompileOptionsPanel.Controls.Add(Me.OptionsGroupBox)
        Me.DecompileOptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DecompileOptionsPanel.Location = New System.Drawing.Point(0, 0)
        Me.DecompileOptionsPanel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DecompileOptionsPanel.Name = "DecompileOptionsPanel"
        Me.DecompileOptionsPanel.Size = New System.Drawing.Size(898, 250)
        Me.DecompileOptionsPanel.TabIndex = 8
        '
        'ReCreateFilesGroupBox
        '
        Me.ReCreateFilesGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ReCreateFilesGroupBox.Controls.Add(Me.Panel1)
        Me.ReCreateFilesGroupBox.Location = New System.Drawing.Point(0, 0)
        Me.ReCreateFilesGroupBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ReCreateFilesGroupBox.Name = "ReCreateFilesGroupBox"
        Me.ReCreateFilesGroupBox.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ReCreateFilesGroupBox.Size = New System.Drawing.Size(609, 246)
        Me.ReCreateFilesGroupBox.TabIndex = 0
        Me.ReCreateFilesGroupBox.TabStop = False
        Me.ReCreateFilesGroupBox.Text = "重新创建文件"
        '
        'Panel1
        '
        Me.Panel1.AutoScroll = True
        Me.Panel1.Controls.Add(Me.UseNonValveUvConversionCheckBox)
        Me.Panel1.Controls.Add(Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox)
        Me.Panel1.Controls.Add(Me.UseMixedCaseForKeywordsCheckBox)
        Me.Panel1.Controls.Add(Me.RemovePathFromMaterialFileNamesCheckBox)
        Me.Panel1.Controls.Add(Me.SkinFamilyOnSingleLineCheckBox)
        Me.Panel1.Controls.Add(Me.TextureBmpFilesCheckBox)
        Me.Panel1.Controls.Add(Me.DecompileOptionsUseDefaultsButton)
        Me.Panel1.Controls.Add(Me.ComboBox2)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.IncludeDefineBoneLinesCheckBox)
        Me.Panel1.Controls.Add(Me.GroupIntoQciFilesCheckBox)
        Me.Panel1.Controls.Add(Me.PlaceInAnimsSubfolderCheckBox)
        Me.Panel1.Controls.Add(Me.LodMeshSmdFilesCheckBox)
        Me.Panel1.Controls.Add(Me.ProceduralBonesVrdFileCheckBox)
        Me.Panel1.Controls.Add(Me.BoneAnimationSmdFilesCheckBox)
        Me.Panel1.Controls.Add(Me.VertexAnimationVtaFileCheckBox)
        Me.Panel1.Controls.Add(Me.PhysicsMeshSmdFileCheckBox)
        Me.Panel1.Controls.Add(Me.ReferenceMeshSmdFileCheckBox)
        Me.Panel1.Controls.Add(Me.QcFileCheckBox)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(4, 18)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(601, 224)
        Me.Panel1.TabIndex = 11
        '
        'OnlyChangedMaterialsInTextureGroupLinesCheckBox
        '
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.AutoSize = True
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.Location = New System.Drawing.Point(23, 60)
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.Name = "OnlyChangedMaterialsInTextureGroupLinesCheckBox"
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.Size = New System.Drawing.Size(268, 20)
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.TabIndex = 43
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.Text = "仅 $texturegroup 行中已更改的材质"
        Me.OnlyChangedMaterialsInTextureGroupLinesCheckBox.UseVisualStyleBackColor = True
        '
        'SkinFamilyOnSingleLineCheckBox
        '
        Me.SkinFamilyOnSingleLineCheckBox.AutoSize = True
        Me.SkinFamilyOnSingleLineCheckBox.Location = New System.Drawing.Point(23, 32)
        Me.SkinFamilyOnSingleLineCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.SkinFamilyOnSingleLineCheckBox.Name = "SkinFamilyOnSingleLineCheckBox"
        Me.SkinFamilyOnSingleLineCheckBox.Size = New System.Drawing.Size(263, 20)
        Me.SkinFamilyOnSingleLineCheckBox.TabIndex = 39
        Me.SkinFamilyOnSingleLineCheckBox.Text = "每个 $texturegroup 皮肤族占一行"
        Me.SkinFamilyOnSingleLineCheckBox.UseVisualStyleBackColor = True
        '
        'TextureBmpFilesCheckBox
        '
        Me.TextureBmpFilesCheckBox.AutoSize = True
        Me.TextureBmpFilesCheckBox.Location = New System.Drawing.Point(371, 4)
        Me.TextureBmpFilesCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextureBmpFilesCheckBox.Name = "TextureBmpFilesCheckBox"
        Me.TextureBmpFilesCheckBox.Size = New System.Drawing.Size(189, 20)
        Me.TextureBmpFilesCheckBox.TabIndex = 38
        Me.TextureBmpFilesCheckBox.Text = "纹理 BMP 文件（GoldSource）"
        Me.TextureBmpFilesCheckBox.UseVisualStyleBackColor = True
        '
        'DecompileOptionsUseDefaultsButton
        '
        Me.DecompileOptionsUseDefaultsButton.Location = New System.Drawing.Point(248, 242)
        Me.DecompileOptionsUseDefaultsButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DecompileOptionsUseDefaultsButton.Name = "DecompileOptionsUseDefaultsButton"
        Me.DecompileOptionsUseDefaultsButton.Size = New System.Drawing.Size(105, 28)
        Me.DecompileOptionsUseDefaultsButton.TabIndex = 37
        Me.DecompileOptionsUseDefaultsButton.Text = "使用默认值"
        Me.DecompileOptionsUseDefaultsButton.UseVisualStyleBackColor = True
        '
        'ComboBox2
        '
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(93, 245)
        Me.ComboBox2.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(145, 24)
        Me.ComboBox2.TabIndex = 15
        Me.ComboBox2.Visible = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(4, 249)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 16)
        Me.Label2.TabIndex = 23
        Me.Label2.Text = "模型格式："
        Me.Label2.Visible = False
        '
        'IncludeDefineBoneLinesCheckBox
        '
        Me.IncludeDefineBoneLinesCheckBox.AutoSize = True
        Me.IncludeDefineBoneLinesCheckBox.Location = New System.Drawing.Point(23, 89)
        Me.IncludeDefineBoneLinesCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.IncludeDefineBoneLinesCheckBox.Name = "IncludeDefineBoneLinesCheckBox"
        Me.IncludeDefineBoneLinesCheckBox.Size = New System.Drawing.Size(293, 20)
        Me.IncludeDefineBoneLinesCheckBox.TabIndex = 2
        Me.IncludeDefineBoneLinesCheckBox.Text = "包含 $definebone 行（视图模型常用）"
        Me.IncludeDefineBoneLinesCheckBox.UseVisualStyleBackColor = True
        '
        'GroupIntoQciFilesCheckBox
        '
        Me.GroupIntoQciFilesCheckBox.AutoSize = True
        Me.GroupIntoQciFilesCheckBox.Location = New System.Drawing.Point(93, 4)
        Me.GroupIntoQciFilesCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupIntoQciFilesCheckBox.Name = "GroupIntoQciFilesCheckBox"
        Me.GroupIntoQciFilesCheckBox.Size = New System.Drawing.Size(129, 20)
        Me.GroupIntoQciFilesCheckBox.TabIndex = 1
        Me.GroupIntoQciFilesCheckBox.Text = "分组到 QCI 文件中"
        Me.GroupIntoQciFilesCheckBox.UseVisualStyleBackColor = True
        '
        'PlaceInAnimsSubfolderCheckBox
        '
        Me.PlaceInAnimsSubfolderCheckBox.AutoSize = True
        Me.PlaceInAnimsSubfolderCheckBox.Location = New System.Drawing.Point(391, 174)
        Me.PlaceInAnimsSubfolderCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.PlaceInAnimsSubfolderCheckBox.Name = "PlaceInAnimsSubfolderCheckBox"
        Me.PlaceInAnimsSubfolderCheckBox.Size = New System.Drawing.Size(164, 20)
        Me.PlaceInAnimsSubfolderCheckBox.TabIndex = 9
        Me.PlaceInAnimsSubfolderCheckBox.Text = "放入 ""anims"" 子文件夹"
        Me.PlaceInAnimsSubfolderCheckBox.UseVisualStyleBackColor = True
        '
        'LodMeshSmdFilesCheckBox
        '
        Me.LodMeshSmdFilesCheckBox.AutoSize = True
        Me.LodMeshSmdFilesCheckBox.Location = New System.Drawing.Point(371, 32)
        Me.LodMeshSmdFilesCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.LodMeshSmdFilesCheckBox.Name = "LodMeshSmdFilesCheckBox"
        Me.LodMeshSmdFilesCheckBox.Size = New System.Drawing.Size(132, 20)
        Me.LodMeshSmdFilesCheckBox.TabIndex = 5
        Me.LodMeshSmdFilesCheckBox.Text = "LOD 网格 SMD 文件"
        Me.LodMeshSmdFilesCheckBox.UseVisualStyleBackColor = True
        '
        'ProceduralBonesVrdFileCheckBox
        '
        Me.ProceduralBonesVrdFileCheckBox.AutoSize = True
        Me.ProceduralBonesVrdFileCheckBox.Location = New System.Drawing.Point(371, 117)
        Me.ProceduralBonesVrdFileCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ProceduralBonesVrdFileCheckBox.Name = "ProceduralBonesVrdFileCheckBox"
        Me.ProceduralBonesVrdFileCheckBox.Size = New System.Drawing.Size(162, 20)
        Me.ProceduralBonesVrdFileCheckBox.TabIndex = 10
        Me.ProceduralBonesVrdFileCheckBox.Text = "程序化骨骼 VRD 文件"
        Me.ProceduralBonesVrdFileCheckBox.UseVisualStyleBackColor = True
        '
        'BoneAnimationSmdFilesCheckBox
        '
        Me.BoneAnimationSmdFilesCheckBox.AutoSize = True
        Me.BoneAnimationSmdFilesCheckBox.Location = New System.Drawing.Point(371, 145)
        Me.BoneAnimationSmdFilesCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.BoneAnimationSmdFilesCheckBox.Name = "BoneAnimationSmdFilesCheckBox"
        Me.BoneAnimationSmdFilesCheckBox.Size = New System.Drawing.Size(161, 20)
        Me.BoneAnimationSmdFilesCheckBox.TabIndex = 8
        Me.BoneAnimationSmdFilesCheckBox.Text = "骨骼动画 SMD 文件"
        Me.BoneAnimationSmdFilesCheckBox.UseVisualStyleBackColor = True
        '
        'VertexAnimationVtaFileCheckBox
        '
        Me.VertexAnimationVtaFileCheckBox.AutoSize = True
        Me.VertexAnimationVtaFileCheckBox.Location = New System.Drawing.Point(371, 89)
        Me.VertexAnimationVtaFileCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.VertexAnimationVtaFileCheckBox.Name = "VertexAnimationVtaFileCheckBox"
        Me.VertexAnimationVtaFileCheckBox.Size = New System.Drawing.Size(199, 20)
        Me.VertexAnimationVtaFileCheckBox.TabIndex = 7
        Me.VertexAnimationVtaFileCheckBox.Text = "顶点动画 VTA 文件（表情）"
        Me.VertexAnimationVtaFileCheckBox.UseVisualStyleBackColor = True
        '
        'PhysicsMeshSmdFileCheckBox
        '
        Me.PhysicsMeshSmdFileCheckBox.AutoSize = True
        Me.PhysicsMeshSmdFileCheckBox.Location = New System.Drawing.Point(371, 60)
        Me.PhysicsMeshSmdFileCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.PhysicsMeshSmdFileCheckBox.Name = "PhysicsMeshSmdFileCheckBox"
        Me.PhysicsMeshSmdFileCheckBox.Size = New System.Drawing.Size(143, 20)
        Me.PhysicsMeshSmdFileCheckBox.TabIndex = 6
        Me.PhysicsMeshSmdFileCheckBox.Text = "物理网格 SMD 文件"
        Me.PhysicsMeshSmdFileCheckBox.UseVisualStyleBackColor = True
        '
        'ReferenceMeshSmdFileCheckBox
        '
        Me.ReferenceMeshSmdFileCheckBox.AutoSize = True
        Me.ReferenceMeshSmdFileCheckBox.Location = New System.Drawing.Point(4, 145)
        Me.ReferenceMeshSmdFileCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ReferenceMeshSmdFileCheckBox.Name = "ReferenceMeshSmdFileCheckBox"
        Me.ReferenceMeshSmdFileCheckBox.Size = New System.Drawing.Size(156, 20)
        Me.ReferenceMeshSmdFileCheckBox.TabIndex = 3
        Me.ReferenceMeshSmdFileCheckBox.Text = "参考网格 SMD 文件"
        Me.ReferenceMeshSmdFileCheckBox.UseVisualStyleBackColor = True
        '
        'QcFileCheckBox
        '
        Me.QcFileCheckBox.AutoSize = True
        Me.QcFileCheckBox.Location = New System.Drawing.Point(4, 4)
        Me.QcFileCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.QcFileCheckBox.Name = "QcFileCheckBox"
        Me.QcFileCheckBox.Size = New System.Drawing.Size(61, 20)
        Me.QcFileCheckBox.TabIndex = 0
        Me.QcFileCheckBox.Text = "QC 文件"
        Me.QcFileCheckBox.UseVisualStyleBackColor = True
        '
        'OptionsGroupBox
        '
        Me.OptionsGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OptionsGroupBox.Controls.Add(Me.Panel3)
        Me.OptionsGroupBox.Location = New System.Drawing.Point(616, 0)
        Me.OptionsGroupBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.OptionsGroupBox.Name = "OptionsGroupBox"
        Me.OptionsGroupBox.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.OptionsGroupBox.Size = New System.Drawing.Size(282, 246)
        Me.OptionsGroupBox.TabIndex = 6
        Me.OptionsGroupBox.TabStop = False
        Me.OptionsGroupBox.Text = "选项"
        '
        'Panel3
        '
        Me.Panel3.AutoScroll = True
        Me.Panel3.Controls.Add(Me.PrefixMeshFileNamesWithModelNameCheckBox)
        Me.Panel3.Controls.Add(Me.OverrideMdlVersionLabel)
        Me.Panel3.Controls.Add(Me.OverrideMdlVersionComboBox)
        Me.Panel3.Controls.Add(Me.FolderForEachModelCheckBox)
        Me.Panel3.Controls.Add(Me.DebugInfoCheckBox)
        Me.Panel3.Controls.Add(Me.LogFileCheckBox)
        Me.Panel3.Controls.Add(Me.DeclareSequenceQciCheckBox)
        Me.Panel3.Controls.Add(Me.FormatForStricterImportersCheckBox)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(4, 18)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(274, 224)
        Me.Panel3.TabIndex = 0
        '
        'FolderForEachModelCheckBox
        '
        Me.FolderForEachModelCheckBox.AutoSize = True
        Me.FolderForEachModelCheckBox.Location = New System.Drawing.Point(4, 4)
        Me.FolderForEachModelCheckBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.FolderForEachModelCheckBox.Name = "FolderForEachModelCheckBox"
        Me.FolderForEachModelCheckBox.Size = New System.Drawing.Size(141, 20)
        Me.FolderForEachModelCheckBox.TabIndex = 11
        Me.FolderForEachModelCheckBox.Text = "每个模型一个文件夹"
        Me.FolderForEachModelCheckBox.UseVisualStyleBackColor = True
        '
        'DecompilerLogTextBox
        '
        Me.DecompilerLogTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.DecompilerLogTextBox.CueBannerText = ""
        Me.DecompilerLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DecompilerLogTextBox.ForeColor = System.Drawing.Color.FromArgb(CType(CType(200, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(200, Byte), Integer))
        Me.DecompilerLogTextBox.HideSelection = False
        Me.DecompilerLogTextBox.Location = New System.Drawing.Point(0, 32)
        Me.DecompilerLogTextBox.Margin = New System.Windows.Forms.Padding(5, 5, 5, 5)
        Me.DecompilerLogTextBox.Name = "DecompilerLogTextBox"
        Me.DecompilerLogTextBox.ReadOnly = True
        Me.DecompilerLogTextBox.Size = New System.Drawing.Size(898, 262)
        Me.DecompilerLogTextBox.TabIndex = 0
        Me.DecompilerLogTextBox.Text = ""
        Me.DecompilerLogTextBox.WordWrap = False
        '
        'DecompileButtonsPanel
        '
        Me.DecompileButtonsPanel.Controls.Add(Me.DecompileButton)
        Me.DecompileButtonsPanel.Controls.Add(Me.SkipCurrentModelButton)
        Me.DecompileButtonsPanel.Controls.Add(Me.CancelDecompileButton)
        Me.DecompileButtonsPanel.Controls.Add(Me.UseAllInCompileButton)
        Me.DecompileButtonsPanel.Controls.Add(Me.MeshResourceFolderButton)
        Me.DecompileButtonsPanel.Controls.Add(Me.VtfHeaderButton)
        Me.DecompileButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.DecompileButtonsPanel.Location = New System.Drawing.Point(0, 0)
        Me.DecompileButtonsPanel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DecompileButtonsPanel.Name = "DecompileButtonsPanel"
        Me.DecompileButtonsPanel.Size = New System.Drawing.Size(898, 32)
        Me.DecompileButtonsPanel.TabIndex = 7
        '
        'DecompileButton
        '
        Me.DecompileButton.Location = New System.Drawing.Point(0, 0)
        Me.DecompileButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DecompileButton.Name = "DecompileButton"
        Me.DecompileButton.Size = New System.Drawing.Size(140, 28)
        Me.DecompileButton.TabIndex = 2
        Me.DecompileButton.Text = "反编译"
        Me.DecompileButton.UseVisualStyleBackColor = True
        '
        'SkipCurrentModelButton
        '
        Me.SkipCurrentModelButton.Enabled = False
        Me.SkipCurrentModelButton.Location = New System.Drawing.Point(147, 0)
        Me.SkipCurrentModelButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.SkipCurrentModelButton.Name = "SkipCurrentModelButton"
        Me.SkipCurrentModelButton.Size = New System.Drawing.Size(140, 28)
        Me.SkipCurrentModelButton.TabIndex = 3
        Me.SkipCurrentModelButton.Text = "跳过当前模型"
        Me.SkipCurrentModelButton.UseVisualStyleBackColor = True
        '
        'CancelDecompileButton
        '
        Me.CancelDecompileButton.Enabled = False
        Me.CancelDecompileButton.Location = New System.Drawing.Point(294, 0)
        Me.CancelDecompileButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CancelDecompileButton.Name = "CancelDecompileButton"
        Me.CancelDecompileButton.Size = New System.Drawing.Size(140, 28)
        Me.CancelDecompileButton.TabIndex = 4
        Me.CancelDecompileButton.Text = "取消反编译"
        Me.CancelDecompileButton.UseVisualStyleBackColor = True
        '
        'UseAllInCompileButton
        '
        Me.UseAllInCompileButton.Enabled = False
        Me.UseAllInCompileButton.Location = New System.Drawing.Point(441, 0)
        Me.UseAllInCompileButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.UseAllInCompileButton.Name = "UseAllInCompileButton"
        Me.UseAllInCompileButton.Size = New System.Drawing.Size(140, 28)
        Me.UseAllInCompileButton.TabIndex = 5
        Me.UseAllInCompileButton.Text = "在编译中使用全部"
        Me.UseAllInCompileButton.UseVisualStyleBackColor = True
        '
        'MeshResourceFolderButton
        '
        Me.MeshResourceFolderButton.Location = New System.Drawing.Point(588, 0)
        Me.MeshResourceFolderButton.Name = "MeshResourceFolderButton"
        Me.MeshResourceFolderButton.Size = New System.Drawing.Size(128, 28)
        Me.MeshResourceFolderButton.TabIndex = 6
        Me.MeshResourceFolderButton.Text = "配套资源目录"
        Me.MeshResourceFolderButton.UseVisualStyleBackColor = True
        '
        'VtfHeaderButton
        '
        Me.VtfHeaderButton.Location = New System.Drawing.Point(724, 0)
        Me.VtfHeaderButton.Name = "VtfHeaderButton"
        Me.VtfHeaderButton.Size = New System.Drawing.Size(128, 28)
        Me.VtfHeaderButton.TabIndex = 7
        Me.VtfHeaderButton.Text = "修复 VTF 文件头…"
        Me.ToolTip1.SetToolTip(Me.VtfHeaderButton, "检查 PC VTF 7.0–7.2 二维纹理，并另存头长度修复副本。")
        Me.VtfHeaderButton.UseVisualStyleBackColor = True
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.DecompiledFilesComboBox)
        Me.Panel4.Controls.Add(Me.UseInEditButton)
        Me.Panel4.Controls.Add(Me.UseInCompileButton)
        Me.Panel4.Controls.Add(Me.GotoDecompiledFileButton)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 294)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(898, 32)
        Me.Panel4.TabIndex = 8
        '
        'DecompiledFilesComboBox
        '
        Me.DecompiledFilesComboBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DecompiledFilesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.DecompiledFilesComboBox.FormattingEnabled = True
        Me.DecompiledFilesComboBox.Location = New System.Drawing.Point(0, 5)
        Me.DecompiledFilesComboBox.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DecompiledFilesComboBox.Name = "DecompiledFilesComboBox"
        Me.DecompiledFilesComboBox.Size = New System.Drawing.Size(724, 24)
        Me.DecompiledFilesComboBox.TabIndex = 1
        '
        'UseInEditButton
        '
        Me.UseInEditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UseInEditButton.Enabled = False
        Me.UseInEditButton.Location = New System.Drawing.Point(643, 4)
        Me.UseInEditButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.UseInEditButton.Name = "UseInEditButton"
        Me.UseInEditButton.Size = New System.Drawing.Size(84, 28)
        Me.UseInEditButton.TabIndex = 2
        Me.UseInEditButton.Text = "在编辑中使用"
        Me.UseInEditButton.UseVisualStyleBackColor = True
        Me.UseInEditButton.Visible = False
        '
        'UseInCompileButton
        '
        Me.UseInCompileButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UseInCompileButton.Enabled = False
        Me.UseInCompileButton.Location = New System.Drawing.Point(732, 4)
        Me.UseInCompileButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.UseInCompileButton.Name = "UseInCompileButton"
        Me.UseInCompileButton.Size = New System.Drawing.Size(110, 28)
        Me.UseInCompileButton.TabIndex = 3
        Me.UseInCompileButton.Text = "在编译中使用"
        Me.UseInCompileButton.UseVisualStyleBackColor = True
        '
        'GotoDecompiledFileButton
        '
        Me.GotoDecompiledFileButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GotoDecompiledFileButton.Location = New System.Drawing.Point(848, 4)
        Me.GotoDecompiledFileButton.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GotoDecompiledFileButton.Name = "GotoDecompiledFileButton"
        Me.GotoDecompiledFileButton.Size = New System.Drawing.Size(50, 28)
        Me.GotoDecompiledFileButton.TabIndex = 4
        Me.GotoDecompiledFileButton.Text = "转到"
        Me.GotoDecompiledFileButton.UseVisualStyleBackColor = True
        '
        'DecompileUserControl
        '
        Me.Controls.Add(Me.Panel2)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "DecompileUserControl"
        Me.Size = New System.Drawing.Size(905, 660)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Options_LogSplitContainer.Panel1.ResumeLayout(False)
        Me.Options_LogSplitContainer.Panel2.ResumeLayout(False)
        CType(Me.Options_LogSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Options_LogSplitContainer.ResumeLayout(False)
        Me.DecompileOptionsPanel.ResumeLayout(False)
        Me.ReCreateFilesGroupBox.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.OptionsGroupBox.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.DecompileButtonsPanel.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DecompileButton As System.Windows.Forms.Button
	Friend WithEvents MdlPathFileNameTextBox As Crowbar.TextBoxEx
	Friend WithEvents BrowseForMdlPathFolderOrFileNameButton As System.Windows.Forms.Button
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents ReCreateFilesGroupBox As System.Windows.Forms.GroupBox
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents LodMeshSmdFilesCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents ProceduralBonesVrdFileCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents BoneAnimationSmdFilesCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents VertexAnimationVtaFileCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents PhysicsMeshSmdFileCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents DebugInfoCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents ReferenceMeshSmdFileCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents QcFileCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents Panel2 As System.Windows.Forms.Panel
	Friend WithEvents DecompilerLogTextBox As Crowbar.RichTextBoxEx
	Friend WithEvents CancelDecompileButton As System.Windows.Forms.Button
	Friend WithEvents SkipCurrentModelButton As System.Windows.Forms.Button
	Friend WithEvents DecompileComboBox As System.Windows.Forms.ComboBox
	Friend WithEvents Options_LogSplitContainer As System.Windows.Forms.SplitContainer
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents ComboBox2 As System.Windows.Forms.ComboBox
	Friend WithEvents FormatForStricterImportersCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents GroupIntoQciFilesCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents PlaceInAnimsSubfolderCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents LogFileCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents GotoMdlButton As System.Windows.Forms.Button
	Friend WithEvents IncludeDefineBoneLinesCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents FolderForEachModelCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents DecompiledFilesComboBox As System.Windows.Forms.ComboBox
	Friend WithEvents GotoDecompiledFileButton As System.Windows.Forms.Button
	Friend WithEvents UseInEditButton As System.Windows.Forms.Button
	Friend WithEvents UseInCompileButton As System.Windows.Forms.Button
	Friend WithEvents UseAllInCompileButton As System.Windows.Forms.Button
	Friend WithEvents MeshResourceFolderButton As System.Windows.Forms.Button
	Friend WithEvents VtfHeaderButton As System.Windows.Forms.Button
	Friend WithEvents DecompileOptionsUseDefaultsButton As System.Windows.Forms.Button
	Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
	Friend WithEvents TextureBmpFilesCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents SkinFamilyOnSingleLineCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents DeclareSequenceQciCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents RemovePathFromMaterialFileNamesCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents UseMixedCaseForKeywordsCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents GotoOutputPathButton As System.Windows.Forms.Button
	Friend WithEvents BrowseForOutputPathButton As System.Windows.Forms.Button
	Friend WithEvents OutputPathTextBox As Crowbar.TextBoxEx
	Friend WithEvents OutputPathComboBox As System.Windows.Forms.ComboBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents UseDefaultOutputSubfolderButton As System.Windows.Forms.Button
	Friend WithEvents OptionsGroupBox As System.Windows.Forms.GroupBox
	Friend WithEvents Panel3 As System.Windows.Forms.Panel
	Friend WithEvents OnlyChangedMaterialsInTextureGroupLinesCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents OutputSubfolderTextBox As Crowbar.TextBoxEx
	Friend WithEvents UseNonValveUvConversionCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents OverrideMdlVersionLabel As System.Windows.Forms.Label
	Friend WithEvents OverrideMdlVersionComboBox As System.Windows.Forms.ComboBox
	Friend WithEvents PrefixMeshFileNamesWithModelNameCheckBox As CheckBox
	Friend WithEvents DecompileButtonsPanel As Panel
	Friend WithEvents DecompileOptionsPanel As Panel
	Friend WithEvents Panel4 As Panel
End Class
