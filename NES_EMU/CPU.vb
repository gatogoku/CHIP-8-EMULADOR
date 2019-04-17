Imports CHIP_8.Graphics, CHIP_8.Module1
Public Class CPU
    Dim DelayTimer, SoundTimer As Integer
    Public V As New List(Of Integer)
    Public STACK As New List(Of Integer)
    Public PC, SP As Integer
    Dim INDEX As String
    Dim MemoryOpcodes As New List(Of String)
    Public DRAW_SCREEN_FLAG As Boolean = False
    Dim OPCODE
    Dim StartTime As DateTime
    Public ScreenData(64 * 32) As Integer  ' Array to hold the Screen information
    Public DrawSprite As Boolean    ' Set during opcode Dxyn to repaint the GLControl

    Public CHIP8_CHARS(,) As Integer = {
  {&HF0, &H90, &H90, &H90, &HF0},
  {&H20, &H60, &H20, &H20, &H70},
  {&HF0, &H10, &HF0, &H80, &HF0},
  {&HF0, &H10, &HF0, &H10, &HF0},
  {&H90, &H90, &HF0, &H10, &H10},
  {&HF0, &H80, &HF0, &H10, &HF0},
  {&HF0, &H80, &HF0, &H90, &HF0},
  {&HF0, &H10, &H20, &H40, &H40},
  {&HF0, &H90, &HF0, &H90, &HF0},
  {&HF0, &H90, &HF0, &H10, &HF0},
  {&HF0, &H90, &HF0, &H90, &H90},
  {&HE0, &H90, &HE0, &H90, &HE0},
  {&HF0, &H80, &H80, &H80, &HF0},
  {&HE0, &H90, &H90, &H90, &HE0},
  {&HF0, &H80, &HF0, &H80, &HF0},
  {&HF0, &H80, &HF0, &H80, &H80}} '0 1 2 3 4 5 6 7 8 9 A B C D E F           5 HEIGHT 4 WIDTH

    Public Sub New()
        initialize()
    End Sub

    Public Sub initialize()
        PC = &H200  ' Program counter starts at 0x200
        OPCODE = 0     '' Reset current opcode	
        INDEX = 0    '' Reset index register
        SP = 0    '' Reset stack pointer
        DRAW_SCREEN_FLAG = False
        ' Module1.graphics._window.Clear()
        STACK.Clear()
        V.Clear()
        Dim i = 0 ' 80
        For c As Integer = 0 To 15 : For d As Integer = 0 To 4 : ROM.MEMORY(i) = "&H" & Hex(CHIP8_CHARS(c, d)) : i += 1 : Next : Next
        For c As Integer = 0 To 15 : STACK.Add(0) : V.Add(0) : Next
        'For c As Integer = 0 To ROM.MEMORYHEX.Count - 1 Step 2
        '    Dim opc = ("&H" + Hex((ROM.MEMORY(c) << 8) Or ROM.MEMORY(c + 1)))
        '    If opc Like "&H8??7" Then
        '        opc = opc
        '    End If
        '    MemoryOpcodes.Add(opc)
        'Next

    End Sub

    Public Sub EMULATE_CPU()
        ' MI CODIGO CORREGIDO
        Dim OPCODE = (Hex((ROM.MEMORY((PC)) << 8) Or ROM.MEMORY((PC) + 1))).ToString
        If OPCODE.Length < 4 Then : While OPCODE.Length < 4 : OPCODE = "0" & OPCODE : End While : End If
        Dim X, Y, N, NNN, NN As String
        X = Val("&H" & OPCODE.Substring(1, 1))
        Y = Val("&H" & OPCODE.Substring(2, 1))
        N = Val("&H" & OPCODE.Substring(3, 1))
        NN = Val("&H" & OPCODE.Substring(2, 2))
        NNN = Val("&H" & OPCODE.Substring(1, 3))

        Select Case True
            Case OPCODE Like "00E0" '00E0 
                ClearScreen() : PC += 2     ' Clears the screen
            Case OPCODE Like "00EE" '00EE
                PC = STACK(SP) : SP -= 1 : PC += 2    ' Returns from a subroutine
            Case OPCODE Like "1???"
                PC = NNN
            Case OPCODE Like "2???"
                SP += 1 : STACK(SP) = PC : PC = NNN
            Case OPCODE Like "3???" '3XNN	
                If V(X) = NN Then PC += 4 Else PC += 2
            Case OPCODE Like "4???" '4XKK	
                If Not V(X) = NN Then PC += 4 Else PC += 2
            Case OPCODE Like "5???" '5XY0	
                If V(X) = V(Y) Then PC += 4 Else PC += 2 ' If VX <> VY Then PC += 1 
            Case OPCODE Like "6???" '6XKK	
                V(X) = NN : PC += 2
            Case OPCODE Like "7???" '7XKK  ESTA INSTRUCCION ESTABA ERRONEA
                Dim compare As Integer = CInt(V(X)) + CInt(NN)  ' Dim COMPARE = V(X) + (NN)
                If Not compare < 256 Then compare -= 256
                V(X) = compare : PC += 2
            Case OPCODE Like "8??0" '8XY0
                V(X) = V(Y) : PC += 2
            Case OPCODE Like "8??1" '8XY1
                V(X) = V(X) Or V(Y) : PC += 2
            Case OPCODE Like "8??2" '8XY2	Hace VX = VX AND VY.
                V(X) = V(X) And V(Y) : PC += 2 '  Dim VAL = V(X) And V(Y) : V(X) = VAL : PC += 2
            Case OPCODE Like "8??3" '8XY3	Hace VX = VX XOR VY.
                V(X) = V(X) Xor V(Y) : PC += 2
            Case OPCODE Like "8??4" '8XY4
                Dim value As Integer = CInt(V(X)) + CInt(V(Y))      ' VB.NET stupidness again'  Dim value As Integer = V(X) + V(Y)
                If value > 255 Then V(15) = 1 : V(X) = NN Else V(15) = 0
                V(X) = value : PC += 2
            Case OPCODE Like "8??5" '8XY5
                If V(X) >= V(Y) Then
                    V(15) = 1 : V(X) = V(X) - V(Y)
                ElseIf Not V(X) >= V(Y) Then
                    V(15) = 0 : Dim value As Integer = (CInt(V(X)) + 256) - CInt(V(Y)) : V(X) = value        ' Sigh...
                End If
                PC += 2
            Case OPCODE Like "8??6" '8XY6
                Dim binary As String = Convert.ToString(V(X), 2).PadLeft(8, "0"c)   ' Convert to binary
                If binary.EndsWith("1") Then V(15) = 1 Else V(15) = 0
                V(X) >>= 1
                PC += 2     ' Bitshift by 1 to the right
            Case OPCODE Like "8??7"  '8XY7
                If V(X) < V(Y) Then V(15) = 1 Else V(15) = 0
                If V(Y) >= V(X) Then
                    V(15) = 1 : V(X) = V(Y) - V(X)
                ElseIf Not V(Y) >= V(X) Then
                    V(15) = 0 : Dim value As Integer = (CInt(V(Y)) + 256) - CInt(V(X)) : V(X) = value         ' Sigh...
                End If
                PC += 2
            Case OPCODE Like "8??E" '8XYE
                Dim binary As String = Convert.ToString(V(X), 2).PadLeft(8, "0"c)   ' Convert to binary
                If binary.StartsWith("1") Then V(15) = 1 Else V(15) = 0
                V(X) <<= 1 : PC += 2
            Case OPCODE Like "9??0" '9XY0
                If Not V(X) = V(Y) Then PC += 4 Else PC += 2
            Case OPCODE Like "A???" 'ANNN	Establece I = NNNN.
                INDEX = NNN : PC += 2
            Case OPCODE.StartsWith("B") And OPCODE.Length = 6 'BNNN	
                PC = V(0) + NNN
            Case OPCODE Like "C???" 'CXKK
                Dim value = New Random().Next(0, 255) And NN : V(X) = value : PC += 2
            Case OPCODE Like "D???" 'DXYN
                V(15) = 0
                Dim pixel As Integer
                For yline = 0 To N - 1
                    Dim pixel_Y = (V(Y) + yline)
                    pixel = ROM.MEMORY(INDEX + yline)   ' Get byte from memory
                    For xline = 0 To 7
                        Dim pixel_X = (V(X) + xline)
                        Dim b As String = Convert.ToString(pixel, 2).PadLeft(8, "0"c).Substring(xline, 1) ' Get binary byte
                        If b = "1" Then ' If we should draw a pixel...
                            If DrawPixel(pixel_X, pixel_Y) = 0 Then ' Draw a pixel, if a pixel was removed...
                                V(15) = 1   ' Set collision
                            End If
                        End If
                    Next
                Next
                DrawSprite = True
                PC += 2
                Module1.graphics.DibujarSpritesChip8(ScreenData)
                '  Threading.Thread.Sleep(200)

            Case OPCODE Like "E?9E" 'EX9E
                If V(X).ToString.Equals(KEY_PRESSED) Then PC += 4 Else PC += 2
                'If Keys(V(X)) = True Then
                '    PC += 4
                'Else
                '    PC += 2
                'End If
            Case OPCODE Like "E?A1" 'EXA1
                If V(X).ToString.Equals(KEY_PRESSED) Then PC += 4 Else PC += 2
                'If Keys(V(X)) = False Then
                '    PC += 4
                'Else
                '    PC += 2
                'End If
            Case OPCODE Like "F?07" 'FX07	
                V(X) = DelayTimer : PC += 2
            Case OPCODE Like "F?0A" 'FX0A
                'While Emulating = True
                '    If KeyPressFired = True Then
                '        V(X) = LastKeyPress         ' Kinda hacky, but I can't think of a better way
                '        Exit While
                '    End If
                'End While
                Do
                    Module1.joypad.fetchKeyBoard()
                Loop While KEY_PRESSED = "" And EMULATOR_IS_RUNNING
                If KEY_PRESSED <> "" Then V(X) = KEY_PRESSED
                '   MsgBox(KEY_PRESSED)
                PC += 2
            Case OPCODE Like "F?15" 'FX15
                DelayTimer = V(X) : PC += 2
            Case OPCODE Like "F?18" 'FX18	
                SoundTimer = V(X) : PC += 2
            Case OPCODE Like "F?1E" 'FX1E

                If INDEX + V(X) > 4095 Then     ' Stupid VB.NET :(
                    INDEX = INDEX + V(X) - 4096
                Else
                    INDEX += V(X)
                End If
                PC += 2
            Case OPCODE Like "F?29" 'FX29	
                Select Case V(X)
                    Case 0
                        INDEX = 0
                    Case 1
                        INDEX = 5
                    Case 2
                        INDEX = 10
                    Case 3
                        INDEX = 15
                    Case 4
                        INDEX = 20
                    Case 5
                        INDEX = 25
                    Case 6
                        INDEX = 30
                    Case 7
                        INDEX = 35
                    Case 8
                        INDEX = 40
                    Case 9
                        INDEX = 45
                    Case 10
                        INDEX = 50
                    Case 11
                        INDEX = 55
                    Case 12
                        INDEX = 60
                    Case 13
                        INDEX = 65
                    Case 14
                        INDEX = 70
                    Case 15
                        INDEX = 75
                End Select
                PC += 2  '?????
            Case OPCODE Like "F?33" 'FX33	Guarda la representación de VX en formato humano. Poniendo las centenas en la posición de memoria I, las decenas en I + 1 y las unidades en I + 2
                ROM.MEMORY(INDEX) = V(X) / 100                  ' Store hundreds in I
                ROM.MEMORY(INDEX + 1) = (V(X) / 10) Mod 10      ' Tens in I + 1
                ROM.MEMORY(INDEX + 2) = (V(X) Mod 100) Mod 10   ' And ones in I + 2
                PC += 2
            Case OPCODE Like "F?55" 'FX55	Almacena el contenido de V0 a VX en la memoria empezando por la dirección I
                ' For c As Integer = INDEX To INDEX + 16 - 1 : ROM.MEMORY(INDEX) = V(c - INDEX) : Next
                ' INDEX = HexToDecimal(INDEX) + HexToDecimal(X) + HexToDecimal(1)
                'ROM.MEMORY(INDEX + 1) = V(X) : ROM.MEMORY(INDEX + 2) = V(Y)
                For a As Integer = 0 To X : ROM.MEMORY(INDEX + a) = V(a) : Next : PC += 2
            Case OPCODE Like "F?65" 'FX65	Almacena el contenido de la dirección de memoria I en los registros del V0 al VX
                ' For c As Integer = INDEX To INDEX + 16 - 1 : V(c - INDEX) = ROM.MEMORY(INDEX) : Next
                '   INDEX = HexToDecimal(INDEX) + HexToDecimal(X) + HexToDecimal(1)
                '   V(X) = ROM.MEMORY(INDEX + 1) : Y = ROM.MEMORY(INDEX + 2)
                For a As Integer = 0 To X : V(a) = ROM.MEMORY(INDEX + a) : Next : PC += 2

            Case Else
                MsgBox("OPCODE INVALIDO: " & OPCODE & vbNewLine & "PC: " & PC & vbNewLine & "I: " & INDEX & vbNewLine, MsgBoxStyle.OkOnly, "CHIP-8 Error")
        End Select

        If DelayTimer > 0 Then
            DelayTimer -= 1
        End If
        If SoundTimer > 0 Then
            'Module1.Audio.PlaySound()
            My.Computer.Audio.Play("beep.wav", AudioPlayMode.Background)
            SoundTimer -= 1
        End If

        'ContadorCiclos += 1
        'If ContadorCiclos = 16 Then
        '    ContadorCiclos = 0
        '    '  Dim ElapsedTime = Now.Subtract(StartTime).Milliseconds
        '    '  MsgBox("Elapsed milliseconds: " & ElapsedTime, MsgBoxStyle.Information)
        '    ' Threading.Thread.Sleep(16 - ElapsedTime)
        'End If
    End Sub

    Private Function DrawPixel(ByVal x As Integer, ByVal y As Integer)
        x = x Mod 64 : y = y Mod 32
        ScreenData(x + (y * 64)) = ScreenData(x + (y * 64)) Xor 1
        Return ScreenData(x + (y * 64))
    End Function

    Public Sub ClearScreen()
        For d = 0 To 2048 : ScreenData(d) = 0 : Next
    End Sub

End Class
