Imports System.IO

Friend Module DreamcastStudioTexture

	Public Sub Read(reader As BinaryReader, texture As SourceMdlTexture10)
		If reader.ReadUInt32() <> &H58494247UI Then ' GBIX
			Throw New InvalidDataException("Dreamcast MDL 纹理缺少 GBIX 文件头。")
		End If
		reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Current)
		If reader.ReadUInt32() <> &H54525650UI Then ' PVRT
			Throw New InvalidDataException("Dreamcast MDL 纹理缺少 PVRT 文件头。")
		End If
		Dim chunkSize As Long = reader.ReadUInt32()
		Dim colorFormat As Byte = reader.ReadByte()
		Dim imageFormat As Byte = reader.ReadByte()
		reader.ReadUInt16()
		Dim width As Integer = reader.ReadUInt16()
		Dim height As Integer = reader.ReadUInt16()
		If colorFormat <> 1 OrElse width < 1 OrElse height < 1 OrElse width > 1024 OrElse height > 1024 Then
			Throw New InvalidDataException("Dreamcast MDL 纹理的颜色格式或尺寸不受支持。")
		End If

		Dim pixelCount As Integer = width * height
		Dim dataSize As Integer
		Select Case imageFormat
			Case 9 ' Linear RGB565.
				dataSize = pixelCount * 2
			Case 1, 3 ' Morton order, with an optional 2×2 vector-quantization codebook.
				If width <> height OrElse (width And (width - 1)) <> 0 OrElse (imageFormat = 3 AndAlso width < 2) Then
					Throw New InvalidDataException("Dreamcast MDL 纹理的尺寸与其排列格式不符。")
				End If
				dataSize = If(imageFormat = 1, pixelCount * 2, 2048 + pixelCount \ 4)
			Case Else
				Throw New InvalidDataException("Dreamcast MDL 纹理的排列格式不受支持。")
		End Select
		If chunkSize < 8L + dataSize OrElse dataSize > reader.BaseStream.Length - reader.BaseStream.Position Then
			Throw New InvalidDataException("Dreamcast MDL 纹理数据不完整。")
		End If

		Dim packed As Byte() = reader.ReadBytes(dataSize)
		Dim pixels(pixelCount - 1) As UShort
		For y As Integer = 0 To height - 1
			For x As Integer = 0 To width - 1
				Dim offset As Integer
				Select Case imageFormat
					Case 1
						offset = 2 * MortonIndex(x, y)
					Case 3
						Dim entry As Integer = packed(2048 + MortonIndex(x \ 2, y \ 2))
						offset = 8 * entry + 2 * (2 * (x And 1) + (y And 1))
					Case Else
						offset = 2 * (y * width + x)
				End Select
				pixels(y * width + x) = BitConverter.ToUInt16(packed, offset)
			Next
		Next

		texture.theData = CreateIndexedTexture(pixels)
		texture.width = CUInt(width)
		texture.height = CUInt(height)
		texture.theFileName = Path.ChangeExtension(texture.theFileName, ".bmp")
	End Sub

	Private Function MortonIndex(x As Integer, y As Integer) As Integer
		Dim index As Integer = 0
		Dim shift As Integer = 0
		While (x Or y) <> 0
			index = index Or (((x And 1) * 2 + (y And 1)) << shift)
			x >>= 1
			y >>= 1
			shift += 2
		End While
		Return index
	End Function

	Private Function CreateIndexedTexture(pixels As UShort()) As List(Of Byte)
		Dim indices As New Dictionary(Of UShort, Byte)()
		Dim output(pixels.Length + 767) As Byte
		For index As Integer = 0 To pixels.Length - 1
			Dim color As UShort = pixels(index)
			Dim entry As Byte
			If Not indices.TryGetValue(color, entry) Then
				If indices.Count = 256 Then
					Return CreateReducedPalette(pixels, output)
				End If
				entry = CByte(indices.Count)
				indices.Add(color, entry)
				Dim paletteOffset As Integer = pixels.Length + 3 * entry
				output(paletteOffset) = CByte(((CInt(color) >> 11) And 31) * 255 \ 31)
				output(paletteOffset + 1) = CByte(((CInt(color) >> 5) And 63) * 255 \ 63)
				output(paletteOffset + 2) = CByte((CInt(color) And 31) * 255 \ 31)
			End If
			output(index) = entry
		Next
		Return New List(Of Byte)(output)
	End Function

	Private Function CreateReducedPalette(pixels As UShort(), output As Byte()) As List(Of Byte)
		For index As Integer = 0 To pixels.Length - 1
			Dim rgb565 As Integer = pixels(index)
			output(index) = CByte(((rgb565 >> 8) And &HE0) Or ((rgb565 >> 6) And &H1C) Or ((rgb565 >> 3) And 3))
		Next
		For entry As Integer = 0 To 255
			Dim offset As Integer = pixels.Length + 3 * entry
			output(offset) = CByte((entry >> 5) * 255 \ 7)
			output(offset + 1) = CByte(((entry >> 2) And 7) * 255 \ 7)
			output(offset + 2) = CByte((entry And 3) * 255 \ 3)
		Next
		Return New List(Of Byte)(output)
	End Function

End Module
