Public Class InvalidUsernamePasswordException
    Inherits Exception
    Sub New()
        MyBase.New("用户名或密码错误")
    End Sub

    Sub New(message As String)
        MyBase.New(message)
    End Sub

    Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class
