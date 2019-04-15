Public Class JOYPAD
    '1 2 3 C                                   1 2 3 4
    '4 5 6 D    This is emulated with these    Q W E R
    '7 8 9 E    keyboard keys -->              A S D F
    'A 0 B F                                   Z X C V


    'Dim CodigosAsciNativos = {
    Dim CodigosAsciRemap2 = {
 &H31,
 &H32,
 &H43,
 &H33,
 &H34,
 &H35,
 &H36,
 &H44,
 &H37,
 &H38,
 &H39,
 &H45,
 &H41,
 &H30,
 &H42,
 &H46
}


    Dim CodigosAsciRemap = {
&H31,
&H32,
&H33,
&H34,
&H51,
&H57,
&H45,
&H52,
&H41,
&H53,
&H44,
&H46,
&H5A,
&H58,
&H43,
&H56
}


    Public Sub fetchKeyBoard()
        '1 2 3 C                                   1 2 3 4
        '4 5 6 D    This is emulated with these    Q W E R
        '7 8 9 E    keyboard keys -->              A S D F
        'A 0 B F                                   Z X C V
        If Keyboard.IsKeyPressed(Keyboard.Key.Num1) Then
            ' MsgBox(Keyboard.Key.Num1)
            KEY_PRESSED = CodigosAsciRemap2(0)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Num2) Then
            ' MsgBox((Keyboard.Key.Num2))
            KEY_PRESSED = CodigosAsciRemap2(1)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Num3) Then
            ' MsgBox((Keyboard.Key.Num3))
            KEY_PRESSED = CodigosAsciRemap2(2)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Num4) Then
            '  MsgBox((Keyboard.Key.Num4))
            KEY_PRESSED = CodigosAsciRemap2(3)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Q) Then
            '  MsgBox((Keyboard.Key.Q))
            KEY_PRESSED = CodigosAsciRemap2(4)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.W) Then
            '  MsgBox((Keyboard.Key.W))
            KEY_PRESSED = CodigosAsciRemap2(5)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.E) Then
            '   MsgBox((Keyboard.Key.E))
            KEY_PRESSED = CodigosAsciRemap2(6)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.R) Then
            '  MsgBox((Keyboard.Key.R))
            KEY_PRESSED = CodigosAsciRemap2(7)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.A) Then
            '  MsgBox((Keyboard.Key.A))
            KEY_PRESSED = CodigosAsciRemap2(8)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.S) Then
            KEY_PRESSED = CodigosAsciRemap2(9)
            ' MsgBox((Keyboard.Key.S))
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.D) Then
            '  MsgBox((Keyboard.Key.D))
            KEY_PRESSED = CodigosAsciRemap2(10)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.F) Then
            ' MsgBox((Keyboard.Key.F))
            KEY_PRESSED = CodigosAsciRemap2(11)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.Z) Then
            '  MsgBox((Keyboard.Key.Z))
            KEY_PRESSED = CodigosAsciRemap2(12)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.X) Then
            ' MsgBox((Keyboard.Key.X))
            KEY_PRESSED = CodigosAsciRemap2(13)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.C) Then
            ' MsgBox((Keyboard.Key.C))
            KEY_PRESSED = CodigosAsciRemap2(14)
        ElseIf Keyboard.IsKeyPressed(Keyboard.Key.V) Then
            ' MsgBox((Keyboard.Key.V))
            KEY_PRESSED = CodigosAsciRemap2(15)
        Else
            'KEY_PRESSED = ""
        End If
        ' MsgBox(KEY_PRESSED)
    End Sub




  



End Class
