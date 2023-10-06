Imports NenuHttpClient
Imports System.Data

Friend Class Main
    Shared Sub Main()
        Dim client As NenuClient = New NenuClient("2022013519", "Ljn58243373")
        client.Login()
        Dim process1 As NenuExamGrade = New NenuExamGrade(client)
        Dim dt As DataTable = process1.DoService(Nothing)
    End Sub
End Class
