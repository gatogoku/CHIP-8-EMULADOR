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
    ' Public LastKeyPress As String   ' Stores the last pressed key, used for opcode Fx0A
    ' Public Repaint As Boolean = False   ' Set during opcode Dxyn to repaint the GLControl
    ' Public Keys(15) As Boolean      ' Stores the key pressed values
    ' Public KeyPressFired As Boolean = False ' Set to true when a key is pressed, used for opcode Fx0A
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

    'Public Function EMULATE_CPU2()
    '    'CODIGO AJENO FUNCIONA PERFECTO
    '    Dim OPCODE = (Hex((ROM.MEMORY((PC)) << 8) Or ROM.MEMORY((PC) + 1))).ToString
    '    Dim H, X, Y, N As String
    '    Dim NNN As String = "0" : Dim NN As String = "0"
    '    If OPCODE.Length >= 1 Then H = ("&H" & OPCODE.Substring(0, 1)) Else H = 0
    '    If OPCODE.Length >= 2 Then X = ("&H" & OPCODE.Substring(1, 1)) Else X = 0
    '    If OPCODE.Length >= 3 Then Y = ("&H" & OPCODE.Substring(2, 1)) Else Y = 0
    '    If OPCODE.Length >= 4 Then N = ("&H" & OPCODE.Substring(3, 1)) Else N = 0

    '    If OPCODE.Length >= 4 Then NNN = ("&H" & OPCODE.Substring(1, 3)) Else NNN = 0
    '    If OPCODE.Length >= 4 Then NN = ("&H" & OPCODE.Substring(2, 2)) Else NN = 0


    '    Select Case True
    '        Case OPCODE = "E0"  ' 00E0, CLS
    '            ClearScreen() : PC += 2     ' Clears the screen

    '        Case OPCODE = "EE"      ' 00EE, RET
    '            PC = STACK(SP) : SP -= 1 : PC += 2    ' Returns from a subroutine


    '        Case OPCODE.StartsWith("1") And OPCODE.Length = 4         ' 1nnn, JP addr
    '            ' Jumps to an address at nnn
    '            PC = NNN
    '        Case OPCODE.StartsWith("2") And OPCODE.Length = 4    ' 2nnn, CALL addr
    '            ' Calls a subroutine at nnn
    '            SP += 1 : STACK(SP) = PC : PC = NNN
    '        Case OPCODE.StartsWith("3") And OPCODE.Length = 4    ' 3xkk - SE Vx, byte
    '            ' Skip next opcode if the value in register Vx = kk

    '            If V(X) = NN Then PC += 4 Else PC += 2

    '        Case OPCODE.StartsWith("4") And OPCODE.Length = 4     ' 4xkk - SNE Vx, byte

    '            If Not V(X) = NN Then PC += 4 Else PC += 2
    '        Case OPCODE.StartsWith("5") And OPCODE.Length = 4     ' 5xy0 - SE Vx, Vy

    '            If V(X) = V(Y) Then PC += 4 Else PC += 2
    '        Case OPCODE.StartsWith("6") And OPCODE.Length = 4     ' 6xkk - LD Vx, byte

    '            V(X) = NN : PC += 2
    '        Case OPCODE.StartsWith("7") And OPCODE.Length = 4     ' 7xkk - ADD Vx, byte

    '            Dim compare As Integer = CInt(V(X)) + CInt(NN)        ' VB.NET doesn't wrap bytes like C#.
    '            If Not compare < 256 Then : compare -= 256 : End If     ' You can do 255 + 1 = 0 in C#, but VB.NET throws an exception
    '            V(X) = compare : PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("0")    ' 8xy0 - LD Vx, Vy

    '            V(X) = V(Y) : PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("1")    ' 8xy1 - OR Vx, Vy

    '            Dim value As Byte = V(X) Or V(Y) : V(X) = value : PC += 2   ' Thank goodness VB.NET has bitwise operators

    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("2")    ' 8xy2 - AND Vx, Vy

    '            Dim value As Byte = V(X) And V(Y) : V(X) = value : PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("3")    ' 8xy3 - XOR Vx, Vy

    '            Dim value As Byte = V(X) Xor V(Y) : V(X) = value : PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("4")    ' 8xy4 - ADD Vx, Vy

    '            Dim value As Integer = CInt(V(X)) + CInt(V(Y))      ' VB.NET stupidness again
    '            If value > 255 Then
    '                V(15) = 1 : V(X) = NN
    '            Else
    '                V(15) = 0 : V(X) = value
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("5")    ' 8xy5 - SUB Vx, Vy
    '            If V(X) >= V(Y) Then
    '                V(15) = 1 : V(X) = V(X) - V(Y)
    '            ElseIf Not V(X) >= V(Y) Then
    '                V(15) = 0 : Dim value As Integer = (CInt(V(X)) + 256) - CInt(V(Y)) : V(X) = value        ' Sigh...
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("6")    ' 8xy6 - SHR Vx
    '            ' Bitshift Vx to the right by 1
    '            Dim binary As String = Convert.ToString(V(X), 2).PadLeft(8, "0"c)   ' Convert to binary
    '            If binary.EndsWith("&H1") Then
    '                V(15) = 1
    '            Else
    '                V(15) = 0
    '            End If
    '            V(X) >>= 1 : PC += 2     ' Bitshift by 1 to the right

    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("7")    ' 8xy7 - SUBN Vx, Vy
    '            If V(Y) >= V(X) Then
    '                V(15) = 1 : V(X) = V(Y) - V(X)
    '            ElseIf Not V(Y) >= V(X) Then
    '                V(15) = 0 : Dim value As Integer = (CInt(V(Y)) + 256) - CInt(V(X)) : V(X) = value         ' Sigh...
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("E")    ' 8xyE - SHL Vx {, Vy}
    '            ' Bitshift Vx to the left by 1
    '            Dim binary As String = Convert.ToString(V(X), 2).PadLeft(8, "0"c)   ' Convert to binary
    '            If binary.StartsWith("1") Then
    '                V(15) = 1
    '            Else
    '                V(15) = 0
    '            End If
    '            V(X) <<= 1      ' Bitshift by 1 to the left
    '            PC += 2
    '        Case OPCODE.StartsWith("9") And OPCODE.Length = 4     ' 9xy0 - SNE Vx, Vy
    '            If Not V(X) = V(Y) Then
    '                PC += 4
    '            Else
    '                PC += 2
    '            End If
    '        Case OPCODE.StartsWith("A") And OPCODE.Length = 4         ' Annn - LD I, addr
    '            ' Set I register to nnn
    '            INDEX = NNN : PC += 2
    '        Case OPCODE.StartsWith("B") And OPCODE.Length = 4         ' Bnnn - JP V0, addr
    '            ' Set PC to nnn + the value of V0
    '            PC = NNN + V(0)
    '        Case OPCODE.StartsWith("C") And OPCODE.Length = 4         ' Cxkk - RND Vx, byte
    '            Dim value = New Random().Next(0, 255) And NN : V(X) = value : PC += 2

    '        Case OPCODE.StartsWith("D") And OPCODE.Length = 4         ' Dxyn - DRW Vx, Vy, nibble
    '            ' Draw a n-byte sprite (which is n pixels tall) starting at the memory address in I, at position Vx, Vy
    '            ' Set VF is there is sprite collision
    '            Dim height As Integer = Val("&H" & OPCODE.Substring(3, 1)) : Dim sprite_x As Byte = V(X) : Dim sprite_y As Byte = V(Y)
    '            V(15) = 0
    '            Dim pixel As Integer
    '            For yline = 0 To height - 1
    '                Dim pixel_Y = (sprite_y + yline)
    '                pixel = ROM.MEMORY(INDEX + yline)   ' Get byte from memory
    '                For xline = 0 To 7
    '                    Dim pixel_X = (sprite_x + xline)
    '                    Dim b As String = Convert.ToString(pixel, 2).PadLeft(8, "0"c).Substring(xline, 1) ' Get binary byte
    '                    If b = "1" Then ' If we should draw a pixel...
    '                        If DrawPixel(pixel_X, pixel_Y) = 0 Then ' Draw a pixel, if a pixel was removed...
    '                            V(15) = 1   ' Set collision
    '                        End If
    '                    End If
    '                Next
    '            Next
    '            Module1.graphics.DibujarSpritesChip8(ScreenData)
    '            '   Repaint = True  ' Tell OpenGL to redraw.
    '            PC += 2
    '        Case OPCODE.StartsWith("E") And OPCODE.Length = 4 And OPCODE.EndsWith("9E")   ' Ex9E - SKP Vx
    '            ' Skip opcode if key (Vx) is pressed
    '            If Keys(V(X)) = True Then PC += 4 Else PC += 2

    '        Case OPCODE.StartsWith("E") And OPCODE.Length = 4 And OPCODE.EndsWith("A1")   ' ExA1 - SKNP Vx
    '            ' Skip opcode if key (Vx) is not pressed
    '            If Keys(V(X)) = False Then PC += 4 Else PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("07")   ' Fx07 - LD Vx, DT
    '            ' Set Vx = delay timer value
    '            V(X) = DelayTimer : PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("0A")   ' Fx0A - LD Vx, K
    '            ' Wait for key press, store into Vx
    '            While Emulating = True
    '                If KeyPressFired = True Then
    '                    V(X) = LastKeyPress : Exit While       ' Kinda hacky, but I can't think of a better way
    '                End If
    '            End While
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("15")   ' Fx15 - LD DT, Vx
    '            ' Set delay timer to value of Vx
    '            DelayTimer = V(X) : PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("18")   ' Fx18 - LD ST, Vx
    '            ' Set sound timer to value of Vx
    '            SoundTimer = V(X) : PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("1E")   ' Fx1E - ADD I, Vx
    '            ' Add Vx to register I
    '            If INDEX + V(X) > 4095 Then     ' Stupid VB.NET :(
    '                INDEX = INDEX + V(X) - 4096
    '            Else
    '                INDEX += V(X)
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("29")   ' Fx29 - LD F, Vx
    '            ' Set I to the location of the sprite for digit Vx
    '            Select Case V(X)
    '                Case 0

    '                    INDEX = 0
    '                Case 1
    '                    INDEX = 5
    '                Case 2
    '                    INDEX = 10
    '                Case 3
    '                    INDEX = 15
    '                Case 4
    '                    INDEX = 20
    '                Case 5
    '                    INDEX = 25
    '                Case 6
    '                    INDEX = 30
    '                Case 7
    '                    INDEX = 35
    '                Case 8
    '                    INDEX = 40
    '                Case 9
    '                    INDEX = 45
    '                Case 10
    '                    INDEX = 50
    '                Case 11
    '                    INDEX = 55
    '                Case 12
    '                    INDEX = 60
    '                Case 13
    '                    INDEX = 65
    '                Case 14
    '                    INDEX = 70
    '                Case 15
    '                    INDEX = 75
    '            End Select
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("33")   ' Fx33 - LD B, Vx
    '            ' Convert a hex number to decimal
    '            ROM.MEMORY(INDEX) = V(X) / 100                  ' Store hundreds in I
    '            ROM.MEMORY(INDEX + 1) = (V(X) / 10) Mod 10      ' Tens in I + 1
    '            ROM.MEMORY(INDEX + 2) = (V(X) Mod 100) Mod 10   ' And ones in I + 2
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("55")   ' Fx55 - LD [I], Vx
    '            ' Store registers V0 to Vx in memory starting at I
    '            For a As Integer = 0 To X
    '                ROM.MEMORY(INDEX + a) = V(a)
    '            Next
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("65")   ' Fx65 - LD Vx, [I]
    '            ' Read registers V0 to Vx from memory starting at I
    '            For a As Integer = 0 To X
    '                V(a) = ROM.MEMORY(INDEX + a)
    '            Next
    '            PC += 2
    '        Case Else
    '            Emulating = False
    '            MsgBox("Invalid opcode: " & OPCODE & vbNewLine & "PC: " & PC & vbNewLine & "I: " & INDEX & vbNewLine, MsgBoxStyle.OkOnly, "ChipGL Error")
    '    End Select

    'End Function


    Public Function EMULATE_CPU() As Integer
        ' MI CODIGO CORREGIDO
        Dim OPCODE = (Hex((ROM.MEMORY((PC)) << 8) Or ROM.MEMORY((PC) + 1))).ToString
        If OPCODE.Length < 4 Then : While OPCODE.Length < 4 : OPCODE = "0" & OPCODE : End While : End If
        Dim H, X, Y, N, NNN, NN As String
        'If OPCODE.Length >= 1 Then H = Val("&H" & OPCODE.Substring(0, 1)) Else H = 0
        If OPCODE.Length >= 2 Then X = Val("&H" & OPCODE.Substring(1, 1)) Else X = 0
        If OPCODE.Length >= 3 Then Y = Val("&H" & OPCODE.Substring(2, 1)) Else Y = 0
        If OPCODE.Length >= 4 Then N = Val("&H" & OPCODE.Substring(3, 1)) Else N = 0
        If OPCODE.Length >= 4 Then NNN = Val("&H" & OPCODE.Substring(1, 3)) Else NNN = 0
        If OPCODE.Length >= 4 Then NN = Val("&H" & OPCODE.Substring(2, 2)) Else NN = 0

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
            Case OPCODE.StartsWith("8") And OPCODE.Length = 6 And OPCODE.EndsWith("7")   '8XY7
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
                Module1.graphics.DibujarSpritesChip8(ScreenData)
                '  Threading.Thread.Sleep(200)
                PC += 2
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
                Loop While KEY_PRESSED = ""
                If KEY_PRESSED <> "" Then V(X) = KEY_PRESSED
                MsgBox(KEY_PRESSED)
                PC += 2
            Case OPCODE Like "F?15" 'FX15
                DelayTimer = V(X) : PC += 2
            Case OPCODE Like "F?18" 'FX18	
                SoundTimer = V(X) : PC += 2
            Case OPCODE Like "F?1E" 'FX1E
                'INDEX = INDEX + V(X) : PC += 2
                'If INDEX > 255 Then V(15) = 1 Else V(15) = 0
                ' Add Vx to register I
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
                : PC += 2
            Case OPCODE Like "F?55" 'FX55	Almacena el contenido de V0 a VX en la memoria empezando por la dirección I
                'For c As Integer = INDEX To INDEX + 16 - 1 : ROM.MEMORY(INDEX) = V(c - INDEX) : Next
                'INDEX = HexToDecimal(INDEX) + HexToDecimal(X) + HexToDecimal(1)
                'ROM.MEMORY(INDEX + 1) = V(X) : ROM.MEMORY(INDEX + 2) = V(Y)
                For a As Integer = 0 To X : ROM.MEMORY(INDEX + a) = V(a) : Next : PC += 2
            Case OPCODE Like "F?65" 'FX65	Almacena el contenido de la dirección de memoria I en los registros del V0 al VX
                'For c As Integer = INDEX To INDEX + 16 - 1 : V(c - INDEX) = ROM.MEMORY(INDEX) : Next
                'INDEX = HexToDecimal(INDEX) + HexToDecimal(X) + HexToDecimal(1)
                'V(X) = ROM.MEMORY(INDEX + 1) : Y = ROM.MEMORY(INDEX + 2)
                For a As Integer = 0 To X : V(a) = ROM.MEMORY(INDEX + a) : Next : PC += 2

            Case Else
                '  Emulating = False
                MsgBox("Invalid opcode: " & OPCODE & vbNewLine & "PC: " & PC & vbNewLine & "I: " & INDEX & vbNewLine, MsgBoxStyle.OkOnly, "ChipGL Error")
        End Select

        If DelayTimer > 0 Then
            DelayTimer -= 1
        End If
        If SoundTimer > 0 Then
            If (True) Then

            End If
            SoundTimer -= 1
        End If

        'ContadorCiclos += 1
        'If ContadorCiclos = 16 Then
        '    ContadorCiclos = 0
        '    '  Dim ElapsedTime = Now.Subtract(StartTime).Milliseconds
        '    '  MsgBox("Elapsed milliseconds: " & ElapsedTime, MsgBoxStyle.Information)
        '    ' Threading.Thread.Sleep(16 - ElapsedTime)
        'End If

        'Return ContadorCiclos
    End Function



    'Public Function EMULATE_CPU()
    '    'FUNCIONA CORRECTAMENTE 
    '    ' Dim OPCODE As String = "&H" & Hex(Hex(ROM.MEMORY((PC)) << 8) Or ROM.MEMORY((PC) + 1))
    '    Dim OPCODE = (Hex((ROM.MEMORY((PC)) << 8) Or ROM.MEMORY((PC) + 1))).ToString
    '    Dim H, X, Y, N As String
    '    Dim NNN As String = "0" : Dim NN As String = "0"
    '    If OPCODE.Length >= 1 Then H = ("&H" & OPCODE.Substring(0, 1)) Else H = 0
    '    If OPCODE.Length >= 2 Then X = ("&H" & OPCODE.Substring(1, 1)) Else X = 0
    '    If OPCODE.Length >= 3 Then Y = ("&H" & OPCODE.Substring(2, 1)) Else Y = 0
    '    If OPCODE.Length >= 4 Then N = ("&H" & OPCODE.Substring(3, 1)) Else N = 0

    '    If OPCODE.Length >= 4 Then NNN = ("&H" & OPCODE.Substring(1, 3)) Else NNN = 0
    '    If OPCODE.Length >= 4 Then NN = ("&H" & OPCODE.Substring(2, 2)) Else NN = 0

    '    ' The Chip-8 has 35 different opcodes. (36 if you include 0nnn - SYS addr, but it is ignored by modern interpreters)
    '    ' Each opcode is 2 bytes long
    '    ' Simple, right?
    '    Select Case True
    '        Case OPCODE = "E0"  ' 00E0, CLS
    '            ClearScreen()       ' Clears the screen
    '            PC += 2
    '        Case OPCODE = "EE"      ' 00EE, RET
    '            PC = STACK(SP)    ' Returns from a subroutine
    '            SP -= 1
    '            PC += 2
    '        Case OPCODE.StartsWith("1") And OPCODE.Length = 4         ' 1nnn, JP addr
    '            ' Jumps to an address at nnn
    '            PC = NNN
    '        Case OPCODE.StartsWith("2") And OPCODE.Length = 4    ' 2nnn, CALL addr
    '            ' Calls a subroutine at nnn
    '            SP += 1
    '            STACK(SP) = PC
    '            PC = NNN
    '        Case OPCODE.StartsWith("3") And OPCODE.Length = 4    ' 3xkk - SE Vx, byte
    '            ' Skip next opcode if the value in register Vx = kk

    '            If V(X) = NN Then
    '                PC += 4
    '            Else
    '                PC += 2
    '            End If
    '        Case OPCODE.StartsWith("4") And OPCODE.Length = 4     ' 4xkk - SNE Vx, byte

    '            If Not V(X) = NN Then
    '                PC += 4
    '            Else
    '                PC += 2
    '            End If
    '        Case OPCODE.StartsWith("5") And OPCODE.Length = 4     ' 5xy0 - SE Vx, Vy

    '            If V(X) = V(Y) Then
    '                PC += 4
    '            Else
    '                PC += 2
    '            End If
    '        Case OPCODE.StartsWith("6") And OPCODE.Length = 4     ' 6xkk - LD Vx, byte

    '            V(X) = NN
    '            PC += 2
    '        Case OPCODE.StartsWith("7") And OPCODE.Length = 4     ' 7xkk - ADD Vx, byte

    '            Dim compare As Integer = CInt(V(X)) + CInt(NN)        ' VB.NET doesn't wrap bytes like C#.
    '            If Not compare < 256 Then : compare -= 256 : End If     ' You can do 255 + 1 = 0 in C#, but VB.NET throws an exception
    '            V(X) = compare
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("0")    ' 8xy0 - LD Vx, Vy

    '            V(X) = V(Y)
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("1")    ' 8xy1 - OR Vx, Vy

    '            Dim value As Byte = V(X) Or V(Y)    ' Thank goodness VB.NET has bitwise operators
    '            V(X) = value
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("2")    ' 8xy2 - AND Vx, Vy

    '            Dim value As Byte = V(X) And V(Y)
    '            V(X) = value
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("3")    ' 8xy3 - XOR Vx, Vy

    '            Dim value As Byte = V(X) Xor V(Y)
    '            V(X) = value
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("4")    ' 8xy4 - ADD Vx, Vy

    '            Dim value As Integer = CInt(V(X)) + CInt(V(Y))      ' VB.NET stupidness again
    '            If value > 255 Then
    '                V(15) = 1

    '                V(X) = NN
    '            Else
    '                V(15) = 0
    '                V(X) = value
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("5")    ' 8xy5 - SUB Vx, Vy

    '            If V(X) >= V(Y) Then
    '                V(15) = 1
    '                V(X) = V(X) - V(Y)
    '            ElseIf Not V(X) >= V(Y) Then
    '                V(15) = 0
    '                Dim value As Integer = (CInt(V(X)) + 256) - CInt(V(Y))          ' Sigh...
    '                V(X) = value
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("4")    ' 8xy6 - SHR Vx
    '            ' Bitshift Vx to the right by 1
    '            Dim binary As String = Convert.ToString(V(X), 2).PadLeft(8, "0"c)   ' Convert to binary
    '            If binary.EndsWith("1") Then
    '                V(15) = 1
    '            Else
    '                V(15) = 0
    '            End If

    '            V(X) >>= 1      ' Bitshift by 1 to the right
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("7")    ' 8xy7 - SUBN Vx, Vy

    '            If V(Y) >= V(X) Then
    '                V(15) = 1
    '                V(X) = V(Y) - V(X)
    '            ElseIf Not V(Y) >= V(X) Then
    '                V(15) = 0
    '                Dim value As Integer = (CInt(V(Y)) + 256) - CInt(V(X))          ' Sigh...
    '                V(X) = value
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("E")    ' 8xyE - SHL Vx {, Vy}
    '            ' Bitshift Vx to the left by 1
    '            Dim binary As String = Convert.ToString(V(X), 2).PadLeft(8, "0"c)   ' Convert to binary
    '            If binary.StartsWith("1") Then
    '                V(15) = 1
    '            Else
    '                V(15) = 0
    '            End If
    '            V(X) <<= 1      ' Bitshift by 1 to the left
    '            PC += 2
    '        Case OPCODE.StartsWith("9") And OPCODE.Length = 4     ' 9xy0 - SNE Vx, Vy

    '            If Not V(X) = V(Y) Then
    '                PC += 4
    '            Else
    '                PC += 2
    '            End If
    '        Case OPCODE.StartsWith("A") And OPCODE.Length = 4         ' Annn - LD I, addr
    '            ' Set I register to nnn
    '            INDEX = NNN
    '            PC += 2
    '        Case OPCODE.StartsWith("B") And OPCODE.Length = 4         ' Bnnn - JP V0, addr
    '            ' Set PC to nnn + the value of V0
    '            PC = NNN + V(0)
    '        Case OPCODE.StartsWith("C") And OPCODE.Length = 4         ' Cxkk - RND Vx, byte

    '            Dim r As New Random
    '            Dim value = r.Next(0, 255) And NN
    '            V(X) = value
    '            PC += 2
    '        Case OPCODE.StartsWith("D") And OPCODE.Length = 4         ' Dxyn - DRW Vx, Vy, nibble
    '            ' Draw a n-byte sprite (which is n pixels tall) starting at the memory address in I, at position Vx, Vy
    '            ' Set VF is there is sprite collision
    '            Dim height As Integer = Val("&H" & OPCODE.Substring(3, 1))
    '            Dim sprite_x As Byte = V(X)
    '            Dim sprite_y As Byte = V(Y)
    '            V(15) = 0
    '            Dim pixel As Integer
    '            For yline = 0 To height - 1
    '                Dim pixel_Y = (sprite_y + yline)
    '                pixel = ROM.MEMORY(INDEX + yline)   ' Get byte from memory
    '                For xline = 0 To 7
    '                    Dim pixel_X = (sprite_x + xline)
    '                    Dim b As String = Convert.ToString(pixel, 2).PadLeft(8, "0"c).Substring(xline, 1) ' Get binary byte
    '                    If b = "1" Then ' If we should draw a pixel...
    '                        If DrawPixel(pixel_X, pixel_Y) = 0 Then ' Draw a pixel, if a pixel was removed...
    '                            V(15) = 1   ' Set collision
    '                        End If
    '                    End If
    '                Next
    '            Next
    '            Module1.graphics.DibujarSpritesChip8(ScreenData)
    '            Repaint = True  ' Tell OpenGL to redraw.
    '            PC += 2
    '        Case OPCODE.StartsWith("E") And OPCODE.Length = 4 And OPCODE.EndsWith("9E")   ' Ex9E - SKP Vx
    '            ' Skip opcode if key (Vx) is pressed
    '            If Keys(V(X)) = True Then
    '                PC += 4
    '            Else
    '                PC += 2
    '            End If
    '        Case OPCODE.StartsWith("E") And OPCODE.Length = 4 And OPCODE.EndsWith("A1")   ' ExA1 - SKNP Vx
    '            ' Skip opcode if key (Vx) is not pressed
    '            If Keys(V(X)) = False Then
    '                PC += 4
    '            Else
    '                PC += 2
    '            End If
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("07")   ' Fx07 - LD Vx, DT
    '            ' Set Vx = delay timer value
    '            V(X) = DelayTimer
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("0A")   ' Fx0A - LD Vx, K
    '            ' Wait for key press, store into Vx
    '            While Emulating = True
    '                If KeyPressFired = True Then
    '                    V(X) = LastKeyPress         ' Kinda hacky, but I can't think of a better way
    '                    Exit While
    '                End If
    '            End While
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("15")   ' Fx15 - LD DT, Vx
    '            ' Set delay timer to value of Vx
    '            DelayTimer = V(X)
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("18")   ' Fx18 - LD ST, Vx
    '            ' Set sound timer to value of Vx
    '            SoundTimer = V(X)
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("1E")   ' Fx1E - ADD I, Vx
    '            ' Add Vx to register I
    '            If INDEX + V(X) > 4095 Then     ' Stupid VB.NET :(
    '                INDEX = INDEX + V(X) - 4096
    '            Else
    '                INDEX += V(X)
    '            End If
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("29")   ' Fx29 - LD F, Vx
    '            ' Set I to the location of the sprite for digit Vx
    '            Select Case V(X)
    '                Case 0

    '                    INDEX = 0
    '                Case 1
    '                    INDEX = 5
    '                Case 2
    '                    INDEX = 10
    '                Case 3
    '                    INDEX = 15
    '                Case 4
    '                    INDEX = 20
    '                Case 5
    '                    INDEX = 25
    '                Case 4
    '                    INDEX = 30
    '                Case 7
    '                    INDEX = 35
    '                Case 8
    '                    INDEX = 40
    '                Case 9
    '                    INDEX = 45
    '                Case 10
    '                    INDEX = 50
    '                Case 11
    '                    INDEX = 55
    '                Case 12
    '                    INDEX = 60
    '                Case 13
    '                    INDEX = 65
    '                Case 14
    '                    INDEX = 70
    '                Case 15
    '                    INDEX = 75
    '            End Select
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("33")   ' Fx33 - LD B, Vx
    '            ' Convert a hex number to decimal
    '            ROM.MEMORY(INDEX) = V(X) / 100                  ' Store hundreds in I
    '            ROM.MEMORY(INDEX + 1) = (V(X) / 10) Mod 10      ' Tens in I + 1
    '            ROM.MEMORY(INDEX + 2) = (V(X) Mod 100) Mod 10   ' And ones in I + 2
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("55")   ' Fx55 - LD [I], Vx
    '            ' Store registers V0 to Vx in memory starting at I
    '            For a As Integer = 0 To X
    '                ROM.MEMORY(INDEX + a) = V(a)
    '            Next
    '            PC += 2
    '        Case OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("65")   ' Fx65 - LD Vx, [I]
    '            ' Read registers V0 to Vx from memory starting at I
    '            For a As Integer = 0 To X
    '                V(a) = ROM.MEMORY(INDEX + a)
    '            Next
    '            PC += 2
    '        Case Else
    '            Emulating = False
    '            MsgBox("Invalid opcode: " & OPCODE & vbNewLine & "PC: " & PC & vbNewLine & "I: " & INDEX & vbNewLine, MsgBoxStyle.OkOnly, "ChipGL Error")
    '    End Select

    'End Function

    Private Function DrawPixel(ByVal x As Integer, ByVal y As Integer)
        x = x Mod 64    ' If X or Y is greater than 64 or 32 respectively, then the pixels must wrap around the screen
        y = y Mod 32
        ScreenData(x + (y * 64)) = ScreenData(x + (y * 64)) Xor 1   ' Pixel is XORed to the screen
        Return ScreenData(x + (y * 64))
    End Function

    Public Sub ClearScreen()
        For d = 0 To 2048       ' Clear screen
            ScreenData(d) = 0
        Next
    End Sub

   



    'Public Function EMULATE_CPU()
    '    Dim OPCODE = (Hex((ROM.MEMORY((PC)) << 8) Or ROM.MEMORY((PC) + 1))).ToString
    '    If OPCODE.Length < 4 Then
    '        While OPCODE.Length < 4
    '            OPCODE = "0" & OPCODE
    '        End While
    '    End If
    '    ' The Chip-8 has 35 different opcodes. (36 if you include 0nnn - SYS addr, but it is ignored by modern interpreters)
    '    ' Each opcode is 2 bytes long
    '    ' Simple, right?
    '    If OPCODE = "00E0" Then ' 00E0, CLS
    '        ClearScreen()       ' Clears the screen
    '        PC += 2
    '    ElseIf OPCODE = "00EE" Then     ' 00EE, RET
    '        PC = STACK(SP)    ' Returns from a subroutine
    '        SP -= 1
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("1") And OPCODE.Length = 4 Then        ' 1nnn, JP addr
    '        Dim addr As Integer = Val("&H" & OPCODE.Substring(1, 3))    ' Jumps to an address at nnn
    '        PC = addr
    '    ElseIf OPCODE.StartsWith("2") And OPCODE.Length = 4 Then     ' 2nnn, CALL addr
    '        Dim addr As Integer = Val("&H" & OPCODE.Substring(1, 3)) ' Calls a subroutine at nnn
    '        SP += 1
    '        STACK(SP) = PC
    '        PC = addr
    '    ElseIf OPCODE.StartsWith("3") And OPCODE.Length = 4 Then    ' 3xkk - SE Vx, byte
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))   ' Skip next opcode if the value in register Vx = kk
    '        Dim bite As Integer = Val("&H" & OPCODE.Substring(2, 2))
    '        If V(x) = bite Then
    '            PC += 4
    '        Else
    '            PC += 2
    '        End If
    '    ElseIf OPCODE.StartsWith("4") And OPCODE.Length = 4 Then    ' 4xkk - SNE Vx, byte
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))   ' Skip next opcode if the value in register Vx != kk
    '        Dim bite As Integer = Val("&H" & OPCODE.Substring(2, 2))
    '        If Not V(x) = bite Then
    '            PC += 4
    '        Else
    '            PC += 2
    '        End If
    '    ElseIf OPCODE.StartsWith("5") And OPCODE.Length = 4 Then    ' 5xy0 - SE Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))   ' Compare the values in the registers Vx and Vy, skip opcode if they are equal
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        If V(x) = V(y) Then
    '            PC += 4
    '        Else
    '            PC += 2
    '        End If
    '    ElseIf OPCODE.StartsWith("6") And OPCODE.Length = 4 Then    ' 6xkk - LD Vx, byte
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))   ' Load kk into register Vx
    '        Dim bite As Byte = Val("&H" & OPCODE.Substring(2, 2))
    '        V(x) = bite
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("7") And OPCODE.Length = 4 Then    ' 7xkk - ADD Vx, byte
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))   ' Add kk to Vx
    '        Dim bite As Byte = Val("&H" & OPCODE.Substring(2, 2))
    '        Dim compare As Integer = CInt(V(x)) + CInt(bite)        ' VB.NET doesn't wrap bytes like C#.
    '        If Not compare < 256 Then : compare -= 256 : End If     ' You can do 255 + 1 = 0 in C#, but VB.NET throws an exception
    '        V(x) = compare
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("0") Then   ' 8xy0 - LD Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Copy the value of register Vy and put it in Vx
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        V(x) = V(y)
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("1") Then   ' 8xy1 - OR Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Do a bitwise OR on Vx and Vy.
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        Dim value As Byte = V(x) Or V(y)    ' Thank goodness VB.NET has bitwise operators
    '        V(x) = value
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("2") Then   ' 8xy2 - AND Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Do a bitwise AND on Vx and Vy
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        Dim value As Byte = V(x) And V(y)
    '        V(x) = value
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("3") Then   ' 8xy3 - XOR Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Do a bitwise XOR on Vx and Vy
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        Dim value As Byte = V(x) Xor V(y)
    '        V(x) = value
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("4") Then   ' 8xy4 - ADD Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Add Vy to Vx, set VF is there is a carry
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        Dim value As Integer = CInt(V(x)) + CInt(V(y))      ' VB.NET stupidness again
    '        If value > 255 Then
    '            V(15) = 1
    '            Dim bite As Byte = Val("&H" & value.ToString("X").Substring(1, 2))
    '            V(x) = bite
    '        Else
    '            V(15) = 0
    '            V(x) = value
    '        End If
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("5") Then   ' 8xy5 - SUB Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Subtract Vy from Vx, store in Vx, set VF if there isn't a borrow
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        If V(x) >= V(y) Then
    '            V(15) = 1
    '            V(x) = V(x) - V(y)
    '        ElseIf Not V(x) >= V(y) Then
    '            V(15) = 0
    '            Dim value As Integer = (CInt(V(x)) + 256) - CInt(V(y))          ' Sigh...
    '            V(x) = value
    '        End If
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("6") Then   ' 8xy6 - SHR Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Bitshift Vx to the right by 1
    '        Dim binary As String = Convert.ToString(V(x), 2).PadLeft(8, "0"c)   ' Convert to binary
    '        If binary.EndsWith("1") Then
    '            V(15) = 1
    '        Else
    '            V(15) = 0
    '        End If

    '        V(x) >>= 1      ' Bitshift by 1 to the right
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("7") Then   ' 8xy7 - SUBN Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Subtract Vx from Vy, store in Vx, set VF is there isn't a borrow
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        If V(y) >= V(x) Then
    '            V(15) = 1
    '            V(x) = V(y) - V(x)
    '        ElseIf Not V(y) >= V(x) Then
    '            V(15) = 0
    '            Dim value As Integer = (CInt(V(y)) + 256) - CInt(V(x))          ' Sigh...
    '            V(x) = value
    '        End If
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("8") And OPCODE.Length = 4 And OPCODE.EndsWith("E") Then   ' 8xyE - SHL Vx {, Vy}
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Bitshift Vx to the left by 1
    '        Dim binary As String = Convert.ToString(V(x), 2).PadLeft(8, "0"c)   ' Convert to binary
    '        If binary.StartsWith("1") Then
    '            V(15) = 1
    '        Else
    '            V(15) = 0
    '        End If
    '        V(x) <<= 1      ' Bitshift by 1 to the left
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("9") And OPCODE.Length = 4 Then    ' 9xy0 - SNE Vx, Vy
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))   ' Skip next opcode if Vx != Vy
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))
    '        If Not V(x) = V(y) Then
    '            PC += 4
    '        Else
    '            PC += 2
    '        End If
    '    ElseIf OPCODE.StartsWith("A") And OPCODE.Length = 4 Then        ' Annn - LD I, addr
    '        Dim addr As Integer = Val("&H" & OPCODE.Substring(1, 3))    ' Set I register to nnn
    '        INDEX = addr
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("B") And OPCODE.Length = 4 Then        ' Bnnn - JP V0, addr
    '        Dim addr As Integer = Val("&H" & OPCODE.Substring(1, 3))    ' Set PC to nnn + the value of V0
    '        PC = addr + V(0)
    '    ElseIf OPCODE.StartsWith("C") And OPCODE.Length = 4 Then        ' Cxkk - RND Vx, byte
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))       ' Set Vx = random byte (0-255) AND kk
    '        Dim bite As Byte = Val("&H" & OPCODE.Substring(2, 2))
    '        Dim r As New Random
    '        Dim value = r.Next(0, 255) And bite
    '        V(x) = value
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("D") And OPCODE.Length = 4 Then        ' Dxyn - DRW Vx, Vy, nibble
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))       ' Draw a n-byte sprite (which is n pixels tall) starting at the memory address in I, at position Vx, Vy
    '        Dim y As Integer = Val("&H" & OPCODE.Substring(2, 1))       ' Set VF is there is sprite collision
    '        Dim height As Integer = Val("&H" & OPCODE.Substring(3, 1))
    '        Dim sprite_x As Byte = V(x)
    '        Dim sprite_y As Byte = V(y)
    '        V(15) = 0
    '        Dim pixel As Integer
    '        For yline = 0 To height - 1
    '            Dim pixel_Y = (sprite_y + yline)
    '            pixel = ROM.MEMORY(INDEX + yline)   ' Get byte from memory
    '            For xline = 0 To 7
    '                Dim pixel_X = (sprite_x + xline)
    '                Dim b As String = Convert.ToString(pixel, 2).PadLeft(8, "0"c).Substring(xline, 1) ' Get binary byte
    '                If b = "1" Then ' If we should draw a pixel...
    '                    If DrawPixel(pixel_X, pixel_Y) = 0 Then ' Draw a pixel, if a pixel was removed...
    '                        V(15) = 1   ' Set collision
    '                    End If
    '                End If
    '            Next
    '        Next
    '        Module1.graphics.DibujarSpritesChip8(ScreenData)
    '        Repaint = True  ' Tell OpenGL to redraw.
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("E") And OPCODE.Length = 4 And OPCODE.EndsWith("9E") Then  ' Ex9E - SKP Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Skip opcode if key (Vx) is pressed
    '        If Keys(V(x)) = True Then
    '            PC += 4
    '        Else
    '            PC += 2
    '        End If
    '    ElseIf OPCODE.StartsWith("E") And OPCODE.Length = 4 And OPCODE.EndsWith("A1") Then  ' ExA1 - SKNP Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Skip opcode if key (Vx) is not pressed
    '        If Keys(V(x)) = False Then
    '            PC += 4
    '        Else
    '            PC += 2
    '        End If
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("07") Then  ' Fx07 - LD Vx, DT
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Set Vx = delay timer value
    '        V(x) = DelayTimer
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("0A") Then  ' Fx0A - LD Vx, K
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Wait for key press, store into Vx
    '        While Emulating = True
    '            If KeyPressFired = True Then
    '                V(x) = LastKeyPress         ' Kinda hacky, but I can't think of a better way
    '                Exit While
    '            End If
    '        End While
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("15") Then  ' Fx15 - LD DT, Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Set delay timer to value of Vx
    '        DelayTimer = V(x)
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("18") Then  ' Fx18 - LD ST, Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Set sound timer to value of Vx
    '        SoundTimer = V(x)
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("1E") Then  ' Fx1E - ADD I, Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Add Vx to register I
    '        If INDEX + V(x) > 4095 Then     ' Stupid VB.NET :(
    '            INDEX = INDEX + V(x) - 4096
    '        Else
    '            INDEX += V(x)
    '        End If
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("29") Then  ' Fx29 - LD F, Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Set I to the location of the sprite for digit Vx
    '        Select Case V(x)
    '            Case 0

    '                INDEX = 0
    '            Case 1
    '                INDEX = 5
    '            Case 2
    '                INDEX = 10
    '            Case 3
    '                INDEX = 15
    '            Case 4
    '                INDEX = 20
    '            Case 5
    '                INDEX = 25
    '            Case 6
    '                INDEX = 30
    '            Case 7
    '                INDEX = 35
    '            Case 8
    '                INDEX = 40
    '            Case 9
    '                INDEX = 45
    '            Case 10
    '                INDEX = 50
    '            Case 11
    '                INDEX = 55
    '            Case 12
    '                INDEX = 60
    '            Case 13
    '                INDEX = 65
    '            Case 14
    '                INDEX = 70
    '            Case 15
    '                INDEX = 75
    '        End Select
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("33") Then  ' Fx33 - LD B, Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Convert a hex number to decimal
    '        ROM.MEMORY(INDEX) = V(x) / 100                  ' Store hundreds in I
    '        ROM.MEMORY(INDEX + 1) = (V(x) / 10) Mod 10      ' Tens in I + 1
    '        ROM.MEMORY(INDEX + 2) = (V(x) Mod 100) Mod 10   ' And ones in I + 2
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("55") Then  ' Fx55 - LD [I], Vx
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Store registers V0 to Vx in memory starting at I
    '        For a = 0 To x
    '            ROM.MEMORY(INDEX + a) = V(a)
    '        Next
    '        PC += 2
    '    ElseIf OPCODE.StartsWith("F") And OPCODE.Length = 4 And OPCODE.EndsWith("65") Then  ' Fx65 - LD Vx, [I]
    '        Dim x As Integer = Val("&H" & OPCODE.Substring(1, 1))                           ' Read registers V0 to Vx from memory starting at I
    '        For a = 0 To x
    '            V(a) = ROM.MEMORY(INDEX + a)
    '        Next
    '        PC += 2
    '    Else
    '        Emulating = False
    '        MsgBox("Invalid opcode: " & OPCODE & vbNewLine & "PC: " & PC & vbNewLine & "I: " & INDEX & vbNewLine, MsgBoxStyle.OkOnly, "ChipGL Error")
    '    End If

    'End Function



End Class
