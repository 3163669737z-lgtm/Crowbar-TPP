Imports System.ComponentModel

Public Module AppEnums

	Public Enum InputOptions
		<Description("文件")> File
		<Description("文件夹")> Folder
		<Description("文件夹及子文件夹")> FolderRecursion
	End Enum

	Public Enum DownloadOutputPathOptions
		'<Description("Downloads folder")> DownloadsFolder
		<Description("文档文件夹")> DocumentsFolder
		<Description("工作文件夹")> WorkFolder
	End Enum

	Public Enum UnpackOutputPathOptions
		<Description("包文件所在文件夹")> SameFolder
		<Description("包文件目录的子文件夹")> Subfolder
		<Description("工作文件夹")> WorkFolder
		<Description("游戏的 addons 文件夹")> GameAddonsFolder
	End Enum

	Public Enum DecompileOutputPathOptions
		<Description("工作文件夹")> WorkFolder
		<Description("MDL 输入目录的子文件夹")> Subfolder
	End Enum

	Public Enum CompileOutputPathOptions
		<Description("游戏的 models 文件夹")> GameModelsFolder
		<Description("工作文件夹")> WorkFolder
		<Description("QC 输入目录的子文件夹")> Subfolder
	End Enum

	Public Enum PackInputOptions
		<Description("文件夹")> Folder
		<Description("多个子文件夹的上一级")> ParentFolder
	End Enum

	Public Enum PackOutputPathOptions
		<Description("工作文件夹")> WorkFolder
		<Description("上一级文件夹")> ParentFolder
	End Enum

	Public Enum PublishSearchFieldOptions
		<Description("ID")> ID
		<Description("所有者")> Owner
		<Description("标题")> Title
		<Description("描述")> Description
		<Description("全部字段")> AllFields
	End Enum

	Public Enum StatusMessage
		<Description("Success")> Success
		<Description("Error")> [Error]
		<Description("Canceled")> Canceled
		<Description("Skipped")> Skipped

		<Description("ErrorUnableToCreateTempFolder")> ErrorUnableToCreateTempFolder

		<Description("ErrorRequiredSequenceGroupMdlFileNotFound")> ErrorRequiredSequenceGroupMdlFileNotFound
		<Description("ErrorRequiredTextureMdlFileNotFound")> ErrorRequiredTextureMdlFileNotFound

		<Description("ErrorRequiredMdlFileNotFound")> ErrorRequiredMdlFileNotFound
		<Description("ErrorRequiredAniFileNotFound")> ErrorRequiredAniFileNotFound
		<Description("ErrorRequiredVtxFileNotFound")> ErrorRequiredVtxFileNotFound
		<Description("ErrorRequiredVvdFileNotFound")> ErrorRequiredVvdFileNotFound

		<Description("ErrorInvalidMdlFileId")> ErrorInvalidMdlFileId
		<Description("ErrorInvalidInternalMdlFileSize")> ErrorInvalidInternalMdlFileSize
		<Description("PartialSuccess")> PartialSuccess
		<Description("Unsupported")> Unsupported
	End Enum

	<FlagsAttribute>
	Public Enum FilesFoundFlags
		<Description("AllFilesFound")> AllFilesFound = 0
		<Description("ErrorRequiredSequenceGroupMdlFileNotFound")> ErrorRequiredSequenceGroupMdlFileNotFound = 1
		<Description("ErrorRequiredTextureMdlFileNotFound")> ErrorRequiredTextureMdlFileNotFound = 2

		<Description("ErrorRequiredMdlFileNotFound")> ErrorRequiredMdlFileNotFound = 4
		<Description("ErrorRequiredAniFileNotFound")> ErrorRequiredAniFileNotFound = 8
		<Description("ErrorRequiredVtxFileNotFound")> ErrorRequiredVtxFileNotFound = 16
		<Description("ErrorRequiredVvdFileNotFound")> ErrorRequiredVvdFileNotFound = 32

		<Description("Error")> [Error] = 64
	End Enum

	Public Enum ActionType
		<Description("Unknown")> Unknown
		<Description("SetUpGames")> SetUpGames
		<Description("Download")> Download
		<Description("Unpack")> Unpack
		<Description("Preview")> Preview
		<Description("Decompile")> Decompile
		<Description("Edit")> Edit
		<Description("Compile")> Compile
		<Description("View")> View
		<Description("Pack")> Pack
		<Description("Publish")> Publish
		'<Description("Options")> Options
	End Enum

	Public Enum ContainerType
		GMA
		VPK
	End Enum

	Public Enum PackageAction
		Undefined
		Insert
		List
		Pack
		Unpack
		UnpackToTemp
		UnpackToTempAndOpen
		UnpackFolderTree
	End Enum

	Public Enum ViewerType
		<Description("Preview")> Preview
		<Description("View")> View
	End Enum

	Public Enum DecompiledFileType
		QC
		ReferenceMesh
		LodMesh
		BoneAnimation
		PhysicsMesh
		VertexAnimation
		ProceduralBones
		TextureBmp
		Debug
		DeclareSequenceQci
	End Enum

	Public Enum ProgressOptions
		WarningPhyFileChecksumDoesNotMatchMdlFileChecksum

		WritingFileStarted
		WritingFileFailed
		WritingFileFinished
	End Enum

	Public Enum FindDirection
		Previous
		[Next]
	End Enum

	Public Enum GameEngine
		<Description("GoldSource")> GoldSource
		<Description("Source")> Source
		<Description("Source 2")> Source2
	End Enum

	Public Enum SupportedMdlVersion
		<Description("不覆盖版本")> DoNotOverride
		<Description("04")> MDLv04
		<Description("06")> MDLv06
		<Description("10")> MDLv10
		<Description("14")> MDLv14
		<Description("2531")> MDLv2531
		<Description("27")> MDLv27
		<Description("28")> MDLv28
		<Description("29")> MDLv29
		<Description("30")> MDLv30
		<Description("31")> MDLv31
		<Description("32")> MDLv32
		<Description("35")> MDLv35
		<Description("36")> MDLv36
		<Description("37")> MDLv37
		<Description("38")> MDLv38
		<Description("44")> MDLv44
		<Description("45")> MDLv45
		<Description("46")> MDLv46
		<Description("47")> MDLv47
		<Description("48")> MDLv48
		<Description("49")> MDLv49
		<Description("52")> MDLv52
		<Description("53")> MDLv53
		<Description("57")> MDLv57
	End Enum

	Public Enum OrientationType
		<Description("Horizontal")> Horizontal
		<Description("Vertical")> Vertical
	End Enum

	Public Enum ByteUnitsOption
		<Description("Bytes")> Bytes
		<Description("Binary")> Binary
		<Description("Decimal")> [Decimal]
	End Enum

End Module
