Friend Module UiAppearance
	Friend ReadOnly Accent As Color = Color.FromArgb(26, 98, 166)
	Private ReadOnly AccentHover As Color = Color.FromArgb(21, 85, 145)
	Private ReadOnly AccentPressed As Color = Color.FromArgb(18, 71, 121)
	Friend ReadOnly SelectionTint As Color = Color.FromArgb(223, 236, 249)
	Friend ReadOnly Border As Color = Color.FromArgb(192, 197, 203)
	Friend ReadOnly TableHeader As Color = Color.FromArgb(236, 238, 240)
	Private ReadOnly TableStripe As Color = Color.FromArgb(247, 248, 249)
	Private ReadOnly PrimaryActions As New HashSet(Of String)(StringComparer.Ordinal) From {
		"AddGameSetupButton", "DownloadButton", "UnpackButton", "DecompileButton", "CompileButton",
		"ViewButton", "PackButton", "PublishItemButton", "CheckForUpdateButton"
	}

	Friend Sub StyleControls(ByVal root As Control)
		If SystemInformation.HighContrast Then Return

		Dim button As Button = TryCast(root, Button)
		If button IsNot Nothing AndAlso Not button.AutoSize AndAlso button.Image Is Nothing AndAlso
				button.BackgroundImage Is Nothing AndAlso button.FlatStyle = FlatStyle.Standard AndAlso button.UseVisualStyleBackColor Then
			StyleButton(button, PrimaryActions.Contains(button.Name))
		ElseIf TypeOf root Is DataGridView Then
			StyleGrid(DirectCast(root, DataGridView))
		End If

		For Each child As Control In root.Controls
			StyleControls(child)
		Next
	End Sub

	Private Sub StyleButton(ByVal button As Button, ByVal primary As Boolean)
		button.FlatStyle = FlatStyle.Flat
		button.UseVisualStyleBackColor = False
		button.FlatAppearance.MouseOverBackColor = If(primary, AccentHover, SelectionTint)
		button.FlatAppearance.MouseDownBackColor = If(primary, AccentPressed, TableHeader)
		UpdateButtonColors(button, primary)
		AddHandler button.EnabledChanged, Sub() UpdateButtonColors(button, primary)
	End Sub

	Private Sub UpdateButtonColors(ByVal button As Button, ByVal primary As Boolean)
		Dim active As Boolean = button.Enabled AndAlso primary
		button.BackColor = If(active, Accent, If(button.Enabled, SystemColors.Window, TableStripe))
		button.ForeColor = If(active, Color.White, If(button.Enabled, SystemColors.ControlText, SystemColors.GrayText))
		button.FlatAppearance.BorderColor = If(active, Accent, If(button.Enabled, Border, Color.FromArgb(219, 222, 225)))
	End Sub

	Private Sub StyleGrid(ByVal grid As DataGridView)
		grid.BackgroundColor = TableStripe
		grid.GridColor = Border
		grid.EnableHeadersVisualStyles = False
		grid.ColumnHeadersDefaultCellStyle.BackColor = TableHeader
		grid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText
		grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = TableHeader
		grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.ControlText
		If grid.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty Then
			grid.AlternatingRowsDefaultCellStyle.BackColor = TableStripe
		End If
	End Sub

	Friend Sub StyleMenus()
		If SystemInformation.HighContrast Then Return
		ToolStripManager.Renderer = New ToolStripProfessionalRenderer(New MenuColors()) With {.RoundedEdges = False}
	End Sub

	Private Class MenuColors
		Inherits ProfessionalColorTable

		Public Overrides ReadOnly Property ToolStripGradientBegin As Color
			Get
				Return TableHeader
			End Get
		End Property

		Public Overrides ReadOnly Property ToolStripGradientMiddle As Color
			Get
				Return TableHeader
			End Get
		End Property

		Public Overrides ReadOnly Property ToolStripGradientEnd As Color
			Get
				Return TableHeader
			End Get
		End Property

		Public Overrides ReadOnly Property MenuItemSelected As Color
			Get
				Return SelectionTint
			End Get
		End Property

		Public Overrides ReadOnly Property MenuItemSelectedGradientBegin As Color
			Get
				Return SelectionTint
			End Get
		End Property

		Public Overrides ReadOnly Property MenuItemSelectedGradientEnd As Color
			Get
				Return SelectionTint
			End Get
		End Property

		Public Overrides ReadOnly Property MenuItemBorder As Color
			Get
				Return Border
			End Get
		End Property

		Public Overrides ReadOnly Property MenuBorder As Color
			Get
				Return Border
			End Get
		End Property
	End Class
End Module

Friend Class CrowbarTabControl
	Inherits TabControl

	Private hoveredIndex As Integer = -1

	Protected Overrides Sub OnHandleCreated(ByVal e As EventArgs)
		SetStyle(ControlStyles.UserPaint, False)
		MyBase.OnHandleCreated(e)
		If Not SystemInformation.HighContrast Then
			SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or
				ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
		End If
	End Sub

	Protected Overrides Sub OnFontChanged(ByVal e As EventArgs)
		Dim paintsTabs As Boolean = GetStyle(ControlStyles.UserPaint)
		SetStyle(ControlStyles.UserPaint, False)
		Try
			MyBase.OnFontChanged(e)
		Finally
			SetStyle(ControlStyles.UserPaint, paintsTabs)
		End Try
		Invalidate()
	End Sub

	Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
		MyBase.OnPaint(e)
		Dim highContrast As Boolean = SystemInformation.HighContrast
		Dim pageBounds As Rectangle = DisplayRectangle
		Dim frame As Rectangle = Rectangle.Inflate(pageBounds, 1, 1)
		Using pen As New Pen(If(highContrast, SystemColors.WindowText, UiAppearance.Border))
			If frame.Width > 0 AndAlso frame.Height > 0 Then e.Graphics.DrawRectangle(pen, frame)
		End Using

		For index As Integer = 0 To Me.TabCount - 1
			Dim bounds As Rectangle = Me.GetTabRect(index)
			If Not e.ClipRectangle.IntersectsWith(bounds) Then Continue For
			Dim selected As Boolean = (index = Me.SelectedIndex)
			Dim background As Color = SystemColors.Control
			Dim foreground As Color = SystemColors.ControlText
			If highContrast AndAlso selected Then
				background = SystemColors.Highlight
				foreground = SystemColors.HighlightText
			ElseIf Not highContrast Then
				background = If(selected, SystemColors.Window, If(hoveredIndex = index, UiAppearance.SelectionTint, UiAppearance.TableHeader))
				foreground = If(selected, UiAppearance.Accent, SystemColors.ControlText)
			End If

			Dim fillBounds As New Rectangle(bounds.X + 1, bounds.Y, Math.Max(0, bounds.Width - 2), bounds.Height)
			Using fill As New SolidBrush(background)
				e.Graphics.FillRectangle(fill, fillBounds)
			End Using
			If selected AndAlso Not highContrast Then
				Dim thickness As Integer = LogicalToDeviceUnits(2)
				Dim inset As Integer = LogicalToDeviceUnits(4)
				Using fill As New SolidBrush(UiAppearance.Accent)
					e.Graphics.FillRectangle(fill, bounds.X + inset, bounds.Bottom - thickness - LogicalToDeviceUnits(1),
						Math.Max(0, bounds.Width - 2 * inset), thickness)
				End Using
			End If

			TextRenderer.DrawText(e.Graphics, Me.TabPages(index).Text, Me.Font, bounds, foreground,
				TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding)
			If selected AndAlso Me.Focused AndAlso Me.ShowFocusCues Then
				ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(bounds, -3, -3), foreground, background)
			End If
		Next
	End Sub

	Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
		MyBase.OnMouseMove(e)
		Dim hitIndex As Integer = -1
		For index As Integer = 0 To Me.TabCount - 1
			If Me.GetTabRect(index).Contains(e.Location) Then
				hitIndex = index
				Exit For
			End If
		Next
		If hitIndex = hoveredIndex Then Return
		hoveredIndex = hitIndex
		Invalidate()
	End Sub

	Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
		MyBase.OnMouseLeave(e)
		If hoveredIndex = -1 Then Return
		hoveredIndex = -1
		Invalidate()
	End Sub

	Protected Overrides Sub OnSelectedIndexChanged(ByVal e As EventArgs)
		MyBase.OnSelectedIndexChanged(e)
		Invalidate()
	End Sub

	Protected Overrides Sub OnDpiChangedAfterParent(ByVal e As EventArgs)
		MyBase.OnDpiChangedAfterParent(e)
		Invalidate()
	End Sub

	Protected Overrides Sub OnGotFocus(ByVal e As EventArgs)
		MyBase.OnGotFocus(e)
		Invalidate()
	End Sub

	Protected Overrides Sub OnLostFocus(ByVal e As EventArgs)
		MyBase.OnLostFocus(e)
		Invalidate()
	End Sub
End Class
