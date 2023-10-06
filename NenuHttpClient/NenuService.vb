Public MustInherit Class NenuService(Of T)
    Protected __Client As NenuClient
    Protected __ServiceValue As T

    ReadOnly Property ServiceValue As T
        Get
            Return __ServiceValue
        End Get
    End Property

    Public Sub New(NenuClient As NenuClient)
        If NenuClient.IsLogin Then
            __Client = NenuClient
        Else
            Throw New NotAuthenticatedException
        End If
    End Sub

    Public MustOverride Function DoService(obj As Object) As T
End Class
