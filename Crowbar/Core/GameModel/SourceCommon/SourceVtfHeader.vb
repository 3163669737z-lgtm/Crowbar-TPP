Imports System.IO

''' <summary>Validates and repairs the header length of a legacy PC VTF texture.</summary>
Friend NotInheritable Class SourceVtfHeader
	Private Const HeaderLengthOffset As Integer = 12
	Private Const LegacyHeaderSize As Integer = 64
	Private Const VolumeHeaderSize As Integer = 80
	Private Const EnvironmentMapFlag As UInteger = &H4000UI

	Friend ReadOnly MinorVersion As UInteger
	Friend ReadOnly DeclaredSize As UInteger
	Friend ReadOnly ExpectedSize As Integer

	Private Sub New(minorVersion As UInteger, declaredSize As UInteger, expectedSize As Integer)
		Me.MinorVersion = minorVersion
		Me.DeclaredSize = declaredSize
		Me.ExpectedSize = expectedSize
	End Sub

	Friend ReadOnly Property NeedsRepair As Boolean
		Get
			Return Me.DeclaredSize <> Me.ExpectedSize
		End Get
	End Property

	Friend Shared Function Inspect(pathFileName As String) As SourceVtfHeader
		Using input As FileStream = File.Open(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read)
			Return Inspect(input)
		End Using
	End Function

	Friend Shared Function SaveRepairedCopy(sourcePath As String, outputPath As String) As SourceVtfHeader
		Dim destination As String = Path.GetFullPath(outputPath)
		Using input As FileStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read)
			Dim header As SourceVtfHeader = Inspect(input)
			If Not header.NeedsRepair Then Throw New InvalidDataException("该 VTF 的头长度已正确。")
			If File.Exists(destination) Then Throw New IOException("目标文件已存在，请为修复副本选择新的文件名。")

			Dim temporary As String = Path.Combine(Path.GetDirectoryName(destination), ".crowbar-" + Guid.NewGuid().ToString("N") + ".tmp")
			Try
				Using output As New FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None)
					input.Position = 0
					Dim prefix(HeaderLengthOffset - 1) As Byte
					If input.Read(prefix, 0, prefix.Length) <> prefix.Length Then Throw New EndOfStreamException("VTF 文件头提前结束。")
					output.Write(prefix, 0, prefix.Length)
					Dim corrected As Byte() = BitConverter.GetBytes(header.ExpectedSize)
					output.Write(corrected, 0, corrected.Length)
					input.Position = HeaderLengthOffset + corrected.Length
					input.CopyTo(output)
				End Using
				File.Move(temporary, destination)
			Finally
				If File.Exists(temporary) Then File.Delete(temporary)
			End Try
			Return header
		End Using
	End Function

	Private Shared Function Inspect(input As Stream) As SourceVtfHeader
		Dim header As Byte()
		Using reader As New BinaryReader(input, Text.Encoding.UTF8, leaveOpen:=True)
			header = reader.ReadBytes(VolumeHeaderSize)
		End Using
		If header.Length < 16 Then Throw New InvalidDataException("VTF 基础文件头已截断。")
		If BitConverter.ToUInt32(header, 0) <> &H465456UI Then Throw New InvalidDataException("文件缺少 PC VTF 签名。")
		Dim major As UInteger = BitConverter.ToUInt32(header, 4)
		Dim minor As UInteger = BitConverter.ToUInt32(header, 8)
		If major <> 7 OrElse minor > 2 Then
			Throw New InvalidDataException("此工具支持 PC VTF 7.0–7.2 的二维纹理。文件声明版本为 " + major.ToString() + "." + minor.ToString() + "。")
		End If

		Dim expectedSize As Integer = If(minor = 2, VolumeHeaderSize, LegacyHeaderSize)
		If header.Length < expectedSize Then Throw New InvalidDataException("VTF 文件头已截断。")
		Dim flags As UInteger = BitConverter.ToUInt32(header, 20)
		If (flags And EnvironmentMapFlag) <> 0 OrElse (minor = 2 AndAlso BitConverter.ToUInt16(header, 63) <> 1) Then
			Throw New InvalidDataException("立方体贴图和体积纹理超出此修复工具的验证范围。")
		End If

		Dim width As Integer = BitConverter.ToUInt16(header, 16)
		Dim height As Integer = BitConverter.ToUInt16(header, 18)
		Dim frames As Integer = BitConverter.ToUInt16(header, 24)
		Dim mipLevels As Integer = header(56)
		If width = 0 OrElse height = 0 OrElse frames = 0 OrElse mipLevels = 0 Then
			Throw New InvalidDataException("VTF 宽度、高度、帧数或 Mip 层数无效。")
		End If

		Dim thumbnailWidth As Integer = header(61)
		Dim thumbnailHeight As Integer = header(62)
		If (thumbnailWidth = 0) Xor (thumbnailHeight = 0) Then Throw New InvalidDataException("VTF 缩略图尺寸不完整。")
		Dim thumbnailBytes As Long = 0
		If thumbnailWidth > 0 Then
			thumbnailBytes = ImageBytes(thumbnailWidth, thumbnailHeight, BitConverter.ToInt32(header, 57))
		End If

		Dim format As Integer = BitConverter.ToInt32(header, 52)
		Dim bytesPerFrame As Long = 0
		For level As Integer = 0 To mipLevels - 1
			bytesPerFrame += ImageBytes(width, height, format)
			If width = 1 AndAlso height = 1 AndAlso level < mipLevels - 1 Then Throw New InvalidDataException("VTF Mip 层数超出纹理尺寸允许的范围。")
			width = Math.Max(1, width \ 2)
			height = Math.Max(1, height \ 2)
		Next

		Dim expectedLength As Long = expectedSize + thumbnailBytes + bytesPerFrame * frames
		If input.Length <> expectedLength Then
			Throw New InvalidDataException("缩略图、各帧和 Mip 层对应的标准文件长度为 " + expectedLength.ToString() +
				" 字节，实际为 " + input.Length.ToString() + " 字节。无法仅修复头长度。")
		End If
		Return New SourceVtfHeader(minor, BitConverter.ToUInt32(header, HeaderLengthOffset), expectedSize)
	End Function

	Private Shared Function ImageBytes(width As Integer, height As Integer, format As Integer) As Long
		Dim pixels As Long = CLng(width) * height
		' VTF format IDs are defined by Valve's ImageFormat enum.
		Select Case format
			Case 13, 20 ' DXT1 / DXT1 with one-bit alpha
				Return CLng((width + 3) \ 4) * ((height + 3) \ 4) * 8
			Case 14, 15 ' DXT3 / DXT5
				Return CLng((width + 3) \ 4) * ((height + 3) \ 4) * 16
			Case 5, 8 ' I8 / A8
				Return pixels
			Case 4, 6, 17, 18, 19, 21, 22 ' RGB565, IA88, packed 16-bit formats, UV88
				Return pixels * 2
			Case 2, 3, 9, 10 ' RGB/BGR888, with or without bluescreen alpha
				Return pixels * 3
			Case 0, 1, 11, 12, 16, 23, 26 ' 32-bit color and UV formats
				Return pixels * 4
			Case 24, 25 ' RGBA16161616F / RGBA16161616
				Return pixels * 8
			Case Else
				Throw New InvalidDataException("此工具尚未验证 VTF 像素格式 " + format.ToString() + "。")
		End Select
	End Function
End Class

