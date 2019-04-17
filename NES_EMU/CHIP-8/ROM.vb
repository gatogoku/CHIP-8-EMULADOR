Public Class ROM
    Public Shared MEMORY As New List(Of String)  '= La memoria del CHIP-8 tienen un rango 200h a FFFh
    Public Shared DECODEDMEMORY As New List(Of String)
    Public Shared MEMORYBIN As New List(Of String) '= La memoria del CHIP-8 tienen un rango 200h a FFFh
    Public Shared MEMORYHEX As New List(Of String)  '= La memoria del CHIP-8 tienen un rango 200h a FFFh
    Public Shared CPU_BANK_1_INIT As Integer = &H200  'CPU $ 6000- $ 7FFF: solo Family Basic: PRG RAM, duplicado SI necesario para llenar la ventana completa de 8 KiB, protegido contra escritura 
    Public Shared CPU_BANK_1_END As Integer = &HFFF

    Public Structure MAPPERNROM0
        Public Shared Sub CARGAR_MEMORIA()
            ROM.MEMORY.Clear() : MAPPEDMEMORY.Clear() : MEMORYBIN.Clear() : MEMORYHEX.Clear() : DECODEDMEMORY.Clear()
            For c As Integer = 0 To CPU_BANK_1_END : MAPPEDMEMORY.Add(Nothing) : MEMORY.Add(Nothing) : Next ' 3583 bytes of memory
            Dim index = 0
            For c As Integer = CPU_BANK_1_INIT To MEMHEX.Count + CPU_BANK_1_INIT - 1 : MAPPEDMEMORY(c) = MEMHEX(index) : index += 1 : Next
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
        For c As Integer = 0 To MEMORY.Count - 1 Step 2 : DECODEDMEMORY.Add(("&H" + Hex((ROM.MEMORY(c) << 8) Or ROM.MEMORY(c + 1)))): Next
    End Sub

End Class

