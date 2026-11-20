Imports System.IO

' Example: PLAYER from HLAlpha
Public Class SourceModel04
	Inherits SourceModel

#Region "Creation and Destruction"

	Public Sub New(ByVal mdlPathFileName As String, ByVal mdlVersion As Integer)
		MyBase.New(mdlPathFileName, mdlVersion)
	End Sub

#End Region

#Region "Properties"

	Public Overrides ReadOnly Property HasMeshData As Boolean
		Get
			If Me.theMdlFileData.theBones IsNot Nothing _
					 AndAlso Me.theMdlFileData.theBones.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasBoneAnimationData As Boolean
		Get
			If Me.theMdlFileData.theSequenceDescs IsNot Nothing _
			 AndAlso Me.theMdlFileData.theSequenceDescs.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasTextureFileData As Boolean
		Get
			Return Me.theMdlFileData.theBodyParts IsNot Nothing AndAlso Me.theMdlFileData.theBodyParts.Count > 0
		End Get
	End Property

#End Region

#Region "Methods"

	Public Overrides Function CheckForRequiredFiles() As FilesFoundFlags
		Dim status As AppEnums.FilesFoundFlags = FilesFoundFlags.AllFilesFound

		Return status
	End Function

	Public Overrides Function WriteReferenceMeshFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim aBodyPart As SourceMdlBodyPart04
		Dim aBodyModel As SourceMdlModel04
		'Dim smdFileName As String
		Dim smdPathFileName As String
		If Me.theMdlFileData.theBodyParts IsNot Nothing Then
			For bodyPartIndex As Integer = 0 To Me.theMdlFileData.theBodyParts.Count - 1
				aBodyPart = Me.theMdlFileData.theBodyParts(bodyPartIndex)

				If aBodyPart.theModels IsNot Nothing Then
					For modelIndex As Integer = 0 To aBodyPart.theModels.Count - 1
						aBodyModel = aBodyPart.theModels(modelIndex)

						aBodyModel.theSmdFileName = SourceFileNamesModule.CreateBodyGroupSmdFileName(aBodyModel.theSmdFileName, bodyPartIndex, modelIndex, 0, Me.theName, "")
						smdPathFileName = Path.Combine(modelOutputPath, aBodyModel.theSmdFileName)

						Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, smdPathFileName)
						'NOTE: Check here in case writing is canceled in the above event.
						If Me.theWritingIsCanceled Then
							status = StatusMessage.Canceled
							Return status
						ElseIf Me.theWritingSingleFileIsCanceled Then
							Me.theWritingSingleFileIsCanceled = False
							Continue For
						End If

						Me.WriteMeshSmdFile(smdPathFileName, aBodyModel)

						Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, smdPathFileName)
					Next
				End If
			Next
		End If

		Return status
	End Function

	Public Overrides Function WriteBoneAnimationSmdFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim aSequenceDesc As SourceMdlSequenceDesc04
		Dim smdPath As String
		Dim smdPathFileName As String

		Try
			For aSequenceIndex As Integer = 0 To Me.theMdlFileData.theSequenceDescs.Count - 1
				aSequenceDesc = Me.theMdlFileData.theSequenceDescs(aSequenceIndex)

				aSequenceDesc.theSmdRelativePathFileName = SourceFileNamesModule.CreateAnimationSmdRelativePathFileName(aSequenceDesc.theSmdRelativePathFileName, Me.theName, aSequenceDesc.theName, -1)

				smdPathFileName = Path.Combine(modelOutputPath, aSequenceDesc.theSmdRelativePathFileName)
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

					Me.WriteBoneAnimationSmdFile(smdPathFileName, aSequenceDesc)

					Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, smdPathFileName)
				End If
			Next
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try

		Return status
	End Function

	Public Overrides Function WriteTextureFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim aBodyPart As SourceMdlBodyPart04
		Dim aModel As SourceMdlModel04
		Dim aMesh As SourceMdlMesh04
		Dim texturePath As String
		Dim textureFileName As String
		Dim texturePathFileName As String
		For bodyPartIndex As Integer = 0 To Me.theMdlFileData.theBodyParts.Count - 1
			aBodyPart = Me.theMdlFileData.theBodyParts(bodyPartIndex)
			For modelIndex As Integer = 0 To aBodyPart.theModels.Count - 1
				aModel = aBodyPart.theModels(modelIndex)
				For meshIndex As Integer = 0 To aModel.theMeshes.Count - 1
					aMesh = aModel.theMeshes(meshIndex)
					Try
						texturePath = modelOutputPath
						'textureFileName = "bodypart" + bodyPartIndex.ToString() + "_model" + modelIndex.ToString() + "_mesh" + meshIndex.ToString() + ".bmp"
						textureFileName = aMesh.theTextureFileName
						texturePathFileName = Path.Combine(texturePath, textureFileName)
						If FileManager.PathExistsAfterTryToCreate(texturePath) Then
							Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, texturePathFileName)
							'NOTE: Check here in case writing is canceled in the above event.
							If Me.theWritingIsCanceled Then
								status = StatusMessage.Canceled
								Return status
							ElseIf Me.theWritingSingleFileIsCanceled Then
								Me.theWritingSingleFileIsCanceled = False
								Continue For
							End If

							Dim aBitmap As New BitmapFile(texturePathFileName, aMesh.textureWidth, aMesh.textureHeight, aMesh.theTextureBmpData)
							aBitmap.Write()

							Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, texturePathFileName)
						End If
					Catch ex As Exception
						status = StatusMessage.Error
					End Try
				Next
			Next
		Next

		Return status
	End Function

	Public Overrides Function WriteAccessedBytesDebugFiles(ByVal debugPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim debugPathFileName As String

		If Me.theMdlFileData IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugMdlFileNameSuffix)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, debugPathFileName)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theMdlFileData.theFileSeekLog)
			Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, debugPathFileName)
		End If

		Return status
	End Function

#End Region

#Region "Private Methods"

	Protected Overrides Sub ReadMdlFileHeader_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData04()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile04(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader()
	End Sub

	Protected Overrides Sub ReadMdlFileForViewer_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData04()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If


		Dim mdlFile As New SourceMdlFile04(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader()
	End Sub

	Protected Overrides Sub ReadMdlFile_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData04()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If


		Dim mdlFile As New SourceMdlFile04(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader()

		mdlFile.ReadBones()
		mdlFile.ReadSequenceDescs()
		mdlFile.ReadBodyParts()
		Me.BuildReferencePose()

		mdlFile.ReadUnreadBytes()

		' Post-processing.
		'mdlFile.GetBoneDataFromFirstSequenceFirstFrame()
	End Sub

	Private Sub BuildReferencePose()
		Dim data As SourceMdlFileData04 = Me.theMdlFileData
		If data.theBones Is Nothing Then
			Return
		End If
		Dim firstFrame As SourceMdlSequence04 = Nothing
		If data.theSequenceDescs IsNot Nothing Then
			For Each sequence As SourceMdlSequenceDesc04 In data.theSequenceDescs
				If sequence.theSequences IsNot Nothing AndAlso sequence.theSequences.Count > 0 Then
					firstFrame = sequence.theSequences(0)
					Exit For
				End If
			Next
		End If

		data.theReferencePose = New List(Of SourceMdlSequenceValue04)(data.theBones.Count)
		data.theBoneTransforms = New List(Of SourceBoneTransform10)(data.theBones.Count)
		For boneIndex As Integer = 0 To data.theBones.Count - 1
			Dim bone As SourceMdlBone04 = data.theBones(boneIndex)
			Dim position As SourceVector = bone.position
			Dim rotation As New SourceVector()
			If firstFrame IsNot Nothing Then
				position = firstFrame.thePositionsAndRotations(boneIndex).position
				rotation = firstFrame.thePositionsAndRotations(boneIndex).rotation
			End If
			If bone.parentBoneIndex = -1 Then
				position = New SourceVector With {.x = position.y, .y = -position.x, .z = position.z}
				rotation = New SourceVector With {.x = rotation.x, .y = rotation.y, .z = rotation.z - Math.PI / 2}
			End If
			data.theReferencePose.Add(New SourceMdlSequenceValue04 With {.position = position, .rotation = rotation})

			Dim local As New SourceBoneTransform10()
			MathModule.AngleMatrix(rotation.y, rotation.z, rotation.x, local.matrixColumn0, local.matrixColumn1, local.matrixColumn2, local.matrixColumn3)
			local.matrixColumn3 = position
			If bone.parentBoneIndex = -1 Then
				data.theBoneTransforms.Add(local)
			Else
				Dim parent As SourceBoneTransform10 = data.theBoneTransforms(bone.parentBoneIndex)
				Dim world As New SourceBoneTransform10()
				MathModule.R_ConcatTransforms(parent.matrixColumn0, parent.matrixColumn1, parent.matrixColumn2, parent.matrixColumn3,
					local.matrixColumn0, local.matrixColumn1, local.matrixColumn2, local.matrixColumn3,
					world.matrixColumn0, world.matrixColumn1, world.matrixColumn2, world.matrixColumn3)
				data.theBoneTransforms.Add(world)
			End If
		Next
	End Sub

	Protected Overrides Sub WriteQcFile()
		Dim output As StreamWriter = Me.theOutputFileTextWriter
		output.WriteLine("$modelname ""{0}.mdl""", Me.theName)
		If Me.theMdlFileData.theBodyParts IsNot Nothing Then
			For bodyIndex As Integer = 0 To Me.theMdlFileData.theBodyParts.Count - 1
				Dim part As SourceMdlBodyPart04 = Me.theMdlFileData.theBodyParts(bodyIndex)
				output.WriteLine("$bodygroup ""bodypart{0}""", bodyIndex)
				output.WriteLine("{")
				For modelIndex As Integer = 0 To part.theModels.Count - 1
					Dim model As SourceMdlModel04 = part.theModels(modelIndex)
					model.theSmdFileName = SourceFileNamesModule.CreateBodyGroupSmdFileName(model.theSmdFileName, bodyIndex, modelIndex, 0, Me.theName, "")
					output.WriteLine("    studio ""{0}""", model.theSmdFileName)
				Next
				output.WriteLine("}")
			Next
		End If
		If Me.theMdlFileData.theSequenceDescs IsNot Nothing Then
			For Each sequence As SourceMdlSequenceDesc04 In Me.theMdlFileData.theSequenceDescs
				sequence.theSmdRelativePathFileName = SourceFileNamesModule.CreateAnimationSmdRelativePathFileName(sequence.theSmdRelativePathFileName, Me.theName, sequence.theName, -1)
				output.WriteLine("$sequence ""{0}"" ""{1}""", sequence.theName, sequence.theSmdRelativePathFileName)
			Next
		End If
	End Sub

	Protected Overloads Function WriteMeshSmdFile(ByVal smdPathFileName As String, ByVal aModel As SourceMdlModel04) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Try
			Me.theOutputFileTextWriter = File.CreateText(smdPathFileName)

			Me.WriteMeshSmdFile(aModel)
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

	Protected Overloads Sub WriteMeshSmdFile(ByVal aModel As SourceMdlModel04)
		Dim externalTexturesAreUsed As Boolean = False

		Dim smdFile As New SourceSmdFile04(Me.theOutputFileTextWriter, Me.theMdlFileData)

		Try
			smdFile.WriteHeaderComment()

			smdFile.WriteHeaderSection()
			smdFile.WriteNodesSection()
			smdFile.WriteSkeletonSection()
			smdFile.WriteTrianglesSection(aModel)
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try
	End Sub

	Public Overloads Function WriteBoneAnimationSmdFile(ByVal smdPathFileName As String, ByVal aSequence As SourceMdlSequenceDesc04) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Try
			Me.theOutputFileTextWriter = File.CreateText(smdPathFileName)

			Dim smdFile As New SourceSmdFile04(Me.theOutputFileTextWriter, Me.theMdlFileData)

			smdFile.WriteHeaderComment()

			smdFile.WriteHeaderSection()
			smdFile.WriteNodesSection()
			smdFile.WriteSkeletonSectionForAnimation(aSequence)
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

#Region "Data"

	Private theMdlFileData As SourceMdlFileData04

#End Region

End Class
