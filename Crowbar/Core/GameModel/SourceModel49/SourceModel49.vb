Imports System.IO

Public Class SourceModel49
	Inherits SourceModel37

#Region "Creation and Destruction"

	Public Sub New(ByVal mdlPathFileName As String, ByVal mdlVersion As Integer)
		MyBase.New(mdlPathFileName, mdlVersion)
	End Sub

	Friend Sub New(mdlPathFileName As String, mdlVersion As Integer, externalMesh As Source49ExternalMesh)
		Me.New(mdlPathFileName, mdlVersion)
		Me.theExternalMesh = externalMesh
	End Sub

	Private ReadOnly theExternalMesh As Source49ExternalMesh
	Private theMeshLayout As Source49ExternalMesh

#End Region

#Region "Properties"

	Public Overrides ReadOnly Property PhyFileIsUsed As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.thePhyPathFileName) AndAlso File.Exists(Me.thePhyPathFileName)
		End Get
	End Property

	Public Overrides ReadOnly Property VtxFileIsUsed As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.theVtxPathFileName) AndAlso File.Exists(Me.theVtxPathFileName)
		End Get
	End Property

	Public Overrides ReadOnly Property AniFileIsUsed As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.theAniPathFileName) AndAlso File.Exists(Me.theAniPathFileName)
		End Get
	End Property

	Public Overrides ReadOnly Property VvdFileIsUsed As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.theVvdPathFileName) AndAlso File.Exists(Me.theVvdPathFileName)
		End Get
	End Property

	Public Overrides ReadOnly Property HasTextureData As Boolean
		Get
			Return Not Me.theMdlFileDataGeneric.theMdlFileOnlyHasAnimations AndAlso Me.theMdlFileData.theTextures IsNot Nothing AndAlso Me.theMdlFileData.theTextures.Count > 0
		End Get
	End Property

	Public Overrides ReadOnly Property HasMeshData As Boolean
		Get
			If Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
					 AndAlso Me.theMdlFileData.theBones IsNot Nothing _
					 AndAlso Me.theMdlFileData.theBones.Count > 0 _
					 AndAlso Me.theVtxFileData IsNot Nothing AndAlso Me.theVvdFileData49 IsNot Nothing Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasLodMeshData As Boolean
		Get
			If Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
					 AndAlso Me.theMdlFileData.theBones IsNot Nothing _
					 AndAlso Me.theMdlFileData.theBones.Count > 0 _
					 AndAlso Me.theVtxFileData IsNot Nothing _
					 AndAlso Me.theVvdFileData49 IsNot Nothing _
					 AndAlso Me.theVtxFileData.lodCount > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasPhysicsMeshData As Boolean
		Get
			If Me.thePhyFileDataGeneric IsNot Nothing _
			 AndAlso Me.thePhyFileDataGeneric.theSourcePhyCollisionDatas IsNot Nothing _
			 AndAlso Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
			 AndAlso Me.theMdlFileData.theBones IsNot Nothing _
			 AndAlso Me.theMdlFileData.theBones.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasProceduralBonesData As Boolean
		Get
			If Me.theMdlFileData IsNot Nothing _
			 AndAlso Me.theMdlFileData.theProceduralBonesCommandIsUsed _
			 AndAlso Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
			 AndAlso Me.theMdlFileData.theBones IsNot Nothing _
			 AndAlso Me.theMdlFileData.theBones.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasBoneAnimationData As Boolean
		Get
			If Me.theMdlFileData.theAnimationDescs IsNot Nothing _
			 AndAlso Me.theMdlFileData.theAnimationDescs.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasVertexAnimationData As Boolean
		Get
			If Me.HasMeshData _
			 AndAlso Me.theMdlFileData.theFlexDescs IsNot Nothing _
			 AndAlso Me.theMdlFileData.theFlexDescs.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

#End Region

#Region "Methods"

	Public Overrides Function CheckForRequiredFiles() As FilesFoundFlags
		Dim status As AppEnums.FilesFoundFlags = FilesFoundFlags.AllFilesFound

		If Me.theMdlFileData.animBlockCount > 0 Then
			Me.theAniPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".ani")
			If Not File.Exists(Me.theAniPathFileName) Then
				status = status Or FilesFoundFlags.ErrorRequiredAniFileNotFound
			End If
		End If

		'If Not Me.theMdlFileDataGeneric.theMdlFileOnlyHasAnimations Then
		Me.thePhyPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".phy")

		Me.theVtxPathFileName = Nothing
		Me.theVvdPathFileName = Nothing
		Me.CompatibilityDiagnostics.Clear()
		If Me.theMeshLayout IsNot Nothing Then
			If Not Me.theMeshLayout.FindFiles(Me.theVtxPathFileName, Me.theVvdPathFileName, Me.MeshResourceRoot, Me.CompatibilityDiagnostics) Then
				status = status Or FilesFoundFlags.ErrorRequiredVtxFileNotFound Or FilesFoundFlags.ErrorRequiredVvdFileNotFound
				Me.CompatibilityDiagnostics.Add("网格不可用，可以导出独立的骨骼、动画和元数据。")
			End If
		ElseIf Not Me.theMdlFileData.theMdlFileOnlyHasAnimations Then
			For Each extension As String In Source49ExternalMesh.VtxExtensions
				Dim candidate As String = Path.ChangeExtension(Me.theMdlPathFileName, extension)
				If Not File.Exists(candidate) Then Continue For
				Me.theVtxPathFileName = candidate
				Try
					If Me.VtxFileHasSameChecksum() Then Exit For
					Me.CompatibilityDiagnostics.Add("跳过 VTX 候选 — " + candidate + " — 校验和与 MDL 不符。")
				Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
					Me.CompatibilityDiagnostics.Add("跳过 VTX 候选 — " + candidate + " — " + ex.Message)
				End Try
				Me.theVtxPathFileName = Nothing
			Next
			Me.theVtxFileData = Nothing
			If Me.theVtxPathFileName Is Nothing Then status = status Or FilesFoundFlags.ErrorRequiredVtxFileNotFound
			Dim vvd As String = Path.ChangeExtension(Me.theMdlPathFileName, ".vvd")
			If File.Exists(vvd) Then
				Me.theVvdPathFileName = vvd
			Else
				status = status Or FilesFoundFlags.ErrorRequiredVvdFileNotFound
			End If
		End If

		Return status
	End Function

	Public Overrides Function ReadMdlFileHeader() As StatusMessage
		Dim status As StatusMessage = MyBase.ReadMdlFileHeader()
		If status = StatusMessage.Success AndAlso Me.theMdlFileDataGeneric IsNot Nothing AndAlso
			Me.theMdlFileDataGeneric.fileSize <> Me.theMdlFileDataGeneric.theActualFileSize Then
			Return StatusMessage.ErrorInvalidInternalMdlFileSize
		End If
		Return status
	End Function

	Public Overrides Function ReadVtxFile() As StatusMessage
		Try
			Return MyBase.ReadVtxFile()
		Catch
			Me.theVtxFileData = Nothing
			Throw
		End Try
	End Function

	Public Overrides Function ReadVvdFile() As StatusMessage
		Try
			Return MyBase.ReadVvdFile()
		Catch
			Me.theVvdFileData49 = Nothing
			Me.theVtxFileData = Nothing
			Throw
		End Try
	End Function

	Public Overrides Function ReadPhyFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		'If String.IsNullOrEmpty(Me.thePhyPathFileName) Then
		'	status = Me.CheckForRequiredFiles()
		'End If

		If Not String.IsNullOrEmpty(Me.thePhyPathFileName) Then
			If status = StatusMessage.Success Then
				Try
					Me.ReadFile(Me.thePhyPathFileName, AddressOf Me.ReadPhyFile_Internal)
					If Me.thePhyFileDataGeneric.checksum <> Me.theMdlFileData.checksum Then
						'status = StatusMessage.WarningPhyChecksumDoesNotMatchMdl
						Me.NotifySourceModelProgress(ProgressOptions.WarningPhyFileChecksumDoesNotMatchMdlFileChecksum, "")
					End If
				Catch ex As Exception
					status = StatusMessage.Error
				End Try
			End If
		End If

		Return status
	End Function

	Public Overrides Function ReadAniFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		'If String.IsNullOrEmpty(Me.theAniPathFileName) Then
		'	status = Me.CheckForRequiredFiles()
		'End If

		If Not String.IsNullOrEmpty(Me.theAniPathFileName) Then
			If status = StatusMessage.Success Then
				Try
					Me.ReadFile(Me.theAniPathFileName, AddressOf Me.ReadAniFile_Internal)
				Catch ex As Exception
					status = StatusMessage.Error
				End Try
			End If
		End If

		Return status
	End Function

	Public Overrides Function WriteReferenceMeshFiles(modelOutputPath As String) As StatusMessage
		If Me.CanWriteSkeletonOnly AndAlso Not Me.HasMeshData Then
			Dim skeletonPath As String = Path.Combine(modelOutputPath, Me.Name + "_skeleton.smd")
			Return Me.WriteTextOutput(skeletonPath,
				Sub()
					Dim smd As New SourceSmdFile49(Me.theOutputFileTextWriter, Me.theMdlFileData)
					smd.WriteHeaderComment()
					smd.WriteHeaderSection()
					smd.WriteNodesSection()
					smd.WriteSkeletonSection(0)
				End Sub)
		End If
		Return Me.WriteMeshSmdFiles(modelOutputPath, 0, 0)
	End Function

	Friend ReadOnly Property CanWriteSkeletonOnly As Boolean
		Get
			Return Me.theMeshLayout IsNot Nothing AndAlso Me.theMeshLayout.NeedsMesh AndAlso
				Me.theMdlFileData.theBones IsNot Nothing AndAlso Me.theMdlFileData.theBones.Count > 0
		End Get
	End Property

	Public Overrides Function WriteLodMeshFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = Me.WriteMeshSmdFiles(modelOutputPath, 1, Me.theVtxFileData.lodCount - 1)

		Return status
	End Function

	Public Overrides Function WriteBoneAnimationSmdFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim smdPathFileName As String
		Dim smdPath As String
		Dim writeStatus As String

		Try
			Me.EnsureDeltaBaseAnimationDesc()

			If Me.theMdlFileData.theCorrectiveAnimationDescs IsNot Nothing Then
				For Each anAnimationDesc As SourceMdlAnimationDesc49 In Me.theMdlFileData.theCorrectiveAnimationDescs
					smdPathFileName = Path.Combine(modelOutputPath, SourceFileNamesModule.CreateCorrectiveAnimationSmdRelativePathFileName(anAnimationDesc.theName, Me.Name))
					smdPath = FileManager.GetPath(smdPathFileName)

					If FileManager.PathExistsAfterTryToCreate(smdPath) Then
						Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, smdPathFileName)
						'NOTE: Check here in case writing is canceled in the above event.
						If Me.theWritingIsCanceled Then
							status = StatusMessage.Canceled
							Return status
						ElseIf Me.theWritingSingleFileIsCanceled Then
							Me.theWritingSingleFileIsCanceled = False
							Continue For
						End If

						writeStatus = "Failed"
						If Me.theMdlFileData.theDeltaBaseAnimationDesc IsNot Nothing Then
							writeStatus = Me.WriteCorrectiveAnimationSmdFile(smdPathFileName, Nothing, Me.theMdlFileData.theDeltaBaseAnimationDesc, False)
						Else
							writeStatus = Me.WriteCorrectiveAnimationSmdFile(smdPathFileName, Nothing, anAnimationDesc, True)
						End If
						If writeStatus = "Success" Then
							Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, smdPathFileName)
						Else
							Me.NotifySourceModelProgress(ProgressOptions.WritingFileFailed, writeStatus)
						End If
					End If
					Exit For
				Next
			End If

			For Each anAnimationDesc As SourceMdlAnimationDesc49 In Me.theMdlFileData.theAnimationDescs
				smdPathFileName = Path.Combine(modelOutputPath, anAnimationDesc.GetSmdRelativePathFileName(Me.Name))
				smdPath = FileManager.GetPath(smdPathFileName)
				If FileManager.PathExistsAfterTryToCreate(smdPath) Then
					Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, smdPathFileName)
					'NOTE: Check here in case writing is canceled in the above event.
					If Me.theWritingIsCanceled Then
						status = StatusMessage.Canceled
						Return status
					ElseIf Me.theWritingSingleFileIsCanceled Then
						Me.theWritingSingleFileIsCanceled = False
						Continue For
					End If

					writeStatus = Me.WriteBoneAnimationSmdFile(smdPathFileName, Nothing, anAnimationDesc)

					If writeStatus = "Success" Then
						Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, smdPathFileName)
					Else
						Me.NotifySourceModelProgress(ProgressOptions.WritingFileFailed, writeStatus)
					End If
				End If
			Next
		Catch ex As Exception
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFailed, modelOutputPath + " — " + ex.Message)
			status = StatusMessage.Error
		End Try

		Return status
	End Function

	Public Overrides Function WriteVrdFile(vrdPathFileName As String) As StatusMessage
		Return Me.WriteTextOutput(vrdPathFileName, AddressOf Me.WriteVrdFile)
	End Function

	Public Overrides Function WriteDeclareSequenceQciFile(declareSequenceQciPathFileName As String) As StatusMessage
		Return Me.WriteTextOutput(declareSequenceQciPathFileName, AddressOf Me.WriteDeclareSequenceQciFile)
	End Function

	Public Overrides Function WriteVertexAnimationVtaFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim aBodyPart As SourceMdlBodyPart
		Dim vtaFileName As String
		Dim vtaPath As String
		Dim vtaPathFileName As String

		Try
			For aBodyPartIndex As Integer = 0 To Me.theMdlFileData.theBodyParts.Count - 1
				aBodyPart = Me.theMdlFileData.theBodyParts(aBodyPartIndex)

				If aBodyPart.theExportFlexVariantsAsDmx OrElse aBodyPart.theFlexFrames Is Nothing OrElse aBodyPart.theFlexFrames.Count <= 1 Then
					Continue For
				End If

				vtaFileName = SourceFileNamesModule.GetVtaFileName(Me.Name, aBodyPartIndex)
				vtaPathFileName = Path.Combine(modelOutputPath, vtaFileName)
				vtaPath = FileManager.GetPath(vtaPathFileName)
				If FileManager.PathExistsAfterTryToCreate(vtaPath) Then
					Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, vtaPathFileName)
					'NOTE: Check here in case writing is canceled in the above event.
					If Me.theWritingIsCanceled Then
						status = StatusMessage.Canceled
						Return status
					ElseIf Me.theWritingSingleFileIsCanceled Then
						Me.theWritingSingleFileIsCanceled = False
						Continue For
					End If

					Me.WriteVertexAnimationVtaFile(vtaPathFileName, aBodyPart)

					Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, vtaPathFileName)
				End If
			Next
		Catch ex As Exception
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFailed, modelOutputPath + " — " + ex.Message)
			status = StatusMessage.Error
		End Try

		Return status
	End Function

	Public Overrides Function WriteAccessedBytesDebugFiles(ByVal debugPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim debugPathFileName As String

		If Me.theMdlFileDataGeneric IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugMdlFileNameSuffix)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, debugPathFileName)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theMdlFileDataGeneric.theFileSeekLog)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, debugPathFileName)
		End If

		If Me.theAniFileDataGeneric IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugAniFileNameSuffix)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, debugPathFileName)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theAniFileDataGeneric.theFileSeekLog)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, debugPathFileName)
		End If

		If Me.theVtxFileData IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugVtxFileNameSuffix)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, debugPathFileName)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theVtxFileData.theFileSeekLog)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, debugPathFileName)
		End If

		If Me.theVvdFileData49 IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugVvdFileNameSuffix)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, debugPathFileName)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theVvdFileData49.theFileSeekLog)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, debugPathFileName)
		End If

		If Me.thePhyFileDataGeneric IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugPhyFileNameSuffix)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, debugPathFileName)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.thePhyFileDataGeneric.theFileSeekLog)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, debugPathFileName)
		End If

		Return status
	End Function

	Public Overrides Function GetTextureFolders() As List(Of String)
		Dim textureFolders As New List(Of String)()

		For i As Integer = 0 To Me.theMdlFileData.theTexturePaths.Count - 1
			Dim aTextureFolder As String
			aTextureFolder = Me.theMdlFileData.theTexturePaths(i)

			textureFolders.Add(aTextureFolder)
		Next

		Return textureFolders
	End Function

	Public Overrides Function GetTextureFileNames() As List(Of String)
		Dim textureFileNames As New List(Of String)()

		For i As Integer = 0 To Me.theMdlFileData.theTextures.Count - 1
			Dim aTexture As SourceMdlTexture
			aTexture = Me.theMdlFileData.theTextures(i)

			textureFileNames.Add(aTexture.thePathFileName)
		Next

		Return textureFileNames
	End Function

	'Public Overrides Function GetSequenceInfo() As List(Of String)
	'	Dim sequenceFileNames As New List(Of String)()

	'	For i As Integer = 0 To Me.theMdlFileData.theSequenceDescs.Count - 1
	'		Dim aSequence As SourceMdlSequenceDesc
	'		aSequence = Me.theMdlFileData.theSequenceDescs(i)

	'		sequenceFileNames.Add(aSequence.theName)
	'	Next

	'	Return sequenceFileNames
	'End Function

#End Region

#Region "Private Methods"

	Protected Overrides Sub ReadAniFile_Internal()
		If Me.theAniFileData49 Is Nothing Then
			'Me.theAniFileData49 = New SourceAniFileData49()
			Me.theAniFileData49 = New SourceMdlFileData49()
			Me.theAniFileDataGeneric = Me.theAniFileData49
		End If

		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData49()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim aniFile As New SourceAniFile49(Me.theInputFileReader, Me.theAniFileData49, Me.theMdlFileData)

		aniFile.ReadMdlHeader00("ANI File Header 00")
		aniFile.ReadMdlHeader01("ANI File Header 01")

		aniFile.ReadAnimationAniBlocks()
		aniFile.ReadUnreadBytes()
	End Sub

	Protected Overrides Sub ReadMdlFile_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData49()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile49(Me.theInputFileReader, Me.theMdlFileData, Me.ReadDiagnostics, Me.NameDiagnostics)

		Me.theMdlFileData.theSectionFrameCount = 0
		'Me.theMdlFileData.theModelCommandIsUsed = False
		Me.theMdlFileData.theProceduralBonesCommandIsUsed = False
		Me.theMdlFileData.theAnimBlockSizeNoStallOptionIsUsed = False

		mdlFile.ReadMdlHeader00("MDL File Header 00", If(Me.theExternalMesh IsNot Nothing, Me.Version, 0))
		mdlFile.ReadMdlHeader01("MDL File Header 01")
		If Me.theMdlFileData.studioHeader2Offset > 0 Then
			Me.theInputFileReader.BaseStream.Position = Me.theMdlFileData.studioHeader2Offset
			mdlFile.ReadMdlHeader02("MDL File Header 02")
		End If

		' Read what WriteBoneInfo() writes.
		mdlFile.ReadBones()
		mdlFile.ReadBoneControllers()
		mdlFile.ReadAttachments()

		mdlFile.ReadHitboxSets()

		mdlFile.ReadBoneTableByName()

		' Read what WriteAnimations() writes.
		If Me.theMdlFileData.localAnimationCount > 0 Then
			Try
				mdlFile.ReadLocalAnimationDescs()
				mdlFile.ReadAnimationSections()
				mdlFile.ReadAnimationMdlBlocks()
			Catch ex As Exception
				Me.ReadDiagnostics.Add("读取 MDL 动画 — " + ex.Message)
			End Try
		End If

		' Read what WriteSequenceInfo() writes.
		mdlFile.ReadSequenceDescs()
		mdlFile.ReadLocalNodeNames()
		mdlFile.ReadLocalNodes()

		' Read what WriteModel() writes.
		'Me.theCurrentFrameIndex = 0
		'NOTE: Read flex descs before body parts so that flexes (within body parts) can add info to flex descs.
		mdlFile.ReadFlexDescs()
		mdlFile.ReadBodyParts()
		mdlFile.ReadFlexControllers()
		'NOTE: This must be after flex descs are read so that flex desc usage can be saved in flex desc.
		mdlFile.ReadFlexRules()
		mdlFile.ReadIkChains()
		mdlFile.ReadIkLocks()
		mdlFile.ReadMouths()
		mdlFile.ReadPoseParamDescs()
		mdlFile.ReadModelGroups()
		'TODO: Me.ReadAnimBlocks()
		'TODO: Me.ReadAnimBlockName()

		' Read what WriteTextures() writes.
		mdlFile.ReadTexturePaths()
		'NOTE: ReadTextures must be after ReadTexturePaths(), so it can compare with the texture paths.
		mdlFile.ReadTextures()
		mdlFile.ReadSkinFamilies()

		' Read what WriteKeyValues() writes.
		mdlFile.ReadKeyValues()

		' Read what WriteBoneTransforms() writes.
		mdlFile.ReadBoneTransforms()
		mdlFile.ReadLinearBoneTable()

		'TODO: ReadLocalIkAutoPlayLocks()
		mdlFile.ReadFlexControllerUis()

		mdlFile.ReadBodygroupPresets()

		'mdlFile.ReadFinalBytesAlignment()
		'mdlFile.ReadUnknownValues(Me.theMdlFileData.theFileSeekLog)
		mdlFile.ReadUnreadBytes()

		' Post-processing.
		mdlFile.PostProcess()
		mdlFile.NormalizeNamesForExport()
		mdlFile.CreateFlexFrameList()
		If Me.theMdlFileData.version = 49 AndAlso (Me.theMdlFileData.flags And SourceMdlFileData.STUDIOHDR_FLAGS_STATIC_PROP) = 0 AndAlso
			Me.theMdlFileData.theBodyParts IsNot Nothing AndAlso
			Me.theMdlFileData.theBodyParts.Exists(Function(part) part.theModels IsNot Nothing AndAlso part.theModels.Count = 1 AndAlso part.theFlexFrames IsNot Nothing AndAlso part.theFlexFrames.Count > 1) Then
			For Each part As SourceMdlBodyPart In Me.theMdlFileData.theBodyParts
				part.theExportFlexVariantsAsDmx = SourceDmxBodyModel49.CanExportBodyPart(part)
			Next
		End If
		Common.ProcessTexturePaths(Me.theMdlFileData.theTexturePaths, Me.theMdlFileData.theTextures, Me.theMdlFileData.theModifiedTexturePaths, Me.theMdlFileData.theModifiedTextureFileNames)
	End Sub

	Protected Overrides Sub ReadPhyFile_Internal()
		If Me.thePhyFileDataGeneric Is Nothing Then
			Me.thePhyFileDataGeneric = New SourcePhyFileData()
		End If

		Dim phyFile As New SourcePhyFile(Me.theInputFileReader, Me.thePhyFileDataGeneric)

		phyFile.ReadSourcePhyHeader()
		If Me.thePhyFileDataGeneric.solidCount > 0 Then
			phyFile.ReadSourceCollisionData()
			phyFile.CalculateVertexNormals()
			phyFile.ReadSourcePhysCollisionModels()
			phyFile.ReadSourcePhyRagdollConstraintDescs()
			phyFile.ReadSourcePhyCollisionRules()
			phyFile.ReadSourcePhyAnimatedFrictionSection()
			phyFile.ReadSourcePhyEditParamsSection()
			phyFile.ReadCollisionTextSection()
		End If
		phyFile.ReadUnreadBytes()
	End Sub

	Private Function VtxFileHasSameChecksum() As Boolean
		Me.ReadVtxFileHeader()
		Return Me.theMdlFileData.checksum = Me.theVtxFileData.checksum
	End Function

	Protected Overrides Sub ReadVtxFileHeader_Internal()
		If Me.theVtxFileData Is Nothing Then
			Me.theVtxFileData = New SourceVtxFileData07()
		End If

		'TEST: When a model has a nameCopy, it seems to also use the VTF file strip group topology fields.
		Dim vtxFile As New SourceVtxFile07(Me.theInputFileReader, Me.theVtxFileData)

		vtxFile.ReadSourceVtxHeader()
	End Sub

	Protected Overrides Sub ReadVtxFile_Internal()
		If Me.theVtxFileData Is Nothing Then
			Me.theVtxFileData = New SourceVtxFileData07()
		End If

		'TEST: When a model has a nameCopy, it seems to also use the VTF file strip group topology fields.
		Dim vtxFile As New SourceVtxFile07(Me.theInputFileReader, Me.theVtxFileData, 0, Me.theMeshLayout?.UsesExtendedStrips)

		vtxFile.ReadSourceVtxHeader()
		'TODO: Why is this "If" statement needed?
		If Me.theVtxFileData.lodCount > 0 Then
			vtxFile.ReadSourceVtxBodyParts()
		End If
		vtxFile.ReadSourceVtxMaterialReplacementLists()
		vtxFile.ReadUnreadBytes()
	End Sub

	Protected Overrides Sub ReadVvdFile_Internal()
		If Me.theVvdFileData49 Is Nothing Then
			Me.theVvdFileData49 = New SourceVvdFileData04()
		End If

		Dim vvdFile As New SourceVvdFile04(Me.theInputFileReader, Me.theVvdFileData49)

		vvdFile.ReadSourceVvdHeader()
		vvdFile.ReadVertexes(Me.theMdlFileData.version)
		vvdFile.ReadTangents()
		vvdFile.ReadFixups()
		vvdFile.ReadExtraData()
		vvdFile.ReadUnreadBytes()
	End Sub

	Protected Overrides Sub WriteQcFile()
		'Dim qcFile As New SourceQcFile49(Me.theOutputFileTextWriter, Me.theQcPathFileName, Me.theMdlFileData, Me.theVtxFileData, Me.thePhyFileDataGeneric, Me.theAniFileData49, Me.theName)
		Dim qcFile As New SourceQcFile49(Me.theOutputFileTextWriter, Me.theQcPathFileName, Me.theMdlFileData, Me.theVtxFileData, Me.thePhyFileDataGeneric, Me.theName)
		qcFile.GroupCommitted = Sub(pathFileName) Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, pathFileName)

		Try
			qcFile.WriteHeaderComment()

			qcFile.WriteModelNameCommand()

			qcFile.WriteUpAxisCommand()
			qcFile.WriteStaticPropCommand()
			qcFile.WriteConstantDirectionalLightCommand()

			'If Me.theMdlFileData.theModelCommandIsUsed Then
			'	qcFile.WriteModelCommand()
			'	qcFile.WriteBodyGroupCommand(1)
			'Else
			'	qcFile.WriteBodyGroupCommand(0)
			'End If
			'qcFile.WriteModelCommand()
			qcFile.WriteBodyGroupCommand()
			qcFile.WriteBodyGroupPresetCommand(Me.theExternalMesh IsNot Nothing)
			qcFile.WriteGroup("lod", AddressOf qcFile.WriteGroupLod, False, False)

			qcFile.WriteSurfacePropCommand()
			qcFile.WriteJointSurfacePropCommand()
			qcFile.WriteContentsCommand()
			qcFile.WriteJointContentsCommand()
			qcFile.WriteIllumPositionCommand()

			qcFile.WriteEyePositionCommand()
			qcFile.WriteMaxEyeDeflectionCommand()
			qcFile.WriteNoForcedFadeCommand()
			qcFile.WriteForcePhonemeCrossfadeCommand()

			qcFile.WriteAmbientBoostCommand()
			qcFile.WriteOpaqueCommand()
			qcFile.WriteObsoleteCommand()
			qcFile.WriteCastTextureShadowsCommand()
			qcFile.WriteDoNotCastShadowsCommand()
			qcFile.WriteCdMaterialsCommand()
			qcFile.WriteTextureGroupCommand()
			If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
				qcFile.WriteTextureFileNameComments()
			End If

			qcFile.WriteAttachmentCommand()

			qcFile.WriteGroup("box", AddressOf qcFile.WriteGroupBox, True, False)

			qcFile.WriteControllerCommand()
			qcFile.WriteScreenAlignCommand()

			qcFile.WriteGroup("bone", AddressOf qcFile.WriteGroupBone, False, False)

			Me.SetUpCorrectiveSubtractAnimationBlocks()
			qcFile.WriteGroup("animation", AddressOf qcFile.WriteGroupAnimation, False, False)

			qcFile.WriteGroup("collision", AddressOf qcFile.WriteGroupCollision, False, False)

			Dim command As String
			If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
				command = "$KeyValues"
			Else
				command = "$keyvalues"
			End If
			qcFile.WriteKeyValues(Me.theMdlFileData.theKeyValuesText, command)
		Catch ex As Exception
			Throw
		Finally
		End Try
	End Sub

	Protected Overrides Sub ReadMdlFileHeader_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData49()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		If Me.Version = 49 Then
			Me.theMeshLayout = Source49ExternalMesh.ReadLayout(Me.theInputFileReader, Me.theMdlPathFileName, Me.theExternalMesh IsNot Nothing)
			Me.theInputFileReader.BaseStream.Position = 0
		End If
		Dim mdlFile As New SourceMdlFile49(Me.theInputFileReader, Me.theMdlFileData, Me.ReadDiagnostics, Me.NameDiagnostics)

		mdlFile.ReadMdlHeader00("MDL File Header 00", If(Me.theExternalMesh IsNot Nothing, Me.Version, 0))
		mdlFile.ReadMdlHeader01("MDL File Header 01")
		If Me.theMdlFileData.studioHeader2Offset > 0 Then
			Me.theInputFileReader.BaseStream.Position = Me.theMdlFileData.studioHeader2Offset
			mdlFile.ReadMdlHeader02("MDL File Header 02")
		End If

		'If Me.theMdlFileData.fileSize <> Me.theMdlFileData.theActualFileSize Then
		'	status = StatusMessage.ErrorInvalidInternalMdlFileSize
		'End If
	End Sub

	Protected Overrides Sub ReadMdlFileForViewer_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData49()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile49(Me.theInputFileReader, Me.theMdlFileData, Me.ReadDiagnostics, Me.NameDiagnostics)

		mdlFile.ReadMdlHeader00("MDL File Header 00", If(Me.theExternalMesh IsNot Nothing, Me.Version, 0))
		mdlFile.ReadMdlHeader01("MDL File Header 01")
		If Me.theMdlFileData.studioHeader2Offset > 0 Then
			Me.theInputFileReader.BaseStream.Position = Me.theMdlFileData.studioHeader2Offset
			mdlFile.ReadMdlHeader02("MDL File Header 02")
		End If

		mdlFile.ReadTexturePaths()
		mdlFile.ReadTextures()
		mdlFile.ReadSequenceDescs()
	End Sub

	Protected Overrides Function WriteMeshSmdFiles(ByVal modelOutputPath As String, ByVal lodStartIndex As Integer, ByVal lodStopIndex As Integer) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim smdFileName As String
		Dim smdPathFileName As String
		Dim aVtxBodyPart As SourceVtxBodyPart07
		Dim aVtxBodyModel As SourceVtxModel07
		Dim aBodyModel As SourceMdlModel
		Dim bodyPartVertexIndexStart As Integer
		Dim extraUvChannelCount As Integer

		bodyPartVertexIndexStart = 0
		If Me.theVtxFileData.theVtxBodyParts IsNot Nothing AndAlso Me.theMdlFileData.theBodyParts IsNot Nothing Then
			For bodyPartIndex As Integer = 0 To Me.theVtxFileData.theVtxBodyParts.Count - 1
				aVtxBodyPart = Me.theVtxFileData.theVtxBodyParts(bodyPartIndex)

				If aVtxBodyPart.theVtxModels IsNot Nothing Then
					For modelIndex As Integer = 0 To aVtxBodyPart.theVtxModels.Count - 1
						aVtxBodyModel = aVtxBodyPart.theVtxModels(modelIndex)

						If aVtxBodyModel.theVtxModelLods IsNot Nothing Then
							aBodyModel = Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex)
							If aBodyModel.name(0) = ChrW(0) AndAlso aVtxBodyModel.theVtxModelLods(0).theVtxMeshes Is Nothing Then
								Continue For
							End If

							For lodIndex As Integer = lodStartIndex To lodStopIndex
								'TODO: Why would this count be different than the file header count?
								If lodIndex >= aVtxBodyModel.theVtxModelLods.Count Then
									Exit For
								End If

								If lodIndex = 0 AndAlso Me.theVvdFileData49.theExtraDatas IsNot Nothing AndAlso Me.theVvdFileData49.theExtraDatas.Count > 0 Then
									extraUvChannelCount = Me.theVvdFileData49.theExtraDatas.Count
								Else
									extraUvChannelCount = 0
								End If

								For extraUvChannelIndex As Integer = -1 To extraUvChannelCount - 1
									Dim part As SourceMdlBodyPart = Me.theMdlFileData.theBodyParts(bodyPartIndex)
									smdFileName = SourceFileNamesModule.CreateBodyGroupSmdFileName(aBodyModel.theSmdFileNames(lodIndex), bodyPartIndex, modelIndex, lodIndex, Me.theName, aBodyModel.name, extraUvChannelIndex, part.theName)
									smdPathFileName = Path.Combine(modelOutputPath, smdFileName)
									If lodIndex = 0 AndAlso extraUvChannelIndex = -1 Then
										Dim storedName As String = CStr(aBodyModel.name).Trim(ChrW(0))
										Dim baseName As String = storedName.Substring(storedName.LastIndexOfAny({"/"c, "\"c}) + 1)
										If Not String.Equals(baseName, smdFileName, StringComparison.OrdinalIgnoreCase) Then
											Me.NameDiagnostics.Add("mesh[" + bodyPartIndex.ToString() + "," + modelIndex.ToString() + "] — 导出文件名 """ + smdFileName + """")
										End If
									End If

									Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, smdPathFileName)
									'NOTE: Check here in case writing is canceled in the above event.
									If Me.theWritingIsCanceled Then
										status = StatusMessage.Canceled
										Return status
									ElseIf Me.theWritingSingleFileIsCanceled Then
										Me.theWritingSingleFileIsCanceled = False
										Continue For
									End If

									Me.theExtraUvChannelIndex = extraUvChannelIndex
									Dim smdStatus As String = Me.WriteMeshSmdFile(smdPathFileName, lodIndex, aVtxBodyModel, aBodyModel, bodyPartVertexIndexStart)
									If smdStatus <> "Success" Then
										Me.NotifySourceModelProgress(ProgressOptions.WritingFileFailed, smdStatus)
										status = StatusMessage.Error
									Else
										Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, smdPathFileName)
									End If

									If lodIndex = 0 AndAlso extraUvChannelIndex = -1 AndAlso part.theExportFlexVariantsAsDmx AndAlso SourceDmxBodyModel49.HasFlexes(aBodyModel) Then
										Dim dmxPath As String = Path.ChangeExtension(smdPathFileName, ".dmx")
										Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, dmxPath)
										Try
											FileManager.WriteTextAtomically(dmxPath, Sub(output)
												Dim dmx As New SourceDmxBodyModel49(output)
												dmx.Write(Path.GetFileNameWithoutExtension(dmxPath), Me.theMdlFileData, Me.theVvdFileData49, aBodyModel,
														  aVtxBodyModel.theVtxModelLods(0), bodyPartVertexIndexStart)
											End Sub)
										Catch ex As Exception
											Me.NotifySourceModelProgress(ProgressOptions.WritingFileFailed, dmxPath + " " + ex.Message)
											status = StatusMessage.Error
											Continue For
										End Try
										Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, dmxPath)
									End If
								Next
							Next

							bodyPartVertexIndexStart += aBodyModel.vertexCount
						End If
					Next
				End If
			Next
		End If

		Return status
	End Function

	Protected Overrides Sub WriteMeshSmdFile(ByVal lodIndex As Integer, ByVal aVtxModel As SourceVtxModel07, ByVal aModel As SourceMdlModel, ByVal bodyPartVertexIndexStart As Integer)
		Dim smdFile As New SourceSmdFile49(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.theVvdFileData49)

		smdFile.WriteHeaderComment()

		smdFile.WriteHeaderSection()
		smdFile.WriteNodesSection()
		smdFile.WriteSkeletonSection(lodIndex)
		smdFile.WriteTrianglesSection(aVtxModel, lodIndex, aModel, bodyPartVertexIndexStart, Me.theExtraUvChannelIndex)
	End Sub

	Protected Overrides Sub WritePhysicsMeshSmdFile()
		Dim physicsMeshSmdFile As New SourceSmdFile49(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.thePhyFileDataGeneric)

		physicsMeshSmdFile.WriteHeaderComment()

		physicsMeshSmdFile.WriteHeaderSection()
		physicsMeshSmdFile.WriteNodesSection()
		physicsMeshSmdFile.WriteSkeletonSection(-1)
		physicsMeshSmdFile.WriteTrianglesSectionForPhysics()
	End Sub

	Protected Overrides Sub WriteVrdFile()
		Dim vrdFile As New SourceVrdFile49(Me.theOutputFileTextWriter, Me.theMdlFileData)

		vrdFile.WriteHeaderComment()
		vrdFile.WriteCommands()
	End Sub

	Protected Overrides Sub WriteDeclareSequenceQciFile()
		Dim qciFile As New SourceQcFile49(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.theName)

		qciFile.WriteHeaderComment()

		qciFile.WriteQciDeclareSequenceLines()
	End Sub

	Protected Overrides Sub WriteBoneAnimationSmdFile(ByVal aSequenceDesc As SourceMdlSequenceDescBase, ByVal anAnimationDesc As SourceMdlAnimationDescBase)
		Dim smdFile As New SourceSmdFile49(Me.theOutputFileTextWriter, Me.theMdlFileData)

		smdFile.WriteHeaderComment()

		smdFile.WriteHeaderSection()
		smdFile.WriteNodesSection()
		smdFile.WriteSkeletonSectionForAnimation(aSequenceDesc, anAnimationDesc)
	End Sub

	Protected Overrides Sub WriteVertexAnimationVtaFile(ByVal bodyPart As SourceMdlBodyPart)
		Dim vertexAnimationVtaFile As New SourceVtaFile49(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.theVvdFileData49, bodyPart)

		vertexAnimationVtaFile.WriteHeaderComment()

		vertexAnimationVtaFile.WriteHeaderSection()
		vertexAnimationVtaFile.WriteNodesSection()
		vertexAnimationVtaFile.WriteSkeletonSectionForVertexAnimation()
		vertexAnimationVtaFile.WriteVertexAnimationSection()
		Dim debug As Integer = 4242
	End Sub

	Protected Overrides Sub WriteMdlFileNameToMdlFile(ByVal internalMdlFileName As String)
		Dim mdlFile As New SourceMdlFile49(Me.theOutputFileBinaryWriter, Me.theMdlFileData)

		mdlFile.WriteInternalMdlFileName(internalMdlFileName)
		mdlFile.WriteInternalMdlFileNameCopy(internalMdlFileName)
	End Sub

	Protected Overrides Sub WriteAniFileNameToMdlFile(ByVal internalAniFileName As String)
		Dim mdlFile As New SourceMdlFile49(Me.theOutputFileBinaryWriter, Me.theMdlFileData)

		mdlFile.WriteInternalAniFileName(internalAniFileName)
	End Sub

	Private Sub SetUpCorrectiveSubtractAnimationBlocks()
		If Me.theMdlFileData.theSequenceDescs IsNot Nothing Then
			Dim anAnimationDesc As SourceMdlAnimationDesc49
			Dim name As String
			Me.theMdlFileData.theCorrectiveAnimationDescs = New List(Of SourceMdlAnimationDesc49)()

			For Each aSequenceDesc As SourceMdlSequenceDesc In Me.theMdlFileData.theSequenceDescs
				If (aSequenceDesc.flags And SourceMdlAnimationDesc.STUDIO_DELTA) > 0 Then
					If aSequenceDesc.theAnimDescIndexes IsNot Nothing AndAlso aSequenceDesc.theAnimDescIndexes.Count > 0 Then
						For j As Integer = 0 To aSequenceDesc.theAnimDescIndexes.Count - 1
							anAnimationDesc = Me.theMdlFileData.theAnimationDescs(aSequenceDesc.theAnimDescIndexes(j))
							name = anAnimationDesc.theName

							If name = "" OrElse name(0) = "@" Then
								'NOTE: There should only be one implied anim desc.
								'aSequenceDesc.theCorrectiveSubtractAnimationOptionIsUsed = True
								aSequenceDesc.theCorrectiveAnimationName = SourceFileNamesModule.CreateCorrectiveAnimationName(name)
							Else
								'anAnimationDesc.theCorrectiveSubtractAnimationOptionIsUsed = True
								anAnimationDesc.theCorrectiveAnimationName = SourceFileNamesModule.CreateCorrectiveAnimationName(name)
							End If
							If Not Me.theMdlFileData.theCorrectiveAnimationDescs.Contains(anAnimationDesc) Then
								Me.theMdlFileData.theCorrectiveAnimationDescs.Add(anAnimationDesc)
							End If
						Next
					End If
				End If
			Next
		End If

		Me.EnsureDeltaBaseAnimationDesc()
	End Sub

	Private Sub EnsureDeltaBaseAnimationDesc()
		If Me.theMdlFileData Is Nothing OrElse Me.theMdlFileData.theAnimationDescs Is Nothing Then
			Return
		End If
		If Me.theMdlFileData.theDeltaBaseAnimationDesc IsNot Nothing Then
			Return
		End If

		Dim hasDelta As Boolean = False
		For Each anAnim As SourceMdlAnimationDesc49 In Me.theMdlFileData.theAnimationDescs
			If (anAnim.flags And SourceMdlAnimationDesc.STUDIO_DELTA) > 0 Then
				hasDelta = True
				Exit For
			End If
		Next
		If Not hasDelta AndAlso Me.theMdlFileData.theSequenceDescs IsNot Nothing Then
			For Each aSeq As SourceMdlSequenceDesc In Me.theMdlFileData.theSequenceDescs
				If (aSeq.flags And SourceMdlAnimationDesc.STUDIO_DELTA) > 0 Then
					hasDelta = True
					Exit For
				End If
			Next
		End If

		If hasDelta Then
			Me.theMdlFileData.theDeltaBaseAnimationDesc = Me.SelectDeltaBaseAnimationDesc()
		End If
	End Sub

	Private Function SelectDeltaBaseAnimationDesc() As SourceMdlAnimationDesc49
		Dim bestAnim As SourceMdlAnimationDesc49 = Nothing
		Dim bestScore As Integer = -1

		For Each anAnim As SourceMdlAnimationDesc49 In Me.theMdlFileData.theAnimationDescs
			If (anAnim.flags And SourceMdlAnimationDesc.STUDIO_DELTA) > 0 Then
				Continue For
			End If
			If (anAnim.flags And SourceMdlAnimationDesc.STUDIO_ALLZEROS) > 0 Then
				Continue For
			End If
			If anAnim.frameCount <= 0 Then
				Continue For
			End If

			Dim cleanName As String = If(anAnim.theName, "").Trim()
			If cleanName.StartsWith("@") Then
				cleanName = cleanName.Substring(1)
			End If
			Dim lowerName As String = cleanName.ToLowerInvariant()

			Dim score As Integer = 20
			If lowerName = "idle_raw" Then
				score = 100
			ElseIf lowerName.Contains("idle_raw") Then
				score = 90
			ElseIf lowerName = "idle" OrElse lowerName = "a_idle" OrElse lowerName = "a_idle_1" Then
				score = 80
			ElseIf lowerName.Contains("idle") AndAlso Not lowerName.Contains("layer") Then
				score = 70
			ElseIf lowerName = "ref" OrElse lowerName = "reference" OrElse lowerName = "ragdoll" OrElse lowerName = "bindpose" Then
				score = 60
			ElseIf anAnim.theAnimIsLinkedToSequence AndAlso anAnim.theLinkedSequences IsNot Nothing AndAlso anAnim.theLinkedSequences.Count > 0 Then
				If (anAnim.theLinkedSequences(0).flags And SourceMdlAnimationDesc.STUDIO_DELTA) = 0 Then
					score = 40
				End If
			End If

			If score > bestScore Then
				bestScore = score
				bestAnim = anAnim
			End If
		Next

		Return bestAnim
	End Function

	Private Function WriteCorrectiveAnimationSmdFile(smdPathFileName As String, sequence As SourceMdlSequenceDescBase, animation As SourceMdlAnimationDescBase, Optional onlyWriteRootBones As Boolean = True) As String
		Return Me.WriteTextFile(smdPathFileName,
			Sub()
				Dim smd As New SourceSmdFile49(Me.theOutputFileTextWriter, Me.theMdlFileData)
				smd.WriteHeaderComment()
				smd.WriteHeaderSection()
				smd.WriteNodesSection()
				smd.WriteSkeletonSectionForAnimation(sequence, animation, onlyWriteRootBones, 1)
			End Sub)
	End Function

#End Region

#Region "Data"

	'Private theAniFileData49 As SourceAniFileData49
	Private theAniFileData49 As SourceMdlFileData49
	Private theMdlFileData As SourceMdlFileData49
	'Private thePhyFileData49 As SourcePhyFileData
	Private theVtxFileData As SourceVtxFileData07
	Private theVvdFileData49 As SourceVvdFileData04

	'Private theCorrectiveAnimationDescs As List(Of SourceMdlAnimationDesc49)
	Private theExtraUvChannelIndex As Integer

#End Region

End Class
