Imports SFML.Graphics
Module Module1

    Public EMULATOR_IS_RUNNING As Boolean

    Public filename As String = ""
    Public GAME_COLOR As Color

    Public ancho As Integer = 64 '600
    Public alto As Integer = 32 '240 400

    Public MEMBIN As New List(Of Integer)
    Public MEMHEX As New List(Of String)
    Public MEMDEC As New List(Of Byte)

    Public CHRMEMDEC As New List(Of Byte)
    Public CHRMEMHEX As New List(Of String)
    Public CHRMEMBIN As New List(Of String)
    Public MAPPEDMEMORY As New List(Of Byte)

    Public JoyPad As New JOYPAD
    Public Graphics As New Graphics
    Public Rom As New ROM
    Public Cpu As CPU

    Public KEY_PRESSED As String

    Public OPCODES2 As New List(Of Byte)
    Public OPCODESSTR As New List(Of String)

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
    Public Hex_MAP_BIN2 = {
       &H0,
       &H1,
       &H2,
       &H3,
       &H4,
       &H5,
       &H6,
       &H7,
       &H8,
       &H9,
       &HA,
       &HB,
       &HC,
       &HD,
       &HE,
       &HF
  }

    Public Function BinaryToDecimal(Binary As String) As Long
        Dim n As Long
        Dim s As Integer
        For s = 1 To Len(Binary)
            n = n + (Mid(Binary, Len(Binary) - s + 1, 1) * (2 ^(s - 1)))
        Next s
        BinaryToDecimal = n
    End Function

    Public Function DecimalToBinary(DecNum As Long) As String
        Dim tmp As String
        tmp = Trim(Str(DecNum Mod 2))
        DecNum = DecNum \ 2
        Do While DecNum <> 0
            tmp = Trim(Str(DecNum Mod 2)) & tmp
            DecNum = DecNum \ 2
        Loop
        DecimalToBinary = tmp
    End Function

    Public Function HextoBinary(HexVal As String) As String
        Dim binVal As String
        If Len(HexVal) = 1 Then
            binVal = Hex_MAP_BIN2(HexVal)
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

    Public Function HexToDecimal(HexVal As String) As Long
        Dim binVal As String
        Dim decVal As Long
        binVal = HextoBinary(HexVal)
        If binVal = "01011110101101010010" Then MsgBox("OK")
        decVal = BinaryToDecimal(binVal)
        HexToDecimal = decVal
    End Function

End Module
