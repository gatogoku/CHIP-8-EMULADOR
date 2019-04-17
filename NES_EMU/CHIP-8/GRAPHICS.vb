Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices
Imports SFML.Graphics
Imports SFML.Window
Imports SFML.Audio
Imports SFML.System
Public Class Graphics
    'Resolucion
    Public WithEvents _window As RenderWindow
    Public fps = 60
    Public frameInterval = 1000
    Public pixel_color(ancho, alto) As Color
    Public TestCounter = 0
    Public mycustomSprite() As String = {&H3, &HF, &H1F, &H1F, &H1C, &H24, &H26, &H66, &H0, &H0, &H0, &H0, &H1F, &H3F, &H3F, &H7F}
    Public mycustomSprite2
    Public negro As Boolean = False
    Public miscolores
    Public ListSprite As New List(Of String)
    Public LastPixelsSample(31, 63) As Color

    Public Sub StartSFMLGraphics()
        If Module1.GAME_COLOR = Nothing Then Module1.GAME_COLOR = New Color(255, 255, 255)
        _window = New RenderWindow(New VideoMode(ancho, alto, 32), "2D GRAPHICS CHIP-8    -    GAME:  " & Module1.filename, Styles.None)
        _window.SetVisible(True)
        _window.Position = (New SFML.System.Vector2i(0, 0)) : _window.Size = New SFML.System.Vector2i(1920, 1080)
        _window.SetFramerateLimit(fps)
        EMULATOR_IS_RUNNING = True
        While _window.IsOpen And EMULATOR_IS_RUNNING
            Module1.JoyPad.fetchKeyBoard()
            Module1.Cpu.EMULATE_CPU()
            If Module1.Cpu.DrawSprite Then
                _window.Display()
                Module1.Cpu.DrawSprite = False
            End If
        End While
        _window.Close()
    End Sub

    Public Sub DibujarSpritesChip8(ByVal SCREEN_ARRAY)
        ' _window.Clear()
        Dim tex = New Texture(ancho, alto).CopyToImage ': tex.CreateMaskFromColor(New SFML.Graphics.Color(0, 0, 0))
        For c As Integer = 0 To 31
            For d As Integer = 0 To 63
                If Module1.DecToBinary(Module1.Cpu.ScreenData(c * 64 + d)) = 1 Then tex.SetPixel(d, c, GAME_COLOR) Else tex.SetPixel(d, c, New Color(0, 0, 0))
            Next
        Next
        Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, ancho, alto)), New IntRect(0, 0, ancho, alto)) : _window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha))
    End Sub
End Class
