Public Class InvalidNetworkException
    Inherits Exception
    Sub New()
        MyBase.New("确保你的计算机连接的是iNENU-2G或iNENU-5G等基于校园网的网络")
    End Sub

    Sub New(message As String)
        MyBase.New(message)
    End Sub

    Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class
