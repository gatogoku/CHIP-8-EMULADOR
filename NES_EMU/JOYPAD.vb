Public Class JOYPAD
    '1 2 3 C                                   1 2 3 4
    '4 5 6 D    This is emulated with these    Q W E R
    '7 8 9 E    keyboard keys -->              A S D F
    'A 0 B F                                   Z X C V
    Public Hex_MAP_BIN = {
    15, 1, 2, 3, 12, 4, 5, 6, 13, 7, 8, 9, 14, 10, 0, 11
}


    Public Sub fetchKeyBoard()
        If Keyboard.IsKeyPressed(Keyboard.Key.Num1) Then
            ' MsgBox(Keyboard.Key.Num1)
            KEY_PRESSED = Hex_MAP_BIN(1)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Num2) Then
            ' MsgBox((Keyboard.Key.Num2))
            KEY_PRESSED = Hex_MAP_BIN(2) '  KEY_PRESSED = CodigosAsciRemap2(1)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Num3) Then
            ' MsgBox((Keyboard.Key.Num3))
            KEY_PRESSED = Hex_MAP_BIN(3) '   KEY_PRESSED = CodigosAsciRemap2(2)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Num4) Then
            '  MsgBox((Keyboard.Key.Num4))
            KEY_PRESSED = Hex_MAP_BIN(4) '   KEY_PRESSED = CodigosAsciRemap2(3)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Q) Then
            '  MsgBox((Keyboard.Key.Q))
            KEY_PRESSED = Hex_MAP_BIN(5) '   KEY_PRESSED = CodigosAsciRemap2(4)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.W) Then
            '  MsgBox((Keyboard.Key.W))
            KEY_PRESSED = Hex_MAP_BIN(6) '  KEY_PRESSED = CodigosAsciRemap2(5)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.E) Then
            '   MsgBox((Keyboard.Key.E))
            KEY_PRESSED = Hex_MAP_BIN(7) '  KEY_PRESSED = CodigosAsciRemap2(6)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.R) Then
            '  MsgBox((Keyboard.Key.R))
            KEY_PRESSED = Hex_MAP_BIN(8) '  KEY_PRESSED = CodigosAsciRemap2(7)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.A) Then
            '  MsgBox((Keyboard.Key.A))
            KEY_PRESSED = Hex_MAP_BIN(9) '  KEY_PRESSED = CodigosAsciRemap2(8)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.S) Then
            KEY_PRESSED = Hex_MAP_BIN(10) '  KEY_PRESSED = CodigosAsciRemap2(9)
            ' MsgBox((Keyboard.Key.S))
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.D) Then
            '  MsgBox((Keyboard.Key.D))
            KEY_PRESSED = Hex_MAP_BIN(11) '   KEY_PRESSED = CodigosAsciRemap2(10)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.F) Then
            ' MsgBox((Keyboard.Key.F))
            KEY_PRESSED = Hex_MAP_BIN(12) '   KEY_PRESSED = CodigosAsciRemap2(11)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Z) Then
            '  MsgBox((Keyboard.Key.Z))
            KEY_PRESSED = Hex_MAP_BIN(13) '   KEY_PRESSED = CodigosAsciRemap2(12)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.X) Then
            ' MsgBox((Keyboard.Key.X))
            KEY_PRESSED = Hex_MAP_BIN(14) '   KEY_PRESSED = CodigosAsciRemap2(13)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.C) Then
            ' MsgBox((Keyboard.Key.C))
            KEY_PRESSED = Hex_MAP_BIN(15) '   KEY_PRESSED = CodigosAsciRemap2(14)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.V) Then
            ' MsgBox((Keyboard.Key.V))
            KEY_PRESSED = Hex_MAP_BIN(0) '    KEY_PRESSED = CodigosAsciRemap2(15)
        Else
            KEY_PRESSED = ""
        End If
        '  MsgBox(KEY_PRESSED)
    End Sub

End Class
