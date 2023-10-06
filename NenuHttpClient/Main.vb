Imports System.Resources

Friend Class Main
    Shared Sub Main()
        Dim client As New NenuClient("2022013519", "Ljn58243373")
        client.Login()
        Dim gradesUrl As String = "http://dsjx.nenu.edu.cn/xszqcjglAction.do?method=queryxscj"
        Dim tmpResponse As String = client.GetAsync(gradesUrl).Result.Content.ReadAsStringAsync.Result
        Console.WriteLine(tmpResponse)
        Console.ReadKey()
    End Sub
End Class
