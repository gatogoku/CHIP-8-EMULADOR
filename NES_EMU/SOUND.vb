Public Class SOUND



    Public Function PlaySound()
        Const SAMPLES = 44100
        Const SAMPLE_RATE = 44100
        Const AMPLITUDE = 30000


        Dim raw As New List(Of Byte)


        Const TWO_PI = 6.28318
        Const increment = 440 / SAMPLE_RATE
        Dim x = 0
        For i As Integer = 0 To 440 - 1
            raw.Add(Nothing)
            raw(i) = AMPLITUDE * Math.Sin(x * TWO_PI)
            x += increment

        Next



        Dim Buffer As New SFML.Audio.SoundBuffer(raw.ToArray)

        'If Not Buffer.(raw, SAMPLES, 1, SAMPLE_RATE) Then
        '    Exit For
        'End If

        Dim Sound As New SFML.Audio.Sound
        Sound.SoundBuffer = (Buffer)
        Sound.Loop = (True)
        Sound.Play()

        Sound.Attenuation = 100

        'While (1)
        '    Sound.Loop = True
        '    '  Sound.PlayingOffset = 100

        '    Return 0
        'End While

    End Function


End Class
