Public Class DeleteItemForm

	Protected Overrides Sub OnLoad(ByVal e As EventArgs)
		If Not DesignMode Then UiAppearance.StyleControls(Me)
		MyBase.OnLoad(e)
	End Sub

	Private Sub DeleteButton_Click(sender As Object, e As EventArgs) Handles DeleteButton.Click
		Me.DialogResult = DialogResult.OK
		Me.Close()
	End Sub

	Private Sub CancelDeleteButton_Click(sender As Object, e As EventArgs) Handles CancelDeleteButton.Click
		Me.DialogResult = DialogResult.Cancel
		Me.Close()
	End Sub

End Class