Imports System.IO

''' <summary>Checks Source v49 layouts and locates a matching external mesh pair.</summary>
Friend NotInheritable Class Source49ExternalMesh
	Friend Const StudioHeaderSize As Integer = 408
	Friend Shared ReadOnly VtxExtensions As String() = {".dx11.vtx", ".dx90.vtx", ".dx80.vtx", ".sw.vtx", ".vtx"}

	Private ReadOnly modelPath As String
	Private ReadOnly relativeStudioName As String
	Private ReadOnly checksum As Integer
	Private ReadOnly vertexCount As Long
	Private ReadOnly bodyParts As List(Of List(Of Integer()))
	Friend ReadOnly IsMeshCompanion As Boolean
	Friend Property UsesExtendedStrips As Boolean?

	Private Sub New(modelPath As String, studioName As String, checksum As Integer, bones As Integer,
					bodyParts As List(Of List(Of Integer())), vertexCount As Long)
		Me.modelPath = Path.GetFullPath(modelPath)
		Me.relativeStudioName = studioName
		Me.checksum = checksum
		Me.bodyParts = bodyParts
		Me.vertexCount = vertexCount
		Me.IsMeshCompanion = bones = 0 AndAlso bodyParts.Count = 0
	End Sub

	Friend Shared Function Probe(modelPath As String) As Source49ExternalMesh
		Try
			Using reader As New BinaryReader(New FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				Dim result As Source49ExternalMesh = ReadLayout(reader, modelPath, True)
				If result.IsMeshCompanion Then
					Dim vtx As String = Nothing
					Dim vvd As String = Nothing
					If Not result.FindFiles(vtx, vvd, Nothing, New List(Of String)) Then Return Nothing
				End If
				Return result
			End Using
		Catch ex As InvalidDataException
			Return Nothing
		End Try
	End Function

	Friend Shared Function ReadLayout(reader As BinaryReader, modelPath As String, recovering As Boolean) As Source49ExternalMesh
		reader.BaseStream.Position = 0
		Dim header As Byte() = reader.ReadBytes(StudioHeaderSize)
		If header.Length < StudioHeaderSize Then Throw New InvalidDataException("MDL 文件头不足 408 字节。")
		If IntAt(header, 0) <> &H54534449 Then Throw New InvalidDataException("Source MDL 文件头应为 IDST。")
		Dim length As Long = reader.BaseStream.Length
		Dim declaredLength As Integer = IntAt(header, 76)
		If declaredLength < StudioHeaderSize OrElse (recovering AndAlso declaredLength <> length) Then
			Throw New InvalidDataException("MDL 声明长度无效。")
		End If
		length = Math.Min(length, declaredLength)
		ValidateHeaderTables(header, length)

		Dim nameOffset53 As Integer = IntAt(header, 12)
		Dim terminator As Integer = Array.IndexOf(header, CByte(0), 12, 64)
		If recovering AndAlso (terminator <= 12 OrElse (nameOffset53 >= 0 AndAlso nameOffset53 < length)) Then
			Throw New InvalidDataException("文件头未提供 Source v49 的内联模型名。")
		End If
		Dim name As String = If(terminator > 12, Text.Encoding.UTF8.GetString(header, 12, terminator - 12), "").Replace("/"c, Path.DirectorySeparatorChar)
		If recovering AndAlso Not name.EndsWith(".mdl", StringComparison.OrdinalIgnoreCase) Then
			Throw New InvalidDataException("文件头未提供 Source v49 的内联 MDL 路径。")
		End If

		Dim bones As Integer = IntAt(header, 156)
		Dim boneStart As Integer = IntAt(header, 160)
		Dim bodyCount As Integer = IntAt(header, 232)
		Dim bodyStart As Integer = IntAt(header, 236)
		Dim studio2 As Integer = IntAt(header, 400)
		If studio2 <> 0 Then RequireTable(studio2, 1, 256, length, StudioHeaderSize, "MDL studiohdr2")
		RequireTable(boneStart, bones, 216, length, StudioHeaderSize, "MDL 骨骼表")
		RequireTable(bodyStart, bodyCount, 16, length, StudioHeaderSize, "MDL 体组表")
		Dim parents As New List(Of Integer)(bones)
		For index As Integer = 0 To bones - 1
			reader.BaseStream.Position = boneStart + CLng(index) * 216 + 4
			Dim parent As Integer = reader.ReadInt32()
			If parent < -1 OrElse parent >= bones Then Throw New InvalidDataException("MDL 骨骼父索引无效 [" + index.ToString() + "]。")
			parents.Add(parent)
		Next
		Dim visited(bones - 1) As Byte
		For index As Integer = 0 To bones - 1
			Dim parent As Integer = index
			While parent >= 0 AndAlso visited(parent) = 0
				visited(parent) = 1
				parent = parents(parent)
			End While
			If parent >= 0 AndAlso visited(parent) = 1 Then Throw New InvalidDataException("MDL 骨骼父索引形成循环 [" + index.ToString() + "]。")
			parent = index
			While parent >= 0 AndAlso visited(parent) = 1
				visited(parent) = 2
				parent = parents(parent)
			End While
		Next

		Dim bodies As New List(Of List(Of Integer()))(bodyCount)
		Dim vertices As Long = 0
		For bodyIndex As Integer = 0 To bodyCount - 1
			Dim body As Long = bodyStart + CLng(bodyIndex) * 16
			Dim data As Byte() = ReadAt(reader, body, 16)
			Dim count As Integer = IntAt(data, 4)
			Dim modelStart As Long = body + IntAt(data, 12)
			RequireTable(modelStart, count, 148, length, body + 16, "MDL 子模型表")
			Dim models As New List(Of Integer())(count)
			For modelIndex As Integer = 0 To count - 1
				Dim model As Long = modelStart + CLng(modelIndex) * 148
				data = ReadAt(reader, model + 72, 36)
				Dim meshCount As Integer = IntAt(data, 0)
				Dim meshStart As Long = model + IntAt(data, 4)
				Dim modelVertices As Integer = IntAt(data, 8)
				Dim eyeballs As Integer = IntAt(data, 28)
				RequireTable(meshStart, meshCount, 116, length, model + 148, "MDL 网格表")
				RequireTable(model + IntAt(data, 32), eyeballs, 172, length, model + 148, "MDL 眼球表")
				If modelVertices < 0 Then Throw New InvalidDataException("MDL 子模型顶点计数无效。")
				vertices += modelVertices
				Dim meshVertices(meshCount - 1) As Integer
				For meshIndex As Integer = 0 To meshCount - 1
					Dim mesh As Long = meshStart + CLng(meshIndex) * 116
					data = ReadAt(reader, mesh + 8, 16)
					meshVertices(meshIndex) = IntAt(data, 0)
					Dim firstVertex As Integer = IntAt(data, 4)
					If meshVertices(meshIndex) < 0 OrElse firstVertex < 0 OrElse CLng(firstVertex) + meshVertices(meshIndex) > modelVertices Then
						Throw New InvalidDataException("MDL 网格顶点范围无效。")
					End If
					RequireTable(mesh + IntAt(data, 12), IntAt(data, 8), 60, length, mesh + 116, "MDL 网格表情表")
				Next
				models.Add(meshVertices)
			Next
			bodies.Add(models)
		Next
		If recovering AndAlso bones = 0 AndAlso bodyCount > 0 Then Throw New InvalidDataException("恢复布局缺少骨骼表。")
		Return New Source49ExternalMesh(modelPath, name, IntAt(header, 8), bones, bodies, vertices)
	End Function

	Friend ReadOnly Property NeedsMesh As Boolean
		Get
			Return Me.vertexCount > 0 OrElse Me.IsMeshCompanion
		End Get
	End Property

	Friend Function FindFiles(ByRef vtxPath As String, ByRef vvdPath As String, resourceRoot As String, diagnostics As List(Of String)) As Boolean
		If Not Me.NeedsMesh Then Return True
		Dim topology As Boolean
		If Me.FindPair(Me.modelPath, vtxPath, vvdPath, topology, diagnostics, Path.GetDirectoryName(Me.modelPath)) Then
			Me.UsesExtendedStrips = topology
			diagnostics.Add("使用已验证的配套 — " + vtxPath + " / " + vvdPath)
			Return True
		End If
		Dim candidates As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
		Dim folder As DirectoryInfo = New FileInfo(Me.modelPath).Directory
		While folder IsNot Nothing AndAlso Not folder.Name.Equals("models", StringComparison.OrdinalIgnoreCase)
			folder = folder.Parent
		End While

		' A selected resource root lets the user resolve a conflict by choosing a specific directory.
		If Not String.IsNullOrEmpty(resourceRoot) Then
			resourceRoot = Path.GetFullPath(resourceRoot)
			Me.AddCandidate(candidates, resourceRoot, Path.GetFileName(Me.relativeStudioName))
			Me.AddCandidate(candidates, resourceRoot, Path.GetFileName(Me.modelPath))
			Me.AddCandidate(candidates, resourceRoot, Me.relativeStudioName)
			Me.AddCandidate(candidates, resourceRoot, Path.Combine("models", Me.relativeStudioName))
		ElseIf folder IsNot Nothing Then
			Me.AddCandidate(candidates, folder.FullName, Me.relativeStudioName)
		End If

		Dim found As New List(Of (Vtx As String, Vvd As String, Extra As Boolean))
		For Each entry As KeyValuePair(Of String, String) In candidates
			Dim vtx As String = Nothing
			Dim vvd As String = Nothing
			If Me.FindPair(entry.Key, vtx, vvd, topology, diagnostics, entry.Value) Then found.Add((vtx, vvd, topology))
		Next
		If found.Count > 1 Then
			diagnostics.Add("配套文件有歧义，请选择包含所需配套文件的具体资源目录。")
			For Each pair As (Vtx As String, Vvd As String, Extra As Boolean) In found
				diagnostics.Add("匹配的候选 — " + pair.Vtx + " / " + pair.Vvd)
			Next
		ElseIf found.Count = 1 Then
			vtxPath = found(0).Vtx
			vvdPath = found(0).Vvd
			Me.UsesExtendedStrips = found(0).Extra
			diagnostics.Add("使用已验证的配套 — " + vtxPath + " / " + vvdPath)
			Return True
		Else
			diagnostics.Add("未找到结构一致的 VVD/VTX 配套。")
		End If
		Return False
	End Function

	Private Sub AddCandidate(candidates As Dictionary(Of String, String), root As String, relativeName As String)
		If String.IsNullOrEmpty(relativeName) OrElse Path.IsPathRooted(relativeName) Then Return
		For Each part As String In relativeName.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
			If part.Length = 0 OrElse part.EndsWith(".", StringComparison.Ordinal) OrElse
				part.EndsWith(" ", StringComparison.Ordinal) OrElse part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 Then Return
		Next
		Dim pathFileName As String = Path.GetFullPath(Path.Combine(root, relativeName))
		If Not String.Equals(pathFileName, Me.modelPath, StringComparison.OrdinalIgnoreCase) Then candidates(pathFileName) = root
	End Sub

	Private Shared Sub CheckResourcePath(pathFileName As String, root As String)
		If String.IsNullOrEmpty(root) Then Return
		Dim prefix As String = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar
		If Not pathFileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then Throw New InvalidDataException("路径超出资源目录。")
		Dim partPath As String = prefix
		For Each part As String In pathFileName.Substring(prefix.Length).Split(Path.DirectorySeparatorChar)
			partPath = Path.Combine(partPath, part)
			If (File.GetAttributes(partPath) And FileAttributes.ReparsePoint) <> 0 Then
				Throw New InvalidDataException("资源路径包含重解析点。")
			End If
		Next
	End Sub

	Private Function FindPair(meshModelPath As String, ByRef vtxPath As String, ByRef vvdPath As String, ByRef extended As Boolean,
							  diagnostics As List(Of String), root As String) As Boolean
		Dim vvd As String = Path.ChangeExtension(meshModelPath, ".vvd")
		Dim lods As Integer
		If File.Exists(vvd) Then
			Try
				CheckResourcePath(vvd, root)
				lods = Me.ValidateVvd(vvd)
			Catch ex As Exception When TypeOf ex Is InvalidDataException OrElse TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
				diagnostics.Add("跳过 VVD 候选 — " + vvd + " — " + ex.Message)
			End Try
		End If
		For Each extension As String In VtxExtensions
			Dim vtx As String = Path.ChangeExtension(meshModelPath, extension)
			If Not File.Exists(vtx) Then Continue For
			Try
				CheckResourcePath(vtx, root)
				extended = Me.ValidateVtx(vtx, lods)
				If lods = 0 Then Throw New InvalidDataException("同一位置没有通过验证的 VVD。")
				vtxPath = vtx
				vvdPath = vvd
				Return True
			Catch ex As Exception When TypeOf ex Is InvalidDataException OrElse TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
				diagnostics.Add("跳过 VTX 候选 — " + vtx + " — " + ex.Message)
			End Try
		Next
		Return False
	End Function

	Private Function ValidateVvd(pathFileName As String) As Integer
		Using reader As New BinaryReader(New FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			Dim length As Long = reader.BaseStream.Length
			Dim header As Byte() = reader.ReadBytes(64)
			If header.Length <> 64 Then Throw New InvalidDataException("VVD 文件头不足 64 字节。")
			If IntAt(header, 0) <> &H56534449 OrElse IntAt(header, 4) <> 4 Then Throw New InvalidDataException("VVD 签名或版本无效。")
			If IntAt(header, 8) <> Me.checksum Then Throw New InvalidDataException("VVD 校验和与 MDL 不符。")
			Dim lods As Integer = IntAt(header, 12)
			Dim count As Integer = IntAt(header, 16)
			If lods < 1 OrElse lods > 8 OrElse count <= 0 OrElse (Not Me.IsMeshCompanion AndAlso count <> Me.vertexCount) Then
				Throw New InvalidDataException("VVD LOD 或顶点计数与 MDL 布局不符。")
			End If
			For lod As Integer = 1 To lods - 1
				Dim lodCount As Integer = IntAt(header, 16 + lod * 4)
				If lodCount < 0 OrElse lodCount > IntAt(header, 12 + lod * 4) Then Throw New InvalidDataException("VVD LOD 顶点计数无效。")
			Next
			RequireTable(IntAt(header, 56), count, 48, length, 64, "VVD 顶点表")
			RequireTable(IntAt(header, 60), count, 16, length, 64, "VVD 切线表")
			Dim fixups As Integer = IntAt(header, 48)
			Dim fixupStart As Integer = IntAt(header, 52)
			RequireTable(fixupStart, fixups, 12, length, 64, "VVD fixup 表")
			Dim fixedCounts(lods - 1) As Long
			For index As Integer = 0 To fixups - 1
				Dim data As Byte() = ReadAt(reader, fixupStart + CLng(index) * 12, 12)
				Dim lod As Integer = IntAt(data, 0)
				Dim first As Integer = IntAt(data, 4)
				Dim size As Integer = IntAt(data, 8)
				If lod < 0 OrElse lod >= lods OrElse first < 0 OrElse size < 0 OrElse CLng(first) + size > count Then Throw New InvalidDataException("VVD fixup 顶点范围无效。")
				For usedLod As Integer = 0 To lod
					fixedCounts(usedLod) += size
					If fixedCounts(usedLod) > IntAt(header, 16 + usedLod * 4) Then Throw New InvalidDataException("VVD fixup 顶点计数无效。")
				Next
			Next
			If fixups > 0 Then
				For lod As Integer = 0 To lods - 1
					If fixedCounts(lod) <> IntAt(header, 16 + lod * 4) Then Throw New InvalidDataException("VVD fixup 顶点计数与 LOD 不符。")
				Next
			End If
			Return lods
		End Using
	End Function

	Private Function ValidateVtx(pathFileName As String, vvdLods As Integer) As Boolean
		Using reader As New BinaryReader(New FileStream(pathFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			Dim length As Long = reader.BaseStream.Length
			Dim header As Byte() = reader.ReadBytes(36)
			If header.Length <> 36 Then Throw New InvalidDataException("VTX 文件头不足 36 字节。")
			If IntAt(header, 0) <> 7 Then Throw New InvalidDataException("VTX 版本无效。")
			If IntAt(header, 16) <> Me.checksum Then Throw New InvalidDataException("VTX 校验和与 MDL 不符。")
			Dim lods As Integer = IntAt(header, 20)
			Dim count As Integer = IntAt(header, 28)
			Dim bodyStart As Integer = IntAt(header, 32)
			If lods < 1 OrElse lods > 8 OrElse (vvdLods > 0 AndAlso lods <> vvdLods) OrElse
				count <= 0 OrElse (Not Me.IsMeshCompanion AndAlso count <> Me.bodyParts.Count) Then
				Throw New InvalidDataException("VTX LOD 或体组计数与 MDL/VVD 不符。")
			End If
			RequireTable(bodyStart, count, 8, length, 36, "VTX 体组表")
			Dim replacementStart As Integer = IntAt(header, 24)
			If replacementStart <> 0 Then ValidateReplacements(reader, replacementStart, lods)
			Try
				Me.ValidateVtxTables(reader, bodyStart, count, lods, False)
				Return False
			Catch standardError As InvalidDataException
				Try
					Me.ValidateVtxTables(reader, bodyStart, count, lods, True)
					Return True
				Catch extraError As InvalidDataException
					Throw New InvalidDataException(standardError.Message + " 扩展拓扑布局：" + extraError.Message, extraError)
				End Try
			End Try
		End Using
	End Function

	Private Shared Sub ValidateReplacements(reader As BinaryReader, offset As Long, lods As Integer)
		Dim length As Long = reader.BaseStream.Length
		RequireTable(offset, lods, 8, length, 36, "VTX 材质替换表")
		For lod As Integer = 0 To lods - 1
			Dim item As Long = offset + lod * 8
			Dim list As Byte() = ReadAt(reader, item, 8)
			Dim count As Integer = IntAt(list, 0)
			Dim start As Long = item + IntAt(list, 4)
			RequireTable(start, count, 6, length, item + 8, "VTX 材质替换项")
			For index As Integer = 0 To count - 1
				Dim entry As Long = start + CLng(index) * 6
				Dim nameOffset As Integer = IntAt(ReadAt(reader, entry, 6), 2)
				If nameOffset = 0 Then Continue For
				Dim name As Long = entry + nameOffset
				RequireTable(name, 1, 1, length, 36, "VTX 材质替换名称")
				reader.BaseStream.Position = name
				FileManager.ReadNullTerminatedString(reader, Math.Min(name + 4096, length))
			Next
		Next
	End Sub

	Private Sub ValidateVtxTables(reader As BinaryReader, bodyStart As Long, count As Integer, lods As Integer, extra As Boolean)
		Dim length As Long = reader.BaseStream.Length
		For bodyIndex As Integer = 0 To count - 1
			Dim body As Long = bodyStart + CLng(bodyIndex) * 8
			Dim header As Byte() = ReadAt(reader, body, 8)
			Dim modelCount As Integer = IntAt(header, 0)
			Dim modelStart As Long = body + IntAt(header, 4)
			If Not Me.IsMeshCompanion AndAlso modelCount <> Me.bodyParts(bodyIndex).Count Then Throw New InvalidDataException("VTX 子模型计数与 MDL 不符。")
			RequireTable(modelStart, modelCount, 8, length, body + 8, "VTX 子模型表")
			For modelIndex As Integer = 0 To modelCount - 1
				Dim model As Long = modelStart + CLng(modelIndex) * 8
				header = ReadAt(reader, model, 8)
				Dim modelLods As Integer = IntAt(header, 0)
				Dim lodStart As Long = model + IntAt(header, 4)
				If modelLods < 1 OrElse modelLods > lods Then Throw New InvalidDataException("VTX 子模型 LOD 计数无效。")
				RequireTable(lodStart, modelLods, 12, length, model + 8, "VTX 子模型 LOD 表")
				Dim meshVertices As Integer() = If(Me.IsMeshCompanion, Nothing, Me.bodyParts(bodyIndex)(modelIndex))
				For lodIndex As Integer = 0 To modelLods - 1
					Dim lod As Long = lodStart + CLng(lodIndex) * 12
					header = ReadAt(reader, lod, 12)
					Dim meshCount As Integer = IntAt(header, 0)
					Dim meshStart As Long = lod + IntAt(header, 4)
					If meshVertices IsNot Nothing AndAlso (meshCount > meshVertices.Length OrElse (lodIndex = 0 AndAlso meshCount <> meshVertices.Length)) Then
						Throw New InvalidDataException("VTX LOD 网格计数与 MDL 不符。")
					End If
					RequireTable(meshStart, meshCount, 9, length, lod + 12, "VTX LOD 网格表")
					For meshIndex As Integer = 0 To meshCount - 1
						Me.ValidateStripGroups(reader, meshStart + CLng(meshIndex) * 9, If(meshVertices Is Nothing, Integer.MaxValue, meshVertices(meshIndex)), extra)
					Next
				Next
			Next
		Next
	End Sub

	Private Sub ValidateStripGroups(reader As BinaryReader, mesh As Long, limit As Integer, extra As Boolean)
		Dim length As Long = reader.BaseStream.Length
		Dim groupSize As Integer = If(extra, 33, 25)
		Dim stripSize As Integer = If(extra, 35, 27)
		Dim header As Byte() = ReadAt(reader, mesh, 9)
		Dim groups As Integer = IntAt(header, 0)
		Dim groupStart As Long = mesh + IntAt(header, 4)
		RequireTable(groupStart, groups, groupSize, length, mesh + 9, "VTX strip group 表")
		For i As Integer = 0 To groups - 1
			Dim group As Long = groupStart + CLng(i) * groupSize
			header = ReadAt(reader, group, groupSize)
			Dim vertices As Integer = IntAt(header, 0)
			Dim vertexStart As Long = group + IntAt(header, 4)
			Dim indices As Integer = IntAt(header, 8)
			Dim indexStart As Long = group + IntAt(header, 12)
			Dim strips As Integer = IntAt(header, 16)
			Dim stripStart As Long = group + IntAt(header, 20)
			RequireTable(vertexStart, vertices, 9, length, group + groupSize, "VTX strip group 顶点")
			RequireTable(indexStart, indices, 2, length, group + groupSize, "VTX strip group 索引")
			RequireTable(stripStart, strips, stripSize, length, group + groupSize, "VTX strip 表")
			If extra Then RequireTable(group + IntAt(header, 29), IntAt(header, 25), 2, length, group + groupSize, "VTX 拓扑索引")
			If vertices > 0 Then
				reader.BaseStream.Position = vertexStart
				For j As Integer = 0 To vertices - 1
					Dim vertex As Byte() = reader.ReadBytes(9)
					If BitConverter.ToUInt16(vertex, 4) >= limit Then Throw New InvalidDataException("VTX 顶点映射超出 MDL 网格。")
				Next
			End If
			If indices > 0 Then
				reader.BaseStream.Position = indexStart
				For j As Integer = 0 To indices - 1
					If reader.ReadUInt16() >= vertices Then Throw New InvalidDataException("VTX 索引超出 strip group 的顶点表。")
				Next
			End If
			For j As Integer = 0 To strips - 1
				Dim strip As Long = stripStart + CLng(j) * stripSize
				Dim data As Byte() = ReadAt(reader, strip, stripSize)
				RequireTable(IntAt(data, 4), IntAt(data, 0), 1, indices, 0, "VTX strip 索引范围")
				RequireTable(IntAt(data, 12), IntAt(data, 8), 1, vertices, 0, "VTX strip 顶点范围")
			Next
		Next
	End Sub

	Friend Shared Sub RequireTable(offset As Long, count As Long, recordSize As Integer, length As Long, minimum As Long, field As String)
		If count < 0 OrElse (count > 0 AndAlso (offset < minimum OrElse offset > length OrElse count > (length - offset) \ recordSize)) Then
			Throw New InvalidDataException(field + " 超出文件范围 (offset " + offset.ToString() + ", count " + count.ToString() + ")。")
		End If
	End Sub

	Private Shared Sub ValidateHeaderTables(header As Byte(), length As Long)
		Dim tables As (Field As String, CountAt As Integer, Size As Integer)() = {
			("骨骼控制器", 164, 56), ("碰撞盒组", 172, 12), ("动画描述", 180, 100), ("序列描述", 188, 212),
			("材质", 204, 64), ("材质目录", 212, 4), ("附件", 240, 92), ("表情描述", 260, 4),
			("表情控制器", 268, 20), ("表情规则", 276, 12), ("IK 链", 284, 16),
			("嘴部", 292, 20), ("姿势参数", 300, 20), ("引用模型", 336, 8),
			("动画块", 352, 8), ("表情界面控制器", 384, 20)
		}
		For Each table As (Field As String, CountAt As Integer, Size As Integer) In tables
			RequireTable(IntAt(header, table.CountAt + 4), IntAt(header, table.CountAt), table.Size, length, StudioHeaderSize, "MDL " + table.Field)
		Next
		Dim skinReferences As Integer = IntAt(header, 220)
		Dim skinFamilies As Integer = IntAt(header, 224)
		If skinReferences < 0 OrElse skinFamilies < 0 Then Throw New InvalidDataException("MDL 皮肤表计数无效。")
		RequireTable(IntAt(header, 228), CLng(skinReferences) * skinFamilies, 2, length, StudioHeaderSize, "MDL 皮肤表")
		RequireTable(IntAt(header, 312), IntAt(header, 316), 1, length, StudioHeaderSize, "MDL KeyValues")
		Dim nodes As Integer = IntAt(header, 248)
		RequireTable(IntAt(header, 252), CLng(nodes) * nodes, 1, length, StudioHeaderSize, "MDL 节点映射表")
		RequireTable(IntAt(header, 256), nodes, 4, length, StudioHeaderSize, "MDL 节点名称表")
	End Sub

	Private Shared Function ReadAt(reader As BinaryReader, offset As Long, size As Integer) As Byte()
		reader.BaseStream.Position = offset
		Dim data As Byte() = reader.ReadBytes(size)
		If data.Length <> size Then Throw New InvalidDataException("读取表项时文件提前结束。")
		Return data
	End Function

	Private Shared Function IntAt(data As Byte(), offset As Integer) As Integer
		Return BitConverter.ToInt32(data, offset)
	End Function
End Class
