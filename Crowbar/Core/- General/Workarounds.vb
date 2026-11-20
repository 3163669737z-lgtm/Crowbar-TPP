Module Workarounds

	Friend Function FieldLeftAfter(ByVal caption As Control, ByVal field As Control) As Integer
		Return caption.Right + caption.Margin.Right + field.Margin.Left
	End Function

	Friend Sub FitFieldBefore(ByVal field As Control, ByVal left As Integer, ByVal nextControl As Control)
		Dim right As Integer = nextControl.Left - nextControl.Margin.Left - field.Margin.Right
		field.SetBounds(left, field.Top, Math.Max(0, right - left), field.Height)
	End Sub

	' Call this in Form.Load() or UserControl.Load().
	' Input is the sibling control to the immediate right or the parent control of this control.
	Public Sub WorkaroundForFrameworkAnchorRightSizingBug(ByVal anchoredWidget As Control, ByVal siblingOrParentWidget As Control, Optional ByVal widgetIsParent As Boolean = False)
		If widgetIsParent Then
			anchoredWidget.Size = New System.Drawing.Size(siblingOrParentWidget.Width - siblingOrParentWidget.Padding.Right - anchoredWidget.Margin.Right - anchoredWidget.Left, anchoredWidget.Height)
		Else
			FitFieldBefore(anchoredWidget, anchoredWidget.Left, siblingOrParentWidget)
		End If
	End Sub

End Module
