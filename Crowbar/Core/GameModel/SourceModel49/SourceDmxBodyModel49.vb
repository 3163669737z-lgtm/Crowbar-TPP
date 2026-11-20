Imports System.Globalization
Imports System.IO
Imports System.Linq

Friend NotInheritable Class SourceDmxBodyModel49
	Private ReadOnly writer As TextWriter

	Public Sub New(output As TextWriter)
		writer = output
	End Sub

	Public Shared Function CanExportBodyPart(part As SourceMdlBodyPart) As Boolean
		If part.theModels Is Nothing OrElse part.theModels.Count < 2 Then Return False

		Dim hasFlex As Boolean = False
		For Each model As SourceMdlModel In part.theModels
			If model.eyeballCount > 0 Then Return False
			If model.theMeshes Is Nothing Then Continue For
			For Each mesh As SourceMdlMesh In model.theMeshes
				If mesh.theFlexes Is Nothing Then Continue For
				For Each flex As SourceMdlFlex In mesh.theFlexes
					' DMX's basic morph targets use the usual 0/1/10/11 weights.
					If flex.flexDescPartnerIndex > 0 OrElse flex.vertAnimType <> 0 OrElse
						flex.target0 <> 0 OrElse flex.target1 <> 1 OrElse flex.target2 <> 10 OrElse flex.target3 <> 11 Then Return False
					hasFlex = True
				Next
			Next
		Next
		Return hasFlex
	End Function

	Public Shared Function HasFlexes(model As SourceMdlModel) As Boolean
		Return model.theMeshes IsNot Nothing AndAlso model.theMeshes.Exists(Function(mesh) mesh.theFlexes IsNot Nothing AndAlso mesh.theFlexes.Count > 0)
	End Function

	Public Sub Write(name As String, data As SourceMdlFileData49, vertexData As SourceVvdFileData04,
					 model As SourceMdlModel, lod As SourceVtxModelLod07, firstVertex As Integer)
		Dim bones As List(Of SourceMdlBone) = data.theBones
		Dim vertices As List(Of SourceVertex) = If(vertexData.fixupCount = 0, vertexData.theVertexes, vertexData.theFixedVertexesByLod(0))
		Dim count As Integer = model.vertexCount
		Dim positions(count - 1) As String, normals(count - 1) As String, uv(count - 1) As String
		Dim weights As New List(Of String)(count * 3), joints As New List(Of String)(count * 3)
		For index As Integer = 0 To count - 1
			Dim vertex As SourceVertex = vertices(firstVertex + index)
			positions(index) = TupleText(vertex.positionX, vertex.positionY, vertex.positionZ)
			normals(index) = TupleText(vertex.normalX, vertex.normalY, vertex.normalZ)
			uv(index) = TupleText(vertex.texCoordX, 1 - vertex.texCoordY)
			For link As Integer = 0 To 2
				weights.Add(NumberText(If(link < vertex.boneWeight.boneCount, vertex.boneWeight.weight(link), 0)))
				joints.Add(NumberText(If(link < vertex.boneWeight.boneCount, vertex.boneWeight.bone(link), 0)))
			Next
		Next

		Dim modelId As String = NewId(), meshId As String = NewId(), dagId As String = NewId(), bindId As String = NewId()
		Dim dagTransform As String = NewId()
		Dim jointIds As String() = bones.Select(Function(bone) NewId()).ToArray()
		Dim transformIds As String() = bones.Select(Function(bone) NewId()).ToArray()
		Dim childIds As New List(Of List(Of String))(bones.Count)
		Dim roots As New List(Of String)()
		For index As Integer = 0 To bones.Count - 1
			childIds.Add(New List(Of String)())
		Next
		For index As Integer = 0 To bones.Count - 1
			Dim parent As Integer = bones(index).parentBoneIndex
			If parent < 0 Then
				roots.Add(jointIds(index))
			Else
				childIds(parent).Add(jointIds(index))
			End If
		Next
		roots.Add(dagId)

		writer.WriteLine("<!-- dmx encoding keyvalues2 1 format model 18 -->")
		Element("DmElement", NewId(), name)
		Field("model", "element", modelId)
		Field("skeleton", "element", modelId)
		writer.WriteLine(Quote("combinationOperator"))
		Element("DmeCombinationOperator", NewId(), "combinationOperator")
		ArrayField("controls", "element_array", New String() {})
		ArrayField("controlValues", "vector3_array", New String() {})
		ArrayField("controlValuesLagged", "vector3_array", New String() {})
		Field("usesLaggedValues", "bool", "0")
		ArrayField("dominators", "element_array", New String() {})
		ArrayField("targets", "element_array", {meshId})
		EndElement()
		EndElement()

		Dim modelTransform As String = NewId()
		Element("DmeModel", modelId, name)
		Field("transform", "element", modelTransform)
		ArrayField("children", "element_array", roots)
		ArrayField("jointList", "element_array", jointIds.Concat({dagId}))
		ArrayField("jointTransforms", "element_array", transformIds.Concat({dagTransform}))
		writer.WriteLine(Quote("baseStates") + " " + Quote("element_array") + " [")
		Element("DmeTransformList", NewId(), "base")
		ArrayField("transforms", "element_array", transformIds.Concat({dagTransform}))
		EndElement()
		writer.WriteLine("]")
		EndElement()
		Transform(modelTransform, name, "0 0 0", "0 0 0 1")
		Transform(dagTransform, name, "0 0 0", "0 0 0 1")
		For index As Integer = 0 To bones.Count - 1
			Dim bone As SourceMdlBone = bones(index)
			Element("DmeJoint", jointIds(index), bone.theName)
			Field("transform", "element", transformIds(index))
			ArrayField("children", "element_array", childIds(index))
			EndElement()
			Transform(transformIds(index), bone.theName, TupleText(bone.position.x, bone.position.y, bone.position.z),
					  TupleText(bone.quat.x, bone.quat.y, bone.quat.z, bone.quat.w))
		Next
		Element("DmeDag", dagId, name)
		Field("transform", "element", dagTransform)
		Field("shape", "element", meshId)
		EndElement()

		Dim corners As New List(Of String)()
		Element("DmeMesh", meshId, name)
		Field("visible", "bool", "1")
		Field("bindState", "element", bindId)
		Field("currentState", "element", bindId)
		ArrayField("baseStates", "element_array", {bindId})
		writer.WriteLine(Quote("faceSets") + " " + Quote("element_array") + " [")
		For meshIndex As Integer = 0 To lod.theVtxMeshes.Count - 1
			If meshIndex > 0 Then writer.WriteLine(",")
			Dim mesh As SourceMdlMesh = model.theMeshes(meshIndex)
			Dim material As String = data.theModifiedTextureFileNames(mesh.materialIndex)
			Dim faces As New List(Of String)()
			For Each strip As SourceVtxStripGroup07 In lod.theVtxMeshes(meshIndex).theVtxStripGroups
				For first As Integer = 0 To strip.theVtxIndexes.Count - 3 Step 3
					For Each offset As Integer In {0, 2, 1}
						faces.Add(NumberText(corners.Count))
						corners.Add(NumberText(mesh.vertexIndexStart + strip.theVtxVertexes(strip.theVtxIndexes(first + offset)).originalMeshVertexIndex))
					Next
					faces.Add("-1")
				Next
			Next
			Element("DmeFaceSet", NewId(), material)
			writer.WriteLine(Quote("material"))
			Element("DmeMaterial", NewId(), material)
			Field("mtlName", "string", material)
			EndElement()
			ArrayField("faces", "int_array", faces)
			EndElement()
		Next
		writer.WriteLine("]")

		Dim deltas As New Dictionary(Of Integer, List(Of KeyValuePair(Of Integer, SourceMdlVertAnim)))()
		For Each mesh As SourceMdlMesh In model.theMeshes
			If mesh.theFlexes Is Nothing Then Continue For
			For Each flex As SourceMdlFlex In mesh.theFlexes
				Dim morph As List(Of KeyValuePair(Of Integer, SourceMdlVertAnim)) = Nothing
				If Not deltas.TryGetValue(flex.flexDescIndex, morph) Then
					morph = New List(Of KeyValuePair(Of Integer, SourceMdlVertAnim))()
					deltas.Add(flex.flexDescIndex, morph)
				End If
				For Each vertex As SourceMdlVertAnim In flex.theVertAnims
					morph.Add(New KeyValuePair(Of Integer, SourceMdlVertAnim)(mesh.vertexIndexStart + vertex.index, vertex))
				Next
			Next
		Next
		writer.WriteLine(Quote("deltaStates") + " " + Quote("element_array") + " [")
		Dim written As Integer = 0
		For Each entry As KeyValuePair(Of Integer, List(Of KeyValuePair(Of Integer, SourceMdlVertAnim))) In deltas
			If written > 0 Then writer.WriteLine(",")
			Dim morph As List(Of KeyValuePair(Of Integer, SourceMdlVertAnim)) = entry.Value
			Dim indices As IEnumerable(Of String) = morph.Select(Function(vertex) NumberText(vertex.Key))
			Element("DmeVertexDeltaData", NewId(), data.theFlexDescs(entry.Key).theName)
			ArrayField("vertexFormat", "string_array", {"positions", "normals"})
			ArrayField("positions", "vector3_array", morph.Select(Function(vertex) TupleText(vertex.Value.flDelta(0).TheFloatValue, vertex.Value.flDelta(1).TheFloatValue, vertex.Value.flDelta(2).TheFloatValue)))
			ArrayField("positionsIndices", "int_array", indices)
			ArrayField("normals", "vector3_array", morph.Select(Function(vertex) TupleText(vertex.Value.flNDelta(0).TheFloatValue, vertex.Value.flNDelta(1).TheFloatValue, vertex.Value.flNDelta(2).TheFloatValue)))
			ArrayField("normalsIndices", "int_array", indices)
			EndElement()
			written += 1
		Next
		writer.WriteLine("]")
		ArrayField("deltaStateWeights", "vector2_array", Enumerable.Repeat("0 0", deltas.Count))
		ArrayField("deltaStateWeightsLagged", "vector2_array", Enumerable.Repeat("0 0", deltas.Count))
		EndElement()

		Element("DmeVertexData", bindId, "bind")
		ArrayField("vertexFormat", "string_array", {"positions", "normals", "textureCoordinates", "jointWeights", "jointIndices"})
		Field("flipVCoordinates", "bool", "1")
		Field("jointCount", "int", "3")
		ArrayField("positions", "vector3_array", positions)
		ArrayField("positionsIndices", "int_array", corners)
		ArrayField("normals", "vector3_array", normals)
		ArrayField("normalsIndices", "int_array", corners)
		ArrayField("textureCoordinates", "vector2_array", uv)
		ArrayField("textureCoordinatesIndices", "int_array", corners)
		ArrayField("jointWeights", "float_array", weights)
		ArrayField("jointIndices", "int_array", joints)
		EndElement()
	End Sub

	Private Sub Transform(id As String, name As String, position As String, orientation As String)
		Element("DmeTransform", id, name)
		Field("position", "vector3", position)
		Field("orientation", "quaternion", orientation)
		EndElement()
	End Sub

	Private Sub Element(type As String, id As String, name As String)
		writer.WriteLine(Quote(type))
		writer.WriteLine("{")
		Field("id", "elementid", id)
		Field("name", "string", name)
	End Sub

	Private Sub EndElement()
		writer.WriteLine("}")
	End Sub

	Private Sub Field(name As String, type As String, value As String)
		writer.WriteLine(Quote(name) + " " + Quote(type) + " " + Quote(value))
	End Sub

	Private Sub ArrayField(name As String, type As String, values As IEnumerable(Of String))
		Dim valuePrefix As String = If(type = "element_array", Quote("element") + " ", "")
		writer.WriteLine(Quote(name) + " " + Quote(type) + " [")
		writer.WriteLine(String.Join("," + Environment.NewLine, values.Select(Function(value) valuePrefix + Quote(value))))
		writer.WriteLine("]")
	End Sub

	Private Shared Function NewId() As String
		Return Guid.NewGuid().ToString("D")
	End Function

	Private Shared Function Quote(value As String) As String
		Return """" + value.Replace("\", "\\").Replace("""", "\" + """") + """"
	End Function

	Private Shared Function NumberText(value As Double) As String
		Return value.ToString("R", CultureInfo.InvariantCulture)
	End Function

	Private Shared Function TupleText(ParamArray values As Double()) As String
		Return String.Join(" ", values.Select(AddressOf NumberText))
	End Function
End Class
