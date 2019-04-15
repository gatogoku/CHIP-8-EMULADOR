Imports SFML.Graphics
Module Module1
    Public filename As String = ""

    Public ancho As Integer = 64 '600
    Public alto As Integer = 32 '240 400

    Public MEMBIN As New List(Of Integer)
    Public MEMHEX As New List(Of String)
    Public MEMDEC As New List(Of Byte)

    Public CHRMEMDEC As New List(Of Byte)
    Public CHRMEMHEX As New List(Of String)
    Public CHRMEMBIN As New List(Of String)

    Public joypad As New JOYPAD
    Public MAPPEDMEMORY As New List(Of Byte)
    Public Cpu As CPU
    Public Rom As New ROM
    Public graphics As New Graphics


    Public KEY_PRESSED As String
    ' Public AllCPUParams As New List(Of CPU.CPUParams)
    Public OPCODES2 As New List(Of Byte)
    Public OPCODESSTR As New List(Of String)
    Dim col = New Color(&H75, &H75, &H75)
    Public colorPalete() As SFML.Graphics.Color = {
    New Color(&H0, &H0, &H0),New Color(&H0, 247, 255),
        New Color(&H27, &H1B, &H8F),
        New Color(&H0, &H0, &HAB),
        New Color(&H47, &H0, &H9F),
        New Color(&H8F, &H0, &H77),
        New Color(&HAB, &H0, &H13),
        New Color(&H7F, &HB, &H0),
        New Color(&H43, &H2F, &H0),
        New Color(&H0, &H47, &H0),
        New Color(&H0, &H51, &H0),
        New Color(&H0, &H3F, &H17),
        New Color(&H1B, &H3F, &H5F),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0),
            New Color(&HBC, &HBC, &HBC),
        New Color(&H0, &H73, &HEF),
        New Color(&H23, &H3B, &HEF),
        New Color(&H83, &H0, &HF3),
        New Color(&HBF, &H0, &HBF),
        New Color(&HE7, &H0, &H5B),
        New Color(&HDB, &H2B, &H0),
        New Color(&HCB, &H4F, &HF),
        New Color(&H8B, &H73, &H0),
        New Color(&H0, &H97, &H0),
        New Color(&H0, &HAB, &H0),
        New Color(&H0, &H93, &H3B),
        New Color(&H0, &H83, &H8B),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0),
          New Color(&HFF, &HFF, &HFF),
        New Color(&H3F, &HBF, &HFF),
        New Color(&H5F, &H97, &HFF),
        New Color(&HA7, &H8B, &HFD),
        New Color(&HF7, &H7B, &HFF),
        New Color(&HFF, &H77, &HB7),
        New Color(&HFF, &H77, &H63),
        New Color(&HFF, &H9B, &H3B),
        New Color(&HF3, &HBF, &H3F),
        New Color(&H83, &HD3, &H13),
        New Color(&H4F, &HDF, &H4B),
        New Color(&H58, &HF8, &H98),
        New Color(&H0, &HEB, &HDB),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0),
           New Color(&HFF, &HFF, &HFF),
        New Color(&HAB, &HE7, &HFF),
        New Color(&HC7, &HD7, &HFF),
        New Color(&HD7, &HCB, &HFF),
        New Color(&HFF, &HC7, &HFF),
        New Color(&HFF, &HC7, &HDB),
        New Color(&HFF, &HBF, &HB3),
        New Color(&HFF, &HDB, &HAB),
        New Color(&HFF, &HE7, &HA3),
        New Color(&HE3, &HFF, &HA3),
        New Color(&HAB, &HF3, &HBF),
        New Color(&HB3, &HFF, &HCF),
        New Color(&H9F, &HFF, &HF3),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0),
        New Color(&H0, &H0, &H0)
    } '64 colors palette but not all diferent from &H00 TO &H3F


    Public Function DecToBinary(dec As Integer) As String
        Dim bin As Integer
        Dim output As String = ""
        While dec <> 0
            If dec Mod 2 = 0 Then
                bin = 0
            Else
                bin = 1
            End If
            dec = dec \ 2
            output = Convert.ToString(bin) & output
        End While
        If output Is "" Then
            Return "00000000"
        ElseIf output.Length < 8 Then
            While output.Length < 8
                output = "0" & output
            End While
            Return output
        Else
            Return output
        End If
    End Function

    Public INTSPRITE = {{0, 0, 0, 0, 0, 0, 1, 1}, {0, 0, 0, 0, 1, 1, 1, 1}, {0, 0, 0, 1, 1, 1, 1, 1}, {0, 0, 0, 1, 1, 1, 1, 1},
                        {0, 0, 0, 3, 3, 3, 2, 2}, {0, 0, 3, 2, 2, 3, 2, 2}, {0, 0, 3, 2, 2, 3, 2, 2}, {0, 3, 3, 2, 2, 3, 3, 2}
                        }

    Public INTSPRITE3 = {
    {0, 0, 0, 0, 0, 1, 1, 1}, {1, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 1, 1, 1, 1}, {1, 1, 1, 1, 1, 0, 0, 0},
    {0, 0, 0, 0, 2, 2, 2, 3}, {3, 2, 3, 0, 0, 0, 0, 0},
    {0, 0, 0, 2, 3, 2, 3, 3}, {3, 2, 3, 3, 3, 0, 0, 0},
    {0, 0, 0, 2, 3, 2, 2, 3}, {3, 3, 2, 3, 3, 3, 0, 0},
    {0, 0, 0, 2, 2, 3, 3, 3}, {3, 2, 2, 2, 2, 0, 0, 0},
    {0, 0, 0, 0, 0, 3, 3, 3}, {3, 3, 3, 3, 0, 0, 0, 0},
    {0, 0, 0, 0, 2, 2, 1, 2}, {2, 2, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 2, 2, 2, 1, 2}, {2, 1, 2, 2, 2, 0, 0, 0},
    {0, 0, 2, 2, 2, 2, 1, 1}, {1, 1, 2, 2, 2, 2, 0, 0},
    {0, 0, 3, 3, 2, 1, 3, 1}, {1, 3, 1, 2, 3, 3, 0, 0},
    {0, 0, 3, 3, 3, 1, 1, 1}, {1, 1, 1, 3, 3, 3, 0, 0},
    {0, 0, 3, 3, 1, 1, 1, 1}, {1, 1, 1, 1, 3, 3, 0, 0},
    {0, 0, 0, 0, 1, 1, 1, 0}, {0, 1, 1, 1, 0, 0, 0, 0},
    {0, 0, 0, 2, 2, 2, 0, 0}, {0, 0, 2, 2, 2, 0, 0, 0},
    {0, 0, 2, 2, 2, 2, 0, 0}, {0, 0, 2, 2, 2, 2, 0, 0}
                          }

    Public MARIO_TILE = {
  {0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
  {0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0},
  {0, 0, 0, 0, 2, 2, 2, 3, 3, 2, 3, 0, 0, 0, 0, 0},
  {0, 0, 0, 2, 3, 2, 3, 3, 3, 2, 3, 3, 3, 0, 0, 0},
  {0, 0, 0, 2, 3, 2, 2, 3, 3, 3, 2, 3, 3, 3, 0, 0},
  {0, 0, 0, 2, 2, 3, 3, 3, 3, 2, 2, 2, 2, 0, 0, 0},
  {0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0},
  {0, 0, 0, 0, 2, 2, 1, 2, 2, 2, 0, 0, 0, 0, 0, 0},
  {0, 0, 0, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 0, 0, 0},
  {0, 0, 2, 2, 2, 2, 1, 1, 1, 1, 2, 2, 2, 2, 0, 0},
  {0, 0, 3, 3, 2, 1, 3, 1, 1, 3, 1, 2, 3, 3, 0, 0},
  {0, 0, 3, 3, 3, 1, 1, 1, 1, 1, 1, 3, 3, 3, 0, 0},
  {0, 0, 3, 3, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 0, 0},
  {0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0},
  {0, 0, 0, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 0, 0, 0},
  {0, 0, 2, 2, 2, 2, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0}
                        }



    Public Function BinaryToDecimal(Binary As String) As Long
        Dim n As Long
        Dim s As Integer

        For s = 1 To Len(Binary)
            n = n + (Mid(Binary, Len(Binary) - s + 1, 1) * (2 ^ _
                (s - 1)))
        Next s

        BinaryToDecimal = n
    End Function

    Public Function DecimalToBinary(DecimalNum As Long) As _
        String
        Dim tmp As String
        Dim n As Long

        n = DecimalNum

        tmp = Trim(Str(n Mod 2))
        n = n \ 2

        Do While n <> 0
            tmp = Trim(Str(n Mod 2)) & tmp
            n = n \ 2
        Loop

        DecimalToBinary = tmp
    End Function


    Public Function HextoBinary(HexVal As String) As String
        Dim binVal As String
        If Len(HexVal) = 1 Then
            binVal = Hex_MAP_BIN(HexVal)
            If binVal <> "" Then
                HextoBinary = binVal
                Debug.Print(HexVal & "-" & binVal)
            Else
                Exit Function
            End If
        Else
            binVal = HextoBinary(Mid(HexVal, 1, Len(HexVal) - 1)) & HextoBinary(Mid(HexVal, Len(HexVal), 1))
            HextoBinary = binVal

        End If



    End Function


    Public Function Hex_MAP_BIN(HexVal As String) As String
        Select Case UCase(HexVal)
            Case "0"
                Hex_MAP_BIN = "0000"
            Case "1"
                Hex_MAP_BIN = "0001"
            Case "2"
                Hex_MAP_BIN = "0010"
            Case "3"
                Hex_MAP_BIN = "0011"
            Case "4"
                Hex_MAP_BIN = "0100"
            Case "5"
                Hex_MAP_BIN = "0101"
            Case "6"
                Hex_MAP_BIN = "0110"
            Case "7"
                Hex_MAP_BIN = "0111"
            Case "8"
                Hex_MAP_BIN = "1000"
            Case "9"
                Hex_MAP_BIN = "1001"
            Case "A"
                Hex_MAP_BIN = "1010"
            Case "B"
                Hex_MAP_BIN = "1011"
            Case "C"
                Hex_MAP_BIN = "1100"
            Case "D"
                Hex_MAP_BIN = "1101"
            Case "E"
                Hex_MAP_BIN = "1110"
            Case "F"
                Hex_MAP_BIN = "1111"
            Case Else

        End Select

    End Function

    Public Function HexToDecimal(HexVal As String) As Long
        Dim binVal As String
        Dim decVal As Long

        binVal = HextoBinary(HexVal)
        If binVal = "01011110101101010010" Then MsgBox ("OK")

        decVal = BinaryToDecimal(binVal)
        HexToDecimal = decVal
    End Function

End Module
