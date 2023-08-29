Imports System.IO
Imports System.Security.Principal
Imports System.Text.RegularExpressions

Public Class Form1
    '
    Dim source As String = Nothing

    Dim current As String = Nothing

    Dim selected As String = Nothing

    Dim regex As Regex = New Regex(".*\\php-\d+\.\d+\.\d+-.*")

    Dim paths As Array = Strings.Split(System.Environment.GetEnvironmentVariable("PATH"), ";")

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim isAdmin As Boolean = New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)

        If Not isAdmin Then
            '
            MessageBox.Show("Administrator privileges required to run this program!", "Run as administrator", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Environment.Exit(0)
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '
        For Each path As String In paths
            ' Check if ends with regex
            If regex.Match(path).Success Then
                current = path
                TextBox2.Text = current
                Exit For
            End If
        Next

        If current Is Nothing Then
            ' Not found
            MessageBox.Show("Current PHP version not found!", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            ' Get parent directory
            source = Path.GetDirectoryName(current)

            refreshAvailablePHPVersions()
        End If
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        '
        If ListBox1.SelectedItem IsNot Nothing Then
            '
            selected = source & "\" & ListBox1.SelectedItem

            Button1.Enabled = True
        Else
            Button1.Enabled = False
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '
        FolderBrowserDialog1.SelectedPath = source

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            '
            source = FolderBrowserDialog1.SelectedPath

            refreshAvailablePHPVersions()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '
        Dim cmd As Boolean = IsProcessRunning("cmd")

        Dim shell As Boolean = IsProcessRunning("powershell")

        If cmd OrElse shell Then
            '
            MessageBox.Show("Command Prompt or PowerShell process is running.", "Close applications", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            '
            Dim newPaths As New List(Of String)()

            For Each path As String In paths
                If Not regex.Match(path).Success Then
                    newPaths.Add(path)
                End If
            Next

            newPaths.Add(selected)

            Try
                '
                System.Environment.SetEnvironmentVariable("PATH", String.Join(";", newPaths), EnvironmentVariableTarget.Machine)

                Dim result As DialogResult = MessageBox.Show("System Environment is updated successfully!", "Updated successfully", MessageBoxButtons.OK, MessageBoxIcon.Information)

                If result = DialogResult.OK Then
                    Application.Exit()
                End If
            Catch ex As Exception
                '
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function IsProcessRunning(processName As String) As Boolean
        '
        Dim processes() As Process = Process.GetProcessesByName(processName)

        Return processes.Length > 0
    End Function

    Private Sub refreshAvailablePHPVersions()
        '
        Dim folders() As String = Directory.GetDirectories(source)

        TextBox1.Text = source

        Button1.Enabled = False

        ListBox1.Items.Clear()

        ListBox1.ClearSelected()

        For Each folder As String In folders
            '
            If regex.Match(folder).Success Then
                '
                ListBox1.Items.Add(Path.GetFileName(folder))
            End If
        Next
    End Sub
End Class
