Public Class NotAuthenticatedException
    Inherits Exception
    Sub New()
        MyBase.New("NenuClient尚未登陆成功")
    End Sub

    Sub New(message As String)
        MyBase.New(message)
    End Sub

    Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class
