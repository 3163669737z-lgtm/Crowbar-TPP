Imports System.IO
Imports System.ComponentModel

Public MustInherit Class SourceModel

#Region "Shared"

	Public Shared Function Create(ByVal mdlPathFileName As String, ByVal overrideVersion As SupportedMdlVersion, ByRef version As Integer) As SourceModel
		Dim model As SourceModel = Nothing
		Dim storedVersion As Integer
		Dim id As String = Nothing

		Try
			storedVersion = SourceModel.GetVersion(mdlPathFileName, id)
			If overrideVersion = SupportedMdlVersion.DoNotOverride Then
				version = storedVersion
			Else
				version = CInt(EnumHelper.GetDescription(overrideVersion))
			End If

			If id = "MDLZ" Then
				If version = 14 Then
					Return New SourceModel14(mdlPathFileName, version) With {.HeaderVersion = storedVersion}
				End If
				Return Nothing
			End If


			If storedVersion = 53 AndAlso (overrideVersion = SupportedMdlVersion.DoNotOverride OrElse version = 49) Then
				model = TrySource49Layout(mdlPathFileName, storedVersion)
				If model IsNot Nothing Then
					version = 49
					Return model
				End If
			End If

			If version = 4 Then
				model = New SourceModel04(mdlPathFileName, version)
			ElseIf version = 6 Then
				model = New SourceModel06(mdlPathFileName, version)
			ElseIf version = 10 Then
				model = New SourceModel10(mdlPathFileName, version)
			ElseIf version = 2531 Then
				model = New SourceModel2531(mdlPathFileName, version)
			ElseIf version = 27 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel31(mdlPathFileName, version)
			ElseIf version = 28 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel31(mdlPathFileName, version)
			ElseIf version = 29 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel31(mdlPathFileName, version)
			ElseIf version = 30 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel31(mdlPathFileName, version)
			ElseIf version = 31 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel31(mdlPathFileName, version)
			ElseIf version = 32 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel32(mdlPathFileName, version)
			ElseIf version = 35 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel36(mdlPathFileName, version)
			ElseIf version = 36 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel36(mdlPathFileName, version)
			ElseIf version = 37 Then
				'NOT FULLY IMPLEMENTED YET.
				model = New SourceModel37(mdlPathFileName, version)
			ElseIf version = 44 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 45 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 46 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 47 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 48 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 49 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 52 Then
				'TODO: Finish.
				model = New SourceModel52(mdlPathFileName, version)
			ElseIf version = 53 Then
				'TODO: Finish.
				model = New SourceModel53(mdlPathFileName, version)
			ElseIf version = 54 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 55 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 56 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 58 Then
				model = New SourceModel49(mdlPathFileName, version)
			ElseIf version = 59 Then
				model = New SourceModel49(mdlPathFileName, version)
			Else
				If overrideVersion = SupportedMdlVersion.DoNotOverride Then
					model = TrySource49Layout(mdlPathFileName, storedVersion)
					If model IsNot Nothing Then version = 49
				End If
			End If
		Catch ex As Exception
			Throw
		End Try

		If model IsNot Nothing AndAlso Not model.HeaderVersion.HasValue Then
			model.HeaderVersion = storedVersion
			If overrideVersion <> SupportedMdlVersion.DoNotOverride Then model.LayoutReason = " — 手动指定版本"
		End If
		Return model
	End Function

	Private Shared Function TrySource49Layout(mdlPathFileName As String, declaredVersion As Integer) As SourceModel49
		Dim layout As Source49ExternalMesh = Source49ExternalMesh.Probe(mdlPathFileName)
		If layout Is Nothing Then Return Nothing
		If layout.IsMeshCompanion Then Throw New NotSupportedException("此 MDL 是配套网格。请选择包含骨骼和体组的主 MDL。")
		Return New SourceModel49(mdlPathFileName, 49, layout) With {
			.HeaderVersion = declaredVersion, .LayoutReason = " — 内联名称、文件长度、骨骼和体组表符合 Source v49"}
	End Function

	Private Shared Function GetVersion(mdlPathFileName As String, ByRef id As String) As Integer
		Dim version As Integer
		Dim inputFileStream As FileStream
		Dim inputFileReader As BinaryReader

		version = -1
		inputFileStream = Nothing
		inputFileReader = Nothing
		Try
			inputFileStream = New FileStream(mdlPathFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
			If inputFileStream IsNot Nothing Then
				Try
					'NOTE: Important to set System.Text.Encoding.ASCII so that ReadChars() only reads in one byte per Char.
					inputFileReader = New BinaryReader(inputFileStream, System.Text.Encoding.ASCII)

					id = inputFileReader.ReadChars(4)
					version = inputFileReader.ReadInt32()
					If id = "MDLZ" Then
						If version <> 14 Then
							Throw New FormatException("文件头 ID（文件前 4 字节）为 'MDLZ'（不含引号）的文件没有预期的 MDL 版本 14。MDL 文件不是 GoldSource 或 Source 引擎的 MDL 文件。")
						End If
					ElseIf id <> "IDST" Then
						Throw New FormatException("文件没有预期的 MDL 文件头 ID（文件前 4 字节）'IDST' 或 'MDLZ'（不含引号）。MDL 文件不是 GoldSource 或 Source 引擎的 MDL 文件。")
					End If
				Catch ex As FormatException
					Throw
				Catch ex As Exception
					'Dim debug As Integer = 4242
					Throw
				Finally
					If inputFileReader IsNot Nothing Then
						inputFileReader.Close()
					End If
				End Try
			End If
		Catch ex As FormatException
			Throw
		Catch ex As Exception
			'Dim debug As Integer = 4242
			Throw
		Finally
			If inputFileStream IsNot Nothing Then
				inputFileStream.Close()
			End If
		End Try

		Return version
	End Function

	'Private Shared version_shared As Integer

#End Region

#Region "Creation and Destruction"

	Protected Sub New(ByVal mdlPathFileName As String, ByVal mdlVersion As Integer)
		Me.theMdlPathFileName = mdlPathFileName
		Me.theName = Path.GetFileNameWithoutExtension(mdlPathFileName)
		Me.theVersion = mdlVersion
	End Sub

#End Region

#Region "Properties - Model Data"

	Public ReadOnly Property ID() As String
		Get
			Return Me.theMdlFileDataGeneric.theID
		End Get
	End Property

	Public ReadOnly Property Version() As Integer
		Get
			Return Me.theVersion
		End Get
	End Property
	Friend Property HeaderVersion As Integer?

	Public ReadOnly Property Name() As String
		Get
			Return Me.theName
		End Get
		'Set(ByVal value As String)
		'	Me.theName = value
		'End Set
	End Property

#End Region

#Region "Properties - File-Related"

	' The *Used properties should return whether the files are actually referred to by the MDL file.
	' For the PHY file and others that have no reference in the MDL file, simply return whether each file exists.

	Public Overridable ReadOnly Property SequenceGroupMdlFilesAreUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property TextureMdlFileIsUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property PhyFileIsUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property VtxFileIsUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property AniFileIsUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property VvdFileIsUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Property WritingIsCanceled As Boolean
		Get
			Return Me.theWritingIsCanceled
		End Get
		Set(value As Boolean)
			Me.theWritingIsCanceled = value
		End Set
	End Property

	'Public Property WritingSingleFileIsCanceled As Boolean
	'	Get
	'		Return Me.theWritingSingleFileIsCanceled
	'	End Get
	'	Set(value As Boolean)
	'		Me.theWritingSingleFileIsCanceled = value
	'	End Set
	'End Property

#End Region

#Region "Properties - Data Query"

	Public Overridable ReadOnly Property HasTextureData As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property HasMeshData As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property HasLodMeshData As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property HasPhysicsMeshData As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property HasProceduralBonesData As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property HasBoneAnimationData As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property HasVertexAnimationData As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overridable ReadOnly Property HasTextureFileData As Boolean
		Get
			Return False
		End Get
	End Property

#End Region

#Region "Methods"

	Public Overridable Function ReadMdlFileHeader() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If Not File.Exists(Me.theMdlPathFileName) Then
			status = StatusMessage.ErrorRequiredMdlFileNotFound
		End If

		If status = StatusMessage.Success Then
			Me.ReadFile(Me.theMdlPathFileName, AddressOf Me.ReadMdlFileHeader_Internal)
		End If

		Return status
	End Function

	Public Overridable Function CheckForRequiredFiles() As FilesFoundFlags
		Dim status As AppEnums.FilesFoundFlags

		status = FilesFoundFlags.AllFilesFound

		Return status
	End Function

	Public Overridable Function ReadAniFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = StatusMessage.Error

		Return status
	End Function

	Public Overridable Function ReadSequenceGroupMdlFiles() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = StatusMessage.Error

		Return status
	End Function

	Public Overridable Function ReadTextureMdlFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = StatusMessage.Error

		Return status
	End Function

	Public Overridable Function ReadPhyFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = StatusMessage.Error

		Return status
	End Function

	Public Overridable Function ReadMdlFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Try
			Me.ReadFile(Me.theMdlPathFileName, AddressOf Me.ReadMdlFile_Internal)
		Catch ex As Exception
			status = StatusMessage.Error
		End Try

		Return status
	End Function

	Public Overridable Function ReadVtxFileHeader() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If status = StatusMessage.Success Then
			Me.ReadFile(Me.theVtxPathFileName, AddressOf Me.ReadVtxFileHeader_Internal)
		End If

		Return status
	End Function

	Public Overridable Function ReadVtxFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		'If String.IsNullOrEmpty(Me.theVtxPathFileName) Then
		'	status = Me.CheckForRequiredFiles()
		'End If

		If status = StatusMessage.Success Then
			Me.ReadFile(Me.theVtxPathFileName, AddressOf Me.ReadVtxFile_Internal)
		End If

		Return status
	End Function

	Public Overridable Function ReadVvdFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		'If String.IsNullOrEmpty(Me.theVvdPathFileName) Then
		'	status = Me.CheckForRequiredFiles()
		'End If

		If status = StatusMessage.Success Then
			Me.ReadFile(Me.theVvdPathFileName, AddressOf Me.ReadVvdFile_Internal)
		End If

		Return status
	End Function

	'Public Overridable Function ReadMdlFileForViewer() As AppEnums.StatusMessage
	'	Dim status As AppEnums.StatusMessage = StatusMessage.Success

	'	If Not File.Exists(Me.theMdlPathFileName) Then
	'		status = StatusMessage.ErrorRequiredMdlFileNotFound
	'	End If

	'	If status = StatusMessage.Success Then
	'		Me.ReadFile(Me.theMdlPathFileName, AddressOf Me.ReadMdlFileForViewer_Internal)
	'	End If

	'	Return status
	'End Function

	Public Overridable Function SetAllSmdPathFileNames() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Function WriteQcFile(qcPathFileName As String) As StatusMessage
		Me.theQcPathFileName = qcPathFileName
		Return Me.WriteTextOutput(qcPathFileName, AddressOf Me.WriteQcFile)
	End Function

	Public Overridable Function WriteReferenceMeshFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Function WriteLodMeshFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Function WriteMeshSmdFile(ByVal smdPathFileName As String, ByVal lodIndex As Integer, ByVal aVtxModel As SourceVtxModel07, ByVal aModel As SourceMdlModel, ByVal bodyPartVertexIndexStart As Integer) As String
		Return Me.WriteTextFile(smdPathFileName, Sub() Me.WriteMeshSmdFile(lodIndex, aVtxModel, aModel, bodyPartVertexIndexStart))
	End Function

	Public Overridable Function WritePhysicsMeshSmdFile(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Function WriteBoneAnimationSmdFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Function WriteVertexAnimationVtaFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Function WriteBoneAnimationSmdFile(ByVal smdPathFileName As String, ByVal aSequenceDesc As SourceMdlSequenceDescBase, ByVal anAnimationDesc As SourceMdlAnimationDescBase) As String
		Return Me.WriteTextFile(smdPathFileName, Sub() Me.WriteBoneAnimationSmdFile(aSequenceDesc, anAnimationDesc))
	End Function

	Public Overridable Function WriteVertexAnimationVtaFile(ByVal vtaPathFileName As String, ByVal bodyPart As SourceMdlBodyPart) As String
		Return Me.WriteTextFile(vtaPathFileName, Sub() Me.WriteVertexAnimationVtaFile(bodyPart))
	End Function

	Public Overridable Function WriteVrdFile(ByVal vrdPathFileName As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Function WriteTextureFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = StatusMessage.Error

		Return status
	End Function

	Public Overridable Function WriteDeclareSequenceQciFile(ByVal declareSequenceQciPathFileName As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Return status
	End Function

	Public Overridable Sub WriteMdlFileNameToMdlFile(ByVal mdlPathFileName As String, ByVal internalMdlFileName As String)
		Me.ReadFile(mdlPathFileName, AddressOf Me.ReadMdlFileHeader_Internal)
		Me.WriteFile(mdlPathFileName, AddressOf Me.WriteMdlFileNameToMdlFile, internalMdlFileName, Me.theMdlFileDataGeneric)
	End Sub

	Public Overridable Sub WriteAniFileNameToMdlFile(ByVal mdlPathFileName As String, ByVal internalMdlFileName As String)
		Me.ReadFile(mdlPathFileName, AddressOf Me.ReadMdlFileHeader_Internal)
		Dim internalAniFileName As String
		internalAniFileName = Path.Combine("models", Path.ChangeExtension(internalMdlFileName, ".ani"))
		Me.WriteFile(mdlPathFileName, AddressOf Me.WriteAniFileNameToMdlFile, internalAniFileName, Me.theMdlFileDataGeneric)
	End Sub

	Public Overridable Function WriteAccessedBytesDebugFiles(ByVal debugPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = StatusMessage.Error

		Return status
	End Function

	Public Overridable Function GetOverviewTextLines(ByVal mdlPathFileName As String, ByVal mdlVersionOverride As SupportedMdlVersion) As List(Of String)
		Dim textLines As New List(Of String)()

		Try
			Me.ReadFile(mdlPathFileName, AddressOf Me.ReadMdlFileForViewer_Internal)

			Me.GetHeaderDataFromMdlFile(textLines, mdlVersionOverride)
			textLines.Add("")
			Me.GetModelFileDataFromMdlFile(textLines)
			textLines.Add("")
			Me.GetTextureDataFromMdlFile(textLines)
			'textLines.Add("")
			'Me.GetSequenceDataFromMdlFile(textLines)
		Catch ex As Exception
			'textLines.Add("ERROR: " + ex.Message)
			Throw
		End Try

		Return textLines
	End Function

	Public Overridable Function GetTextureFolders(ByVal mdlPathFileName As String) As List(Of String)
		Dim textureFolders As List(Of String) = Nothing

		Try
			Me.ReadFile(mdlPathFileName, AddressOf Me.ReadMdlFileForViewer_Internal)

			If Me.HasTextureData Then
				If Me.theMdlFileDataGeneric.version <= 10 Then
				Else
					textureFolders = Me.GetTextureFolders()
				End If
			Else
			End If
		Catch ex As Exception
			Throw
		End Try

		Return textureFolders
	End Function

	Public Overridable Function GetTextureFolders() As List(Of String)
		Return New List(Of String)()
	End Function

	Public Overridable Function GetTextureFileNames() As List(Of String)
		Return New List(Of String)()
	End Function

	Public Overridable Function GetSequenceInfo() As List(Of String)
		Return New List(Of String)()
	End Function

#End Region

#Region "Events"

	Public Event SourceModelProgress As SourceModelProgressEventHandler

#End Region

#Region "Protected Methods"

	Protected Overridable Sub ReadAniFile_Internal()

	End Sub

	Protected Overridable Sub ReadMdlFile_Internal()

	End Sub

	Protected Overridable Sub ReadPhyFile_Internal()

	End Sub

	Protected Overridable Function ReadSequenceGroupMdlFile(ByVal pathFileName As String, ByVal sequenceGroupIndex As Integer) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim inputFileStream As FileStream = Nothing
		Me.theInputFileReader = Nothing
		Try
			inputFileStream = New FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
			If inputFileStream IsNot Nothing Then
				Try
					Me.theInputFileReader = New BinaryReader(inputFileStream, System.Text.Encoding.ASCII)

					ReadSequenceGroupMdlFile(sequenceGroupIndex)
				Catch ex As Exception
					Throw
				Finally
					If Me.theInputFileReader IsNot Nothing Then
						Me.theInputFileReader.Close()
					End If
				End Try
			End If
		Catch ex As Exception
			Throw
		Finally
			If inputFileStream IsNot Nothing Then
				inputFileStream.Close()
			End If
		End Try

		Return status
	End Function

	Protected Overridable Sub ReadSequenceGroupMdlFile(ByVal sequenceGroupIndex As Integer)

	End Sub

	Protected Overridable Sub ReadTextureMdlFile_Internal()

	End Sub

	Protected Overridable Sub ReadVtxFileHeader_Internal()

	End Sub

	Protected Overridable Sub ReadVtxFile_Internal()

	End Sub

	Protected Overridable Sub ReadVvdFile_Internal()

	End Sub

	Protected Overridable Sub WriteQcFile()

	End Sub

	Protected Overridable Sub WriteMeshSmdFile(ByVal lodIndex As Integer, ByVal aVtxModel As SourceVtxModel07, ByVal aModel As SourceMdlModel, ByVal bodyPartVertexIndexStart As Integer)

	End Sub

	Protected Overridable Sub WritePhysicsMeshSmdFile()

	End Sub

	Protected Overridable Sub WriteBoneAnimationSmdFile(ByVal aSequenceDesc As SourceMdlSequenceDescBase, ByVal anAnimationDesc As SourceMdlAnimationDescBase)

	End Sub

	Protected Overridable Sub WriteVertexAnimationVtaFile(ByVal bodyPart As SourceMdlBodyPart)

	End Sub

	Protected Overridable Sub WriteVrdFile()

	End Sub

	Protected Overridable Sub WriteTextureFile()

	End Sub

	Protected Overridable Sub WriteDeclareSequenceQciFile()

	End Sub

	Protected Overridable Sub ReadMdlFileHeader_Internal()

	End Sub

	Protected Overridable Sub ReadMdlFileForViewer_Internal()

	End Sub

	Protected Overridable Sub WriteMdlFileNameToMdlFile(ByVal internalMdlFileName As String)

	End Sub

	Protected Overridable Sub WriteAniFileNameToMdlFile(ByVal internalAniFileName As String)

	End Sub

	Protected Sub ReadFile(ByVal pathFileName As String, ByVal readFileAction As ReadFileDelegate)
		Me.LastReadError = Nothing
		Try
			Using input As New FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
				Using reader As New BinaryReader(input, System.Text.Encoding.UTF8)
					Me.theInputFileReader = reader
					readFileAction()
				End Using
			End Using
		Catch ex As Exception
			Me.LastReadError = pathFileName + " — " + ex.Message
			Throw
		Finally
			Me.theInputFileReader = Nothing
		End Try
	End Sub

	Protected Sub WriteFile(ByVal pathFileName As String, ByVal writeFileAction As WriteFileDelegate, ByVal value As String, ByVal fileData As SourceFileData)
		Dim outputFileStream As FileStream = Nothing
		Try
			outputFileStream = New FileStream(pathFileName, FileMode.Open)
			If outputFileStream IsNot Nothing Then
				Try
					Me.theOutputFileBinaryWriter = New BinaryWriter(outputFileStream, System.Text.Encoding.ASCII)

					writeFileAction.Invoke(value)
				Catch ex As Exception
					Dim debug As Integer = 4242
				Finally
					If Me.theOutputFileBinaryWriter IsNot Nothing Then
						Me.theOutputFileBinaryWriter.Close()
					End If
				End Try
			End If
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
			If outputFileStream IsNot Nothing Then
				outputFileStream.Close()
			End If
		End Try
	End Sub

	Protected Function WriteTextOutput(outputPathFileName As String, write As WriteTextFileDelegate) As StatusMessage
		Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, outputPathFileName)
		Dim result As String = Me.WriteTextFile(outputPathFileName, write)
		If result = "Success" Then
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, outputPathFileName)
			Return StatusMessage.Success
		End If
		Me.NotifySourceModelProgress(ProgressOptions.WritingFileFailed, result)
		Return StatusMessage.Error
	End Function

	Protected Function WriteTextFile(ByVal outputPathFileName As String, ByVal writeTextFileAction As WriteTextFileDelegate) As String
		Try
			FileManager.WriteTextAtomically(outputPathFileName,
				Sub(writer)
					Me.theOutputFileTextWriter = writer
					writeTextFileAction()
				End Sub)
			Return "Success"
		Catch ex As Exception
			Return "写出失败 """ + outputPathFileName + """ — " + ex.Message
		Finally
			Me.theOutputFileTextWriter = Nothing
		End Try
	End Function

	Protected Sub NotifySourceModelProgress(ByVal progress As ProgressOptions, ByVal message As String)
		RaiseEvent SourceModelProgress(Me, New SourceModelProgressEventArgs(progress, message))
	End Sub

	Protected Function WriteAccessedBytesDebugFile(ByVal debugPathFileName As String, ByVal fileSeekLog As FileSeekLog) As String
		Dim status As String = "Success"

		Try
			Me.theOutputFileTextWriter = File.CreateText(debugPathFileName)

			Dim debugFile As New AccessedBytesDebugFile(Me.theOutputFileTextWriter)
			debugFile.WriteHeaderComment()
			debugFile.WriteFileSeekLog(fileSeekLog)
		Catch ex As PathTooLongException
			status = "错误：Crowbar 尝试创建 """ + debugPathFileName + """，但系统返回了此消息：" + ex.Message
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
			If Me.theOutputFileTextWriter IsNot Nothing Then
				Me.theOutputFileTextWriter.Flush()
				Me.theOutputFileTextWriter.Close()
			End If
		End Try

		Return status
	End Function

#End Region

#Region "Private Delegates"

	Public Delegate Sub SourceModelProgressEventHandler(ByVal sender As Object, ByVal e As SourceModelProgressEventArgs)

	Protected Delegate Sub ReadFileDelegate()
	Protected Delegate Sub WriteFileDelegate(ByVal value As String)
	Protected Delegate Sub WriteTextFileDelegate()

#End Region

#Region "Private Methods"

	Private Sub GetHeaderDataFromMdlFile(ByVal ioTextLines As List(Of String), ByVal mdlVersionOverride As SupportedMdlVersion)
		'Dim mdlFileData48 As SourceMdlFileData48 = Nothing
		'Dim mdlFileData49 As SourceMdlFileData49 = Nothing
		'If Me.theMdlFileDataGeneric.version = 48 Then
		'	mdlFileData48 = CType(Me.theMdlFileDataGeneric, SourceMdlFileData48)
		'ElseIf Me.theMdlFileDataGeneric.version = 49 Then
		'	mdlFileData49 = CType(Me.theMdlFileDataGeneric, SourceMdlFileData49)
		'End If

		ioTextLines.Add("=== 常规信息 ===")
		ioTextLines.Add("")

		Dim fileTypeId As String
		fileTypeId = Me.theMdlFileDataGeneric.theID
		If fileTypeId = "IDST" Then
			ioTextLines.Add("MDL file type ID: " + fileTypeId)
		Else
			ioTextLines.Add("错误：MDL 文件类型 ID 不是 IDST。这要么是损坏的 MDL 文件，要么不是 GoldSource 或 Source 模型文件。")
		End If

		Dim extraVersionText As String = ""
		If mdlVersionOverride <> SupportedMdlVersion.DoNotOverride Then
			extraVersionText = "   Model version override: " + EnumHelper.GetDescription(mdlVersionOverride)
		ElseIf Me.HeaderVersion.HasValue AndAlso Me.HeaderVersion.Value <> Me.Version Then
			extraVersionText = "   识别的数据布局：v" + Me.Version.ToString()
		End If
		ioTextLines.Add("MDL file version: " + Me.HeaderVersion.GetValueOrDefault(Me.theMdlFileDataGeneric.version).ToString("N0") + extraVersionText)

		ioTextLines.Add("MDL stored file name: """ + Me.theMdlFileDataGeneric.theModelName + """")
		'If mdlFileData48 IsNot Nothing AndAlso mdlFileData48.nameCopyOffset > 0 Then
		'	ioTextLines.Add("MDL stored file name copy: """ + mdlFileData48.theNameCopy + """")
		'End If
		'If mdlFileData49 IsNot Nothing AndAlso mdlFileData49.nameCopyOffset > 0 Then
		'	ioTextLines.Add("MDL stored file name copy: """ + mdlFileData49.theNameCopy + """")
		'End If

		Dim storedFileSize As Long
		Dim actualFileSize As Long
		storedFileSize = Me.theMdlFileDataGeneric.fileSize
		actualFileSize = Me.theMdlFileDataGeneric.theActualFileSize
		If storedFileSize > -1 Then
			ioTextLines.Add("MDL stored file size: " + storedFileSize.ToString("N0") + " bytes")
			ioTextLines.Add("MDL actual file size: " + actualFileSize.ToString("N0") + " bytes")
			If Me.theMdlFileDataGeneric.fileSize <> Me.theMdlFileDataGeneric.theActualFileSize Then
				ioTextLines.Add("警告：MDL 文件大小与内部存储的文件大小不同。这意味着 MDL 文件在编译后被更改过——可能是十六进制编辑导致损坏。")
			End If
		Else
			ioTextLines.Add("MDL file size: " + actualFileSize.ToString("N0") + " bytes")
		End If

		If Me.theMdlFileDataGeneric.theChecksumIsValid Then
			ioTextLines.Add("MDL checksum: " + Me.theMdlFileDataGeneric.checksum.ToString("X8"))
		End If
	End Sub

	Private Sub GetModelFileDataFromMdlFile(ByVal ioTextLines As List(Of String))
		Me.CheckForRequiredFiles()

		ioTextLines.Add("=== 模型文件 ===")
		ioTextLines.Add("")

		If Me.AniFileIsUsed Then
			If File.Exists(Me.theAniPathFileName) Then
				ioTextLines.Add("""" + Path.GetFileName(Me.theAniPathFileName) + """")
			Else
				ioTextLines.Add("错误：文件未找到：""" + Path.GetFileName(Me.theAniPathFileName) + """")
			End If
		End If

		ioTextLines.Add("""" + Path.GetFileName(Me.theMdlPathFileName) + """")

		'TODO: For GoldSource, list all SequenceGroup MDL files.
		'If Me.SequenceGroupMdlFilesAreUsed Then
		'	'If File.Exists(Me.thePhyPathFileName) Then
		'	'	ioTextLines.Add("""" + Path.GetFileName(Me.thePhyPathFileName) + """")
		'	'Else
		'	'	ioTextLines.Add("ERROR: File not found: """ + Path.GetFileName(Me.thePhyPathFileName) + """")
		'	'End If
		'End If

		If Me.TextureMdlFileIsUsed Then
			If File.Exists(Me.theTextureMdlPathFileName) Then
				ioTextLines.Add("""" + Path.GetFileName(Me.theTextureMdlPathFileName) + """")
			Else
				ioTextLines.Add("错误：文件未找到：""" + Path.GetFileName(Me.theTextureMdlPathFileName) + """")
			End If
		End If

		If Me.PhyFileIsUsed Then
			If File.Exists(Me.thePhyPathFileName) Then
				ioTextLines.Add("""" + Path.GetFileName(Me.thePhyPathFileName) + """")
			Else
				ioTextLines.Add("错误：文件未找到：""" + Path.GetFileName(Me.thePhyPathFileName) + """")
			End If
		End If

		'TODO: List all vtx files, not just the one used for decompiling.
		If Me.VtxFileIsUsed Then
			If File.Exists(Me.theVtxPathFileName) Then
				ioTextLines.Add("""" + Path.GetFileName(Me.theVtxPathFileName) + """")
			Else
				ioTextLines.Add("错误：文件未找到：""" + Path.GetFileName(Me.theVtxPathFileName) + """")
			End If
		End If

		If Me.VvdFileIsUsed Then
			If File.Exists(Me.theVvdPathFileName) Then
				ioTextLines.Add("""" + Path.GetFileName(Me.theVvdPathFileName) + """")
			Else
				ioTextLines.Add("错误：文件未找到：""" + Path.GetFileName(Me.theVvdPathFileName) + """")
			End If
		End If
	End Sub

	Private Sub GetTextureDataFromMdlFile(ByVal ioTextLines As List(Of String))
		ioTextLines.Add("=== 材质与纹理信息 ===")
		ioTextLines.Add("")

		If Me.HasTextureData Then
			If Me.theMdlFileDataGeneric.version <= 10 Then
				If Me.TextureMdlFileIsUsed Then
					ioTextLines.Add("Texture files are stored within the separate 't' MDL file: " + Path.GetFileName(Me.theTextureMdlPathFileName))
				Else
					ioTextLines.Add("Texture files are stored within the MDL file.")
				End If
			Else
				ioTextLines.Add("材质文件夹（QC 文件中的 $CDMaterials 行 -- VMT 文件应所在、相对于游戏 ""materials"" 文件夹的文件夹）：")
				Dim textureFolders As List(Of String)
				textureFolders = Me.GetTextureFolders()
				If textureFolders.Count = 0 Then
					ioTextLines.Add("未设置材质文件夹。")
				Else
					For Each aTextureFolder As String In textureFolders
						ioTextLines.Add("""" + aTextureFolder + """")
					Next
				End If
			End If

			ioTextLines.Add("")

			ioTextLines.Add("材质文件名（网格 SMD 文件及 QC $texturegroup 命令中的文件名）：")
			Dim textureFileNames As List(Of String)
			textureFileNames = Me.GetTextureFileNames()
			ioTextLines.Add("(Total used: " + textureFileNames.Count.ToString() + ")")
			If textureFileNames.Count = 0 Then
				ioTextLines.Add("未找到材质文件名。")
			Else
				For Each aTextureFileName As String In textureFileNames
					ioTextLines.Add("""" + aTextureFileName + """")
				Next
			End If
		Else
			'ioTextLines.Add("No texture data because this model only has animation data.")
			ioTextLines.Add("没有纹理数据。")
		End If
	End Sub

	'Private Sub GetSequenceDataFromMdlFile(ByVal ioTextLines As List(Of String))
	'	ioTextLines.Add("=== Sequence Info ===")
	'	ioTextLines.Add("")

	'	If Me.HasBoneAnimationData Then
	'		If Me.theMdlFileDataGeneric.version <= 10 Then
	'		Else
	'			Dim sequenceNames As List(Of String)
	'			sequenceNames = Me.GetSequenceInfo()
	'			For Each aSequenceName As String In sequenceNames
	'				ioTextLines.Add("""" + aSequenceName + """")
	'			Next
	'		End If
	'	Else
	'		ioTextLines.Add("No sequence data.")
	'	End If
	'End Sub

#End Region

#Region "Data"

	Protected theVersion As Integer
	Protected theName As String
	'Protected thePhysicsMeshSmdFileName As String

	Protected theMdlFileDataGeneric As SourceMdlFileDataBase
	Protected theAniFileDataGeneric As SourceFileData
	Protected thePhyFileDataGeneric As SourcePhyFileData

	Friend Property LayoutReason As String = ""
	Friend Property MeshResourceRoot As String
	Friend ReadOnly CompatibilityDiagnostics As New List(Of String)
	Friend ReadOnly NameDiagnostics As New List(Of String)
	Friend ReadOnly ReadDiagnostics As New List(Of String)
	Friend Property LastReadError As String

	Protected theInputFileReader As BinaryReader
	Protected theOutputFileBinaryWriter As BinaryWriter
	Protected theOutputFileTextWriter As StreamWriter

	Protected theWritingIsCanceled As Boolean
	Protected theWritingSingleFileIsCanceled As Boolean

	Protected theAniPathFileName As String
	Protected thePhyPathFileName As String
	Protected theMdlPathFileName As String
	Protected theSequenceGroupMdlPathFileNames As List(Of String)
	Protected theTextureMdlPathFileName As String
	Protected theVtxPathFileName As String
	Protected theVvdPathFileName As String

	Protected theQcPathFileName As String

#End Region

End Class
