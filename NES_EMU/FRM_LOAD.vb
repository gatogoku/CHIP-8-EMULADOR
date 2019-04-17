Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices
Imports SFML.Graphics
Imports SFML.Window
Imports SFML.Audio
Imports SFML.System

Public Class FRM_LOAD
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim result = OpenFileDialog1.ShowDialog()
        If (result = DialogResult.OK) Then
            filename = OpenFileDialog1.FileName
            Me.Cursor = Cursors.WaitCursor
            Try
                OPEN_ROM(filename) 'MessageBox.Show("ROM cargada exitosamente.")
                INIT_EMU()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
            End Try
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Public Sub OPEN_ROM(filename As String)
        MEMBIN.Clear() : MEMDEC.Clear() : MEMHEX.Clear()
        Dim fs As FileStream
        If File.Exists(filename) = False Then
            fs = File.Create(filename)
            Dim info As Byte() = New UTF8Encoding(True).GetBytes("This is some text in the file.")
            fs.Write(info, 0, info.Length)
            fs.Close()
        End If
        fs = File.Open(filename, FileMode.Open, FileAccess.Read)
        Dim b(1024) As Byte
        Dim temp As UTF8Encoding = New UTF8Encoding(True)
        Do While fs.Read(b, 0, b.Length) > 0
            For C As Integer = 0 To b.Length - 1
                Dim X = Convert.ToString(b(C), 16).ToUpper
                MEMDEC.Add(b(C))
                MEMHEX.Add("&H" + X)
                MEMBIN.Add(DecToBinary(b(C)))
            Next
        Loop
        Dim INDEX As Integer = 0
        Dim S = 0
        ROM.MAPPERNROM0.CARGAR_MEMORIA()
        fs.Close()
    End Sub

    Public Function INIT_EMU()
        Module1.Cpu = New CPU
        Dim X As Integer = 0
        For c As Integer = 0 To OPCODES2.Count - 1
            Dim Y
            Y = Convert.ToString(OPCODES2(c), 16).ToUpper()
            OPCODESSTR.Add("&H" + Y)
        Next
        Module1.Graphics.StartSFMLGraphics()
    End Function

    Private Overloads Sub OnClosed(sender As Object, e As EventArgs) Handles MyBase.Closed
        Application.Exit()
    End Sub

    Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        Return New System.Random().Next(Min, Max)
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Not Module1.Graphics._window Is Nothing AndAlso Module1.Graphics._window.IsOpen Then Module1.Graphics._window.Close()
        Application.Exit() : Me.Close()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim x As Integer:Dim y As Integer:  x = Screen.PrimaryScreen.WorkingArea.Width - 400: y = Screen.PrimaryScreen.WorkingArea.Height - 270: Me.Location = New Point(x, y)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ColorDialog1.ShowDialog() : Module1.GAME_COLOR = New Color(ColorDialog1.Color.R, ColorDialog1.Color.G, ColorDialog1.Color.B)
    End Sub
End Class




















