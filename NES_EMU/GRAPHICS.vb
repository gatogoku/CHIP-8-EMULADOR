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
        If Module1.Game_Color = Nothing Then Module1.Game_Color = New Color(255, 255, 255)
        ' miscolores = leerImagen()
        ' cargarListaSprites()
        '  mycustomSprite2 = fetchSprite(mycustomSprite.ToList)
        _window = New RenderWindow(New VideoMode(ancho, alto, 32), "2D GRAPHICS CHIP-8    -    GAME:  " & Module1.filename, Styles.None)
        _window.SetVisible(True)
        _window.Position = (New SFML.System.Vector2i(0, 0)) : _window.Size = New SFML.System.Vector2i(1920, 1080)
        _window.SetFramerateLimit(fps)
        While _window.IsOpen
            Module1.joypad.fetchKeyBoard()
            Module1.Cpu.EMULATE_CPU()
            '  _window.DispatchEvents()
            If Module1.Cpu.DrawSprite Then
                _window.Display()
                Module1.Cpu.DrawSprite = False
            End If
            'Threading.Thread.Sleep(1000)
            'setPixelColor():  'LoadCurrentScreen()
        End While
        _window.Close()
    End Sub
    Public Sub DibujarSpritesChip8(ByVal SCREEN_ARRAY)
        _window.Clear()
        Dim tex = New Texture(ancho, alto).CopyToImage ': tex.CreateMaskFromColor(New SFML.Graphics.Color(0, 0, 0))
        Dim x As Color
        For c As Integer = 0 To 31
            For d As Integer = 0 To 63
                If Module1.DecToBinary(Module1.Cpu.ScreenData(c * 64 + d)) = 1 Then
                    tex.SetPixel(d, c, Game_Color)
                End If
            Next
        Next
        Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, ancho, alto)), New IntRect(0, 0, ancho, alto)) : _window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha))
    End Sub

    'Public Function leerImagen()
    '    Dim cols As New List(Of Color) : Dim Index As Integer = 0 : Dim tex As Image = New Texture("C:\star.png", New IntRect(0, 0, alto, ancho)).CopyToImage
    '    For i As Integer = 0 To alto - 1 : For j As Integer = 0 To ancho - 1
    '            cols.Add(tex.GetPixel(j, i)) : Index += 1
    '        Next : Next
    '    Return cols
    'End Function
    'Dim index2 = 0

    'Public Sub cargarListaSprites()
    '    Dim index = 0
    '    For c As Integer = 0 To Module1.CHRMEMBIN.Count - 1 Step 16
    '        Dim l As New List(Of String)
    '        For d As Integer = 0 To 15 : l.Add(Module1.CHRMEMBIN(d + index)) : Next
    '        index += 16 : ListSprite.AddRange(fetchSprite(l))
    '    Next
    'End Sub
    'Public Function fetchSprite(ByVal bytes As List(Of String))
    '    'bytes = New List(Of Integer):bytes.Add(Convert.ToInt32("01001101", 2)):For c As Integer = 1 To 15: bytes.Add(0):'Next:bytes.Item(7) = Convert.ToInt32("11000100", 2)
    '    Dim spriteRows(7) As String : Dim row As String = ""
    '    For d = 0 To 7 : For c = 0 To 7
    '            row = row & Module1.DecToBinary(bytes(d + 8)).ToString.Substring(c, 1) + Module1.DecToBinary(bytes(d)).ToString.Substring(c, 1)
    '        Next : spriteRows(d) = row : row = "" : Next
    '    Return spriteRows
    'End Function

    'Public Sub setPixelColor()
    '    Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim iY = 0 : Dim tex = New Texture(ancho, alto).CopyToImage '  Dim COL = colorPalete(index)
    '    For c As Integer = 0 To alto - 1 Step tamaño : For d As Integer = 0 To ancho - 1 Step tamaño
    '            ' tex.SetPixel(d, c, colorPalete(MARIO_TILE(iY).ToString.Substring(iX, 2) + 10)) : iX += 2 : If iX > 15 Then iX = 0
    '            tex.SetPixel(d, c, colorPalete(MARIO_TILE(iY, iX))) : iX += 2 : If iX > 15 Then iX = 0
    '            'tex.SetPixel(d, c, colorPalete(mycustomSprite2(iY).ToString.Substring(iX, 2) + 15)) : iX += 2 : If iX > 15 Then iX = 0 
    '        Next : iY += 1 : If iY > 15 Then iY = 0
    '    Next
    '    Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, ancho, alto)), New IntRect(0, 0, ancho, alto)) : _window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha))
    'End Sub

    'Dim ind2 = 0
    ''EDITANDO XD
    'Public Sub DRAW_CHARACTERS()
    '    Dim i = 0
    '    Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim iY = 0 : Dim tex = New Texture(ancho, alto).CopyToImage '  Dim COL = colorPalete(index)
    '    For c As Integer = 4 To 9 Step tamaño : For d As Integer = 4 To ancho - 1 Step tamaño
    '            tex.SetPixel(d + i, c, colorPalete(Module1.DecToBinary(ROM.MEMORY(iY + ind2)).Substring(iX, 1))) : iX += 1 : If iX > 3 Then iX = 0
    '        Next
    '        iY += 1 : If iY > 79 Then iY = 0 ' If c Mod 5 = 0 Then ind += 5
    '        '  If iX = 3 And (iY Mod 4) = 0 Then c += 5
    '    Next
    '    i += 20
    '    ind2 += 10
    '    If ind2 > 79 Then ind2 = 0
    '    Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, ancho, alto)), New IntRect(0, 0, ancho, alto)) : _window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha))
    '    ' Threading.Thread.Sleep(1000)
    'End Sub

    'Public Sub DRAW_SCREEN()
    '    Dim ind = 0
    '    Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim iY = 0 : Dim tex = New Texture(ancho, alto).CopyToImage '  Dim COL = colorPalete(index)
    '    For c As Integer = 4 To 8 Step tamaño : For d As Integer = 4 To ancho - 1 Step tamaño
    '            tex.SetPixel(d + ind, c, colorPalete(Module1.DecToBinary(ROM.MEMORY(iY + ind2)).Substring(iX, 1))) : iX += 1 : If iX > 3 Then iX = 0
    '            If (d - 1) Mod 4 = 0 Then ind += 15
    '        Next
    '        If ind > 19 Then
    '            ind = 0
    '        End If
    '        iY += 1 : If iY > 79 Then iY = 0 ' If c Mod 5 = 0 Then ind += 5
    '    Next
    '    ind2 += 5
    '    If ind2 > 79 Then ind2 = 0
    '    Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, ancho, alto)), New IntRect(0, 0, ancho, alto)) : _window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha))
    '    Threading.Thread.Sleep(1000)
    'End Sub

    'Public Sub pintarSprite()
    '    Dim N = 16
    '    Dim TEMPMEMORY As New List(Of String)
    '    For c As Integer = index2 To index2 + N
    '        TEMPMEMORY.Add(ROM.MEMORY(index2))
    '    Next
    '    Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim iY = 0 : Dim tex = New Texture(Module1.ancho, Module1.alto).CopyToImage '  Dim COL = colorPalete(index)
    '    For c As Integer = 0 To Module1.alto - 1 Step tamaño : For d As Integer = 0 To Module1.ancho - 1 Step tamaño
    '            tex.SetPixel(d, c, colorPalete(MARIO_TILE(iY, iX))) : iX += 1 : If iX > 15 Then iX = 0
    '        Next : iY += 1 : If iY > 15 Then iY = 0
    '    Next
    '    Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, Module1.ancho, Module1.alto)), New IntRect(0, 0, Module1.ancho, Module1.alto)) : Module1.graphics._window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha))
    'End Sub

    'Public Function LoadCurrentScreen()
    '    'bytes = New List(Of Integer):bytes.Add(Convert.ToInt32("01001101", 2)):For c As Integer = 1 To 15: bytes.Add(0):'Next:bytes.Item(7) = Convert.ToInt32("11000100", 2)

    '    For d = 0 To 31 : For c = 0 To 63
    '            ' row = row & Module1.DecToBinary(bytes(d + 8)).ToString.Substring(c, 1) + Module1.DecToBinary(bytes(d)).ToString.Substring(c, 1)
    '           LastPixelsSample(d, c) = _window.Capture.GetPixel(d, c)
    '        Next : Next
    '    Return Nothing
    'End Function

    ''Public Sub DibujarSpritesChip8(ByVal INDEX, X, Y, N, VX)
    ''    Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim tex = New Texture(ancho, alto).CopyToImage ':tex.CreateMaskFromColor(New SFML.Graphics.Color(0, 0, 0))
    ''    'For c As Integer = 0 To 31 : For d As Integer = 0 To 63 : LastPixelsSample(c, d) = _window.Capture.GetPixel(d, c) : Next : Next
    ''    For c As Integer = CInt(Y) To CInt(Y) + CInt(N) Step tamaño : For d As Integer = CInt(X) To CInt(X) + 8 Step tamaño
    ''            tex.SetPixel(d, c, colorPalete(Module1.DecToBinary(ROM.MEMORY(INDEX)).Substring(iX, 1))) : iX += 1 : If iX > 7 Then iX = 0
    ''        Next : INDEX += 1 : Next
    ''    'For c As Integer = 0 To 31 : For d As Integer = 0 To 63
    ''    '        If LastPixelsSample(c, d) <> tex.GetPixel(d, c) Then VX = 1 : Exit For
    ''    '    Next : Next
    ''    Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, ancho, alto)), New IntRect(0, 0, ancho, alto)):_window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha))

    ''End Sub


    'Public Sub DibujarSpritesChip8(ByVal INDEX, X_OPCODE, Y_OPCODE, N_OPCODE, VX)
    '    Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim iY = 512 : Dim tex = New Texture(ancho, alto).CopyToImage '  Dim COL = colorPalete(index)
    '    tex.CreateMaskFromColor(New SFML.Graphics.Color(0, 0, 200))

    '    'For c As Integer = 0 To 31 : For d As Integer = 0 To 63 : LastPixelsSample(c, d) = _window.Capture.GetPixel(d, c) : Next : Next

    '    For c As Integer = CInt(Y_OPCODE) To CInt(Y_OPCODE) + CInt(N_OPCODE) Step tamaño : For d As Integer = CInt(X_OPCODE) To CInt(X_OPCODE) + 8 Step tamaño
    '            tex.SetPixel(d, c, colorPalete(Module1.DecToBinary(ROM.MEMORY(INDEX)).Substring(iX, 1))) : iX += 1 : If iX > 7 Then iX = 0
    '        Next : INDEX += 1 : Next

    '    'For c As Integer = 0 To 31 : For d As Integer = 0 To 63
    '    '        If LastPixelsSample(c, d) <> tex.GetPixel(d, c) Then VX = 1 : Exit For
    '    '    Next : Next

    '    Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, ancho, alto)), New IntRect(0, 0, ancho, alto))
    '    _window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha)) ': Threading.Thread.Sleep(1000)

    'End Sub


    'Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim iY = 512 : Dim tex = New Texture(Module1.graphics.ancho, Module1.graphics.alto).CopyToImage '  Dim COL = colorPalete(index)
    'tex.CreateMaskFromColor(New SFML.Graphics.Color(0, 0, 200))

    'For c As Integer = CInt(Y_OPCODE) To CInt(Y_OPCODE) + CInt(N_OPCODE) Step tamaño : For d As Integer = CInt(X_OPCODE) To CInt(X_OPCODE) + 8 Step tamaño
    '        tex.SetPixel(d, c, colorPalete(Module1.DecToBinary(ROM.MEMORY(Y_OPCODE + index)).Substring(iX, 1))) ': iX += 1 : If iX > 7 Then iX = 0
    '    Next
    '    '  iY += 1 : If iY > 1000 Then iY = 0
    'Next
    '' tex.SetPixel(VX, VY, colorPalete(Module1.DecToBinary(ROM.MEMORY(PC + INDEX)).Substring(iX, 1))) : iX += 1 : If iX > 7 Then iX = 0
    'Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, Module1.graphics.ancho, Module1.graphics.alto)), New IntRect(0, 0, Module1.graphics.ancho, Module1.graphics.alto))
    'Module1.graphics._window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha)) ': Threading.Thread.Sleep(1000)

    'DRAW_SCREEN_FLAG = True
    ''   Dim X2 = VY : Dim Y2 = VX
    'Dim I1 = 0 : Dim I2 = 0
    '' Dim x2 = V(Module1.ToBinary(X & &HF00) >> 8)
    '' Dim y2 = V((OPCODE & &HF00) >> 4)
    'Dim tamaño = 1 : Dim rnd = New Random : Dim iX = 0 : Dim iY = 512 : Dim tex = New Texture(Module1.graphics.ancho, Module1.graphics.alto).CopyToImage '  Dim COL = colorPalete(index)
    'tex.CreateMaskFromColor(New SFML.Graphics.Color(0, 0, 200, 0))
    'For c As Integer = 0 To Module1.graphics.alto - 1 Step tamaño : For d As Integer = 0 To Module1.graphics.ancho - 1 Step tamaño

    '        tex.SetPixel(d + I1, c + I2, colorPalete(Module1.ToBinary(ROM.MEMORY(iY + INDEX + I1)).Substring(iX, 1))) : iX += 1 : If iX > 7 Then iX = 0
    '    Next
    '    iY += 1 : If iY > 79 Then iY = 0
    'Next
    'Dim bufferSprite2 As New Sprite(New Texture(tex, New IntRect(0, 0, Module1.graphics.ancho, Module1.graphics.alto)), New IntRect(0, 0, Module1.graphics.ancho, Module1.graphics.alto))
    'Module1.graphics._window.Draw(bufferSprite2, New RenderStates(BlendMode.Alpha)) 'Threading.Thread.Sleep(1000)

End Class
