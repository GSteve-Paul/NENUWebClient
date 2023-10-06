Public Class GradeRecord
    Implements IComparable
    Private __totalScore As String
    Private __usualScore As List(Of String)

    Property TotalScore As String
        Get
            Return __totalScore
        End Get
        Set(value As String)
            __totalScore = value
        End Set
    End Property

    Property UsualScore As List(Of String)
        Get
            Return __usualScore
        End Get
        Set(value As List(Of String))
            __usualScore = value
        End Set
    End Property

    Sub New(_totalScore As String)
        __totalScore = _totalScore
    End Sub

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        Dim gr2 As GradeRecord = obj
        Dim res1, res2 As Integer
        If Not Integer.TryParse(Me.TotalScore, res1) Then
            res1 = 0
        End If
        If Not Integer.TryParse(gr2.TotalScore, res2) Then
            res2 = 0
        End If
        If res1 < res2 Then
            Return -1
        ElseIf res1 > res2 Then
            Return 1
        Else
            Return 0
        End If
    End Function
End Class
