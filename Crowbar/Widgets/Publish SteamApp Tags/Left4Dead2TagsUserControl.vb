Public Class Left4Dead2TagsUserControl

	Protected Overrides Sub Init()
		' Size each Workshop category column to its longest caption.
		Dim columns As Control()() = {
			New Control() {Label1, SurvivorsCheckBox, BillCheckBox, FrancisCheckBox, LouisCheckBox,
				ZoeyCheckBox, CoachCheckBox, EllisCheckBox, NickCheckBox, RochelleCheckBox},
			New Control() {Label2, CommonInfectedCheckBox, SpecialInfectedCheckBox, BoomerCheckBox, ChargerCheckBox,
				HunterCheckBox, JockeyCheckBox, SmokerCheckBox, SpitterCheckBox, TankCheckBox, WitchCheckBox},
			New Control() {Label3, CampaignsCheckBox, MiscellaneousCheckBox, ModelsCheckBox, ScriptsCheckBox,
				SoundsCheckBox, TexturesCheckBox, UICheckBox},
			New Control() {Label4, CoopCheckBox, MutationsCheckBox, RealismCheckBox, RealismVersusCheckBox,
				ScavengeCheckBox, SinglePlayerCheckBox, SurvivalCheckBox, VersusCheckBox},
			New Control() {Label5, WeaponsCheckBox, GrenadeLauncherCheckBox, M60CheckBox, MeleeCheckBox,
				PistolCheckBox, RifleCheckBox, ShotgunCheckBox, SMGCheckBox, SniperCheckBox, ThrowableCheckBox},
			New Control() {Label6, ItemsCheckBox, AdrenalineCheckBox, DefibrillatorCheckBox, MedkitCheckBox, OtherCheckBox, PillsCheckBox}
		}
		Dim table As New TableLayoutPanel With {
			.Name = "TagsTable",
			.AutoSize = True,
			.AutoSizeMode = AutoSizeMode.GrowAndShrink,
			.ColumnCount = columns.Length,
			.GrowStyle = TableLayoutPanelGrowStyle.AddRows,
			.Margin = Padding.Empty
		}
		Dim columnGap As Integer = LogicalToDeviceUnits(12)
		Me.SuspendLayout()
		table.SuspendLayout()
		For column As Integer = 0 To columns.Length - 1
			table.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
			For row As Integer = 0 To columns(column).Length - 1
				Dim widget As Control = columns(column)(row)
				widget.Margin = New Padding(widget.Margin.Left, widget.Margin.Top, columnGap, widget.Margin.Bottom)
				table.Controls.Add(widget, column, row)
			Next
		Next
		Me.Controls.Add(table)
		table.ResumeLayout(True)
		Me.ResumeLayout(True)
		MyBase.Init()
	End Sub

End Class