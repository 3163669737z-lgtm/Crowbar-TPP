Imports System.ComponentModel
Imports System.IO
Imports System.Linq

Public Class Decompiler
	Inherits BackgroundWorker

#Region "Create and Destroy"

	Public Sub New()
		MyBase.New()

		Me.theDecompiledQcFiles = New BindingListEx(Of String)()
		Me.theDecompiledFirstRefSmdFiles = New BindingListEx(Of String)()
		Me.theDecompiledFirstLodSmdFiles = New BindingListEx(Of String)()
		Me.theDecompiledPhysicsFiles = New BindingListEx(Of String)()
		Me.theDecompiledVtaFiles = New BindingListEx(Of String)()
		Me.theDecompiledFirstBoneAnimSmdFiles = New BindingListEx(Of String)()
		Me.theDecompiledVrdFiles = New BindingListEx(Of String)()
		Me.theDecompiledDeclareSequenceQciFiles = New BindingListEx(Of String)()
		Me.theDecompiledFirstTextureBmpFiles = New BindingListEx(Of String)()
		Me.theDecompiledLogFiles = New BindingListEx(Of String)()
		Me.theDecompiledFirstDebugFiles = New BindingListEx(Of String)()

		Me.WorkerReportsProgress = True
		Me.WorkerSupportsCancellation = True
		AddHandler Me.DoWork, AddressOf Me.Decompiler_DoWork
	End Sub

#End Region

#Region "Properties"

#End Region

#Region "Methods"

	Public Sub Run()
		Me.RunWorkerAsync()
	End Sub

	Public Sub SkipCurrentModel()
		'NOTE: This might have thread race condition, but it probably doesn't matter.
		Me.theSkipCurrentModelIsActive = True
	End Sub

	Public Function GetOutputPathFileName(ByVal relativePathFileName As String) As String
		Dim pathFileName As String

		pathFileName = Path.Combine(Me.theOutputPath, relativePathFileName)
		pathFileName = Path.GetFullPath(pathFileName)

		Return pathFileName
	End Function

	Public Function GetOutputPathFolderOrFileName() As String
		Return Me.theOutputPathOrModelOutputFileName
	End Function

#End Region

#Region "Private Methods"

#End Region

#Region "Private Methods in Background Thread"

	Private Sub Decompiler_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
		Me.ReportProgress(0, "")

		Me.theOutputPath = Me.GetOutputPath()

		Dim status As AppEnums.StatusMessage
		If Me.DecompilerInputsAreValid() Then
			status = Me.Decompile()
		Else
			status = StatusMessage.Error
		End If
		e.Result = Me.GetDecompilerOutputs(status)

		If Me.CancellationPending Then
			e.Cancel = True
		End If
	End Sub

	Private Function GetOutputPath() As String
		Dim outputPath As String

		If TheApp.Settings.DecompileOutputFolderOption = DecompileOutputPathOptions.Subfolder Then
			If File.Exists(TheApp.Settings.DecompileMdlPathFileName) Then
				outputPath = Path.Combine(FileManager.GetPath(TheApp.Settings.DecompileMdlPathFileName), TheApp.Settings.DecompileOutputSubfolderName)
			ElseIf Directory.Exists(TheApp.Settings.DecompileMdlPathFileName) Then
				outputPath = Path.GetFullPath(Path.Combine(TheApp.Settings.DecompileMdlPathFileName, TheApp.Settings.DecompileOutputSubfolderName))
			Else
				outputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			End If
		Else
			outputPath = TheApp.Settings.DecompileOutputFullPath
		End If

		Return outputPath
	End Function

	Private Function DecompilerInputsAreValid() As Boolean
		Dim inputsAreValid As Boolean

		If String.IsNullOrEmpty(TheApp.Settings.DecompileMdlPathFileName) Then
			inputsAreValid = False
		Else
			inputsAreValid = FileManager.PathExistsAfterTryToCreate(Me.theOutputPath)
		End If

		Return inputsAreValid
	End Function

	Private Function GetDecompilerOutputs(ByVal status As AppEnums.StatusMessage) As DecompilerOutputInfo
		Dim decompileResultInfo As New DecompilerOutputInfo()

		decompileResultInfo.theStatus = status

		If TheApp.Settings.DecompileQcFileIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledQcFiles
		ElseIf TheApp.Settings.DecompileReferenceMeshSmdFileIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledFirstRefSmdFiles
		ElseIf TheApp.Settings.DecompileLodMeshSmdFilesIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledFirstLodSmdFiles
		ElseIf TheApp.Settings.DecompilePhysicsMeshSmdFileIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledPhysicsFiles
		ElseIf TheApp.Settings.DecompileVertexAnimationVtaFileIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledVtaFiles
		ElseIf TheApp.Settings.DecompileBoneAnimationSmdFilesIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledFirstBoneAnimSmdFiles
		ElseIf TheApp.Settings.DecompileProceduralBonesVrdFileIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledVrdFiles
		ElseIf TheApp.Settings.DecompileDeclareSequenceQciFileIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledDeclareSequenceQciFiles
		ElseIf TheApp.Settings.DecompileTextureBmpFilesIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledFirstTextureBmpFiles
		ElseIf TheApp.Settings.DecompileLogFileIsChecked Then
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledLogFiles
		Else
			decompileResultInfo.theDecompiledRelativePathFileNames = Me.theDecompiledFirstDebugFiles
		End If

		If decompileResultInfo.theDecompiledRelativePathFileNames.Count = 0 Then
			Dim alternatives As BindingListEx(Of String)() = {Me.theDecompiledFirstRefSmdFiles, Me.theDecompiledFirstLodSmdFiles,
				Me.theDecompiledPhysicsFiles, Me.theDecompiledVtaFiles, Me.theDecompiledFirstBoneAnimSmdFiles, Me.theDecompiledVrdFiles,
				Me.theDecompiledDeclareSequenceQciFiles, Me.theDecompiledFirstTextureBmpFiles, Me.theDecompiledLogFiles, Me.theDecompiledFirstDebugFiles}
			Dim firstAvailable As BindingListEx(Of String) = alternatives.FirstOrDefault(Function(files) files.Count > 0)
			If firstAvailable IsNot Nothing Then decompileResultInfo.theDecompiledRelativePathFileNames = firstAvailable
		End If
		If decompileResultInfo.theDecompiledRelativePathFileNames.Count = 0 Then
			Me.theOutputPathOrModelOutputFileName = ""
			'ElseIf decompileResultInfo.theDecompiledRelativePathFileNames.Count = 1 Then
			'	Me.theOutputPathOrModelOutputFileName = decompileResultInfo.theDecompiledRelativePathFileNames(0)
		Else
			Me.theOutputPathOrModelOutputFileName = Me.theOutputPath
		End If

		Return decompileResultInfo
	End Function

	Private Function Decompile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Me.theSkipCurrentModelIsActive = False
		Me.theInputMdlPathName = ""
		Dim modelResults As New List(Of StatusMessage)

		Me.theDecompiledQcFiles.Clear()
		Me.theDecompiledFirstRefSmdFiles.Clear()
		Me.theDecompiledFirstLodSmdFiles.Clear()
		Me.theDecompiledPhysicsFiles.Clear()
		Me.theDecompiledVtaFiles.Clear()
		Me.theDecompiledFirstBoneAnimSmdFiles.Clear()
		Me.theDecompiledVrdFiles.Clear()
		Me.theDecompiledDeclareSequenceQciFiles.Clear()
		Me.theDecompiledFirstTextureBmpFiles.Clear()
		Me.theDecompiledLogFiles.Clear()
		Me.theDecompiledFirstDebugFiles.Clear()

		Dim mdlPathFileName As String
		mdlPathFileName = TheApp.Settings.DecompileMdlPathFileName
		If File.Exists(mdlPathFileName) Then
			Me.theInputMdlPathName = FileManager.GetPath(mdlPathFileName)
		ElseIf Directory.Exists(mdlPathFileName) Then
			Me.theInputMdlPathName = mdlPathFileName
		End If

		Dim progressDescriptionText As String
		progressDescriptionText = "正在使用 ... 反编译：" + TheApp.GetProductNameAndVersion() + ": "

		Try
			If Me.theInputMdlPathName = "" Then
				'Can get here if mdlPathFileName exists, but only with parts of the path using "Length8.3" names.
				'Somehow when drag-dropping such a pathFileName, even though Windows shows full names in the path, Crowbar shows it with "Length8.3" names.
				progressDescriptionText += """" + mdlPathFileName + """"
				Me.UpdateProgressStart(progressDescriptionText + " ...")
				Me.UpdateProgress()
				Me.UpdateProgress(1, "错误：失败，因为实际路径过长。")
				status = StatusMessage.Error
			ElseIf TheApp.Settings.DecompileMode = InputOptions.FolderRecursion Then
				progressDescriptionText += """" + Me.theInputMdlPathName + """ (folder + subfolders)"
				Me.UpdateProgressStart(progressDescriptionText + " ...")

				status = Me.CreateLogTextFile("")
				'If status = StatusMessage.Error Then
				'	Return status
				'End If

				Me.DecompileModelsInFolderRecursively(Me.theInputMdlPathName, modelResults)
			ElseIf TheApp.Settings.DecompileMode = InputOptions.Folder Then
				progressDescriptionText += """" + Me.theInputMdlPathName + """ (folder)"
				Me.UpdateProgressStart(progressDescriptionText + " ...")

				status = Me.CreateLogTextFile("")
				'If status = StatusMessage.Error Then
				'	Return status
				'End If

				Me.DecompileModelsInFolder(Me.theInputMdlPathName, modelResults)
			Else
				progressDescriptionText += """" + mdlPathFileName + """"
				Me.UpdateProgressStart(progressDescriptionText + " ...")
				status = Me.DecompileOneModel(mdlPathFileName)
			End If
			If modelResults.Count > 0 Then
				Dim counts As String = String.Join("，", modelResults.GroupBy(Function(result) result).
					Select(Function(group) DescribeResult(group.Key) + " " + group.Count().ToString()))
				Me.UpdateProgress(1, counts)
				If status = StatusMessage.Success Then status = SummarizeResults(modelResults)
			End If
		Catch ex As Exception
			Me.UpdateProgress(1, "批处理失败 — " + ex.Message)
			status = StatusMessage.Error
		Finally
			If Me.theLogFileStream IsNot Nothing Then
				Me.CloseLog()
			End If
		End Try

		Me.UpdateProgressStop("反编译结果 — " + DescribeResult(If(Me.CancellationPending, StatusMessage.Canceled, status)))

		Return status
	End Function

	Private Sub DecompileModelsInFolderRecursively(modelsPathName As String, results As List(Of StatusMessage))
		Me.DecompileModelsInFolder(modelsPathName, results)
		If Me.CancellationPending Then Return
		For Each pathName As String In Directory.GetDirectories(modelsPathName)
			Me.DecompileModelsInFolderRecursively(pathName, results)
			If Me.CancellationPending Then Exit For
		Next
	End Sub

	Private Sub DecompileModelsInFolder(modelsPathName As String, results As List(Of StatusMessage))
		For Each pathFileName As String In Directory.GetFiles(modelsPathName, "*.mdl")
			If Me.CancellationPending Then Exit For
			results.Add(Me.DecompileOneModel(pathFileName))
			Me.theSkipCurrentModelIsActive = False
		Next
	End Sub

	Private Shared Function SummarizeResults(results As List(Of StatusMessage)) As StatusMessage
		Dim completed As StatusMessage() = results.Where(Function(result) result <> StatusMessage.Skipped).Distinct().ToArray()
		If completed.Length = 0 Then Return StatusMessage.Skipped
		If completed.Length = 1 Then Return completed(0)
		If completed.Contains(StatusMessage.Success) OrElse completed.Contains(StatusMessage.PartialSuccess) Then Return StatusMessage.PartialSuccess
		Return StatusMessage.Error
	End Function

	Private Shared Function DescribeResult(status As StatusMessage) As String
		Select Case status
			Case StatusMessage.Success : Return "成功"
			Case StatusMessage.PartialSuccess : Return "部分成功"
			Case StatusMessage.Unsupported : Return "不支持"
			Case StatusMessage.Canceled : Return "已取消"
			Case StatusMessage.Skipped : Return "已跳过"
			Case Else : Return "失败"
		End Select
	End Function

	Private Function DecompileOneModel(mdlPathFileName As String) As StatusMessage
		Me.theCommittedModelFiles.Clear()
		Me.theModelWriteFailed = False
		Dim status As StatusMessage = StatusMessage.Error
		Dim stage As String = "准备输出目录"
		Try
			Dim mdlRelativePathName As String = FileManager.GetRelativePathFileName(Me.theInputMdlPathName, FileManager.GetPath(mdlPathFileName))
			Me.theModelOutputPath = Path.GetFullPath(Path.Combine(Me.theOutputPath, mdlRelativePathName))
			If TheApp.Settings.DecompileFolderForEachModelIsChecked Then
				Me.theModelOutputPath = Path.Combine(Me.theModelOutputPath, Path.GetFileNameWithoutExtension(mdlPathFileName))
			End If
			Directory.CreateDirectory(Me.theModelOutputPath)

			Dim logFailed As Boolean = TheApp.Settings.DecompileMode = InputOptions.File AndAlso
				Me.CreateLogTextFile(mdlPathFileName) <> StatusMessage.Success
			Me.UpdateProgress()
			Me.UpdateProgress(1, "正在反编译 """ + mdlPathFileName + """")
			stage = "识别 MDL 布局"
			Dim version As Integer
			Dim model As SourceModel = SourceModel.Create(mdlPathFileName, TheApp.Settings.DecompileOverrideMdlVersion, version)
			If model Is Nothing Then
				Me.UpdateProgress(2, stage + " — 不支持文件声明的版本 v" + version.ToString())
				status = StatusMessage.Unsupported
				Return status
			End If
			model.MeshResourceRoot = TheApp.Settings.DecompileMeshResourceRoot
			Me.UpdateProgress(2, "MDL 声明版本 v" + model.HeaderVersion.GetValueOrDefault(model.Version).ToString() +
				"，读取布局 v" + model.Version.ToString() + " (" + model.GetType().Name + ")" + model.LayoutReason)

			stage = "读取 MDL 文件头"
			Dim headerStatus As StatusMessage = model.ReadMdlFileHeader()
			If headerStatus <> StatusMessage.Success AndAlso headerStatus <> StatusMessage.ErrorInvalidInternalMdlFileSize Then
				Me.UpdateProgress(2, stage + " — " + headerStatus.ToString())
				Return status
			End If
			If headerStatus = StatusMessage.ErrorInvalidInternalMdlFileSize Then
				Me.UpdateProgress(2, stage + " — 声明长度与实际文件大小不同。")
			End If

			stage = "检查配套文件"
			Dim missing As FilesFoundFlags = model.CheckForRequiredFiles()
			For Each entry As String In model.CompatibilityDiagnostics
				Me.UpdateProgress(3, entry)
			Next
			If missing <> FilesFoundFlags.AllFilesFound Then
				Me.UpdateProgress(2, stage + " — " + missing.ToString())
			End If
			If (missing And (FilesFoundFlags.ErrorRequiredMdlFileNotFound Or FilesFoundFlags.ErrorRequiredSequenceGroupMdlFileNotFound Or
				FilesFoundFlags.ErrorRequiredTextureMdlFileNotFound Or FilesFoundFlags.Error)) <> 0 Then Return status

			If Me.CancellationPending OrElse Me.theSkipCurrentModelIsActive Then
				status = If(Me.CancellationPending, StatusMessage.Canceled, StatusMessage.Skipped)
				Return status
			End If

			stage = "读取模型数据"
			Dim readStatus As StatusMessage = Me.ReadCompiledFiles(model)
			For Each entry As String In model.ReadDiagnostics
				Me.UpdateProgress(3, entry)
			Next
			If readStatus = StatusMessage.Error Then Return status


			If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
				stage = "写出反编译信息"
				Me.WriteDebugFiles(model)
			End If

			stage = "写出模型文件"
			Dim writeStatus As StatusMessage = Me.WriteDecompiledFiles(model)
			For Each entry As String In model.NameDiagnostics
				Me.UpdateProgress(3, entry)
			Next
			If writeStatus = StatusMessage.Canceled OrElse writeStatus = StatusMessage.Skipped Then
				status = writeStatus
				Return status
			End If

			If logFailed OrElse missing <> FilesFoundFlags.AllFilesFound OrElse headerStatus <> StatusMessage.Success OrElse
				readStatus <> StatusMessage.Success OrElse model.ReadDiagnostics.Count > 0 OrElse writeStatus <> StatusMessage.Success OrElse Me.theModelWriteFailed Then
				status = If(Me.theCommittedModelFiles.Count > 0, StatusMessage.PartialSuccess, StatusMessage.Error)
				Me.ReportUnwrittenQcReferences(model)
			Else
				status = StatusMessage.Success
			End If
		Catch ex As NotSupportedException When stage = "识别 MDL 布局"
			status = StatusMessage.Unsupported
			Me.UpdateProgress(2, stage + " — " + ex.Message)
		Catch ex As Exception
			status = If(Me.theCommittedModelFiles.Count > 0, StatusMessage.PartialSuccess, StatusMessage.Error)
			Me.UpdateProgress(2, stage + " — """ + mdlPathFileName + """ — " + ex.Message)
		Finally
			Me.UpdateProgress(1, """" + mdlPathFileName + """ — " + DescribeResult(status))
			If TheApp.Settings.DecompileMode = InputOptions.File Then Me.CloseLog()
		End Try
		Return status
	End Function

	Private Sub CloseLog()
		Dim writer As StreamWriter = Me.theLogFileStream
		Me.theLogFileStream = Nothing
		If writer Is Nothing Then Return
		Try
			writer.Dispose()
		Catch ex As IOException
			Me.ReportProgress(1, "关闭反编译日志失败 — " + ex.Message)
		End Try
	End Sub

	Private Sub ReportUnwrittenQcReferences(model As SourceModel)
		Dim qc As String = Path.Combine(Me.theModelOutputPath, model.Name + ".qc")
		If Not Me.theCommittedModelFiles.Contains(qc) Then
			If TheApp.Settings.DecompileQcFileIsChecked AndAlso Me.theCommittedModelFiles.Count > 0 Then
				Me.UpdateProgress(3, "本次未提交 QC，已保留成功写出的文件。")
			End If
			Return
		End If
		Dim prefix As String = Me.theModelOutputPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar
		Dim scripts As New Queue(Of String)
		Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
		scripts.Enqueue(qc)
		seen.Add(qc)
		While scripts.Count > 0
			Dim script As String = scripts.Dequeue()
			For Each line As String In File.ReadLines(script)
				If line.TrimStart().StartsWith("//", StringComparison.Ordinal) Then Continue For
				For Each reference As System.Text.RegularExpressions.Match In System.Text.RegularExpressions.Regex.Matches(line,
					"""([^""]+\.(?:smd|dmx|vta|vrd|qci))""", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
					Dim relativeName As String = reference.Groups(1).Value
					If Path.IsPathRooted(relativeName) OrElse relativeName.IndexOfAny(Path.GetInvalidPathChars()) >= 0 Then Continue For
					Dim pathFileName As String = Path.GetFullPath(Path.Combine(Me.theModelOutputPath, relativeName))
					If Not pathFileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) OrElse Not seen.Add(pathFileName) Then Continue For
					If String.Equals(Path.GetExtension(pathFileName), ".qci", StringComparison.OrdinalIgnoreCase) AndAlso
						File.Exists(pathFileName) AndAlso (File.GetAttributes(pathFileName) And FileAttributes.ReparsePoint) = 0 Then
						scripts.Enqueue(pathFileName)
					ElseIf Not Me.theCommittedModelFiles.Contains(pathFileName) Then
						Me.UpdateProgress(3, "本次未生成的 QC 引用 — " + relativeName)
					End If
				Next
			Next
		End While
	End Sub

	Private Function CreateLogTextFile(ByVal mdlPathFileName As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileLogFileIsChecked Then
			Dim mdlFileName As String
			Dim logPath As String
			Dim logFileName As String
			Dim logPathFileName As String

			Try
				If mdlPathFileName <> "" Then
					logPath = Me.theModelOutputPath
					mdlFileName = Path.GetFileNameWithoutExtension(mdlPathFileName)
					logFileName = mdlFileName + " " + My.Resources.Decompile_LogFileNameSuffix
				Else
					logPath = Me.theOutputPath
					logFileName = My.Resources.Decompile_LogFileNameSuffix
				End If
				FileManager.CreatePath(logPath)
				logPathFileName = Path.Combine(logPath, logFileName)

				Me.theLogFileStream = File.CreateText(logPathFileName)
				Me.theLogFileStream.AutoFlush = True

				If File.Exists(logPathFileName) Then
					Me.theDecompiledLogFiles.Add(FileManager.GetRelativePathFileName(Me.theOutputPath, logPathFileName))
				End If

				Me.theLogFileStream.WriteLine("// " + TheApp.GetHeaderComment())
				Me.theLogFileStream.Flush()
			Catch ex As Exception
				Me.UpdateProgress()
				Me.UpdateProgress(2, "错误：Crowbar 尝试写入反编译日志文件，但系统返回了此消息：" + ex.Message)
				status = StatusMessage.Error
			End Try
		Else
			Me.theLogFileStream = Nothing
		End If

		Return status
	End Function

	Private Function ReadCompiledFiles(model As SourceModel) As StatusMessage
		If Not Me.ReadStage(model, "MDL", AddressOf model.ReadMdlFile) Then Return StatusMessage.Error
		Dim complete As Boolean = True
		If model.SequenceGroupMdlFilesAreUsed Then complete = Me.ReadStage(model, "序列组 MDL", AddressOf model.ReadSequenceGroupMdlFiles) And complete
		If model.TextureMdlFileIsUsed Then complete = Me.ReadStage(model, "纹理 MDL", AddressOf model.ReadTextureMdlFile) And complete
		If model.PhyFileIsUsed Then
			AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
			Try
				complete = Me.ReadStage(model, "PHY", AddressOf model.ReadPhyFile) And complete
			Finally
				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
			End Try
		End If
		If model.VtxFileIsUsed Then complete = Me.ReadStage(model, "VTX", AddressOf model.ReadVtxFile) And complete
		If model.AniFileIsUsed AndAlso TheApp.Settings.DecompileBoneAnimationSmdFilesIsChecked Then
			complete = Me.ReadStage(model, "ANI", AddressOf model.ReadAniFile) And complete
		End If
		If model.VvdFileIsUsed Then complete = Me.ReadStage(model, "VVD", AddressOf model.ReadVvdFile) And complete
		Return If(complete, StatusMessage.Success, StatusMessage.PartialSuccess)
	End Function

	Private Function ReadStage(model As SourceModel, inputType As String, read As Func(Of StatusMessage)) As Boolean
		Me.UpdateProgress(3, "正在读取 " + inputType)
		model.LastReadError = Nothing
		Try
			Dim status As StatusMessage = read()
			If status = StatusMessage.Success Then Return True
			Me.UpdateProgress(3, "读取失败 — " + inputType + " — " + If(model.LastReadError, status.ToString()))
		Catch ex As Exception
			Me.UpdateProgress(3, "读取失败 — " + inputType + " — " + If(model.LastReadError, ex.Message))
		End Try
		Return False
	End Function

	'Private Function ProcessData(ByVal model As SourceModel) As AppEnums.StatusMessage
	'	Dim status As AppEnums.StatusMessage = StatusMessage.Success

	'	'TODO: Create all possible SMD file names before using them, so can handle any name collisions.
	'	'      Store mesh SMD file names in list in SourceMdlModel where the index is lodIndex.
	'	'      Store anim SMD file name in SourceMdlAnimationDesc48.

	'	Return status
	'End Function

	Private Function WriteDecompiledFiles(model As SourceModel) As StatusMessage
		TheApp.SmdFileNames.Clear()
		Dim stages As (Name As String, Write As Func(Of SourceModel, StatusMessage))() = {
			("QC", AddressOf Me.WriteQcFile),
			("参考网格", AddressOf Me.WriteReferenceMeshFiles),
			("LOD 网格", AddressOf Me.WriteLodMeshFiles),
			("物理网格", AddressOf Me.WritePhysicsMeshFile),
			("VRD", AddressOf Me.WriteProceduralBonesFile),
			("VTA", AddressOf Me.WriteVertexAnimationFiles),
			("骨骼动画", AddressOf Me.WriteBoneAnimationFiles),
			("BMP", AddressOf Me.WriteTextureFiles),
			("DeclareSequence QCI", AddressOf Me.WriteDeclareSequenceQciFile)
		}
		Dim result As StatusMessage = StatusMessage.Success
		For Each stage As (Name As String, Write As Func(Of SourceModel, StatusMessage)) In stages
			If Me.CancellationPending Then Return StatusMessage.Canceled
			If Me.theSkipCurrentModelIsActive Then Return StatusMessage.Skipped
			Try
				Dim status As StatusMessage = stage.Write(model)
				If status = StatusMessage.Canceled OrElse status = StatusMessage.Skipped Then Return status
				If status <> StatusMessage.Success Then
					result = StatusMessage.Error
					Me.UpdateProgress(3, "写出阶段失败 — " + stage.Name + " — " + status.ToString())
				End If
			Catch ex As Exception
				result = StatusMessage.Error
				Me.UpdateProgress(3, "写出阶段失败 — " + stage.Name + " — " + ex.Message)
			Finally
				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
			End Try
		Next
		Return result
	End Function

	'Private Sub TestWriteDmx()
	'	Dim currentFolder As String
	'	currentFolder = Directory.GetCurrentDirectory()
	'	Directory.SetCurrentDirectory(Me.theModelOutputPath)

	'	Dim HelloWorld As New Datamodel.Datamodel("model", 1)		' must provide a format name (can be anything) and version
	'	HelloWorld.Root = HelloWorld.CreateElement("my_root")
	'	HelloWorld.Root("Hello") = "World"		' any supported attribute type can be assigned
	'	Dim MyString As String = HelloWorld.Root.[Get](Of String)("Hello")

	'	HelloWorld.Save("hello world.dmx", "binary", 2)		' must provide an encoding name and version	

	'	Directory.SetCurrentDirectory(currentFolder)
	'End Sub

	Private Function WriteQcFile(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileQcFileIsChecked Then
			If TheApp.Settings.DecompileGroupIntoQciFilesIsChecked Then
				'Me.UpdateProgress(3, "Writing QC and QCI files ...")
				Me.UpdateProgress(3, "QC 和 QCI 文件：")
			Else
				'Me.UpdateProgress(3, "Writing QC file ...")
				Me.UpdateProgress(3, "QC 文件：")
			End If
			Me.theDecompiledFileType = DecompiledFileType.QC
			Me.theFirstDecompiledFileHasBeenAdded = False
			AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

			Dim qcPathFileName As String
			qcPathFileName = Path.Combine(Me.theModelOutputPath, model.Name + ".qc")

			status = model.WriteQcFile(qcPathFileName)

			If status = StatusMessage.Success AndAlso Me.theCommittedModelFiles.Contains(qcPathFileName) Then
				Me.theDecompiledQcFiles.Add(FileManager.GetRelativePathFileName(Me.theOutputPath, qcPathFileName))
			End If

			RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
			'If TheApp.Settings.DecompileGroupIntoQciFilesIsChecked Then
			'	Me.UpdateProgress(3, "... Writing QC and QCI files finished.")
			'Else
			'	Me.UpdateProgress(3, "... Writing QC file finished.")
			'End If
		End If

		If Me.CancellationPending Then
			status = StatusMessage.Canceled
		ElseIf Me.theSkipCurrentModelIsActive Then
			status = StatusMessage.Skipped
		End If

		Return status
	End Function

	Private Function WriteReferenceMeshFiles(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileReferenceMeshSmdFileIsChecked Then
			Dim source49 As SourceModel49 = TryCast(model, SourceModel49)
			If model.HasMeshData OrElse (source49 IsNot Nothing AndAlso source49.CanWriteSkeletonOnly) Then
				'Me.UpdateProgress(3, "Writing reference mesh files ...")
				Me.UpdateProgress(3, "参考网格文件：")
				Me.theDecompiledFileType = DecompiledFileType.ReferenceMesh
				Me.theFirstDecompiledFileHasBeenAdded = False
				AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

				status = model.WriteReferenceMeshFiles(Me.theModelOutputPath)

				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
				'Me.UpdateProgress(3, "... Writing reference mesh files finished.")
			End If
		End If

		Return status
	End Function

	Private Function WriteLodMeshFiles(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileLodMeshSmdFilesIsChecked Then
			If model.HasLodMeshData Then
				'Me.UpdateProgress(3, "Writing LOD mesh files ...")
				Me.UpdateProgress(3, "LOD 网格文件：")
				Me.theDecompiledFileType = DecompiledFileType.LodMesh
				Me.theFirstDecompiledFileHasBeenAdded = False
				AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

				status = model.WriteLodMeshFiles(Me.theModelOutputPath)

				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
				'Me.UpdateProgress(3, "... Writing LOD mesh files finished.")
			End If
		End If

		Return status
	End Function

	Private Function WritePhysicsMeshFile(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompilePhysicsMeshSmdFileIsChecked Then
			If model.HasPhysicsMeshData Then
				'Me.UpdateProgress(3, "Writing physics mesh file ...")
				Me.UpdateProgress(3, "物理网格文件：")
				Me.theDecompiledFileType = DecompiledFileType.PhysicsMesh
				Me.theFirstDecompiledFileHasBeenAdded = False
				AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

				status = model.WritePhysicsMeshSmdFile(Me.theModelOutputPath)

				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
				'Me.UpdateProgress(3, "... Writing physics mesh file finished.")
			End If
		End If

		If Me.CancellationPending Then
			status = StatusMessage.Canceled
		ElseIf Me.theSkipCurrentModelIsActive Then
			status = StatusMessage.Skipped
		End If

		Return status
	End Function

	Private Function WriteVertexAnimationFiles(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileVertexAnimationVtaFileIsChecked Then
			If model.HasVertexAnimationData Then
				'Me.UpdateProgress(3, "Writing VTA file ...")
				Me.UpdateProgress(3, "顶点动画文件：")
				Me.theDecompiledFileType = DecompiledFileType.VertexAnimation
				Me.theFirstDecompiledFileHasBeenAdded = False
				AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

				'Dim vtaPathFileName As String
				'vtaPathFileName = Path.Combine(Me.theModelOutputPath, SourceFileNamesModule.GetVtaFileName(model.Name))

				'status = model.WriteVertexAnimationVtaFile(vtaPathFileName)
				status = model.WriteVertexAnimationVtaFiles(Me.theModelOutputPath)

				'If File.Exists(vtaPathFileName) Then
				'	Me.theDecompiledVtaFiles.Add(FileManager.GetRelativePathFileName(Me.theOutputPath, vtaPathFileName))
				'End If

				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
				'Me.UpdateProgress(3, "... Writing VTA file finished.")
			End If
		End If

		If Me.CancellationPending Then
			status = StatusMessage.Canceled
		ElseIf Me.theSkipCurrentModelIsActive Then
			status = StatusMessage.Skipped
		End If

		Return status
	End Function

	Private Function WriteBoneAnimationFiles(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileBoneAnimationSmdFilesIsChecked Then
			If model.HasBoneAnimationData Then
				Dim outputPath As String
				outputPath = Path.Combine(Me.theModelOutputPath, SourceFileNamesModule.GetAnimationSmdRelativePath(model.Name))
				If FileManager.PathExistsAfterTryToCreate(outputPath) Then
					'Me.UpdateProgress(3, "Writing bone animation SMD files ...")
					Me.UpdateProgress(3, "骨骼动画文件：")
					Me.theDecompiledFileType = DecompiledFileType.BoneAnimation
					Me.theFirstDecompiledFileHasBeenAdded = False
					AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

					status = model.WriteBoneAnimationSmdFiles(Me.theModelOutputPath)

					RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
					'Me.UpdateProgress(3, "... Writing bone animation SMD files finished.")
				Else
					Me.UpdateProgress(3, "无法创建骨骼动画输出目录 — " + outputPath)
					status = StatusMessage.Error
				End If
			End If
		End If

		Return status
	End Function

	Private Function WriteProceduralBonesFile(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileProceduralBonesVrdFileIsChecked Then
			If model.HasProceduralBonesData Then
				'Me.UpdateProgress(3, "Writing VRD file ...")
				Me.UpdateProgress(3, "程序化骨骼文件：")
				Me.theDecompiledFileType = DecompiledFileType.ProceduralBones
				Me.theFirstDecompiledFileHasBeenAdded = False
				AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

				Dim vrdPathFileName As String
				vrdPathFileName = Path.Combine(Me.theModelOutputPath, SourceFileNamesModule.GetVrdFileName(model.Name))

				status = model.WriteVrdFile(vrdPathFileName)

				If status = StatusMessage.Success AndAlso Me.theCommittedModelFiles.Contains(vrdPathFileName) Then
					Me.theDecompiledVrdFiles.Add(FileManager.GetRelativePathFileName(Me.theOutputPath, vrdPathFileName))
				End If

				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
				'Me.UpdateProgress(3, "... Writing VRD file finished.")
			End If
		End If

		If Me.CancellationPending Then
			status = StatusMessage.Canceled
		ElseIf Me.theSkipCurrentModelIsActive Then
			status = StatusMessage.Skipped
		End If

		Return status
	End Function

	Private Function WriteDeclareSequenceQciFile(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileDeclareSequenceQciFileIsChecked Then
			If model.HasBoneAnimationData Then
				Me.UpdateProgress(3, "DeclareSequence QCI 文件：")
				Me.theDecompiledFileType = DecompiledFileType.DeclareSequenceQci
				Me.theFirstDecompiledFileHasBeenAdded = False
				AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

				Dim declareSequenceQciPathFileName As String
				declareSequenceQciPathFileName = Path.Combine(Me.theModelOutputPath, SourceFileNamesModule.GetDeclareSequenceQciFileName(model.Name))

				status = model.WriteDeclareSequenceQciFile(declareSequenceQciPathFileName)

				If status = StatusMessage.Success AndAlso Me.theCommittedModelFiles.Contains(declareSequenceQciPathFileName) Then
					Me.theDecompiledDeclareSequenceQciFiles.Add(FileManager.GetRelativePathFileName(Me.theOutputPath, declareSequenceQciPathFileName))
				End If

				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
			End If
		End If

		If Me.CancellationPending Then
			status = StatusMessage.Canceled
		ElseIf Me.theSkipCurrentModelIsActive Then
			status = StatusMessage.Skipped
		End If

		Return status
	End Function

	Private Function WriteTextureFiles(ByVal model As SourceModel) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If TheApp.Settings.DecompileTextureBmpFilesIsChecked Then
			If model.HasTextureFileData Then
				'Me.UpdateProgress(3, "Writing texture files ...")
				Me.UpdateProgress(3, "纹理文件：")
				Me.theDecompiledFileType = DecompiledFileType.TextureBmp
				Me.theFirstDecompiledFileHasBeenAdded = False
				AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

				status = model.WriteTextureFiles(Me.theModelOutputPath)

				RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress
				'Me.UpdateProgress(3, "... Writing texture files finished.")
			End If
		End If

		Return status
	End Function

	Private Sub WriteDebugFiles(ByVal model As SourceModel)
		Dim debugPath As String

		debugPath = TheApp.GetDebugPath(Me.theModelOutputPath, model.Name)
		FileManager.CreatePath(debugPath)

		Me.theDecompiledFileType = DecompiledFileType.Debug
		Me.theFirstDecompiledFileHasBeenAdded = False
		AddHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

		model.WriteAccessedBytesDebugFiles(debugPath)
		If Me.CancellationPending Then
			Return
		ElseIf Me.theSkipCurrentModelIsActive Then
			Return
		End If

		RemoveHandler model.SourceModelProgress, AddressOf Me.Model_SourceModelProgress

		'Dim debug3File As AppDebug3File
		'debug3File = New AppDebug3File()
		'debugPathFileName = Path.Combine(debugPathName, model.Name + " debug - unknown bytes.txt")
		'debug3File.WriteFile(debugPathFileName, model.MdlFileData.theUnknownValues)

		'	Dim debugFile As AppDebug1File
		'	debugFile = New AppDebug1File()
		'	debugPathFileName = Path.Combine(debugPathName, TheSourceEngineModel.ModelName + " debug - Structure info.txt")
	End Sub

	Private Sub UpdateProgressStart(ByVal line As String)
		Me.UpdateProgressInternal(0, line)
	End Sub

	Private Sub UpdateProgressStop(ByVal line As String)
		Me.UpdateProgressInternal(100, vbCr + line)
	End Sub

	Private Sub UpdateProgress()
		Me.UpdateProgressInternal(1, "")
	End Sub

	Private Sub UpdateProgress(ByVal indentLevel As Integer, ByVal line As String)
		Dim indentedLine As String

		indentedLine = ""
		For i As Integer = 1 To indentLevel
			indentedLine += "  "
		Next
		indentedLine += line
		Me.UpdateProgressInternal(1, indentedLine)
	End Sub

	Private Sub UpdateProgressInternal(ByVal progressValue As Integer, ByVal line As String)
		If progressValue = 1 AndAlso Me.theLogFileStream IsNot Nothing Then
			Try
				Me.theLogFileStream.WriteLine(line)
			Catch ex As IOException
				Me.theModelWriteFailed = True
				Me.CloseLog()
				Me.ReportProgress(1, "写出反编译日志失败 — " + ex.Message)
			End Try
		End If

		Me.ReportProgress(progressValue, line)
	End Sub

#End Region

#Region "Event Handlers"

	Private Sub Model_SourceModelProgress(ByVal sender As Object, ByVal e As SourceModelProgressEventArgs)
		If e.Progress = ProgressOptions.WarningPhyFileChecksumDoesNotMatchMdlFileChecksum Then
			'TODO: Test that this shows when needed.
			Me.UpdateProgress(4, "警告：PHY 文件的校验和值与 MDL 文件的校验和值不匹配。")
		'ElseIf e.Progress = ProgressOptions.WritingFileStarted Then
		'	Dim pathFileName As String
		'	Dim fileName As String
		'	pathFileName = e.Message
		'	fileName = Path.GetFileName(pathFileName)
		'	'TODO: Figure out how to rename SMD file if already written in a previous step, which might happen if, for example, an anim is named "<modelname>_reference" or "<modelname>_physics".
		'	'      Could also happen if the loop through SequenceDescs has already created the SMD file before the loop through AnimationDescs.
		'	'      The same name can be used by multiple sequences, as is the case for 3 "frontkick" sequences in Half-Life Opposing Force "massn.mdl".
		'	If TheApp.SmdFileNames.Contains(pathFileName) Then
		'		Dim model As SourceModel
		'		model = CType(sender, SourceModel)
		'		model.WritingSingleFileIsCanceled = True
		'		'Me.UpdateProgress(4, "WARNING: The file, """ + smdFileName + """, was written already in a previous step.")
		'		'Else
		'		'	Me.UpdateProgress(4, "Writing """ + fileName + """ file ...")
		'	End If
		ElseIf e.Progress = ProgressOptions.WritingFileFailed Then
			Me.theModelWriteFailed = True
			Me.UpdateProgress(4, e.Message)
		ElseIf e.Progress = ProgressOptions.WritingFileFinished Then
			Dim pathFileName As String
			Dim fileName As String
			pathFileName = e.Message
			If File.Exists(pathFileName) Then Me.theCommittedModelFiles.Add(pathFileName)
			fileName = Path.GetFileName(pathFileName)
			'Me.UpdateProgress(4, "... Writing """ + fileName + """ file finished.")
			Me.UpdateProgress(4, fileName)

			If Not Me.theFirstDecompiledFileHasBeenAdded AndAlso File.Exists(pathFileName) Then
				Dim relativePathFileName As String
				relativePathFileName = FileManager.GetRelativePathFileName(Me.theOutputPath, pathFileName)

				If Me.theDecompiledFileType = DecompiledFileType.ReferenceMesh Then
					Me.theDecompiledFirstRefSmdFiles.Add(relativePathFileName)
				ElseIf Me.theDecompiledFileType = DecompiledFileType.LodMesh Then
					Me.theDecompiledFirstLodSmdFiles.Add(relativePathFileName)
				ElseIf Me.theDecompiledFileType = DecompiledFileType.BoneAnimation Then
					Me.theDecompiledFirstBoneAnimSmdFiles.Add(relativePathFileName)
				ElseIf Me.theDecompiledFileType = DecompiledFileType.PhysicsMesh Then
					Me.theDecompiledPhysicsFiles.Add(relativePathFileName)
				ElseIf Me.theDecompiledFileType = DecompiledFileType.VertexAnimation Then
					Me.theDecompiledVtaFiles.Add(relativePathFileName)
				ElseIf Me.theDecompiledFileType = DecompiledFileType.TextureBmp Then
					Me.theDecompiledFirstTextureBmpFiles.Add(relativePathFileName)
				ElseIf Me.theDecompiledFileType = DecompiledFileType.Debug Then
					Me.theDecompiledFirstDebugFiles.Add(relativePathFileName)
				End If

				Me.theFirstDecompiledFileHasBeenAdded = True
			End If
			'TheApp.SmdFileNames.Add(pathFileName)

			Dim model As SourceModel
			model = CType(sender, SourceModel)
			If Me.CancellationPending Then
				'status = StatusMessage.Canceled
				model.WritingIsCanceled = True
				'ElseIf Me.theSkipCurrentModelIsActive Then
				'	'status = StatusMessage.Skipped
				'	model.WritingSingleFileIsCanceled = True
			End If
		Else
			Dim progressUnhandled As Integer = 4242
		End If
	End Sub

#End Region

#Region "Data"

	Private ReadOnly theCommittedModelFiles As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
	Private theModelWriteFailed As Boolean
	Private theSkipCurrentModelIsActive As Boolean
	Private theInputMdlPathName As String
	Private theOutputPath As String
	Private theModelOutputPath As String
	Private theOutputPathOrModelOutputFileName As String

	Private theLogFileStream As StreamWriter

	Private theDecompiledQcFiles As BindingListEx(Of String)
	Private theDecompiledFirstRefSmdFiles As BindingListEx(Of String)
	Private theDecompiledFirstLodSmdFiles As BindingListEx(Of String)
	Private theDecompiledPhysicsFiles As BindingListEx(Of String)
	Private theDecompiledVtaFiles As BindingListEx(Of String)
	Private theDecompiledFirstBoneAnimSmdFiles As BindingListEx(Of String)
	Private theDecompiledVrdFiles As BindingListEx(Of String)
	Private theDecompiledDeclareSequenceQciFiles As BindingListEx(Of String)
	Private theDecompiledFirstTextureBmpFiles As BindingListEx(Of String)
	Private theDecompiledLogFiles As BindingListEx(Of String)
	Private theDecompiledFirstDebugFiles As BindingListEx(Of String)

	Private theDecompiledFileType As AppEnums.DecompiledFileType
	Private theFirstDecompiledFileHasBeenAdded As Boolean

#End Region

End Class
