Imports System.ComponentModel
Imports System.IO
Imports Steamworks

Public Class ContagionSteamAppInfo
	Inherits SteamAppInfoBase

	Public Sub New()
		MyBase.New()

		Me.ID = New AppId_t(238430)
		Me.Name = "Contagion"
		Me.UsesSteamUGC = True
		Me.CanUseContentFolderOrFile = True
		Me.ContentFileExtensionsAndDescriptions.Add("vpk", "Source 引擎 VPK 文件")
		Me.TagsControlType = GetType(ContagionTagsUserControl)
	End Sub

End Class
