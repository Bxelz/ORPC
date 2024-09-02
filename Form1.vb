Imports System.IO
Imports System.Runtime.InteropServices
Imports Newtonsoft.Json
Imports DiscordRPC
Imports DiscordRPC.Logging
Imports IWshRuntimeLibrary

Public Class Form1
    Private client As DiscordRpcClient
    Private config As Config
    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(
        hwnd As IntPtr, dwAttribute As Integer, ByRef pvAttribute As Integer, cbAttribute As Integer) As Integer
    End Function

    Private Sub EnableDarkModeForTitleBar()
        Dim darkMode As Integer = 1
        DwmSetWindowAttribute(Me.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, darkMode, Marshal.SizeOf(Of Integer)(darkMode))
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EnableDarkModeForTitleBar()
        Try
            Dim configContent As String = System.IO.File.ReadAllText("config.json")
            config = JsonConvert.DeserializeObject(Of Config)(configContent)
        Catch ex As Exception
            MessageBox.Show("Error loading configuration file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            client = New DiscordRpcClient(config.ApplicationId)
            client.Logger = New ConsoleLogger() With {.Level = LogLevel.Warning}
            client.Initialize()
            Dim presence As New RichPresence() With {
                .Details = config.Details,
                .State = config.State,
                .Assets = New Assets() With {
                    .LargeImageKey = config.LargeImageKey,
                    .LargeImageText = config.LargeImageText,
                    .SmallImageKey = config.SmallImageKey,
                    .SmallImageText = config.SmallImageText
                }
            }
            If config.Buttons IsNot Nothing AndAlso config.Buttons.Count > 0 Then
                presence.Buttons = config.Buttons.Select(Function(b) New Button With {
                    .Label = b.Label,
                    .Url = b.Url
                }).ToArray()
            End If

            client.SetPresence(presence)

            Button1.Enabled = False
            Button2.Enabled = True

        Catch ex As Exception
            MessageBox.Show("Failed to start Discord RPC: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            If client IsNot Nothing Then
                client.Dispose()
            End If

            Button1.Enabled = True
            Button2.Enabled = False

        Catch ex As Exception
            MessageBox.Show("Failed to stop Discord RPC: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub LinkLabel1_Click(sender As Object, e As EventArgs) Handles LinkLabel1.Click
        Try
            Dim desktopPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            Dim shortcutPath As String = System.IO.Path.Combine(desktopPath, "ORPC.lnk")
            Dim exePath As String = Application.ExecutablePath
            Dim shell As New WshShell()
            Dim shortcut As IWshShortcut = CType(shell.CreateShortcut(shortcutPath), IWshShortcut)
            shortcut.TargetPath = exePath
            shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(exePath)
            shortcut.IconLocation = exePath
            shortcut.Description = "Shortcut to ORPC"
            shortcut.Save()

            MessageBox.Show("Shortcut created on the desktop!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Failed to create shortcut: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
