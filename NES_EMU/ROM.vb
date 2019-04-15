Public Class ROM
    Public Shared MEMORY As New List(Of String)  '= La memoria del CHIP-8 tienen un rango 200h a FFFh
    Public Shared DECODEDMEMORY As New List(Of String)
    Public Shared MEMORYBIN As New List(Of String) '= La memoria del CHIP-8 tienen un rango 200h a FFFh
    Public Shared MEMORYHEX As New List(Of String)  '= La memoria del CHIP-8 tienen un rango 200h a FFFh
    Public Shared CPU_BANK_1_INIT As Integer = &H200  'CPU $ 6000- $ 7FFF: solo Family Basic: PRG RAM, duplicado SI necesario para llenar la ventana completa de 8 KiB, protegido contra escritura 
    Public Shared CPU_BANK_1_END As Integer = &HFFF

    'Public Shared FONT_INIT As Integer = &HF  'CPU $ 6000- $ 7FFF: solo Family Basic: PRG RAM, duplicado SI necesario para llenar la ventana completa de 8 KiB, protegido contra escritura 
    'Public Shared FONT_END As Integer = &HFFF

    'Public Shared MEMORY_FONT As New List(Of String)

    '0xF?? - 0xFFF built in 4x5 pixel font set, A-F, 1-9.
    '0x200 - 0xF?? Program Rom and work RAM
    Public Structure MAPPERNROM0
        Public Shared Sub CARGAR_MEMORIA()

            For c As Integer = 0 To CPU_BANK_1_END : MAPPEDMEMORY.Add(Nothing) : MEMORY.Add(Nothing) : Next ' 3583 bytes of memory
            Dim index = 0
            For c As Integer = CPU_BANK_1_INIT To MEMHEX.Count + CPU_BANK_1_INIT - 1 : MAPPEDMEMORY(c) = MEMHEX(index) : index += 1 : Next

            ' index = 0
            ' For c As Integer = FONT_INIT To FONT_END - 1 : MEMORY_FONT.Add(MEMHEX(c)) : index += 1 : Next
            CONVERTTOHEXANDBIN(MAPPEDMEMORY)
            DECODEOPCODESMEMORY()
        End Sub
    End Structure

    Public Shared Sub CONVERTTOHEXANDBIN(ByVal mem As List(Of Byte))
        For C As Integer = 0 To mem.Count - 1
            Dim X = Convert.ToString(mem(C), 16).ToUpper
            MEMORYHEX.Add("&H" + X)
            MEMORYBIN.Add(Module1.DecToBinary(mem(C)))
            MEMORY.Item(C) = ("&H" + X)
        Next
    End Sub

    Public Shared Sub DECODEOPCODESMEMORY()
        ' Dim OPCODE
        For c As Integer = 0 To MEMORY.Count - 1 Step 2
            'OPCODE = ""
            'Dim FetchOpCode = Module1.DecToBinary(ROM.MEMORY(c) << 8 Or ROM.MEMORY(c + 1)) 'And &HF000
            'Dim Y = Convert.ToString(Int64.Parse(FetchOpCode), 16).ToUpper()
            'Dim X = ("&H" + Y)
            'OPCODE = (X And &HF000) >> 12 'And &HFFF
            'Dim S = OPCODE And &HF000
            'Y = Convert.ToString(Int64.Parse(OPCODE), 16).ToUpper()
            'X = ("&H" + Y)


            DECODEDMEMORY.Add(("&H" + Hex((ROM.MEMORY(c) << 8) Or ROM.MEMORY(c + 1))))
            ' DECODEDMEMORY.Add(X)

        Next

    End Sub
End Class


''''cpu backup 



'  PC += 2

'Function convBinToDec(strBin As String) As Long
'    Dim lTemp As Long, i As Long
'    lTemp = 0
'    For i = Len(strBin) To 1 Step -1
'        If Mid(strBin, i, 1) = "1" Then
'            lTemp = lTemp + 2 ^ (Len(strBin) - i)
'        End If
'    Next i
'    Return lTemp
'End Function



'Select Case (X)
'    Case INDEX Like "0???" '0NNN	Salta a un código de rutina en NNN. Se usaba en los viejos computadores que implementaban Chip-8. Los intérpretes actuales lo ignoran
'        ' Case &H0
'        '   Select Case (OPCODE And &HF)
'    Case "&H0" '00E0	Limpia la pantalla.   
'        DRAW_SCREEN_FLAG = True
'    Case "&HE" '00EE	Retorna de una subrutina. Se decrementa en 1 el Stack Pointer (SP). El intérprete establece el Program Counter como la dirección donde apunta el SP en la Pila.
'        SP -= 1 : PC = SP
'        '  End Select
'    Case "&H1000" '1NNN	Salta a la dirección NNN. El intérprete establece el Program Counter a NNN.
'        '                For example 1NNN is the jump. So:
'        'int a = opcode & 0xF000;
'        'int b = opcode & 0x0FFF;
'        'if(a == 0x1000)
'        '   doJump(b);
'        PC = JNNN
'    Case "&H2000" '2NNN	Llama a la subrutina NNN. El intérprete incrementa el Stack Pointer, luego pone el actual PC en el tope de la Pila. El PC se establece a NNN.
'        SP += 1 : SP = PC : PC = JNNN
'    Case "&H3000" '3XNN	Salta a la siguiente instrucción si VX = NN. El intérprete compara el registro VX con el NN, y si son iguales, incrementa el PC en 2.
'        If VX = JNN Then PC += 2
'    Case "&H4000" '4XKK	Salta a la siguiente instrucción si VX != KK. El intérprete compara el registro VX con el KK, y si no son iguales, incrementa el PC en 2.
'        If VX <> CONST_K Then PC += 2
'    Case "&H5000" '5XY0	Salta a la siguiente instrucción si VX = VY. El intérprete compara el registro VX con el VY, y si no son iguales, incrementa el PC en 2.
'        If VX = VY Then PC += 2
'    Case "&H6000" '6XKK	Hace VX = KK. El intérprete coloca el valor KK dentro del registro VX.
'        VX = CONST_K
'    Case "&H8000" '7XKK	Hace VX = VX + KK. Suma el valor de KK al valor de VX y el resultado lo deja en VX.
'        VX = VX + CONST_K
'    Case "&H0"  '8XY0	Hace VX = VY. Almacena el valor del registro VY en el registro VX.
'        VX = VY
'    Case "&H1"  '8XY1	Hace VX = VX OR VY. 'Realiza un bitwise OR (OR Binario) sobre los valores de VX y VY, entonces almacena el resultado en VX. Un OR binario compara cada uno de los bit respectivos desde 2 valores, y si al menos uno es verdadero (1), entonces el mismo bit en el resultado es 1. De otra forma es 0.
'        VX = VX Or VY
'    Case "&H2" '8XY2	Hace VX = VX AND VY.
'        VX = VX And VY
'    Case "&H3" '8XY3	Hace VX = VX XOR VY.
'        VX = VX Xor VY
'    Case "&H4" '8XY4	Suma VY a VX. VF se pone a 1 cuando hay un acarreo (carry), y a 0 cuando no.
'        If (VX + VY).ToString.Length > 8 Then V(15) = 1
'    Case "&H5" '8XY5	VY se resta de VX. VF se pone a 0 cuando hay que restarle un dígito al número de la izquierda, más conocido como "pedir prestado" o borrow, y se pone a 1 cuando no es necesario.
'        If VY - VX < 0 Then V(15) = 1
'    Case "&H6" '8XY6	Establece VF = 1 o 0 según bit menos significativo de VX. Divide VX por 2.
'        If VX << 7 = 1 Then V(15) = 1
'        V(15) = VX = VX / 2
'    Case "&H7" '8XY7	Si VY > VX => VF = 1, sino 0. VX = VY - VX.
'        If VX < VY Then V(15) = 1 Else V(15) = 0
'        VX = VY - VX
'    Case "&HE" '8XYE	Establece VF = 1 o 0 según bit más significativo de VX. Multiplica VX por 2.
'        If VX >> 7 = 1 Then V(15) = 1 Else V(15) = 0
'        VX = VX * 2
'    Case "&H9000" '9XY0	Salta a la siguiente instrucción si VX != VY.
'        If VX <> VY Then PC += 1
'    Case "&HA000" 'ANNN	Establece I = NNNN.
'        INDEX = JNNNN
'    Case "&HB000" 'BNNN	Salta a la ubicación V[0]+ NNNN.
'        PC = V(0) + JNNNN
'    Case "&HC" 'CXKK	Establece VX = un Byte Aleatorio AND KK.
'        VX = New Random().Next(0, 255) + CONST_K
'    Case "&HD000" 'DXYN	Pinta un sprite en la pantalla. El intérprete lee N bytes desde la memoria, comenzando desde el contenido del registro I. Y se muestra dicho byte en las posiciones VX, VY de la pantalla.
'        'A los sprites que se pintan se le aplica XOR con lo que está en pantalla. Si esto causa que algún pixel se borre, el registro VF se setea a 1, de otra forma se setea a 0. Si el sprite se posiciona afuera de las coordenadas de la pantalla, dicho sprite se le hace aparecer en el lado opuesto de la pantalla.
'        DRAW_SCREEN_FLAG = True
'        'Module1.graphics.pintarSprite()
'        '                case 0xD000:		   
'        '{
'        '  unsigned short x = V[(opcode & 0x0F00) >> 8];
'        '  unsigned short y = V[(opcode & 0x00F0) >> 4];
'        '  unsigned short height = opcode & 0x000F;
'        '  unsigned short pixel;

'        '  V[0xF] = 0;
'        '  for (int yline = 0; yline < height; yline++)
'        '  {
'        '    pixel = memory[I + yline];
'        '    for(int xline = 0; xline < 8; xline++)
'        '    {
'        '      if((pixel & (0x80 >> xline)) != 0)
'        '      {
'        '        if(gfx[(x + xline + ((y + yline) * 64))] == 1)
'        '          V[0xF] = 1;                                 
'        '        gfx[x + xline + ((y + yline) * 64)] ^= 1;
'        '      }
'        '    }
'        '  }

'        '  drawFlag = true;
'        '  pc += 2;
'        '}
'        'break;

'    Case "&HE000" 'EX9E	Salta a la siguiente instrucción si valor de VX coincide con tecla presionada.

'    Case "EXA1" 'EXA1	Salta a la siguiente instrucción si valor de VX no coincide con tecla presionada (soltar tecla). ?? QUE PASA CON LE CPODIGO DE ESTE ???????????????????????????
'    Case "&HF000" 'FX07	Establece Vx = valor del delay timer.
'    Case "FX0A" 'FX0A	Espera por una tecla presionada y la almacena en el registro.
'    Case "&H15" 'FX15	Establece Delay Timer = VX.
'    Case "&H18" 'FX18	Establece Sound Timer = VX.
'    Case "&H1E" 'FX1E	Índice = Índice + VX.
'        INDEX = INDEX + VX
'    Case "&H29" 'FX29	Establece I = VX * largo Sprite Chip-8.

'    Case "&H33" 'FX33	Guarda la representación de VX en formato humano. Poniendo las centenas en la posición de memoria I, las decenas en I + 1 y las unidades en I + 2
'        ROM.MEMORY(INDEX) = VX >> 2
'        ROM.MEMORY(INDEX + 1) = VX >> 1
'        ROM.MEMORY(INDEX + 2) = VX '>> 3
'    Case "&H55" 'FX55	Almacena el contenido de V0 a VX en la memoria empezando por la dirección I
'        For c As Integer = INDEX To INDEX + 16
'            ROM.MEMORY(INDEX) = V(c - INDEX)
'        Next
'        ROM.MEMORY(INDEX + 1) = VX : ROM.MEMORY(INDEX + 2) = VY
'    Case "&H65" 'FX65	Almacena el contenido de la dirección de memoria I en los registros del V0 al VX
'        For c As Integer = INDEX To INDEX + 16
'            V(c - INDEX) = ROM.MEMORY(INDEX)
'        Next
'        VX = ROM.MEMORY(INDEX + 1) : VY = ROM.MEMORY(INDEX + 2)
'    Case Else
'        PC += 2
'End Select














'Select Case (X)
'    Case "&H0"
'        DRAW_SCREEN_FLAG = True
'    Case "&HE" '00EE	Retorna de una subrutina. Se decrementa en 1 el Stack Pointer (SP). El intérprete establece el Program Counter como la dirección donde apunta el SP en la Pila.
'        SP -= 1 : PC = SP
'    Case "&H1000"
'    Case "&H2000"
'    Case "&H3000"
'    Case "&H4000"
'    Case "&H5000"
'    Case "&H6000"
'    Case "&H8000"
'    Case "&H0"
'    Case "&H1"
'    Case "&H2"
'    Case "&H3"
'    Case "&H4"
'    Case "&H5"
'    Case "&H6"
'    Case "&H7"
'    Case "&HE"
'    Case "&H9000"
'    Case "&HA000"
'    Case "&HB000"
'    Case "&HC000"
'    Case "&HD000"
'    Case "&HE000"
'    Case "&HF000"
'    Case "&H15"
'    Case "&H18"
'    Case "&H1E"
'    Case "&H29"
'    Case "&H33"
'    Case "&H55"
'    Case "&H65"

'End Select

'Select Case (X)
'    Case INDEX Like "0???" '0NNN	Salta a un código de rutina en NNN. Se usaba en los viejos computadores que implementaban Chip-8. Los intérpretes actuales lo ignoran.

'        ' Case &H0
'        '   Select Case (OPCODE And &HF)

'    Case "&H0" '00E0	Limpia la pantalla.   
'        DRAW_SCREEN_FLAG = True
'    Case "&HE" '00EE	Retorna de una subrutina. Se decrementa en 1 el Stack Pointer (SP). El intérprete establece el Program Counter como la dirección donde apunta el SP en la Pila.
'        SP -= 1 : PC = SP
'        '  End Select

'    Case "&H1000" '1NNN	Salta a la dirección NNN. El intérprete establece el Program Counter a NNN.
'        '                For example 1NNN is the jump. So:
'        'int a = opcode & 0xF000;
'        'int b = opcode & 0x0FFF;
'        'if(a == 0x1000)
'        '   doJump(b);
'        PC = JNNN
'    Case "2NNN" '2NNN	Llama a la subrutina NNN. El intérprete incrementa el Stack Pointer, luego pone el actual PC en el tope de la Pila. El PC se establece a NNN.
'        SP += 1 : SP = PC : PC = JNNN
'    Case "3XNN" '3XNN	Salta a la siguiente instrucción si VX = NN. El intérprete compara el registro VX con el NN, y si son iguales, incrementa el PC en 2.
'        If VX = JNN Then PC += 2
'    Case "4XKK" '4XKK	Salta a la siguiente instrucción si VX != KK. El intérprete compara el registro VX con el KK, y si no son iguales, incrementa el PC en 2.
'        If VX <> CONST_K Then PC += 2
'    Case "5XY0" '5XY0	Salta a la siguiente instrucción si VX = VY. El intérprete compara el registro VX con el VY, y si no son iguales, incrementa el PC en 2.
'        If VX = VY Then PC += 2
'    Case "6XKK" '6XKK	Hace VX = KK. El intérprete coloca el valor KK dentro del registro VX.
'        VX = CONST_K
'    Case "7XKK" '7XKK	Hace VX = VX + KK. Suma el valor de KK al valor de VX y el resultado lo deja en VX.
'        VX = VX + CONST_K
'    Case "8XY0"  '8XY0	Hace VX = VY. Almacena el valor del registro VY en el registro VX.
'        VX = VY
'    Case "8XY1"  '8XY1	Hace VX = VX OR VY. 'Realiza un bitwise OR (OR Binario) sobre los valores de VX y VY, entonces almacena el resultado en VX. Un OR binario compara cada uno de los bit respectivos desde 2 valores, y si al menos uno es verdadero (1), entonces el mismo bit en el resultado es 1. De otra forma es 0.
'        VX = VX Or VY
'    Case "8XY2" '8XY2	Hace VX = VX AND VY.
'        VX = VX And VY
'    Case "8XY3" '8XY3	Hace VX = VX XOR VY.
'        VX = VX Xor VY
'    Case "8XY4" '8XY4	Suma VY a VX. VF se pone a 1 cuando hay un acarreo (carry), y a 0 cuando no.
'        If (VX + VY).ToString.Length > 8 Then V(15) = 1
'    Case "8XY5" '8XY5	VY se resta de VX. VF se pone a 0 cuando hay que restarle un dígito al número de la izquierda, más conocido como "pedir prestado" o borrow, y se pone a 1 cuando no es necesario.
'        If VY - VX < 0 Then V(15) = 1
'    Case "8XY6" '8XY6	Establece VF = 1 o 0 según bit menos significativo de VX. Divide VX por 2.
'        If VX << 7 = 1 Then V(15) = 1
'        V(15) = VX = VX / 2
'    Case "8XY7" '8XY7	Si VY > VX => VF = 1, sino 0. VX = VY - VX.
'        If VX < VY Then V(15) = 1 Else V(15) = 0
'        VX = VY - VX
'    Case "8XYE" '8XYE	Establece VF = 1 o 0 según bit más significativo de VX. Multiplica VX por 2.
'        If VX >> 7 = 1 Then V(15) = 1 Else V(15) = 0
'        VX = VX * 2
'    Case "9XY0" '9XY0	Salta a la siguiente instrucción si VX != VY.
'        If VX <> VY Then PC += 1
'    Case "ANNN" 'ANNN	Establece I = NNNN.
'        INDEX = JNNNN
'    Case "BNNN" 'BNNN	Salta a la ubicación V[0]+ NNNN.
'        PC = V(0) + JNNNN
'    Case "CXKK" 'CXKK	Establece VX = un Byte Aleatorio AND KK.
'        VX = New Random().Next(0, 255) + CONST_K
'    Case "DXYN" 'DXYN	Pinta un sprite en la pantalla. El intérprete lee N bytes desde la memoria, comenzando desde el contenido del registro I. Y se muestra dicho byte en las posiciones VX, VY de la pantalla.
'        'A los sprites que se pintan se le aplica XOR con lo que está en pantalla. Si esto causa que algún pixel se borre, el registro VF se setea a 1, de otra forma se setea a 0. Si el sprite se posiciona afuera de las coordenadas de la pantalla, dicho sprite se le hace aparecer en el lado opuesto de la pantalla.
'        DRAW_SCREEN_FLAG = True
'        'Module1.graphics.pintarSprite()
'        '                case 0xD000:		   
'        '{
'        '  unsigned short x = V[(opcode & 0x0F00) >> 8];
'        '  unsigned short y = V[(opcode & 0x00F0) >> 4];
'        '  unsigned short height = opcode & 0x000F;
'        '  unsigned short pixel;

'        '  V[0xF] = 0;
'        '  for (int yline = 0; yline < height; yline++)
'        '  {
'        '    pixel = memory[I + yline];
'        '    for(int xline = 0; xline < 8; xline++)
'        '    {
'        '      if((pixel & (0x80 >> xline)) != 0)
'        '      {
'        '        if(gfx[(x + xline + ((y + yline) * 64))] == 1)
'        '          V[0xF] = 1;                                 
'        '        gfx[x + xline + ((y + yline) * 64)] ^= 1;
'        '      }
'        '    }
'        '  }

'        '  drawFlag = true;
'        '  pc += 2;
'        '}
'        'break;
'    Case "EX9E" 'EX9E	Salta a la siguiente instrucción si valor de VX coincide con tecla presionada.
'    Case "EXA1" 'EXA1	Salta a la siguiente instrucción si valor de VX no coincide con tecla presionada (soltar tecla).
'    Case "FX07" 'FX07	Establece Vx = valor del delay timer.
'    Case "FX0A" 'FX0A	Espera por una tecla presionada y la almacena en el registro.
'    Case "FX15" 'FX15	Establece Delay Timer = VX.
'    Case "FX18" 'FX18	Establece Sound Timer = VX.
'    Case "FX1E" 'FX1E	Índice = Índice + VX.
'        INDEX = INDEX + VX
'    Case "FX29" 'FX29	Establece I = VX * largo Sprite Chip-8.

'    Case "FX33" 'FX33	Guarda la representación de VX en formato humano. Poniendo las centenas en la posición de memoria I, las decenas en I + 1 y las unidades en I + 2
'        ROM.MEMORY(INDEX) = VX >> 2
'        ROM.MEMORY(INDEX + 1) = VX >> 1
'        ROM.MEMORY(INDEX + 2) = VX '>> 3
'    Case "FX55" 'FX55	Almacena el contenido de V0 a VX en la memoria empezando por la dirección I
'        For c As Integer = INDEX To INDEX + 16
'            ROM.MEMORY(INDEX) = V(c - INDEX)
'        Next
'        ROM.MEMORY(INDEX + 1) = VX : ROM.MEMORY(INDEX + 2) = VY
'    Case "FX65" 'FX65	Almacena el contenido de la dirección de memoria I en los registros del V0 al VX
'        For c As Integer = INDEX To INDEX + 16
'            V(c - INDEX) = ROM.MEMORY(INDEX)
'        Next
'        VX = ROM.MEMORY(INDEX + 1) : VY = ROM.MEMORY(INDEX + 2)
'    Case Else
'        PC += 2
'End Select

