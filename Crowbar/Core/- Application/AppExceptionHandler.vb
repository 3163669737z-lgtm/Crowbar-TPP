Imports System.IO
Imports System.Threading

Public Class AppExceptionHandler

	Public Sub Application_ThreadException(ByVal sender As Object, ByVal t As ThreadExceptionEventArgs)
		Dim anUnhandledExceptionWindow As New UnhandledExceptionWindow()
		Try
			Dim errorReportText As String

			errorReportText = "################################################################################"
			errorReportText += vbCrLf
			errorReportText += "###### "
			errorReportText += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			errorReportText += "   "
			errorReportText += My.Application.Info.ProductName
			errorReportText += " "
			errorReportText += My.Application.Info.Version.ToString(2)
			errorReportText += vbCrLf
			errorReportText += vbCrLf

			errorReportText += "=== 重现错误的步骤 ==="
			errorReportText += vbCrLf
			errorReportText += "[请描述错误发生前，您在 "
			errorReportText += My.Application.Info.ProductName
			errorReportText += " 中执行的最后几个任务。]"
			errorReportText += vbCrLf
			errorReportText += vbCrLf

			errorReportText += "=== 您期望看到的结果 ==="
			errorReportText += vbCrLf
			errorReportText += "[请说明您期望 "
			errorReportText += My.Application.Info.ProductName
			errorReportText += " 执行的操作。]"
			errorReportText += vbCrLf
			errorReportText += vbCrLf

			errorReportText += "=== 上下文信息 ==="
			errorReportText += vbCrLf
			If TheApp Is Nothing Then
				errorReportText += "异常发生在 TheApp 生命周期之前或之后。"
			Else
				'If TheApp.Settings.ViewerIsRunning Then
				'	errorReportText += "Viewing "
				'	errorReportText += TheApp.Settings.ViewMdlPathFileName
				'	errorReportText += vbCrLf
				'End If
				If TheApp.Settings.DecompilerIsRunning Then
					errorReportText += "正在反编译 "
					errorReportText += TheApp.Settings.DecompileMdlPathFileName
					errorReportText += vbCrLf
				End If
				If TheApp.Settings.CompilerIsRunning Then
					errorReportText += "正在编译 "
					errorReportText += TheApp.Settings.CompileQcPathFileName
					errorReportText += vbCrLf
				End If
			End If
			errorReportText += vbCrLf
			errorReportText += vbCrLf

			errorReportText += "=== 异常错误描述 ==="
			errorReportText += vbCrLf
			errorReportText += t.Exception.Message
			errorReportText += vbCrLf
			errorReportText += vbCrLf

			errorReportText += "=== 调用堆栈 ==="
			errorReportText += vbCrLf
			errorReportText += t.Exception.StackTrace
			errorReportText += vbCrLf

			errorReportText += vbCrLf
			errorReportText += vbCrLf
			errorReportText += vbCrLf

			Me.WriteToErrorFile(errorReportText)

			anUnhandledExceptionWindow.ErrorReportTextBox.Text = errorReportText
			anUnhandledExceptionWindow.ShowDialog()
		Catch ex As Exception
		Finally
			anUnhandledExceptionWindow.Dispose()
		End Try

		Application.Exit()
	End Sub

	Private Sub WriteToErrorFile(ByVal errorReportText As String)
		Using sw As StreamWriter = New StreamWriter(TheApp.ErrorPathFileName, True)
			sw.Write(errorReportText)
			sw.Close()
		End Using
	End Sub

End Class
